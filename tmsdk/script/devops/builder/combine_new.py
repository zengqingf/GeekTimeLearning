# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com

from comlib import TMApkManager,TMIPAManager,SVNManager,workspace,HTTPManager,Path,ZipManager,Version,loadversion,loadversion_str,XMLFile
from comlib import RecordStruct,RecordTool

from comlib.conf.loader import Loader
from comlib.conf.ref import *
from comlib.comobj import *

from builder.build import AndroidProject,LBAndroidLinker,ChannelAndroidLinker,BuildState,getState,setState,PackageNameStruct



import textwrap,shutil,copy

from typing import Tuple,List

downloadpath:List[Tuple[str,str]] = []

rt = RecordTool(os.path.join(G_projconfDir,'record.json'))




class BasePackCombiner():
    def __init__(self) -> None:
        super().__init__()
        self.platform = sys.argv[1]
        self.branchDesc = sys.argv[2]
        self.channels_str = sys.argv[3].lower()
        self.codeFlag = sys.argv[4]
        self.resFlag = com.str2Noneable(sys.argv[5])
        self.notCopyAB = com.str2boolean(sys.argv[6])

        self.resFlagStruct = PackageNameStruct(self.resFlag)
        self.codeFlagStruct = PackageNameStruct(self.codeFlag)
        # .codev 可能不存在
        # self.codeVersion = self.codeFlagStruct.codev

        self.branchurl = Loader.getbranchdesc_chinese2url(self.branchDesc)
        # 当前分支sdk目录url
        # self.sdkrooturl = Loader.获取指定分支的sdk工程scm地址(self.branchurl)
        self.sdkrooturl = 'svn://192.168.2.177/sdk/project_sdk/android/SDK'
        self.sdkroot = os.path.join(workdir,'SDK')
        self.scmpath_devopsconf:scmpath_devops = Loader.load(scmpath_devops)
        self.keystoreurl = self.scmpath_devopsconf.keystore['url']
        self.keystorepath = os.path.join(workdir,'keystore')
        # 小写
        self.channels = self.channels_str.split(',')

        self.sdkProjectPath = os.path.join(self.sdkroot,'SDKProject')
        self.exportProject = os.path.join(self.sdkroot,'AndroidSDKBuild','Program','ExportProejct','Client')
        self.channelExtPath = os.path.join(self.sdkroot,'ChannelExt')
    @workspace
    def combine(self):
        Path.ensure_svn_pathexsits(self.sdkroot,self.sdkrooturl)
        Path.ensure_svn_pathexsits(self.keystorepath,self.keystoreurl)
        # 切换状态REPACKCOPY
        setState(BuildState.REPACKCOPY)
        if self.platform == 'android':
            self.combine_android()
        elif self.platform == 'ios':
            self.combine_ios()
        else:
            raise Exception(f'{self.platform}不支持')

        setState(BuildState.END)
        
        downloadurls = ''
        for url,channel in downloadpath:
            downloadurls += f'<a href="{url}">{channel}</a>\n'
        htmlcontent = textwrap.dedent(f'''
        <!DOCTYPE html>
        <html>
            <head>
                <title>构建产物下载</title>
            </head>
            <body>
                {downloadurls}
            </body>
        </html>
        ''')

        com.savedata(htmlcontent,os.path.join(workdir,'download.html'))
    def combine_android():
        pass
    def combine_ios():
        pass

    def download_ab_code(self,branchDesc):
        '''
        return abrespath,abnamestruct,coderespath,codenamestruct
        '''
        ablist = G_ftp.listdir(com.get_ftp_tempsavepath(f'ResCombine/{branchDesc}/{self.platform}/ab'))
        codelist = G_ftp.listdir(com.get_ftp_tempsavepath(f'ResCombine/{branchDesc}/{self.platform}/code'))
        
        ext = os.path.splitext(codelist[0])[1]

        abnamelist = list(map(lambda x: PackageNameStruct(x.replace('.zip','')),ablist))
        codenamelist = list(map(lambda x: PackageNameStruct(os.path.splitext(x)[0]),codelist))
        
        abnamelist.sort(key=lambda x: int(x.resv),reverse=True)
        codenamelist.sort(key=lambda x: int(x.codev),reverse=True)

        codename = None
        # 第一遍搜索完美匹配
        for codenamestruct in codenamelist:
            if hasattr(self.codeFlagStruct,'codev'):
                ismatch = self.codeFlagStruct.isSameName(codenamestruct)
            else:
                ismatch = self.codeFlagStruct.isSameName(codenamestruct,['codev'])
            if ismatch:
                codename = codenamestruct.__str__()
                break
        # 第二遍搜索模糊匹配
        if codename == None:
            for codenamestruct in codenamelist:
                if self.codeFlagStruct.isSubName(codenamestruct):
                    codename = codenamestruct.__str__()
                    break
        if codename == None:
            raise Exception(f'代码包标志未匹配到{codename}')
        com.logout(f'[eqcodename] {codename}')
        self.codeVersion = codenamestruct.codev


        abname = None
        reqversion = self.codeVersion
        if self.resFlagStruct.resv != False:
            reqversion = self.resFlagStruct.resv
        for abnamestruct in abnamelist:
            abversion = int(abnamestruct.resv)
            if abversion <= int(reqversion) and self.resFlagStruct.isSubName(abnamestruct,['resv']):
                abname = abnamestruct.__str__()
                break
        if abname == None and not self.notCopyAB:
            raise Exception('资源包标志未匹配到')
        com.logout(f'[eqabname] {abname}')

        abftppath = com.get_ftp_tempsavepath(f'ResCombine/{branchDesc}/{self.platform}/ab/{abname}.zip')
        codeftppath = com.get_ftp_tempsavepath(f'ResCombine/{branchDesc}/{self.platform}/code/{codename}{ext}')



        abrespath = f'{abname}.zip'
        coderespath = f'{codename}{ext}'
        if not self.notCopyAB:
            G_ftp.download(abrespath,abftppath)
        G_ftp.download(coderespath,codeftppath)

        return abrespath,abnamestruct,coderespath,codenamestruct
    
    def channelbuild_android(self,project:AndroidProject,channel,codenamestruct:PackageNameStruct,abnamestruct:PackageNameStruct):
        '''
        原生操作，与引擎无关
        '''
        cconf:channelconf = Loader.load(channelconf,channel)
        fullversion = loadversion(self.codeVersion,rt.getserverversion(self.platform))

        cl = ChannelAndroidLinker(self.sdkProjectPath,self.channelExtPath,cconf.usesdk)
        cl.link_nativeProj(project)
        # lbl = LBAndroidLinker(self.sdkProjectPath,self.channelExtPath,'2241')
        # lbl.link_nativeProj(unityExportProjectm)

        # 配置sdk配置
        project.configAndroidProject(channel,self.codeVersion)
        # 配置本地工具路径
        project.configAndroidLocalTool()

        # combinename = self.getCombineConfStr(com.timemark,channel,'2241')
        combinenamestruct = self.getCombinePackageNameStruct(com.timemark,channel,None)
        # 最后发出来的名字
        finallynamestruct = abnamestruct.combine(codenamestruct).combine(combinenamestruct)
        finallynamestruct.codev = fullversion.__str__()

        # 构建
        project.gradleRelease()
        ftpdirname = com.get_ftp_savepath('android',finallynamestruct.mode,channel)
        ftppath = f'{ftpdirname}/{finallynamestruct.toString()}.apk'
        localfilepath = project.getReleaseApkPath()
        G_ftp.upload(localfilepath,ftppath)
        
        httppath = G_ftp.ftppath2httppath(ftppath)
        downloadpath.append((httppath,channel))
        try:
            self.Ding(localfilepath,httppath)
        except Exception as e:
            import traceback
            traceback.print_exc(e)
            
        
        cl.clean()
        # lbl.clean()
        SVNManager.revert(self.exportProject)
    

class UnityPackCombiner(BasePackCombiner):

    def combine_ios(self):
        # 1.ftp下载xcarchive包 和 ab包
        # 2.解压ab放在xxx.xcarchive/Products/Applications/xxx.app/Data/Raw/AssetBundles路径
        # 3.配置ExportOption.plist文件
        # 4.导出ipa

        # 下载ab code
        abpath,abnamestruct,codepath,codenamestruct = self.download_ab_code(self.branchDesc)

        abm = ZipManager(abpath)
        xcarchivem = ZipManager(codepath)

        xcarchivem.unzip()

        xcarchivepath = os.path.join(xcarchivem.unzippath,'code.xcarchive')
        appdir = os.path.join(xcarchivepath,'Products/Applications')
        appname = com.findone(appdir)
        approot = os.path.join(appdir,appname)
        xcarchive_plist = os.path.join(approot,'Info.plist')
        xcarchive_abpath = os.path.join(approot,'Data/Raw/AssetBundles')
        Path.ensure_dirnewest(xcarchive_abpath)
        abm.unzip(xcarchive_abpath)
        

        ipa_output = os.path.join(workdir,'ipa_output')
        Path.ensure_pathnotexsits(ipa_output)

        channel = self.channels[0]
        self.cconf:ioschannelconf = Loader.load(ioschannelconf,channel)
        self.fullversion = loadversion(self.codeVersion,rt.getserverversion(self.platform))
        


        from comlib import PKCS12

        keydir = os.path.join(self.keystorepath,'ios',self.cconf.keystorename)
        passfile = os.path.join(keydir,'pwd.pass')
        mpfile = os.path.join(keydir,f'{self.cconf.keystorename}.mobileprovision')
        p12file = os.path.join(keydir,f'{self.cconf.keystorename}.p12')
        p12m = PKCS12(p12file,passfile)
        p12m.load()

        p12m.import_mac(mpfile)
        mpfileUUID = p12m.getUUID_mpfile(mpfile)
        p12fingerprint = p12m.getfingerprint_sha1()
        p12teamID = p12m.subject_companyid
        p12teamName = p12m.subject_username
        exportType='development'
        if "iPhone Distribution" in p12teamName:
            exportType = 'app-store'
        
        def setPlistValue(filepath,key,val):
            com.logout(f'[setPlist] {key}={val}')
            com.cmd(f'/usr/libexec/PlistBuddy -c "Set :{key} {val}" "{filepath}"',errException=StopException("plist设值失败",locals()))

        setPlistValue(xcarchive_plist,'CFBundleIdentifier',self.cconf.applicationid)
        setPlistValue(xcarchive_plist,'CFBundleDisplayName',self.cconf.applicationname)
        setPlistValue(xcarchive_plist,'CFBundleShortVersionString',rt.getversioncode(self.platform,channel))
        setPlistValue(xcarchive_plist,'CFBundleVersion',self.fullversion.__str__())

        # 全屏
        setPlistValue(xcarchive_plist,'UIRequiresFullScreen','true')
        # 文本结构转二进制
        def convert2bplist(plistpath):
            com.cmd(f'plutil -convert binary1 {plistpath}',errException=StopException("plistz转bplist失败",locals()))
        convert2bplist(xcarchive_plist)

        optfile = self.createExportOption(self.cconf.applicationid,mpfileUUID,p12fingerprint,p12teamID,exportType=exportType)
        # xcodebuild -exportArchive -archivePath /Users/tengmu/Desktop/ceshi/test/build/Tyrion.xcarchive -exportPath /Users/tengmu/Desktop/ceshi/test/build/Tyrion.ipa -exportOptionsPlist ExportOption_exp.plist
        com.cmd(f'xcodebuild -exportArchive -archivePath {xcarchivepath} \
        -exportPath {ipa_output} -exportOptionsPlist {optfile}',errException=Exception('导出ipa失败'))


        ipapath = com.findone(ipa_output,lambda x: x.endswith('.ipa'))
        # 最后发出来的名字
        finallynamestruct = abnamestruct.combine(codenamestruct)
        finallynamestruct.codev = self.fullversion.__str__()
        finallynamestruct.addkvflag('vc',rt.getversioncode(self.platform,channel)).addkvflag('time',com.timemark)


        ftpdirname = com.get_ftp_savepath('ios',finallynamestruct.mode,channel)
        ftppath = f'{ftpdirname}/{finallynamestruct.toString()}.ipa'
        G_ftp.upload(ipapath,ftppath)
        httppath = G_ftp.ftppath2httppath(ftppath)
        downloadpath.append((httppath,channel))

        self.Ding(ipapath,httppath)
        pass
    def createExportOption(self,bundleid,mpfileUUID,p12fingerprint,p12teamID,exportType='development',enableBitcode=False):
        '''
        exportType = app-store，ad-hoc，enterprise和development \n
        
        '''
        optfile = os.path.join(thisdir,"ExportOption.plist")

        data = com.readall(optfile)
        data = data.replace('${bundleid}',bundleid)\
            .replace('${mpfileUUID}',mpfileUUID)\
            .replace('${p12fingerprint}',p12fingerprint)\
            .replace('${p12teamID}',p12teamID)\
            .replace('${exportType}',exportType)\
            .replace('${enableBitcode}',enableBitcode.__str__().lower())
        com.savedata(data,optfile)
        return optfile
    def combine_android(self):
        # 下载ab code
        abpath,abnamestruct,codepath,codenamestruct = self.download_ab_code(self.branchDesc)
        # 清理要进行组包的安卓工程
        SVNManager.clean_up_with_unversion(self.exportProject)
        unityExportProjectm = AndroidProject(self.exportProject)
        abm = ZipManager(abpath)
        codem = TMApkManager(codepath)
        
        codem.unzip()
        mainlibs = codem.getpath_lib()
        mainassets = codem.getpath_assets()
        mainres = codem.getpath_res()
        mainmipmapfolders = codem.getpaths_mipmap()
        mainlaunchimg = codem.getpath_launchimg()
        mainbannerimg = codem.getpath_bannerimg()

        com.combinefolder(unityExportProjectm.soSavePath,mainlibs,method='move',debug=False)
        com.combinefolder(unityExportProjectm.assetsPath,mainassets,method='move',cover=False,debug=False)
        # 合入图标
        for mainmipmapfolder in mainmipmapfolders:
            projectmipmapfolder = mainmipmapfolder.replace(mainres,unityExportProjectm.resPath,1)
            Path.ensure_direxsits(projectmipmapfolder)
            com.combinefolder(projectmipmapfolder,mainmipmapfolder,method='move')
        # 合入launch图
        if os.path.exists(mainlaunchimg):
            projectlaunchimg = mainlaunchimg.replace(mainres,unityExportProjectm.resPath,1)
            Path.ensure_direxsits(os.path.join(unityExportProjectm.resPath,'drawable'))
            shutil.copyfile(mainlaunchimg,projectlaunchimg)
        # 合入banner图
        if os.path.exists(mainbannerimg):
            projectbannerimg = mainbannerimg.replace(mainres,unityExportProjectm.resPath,1)
            Path.ensure_direxsits(os.path.join(unityExportProjectm.resPath,'drawable-xhdpi'))
            shutil.copyfile(mainbannerimg,projectbannerimg)
        
        # 确保ab目录不存在
        Path.ensure_pathnotexsits(unityExportProjectm.AssetBundlesPath)
        # 解压ab资源到安卓工程
        if not self.notCopyAB:
            abm.unzip(unityExportProjectm.AssetBundlesPath)

        
        for channel in self.channels:
            self.channelbuild_android(unityExportProjectm,channel,codenamestruct,abnamestruct)

        setState(BuildState.END)

    
    def Ding(self,filepath,httppath):
        filename = os.path.basename(httppath)
        if filepath.endswith('.apk'):
            apkm = TMApkManager(filepath)
        elif filepath.endswith('.ipa'):
            apkm = TMIPAManager(filepath)
        else:
            raise Exception(f'不支持格式{filepath}')
        md5 = com.get_md5(filepath)
        skilldatamd5,tablefbdatamd5,tabledatamd5 = apkm.get_abmd5()
        robot = Loader.获取出包机器人()

        title = filename
        content = robot.markdown_text_weightORitalic(title,'weight') + '\n\n'
        content += robot.markdown_drawline()

        md5 = 'MD5:' + robot.markdown_text_weightORitalic(md5,'weight')
        skilldatamd5 = 'Skill:' + robot.markdown_text_weightORitalic(skilldatamd5,'weight')
        tablefbdatamd5 = 'FBTab:' + robot.markdown_text_weightORitalic(tablefbdatamd5,'weight')

        content += robot.markdown_textlist(md5,skilldatamd5,tablefbdatamd5) + '\n\n'
        content += robot.markdown_drawline()
        content += robot.markdown_text_weightORitalic('这是新打包脚本出的包','weight')

        data = robot.build_actioncard_mult(title,content,hideAvatar=True,下载=httppath)
        com.logout(f'[http download] {httppath}')
        robot.send(data)

    def getCombinePackageNameStruct(self,time,channel,lebianversion):
        combinename = PackageNameStruct()
        combinename.addkvflag('time',time)
        combinename.addkvflag('channel',channel)
        if lebianversion != None:
            combinename.addkvflag('lebian',lebianversion)
        combinename.addkvflag('vc',rt.getversioncode(self.platform,channel))
        return combinename




class UE4PackCombiner(BasePackCombiner):
    '''
    UE4
    需要整合的目录：
    assets  java  jniLibs  res
    直接替换android manifest ，修改包名
    根据sdk配置修改gradle.properties
    修改local.properties的sdk ndk路径配置
    修改settings.gradle，引入外部库
    '''
    def __init__(self) -> None:
        super().__init__()
        # 临时开启notCopyAB

    def combine_android(self):
        # 下载ab code
        abpath,abnamestruct,codepath,codenamestruct = self.download_ab_code(self.branchDesc)
        # 清理要进行组包的安卓工程
        SVNManager.clean_up_with_unversion(self.exportProject)
        exportProjectm = AndroidProject(self.exportProject)
        abm = ZipManager(abpath)
        codem = ZipManager(codepath)

        # codem.unzip()
        # codeMainFolderPath = os.path.join(codem.unzippath,'main')
        # com.combinefolder(exportProjectm.mainPath,codeMainFolderPath,method='move',debug=False)
        # 目前是直接整个android工程
        codem.unzip()
        ap = AndroidProject(codem.unzippath)
        self.ue4AndroidProjectConfig()
    def ue4AndroidProjectConfig(self,ap:AndroidProject):
        # https://www.jianshu.com/p/c3aca7d3c076
        # aab（Android App Bundle）是google play的格式，国内不需要，暂时不写
        ap.configAndroidLocalTool()
        ap.setGradlePropValue('PACKAGE_NAME','')
        ap.setGradlePropValue('TARGET_SDK_VERSION','')
        ap.setGradlePropValue('STORE_VERSION','') # versioncode
        ap.setGradlePropValue('VERSION_DISPLAY_NAME','') # versionname

        ap.setGradlePropValue('OUTPUT_PATH','')
        ap.setGradlePropValue('OUTPUT_FILENAME','')
        ap.setGradlePropValue('OUTPUT_BUNDLEFILENAME','')
        ap.setGradlePropValue('OUTPUT_UNIVERSALFILENAME','')

        # ap.setGradlePropValue('OBB_FILECOUNT','')
        # ap.setGradlePropValue('OBB_FILE0','')

        ap.setGradlePropValue('BUNDLETOOL_JAR','')
        ap.setGradlePropValue('GENUNIVERSALAPK_JAR','')

        ap.setGradlePropValue('STORE_FILE','')
        ap.setGradlePropValue('STORE_PASSWORD','')
        ap.setGradlePropValue('KEY_ALIAS','')
        ap.setGradlePropValue('KEY_PASSWORD','')


        aarimportFilePath = os.path.join(ap.projectPath,'app','aar-imports.gradle')
        # 不使用磁盘maven
        com.re_replace('maven','',aarimportFilePath)
        # 使用内网maven
        # 这里在打包阶段就完成了



# BuildWithUBT
# E:/ProgramOthers/UE_4.25/UE_4.25/Engine/Binaries/DotNET/UnrealBuildTool.exe  -projectfiles -project="D:/UE4Project/ActionRPG/ActionRPG.uproject" -game -rocket -progress 构建vs工程的命令



# .\UE4Editor-Cmd.exe D:\_workspace\_project\ue4.22\RollingBoll\RollingBoll.uproject -run=Cook  -TargetPlatform=Android_ASTC -fileopenlog -unversioned -abslog=D:\_workspace\_project\ue4.22\RollingBoll\cook.txt -stdout -unattended -UTF8Output

# UnrealPak.exe okeys=D:\UE4WorkStation\TestPatch4\Saved\Cooked\WindowsNoEditor\TestPatch4\Metadata\Crypto.json -order=D:\UE4WorkStation\TestPatch4\Build\WindowsNoEditor\FileOpenOrder\CookerOpenOrder.log -patchpaddingalign=2048 -compressionformats=  -multiprocess -abslog="C:\Users\liuqixiang\AppData\Roaming\Unreal Engine\AutomationTool\Logs\D+UE+UE_4.23\UnrealPak-TestPatch4


# %EngineDir%/Engine/Binaries/Win64/UnrealPak.exe "%MyGame-arm64-es2%/assets/MyGame/Content/Paks/MyGame-Android_ASTC.pak" -Extract "%Output%/MyGame-Android_ASTC"

if __name__ == "__main__":
    # sys.argv = ['','1.5主干','224739','DNL15Dev']
    pc = UnityPackCombiner()
    pc.combine()
    # 下载ab code
    # 解包放到各自目录
    # 链接
    # 打包
    
