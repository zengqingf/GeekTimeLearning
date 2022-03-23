# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace,Version,Jenkins,Jenkins_2_249_2_Manager,BinManager
from comlib.conf.loader import Loader
from comlib.conf.ref import *

from comlib import TMUnityManager,TMApkManager,SVNManager,Path,DictUtil,ZipManager,RamMirror,JsonFile,DictUtil,UnityManager
from comlib import SecretManager,UE4Engine,RecordTool,SVNWorkflow

from comlib.comobj import *

import textwrap,argparse,shutil,re
from enum import Enum,unique

# 枚举
from comlib.ue4m import EGameStartMode,EJobType,ELogLevel


debugbot = Loader.获取脚本调试机器人()
releasebot = Loader.获取客户端维护机器人()
chubaobot = Loader.获取出包机器人()

# androidProjectPath = r'D:\_WorkSpace\_sdk\AndroidProject\SDKProject\AndroidProject'
# unityProjectPath = r'D:\_WorkSpace\_sdk\AndroidProject\UnityProject'
# unityPath = r'C:\Unity'
# workdir = os.getcwd()
# from typing import Literal




# 安卓直接接入工程，ios是通过xuport接入
@unique
class BuildState(Enum):
    START = 0
    PRECOPY = 1
    BUILDAB = 2
    BUILDCODE = 3
    REPACKCOPY = 4
    END = 100
@unique
class LightmassQuality(Enum):
    SKIP=0
    PREVIEW=1
    MEDIUM=2
    HIGH=3
    PRODUCTION=4
@unique
class UseEngine(Enum):
    Installed=0
    SourceInstalled=1
    Source=2
    


state = BuildState.START

def getState():
    return state
def setState(s:BuildState):
    global state
    state = s


class UnityProject(object):
    def __init__(self,unityPath,unityProjectPath,platform,args):
        super().__init__()
        self.unityPath = unityPath
        self.unityProjectPath = unityProjectPath
        self.unityAssetPath = os.path.join(self.unityProjectPath,'Assets')
        self.unityStreamingAssetsPath = os.path.join(self.unityAssetPath,'StreamingAssets')
        self.unityABSavePath = os.path.join(self.unityStreamingAssetsPath,'AssetBundles')
        self.pluginsPath = os.path.join(self.unityAssetPath,'Plugins')

        self.unityLibrary = os.path.join(self.unityProjectPath,'Library')

        self.unityVersion = Loader.getglobalconf().engine
        self.platform = platform

        # android路径
        self.androidPath = os.path.join(self.pluginsPath,'Android')
        self.androidAssetsPath = os.path.join(self.androidPath,'assets')
        self.binPath = os.path.join(self.androidPath,'bin')
        self.libsPath = os.path.join(self.androidPath,'libs')
        self.resPath = os.path.join(self.androidPath,'res')
        self.androidManifestPath = os.path.join(self.androidPath,'AndroidManifest.xml')
        self.gradleLibPath = os.path.join(self.androidPath,'libTemplate.gradle')
        self.gradleMainPath = os.path.join(self.androidPath,'mainTemplate.gradle')
        self.gradleSettingsPath = os.path.join(self.androidPath,'settingsTemplate.gradle')
        # ios路径

        self.unitym = UnityManager(self.unityPath,self.unityProjectPath)
    def member_snapshot(self):
        import copy
        self.send2CSharpDict = copy.deepcopy(self.__dict__)

class TMUnityProject_Code(UnityProject):
    def __init__(self, unityPath, unityProjectPath, platform, args):
        # globalsettings配置
        self.isUsingXYSDK = args.isUsingXYSDK
        self.isGuide = args.isGuide
        self.forceUseAutoFight = args.forceUseAutoFight
        self.enableHotFix = args.enableHotFix
        self.loadFromPackage = args.loadFromPackage
        self.hotFixUrlDebug = args.hotFixUrlDebug
        self.isRecordPVP = args.isRecordPVP
        self.isDebug = args.isDebug
        self.skillHasCooldown = args.skillHasCooldown


        self.member_snapshot()
        # Unity参数 (除宏外全小写)
        # 打ab不需要这些参数,暂时这样写
        self.isUnityDev = args.isUnityDev
        self.isConnectProfiler = args.isConnectProfiler
        self.isIL2CPP = args.isIL2CPP
        self.macro = args.macro

        super().__init__(unityPath, unityProjectPath, platform, args)

        self.unitym = TMUnityManager(self.unityPath,self.unityProjectPath)
    def buildCode(self,outpath):
        self.unitym.build(self.platform,self.isUnityDev,self.isIL2CPP,self.isConnectProfiler,self.macro,outpath,**self.send2CSharpDict)
    def buildRes(self,outpath):
        self.unitym.buildRes(self.platform,self.isUnityDev,self.isIL2CPP,self.isConnectProfiler,self.macro,outpath,**self.send2CSharpDict)

class TMUnityProject_Res(UnityProject):
    def __init__(self, unityPath, unityProjectPath, platform, args):
        # globalsettings配置

        # ab资源配置
        self.needPackScriptData = args.needPackScriptData

        self.member_snapshot()
        super().__init__(unityPath, unityProjectPath, platform, args)
        self.unitym = TMUnityManager(self.unityPath,self.unityProjectPath)

    def buildAB(self):
        # 本地工程
        self.unitym.set_BuildTarget(self.platform)
        
        self.unitym.buildAB(self.platform)
    
class Builder(object):
    def __init__(self,args):
        # 构建参数 (全小写)
        self.platform = args.platform
        self.mode = args.mode
        self.level = args.level
        self.svnUrl = args.svnUrl
        self.branchDesc = Loader.getbranchdesc_url2chinese(self.svnUrl)
        self.svnVersion = args.svnVersion
        if self.svnVersion == 'HEAD':
            self.svnVersion = SVNManager.version(self.svnUrl)

        self.useRamDisk = args.useRamDisk
        # self.packVersion = args.packVersion
        # 工具参数
        # self.isEncrpyt = args.isEncrpyt 加固不应该放这，这是流水线参数
        if isBuildCode():
            self.uwaConf = args.uwaConf



        # 构建路径
        self.CodeProjectRoot = os.path.join(workdir,'CodeProject')
        self.ProjectSDKPath = os.path.join(self.CodeProjectRoot,'SDK')
        self.SDKProjectRoot = os.path.join(self.ProjectSDKPath,'SDKProject')
        self.ChannelExtPath = os.path.join(self.ProjectSDKPath,'ChannelExt')
        self.CodeUnityProjectRoot = os.path.join(self.CodeProjectRoot,'Client')
        self.ABProjectRoot = os.path.join(workdir,'ABProject')
        
        
        self.ABUnityProjectRoot = os.path.join(self.ABProjectRoot,'Client')
        self.outdir = os.path.join(workdir,'out')
        # self.outpath = os.path.join(self.outdir,'test')


        # 确保路径存在
        codekeep = [['',['Client','SDK']],['Client',['Assets','Packages','ProjectSettings']],['Client/Assets/Resources',['Base','Base.meta']]]
        abkeep = [['',['Client']],['Client',['Assets','Packages','ProjectSettings']]]
        env:envconf = Loader.getenvconf()

        if self.level == 3:
            self.rm = RamMirror(self.ABProjectRoot)
        elif self.level == 4:
            self.rm = RamMirror(self.CodeProjectRoot)
        else:
            raise Exception(f'未知level {self.level}')
        self.preclean()
        # 内存镜像
        # self.rm = RamMirror(self.abUnityProject.unityProjectPath)

        # self.ABProjectRoot_rammirror = self.rm.getMirrorPath(self.ABProjectRoot)
        # self.ABProjectRoot_bak = self.rm.getBakPath(self.ABProjectRoot)

        if self.level == 3:
            self.abUnityProject = TMUnityProject_Res(env.enginepath,self.ABUnityProjectRoot,self.platform,args)

            # 创建空文件夹
            # 创建镜像
            # 镜像为空svn checkout，不为空update
            # checkout完成后打包
            # 备份另写一个定时脚本，不应该放这里
            Path.ensure_direxsits(self.ABProjectRoot)
            if self.useRamDisk:
                self.rm.mirror_simple()
            curroot = self.ABProjectRoot
            if os.path.islink(curroot):
                curroot = os.readlink(curroot)
            Path.ensure_svn_pathexsits(curroot,self.svnUrl,checkout_keep=abkeep,version=self.svnVersion)
            SVNManager.clean_up_with_unversion(self.abUnityProject.unityAssetPath)
            
        if self.level == 4:
            self.codeUnityProject = TMUnityProject_Code(env.enginepath,self.CodeUnityProjectRoot,self.platform,args)

            Path.ensure_direxsits(self.CodeProjectRoot)
            if self.useRamDisk:
                self.rm.mirror_simple()
            curroot = self.CodeProjectRoot
            if os.path.islink(curroot):
                curroot = os.readlink(curroot)
            
            if not self.codeUnityProject.loadFromPackage:
                Path.ensure_svn_pathexsits(curroot,self.svnUrl,checkout_keep=None,version=self.svnVersion)
            else:
                Path.ensure_svn_pathexsits(curroot,self.svnUrl,checkout_keep=codekeep,version=self.svnVersion)
            SVNManager.clean_up_with_unversion(self.codeUnityProject.unityAssetPath)

        setState(BuildState.PRECOPY)
    def preclean(self):
        # 清理输出包
        Path.ensure_dirnewest(self.outdir)
        # 清理链接
        self.rm.rollback()

    def copyAB(self):
        extfilter = ('.manifest','.meta')
        com.combinefolder(self.codeUnityProject.unityStreamingAssetsPath,self.abUnityProject.unityStreamingAssetsPath,
        method='move',fileextfilter=extfilter)


    def getABConfStr(self):
        nameStruct = PackageNameStruct()
        nameStruct.addkvflag('resv',self.svnVersion)

        return nameStruct.__str__()

    def getCodeConfStr(self):
        nameStruct = PackageNameStruct()
        nameStruct.addkvflag('codev',self.svnVersion)
        nameStruct.addkvflag('mode',self.mode)
        
        if self.codeUnityProject.isIL2CPP:
            nameStruct.addboolflag('il2cpp')
        if self.codeUnityProject.isUnityDev:
            nameStruct.addboolflag('unitydev')
        if self.codeUnityProject.loadFromPackage:
            nameStruct.addboolflag('res')
        if self.uwaConf != None:
            nameStruct.addboolflag('uwa')
        
        return nameStruct.__str__()

    def unionbuild_ABSide(self):
        # self.rm.mirror()
        setState(BuildState.BUILDAB)
        self.abUnityProject.buildAB()
        
        name = self.getABConfStr()
        abdir = self.abUnityProject.unityABSavePath
        zipname = f'{name}.zip'
        abm = ZipManager(zipname)
        abpt = abm.open('w')
        for abname in filter(lambda x: not x.endswith(('.meta','.manifest')),os.listdir(abdir)):
            abpath = os.path.join(abdir,abname)
            abm.addfile(abpt,abpath,abname,ZipManager.ZIP_DEFLATED)
        abftppath = com.get_ftp_tempsavepath(f'ResCombine/{self.branchDesc}/{self.platform}/ab/{zipname}')
        
        abm.close()
        G_ftp.upload(abm.zippath,abftppath,overwrite=True)

        setState(BuildState.END)
        
        # 不能执行svn操作,因为是跨文件系统
        # SVNManager.clean_up_with_unversion(self.abUnityProject.unityAssetPath)
        self.rm.rollback()
    
    def unionbuild_CodeSide(self):
        self.preConf()

        setState(BuildState.BUILDCODE)
        name = self.getCodeConfStr()
        outpath = os.path.join(self.outdir,name)

        self.unionbuild_CodeSide_internal(name,outpath)

        setState(BuildState.END)
        self.clean()
    def unionbuild_CodeSide_internal(self,name,outpath):
        pass
    def preConf(self):
        pass
    def clean(self):
        pass
    @workspace
    def build(self):
        if self.level == 3:
            self.unionbuild_ABSide()
        if self.level == 4:
            self.unionbuild_CodeSide()

    def getftppath(self,filename,code_or_ab):
        # return com.get_ftp_tempsavepath(f'ResCombine/{Loader.getgame()}_{self.branchDesc}/{self.platform}/{code_or_ab}/{filename}')
        return f'{com.get_ftp_savepath(self.platform,self.branchDesc,self.channel)}/{filename}'
        # return com.get_ftp_tempsavepath(f'ResCombine/a8_dev/{self.platform}/{code_or_ab}/{filename}')

class AndroidBuilder(Builder):
    def __init__(self,args):
        super().__init__(args)


    def unionbuild_CodeSide_internal(self,name,outpath):
        if self.codeUnityProject.loadFromPackage:
            self.codeUnityProject.buildCode(outpath)
        else:
            self.codeUnityProject.buildRes(outpath)
        
        apkpath = outpath + '.apk'

        apkname = f'{name}.apk'
        assert os.path.exists(apkpath)
        if self.codeUnityProject.loadFromPackage:
            codeftppath = self.getftppath(apkname,'code')
        else:
            codeftppath = self.getftppath(apkname,'res')

        G_ftp.upload(apkpath,codeftppath,overwrite=True)

    def preConf(self):
        if self.uwaConf != None:
            self.uwaL = UWAAndroidLinker(self.SDKProjectRoot,self.ChannelExtPath)
            self.uwaL.link_unityProj(self.codeUnityProject,self.uwaConf)
    def clean(self):
        if hasattr(self,'uwaL'):
            self.uwaL.clean()

class IOSBuilder(Builder):
    def __init__(self,args):
        super().__init__(args)

        self.iosChannel = args.iosChannel
        self.iosPath = os.path.join(self.codeUnityProject.pluginsPath,'iOS')
        # ab打包和android一样

        # code打包流程：
        # unity build 导出xcode工程
        # xcode build 打未签名code包
        # combine 组ab和code包，然后手动签名
    def unionbuild_CodeSide_internal(self,name,outpath):
        # 拷贝sdk
        # link用xuport，暂时不手写了
        linker = IOSSDKProjectLinker(self.ChannelExtPath,self.iosChannel)
        linker.link_unityProj(self.codeUnityProject)
        # ios需要先添加渠道名
        name = PackageNameStruct(name).addkvflag('channel',self.iosChannel.lower()).toString()
        # unity build 导出xcode工程
        self.codeUnityProject.buildCode(outpath)

        iosproj = IOSProject(outpath)
        iosproj.build(mode=self.mode)
        xcarchivepath = iosproj.getPackagePath()

        zipname = f'{name}.zip'
        ZipManager.ziptarget([xcarchivepath],zipname)

        G_ftp.upload(zipname,self.getftppath(zipname,'code'),overwrite=True)


        pass

from comlib import UE4Project


class UE4Builder(object):
    def __init__(self,args) -> None:
        super().__init__()

        self.jenkins = Jenkins(Jenkins_2_249_2_Manager())

        self.platform:str = args.platform
        self.mode:str = args.mode
        self.cookFlavor:str = args.cookFlavor
        self.lightmass_quality = com.str2enum(LightmassQuality,args.lightmass_quality)
        self.level:int = args.level
        # self.svnUrl = args.svnUrl
        
        self.bPackageDataInsideApk = args.bPackageDataInsideApk
        self.distribution = args.distribution

        self.userBulidPhoneNum = args.userBulidPhoneNum
        self.userBuildComment = args.userBuildComment
        self.macro:list = args.macro

        self.isA8Project = args.isA8Project
        self.forceSvnUrl = args.forceSvnUrl

        self.isMakePSOCache = args.isMakePSOCache


        # **游戏配置开始**
        self.channel='debug'

        self.startMode = com.str2enum(EGameStartMode,args.startMode)
        self.jobType = com.str2enum(EJobType,args.jobType)
        self.battleRecord:bool = args.battleRecord
        self.logLevel = com.str2enum(ELogLevel,args.logLevel)
        self.skillNoCD = args.skillNoCD
        self.useBattlePreload = args.useBattlePreload
        self.internalTest = args.internalTest
        
        
        # **游戏配置结束**

        # **功能配置开始**
        self.useDolphinResUpdate = False
        # **功能配置结束**
        self.game:str = Loader.getgame()
        branchName = self.game
        branchdescconf = Loader.load(projectconf,use_defalt=True)
        enginec = Loader.load(engineconf,use_defalt=True)
        self.branchDesc = args.branchDesc
        self.branchExtName = args.branchExtName

        if com.strCompare(self.game,'a8',toLow=True):
            branchName = 'trunk'# branchName正规应该是A8的，这里是遗留问题

        extName = SVNWorkflow.buildExtBranchName(self.game,self.platform,extName=self.branchExtName)
        self.projectWorkflow = SVNWorkflow(branchdescconf.reporoot,branchName,extName)
        self.engineWorkflow = SVNWorkflow(enginec.binRepoUrl,enginec.branchName,extName)

        self.engineUrl = self.engineWorkflow.trunkUrl
        self.projectUrl = self.projectWorkflow.trunkUrl
        # 这个分支名是没加描述头的
        self.engineBranchName = self.engineWorkflow.branchName
        self.是发出去的包 = False
        # TODO online 分支
        if Loader.isReleaseBranchDesc(self.branchDesc):
            self.是发出去的包 = True
            # 发布分支不允许使用内部测试模式
            self.internalTest = False
    
            self.channel = 'release'
            self.engineUrl = self.engineWorkflow.releaseBranchUrl
            self.projectUrl = self.projectWorkflow.releaseBranchUrl
            # 这个是加描述头的
            self.engineBranchName = self.engineWorkflow.finalBranchName


        self.svnUrl = com.strjoin('/',self.projectUrl,Loader.获取相对客户端工程路径(sep="/"))
        if not com.isNoneOrEmpty(self.forceSvnUrl):
            self.svnUrl = self.forceSvnUrl
        

        self.svnVersion = args.svnVersion
        self.useEngine = com.str2enum(UseEngine,args.useEngine)
        # self.engineVersion = '4.25'

        if self.svnVersion == 'HEAD':
            self.svnVersion = SVNManager.version_top(self.svnUrl)

        
        self.recordTool = RecordTool()
        self.versionCode = self.recordTool.getversioncode(self.platform,self.channel)
        self.versionName = Version(self.svnVersion,self.recordTool.getserverversion(self.platform),self.versionCode)


        self.programRoot = os.path.join(workdir,self.branchDesc)
        self.projectRoot = os.path.join(self.programRoot,Loader.获取相对客户端游戏工程路径().split(os.path.sep,1)[1])
        self.env:envconf = Loader.getenvconf()
        if not com.isNoneOrEmpty(self.forceSvnUrl):
            self.programRoot = os.path.join(workdir,os.path.basename(self.forceSvnUrl))
            files = SVNManager.list(self.forceSvnUrl,recursive=True)
            uprojectFileRelPath = com.findone_list(files,lambda x: x.endswith('.uproject'))
            self.projectRoot = os.path.join(self.programRoot,os.path.dirname(uprojectFileRelPath))

        # -----------------------------资源相关开始
        # 拉取游戏工程
        Path.ensure_svn_pathexsits(self.programRoot,self.svnUrl,version=self.svnVersion)

        # 拉取引擎
        if self.useEngine == UseEngine.SourceInstalled:
            engineSvnUrl = UE4Engine.getEngineSVNPath(self.engineUrl,installed=True)
            engineWCDir = UE4Engine.getEngineWCPath(self.engineBranchName,True)
            Path.ensure_svn_pathexsits(engineWCDir,engineSvnUrl)
            
        elif self.useEngine == UseEngine.Source:
            # TODO git上的源码引擎引用
            raise Exception('源码引擎不支持')
        else:
            engineWCDir = self.env.enginepath

        if not os.path.exists(engineWCDir) or com.isNoneOrEmpty(engineWCDir):
            raise Exception(f'引擎路径不存在:{engineWCDir}')
        
        
        self.project = UE4Project(engineWCDir,self.projectRoot)
        
        
        SVNManager.clean_up_with_unversion(self.project.contentPath)
        # -----------------------------资源相关结束

        # 注册引擎
        self.project.registEngine()
        
        self.logSummary()

        setState(BuildState.PRECOPY)
    def logSummary(self):
        log = com.textwrap.dedent(f'''
        **********************************************************
        *  game={self.game}
        *  versionName={self.versionName}
        *  versionCode={self.versionCode}
        *  platfrom={self.platform}
        *  channel={self.channel}
        *  useEngine={self.useEngine}
        *  engineVersion={self.project.engine.engineVersion}
        *  mode={self.mode}
        *  dis={self.distribution}
        *  macro={self.macro}
        **********************************************************
        ''')
        com.logout(log)
    def getCodeConfStr(self,translateEnum=False):
        nameStruct = PackageNameStruct()
        
        nameStruct.addkvflag('time',G_timemark)
        nameStruct.addkvflag('branch',self.branchDesc)
        nameStruct.addkvflag('codev',self.versionName.__str__())
        nameStruct.addkvflag('resv',self.svnVersion)
        nameStruct.addkvflag('mode',self.mode)
        nameStruct.addboolflag('dis',self.distribution)
        nameStruct.addkvflag('lightmass',self.lightmass_quality)
        nameStruct.addkvflag('engine',self.useEngine)

        if self.isA8Project:
            # 暂时加这里，以后肯定要删掉
            nameStruct.addkvflag('startScene',self.startMode)
            nameStruct.addkvflag('job',self.jobType)
            # nameStruct.addkvflag('battleRecord',self.battleRecord.__str__())
            nameStruct.addkvflag('logLevel',self.logLevel)
            nameStruct.addkvflag('skillNoCD',self.skillNoCD)
            # nameStruct.addkvflag('useBattlePreload',self.useBattlePreload)
            nameStruct.addkvflag('internalTest',self.internalTest)

        # ---------------------
        return nameStruct.toString(unmarshal=self.是发出去的包,translateEnum=translateEnum)

    def GetPreMacros(self):
        '''
        处理预设宏
        '''
        preMarco = []
        if self.mode == "Shipping":
            preMarco.append('USE_GAME_UPDATE')
        return preMarco

    @workspace
    def build(self):
        self.prebuild()
        if self.level == 0:
            rebuildRes = True
        else:
            rebuildRes = False
        # self.project.engine.setSkipGradleBuildFlag()
        self.project.build_CodePakAndPack(self.platform,rebuildRes,self.mode,self.cookFlavor,isDis=self.distribution,useDolphinResUpdate=self.useDolphinResUpdate)

        self.afterbuild()

        
        setState(BuildState.END)
    def prebuild(self):
        # 清理生成目录
        Path.ensure_pathnotexsits(os.path.join(self.project.binariesPath,self.project.engine._fixPlatformStr(self.platform)))

        manifestpath = com.get_logfile_path('buildmanifest.xml')
        ubtlogpath = com.get_logfile_path('ubtLog.txt')
        self.project.clean(self.project.gameModule,self.platform,self.mode)
        # CommandLet执行需要完整的Editor文件
        out,code = self.project.buildGameModule(self.project.editorModule,self.project.engine.mechinePlatform,'Development',manifestpath,ubtlogpath,clean=True)
        if code != 0:
            raise Exception('打包Editor编译失败')
        # 修复重定向器
        # out,code,log = self.project.FixupRedirectors()
        # if code != 0:
        #     raise Exception(f'修复重定向器失败')
        # 光照烘培
        if not self.lightmass_quality == LightmassQuality.SKIP:
            # out,code,logpath = self.project.LightmassBake(self.lightmass_quality.name)
            out,code,logpath = self.project.LightmassBake()
            if code != 0:
                # G_ftp.upload(logpath,)
                raise Exception('光照烘培失败')

        # ！！！！这里需要A8工程的AutomationTool插件！！！！
        if self.isA8Project:
            # 目前是配置GlobalData的场景和职业
            self.project.TMConfigCommandLet(self.startMode,self.jobType,self.battleRecord,self.logLevel,self.skillNoCD,self.useBattlePreload,self.internalTest)
        

        #设置宏
        self.macro += self.GetPreMacros()
        self.project.SetMacrosPreBuild(self.macro)

        baseDoFile = {
            'defaultFile':{
                "type": "game",
                'data':[
                    {
                        "ConfigLineAction": "Set",
                        "section": "/Script/UnrealEd.ProjectPackagingSettings",
                        "key": "ForDistribution",
                        "value": f'{self.distribution}'
                    }
                ]
            }
        }
        baseDoFile2 = {
            'defaultFile':{
                "type": "engine",
                'data':[
                    {
                        "ConfigLineAction": "Set",
                        "section": "VersionSetting",
                        "key": "CodeVersion",
                        "value": self.versionName.__str__()
                    },
                    {
                        "ConfigLineAction": "Set",
                        "section": "VersionSetting",
                        "key": "ResVersion",
                        "value": self.versionName.__str__()
                    }
                ]
            }
        }
        self.project.configMode(self.platform,baseDoFile)
        self.project.configMode(self.platform,baseDoFile2)

    def afterbuild(self):

        pass
    def getftppath(self,filename):
        return f'{com.get_ftp_savepath(self.platform,self.branchDesc,self.channel)}/{filename}'

    
    def getExtraBuildContent(self):
        '''
        获取额外的出包信息
        '''
        content = f'出包地址:{chubaobot.markdown_textlink("http://192.168.5.10:5001/pkg/index/","http://192.168.5.10:5001/pkg/index/")}'
        if self.userBuildComment:
            content += f"\n\n打包备注:{self.userBuildComment}"
        if not self.isA8Project:
            content += f'\n\n这个不是A8工程的包！！！'
        if not com.isNoneOrEmpty(self.forceSvnUrl):
            content += f'\n\n这是特殊分支的包，分支路径:{self.forceSvnUrl}'

        return content
    
class UE4AndroidBuilder(UE4Builder):
    def __init__(self,args):
        super().__init__(args)
        self.channelconf = Loader.load(channelconf,self.channel)
        

    def prebuild(self):
        super().prebuild()
        # 先直接出包总结流程
        self.project.skipGradleBuild = False

        # 初始化此次打包工程的本地环境
        self.project.SetAndroidPackageEnv(self.channelconf,self.versionName,self.versionCode,self.bPackageDataInsideApk)





        pass
    
    @workspace
    def afterbuild(self):
        super().afterbuild()

        if self.bPackageDataInsideApk:
            packagepath = self.project.getPackagePath(self.platform)
            ftppath = self.getftppath(f'{self.getCodeConfStr(translateEnum=True)}.apk')
            httppath = G_ftp.ftppath2httppath(ftppath)
            content = chubaobot.markdown_textlink(self.getCodeConfStr(translateEnum=False)+'.apk',httppath) +'\n\n'
            G_ftp.upload(packagepath,ftppath)
        else:
            # obb在包外，需要打包main.obb和patch.obb
            androidBP = os.path.join(self.project.binariesPath,'Android')
            obbs = com.listdir_fullpath(androidBP,lambda x:x.endswith('.obb'))
            packagepath = self.project.getPackagePath(self.platform)
            bats = com.listdir_fullpath(androidBP,lambda x:x.endswith('.bat') or x.endswith('.sh'))
            for batfile in bats:
                com.replace_filecontent(batfile,'set ADB=%ANDROIDHOME%\\platform-tools\\adb.exe','set ADB=adb.exe')
            needzip = [packagepath] + obbs + bats + [os.path.join(self.env.androidsdkpath,'platform-tools','adb.exe'),os.path.join(self.env.androidsdkpath,'platform-tools','AdbWinApi.dll'),os.path.join(self.env.androidsdkpath,'platform-tools','AdbWinUsbApi.dll')]
            com.logout(f'[needzip] {needzip}')
            
            zippath = 'bigapk.zip'
            ZipManager.ziptarget(needzip,zippath,8,debug=True)
            
            ftppath = self.getftppath(f'{self.getCodeConfStr(translateEnum=True)}.zip')
            httppath = G_ftp.ftppath2httppath(ftppath)
            content = chubaobot.markdown_textlink(self.getCodeConfStr(translateEnum=False)+'.xapk',httppath) +'\n\n'
            
            G_ftp.upload(zippath,ftppath)
            content += f'这是obb包，需要电脑usb安装\n\n'

        content += self.getExtraBuildContent()
        
        data = chubaobot.build_markdown('新安卓包来啦',content)
        chubaobot.send(data)
        

        if 'None' != self.userBulidPhoneNum:
            atdata = chubaobot.build_text('您打的安卓包好了',self.userBulidPhoneNum)
            chubaobot.send(atdata)

        com.creat_download_html(httppath,workdir)


        # A8项目的配置
        if self.isA8Project and com.isNoneOrEmpty(self.forceSvnUrl):
            #上传shader pso
            if self.isMakePSOCache:
                psopath = os.path.join(self.project.savedPath,'Cooked','Android_ASTC','NextGenActionGame','Metadata','PipelineCaches')
                ftppath = com.get_ftp_tempsavepath(f'pso/{self.platform}')
                G_ftp.deletedir(ftppath)
                G_ftp.upload(psopath,ftppath)
            # 上传符号表到bugly
            symbolpath = os.path.join(self.project.intermediatePath,'Android','arm64','jni','arm64-v8a')
            id = SecretManager.getSecData('bugly','id')
            key = SecretManager.getSecData('bugly','key')
            version = self.versionName.__str__()
            parm = f'-platform Android -appid {id} -appkey {key} -bundleid {self.channelconf.applicationid} -version {version} -inputSymbol {symbolpath}'
            out,code = BinManager.buglysymupload(parm,getstdout=False)
            if code != 0:
                com.logout(f'[bugly] 上传符号表失败')

        pass
class UE4IOSBuilder(UE4Builder):
    def __init__(self,args):
        super().__init__(args)
        # 加载channelconf
        self.channelconf = Loader.load(ioschannelconf,self.channel)
        
    def prebuild(self):
        # 初始化此次打包工程的本地环境
        self.project.SetIOSEnv(self.channelconf,self.versionName.__str__(),self.mode)

        super().prebuild()


        pass
    

    def buildDownloadPlist(self,downloadUrl,appliactionId,version):
        filepath = os.path.join(thisdir,'download.plist')
        copypath = os.path.join(thisdir,'download_copy.plist')
        Path.ensure_pathnewest(filepath,copypath)
        com.replace_filecontent(copypath,'{downloadUrl}',downloadUrl)
        com.replace_filecontent(copypath,'{appliactionId}',appliactionId)
        com.replace_filecontent(copypath,'{version}',version)
        return copypath

    def afterbuild(self):
        super().afterbuild()

        packagepath = self.project.getPackagePath(self.platform)
        # 苹果OTA下载不支持中文和=号，需要encode一下
        ftppath = self.getftppath(f'{self.getCodeConfStr(translateEnum=True)}.ipa')
        G_ftp.upload(packagepath,ftppath)
        httppath = G_ftp.ftppath2httppath(ftppath)
        apkname = os.path.basename(ftppath)

        plistPath = self.buildDownloadPlist(httppath,self.channelconf.applicationid,self.versionName.__str__())
        plistftppath = self.getftppath(f'{apkname}.plist')
        G_ftp.upload(plistPath,plistftppath)

        content = chubaobot.markdown_textlink(self.getCodeConfStr(translateEnum=False)+'.ipa',httppath) +'\n\n'

        content += self.getExtraBuildContent()
        
        data = chubaobot.build_markdown('新IOS包来啦',content)
        chubaobot.send(data)

        if 'None' != self.userBulidPhoneNum:
            atdata = chubaobot.build_text('您打的iOS包好了',self.userBulidPhoneNum)
            chubaobot.send(atdata)
        
        
        com.creat_download_html(httppath,workdir)
        # macos 重签名
        self.jenkins.buildJob_parames("Client_Tool_Temp/A8_Package_Resign", waitBuildId=False, waitFinish=False, 
                            packageUrl=httppath, packageBundleId=self.channelconf.applicationid, ftpPath=os.path.dirname(ftppath))


class UE4WindowsBuilder(UE4Builder):
    def __init__(self,args):
        super().__init__(args)
        # 加载channelconf
        self.channelconf = Loader.load(ioschannelconf,self.channel)
        
    def prebuild(self):
        # 初始化此次打包工程的本地环境
        self.project.SetWindowsEnv(self.versionName.__str__(),self.mode)

        super().prebuild()


    @workspace
    def afterbuild(self):
        super().afterbuild()
        windowsZipPackPath = 'WindowsPackage.zip'
        packagepath = self.project.getPackagePath(self.platform)
        ZipManager.ziptarget([packagepath],windowsZipPackPath,8)

        ftppath = self.getftppath(f'{self.getCodeConfStr(translateEnum=True)}.zip')
        G_ftp.upload(windowsZipPackPath,ftppath)
        httppath = G_ftp.ftppath2httppath(ftppath)
        apkname = os.path.basename(ftppath)

        content = chubaobot.markdown_textlink(self.getCodeConfStr(translateEnum=False)+'.zip',httppath) +'\n\n'

        content += self.getExtraBuildContent()
        
        data = chubaobot.build_markdown('新windwos包来啦',content)
        chubaobot.send(data)

        if 'None' != self.userBulidPhoneNum:
            atdata = chubaobot.build_text('您打的windwos包好了',self.userBulidPhoneNum)
            chubaobot.send(atdata)
        
        com.creat_download_html(httppath,workdir)

kvSplit = '='
class PackageNameStruct(object):
    def __init__(self,name=None):
        if name != None:
            self.load(name)
    def addboolflag(self,flagname,flag=True):
        '''
        bool类型flag,设置为True
        '''
        setattr(self,flagname,flag)
        return self

    def addkvflag(self,key,value):
        '''
        键值对类型flag
        '''
        setattr(self,key,value)
        return self
    def load(self,name):
        '''
        重复键值会被name覆盖
        '''
        namelist = name.split('_')
        for x in namelist:
            if kvSplit in x:
                tmp = x.split(kvSplit)
                self.addkvflag(tmp[0],tmp[1])
            else:
                self.addboolflag(x)
        return self
    def combine(self,other):
        '''
        重复键值会被other覆盖
        '''
        self.load(other.__str__())
        return self
    def toString(self,unmarshal=False,translateEnum=True):
        ls = list(self.__dict__.items())
        orders = ['time','game','channel','codev','resv','vc','mode']
        orders.reverse()
        for order in orders:
            if order not in self.__dict__:
                continue
            value = self.__dict__[order]
            item = (order,value)
            ls.remove(item)
            ls.insert(0,item)
        namelist = []
        shortName = ''
        for item in ls:
            k = item[0]
            v = item[1]
            if isinstance(v,Enum):
                if translateEnum:
                    v = com.enumNameTranslate(v)
                else:
                    v = v.name

            # if com.safeBoolCompare(v,True):
            #     namelist.append(k)
            # else:
            if unmarshal:
                if k in ('time','codev','resv','game'):
                    namelist.append(f'{v}')
                else:
                    if isinstance(v,int) and not isinstance(v,bool):
                        shortName += v.__str__()
                    else:
                        shortName += v.__str__()[0]
            else:
                namelist.append(f'{k}{kvSplit}{v}')
        
            
        return '_'.join(namelist) + f'_{shortName}'

    def __preCompare(self,name,exclude):
        nameStruct = name
        if isinstance(name,str):
            nameStruct = PackageNameStruct(name)
        target = nameStruct.__dict__.copy()
        my = self.__dict__.copy()
        for ex in exclude:
            if ex in target:
                target.pop(ex)
            if ex in my:
                my.pop(ex)
        return target,my
    def isSubName(self,name,exclude=[]):
        target,my = self.__preCompare(name,exclude)
        return DictUtil.isSubDict(target,my)
    def isSameName(self,name,exclude=[]):
        target,my = self.__preCompare(name,exclude)
        return DictUtil.isSame(target,my)
    def __str__(self):
        return self.toString()
    def __getattr__(self,name:str):
        if name.startswith('__') and name.endswith('__'):
            return super().__getattr__(name)
        return False

    
    
class Project():
    def __init__(self,projectPath):
        self.projectPath = projectPath
    def _initAssetsPath(self,assetsPath):
        self.assetsPath = assetsPath
        # 藤木特殊文件夹 字段多了再加类
        self.AssetBundlesPath = os.path.join(self.assetsPath,'AssetBundles')
    def build(self):
        '''
        打包
        '''
    def getPackagePath(self):
        pass
class AndroidProject(Project):
    def __init__(self,projectPath):
        super().__init__(projectPath)
        
        self.buildgradlePath = os.path.join(self.projectPath,'build.gradle')
        self.appBuildGradlePath = os.path.join(self.projectPath,'app','build.gradle')
        self.gradlePropPath = os.path.join(self.projectPath,'gradle.properties')
        self.localPropPath = os.path.join(self.projectPath,'local.properties')
        self.settingsGradlePath = os.path.join(self.projectPath,'settings.gradle')
        self.releaseApkDir = os.path.join(self.projectPath,'build','outputs','apk','release')
        self.soSavePath = os.path.join(self.projectPath,'src','main','jniLibs')
        self.jarSavePath = os.path.join(self.projectPath,'libs')
        self.mainPath = os.path.join(self.projectPath,'src','main')
        self.resPath = os.path.join(self.projectPath,'src','main','res')
        self.assetsPath = os.path.join(self.projectPath,'src','main','assets')
        self.resvaluesPath = os.path.join(self.resPath,'values')
        self.stringxmlPath = os.path.join(self.resvaluesPath,'strings.xml')

        self.javaPath = os.path.join(self.projectPath,'src','main','java')

        self._initAssetsPath(self.assetsPath)


        self.gradleBinName = com.getvalue4plat('gradlew.bat','gradlew')
        self.gradleBinPath = os.path.join(self.projectPath,self.gradleBinName)
        if sys.platform != 'win32':
            com.make_executable(self.gradleBinPath)
    def gradleRelease(self):
        '''
        gradle task:assembleRelease
        罕见偶发性错误，暂未处理
        * What went wrong:
        Execution failed for task ':sdk_base:verifyReleaseResources'.
        > java.util.concurrent.ExecutionException: com.android.builder.internal.aapt.v2.Aapt2InternalException: AAPT2 aapt2-3.2.0-4818971-osx Daemon #0: Daemon startup failed
        This should not happen under normal circumstances, please file an issue if it does.
        '''
        # out,code = com.cmd(f'{self.gradleBinPath} assembleRelease',getstdout=False,errException=Exception('Android工程打包失败'))
        # tt = r"D:\gradle\gradle-5.6\bin\gradle.bat"
        logfile = com.get_logfile_path('gredle.assembleRelease.txt')
        out,code = com.cmd(f'{self.gradleBinPath} assembleRelease',logfile=logfile,getstdout=False,errException=Exception('Android工程打包失败'),cwd=self.projectPath)
    
    def getReleaseApkPath(self):
        if not os.path.exists(self.releaseApkDir):
            raise Exception(f'{self.releaseApkDir}路径不存在')
        apkname = [apk for apk in os.listdir(self.releaseApkDir) if apk.endswith('.apk')][0]
        return os.path.join(self.releaseApkDir,apkname)

    def build(self):
        '''
        打包
        '''
        self.gradleRelease()
    def getPackagePath(self):
        pass
    def configAndroidProject(self,channel,codeVersion):
        from comlib import RecordTool,loadversion,XMLFile
        cconf:channelconf = Loader.load(channelconf,channel)
        rt = RecordTool(os.path.join(G_projconfDir,'record.json'))

        minSDKVersion = cconf.minsdkversion
        targetSDKVersion = cconf.targetsdkversion
        applicationId = cconf.applicationid
        applicationname = cconf.applicationname
        versionCode = rt.getversioncode('android',channel)

        fullversion = loadversion(codeVersion,rt.getserverversion('android'))
        versionName = fullversion.__str__()

        # 更改显示名称
        stringxml = XMLFile(self.stringxmlPath)
        matche = stringxml.find(stringxml.root,'string',name="app_name")
        matche.text = applicationname
        stringxml.save()

        keystorename,keystoreConf = Loader.getKeystoreConf(channel)
        keystorePath = os.path.join(workdir,'keystore',keystorename)

        storePassword = keystoreConf.storepass
        keyAlias = keystoreConf.alias
        keyPassword = keystoreConf.keypass

        
        extconf = com.textwrap.dedent(f'''
        android {{
            defaultConfig {{
                applicationId = '{applicationId}'
                minSdkVersion = {minSDKVersion}
                targetSdkVersion = {targetSDKVersion}
                versionCode = {versionCode}
                versionName = '{versionName}'
            }}
            signingConfigs {{
                release {{
                    storeFile file('{com.convertsep2Unix(keystorePath)}')
                    storePassword '{storePassword}'
                    keyAlias '{keyAlias}'
                    keyPassword '{keyPassword}'
                    v1SigningEnabled true
                    v2SigningEnabled false
                }}
            }}
        }}
        ''')
        curBuildGradlePath = self.appBuildGradlePath
        if not os.path.exists(self.appBuildGradlePath):
            # unity没app文件夹
            curBuildGradlePath = self.buildgradlePath
        
        rawconf = com.readall(curBuildGradlePath)

        # okconf = rawconf \
        #     .replace('!!MINSDKVERSION_MY!!',minSDKVersion) \
        #     .replace('!!TARGETSDKVERSION_MY!!',targetSDKVersion) \
        #     .replace('!!APPLICATIONID_MY!!',applicationId) \
        #     .replace('!!VERSIONCODE_MY!!',str(versionCode)) \
        #     .replace('!!VERSIONNAME_MY!!',versionName) \
        #     .replace('!!SIGN_MY!!',sign)
        okconf = rawconf.replace('android {',f'{extconf}android {{')
        com.savedata(okconf,curBuildGradlePath)

    def configAndroidLocalTool(self):
        '''
        配置安卓工程的local.properties文件内sdk和ndk的本地路径
        '''
        # 配置还没写完，先不用default
        gconf = Loader.load(globalconf)
        conf = Loader.load(envconf,G_ip,gconf.engine,use_defalt=False)
        androidSDKPath = conf.androidsdkpath
        ndkPath = conf.ndkpath
        content = textwrap.dedent(f'''
        ndk.dir={com.convertsep2Unix(ndkPath)}
        sdk.dir={com.convertsep2Unix(androidSDKPath)}
        ''')
        
        com.savedata(content,self.localPropPath)

    def setGradlePropValue(self,key,value):
        propData = com.readall(self.gradlePropPath)
        props = propData.splitlines()
        isFind = False
        for index,prop in enumerate(props):
            if prop.startswith(f'{key}='):
                isFind = True
                props[index] = f'{key}={value}'
        if not isFind:
            props.append(f'{key}={value}')
        com.savedata(propData,self.gradlePropPath)


class IOSProject(Project):
    def __init__(self,projectPath,xcodeprojname='Unity-iPhone'):
        super().__init__(projectPath)
        self.frameworksRoot = os.path.join(projectPath,'Frameworks','Plugins','iOS')
        self.dataPath = os.path.join(projectPath,'Data')
        self.xcodeprojpath = os.path.join(self.projectPath,f'{xcodeprojname}.xcodeproj')



        self.assetsPath = os.path.join(self.dataPath,'Raw')
        self._initAssetsPath(self.assetsPath)
    def build(self,scheme='Unity-iPhone',mode='debug'):
        # xcodebuild
        # 1.
        # "xcodebuild -project \"%s\" clean" % projectPath
        
        com.cmd(f'xcodebuild -project "{self.xcodeprojpath}" clean',getstdout=False)
        # 2.
        #         command = ' '.join(["xcodebuild",
        #     "-project",
        #     "\"%s\"" % projectPath,
        #     "-sdk",
        #     "iphoneos",
        #     "-configuration",
        #     "%s" % 'Debug' if self._mode == 'debug' else 'Release',
	    # "DEBUG_INFORMATION_FORMAT=\"%s\"" % "dwarf-with-dsym",
        #     "CODE_SIGN_IDENTITY=\"%s\"" % self._package_code_sign,
        #     "CODE_SIGN_ENTITLEMENTS=\"Unity-iPhone/test.entitlements\"" if isapsnotification else "",
        #     "PROVISIONING_PROFILE=\"%s\"" % self._package_provision,
        #     "ENABLE_BITCODE=NO",
        #     "ONLY_ACTIVE_ARCH=NO",
        #     "ARCHS=\"%s\"" % "armv7 arm64",
        #     "build"])
        # from comlib import PKCS12

        # keydir = os.path.join(self.keystorepath,'ios',self.cconf.keystorename)
        # passfile = os.path.join(keydir,'pwd.pass')
        # mpfile = os.path.join(keydir,f'{self.cconf.keystorename}.mobileprovision')
        # p12file = os.path.join(keydir,f'{self.cconf.keystorename}.p12')
        # p12m = PKCS12(p12file,passfile)
        # p12m.load()

        # p12m.import_mac(mpfile)

        conf = 'Debug' if mode == 'debug' else 'Release'
        archivelogpath = com.get_logfile_path('xcodebuild_archive.txt')
        com.cmd(f'xcodebuild -project "{self.xcodeprojpath}" -scheme {scheme} -sdk iphoneos -configuration {conf} \
        CODE_SIGNING_REQUIRED="NO" CODE_SIGNING_ALLOWED="NO" \
        DEBUG_INFORMATION_FORMAT="dwarf-with-dsym" ENABLE_BITCODE="NO" ONLY_ACTIVE_ARCH=NO ARCHS="armv7 arm64" \
        -archivePath "{self.getPackagePath()}" \
        archive',logfile=archivelogpath)

        # xcodebuild -project test.xcodeproj -scheme "test" -sdk iphoneos -configuration Release CODE_SIGNING_REQUIRED="NO" CODE_SIGNING_ALLOWED="NO"  -archivePath build/Tyrion.xcarchive archive
        # xcodebuild  -exportArchive -archivePath build/Tyrion.xcarchive -exportPath build/test.ipa
        
        # 打出xcarchive后直接上传ftp 后续就用这个组，组完直接根据ExportOption.plist导出已签好的ipa
        # ab路径是 xxx.xcarchive/Products/Applications/xxx.app/Data/Raw/AssetBundles

        # 3.xcodebuild .app -> .ipa
        # xcodebuild
        # -exportArchive
        # -archivePath <xcarchivepath>
        # -exportPath <destinationpath>
        # -exportOptionsPlist <plistpath>#这个plist文件可以通过打一次ipa包里面去获取，然后根据需求修改
        # com.cmd(com.textwrap.dedent(f'''xcodebuild -exportArchive -archivePath -exportPath -exportOptionsPlist

        # '''))
        # xcodebuild -exportArchive -archivePath /Users/tengmu/Desktop/ceshi/test/build/Tyrion.xcarchive -exportPath /Users/tengmu/Desktop/ceshi/test/build/Tyrion.ipa -exportOptionsPlist ExportOption_exp.plist
        pass
    def getPackagePath(self):
        return os.path.join(self.projectPath,'build','code.xcarchive')


class SDKLinker(object):
    def __init__(self):
        super().__init__()
    # def _link(self,builder:Builder):
    #     pass
    def link_unityProj(self,unityProject:UnityProject):
        raise Exception('无Unity接入')
    def link_nativeProj(self,project:Project):
        raise Exception('无原生接入')
    def clean(self):
        pass
class AndroidSDKProjectLinker(SDKLinker):
    isFirstLink = True
    sdkDepandent=textwrap.dedent('''
    include ':sdkinterface', ':unitylib', ':sdk_base'
    project(':sdkinterface').projectDir = new File(projectPath + '/sdkinterface')
    project(':unitylib').projectDir = new File(projectPath + '/unitylib')
    project(':sdk_base').projectDir = new File(projectPath + '/sdk_base')
    ''')
    depandentSettingFormat = '''\ninclude ':{projectName}'\nproject(':{projectName}').projectDir = new File(projectPath + '/{projectName}') \n'''
    depandentMainFormat = '''implementation project(':{projectName}')'''

    def __init__(self,sdkprojectRootPath,channelExtRootPath,projectName):

        super().__init__()
        self.sdkprojectRootPath = sdkprojectRootPath
        self.channelExtRootPath = channelExtRootPath

        self.projectPath = os.path.join(sdkprojectRootPath,'AndroidProject',projectName)
        self.projectName = projectName
        self.channelExtPath = os.path.join(channelExtRootPath,f'android_{state.name.lower()}',projectName)

        self.tmpref = []
    def _link(self,buildgradlePath,settingsGradlePath):
        if AndroidSDKProjectLinker.isFirstLink:
            com.applydata(AndroidSDKProjectLinker.sdkDepandent,settingsGradlePath)
            AndroidSDKProjectLinker.isFirstLink = False
        depandentSettingStr = AndroidSDKProjectLinker.depandentSettingFormat.format(projectName=self.projectName)
        com.applydata(depandentSettingStr,settingsGradlePath)
        depandentMainStr = AndroidSDKProjectLinker.depandentMainFormat.format(projectName=self.projectName)
        
        com.re_replace('// REF_FLAG',f'// REF_FLAG\n    {depandentMainStr}',buildgradlePath)
    def _copy(self,path):
        '''
        path为unityAssetPath或者androidproject.mainPath目录
        '''
        # if 'ABProject' in path:
        #     raise Exception('不支持拷贝到资源工程，原因：资源工程可能在内存中，不受svn控制')
        if not os.path.exists(self.channelExtPath):
            com.logout(f'[警告] {self.channelExtPath}不存在')
            return
        combineFile,coverFile,combineDir = com.combinefolder(path,self.channelExtPath,cover=False,debug=True,fileextfilter='conf.json')
        self.combineFile = combineFile
        self.coverFile = coverFile
        self.combineDir = combineDir
        self.safeCoverFiles(self.coverFile)
    def loadconf(self,confDirPath):
        return JsonFile(os.path.join(confDirPath,'conf.json'))
    def safeCoverFiles(self,coverFiles):
        for fromfile,tofile in coverFiles:
            tmp_tofile = com.get_random_filename(os.getcwd())
            com.logout(f'[重复资源备份] {tofile} => {tmp_tofile}')
            shutil.move(tofile,tmp_tofile)
            shutil.copy2(fromfile,tofile)
            self.tmpref.append((tofile,tmp_tofile))
    def rollbackCoverFiles(self):
        for tofile,tmp_tofile in self.tmpref:
            com.logout(f'[重复资源还原] {tmp_tofile} => {tofile}')
            os.remove(tofile)
            shutil.move(tmp_tofile,tofile)
    def clean(self):
        self.rollbackCoverFiles()

class IOSSDKProjectLinker(SDKLinker):
    '''
    接入由xuport接管
    '''
    def __init__(self,channelExtRootPath,projectName) -> None:
        super().__init__()
        self.channelExtRootPath = channelExtRootPath
        self.projectName = projectName
        self.channelExtPath = os.path.join(channelExtRootPath,f'ios_{state.name.lower()}',projectName)

    def link_unityProj(self, project: UnityProject):
        if os.path.exists(self.channelExtPath):
            com.combinefolder(project.unityAssetPath,self.channelExtPath)

class LBAndroidLinker(AndroidSDKProjectLinker):
    def __init__(self,sdkprojectRootPath,channelExtRootPath,lebianVersion):
        lebianProjectName = getLBProjectName(lebianVersion)
        super().__init__(sdkprojectRootPath,channelExtRootPath,lebianProjectName)
    def useSplit(self):
        pass
    def link_unityProj(self,unityProject:UnityProject):
        self._link(unityProject.gradleMainPath,unityProject.gradleSettingsPath)
    def link_nativeProj(self,project:AndroidProject):
        self._link(project.buildgradlePath,project.settingsGradlePath)
class LBIOSLinker(IOSSDKProjectLinker):
    pass
class ChannelAndroidLinker(AndroidSDKProjectLinker):
    def __init__(self,sdkprojectRootPath,channelExtRootPath,channel):
        channelSDKProjectName = getChannelProjectName(channel)
        super().__init__(sdkprojectRootPath,channelExtRootPath,channelSDKProjectName)
    def link_unityProj(self,unityProject:UnityProject):
        self._link(unityProject.gradleMainPath,unityProject.gradleSettingsPath)
    def link_nativeProj(self,project:AndroidProject):
        self._link(project.buildgradlePath,project.settingsGradlePath)
        self._copy(project.mainPath)

class ChannelIOSLinker(IOSSDKProjectLinker):
    pass

class UWAAndroidLinker(AndroidSDKProjectLinker):
    def __init__(self,sdkprojectRootPath,channelExtRootPath,sdkname='uwa'):
        super().__init__(sdkprojectRootPath,channelExtRootPath,getThirdPartyProjectName(sdkname))
    
    def link_unityProj(self,unityProject:UnityProject,confparame):
        self._copy(unityProject.unityAssetPath)
        self._confIt(unityProject.unitym,confparame)
    def _confIt(self,unitym:TMUnityManager,confparame):
        conf = self.loadconf(self.channelExtPath)
        DirectMode = conf.trygetvalue(confparame)['DirectMode']
        DirectMonoManual = conf.trygetvalue(confparame)['DirectMonoManual']
        unitym.Conf_UWA(DirectMode,DirectMonoManual)
def getThirdPartyProjectName(sdkname):
    return f'thirdparty_{sdkname}'
def getChannelProjectName(channel):
    return f'sdk_{channel}'
def getLBProjectName(version):
    return f'lebian{version}'





subop = ''
def setsubop_code(namespace):
    global subop
    subop = 'code'
def setsubop_res(namespace):
    global subop
    subop = 'res'
def isBuildCode():
    global subop
    if subop == 'code':
        return True
    return False
def isBuildRes():
    global subop
    if subop == 'res':
        return True
    return False

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    # 打包配置
    parser.add_argument('--platform',required=True)
    parser.add_argument('--mode',required=True)
    parser.add_argument('--level',required=True,type=int)
    parser.add_argument('--svnUrl',required=True)
    parser.add_argument('--svnVersion',required=True)
    parser.add_argument('--useRamDisk',default=False,required=False,type=com.str2boolean,help='使用内存盘打包')



    # parser.add_argument('--packVersion',required=True,type=Version)
    subp = parser.add_subparsers(help='打代码和打资源配置')
    subpcode = subp.add_parser('code')
    # unity工程配置
    subpcode.add_argument('--isUnityDev',required=True,type=com.str2boolean)
    subpcode.add_argument('--isConnectProfiler',required=True,type=com.str2boolean)
    subpcode.add_argument('--isIL2CPP',required=True,type=com.str2boolean)
    subpcode.add_argument('--macro',required=True)
    # globalsettings配置
    subpcode.add_argument('--isUsingXYSDK'      ,default=True ,required=False,type=com.str2boolean,help='（遗留）是否是XYSDK')
    subpcode.add_argument('--isGuide'           ,default=True ,required=False,type=com.str2boolean,help='是否开启新手引导（jenkins可配置）')
    subpcode.add_argument('--forceUseAutoFight' ,default=False,required=False,type=com.str2boolean,help='强制自动战斗（jenkins可配置）')
    subpcode.add_argument('--enableHotFix'      ,default=True ,required=False,type=com.str2boolean,help='是否开启热更（jenkins可配置）')
    subpcode.add_argument('--loadFromPackage'   ,default=True ,required=False,type=com.str2boolean,help='是否从ab包内加载资源')
    subpcode.add_argument('--hotFixUrlDebug'    ,default=True ,required=False,type=com.str2boolean,help='debug下显示热更拉取url资源的日志')
    subpcode.add_argument('--isXYPaySDKDebug'   ,default=False,required=False,type=com.str2boolean,help='未知')
    subpcode.add_argument('--isRecordPVP'       ,default=False,required=False,type=com.str2boolean,help='未知')
    subpcode.add_argument('--isDebug'           ,default=False,required=False,type=com.str2boolean,help='全局debug设置 （jenkins可配置）')
    subpcode.add_argument('--skillHasCooldown'  ,default=False,required=False,type=com.str2boolean,help='未知')

    subpcode.set_defaults(func=setsubop_code)

    # !!!android配置!!!

    # !!!ios配置!!!
    parser.add_argument('--iosChannel',default='DNL15Dev',required=False,help='(ios)拷贝渠道')
    # 工具配置
    # parser.add_argument('--isEncrpyt',required=True,type=com.str2boolean)
    subpcode.add_argument('--uwaConf',default=None,required=False,type=com.str2Noneable,help='使用的uwa配置')
    
    subpres = subp.add_parser('res')
    # 资源配置
    subpres.add_argument('--needPackScriptData',default=True,required=False,type=com.str2boolean,help='转换ScriptData')
    

    subpres.set_defaults(func=setsubop_res)

    args = parser.parse_args()
    args.func(args)
    if args.platform == 'android':
        ab = AndroidBuilder(args)
    else:
        ab = IOSBuilder(args)
    ab.build()






    # 
    pass
