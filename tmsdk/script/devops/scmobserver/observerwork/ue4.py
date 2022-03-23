# -*- encoding: utf-8 -*-
from comlib.comobj import *
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())

from comlib import SVNManager,Path,TMUnityManager,SVNState,LogCommitState,JsonFile,UE4Project

from scmobserver.observer import UE4Observer,ObserverState
from scmobserver.commethod import CommitWorkHelper,CommitWorkData,ObserverResult,ObserverResultCtl,findSuspects,svnRepoFilePath2LocalFilePath

ln = 'UE4OB'

class CompileCode(UE4Observer):
    def __init__(self,proj,workArgs,workMechine,data:CommitWorkData,dependences) -> None:
        super().__init__(proj,workArgs,workMechine,data,dependences)
        # self.targetPlatforms = ['Android']
        self.commitEditor = com.str2boolean(self.workArgs[0])
        self.plats = self.workArgs[1::]
        self.forceCommit = False
        self.forceBuild = False
        if self.data.cmdArgs != None:
            self.forceCommit = com.str2boolean(self.data.cmdArgs[0])
            self.forceBuild = com.str2boolean(self.data.cmdArgs[1])

        self.mechinePlatform = 'Win64'
        if com.isMac():
            self.mechinePlatform = 'Mac'
        self.commited = False

        self.rawLog = com.get_logfile_path('ubt_raw_compile.txt')
        self.logDir = os.path.dirname(self.rawLog)
        self.manifest = os.path.join(self.logDir,f'ubt_manifest.xml')

    def mustDo(self, commitdata: List[Tuple[LogCommitState, str]], revision: int):
        # 不提交上一次结果，因为jenkins会将上一次结果revert
        lastCommitBinariesTsp = CommitWorkHelper.getSuccessData('CompileCode','commitBinariesTsp','0')
        tspDelta = int(G_timemark_tsp) - int(lastCommitBinariesTsp) 
        com.logout(f'[mustDo][二进制文件提交] 当前距离上一次提交时间戳长度：{tspDelta}')
        # 30min提交一次,提交在编译后，所以设置为强制编译
        if tspDelta >= 30 * 60 * 1000:
            self.forceBuild = True
            com.logout(f'[二进制文件提交]设置为强制编译')
    def isForceRun(self):
        return self.forceBuild
    def canRun(self, commitdata: List[Tuple[LogCommitState, str]], revision: int):
        # TODO 处理文件夹
        for commitstate,commitfilepath in commitdata:
            ext = os.path.splitext(commitfilepath)[-1]
            # com.logout(f'[canRun] {commitfilepath}  {ext}')
            
            if ext not in ('.target','.modules', # 通用
            '.dll','.pdb', # win64
            '.so', # android
            'dylib', # mac
            '.a'): # ios
                com.logout(f'[canRun] CompileCode {True} {commitfilepath}  {ext}')

                return True
        com.logout(f'[canRun] CompileCode {False}')
        return False
    def run(self,commitdata:List[Tuple[LogCommitState,str]],revision:int):
        rebuild = False


        self.state = ObserverState.RUNNING
        targetModel = 'HitBoxMakerBlueprint'
        # targetPlatform = 'Android'
        curModel = ''
        curPlatform = self.mechinePlatform
        # ERROR: HitBoxMakerBlueprintEditor: Android does not support modular builds
        buildMode = 'Development'
        # buildMode = 'Shipping'

        # 第一步：编译当前平台Editor        
        curModel = f'{targetModel}Editor'
        
        out,code = self.proj.buildGameModule(curModel,curPlatform,buildMode,self.manifest,self.rawLog,clean=rebuild)
        if code == 0:
            CommitWorkHelper.setSuccessUniqueData('CompileCode',key='binariesRevision',data=revision)
            # 上传Editor dll
            self.tryCommitBinaries(revision)

        compilelogs,lastCompileLog = self.getCompileLog()
        # 第二步：编译目标平台
        if code == 0:
            self.successDing(curModel,curPlatform,lastCompileLog,commitdata)

            for plat in self.plats:
                curPlatform = plat
                curModel = targetModel
                # ios要配置环境，因为要签名
                if curPlatform == 'ios':
                    channelcf = Loader.load(ioschannelconf,'debug')
                    self.proj.SetIOSEnv(channelcf,'1')
                out,code = self.proj.buildGameModule(curModel,curPlatform,buildMode,self.manifest,self.rawLog,clean=rebuild)
                # 回滚配置文件
                SVNManager.revert(self.proj.configPath)
                
                # if code != 0:
                compilelogs,lastCompileLog = self.getCompileLog()
                if code == 0:

                    self.successDing(curModel,curPlatform,lastCompileLog,commitdata)
                else:
                    self.state = ObserverState.FAIL
                    self.errorDing(code,curModel,curPlatform,lastCompileLog,commitdata)
        else:
            self.state = ObserverState.FAIL

            self.errorDing(code,curModel,curPlatform,lastCompileLog,commitdata)

        # TODO拆分平台、编辑器编译
        if self.state == ObserverState.RUNNING:
            self.state = ObserverState.SUCCESS



    def afterRun(self,commitdata:List[Tuple[LogCommitState,str]],revision:int):
        pass
    def getCompileLog(self):
        compilelogs = list(filter(lambda x: x.endswith('CompileLog.log') or x.endswith('CompileLog_Remote.txt'),os.listdir(self.logDir)))
        maxTick = 0
        lastCompileLog = None
        if compilelogs.__len__() != 0:
            lastCompileLog = compilelogs[0]
            for compilelog in compilelogs:
                timetick = int(compilelog.split('_')[0])
                if timetick > maxTick:
                    maxTick = timetick
                    lastCompileLog = compilelog
        return compilelogs,lastCompileLog

    def tryCommitBinaries(self,revision:int):
        # if CommitWorkHelper.isLastUnstable('CompileCode','HitBoxMakerBlueprintEditor_Win64'):
        #     com.logout(f'[二进制文件提交] 由于上次编译失败，跳过本次提交')
        #     return
        lastCommitBinariesTsp = CommitWorkHelper.getSuccessData('CompileCode','commitBinariesTsp','0')
        lastBinariesRevision = CommitWorkHelper.getSuccessData('CompileCode','binariesRevision','0')
        tspDelta = int(G_timemark_tsp) - int(lastCommitBinariesTsp) 
        com.logout(f'[二进制文件提交] 当前距离上一次提交时间戳长度：{tspDelta}')
        canCommit = True
        # 30min提交一次
        if tspDelta < 30 * 60 * 1000 and not self.forceCommit:
            canCommit = False
        if canCommit and self.commitEditor:
            self.commitBinaries(lastBinariesRevision)
            self.commited = True
    def commitBinaries(self,revision:int):
        commitPaths = [self.proj.binariesPath]
        commitPaths += self.proj.getPluginBinariesDirs()
        clPaths =  []
        addPaths = []
        for commitPath in commitPaths:
            if SVNManager.isInversion(commitPath):
                for file in os.listdir(commitPath):
                    fullpath = os.path.join(commitPath,file)
                    if not SVNManager.isInversion(fullpath):
                        SVNManager.add(fullpath)
                        addPaths.append(commitPath)
                    else:
                        SVNManager.add(fullpath)
                        clPaths.append(commitPath)
            else:
                SVNManager.add(commitPath)
                addPaths.append(commitPath)
        
        if addPaths.__len__() > 0:
            for addPath in addPaths:
                SVNManager.commit(addPath,f'[机器人] 编译Editor版本{revision}')
        if clPaths.__len__() > 0:
            cl = SVNManager.changelist('dllci',SVNManager.get_root(self.proj.projectPath),*clPaths)
            SVNManager.commit_changelist(cl,f'[机器人] 编译Editor版本{revision}',self.proj.projectPath)

        CommitWorkHelper.setSuccessUniqueData('CompileCode',key='commitBinariesTsp',data=G_timemark_tsp)

    def successDing(self,curModel:str,curPlatform:str,lastCompileLog:str,commitdata:List[Tuple[LogCommitState,str]]):
        # compileName = f'CompileCode_{curPlatform}'
        # xxx已修复，剩余xxx
        if CommitWorkHelper.isLastUnstable('CompileCode',f'{curModel}_{curPlatform}'):
            bot = Loader.获取客户端维护机器人()
            content = bot.build_markdown(f'{curModel}模块{curPlatform}平台编译成功',f'{curModel}模块{curPlatform}目标编译成功，剩余')
            bot.send(content)
            CommitWorkHelper.resetUnstable('CompileCode',f'{curModel}_{curPlatform}')
            pass

    def errorDing(self,code:int,curModel:str,curPlatform:str,lastCompileLog:str,commitdata:List[Tuple[LogCommitState,str]]):
        logFtpPath = com.get_ftp_logsavepath('codecompile')
        if lastCompileLog == None:
            ubtLogFtpPath = f'{logFtpPath}/ubt_raw_compile.txt'

            # msvc是gbk编码
            if self.mechinePlatform == 'Win64':
                encoding = com.getEncoding(self.rawLog)
                
                com.convertEncoding(self.rawLog,'utf-8','gbk')

            G_ftp.upload(self.rawLog,ubtLogFtpPath)
            # UBT错误
            bot = Loader.获取脚本调试机器人()
            content = bot.markdown_textlevel('UBT执行报错',4)
            content += bot.markdown_drawline()
            content += f'失败模块：{curModel}\n\n'
            content += f'编译平台：{self.mechinePlatform}\n\n'
            content += f'编译目标：{curPlatform}\n\n'
            content += f'编译开始时间：{G_timemark}\n\n'
            content += f'UBT日志：{bot.markdown_textlink(G_ftp.ftppath2httppath(ubtLogFtpPath),G_ftp.ftppath2httppath(ubtLogFtpPath))}\n\n'
            # btn = {'UBT日志':G_ftp.ftppath2httppath(ubtLogFtpPath)}
            # data = bot.build_actioncard_mult('UBT执行报错',content,True,True,**btn)
            data = bot.build_markdown('UBT执行报错',content)
            bot.send(data)
            
            pass
        else:
            com.logout(f'[lastCompileLog] {lastCompileLog}')
            failCompileLogPath = os.path.join(self.logDir,lastCompileLog)
            com.logout(f'[failCompileLogPath] {failCompileLogPath}')

            # 去除Prefix头（ParallelExecutor.ExecuteActions:   ）
            com.re_replace(r'.*?:[ ]{0,3}','',failCompileLogPath,lineMode=True)

            
            # 找嫌疑人，编译器不同日志也会不同
            # clang /Private/DSkillDataOpenActions.cpp:2:10: fatal error: 'Widgets\Images\SImage.h' file not found
            # clang /Public/Common.h:47:9: error: 'M_PI' macro redefined [-Werror,-Wmacro-redefined]
            # android clang++ /TenmoveUEEditorBridge/Source/TenmoveUEEditorBridge/Private/SkillData/EdSkillActor.cpp(475,20): error: out-of-line definition of 'GetAnimationFrameCount' does not match any declaration in 'AEdSkillActor'
            # msvc \HitBoxMakerBlueprint\Public\AI/BehavicaUtility.h(87): error C2864: TenmoveBattle::BehavicaUtility::FilterTypeDescArray: 带有类内初始化表达式的静态 数据成员 必须具有不可变的常量整型类型，或必须被指定为“内联”
            # msvc C:\jenkins\workspace\Client_Tool_Temp\A8_Trunk_Compile_win10\Program\Client\NextGenGame\Source\HitBoxMakerBlueprint\Private\UEBattle/Graphic/GeHitNumberManager.h(6): fatal error C1083: 无法打开包括文件: “AActor/AHitNumber.h”: No such file or directory
            # fastbuild xxx (监控暂不用fastbuild)
            compilelogDataLines = com.readall(failCompileLogPath).splitlines()
            errorInfos: set[Tuple[str,str,str,str]] = set() # 文件路径，行号，错误编号， 文件版本号
            for line in compilelogDataLines:
                # msvc
                if curPlatform.lower() in ('win64',):
                    pt = r'^(.*?)\(([\d]*)\): .*error (.*?):'
                elif curPlatform.lower() == 'android':
                    pt = r'^(.*?)\(([\d]*),[\d]*\): .*error:(.*?)'
                # clang
                else:
                    pt = r'^(.*?):([\d]*):[\d]*: .*error: (.*)'
                m = re.match(pt,line)
                if m == None:
                    # TODO fastbuild
                    continue
                # ios是远程编译，需要转换回本地路径
                if curPlatform == 'ios':
                    localpath = m.group(1).replace(f'/Users/tenmove/UE4/Builds/','',1).split('/',1)[1].replace('/',':\\',1).replace('/','\\')
                    ver = 0
                    if SVNManager.isInversion(localpath):
                        ver = SVNManager.version(localpath)
                    errorInfos.add((m.group(1),m.group(2),m.group(3),ver))
                else:
                    ver = 0
                    if SVNManager.isInversion(m.group(1)):
                        ver = SVNManager.version(m.group(1))
                    errorInfos.add((m.group(1),m.group(2),m.group(3),ver))
            Log.warning('----------------errorInfos-----------------',ln)
            Log.warning(errorInfos)
            Log.warning('---------------------------------')
            def inner(commitstate,commitfilepath):
                svnLocalFilePath = svnRepoFilePath2LocalFilePath(self.data.svnurl,commitfilepath,self.data.projRoot)
                for filepath,line,errorCode,revision in errorInfos:
                    if os.path.abspath(com.convertsep(filepath)) == os.path.abspath(svnLocalFilePath):
                        return commitstate.parent.author
                return None
            suspects = findSuspects(commitdata,inner)
            needNotify = True
            # compileName = f'CompileCode_{curPlatform}'
            # 上一次是不稳定构建
            if CommitWorkHelper.isLastUnstable('CompileCode',f'{curModel}_{curPlatform}'):
                lastUnstableData = CommitWorkHelper.getLastUnstableData('CompileCode',f'{curModel}_{curPlatform}')
                lastErrorInfos = map(lambda x:tuple(x),lastUnstableData['lastErrorInfos'])
                lastErrorInfos = set(lastErrorInfos)
                diff = lastErrorInfos.symmetric_difference(errorInfos)
                Log.warning('---------------lastErrorInfos------------------',ln)
                Log.warning(lastErrorInfos,ln)
                Log.warning('---------------------------------',ln)
                Log.warning('---------------diff------------------',ln)
                Log.warning(diff)
                Log.warning('---------------------------------',ln)
                # 两次编译报错没有差异，则不进行通知
                if diff.__len__() == 0:
                    needNotify = False

            if not needNotify:
                return
            
            CommitWorkHelper.setUnstable('CompileCode',f'{curModel}_{curPlatform}','lastErrorInfos',data=list(errorInfos))
            compileLogFtpPath = f'{logFtpPath}/{curPlatform}_CompileLog.txt'
            ubtLogFtpPath = f'{logFtpPath}/{curPlatform}_ubt_raw_compile.txt'
            ubtManifestFtpPath = f'{logFtpPath}/{curPlatform}_ubt_manifest.xml'
            # msvc是gbk编码
            if self.mechinePlatform == 'Win64':
                encoding = com.getEncoding(failCompileLogPath)
                com.convertEncoding(failCompileLogPath,'utf-8','gbk')
                encoding = com.getEncoding(self.rawLog)
                com.convertEncoding(self.rawLog,'utf-8','gbk')
            G_ftp.upload(failCompileLogPath,compileLogFtpPath)
            G_ftp.upload(self.rawLog,ubtLogFtpPath)
            G_ftp.upload(self.manifest,ubtManifestFtpPath)

            bot = Loader.获取客户端维护机器人()
            content = bot.markdown_textlevel('编译失败报告',4)
            content += bot.markdown_drawline()
            content += f'失败模块：{curModel}\n\n'
            content += f'编译平台：{self.mechinePlatform}\n\n'
            content += f'编译目标：{curPlatform}\n\n'
            content += f'编译开始时间：{G_timemark}\n\n'
            content += f'嫌疑人：{"|".join(suspects)}\n\n'
            # if curPlatform == 'ios':
            #     # FileReference BaseLogFile = Log.OutputFile ?? new FileReference(BaseLogFileName);
            #     # FileReference RemoteLogFile = FileReference.Combine(BaseLogFile.Directory, BaseLogFile.GetFileNameWithoutExtension() + "_Remote.txt");
            #     com.logout()
            #     content += f'编译日志：ios是远程编译，编译日志看UBT日志\n\n'
            # else:
            content += f'编译日志：{bot.markdown_textlink(G_ftp.ftppath2httppath(compileLogFtpPath),G_ftp.ftppath2httppath(compileLogFtpPath))}\n\n'
            content += f'UBT日志：{bot.markdown_textlink(G_ftp.ftppath2httppath(ubtLogFtpPath),G_ftp.ftppath2httppath(ubtLogFtpPath))}\n\n'
            content += f'编译产物：{bot.markdown_textlink(G_ftp.ftppath2httppath(ubtManifestFtpPath),G_ftp.ftppath2httppath(ubtManifestFtpPath))}\n\n'
            # btn = {
            #     '编译日志':G_ftp.ftppath2httppath(compileLogFtpPath),
            #     'UBT日志':G_ftp.ftppath2httppath(ubtLogFtpPath),
            #     '编译产物':G_ftp.ftppath2httppath(ubtManifestFtpPath)
            #     }
            # data = bot.build_actioncard_mult('编译失败',content,True,True,**btn)
            data = bot.build_markdown('编译失败',content)
            bot.send(data)


class CompileBluePrints(UE4Observer):
    def __init__(self, proj: UE4Project, workArgs, workMechine, data: CommitWorkData,dependences) -> None:
        super().__init__(proj, workArgs, workMechine, data,dependences)

    def mustDo(self,commitdata:List[Tuple[LogCommitState,str]],revision:int):
        pass
    def isForceRun(self):
        return True
    def canRun(self,commitdata:List[Tuple[LogCommitState,str]],revision:int):
        return True
    @workspace
    def run(self,commitdata:List[Tuple[LogCommitState,str]],revision:int):
        '''
        commitdata必定不为空
        '''
        Log.info('-----------------------------------',ln)
        Log.info(self.dependences,ln)
        Log.info(self.dependences[0].state,ln)
        Log.info('-----------------------------------',ln)
        self.state = ObserverState.RUNNING

        logp = os.path.join(os.getcwd(),'BP_compile.txt')
        __InWorldBlueprintEditingPath = '/Engine/Tutorial/InWorldBlueprintEditing'
        parames = [f'-IgnoreFolder=/Engine,{__InWorldBlueprintEditingPath}']
        # 由于模块编译失败，导致无法运行蓝图并且卡死
        # 解决方法：
        # 1.单拉一个蓝图编译工程，不和代码编译一起
        # 2.代码编译成功后再运行
        if CommitWorkHelper.isNowUnstable('CompileCode'):
            com.logout(f'[CompileBluePrints] 由于c++编译失败，跳过蓝图编译')
            self.state = ObserverState.FAIL
            return
        out,code,logpath = self.engine.commandLet(self.proj.uprojectfilePath,'CompileAllBlueprints',parames,logp,timeout=4*60*60)
        
        bot = Loader.获取客户端维护机器人()

        observerR = ObserverResult(['CompileBluePrints'],'CompileBluePrints')
        errorInfo = []

        def sucFlag():
            Log.info(f'[code] {code}',ln)
            return code == 0
        def successNotification(o:ObserverResult):
            bot.send(bot.build_text('蓝图编译成功'))
        def failNotification(o:ObserverResult):
            compileLogFtpPath = getFTPPath(o.observerName,'BP_compile.txt')
            lines = com.readall(logp).splitlines()
            content = []
            started = False

            # LogLinker: Warning: Unable to load AnimClipData with outer Package /Game/AnimClipData because its class does not exist
            # TODO 上面这个日志从警告提升到错误
            for line in lines:
                if line.endswith('Warning/Error Summary (Unique only)'):
                    started = True
                if started:
                    m = re.match(r'^.*?Error: \[Compiler (.*?)\] (.*?)from Source: (.*)',line)
                    if m != None:
                        errorInfo.append((m.group(1),m.group(2),m.group(3)))
                    content.append(line)
                if 'Main return this error code' in line:
                    break
            # print('\n'.join(content))
            

            if content.__len__() > 20:
                content = content[0:20]
                content.append('最多20条，详细看日志')
            
            G_ftp.upload(logp,compileLogFtpPath)
            httppath = G_ftp.ftppath2httppath(compileLogFtpPath)

            def inner(commitstate,commitfilepath):
                if 'bp_' in os.path.basename(commitfilepath).lower():
                    return commitstate.parent.author
                return None
            suspects = findSuspects(commitdata,inner)
            suspects_str = '\n'.join(content)
            body =  f'''蓝图编译失败\n嫌疑人：{'|'.join(suspects)}\n日志：{httppath}\n{suspects_str}'''
            data = bot.build_text(body)
            bot.send(data)
        observerR.sucFlag = sucFlag
        observerR.successNotification = successNotification
        observerR.failNotification = failNotification
        if sucFlag():
            self.state = ObserverState.SUCCESS
        else:
            self.state = ObserverState.FAIL

        ObserverResultCtl.run(observerR,{'errorInfos':errorInfo},{})

    
    def afterRun(self,commitdata:List[Tuple[LogCommitState,str]],revision:int):
        '''
        commitdata必定不为空
        '''
        pass

class UE4ConfigIniFileUpdate(UE4Observer):
    def __init__(self, proj: UE4Project, workArgs, workMechine, data: CommitWorkData, dependences) -> None:
        super().__init__(proj, workArgs, workMechine, data, dependences)
    def canRun(self, commitdata: List[Tuple[LogCommitState, str]], revision: int):
        for logstate,commitfilepath in commitdata:
            # 必须过滤机器人提交
            if commitfilepath.endswith('.ini') and logstate.parent.author != 'jenkins':
                return True
        return False

    def run(self, commitdata: List[Tuple[LogCommitState, str]], revision: int):
        self.state = ObserverState.RUNNING
        try:
            noRobotCommitdata: List[Tuple[LogCommitState, str]] = []
            # 过滤机器人提交
            for logstate,commitfilepath in commitdata:
                if logstate.parent.author != 'jenkins':
                    noRobotCommitdata.append((logstate,commitfilepath))


            iniheader1 = '[CurrentIniVersion]'
            iniheader2 = ';Version是修改文件后自动提升的，请勿手动修改！！！'
            versionKey = 'Version'
            footer = ';******************************************\n'
            bot = Loader.获取脚本调试机器人()


            for logstate,commitfilepath in noRobotCommitdata:
                if not commitfilepath.endswith('.ini'):
                    continue

                localfilepath = svnRepoFilePath2LocalFilePath(self.data.svnurl,commitfilepath,self.data.projRoot)
                if not os.path.exists(localfilepath):
                    continue
                com.logout(f'[Inifile]{localfilepath}')
                com.convertEncoding2UTF8NOBOM(localfilepath)
                lines = com.readall(localfilepath).splitlines()
                # 没被修改
                if lines[0].strip() == iniheader1 and lines[1].strip() == iniheader2:
                    lastVersion = int(lines[2].strip().split('=')[1])
                    lines[2] = f'{versionKey}={lastVersion + 1}'
                    com.savelines(lines,localfilepath,divide=com.cmd_divide_linux)

                else:
                    # invaildcommitdata.append((logstate,commitfilepath))
                    lines.insert(0,iniheader1)
                    lines.insert(1,iniheader2)
                    lines.insert(2,f'{versionKey}=1')
                    lines.insert(3,footer)
                    com.savelines(lines,localfilepath,divide=com.cmd_divide_linux)

            SVNManager.commit(self.proj.configPath,'配置文件版本提升')
        except Exception as e:
            self.state = ObserverState.FAIL
            com.logout(e.__str__())
        else:
            self.state = ObserverState.SUCCESS
        # if invaildcommitdata.__len__() != 0:
        #     suspects = findSuspects(commitdata,invaildcommitdata)
        #     filepaths = '\n\n'.join([commitfilepath for logstate,commitfilepath in commitdata])
        #     content = f'{filepaths}\n\n以上文件被非法修改了配置文件版本\n\n嫌疑人：{"|".join(suspects)}'
        #     data = bot.build_markdown('配置文件非法修改',content)
        #     bot.send(data)







class UE4UnluaIntelliSenseGen(UE4Observer):
    def __init__(self, proj: UE4Project, workArgs, workMechine, data: CommitWorkData, dependences) -> None:
        super().__init__(proj, workArgs, workMechine, data, dependences)

        self.IntelliSenseSavePath = os.path.join(workdir,'IntelliSense')
    def isForceRun(self):
        return True
    def canRun(self,commitdata:List[Tuple[LogCommitState,str]],revision:int):
        # for commitstate,commitfilepath in commitdata:
        #     ext = os.path.splitext(commitfilepath)[-1]
        #     if ext in ('.h','.cpp'): 
        #         com.logout(f'[canRun] UE4UnluaIntelliSenseGen {True} {commitfilepath}  {ext}')
        #         return True
        # com.logout(f'[canRun] UE4UnluaIntelliSenseGen {False}')
        # return False
        # 依赖UHT
        return True
    @workspace
    def run(self, commitdata: List[Tuple[LogCommitState, str]], revision: int):
        cwd = os.getcwd()
        manifestFilePath = os.path.join(cwd,'1.xml')
        logFilePath = os.path.join(cwd,'log.log')
        # 生成StaticallyExports
        self.proj.commandLet('UnLuaIntelliSense',[],errException=Exception('UnLuaIntelliSense执行失败'))
        # 改这个得重编译
        # com.replace_filecontent(self.proj.unluaIntelliSenseBuildFilePath,'ENABLE_INTELLISENSE=0','ENABLE_INTELLISENSE=1')
        # 现在用这个标志了
        flagFilePath = os.path.join(self.proj.engine.UHTDir,'UnluaIntelliSense.txt')
        com.savedata('',flagFilePath)
        self.proj.buildGameModule(self.proj.editorModule,'Win64','Development',manifestFilePath,logFilePath,clean=False,onlyRunUHT=True)
        # 及时清理标志
        Path.ensure_pathnotexsits(flagFilePath)

        Path.ensure_svn_pathexsits(self.IntelliSenseSavePath,'https://192.168.2.12:8443/svn/Tenmove_Project_A8/automationData/IntelliSense_lua')
        # StaticallyExports这个文件夹保持不变
        rmlist = com.listdir_fullpath(self.IntelliSenseSavePath,lambda x : not x.endswith('.svn'))
        Path.ensure_pathnotexsits(*rmlist)
        com.combinefolder(self.IntelliSenseSavePath,self.proj.unluaIntelliSenseGenPath,self.proj.unluaStaticIntelliSenseGenPath,method='move')
        
        globMatch = os.path.join(self.IntelliSenseSavePath,'**','*.lua')
        for file in glob.glob(globMatch):
            filepath = file
            encoding = com.getEncoding(filepath,confidence=0.7,raiseErr=True)
            if encoding not in ('utf-8','ascii'):
                com.convertEncoding(filepath,encoding,'utf-8')
        SVNManager.add(self.IntelliSenseSavePath)
        SVNManager.commit(self.IntelliSenseSavePath,'提交lua智能提示',deleteMissing=True)









def getFTPPath(observerName,filename):
    basep = com.get_ftp_logsavepath('observers',f'ue4/{observerName}')
    return f'{basep}/{filename}'






# class DllCompile(UE4Observer):
#     def __init__(self, proj: UE4Project) -> None:
#         super().__init__(proj)
#     def run(self,commitdata:List[Tuple[LogCommitState,str]]):

#         pass


#     def afterRun(self,commitdata:List[Tuple[LogCommitState,str]]):
#         pass