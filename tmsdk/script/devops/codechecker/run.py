# -*- encoding: utf-8 -*-
import os,sys
thisdir = os.path.abspath(os.path.dirname(__file__))
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.comobj import *

from argparse import ArgumentParser
import shlex


class CodeCheckerAnalyzer:
    def __init__(self,configPath,ignorePath) -> None:
        self.configPath = configPath
        self.ignorePath = ignorePath
        self.compileDatabaseFilePath = os.path.join(workdir,'compile_database.json')
    def Init(self):
        pass
    def Analyze(self,reportDir):
        # --ctu需要原生clang的clang-extdef-mapping，ndk的clang是阉割版的
        # 别用--xxx=xxx 弱智codechecker不会处理
        com.cmd(f'CodeChecker analyze "{self.compileDatabaseFilePath}" --output "{reportDir}" --config "{self.configPath}" --ignore "{self.ignorePath}"',
        errException=Exception(f'analyze失败'))
    def Store(self,reportDir,name,url):
        # 别用--xxx=xxx 弱智codechecker不会处理
        com.cmd(f'CodeChecker store "{reportDir}" --name {name} --url {url} --config "{self.configPath}"',
        errException=Exception(f'store失败'))




class UE4Analyze(CodeCheckerAnalyzer):
    def __init__(self, configPath, ignorePath,projectPath,enginePath) -> None:
        super().__init__(configPath, ignorePath)

        self.project = UE4Project(enginePath,projectPath)

    def Init(self):
        manifest = 'manifest.xml'
        logp = 'log.log'
        targetPlatform = 'Android'
        self.project.SetAndroidBuildEnv()
        # 存在Program类型插件，不进行Editor构建并使用onlyRunUHT将会在游戏项目缺失UnrealHeaderTool.target
        # 生成rsp文件
        out,code = self.project.buildGameModule(self.project.gameModule,targetPlatform,'Development',manifestPath=manifest,rawLogPath=logp,clean=True,onlyRunUHT=False)
        if code != 0:
            raise Exception('rsp文件生成失败')
        # 获取所有rsp文件
        # responseFiles = glob.glob(os.path.join(self.project.androidCodeBuildPath,'**',f'*{self.project.engine.GetResponseFileExt(targetPlatform)}'),recursive=True)
        responseFiles = glob.glob(os.path.join(self.project.projectPath,'**','Intermediate','Build','Android','**',f'*{self.project.engine.GetResponseFileExt(targetPlatform)}'),recursive=True)
        # 清理rsp文件内的pch文件和双引号
        database = []
        sharePath = com.getPythonShareRoot()
        packageLayoutFilePath = com.getFirstPythonShareFile(os.path.join('codechecker','config','package_layout.json'))
        
        jf = JsonFile(packageLayoutFilePath)

        if targetPlatform.lower() == 'win64':
            raise Exception('不支持windows')
        else:
            env = Loader.getenvconf()
            clangPath = os.path.join(env.ndkpath,'toolchains','llvm','prebuilt','darwin-x86_64','bin','clang')

            for responseFile in responseFiles:
                Log.debug(f'处理 {responseFile}')
                # 去除双引号是因为codechecker没检查
                data = com.readall(responseFile).replace('"','')
                data_splited = shlex.split(data)
                try:
                    i = data_splited.index('-include-pch')
                    data_splited.pop(i)
                    data_splited.pop(i)
                except:
                    pass
                data_splited.insert(0,clangPath)
                if '.cpp' in responseFile:
                    cppFile = responseFile[:responseFile.index('.cpp') + 4]
                    database.append({
                        "file":cppFile,
                        "command":' '.join(data_splited),
                        "directory":self.project.engine.sourceDir
                    })
                else:
                    Log.info(f'跳过{responseFile}')
            # 将rsp文件生成compile_database.json
            com.dumpfile_json(database,self.compileDatabaseFilePath)
            # 替换clangsa和clang-tidy配置中的路径
            jf.setvalue('runtime','analyzers','clangsa',value=clangPath)
            jf.setvalue('runtime','analyzers','clang-tidy',value=clangPath+'-tidy')
        jf.save()
        

        
        # alpha检查器
        # # alpha.core.CallAndMessageUnInitRefArg 未初始化变量的传递
        # alpha.core.Conversion  有符号和无符号的隐式转换
        # c++:
        # alpha.cplusplus.EnumCastOutOfRange 检查数到枚举的转换是否超范围
        # alpha.cplusplus.InvalidatedIterator 检查非法迭代器
        # alpha.cplusplus.IteratorRange 检查迭代器范围
        # alpha.cplusplus.Move 报告std::move操作造成的内存拷贝
        # c：
        # alpha.security.ArrayBoundV2 数组越限操作
        # alpha.security.ReturnPtrRange 数组返回越限指针
        # alpha.unix.cstring.NotNullTerminated 检查strxxx()操作的参数是否为null
        checkerProfileFilePath = com.getFirstPythonShareFile(os.path.join('codechecker','config','checker_profile_map.json'))
        checkerjf = JsonFile(checkerProfileFilePath)
        checkerName:List[str] = checkerjf.trygetvalue('analyzers','clangsa','sensitive')
        rmlist = [x for x in checkerName if x.startswith('alpha')]
        com.removeList(checkerName,rmlist)
        checkerName = checkerName + [
            'alpha.core.Conversion',
            # 'alpha.cplusplus.EnumCastOutOfRange', 这东西会报错
            'alpha.cplusplus.InvalidatedIterator',
            'alpha.cplusplus.IteratorRange',
            'alpha.cplusplus.Move',
            'alpha.security.ArrayBoundV2',
            'alpha.security.ReturnPtrRange',
            'alpha.unix.cstring.NotNullTerminated'
        ]
        checkerjf.trysetvalue('analyzers','clangsa','sensitive',value=checkerName)
        checkerjf.save()
        



@workspace
def runUE4Analyze(args):

    programSvnUrl = args.svnUrl
    codecheckerServerUrl = args.serverUrl
    reportDir = os.path.join('report')

    configPath = os.path.join(thisdir,'config.json')
    ignorePath = os.path.join(thisdir,'ignore.txt')
    programPath = os.path.join(workdir,'programe')
    projectPath = os.path.join(programPath,Loader.获取相对客户端游戏工程路径().split(os.path.sep,1)[1])

    

    enginePath,engineSVNPath = UE4Engine.getBinEngineTrunkSVNPathAndWCPath()
    Path.ensure_svn_pathexsits(enginePath,engineSVNPath)
    Path.ensure_svn_pathexsits(programPath,programSvnUrl)

    revision = SVNManager.version(projectPath)

    if com.isWindows():
        raise Exception('不支持windows')
    elif com.isLinux():
        raise Exception('不支持linux')
    elif com.isMac():
        pass
    else:
        raise Exception('未知平台')
    analyzer = UE4Analyze(configPath,ignorePath,projectPath,enginePath)
    
    analyzer.Init()
    Log.info('开始分析')
    analyzer.Analyze(reportDir)
    Log.info('开始上传')
    analyzer.Store(reportDir,revision,codecheckerServerUrl)








def main(args_raw=None):
    parser = ArgumentParser()
    subparser = parser.add_subparsers(title='c++工程类型')
    ue4parser = subparser.add_parser('ue4')
    ue4parser.add_argument('--svnUrl',required=True,type=com.str2Noneable,help='ue4游戏工程的svn路径')
    ue4parser.add_argument('--serverUrl',required=True,type=com.str2Noneable,help='codechecker服务URL路径')

    ue4parser.set_defaults(func=runUE4Analyze)

    args =  parser.parse_args(args_raw)
    args.func(args)
    

if __name__ == '__main__':
    main()