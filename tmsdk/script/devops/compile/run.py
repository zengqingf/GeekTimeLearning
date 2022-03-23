# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com

from comlib import TMUnityManager,JsonFile

from comlib.conf.loader import Loader
from comlib.conf.ref import envconf,compileconf,dingrobotsendconf

from comlib import workspace,Path,SVNManager,DictUtil,Dingrobot,XMLFile


import re
from comlib.comobj import *
compconf:compileconf = Loader.load(compileconf)


EditorEnvMarco = f'''UNITY_EDITOR;UNITY_EDITOR_64;{com.getvalue4plat('UNITY_EDITOR_WIN','UNITY_EDITOR_LINUX','UNITY_EDITOR_OSX')}'''

# Editor环境都用NET_4_6
# Editor_EditorMarco = ''
def getEditorMarco(normalMarco:str):
    return normalMarco.replace(compconf.release_dotnet_marco,compconf.editor_dotnet_marco)

# 非Editor环境都用NET_STANDARD_2_0
Standalone_Win_NormalMarco = compconf.standalone_win_normalmarco
Standalone_Mac_NormalMarco = compconf.standalone_mac_normalmarco
Android_NormalMarco = compconf.android_normalmarco
IOS_NormalMarco = compconf.ios_normalmarco
VS_NormalMarco = compconf.vs_normalmarco

svnurl = ''
msbuild = r'G:\VS_IDE\MSBuild\Current\Bin\MSBuild.exe'
projpath = r'G:\__bbbbb\1.5trunk\Client'

baseoutpath = os.path.join(workdir,'dll')
msbuild = compconf.msbuildpath
projpath = os.path.join(workdir,'Client')
version = 'HEAD'
lastversion = '1'

isunstable = False
allSuspect = []

lastversion_filename = os.path.join(workdir,'lastversion.txt')
unstable_filename = os.path.join(workdir,'unstable')
error_filename = os.path.join(workdir,'error.json')

def mark2chinese(mark):
    if mark == 'editor':
        return '编辑器'
    elif mark == 'android':
        return '安卓编辑器'
    elif mark == 'ios':
        return 'ios编辑器'
    elif mark == 'vs':
        return '验证服'
    elif mark == 'android_pack':
        return '安卓打包'
    elif mark == 'ios_pack':
        return 'ios打包'
    return mark
def findSuspect(filepath,lines):
    suspects = set()
    cmd = f'svn blame -r {int(lastversion) + 1}:{version} --xml --force "{filepath}"'
    # cmd = f'svn blame -r {200000}:{version} "{filepath}"'
    out,code = com.cmd(cmd,errException=StopException('svn blame失败',locals()))
    blamexml = XMLFile(out)
    target = blamexml.find(blamexml.root,'target',path=filepath)
    
    for line in lines:
        entry = blamexml.find(target,'entry',**{'line-number':line})
        if entry != None:
            commit = blamexml.find(entry,'commit')
            if commit != None:
                author = blamexml.find(commit,'author')
                if author != None:
                    suspect = author.text
                    suspects.add(suspect)
                    continue
        # 直接查询区间内提交人
        logs = SVNManager.getlog(lastversion,version,svnurl,containLastRivision=False)
        for log in logs:
            for commitState in log.commitStates:
                filename_remote = os.path.basename(commitState.path)
                filename_local = os.path.basename(filepath)
                # 这个文件的提交者
                if filename_local == filename_remote:
                    suspects.add(log.author)
        if suspects.__len__() == 0:
            for log in logs:
                for commitState in log.commitStates:
                    if commitState.path.endswith('.cs'):
                        suspects.add(log.author)
            
        break
        
    return suspects
def getBranchDesc(projpath):
    return '-'.join([
        Loader.getgame(),
        SVNManager.get_branch_Desc(projpath)
    ])
def compiteCS(csporjFiles,macro,outpath,mark):
    for f in csporjFiles:
        macro = macro.replace(';',' ')
        logfilename = f'{mark}_'+os.path.basename(f).replace('.csproj','.txt')
        logfile = com.get_logfile_path(logfilename)
        lc = locals()
        lc.pop('macro')
        # /t:rebuild 
        out,code = com.cmd(f'{msbuild} "{f}" /p:WarningLevel=0 /p:Configuration=Debug /p:DebugType=full /p:DefineConstants="{macro}" \
        /t:rebuild /p:OutputPath="{outpath}" 2>&1',
        getstdout=True)
        com.savedata(out,logfile,encoding='gbk')
        if code != 0:
            # 通知
            if 'Build FAILED' in out:
                rawerrorlog = out.split('CoreCompile target')[1]
            elif '生成失败' in out:
                rawerrorlog = out.split('生成失败')[1]
            else:
                raise Exception('未知输出')
            errorlogs = re.findall(r' .*?\([\d]*?,[\d]*?\)',rawerrorlog)
            errorfilesref = {}
            for errorlog in errorlogs:
                m = re.match(r'  (.*?)\(([\d]*?),([\d]*?)\)',errorlog)
                error_filepath = os.path.join(projpath,m.group(1))
                print(f'file {m.group(1)} line {m.group(2)} pos {m.group(3)}')
                if DictUtil.hasKey(errorfilesref,error_filepath):
                    errorfilesref[error_filepath].append(m.group(2))
                else:
                    errorfilesref[error_filepath] = [m.group(2)]
            suspects = set()
            for filepath,lines in errorfilesref.items():
                subset = findSuspect(filepath,lines)
                for x in subset:
                    suspects.add(x)

            global allSuspect
            for s in suspects:
                if s not in allSuspect:
                    allSuspect.append(s)
            
            
            def Ding():
                suspectsstr = '|'.join(suspects)
                
                bot = getdingrobot()

                desc = getBranchDesc(projpath)
                ftppath = f'/DevOps/ClientToolArtifact/{desc}/ScriptsCompile/{com.timemark}/{logfilename}'
                G_ftp.upload(logfile,ftppath)
                httppath = G_ftp.ftppath2httppath(ftppath)

                svnurlchinese = Loader.getbranchdesc_url2chinese(svnurl)

                data = bot.build_link('犯罪嫌疑人出现了！',f'{svnurlchinese}{mark2chinese(mark)}{logfilename}编译失败，嫌疑人：{suspectsstr}',httppath)
                canat = []
                for suspect in suspects:
                    phone = Loader.根据svnname获取电话(suspect)
                    if phone == None:
                        continue
                    canat.append(phone)
                bot.send(data)
                if canat.__len__() != 0:
                    data = bot.build_text('请犯罪嫌疑人尽快自首',*canat)
                    bot.send(data)
            

            # 储存{编译类型：{错误文件：[错误行号]}}
            # 如果上一次错误和现在错误一样，不提醒

            # 先判断文件存在否
            # 存在则读取文件，判断当前编译类型是否存在，不存在则提醒然后储存，存在则对比文件行号是否相同，不相同则提醒然后储存覆盖
            # 不存在文件则提醒并储存
            if os.path.exists(error_filename):
                lastError = JsonFile(error_filename)
                lastErrorFileRef = lastError.trygetvalue(mark)
                # 存在则对比文件行号是否相同，不相同则提醒然后储存覆盖
                if lastErrorFileRef:
                    if not DictUtil.isSame(lastErrorFileRef,errorfilesref):
                        Ding()
                        lastError.setvalue(mark,value=errorfilesref)
                # 判断当前编译类型是否存在，不存在则提醒然后储存
                else:
                    Ding()
                    lastError.setvalue(mark,value=errorfilesref)
                lastError.save()
            else:
                Ding()
                com.dumpfile_json({mark:errorfilesref},error_filename)
            # if os.path.exists('suspects'):
            #     lastSuspects = com.loadfile_json('suspects')
            #     for lastSuspect in lastSuspects:
            #         if lastSuspect not in suspects:
            #             # 有全新的嫌疑人，需要重新写入并且提示
            #             Ding()
            #             com.dumpfile_json(suspects,'suspects')
            #             break
            # else:
            #     Ding()
            global isunstable
            isunstable = True

            
    pass

def getdingrobot()->Dingrobot:
    import comlib.dingrobot as dingrobotmodel
    return Loader.获取客户端维护机器人()
def create_pack_csprojfiles(csporjFiles):
    # 有引用要改引用
    for csprojf in csporjFiles:
        xmlf = XMLFile(csprojf)
        es = xmlf.findall(xmlf.root,'ItemGroup')
        for e in es:
            xmlf.remove(e,'Reference',Include="UnityEditor")
            # 先手动移除下，unity程序集配置文件是spine-unity-editor.asmdef
            xmlf.remove(e,'ProjectReference',Include="spine-unity-editor.csproj")
        # xmlf.save(csprojf.replace('.csproj','_pack.csproj'))
        xmlf.save()
@errorcatch(HIGH)
@workspace
def run():
    global svnurl
    svnurl = sys.argv[1]
    client_release_marco = sys.argv[2]
    vs_release_marco = sys.argv[3]


    env = Loader.getenvconf()
    unitypath = env.enginepath
    keep = [[os.path.join('Assets','Resources'),['Base','Base.meta']],['',['Assets','ProjectSettings']]]
    Path.ensure_svn_pathexsits(projpath,svnurl,checkout_keep=keep)
    global version,lastversion
    version = SVNManager.version(projpath)
    # 生成.csproj
    tmu = TMUnityManager(unitypath,projpath)
    csporjPaths,code = tmu.Generate_CSProj()
    # 生成dll
    # 编译顺序 firstpass -> editor.firstpass -> normal -> editor
    # editor包含前三引用
    # normal包含firstpass引用
    firstpassEditor = com.remove_match(csporjPaths,lambda x: 'Editor-firstpass'.lower() in x.lower())
    firstpass = com.remove_match(csporjPaths,lambda x: 'firstpass'.lower() in x.lower())
    editor = com.remove_match(csporjPaths,lambda x: 'Assembly-CSharp-Editor.csproj'.lower() in x.lower())
    # 无用的，因为上面editor会自动包含所有editor
    _ = com.remove_match(csporjPaths,lambda x: 'Editor'.lower() in x.lower())
    # 剩余的都是会编译进包里的
    normalfiles = csporjPaths

    # 需要编译测试的环境
    # 编辑器 安卓 ios 验证服
    def compite_editor():
        mark = 'editor'
        thisoutpath = f'{baseoutpath}_{mark}'
        Path.ensure_dirnewest(thisoutpath)
        # editor会引用其他库
        compiteCS(editor,f'{getEditorMarco(Standalone_Win_NormalMarco)};{client_release_marco};{EditorEnvMarco}',thisoutpath,mark)

    def compile_android():
        mark = 'android'
        thisoutpath = f'{baseoutpath}_{mark}'
        Path.ensure_dirnewest(thisoutpath)
        compiteCS(editor,f'{getEditorMarco(Android_NormalMarco)};{client_release_marco};{EditorEnvMarco}',thisoutpath,mark)

    def compile_ios():
        mark = 'ios'
        thisoutpath = f'{baseoutpath}_{mark}'
        Path.ensure_dirnewest(thisoutpath)
        compiteCS(editor,f'{getEditorMarco(IOS_NormalMarco)};{client_release_marco};{EditorEnvMarco}',thisoutpath,mark)

    def compile_vs():
        mark = 'vs'
        thisoutpath = f'{baseoutpath}_{mark}'
        Path.ensure_dirnewest(thisoutpath)
        if Loader.getgame() in ('dnl1.0','dnl1.5'):
            compiteCS(editor,f'{getEditorMarco(VS_NormalMarco)};{vs_release_marco};{EditorEnvMarco}',thisoutpath,mark)
        else:
            compiteCS(normalfiles,f'{VS_NormalMarco};{vs_release_marco}',thisoutpath,mark)
    def compile_android_pack():
        mark = 'android_pack'
        thisoutpath = f'{baseoutpath}_{mark}'
        Path.ensure_dirnewest(thisoutpath)
        compiteCS(normalfiles,f'{Android_NormalMarco};{client_release_marco}',thisoutpath,mark)
    def compile_ios_pack():
        mark = 'ios_pack'
        thisoutpath = f'{baseoutpath}_{mark}'
        Path.ensure_dirnewest(thisoutpath)
        compiteCS(normalfiles,f'{IOS_NormalMarco};{client_release_marco}',thisoutpath,mark)
    

    if os.path.exists(lastversion_filename):
        lastversion = com.readall(lastversion_filename)
    # editor环境
    compite_editor()
    if code == 0:
        compile_android()
        compile_ios()

        if Loader.getgame() in ('dnl1.0','dnl1.5'):
            print('[editor vs]')
            compile_vs()
        # 打包环境
        # 删除editor引用,创建正式打包用csproj文件
        create_pack_csprojfiles(firstpass)
        create_pack_csprojfiles(normalfiles)
        if Loader.getgame() not in ('dnl1.0','dnl1.5'):
            print('[NO editor vs]')
            compile_vs()
        compile_android_pack()
        compile_ios_pack()
    # 不稳定构建不储存
    if not isunstable:
        com.savedata(int(version),lastversion_filename)
        if os.path.exists(unstable_filename):
            Path.ensure_pathnotexsits(unstable_filename)
            Path.ensure_pathnotexsits(error_filename)
            Ding_fixed()
        # com.savedata
        oldcsprojpaths = list(map(lambda x:os.path.join(projpath,x),filter(lambda x:x.endswith('.csproj'),os.listdir(projpath))))
        for old in oldcsprojpaths:
            print(f'[remove] {old}')
            os.remove(old)
    else:
        if not os.path.exists('unstable'):
            com.savedata('unstable',unstable_filename)

def Ding_fixed():
    robot = Loader.获取客户端维护机器人()
    data = robot.build_text('犯罪嫌疑人已改邪归正')
    robot.send(data)
if __name__ == "__main__":
    run()
    pass