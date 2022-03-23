# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com
from comlib.conf.loader import Loader
from comlib.conf.ref import *

from comlib.pathm import Path
from comlib.xmlm import XMLFile
from comlib.dictm import JsonFile,DictUtil
from comlib.wraps import workspace
from comlib.secm import SecretManager
from comlib.compilerm import CSBuilder
from comlib.binm import BinManager

from enum import Enum,unique

oldrepo = com.textwrap.indent(f'''
repositories {{
    google()
    jcenter()
}}
''',prefix='    ')
newrepo = com.textwrap.indent(f'''
repositories {{
    maven{{url "http://192.168.2.200:8081/repository/maven-release-central/"}}
    google()
    jcenter()
}}
''',prefix='    ')

oldgradleurl = r'https\://services.gradle.org/distributions'
# 内网文件
newgradleurl = r'http\://192.168.2.177/dnl/DevOps/gradle_bin/distributions'

@unique
class ExitCode(Enum):
	# // NOTE: this needs to be kept in sync with EditorAnalytics.h and iPhonePackager.cs
    Error_UATNotFound = -1,
    Success = 0,
    Error_Unknown = 1,
    Error_Arguments = 2,
    Error_UnknownCommand = 3,
    Error_SDKNotFound = 10,
    Error_ProvisionNotFound = 11,
    Error_CertificateNotFound = 12,
    Error_ProvisionAndCertificateNotFound = 13,
    Error_InfoPListNotFound = 14,
    Error_KeyNotFoundInPList = 15,
    Error_ProvisionExpired = 16,
    Error_CertificateExpired = 17,
    Error_CertificateProvisionMismatch = 18,
    Error_CodeUnsupported = 19,
    Error_PluginsUnsupported = 20,
    Error_UnknownCookFailure = 25,
    Error_UnknownDeployFailure = 26,
    Error_UnknownBuildFailure = 27,
    Error_UnknownPackageFailure = 28,
    Error_UnknownLaunchFailure = 29,
    Error_StageMissingFile = 30,
    Error_FailedToCreateIPA = 31,
    Error_FailedToCodeSign = 32,
    Error_DeviceBackupFailed = 33,
    Error_AppUninstallFailed = 34,
    Error_AppInstallFailed = 35,
    Error_AppNotFound = 36,
    Error_StubNotSignedCorrectly = 37,
    Error_IPAMissingInfoPList = 38,
    Error_DeleteFile = 39,
    Error_DeleteDirectory = 40,
    Error_CreateDirectory = 41,
    Error_CopyFile = 42,
    Error_OnlyOneObbFileSupported = 50,
    Error_FailureGettingPackageInfo = 51,
    Error_OnlyOneTargetConfigurationSupported = 52,
    Error_ObbNotFound = 53,
    Error_AndroidBuildToolsPathNotFound = 54,
    Error_NoApkSuitableForArchitecture = 55,
    Error_FilesInstallFailed = 56,
    Error_RemoteCertificatesNotFound = 57,
    Error_LauncherFailed = 100,
    Error_UATLaunchFailure = 101,
    Error_FailedToDeleteStagingDirectory = 102,
    Error_MissingExecutable = 103,
    Error_DeviceNotSetupForDevelopment = 150,
    Error_DeviceOSNewerThanSDK = 151,
    Error_TestFailure = 152,
    Error_SymbolizedSONotFound = 153,
    Error_LicenseNotAccepted = 154,
    Error_AndroidOBBError = 155,

# 和GlobalData.h同步
@unique
class ELogLevel(Enum):
    NotSet = -1
    LOG_INFO = 0
    LOG_DEBUG = 1
    LOG_WARN = 2
    LOG_ERROR = 3
    LOG_FATAL = 4

# 和GlobalData.h同步
@unique
class EGameStartMode(Enum):
    NotSet = -1
    登录 = 0
    城镇 = 1
    PVE = 2
    PVP = 3
    练习场 = 4
    translate = {
        'NotSet':'NotSet',
        '登录':'denglu',
        '城镇':'chengzhen',
        'PVE':'PVE',
        'PVP':'PVP',
        '练习场':'lianxichang'
        }

# 和GlobalData.h同步
@unique
class EJobType(Enum):
    NotSet = -1
    狂战 = 101
    浪人 = 102
    魔剑 = 103
    雷烬 = 104
    格斗 = 201
    气功 = 202
    柔道 = 203
    魔法师 = 301
    召唤师 = 302
    战斗法师 = 303
    translate = {
        'NotSet':'NotSet',
        '狂战':'kuangzhan',
        '浪人':'langren',
        '魔剑':'mojian',
        '雷烬':'leijin',
        '格斗':'gedou',
        '气功':'qigong',
        '柔道':'roudao',
        '魔法师':'mofashi',
        '召唤师':'zhaohuanshi',
        '战斗法师':'zhandoufashi'
        }


# 亲测这个路径有效
# C:\Users\tengmu\AppData\Roaming\Unreal Engine\UnrealBuildTool
class UE4Engine(object):
    '''
    https://docs.unrealengine.com/en-US/ProductionPipelines/BuildTools/UnrealBuildTool/BuildConfiguration/index.html

    In addition to being added to the generated UE4 project under the Config/UnrealBuildTool folder, 
    
    UnrealBuildTool reads settings from XML config files in the following locations on Windows:

    Engine/Saved/UnrealBuildTool/BuildConfiguration.xml

    User Folder/AppData/Roaming/Unreal Engine/UnrealBuildTool/BuildConfiguration.xml

    My Documents/Unreal Engine/UnrealBuildTool/BuildConfiguration.xml

    On Linux and Mac, the following paths are used instead:

    /Users//.config//Unreal Engine/UnrealBuildTool/BuildConfiguration.xml

    /Users//Unreal Engine/UnrealBuildTool/BuildConfiguration.xml

    
    '''
    def __init__(self,engineRoot) -> None:
        super().__init__()
        self.rawEngineFiles = []
        
        self.engineRoot = engineRoot

        self.allowFASTBuild = True  
        self.forceFASTBuild = True

        self.engineDir = os.path.join(self.engineRoot,'Engine')
        self.binariesDir = os.path.join(self.engineDir,'Binaries')
        self.sourceDir = os.path.join(self.engineDir,'Source')
        self.programsDir = os.path.join(self.engineDir,'Programs')
        
        self.buildDir = os.path.join(self.engineDir,'Build')
        self.batchfilesDir = os.path.join(self.buildDir,'BatchFiles')

        self.dotNetDir = os.path.join(self.binariesDir,'DotNet')
        self.UHTDir = os.path.join(self.programsDir,'UnrealHeaderTool')

        self.clFilterDir = os.path.join(self.buildDir,'Windows','cl-filter')
        self.versionFile = JsonFile(os.path.join(self.buildDir,'Build.version'))
        self.engineVersion = f'{self.versionFile.trygetvalue("MajorVersion")}.{self.versionFile.trygetvalue("MinorVersion")}'


        self.engineAndroidProject = os.path.join(self.buildDir,'Android','Java','gradle')
        self.skipGradleBuildFileFlagPath = os.path.join(self.engineAndroidProject,'app.iml')
        self.engineBuildGradleFilePath = os.path.join(self.engineAndroidProject,'build.gradle')
        self.gradleWrapperPropFilePath = os.path.join(self.engineAndroidProject,'gradle','wrapper','gradle-wrapper.properties')


        self.UATPath = os.path.join(self.dotNetDir,'AutomationTool.exe')
        self.UBTPath = os.path.join(self.dotNetDir,'UnrealBuildTool.exe')
        self.IPPPath = os.path.join(self.dotNetDir,'IOS','IPhonePackager.exe')
        self.RunMonoSh = os.path.join(self.batchfilesDir,'Mac','RunMono.sh')



        if sys.platform == 'win32':
            self.editorPath = os.path.join(self.binariesDir,'Win64','UE4Editor-Cmd.exe')
            self.runUATPath = os.path.join(self.batchfilesDir,'RunUAT.bat')
            self.mechinePlatform = 'Win64'
            self.registEngineScript = os.path.join(self.engineRoot,'双击注册安装版引擎.bat')

        elif sys.platform == 'darwin':
            self.editorPath = os.path.join(self.binariesDir,'Mac','UE4Editor-Cmd')
            self.runUATPath = os.path.join(self.batchfilesDir,'RunUAT.sh')
            self.mechinePlatform = 'Mac'
            self.registEngineScript = os.path.join(self.engineDir,'双击注册安装版引擎.sh')
            self.UHTPath = os.path.join(self.binariesDir,'Mac','UnrealHeaderTool')

            com.make_executable(self.editorPath)
            com.make_executable(self.UHTPath)
        else:
            raise Exception(f'不支持平台{sys.platform}')


        # 这些是修bug的路径
        __InWorldBlueprintEditingPath = os.path.join(self.engineDir,'Content','Tutorial','InWorldBlueprintEditing')
    def _fixPlatformStr(self,platform:str):
        if platform.lower() == 'android':
            return 'Android'
        elif platform.lower() == 'ios':
            return 'IOS'
        elif platform.lower() == 'win64':
            return 'Win64'
        else:
            return platform
    def GetResponseFileExt(self,targetPlatform:str):
        targetPlatform = targetPlatform.lower()
        if targetPlatform == 'win64':
            return '.response'
        return '.rsp'
    def runUATCmd(self,cmd):
        cmd = f'{self.runUATPath} {cmd} -nop4 -noxge'
        return com.cmd(cmd,getstdout=False,errException=Exception(f'UAT命令执行失败\nCMD:\n{cmd}'))

    def setBuildConfiguration(self):
        '''
        以后加可选配置
        '''
        p = r'C:\Users\tengmu\AppData\Roaming\Unreal Engine\UnrealBuildTool\BuildConfiguration.xml'
        
        roamingpath = Path.getPath_Roaming()
        buildConfFilePath = os.path.join(roamingpath,'Unreal Engine','UnrealBuildTool','BuildConfiguration.xml')
        Path.ensure_direxsits(os.path.dirname(buildConfFilePath))


        basecontent = '''<?xml version="1.0" encoding="utf-8" ?>
        <Configuration xmlns="https://www.unrealengine.com/BuildConfiguration">
        </Configuration>
        '''
        xmlf = XMLFile(basecontent)
        bce = xmlf.find(xmlf.root,'BuildConfiguration')
        if bce == None:
            bce = xmlf.add(xmlf.root,'BuildConfiguration')
        xmlf.add(bce,'bAllowXGE','false')
        if com.dotstrcompare(self.engineVersion,'4.26'):
            xmlf.add(bce,'bAllowFASTBuild',com.bool2lowerstr(False))
            xmlf.add(bce,'bForceFASTBuild',com.bool2lowerstr(False))
            # xmlf.add(bce,'bAllowFASTBuild','false')
        else:
            xmlf.add(bce,'bAllowFASTBuild',com.bool2lowerstr(self.allowFASTBuild))

            pass
        
        xmlf.save(buildConfFilePath)

    def setSkipGradleBuildFlag(self):
        '''
        4.22跳过gradle构建方法
        '''
        com.savedata('',self.skipGradleBuildFileFlagPath)
    def revertSkipGradleBuildFlag(self):
        os.remove(self.skipGradleBuildFileFlagPath)
    def useLocalMaven(self):        
        com.replace_filecontent(self.engineBuildGradleFilePath,oldrepo,newrepo)
    def revertLocalMaven(self):
        com.replace_filecontent(self.engineBuildGradleFilePath,newrepo,oldrepo)
    def useLocalGradleUrl(self):
        '''
        使用内网gradle下载地址
        '''
        com.replace_filecontent(self.gradleWrapperPropFilePath,oldgradleurl,newgradleurl)
    def revertLocalGradleUrl(self):
        '''
        回滚使用内网gradle下载地址
        '''
        com.replace_filecontent(self.gradleWrapperPropFilePath,newgradleurl,oldgradleurl)
    def useLocalEnv(self):
        com.logout(f'[useLocalEnv]')
        self.useLocalMaven()
        self.useLocalGradleUrl()
    def revertLocalEnv(self):
        self.revertLocalMaven()
        self.revertLocalGradleUrl()
    def Fix_UE4D25_MacBuild_dYSMNotFound(self):
        '''
        修复在mac系统上进行打包，ubt会预先编译工程生成mac系统的.dylib库，该.dylib库在development模式下没有生成dSYM文件报错的bug
        '''
        # //4.26 fix dysm error
        # bool IsBuildMachine = Environment.GetEnvironmentVariable("IsBuildMachine") == "1";
        # // If the user explicitly provided an option for dSYM's then do that. If they did not, then we want one for shipping builds or if we're a build machine
        # //bool WantDsym = Target.MacPlatform.bGenerateDsymFile ?? (Target.Configuration == UnrealTargetConfiguration.Shipping || IsBuildMachine);

        # //my fix 1
        # //bool WantDsym = Target.MacPlatform.bGenerateDsymFile;
        # //my fix 2
        # bool WantDsym = Target.Configuration == UnrealTargetConfiguration.Shipping && Platform == UnrealTargetPlatform.Mac && Target.MacPlatform.bGenerateDsymFile;

        # Target.bUsePDBFiles = !Target.bDisableDebugInfo && WantDsym;
        
        # // 4.25 not fix dysm error
        # //Target.bUsePDBFiles = !Target.bDisableDebugInfo && Target.Configuration != UnrealTargetConfiguration.Debug && Platform == UnrealTargetPlatform.Mac && Target.MacPlatform.bGenerateDsymFile;
        okUBT = os.path.join(thisdir,'extfiles','ue4fix',self.engineVersion,'UBT')
        okCLFilter = os.path.join(thisdir,'extfiles','ue4fix',self.engineVersion,'cl-filter')
        combineFiles,coverFiles,combineDirs = com.combinefolder(self.dotNetDir,okUBT,debug=False,cover=False,dirfilters=['.svn',os.path.join('AutomationScripts','.svn'),os.path.join('AutomationScripts','Android','.svn')])
        self.rawEngineFiles += com.safeCoverFiles(coverFiles)
        combineFiles,coverFiles,combineDirs = com.combinefolder(self.clFilterDir,okCLFilter,debug=False,cover=False,dirfilters=['.svn',os.path.join('AutomationScripts','.svn'),os.path.join('AutomationScripts','Android','.svn')])
        self.rawEngineFiles += com.safeCoverFiles(coverFiles)
    def rollbackRawEngineFiles(self):
        com.rollbackCoverFiles(self.rawEngineFiles)
    @workspace
    def cmd(self,cmdstr,localEnv=True,errException=Exception(f'UE4命令执行失败'),**kv):
        # self.setSkipGradleBuildFlag()
        # 打包机加环境变量
        # bool bIsBuildMachine = Environment.GetEnvironmentVariable("IsBuildMachine") == "1";
        os.putenv("IsBuildMachine", "1")

        self.setBuildConfiguration()



        # cmdstr += ' -BuildMachine'
        self.revertLocalEnv()        
        if localEnv:
            self.useLocalEnv()
        
        self.Fix_UE4D25_MacBuild_dYSMNotFound()

        try:
            out,code = com.cmd(cmdstr,errException=errException,**kv)
        except Exception as e:
            raise e
        finally:
            self.rollbackRawEngineFiles()
            if localEnv:
                self.revertLocalEnv()
        return out,code
    def dotNetCmd(self,cmdstr,localEnv=True,errException=Exception(f'dotNet命令执行失败'),**kv):
        '''
        使用mono跨平台
        '''
        if not com.isWindows():
            # cmdstr = f'/bin/sh "{self.RunMonoSh}" {cmdstr}'
            cmdstr = f'mono {cmdstr}'
        return self.cmd(cmdstr,localEnv=localEnv,errException=errException,**kv)

    def ubtCmd(self,cmdstr,localEnv=True,errException=Exception(f'UBT命令执行失败'),**kv):
        cmd = f'"{self.UBTPath}" {cmdstr} -ForceFASTBuild'
        return self.dotNetCmd(cmd,localEnv=localEnv,errException=errException,**kv)
    def commandLet(self,projFilePath,cmdLetName,params:list,logSavePath=None,showVerbosity=False,errException=None,**kv):
        if logSavePath == None:
            logSavePath = com.get_logfile_path(f'{cmdLetName}.log')

        cmd = f'"{self.editorPath}" "{projFilePath}" -run={cmdLetName} {" ".join(params)} -abslog="{logSavePath}"'
        cmd += ' -stdout -NoLogTimes -buildmachine -noxgecontroller'
        if showVerbosity:
            cmd += ' -AllowStdOutLogVerbosity'
        out,code = self.cmd(cmd,errException=errException,**kv)
        return out,code,logSavePath
    @workspace
    def configModeCmd(self,targetPlat,projPath,doFileDict):
        base = {
            "engineFile": None,
            "defaultFile":None,
            "platformFile":None,
            "userFile":None,
            "localFile":None
        }
        dataStruct = DictUtil.combine(doFileDict,base)
        # "E:\softwave\UE_4.25\Engine\Binaries\DotNet\UnrealBuildTool.exe" Config -targetPlatform=Android -projectPath=D:\_workspace\_project\A8\Program\Client\NextGenGame -doFile="D:\_workspace\_project\newsdk\script\devops\4358431160708162617\dofile.json"
        logpath = com.get_logfile_path('ubtconfig.log')
        doFilePath = com.get_logfile_path('dofile.json')
        com.dumpfile_json(dataStruct,doFilePath)
        cmd = f'-Config -targetPlatform={targetPlat} -doFile="{doFilePath}" -log={logpath}'
        if projPath != None:
            cmd += f' -projectPath={projPath}'
        return self.ubtCmd(cmd)

    def registEngine(self):
        if os.path.exists(self.registEngineScript):
            com.cmd(self.registEngineScript)
            # TODO 先写死，看看引擎有没有字段
            return f'TM{self.engineVersion}'
        else:
            return f'{self.engineVersion}'


    def EnsureIPP(self):
        if not os.path.exists(self.IPPPath):
            # 用msbuild 生成ipp
            csprojFilePath = os.path.join(self.sourceDir,'Programs','IOS','iPhonePackager','iPhonePackager.csproj')
            csb = CSBuilder(csprojFilePath)
            out,logfile,code = csb.build('Development')
            if code != 0:
                raise Exception(f'IPP 编译失败')

    def InstallIOSMPFile(self,mpFilePath,bundleName):
        self.EnsureIPP()
        cmd = f'{self.IPPPath} Install Engine -provision "{mpFilePath}" -bundlename "{bundleName}"'
        out,code = self.dotNetCmd(cmd,localEnv=False)

    def InstallCert(self,certFilePath,bundleName):
        self.EnsureIPP()
        cmd = f'{self.IPPPath} Install Engine -certificate "{certFilePath}" -bundlename "{bundleName}"'
        out,code = self.dotNetCmd(cmd,localEnv=False)
    def SetIOSEnv(self,channelcf:ioschannelconf,versionName='',mode='Development',uprojectFile=None):
        remoteServerName = '192.168.2.176'

        # 获取远程打包机ssh私钥
        sshPrivateKeyFilePath = SecretManager.getSSHPrivateKey(remoteServerName)
        # 获取证书和MP文件
        mpFilePath = SecretManager.getSecKey('ios',channelcf.keystorename + '.mobileprovision')
        certFilePath = SecretManager.getSecKey('ios',channelcf.keystorename + '.p12')

        self.SetIOSCert(mpFilePath,certFilePath,channelcf.applicationid,mode)
        modFile = 'engineFile' if uprojectFile == None else 'defaultFile'
        
        dofile = {
            modFile: {
                "type": "engine",
                "data": [
                    {
                        "ConfigLineAction": "Set",
                        "section": "/Script/IOSRuntimeSettings.IOSRuntimeSettings",
                        "key": "VersionInfo",
                        "value": versionName
                    },
                    {
                        "ConfigLineAction": "Set",
                        "section": "/Script/IOSRuntimeSettings.IOSRuntimeSettings",
                        "key": "SSHPrivateKeyOverridePath",
                        "value": sshPrivateKeyFilePath
                    },
                    {
                        "ConfigLineAction": "Set",
                        "section": "/Script/IOSRuntimeSettings.IOSRuntimeSettings",
                        "key": "RemoteServerName",
                        "value": remoteServerName
                    },
                    {
                        "ConfigLineAction": "Set",
                        "section": "/Script/IOSRuntimeSettings.IOSRuntimeSettings",
                        "key": "BundleDisplayName",
                        "value": channelcf.applicationname
                    },
                    {
                        "ConfigLineAction": "Set",
                        "section": "/Script/IOSRuntimeSettings.IOSRuntimeSettings",
                        "key": "BundleName",
                        "value": channelcf.applicationname
                    },
                    {
                        "ConfigLineAction": "Set",
                        "section": "/Script/IOSRuntimeSettings.IOSRuntimeSettings",
                        "key": "BundleIdentifier",
                        "value": channelcf.applicationid
                    },
                    {
                        "ConfigLineAction": "Set",
                        "section": "/Script/IOSRuntimeSettings.IOSRuntimeSettings",
                        "key": "MobileProvision",
                        "value": os.path.basename(mpFilePath)
                    },
                    {
                        "ConfigLineAction": "Set",
                        "section": "/Script/IOSRuntimeSettings.IOSRuntimeSettings",
                        "key": "SigningCertificate",
                        "value": com.getX509CertFriendlyName(certFilePath)
                    }
                ]}
        }
        self.configModeCmd('ios', uprojectFile, dofile)

    

    def SetIOSCert(self,mpFilePath,certFilePath,bundleName,mode='Development'):
        '''
        返回MP文件和证书路径
        '''
        if com.isWindows():
            basePath = os.getenv('LOCALAPPDATA')
            mpFileSaveDir = f'{basePath}\\Apple Computer\\MobileDevice\\Provisioning Profiles'
        elif com.isMac():
            basePath = os.getenv('HOME')
            mpFileSaveDir = f'{basePath}/Library/MobileDevice/Provisioning Profiles'

        Path.ensure_direxsits(mpFileSaveDir)

        self.InstallCert(certFilePath,bundleName)
        self.InstallIOSMPFile(mpFilePath,bundleName)



        targetMPFileName = os.path.basename(mpFilePath)
        installedMPFilePath = os.path.join(mpFileSaveDir,targetMPFileName)
        if not os.path.exists(installedMPFilePath):
            # Dis
            targetMPFileName = f'Distro_{os.path.basename(mpFilePath)}'
            installedMPFilePath = os.path.join(mpFileSaveDir,targetMPFileName)

        assert os.path.exists(installedMPFilePath)

        # certDir = os.path.join(self.intermediatePath,'Remote',self.gameModule,'IOS',mode)

        # projMPFilePath = os.path.join(certDir,f'{targetMPFileName}')
        # projCertFilePath = os.path.join(certDir,'Certificate.p12')
        # Path.ensure_pathnotexsits(certDir)
        # Path.ensure_pathnewest(mpFilePath,projMPFilePath)
        # Path.ensure_pathnewest(certFilePath,projCertFilePath)

        # # string CertificateInfoContents = String.Format("{0}\n{1}", ProvisioningData.MobileProvisionFile, FileReference.GetLastWriteTimeUtc(ProvisioningData.MobileProvisionFile).Ticks);
        # ticks,code = com.cmd(f'powershell -c "[System.IO.File]::GetLastWriteTimeUtc(\'{installedMPFilePath}\').Ticks"')
        # ticks = ticks.strip()
        # com.savedata(f'{installedMPFilePath}\n{ticks}',os.path.join(certDir,'Certificate.txt'),newline='\n')

        # return projMPFilePath,projCertFilePath



    @staticmethod
    def getDefaultEngineSavePath():
        engineSavePath = ''
        if com.isWindows():
            engineSavePath = 'C:\\UE'
        elif com.isMac():
            # mac :mkdir: /UE: Read-only file system
            engineSavePath = '/Applications/UE'
        elif com.isLinux():
            # mkdir: 无法创建目录"/UE": 权限不够
            engineSavePath = '/UE'
            # 默认免密了
            com.cmd('sudo mkdir /UE')
        return engineSavePath
    @staticmethod
    def getEngineWCPath(branchName,installed=True,engineSavePath=None):
        '''
        branchName获取可用从branchconf获取，这个是主干的branchName。也可以通过SVNWorkflow获取，这个可以拿到其他分支的引擎
        '''
        if com.isNoneOrEmpty(engineSavePath):
            engineSavePath = UE4Engine.getDefaultEngineSavePath()
        assert not com.isNoneOrEmpty(engineSavePath)

        Path.ensure_direxsits(engineSavePath)
        workingCopyDirName = f'WC_{branchName}_{"I" if installed else "S"}'
        workingCopyDir = os.path.join(engineSavePath,workingCopyDirName)
        return workingCopyDir
    @staticmethod
    def getEngineSVNPath(engineUrl,installed=True):
        '''
        installed=True =>安装版
        installed=False =>轻量版
        '''
        engineTypeName = 'installed' if installed else 'light'
        return f'{engineUrl}/{engineTypeName}'
    @staticmethod
    def getBinEngineTrunkSVNPathAndWCPath():
        '''
        读取配置获取二进制引擎主干svn路径和本地储存路径
        '''
        from comlib.workflowm import SVNWorkflow
        enginec = Loader.load(engineconf)
        binEngineWorkflow = SVNWorkflow(enginec.binRepoUrl,enginec.branchName)
        engineWCPath = UE4Engine.getEngineWCPath(enginec.branchName,True)
        engineSVNPath = UE4Engine.getEngineSVNPath(binEngineWorkflow.trunkUrl)
        return engineWCPath,engineSVNPath

    @staticmethod
    def getLightEngineWCPath(branchName,engineSavePath=None):
        if com.isNoneOrEmpty(engineSavePath):
            engineSavePath = UE4Engine.getDefaultEngineSavePath()
        assert not com.isNoneOrEmpty(engineSavePath)

        Path.ensure_direxsits(engineSavePath)
        workingCopyDirName = f'WC_{branchName}_L'
        workingCopyDir = os.path.join(engineSavePath,workingCopyDirName)
        return workingCopyDir

class UE4Project(object):
    def __init__(self,enginePath,projectPath) -> None:
        super().__init__()

        # self.isEngineProject = False
        # if enginePath == projectPath:
        #     self.isEngineProject = True
        
        self.projectPath = projectPath

        
        
        self.uprojectfilePath = com.listdir_fullpath(self.projectPath,lambda x:x.endswith('.uproject'))[0]
        self.projectName = os.path.basename(self.uprojectfilePath).replace('.uproject','')

        # self.platform = platform

        # 工程路径
        self.binariesPath = os.path.join(self.projectPath,'Binaries')
        self.buildPath = os.path.join(self.projectPath,'Build')
        self.configPath = os.path.join(self.projectPath,'Config')
        self.contentPath = os.path.join(self.projectPath,'Content')
        self.intermediatePath = os.path.join(self.projectPath,'Intermediate')
        self.savedPath = os.path.join(self.projectPath,'Saved')
        self.pluginsPath = os.path.join(self.projectPath,'Plugins')
        self.sourcePath = os.path.join(self.projectPath,'Source')
        self.codeBuildPath = os.path.join(self.intermediatePath,'Build')
        self.androidCodeBuildPath = os.path.join(self.codeBuildPath,'Android')
        

        # 游戏相关目录
        self.unluaPath = os.path.join(self.pluginsPath,'ThirdParty','Unlua')
        self.unluaIntelliSenseBuildFilePath = os.path.join(self.unluaPath,'Source','UnLuaIntelliSense','UnLuaIntelliSense.Build.cs')
        self.unluaIntelliSenseGenPath = os.path.join(self.unluaPath,'Intermediate','IntelliSense')
        self.unluaStaticIntelliSenseGenPath = os.path.join(self.pluginsPath,'Unlua','Intermediate','IntelliSense')
        
        # 配置文件
        self.defaultEngineIni = os.path.join(self.configPath,'DefaultEngine.ini')
        # 这个配置放在工程内只有编辑器状态才能预读到，命令行不会读取这里
        # eg：
        # ConfigHierarchy configCacheIni = ConfigCache.ReadHierarchy(ConfigHierarchyType.Engine, (DirectoryReference)null, BuildHostPlatform.Current.Platform);
        # self.userEngineIni = os.path.join(self.configPath,'UserEngine.ini') # 配置本地环境用的 例如安卓sdk路径
        localDataPath = Path.getPath_LocalData()
        relPath = os.path.join('Unreal Engine','Engine','Config','UserEngine.ini')
        self.userEngineIni = com.getvalue4plat(os.path.join(localDataPath,relPath),os.path.join(localDataPath,'Epic',relPath))

        # android
        self.skipGradleBuild = False

        # 获取所有游戏模块
        self.modules = list(map(lambda x: x.replace('.Target.cs',''),os.listdir(self.sourcePath)))
        self.editorModule = None
        self.gameModule = None
        for m in self.modules:
            if m.endswith('Editor'):
                self.editorModule = m
                break
        assert self.editorModule != None
        # 去除Editor
        self.gameModule = self.editorModule[:self.editorModule.__len__() - 'Editor'.__len__()]

        # file： *.Build.cs
        self.projBuildCs = os.path.join(self.sourcePath, self.gameModule, self.gameModule+".Build.cs")        
        # file： *.Target.cs
        self.projTargetCs = os.path.join(self.sourcePath, self.gameModule + ".Target.cs")
        # file： *Editor.Target.cs
        self.projEditorTargetCs = os.path.join(self.sourcePath, self.gameModule + "Editor.Target.cs")

        # 初始化引擎
        self.engine = UE4Engine(enginePath)
    def registEngine(self):
        name = self.engine.registEngine()
        com.logout(f'!!注册引擎{name}!!')
        # 修改uproject文件到注册引擎
        uprojFile = JsonFile(self.uprojectfilePath)
        uprojFile.trysetvalue('EngineAssociation',value=name)
        uprojFile.save()
        

    #打包和编译前设置宏（仅本次有效）
    def SetMacrosPreBuild(self, macros:list):
        
        if macros.__len__() == 0:
            com.logout(f'### 预设宏为空')
            return
        Path.ensure_pathexsits_nocopy(self.projTargetCs)
        Path.ensure_pathexsits_nocopy(self.projEditorTargetCs)
        # 格式： "a","b"
        macroStr = '"' + '","'.join(macros) + '"'
        com.logout(f'### 预设宏为：{" | ".join(macros)}')
        com.replace_filecontent(self.projTargetCs,'/*打包脚本宏占位符*/',macroStr)
        com.replace_filecontent(self.projEditorTargetCs,'/*打包脚本宏占位符*/',macroStr)
    

    def getPluginBinariesDirs(self):
        pluginBinariesDirs = []
        # plugins层级最多2层，搜索Binaries只搜索2层
        pluginNames = os.listdir(self.pluginsPath)
        for pluginName in pluginNames:
            pluginDir = os.path.join(self.pluginsPath,pluginName)
            pluginBinaries = os.path.join(pluginDir,'Binaries')
            if os.path.exists(pluginBinaries):
                pluginBinariesDirs.append(pluginBinaries)
                continue
            
            pluginModules = os.listdir(pluginDir)
            for pluginModule in pluginModules:
                pluginModuleBinaries = os.path.join(pluginDir,pluginModule,'Binaries')
                if os.path.exists(pluginModuleBinaries):
                    pluginBinariesDirs.append(pluginModuleBinaries)
        return pluginBinariesDirs
    def SetAndroidBuildEnv(self,sdkApiLevel='latest',ndkApiLevel='21'):
        env = Loader.getenvconf()
        android_env = com.textwrap.dedent(f'''
        [/Script/AndroidPlatformEditor.AndroidSDKSettings]
        SDKPath=(Path="{env.androidsdkpath}")
        NDKPath=(Path="{env.ndkpath}")
        JavaPath=(Path="{env.androidjdkpath}")
        SDKAPILevel={sdkApiLevel}
        NDKAPILevel=android-{ndkApiLevel}
        ''')
        Path.ensure_direxsits(os.path.dirname(self.userEngineIni))
        # 不允许打包机本地设置了
        com.savedata(android_env,self.userEngineIni)

    def SetAndroidPackageEnv(self,conf:channelconf,versionName,versionCode,bPackageDataInsideApk,sdkApiLevel='latest',ndkApiLevel='21'):
        '''
        path="Game/Config/UserEngine.ini"\n
        [/Script/AndroidPlatformEditor.AndroidSDKSettings]
        SDKPath=(Path="E:/tool_build/android_sdk")
        NDKPath=(Path="E:/tool_build/android-ndk-r21b-windows-x86_64/android-ndk-r21b")
        JavaPath=(Path="C:/Program Files/Java/jdk1.8.0_271")
        SDKAPILevel=latest
        NDKAPILevel=android-22
        '''
        # TODO sdk版本和ndk版本设置
        self.SetAndroidBuildEnv(sdkApiLevel,ndkApiLevel)
        keystorenameWithExt = conf.keystorename + '.keystore'
        keystorePath = SecretManager.getSecKey('android',keystorenameWithExt)
        keystoreConf = SecretManager.getSecKeyConf('android',conf.keystorename)
        Path.ensure_pathnewest(keystorePath,os.path.join(self.buildPath,'Android',keystorenameWithExt))

        dofile = {
            "defaultFile": {
                "type": "engine",
                "data": [
                    {
                        "ConfigLineAction": "Set",
                        "section": "/Script/AndroidRuntimeSettings.AndroidRuntimeSettings",
                        "key": "VersionDisplayName",
                        "value": versionName.__str__()
                    },
                    {
                        "ConfigLineAction": "Set",
                        "section": "/Script/AndroidRuntimeSettings.AndroidRuntimeSettings",
                        "key": "StoreVersion",
                        "value": f"{versionCode}"
                    },
                    {
                        "ConfigLineAction": "Set",
                        "section": "/Script/AndroidRuntimeSettings.AndroidRuntimeSettings",
                        "key": "bPackageDataInsideApk",
                        "value": f"{bPackageDataInsideApk}"
                    },
                    {
                        "ConfigLineAction": "Set",
                        "section": "/Script/AndroidRuntimeSettings.AndroidRuntimeSettings",
                        "key": "PackageName",
                        "value": conf.applicationid
                    },
                    {
                        "ConfigLineAction": "Set",
                        "section": "/Script/AndroidRuntimeSettings.AndroidRuntimeSettings",
                        "key": "ApplicationDisplayName",
                        "value": conf.applicationname
                    },
                    {
                        "ConfigLineAction": "Set",
                        "section": "/Script/AndroidRuntimeSettings.AndroidRuntimeSettings",
                        "key": "KeyStore",
                        "value": keystorenameWithExt
                    },
                    {
                        "ConfigLineAction": "Set",
                        "section": "/Script/AndroidRuntimeSettings.AndroidRuntimeSettings",
                        "key": "KeyStorePassword",
                        "value": keystoreConf.trygetvalue('storepass')
                    },
                    {
                        "ConfigLineAction": "Set",
                        "section": "/Script/AndroidRuntimeSettings.AndroidRuntimeSettings",
                        "key": "KeyAlias",
                        "value": keystoreConf.trygetvalue('alias')
                    },
                    {
                        "ConfigLineAction": "Set",
                        "section": "/Script/AndroidRuntimeSettings.AndroidRuntimeSettings",
                        "key": "KeyPassword",
                        "value": keystoreConf.trygetvalue('keypass')
                    }

                ]},

        }
        self.configMode('android',dofile)



        pass
    def SetIOSEnv(self,channelcf:ioschannelconf,versionName,mode='Development'):
        self.engine.SetIOSEnv(channelcf,versionName,mode,self.projectPath)


    def SetWindowsEnv(self,versionName,mode='Development'):
        
        pass
    def SetObbEnable(self):
        # self.defaultEngineIni文件的[/Script/AndroidRuntimeSettings.AndroidRuntimeSettings]的bPackageDataInsideApk=True  ->关闭obb

        import configparser
        pass


    def getNativePorjectPath(self,platform:str):
        """
        获取原生工程路径（android和xcode工程）
        """
        fixedPlatform = self.engine._fixPlatformStr(platform)
        if fixedPlatform == 'Android':
            # \Intermediate\Android
            tmp = os.path.join(self.intermediatePath,fixedPlatform)
            # \Intermediate\Android\APK
            tmp2 = com.findone(tmp,lambda x: os.path.isdir(x))
            # \Intermediate\Android\APK\gradle
            projectPath = os.path.join(tmp2,'gradle')
        else:
            raise Exception(f'不支持平台{platform}')
        return projectPath
    def getPackagePath(self,platform:str):
        fixedPlatform = self.engine._fixPlatformStr(platform)
        if fixedPlatform == 'Android':
            packagedir = os.path.join(self.binariesPath,fixedPlatform)
            packagePath = com.listdir_fullpath(packagedir,lambda x : x.endswith('.apk'))[0]
        elif fixedPlatform == 'IOS':
            packagedir = os.path.join(self.binariesPath,fixedPlatform)
            packagePath = com.listdir_fullpath(packagedir,lambda x : x.endswith('.ipa'))[0]
            pass
        elif fixedPlatform == 'Win64':
            packagePath = os.path.join(self.savedPath,'StagedBuilds','WindowsNoEditor')
        else:
            raise Exception(f'不支持平台{platform}')
        return packagePath
    # def build(self,paramfile):
    
    def configMode(self,targetPlat,doFileDict):
        return self.engine.configModeCmd(targetPlat,self.projectPath,doFileDict)

    def LightmassBake(self,quality='Production',bIgnoreChangelist=True):
        params = ['-BuildHLOD','-buildlighting','-AllowCommandletRendering']
        params.append(f'-Quality={quality}')
        if bIgnoreChangelist:
            params.append(f'-IgnoreChangelist')
        logpath = com.get_logfile_path('lightmass.log')
        # 无视大小写
        # else if (QualityStr.Equals(TEXT("Production"), ESearchCase::IgnoreCase))
        return self.commandLet('ResavePackages',params,logpath,showVerbosity=True,errException=None)
    def FixupRedirectors(self):
        logpath = com.get_logfile_path('FixupRedirectors.log')
        parames = ['-fixupredirects','-projectonly','-unattended']
        return self.commandLet('ResavePackages',parames,logpath,showVerbosity=True,errException=None)
    def TMConfigCommandLet(self,startMode:EGameStartMode,jobType:EJobType,battleRecord:bool,logLevel:ELogLevel,skillNoCD:bool,useBattlePreload:bool,internalTest:bool):
        logpath = com.get_logfile_path('TMConfig.log')
        parames = [f'-startMode={startMode.value}',
        f'-jobType={jobType.value} -battleRecord={battleRecord} -logLevel={logLevel.value} -skillNoCD={skillNoCD}',
        f'-useBattlePreload={useBattlePreload} -internalTest={internalTest} -standaloneGame=true']

        return self.commandLet('TM',parames,logpath,showVerbosity=True,errException=Exception(f"TMConfig执行失败,{parames}"))
    def commandLet(self,cmdLetName,params:list,logSavePath=None,showVerbosity=False,errException=None,**kv):
        return self.engine.commandLet(self.uprojectfilePath,cmdLetName,params,logSavePath,showVerbosity,errException,**kv)




    def cook(self,platform,iterate=True,cookFlavor=None):
        '''
        单纯cook，不进行pak
        '''
        targetPlatform = platform
        if cookFlavor != None:
            targetPlatform += f'_{cookFlavor}'
        parames = ['-unversioned','-buildmachine',
        '-skipeditorcontent',f'-TargetPlatform={targetPlatform}']
        if iterate:
            parames.append('-iterate -iterateshash')


        # allmaps不用声明了，不传任意map和mapIniSection默认会是allmaps
        # if (MapList.Num() == 0 && MapIniSections.Num() == 0)
        # {
        #     MapIniSections.Add(FString(TEXT("AllMaps")));
        # }

        return self.commandLet('cook',parames,showVerbosity=True,errException=Exception(f'{targetPlatform} cook失败'))



    def build(self,platform:str,rebuildRes,mode,cookFlavor,isDis=False,noXGE=True):
        # 服务器cook暂时不搞
        # ,stageOutputDir,cookOutputDir
        # BuildCookRun
        # -project=Path -noP4 -targetplatform（或者-Platform）=PlatformName -pak (-skippak)  -UTF8Output 
        # -build -clientconfig=Development 
        # -cook -ue4exe=ExecutableName -CookFlavor=Multi/ATC/DXT/ETC1/ETC2/PVRTC/ASTC (-MapsToCook=map1+map2+map3)|(-allmaps) (-Compressed) -SkipCookingEditorContent -CookOutputDir=C:\cooked (-FastCook) (-iterativecooking)
        
        
        # -nocompile是不编译 UAT.exe -NoCompile是纯蓝图项目跳过编译


        cmdstr = ''
        if not com.isWindows():
            cmdstr += 'mono '
        cmdstr += f'"{self.engine.UATPath}" BuildCookRun '
        
        if noXGE:
            cmdstr += '-NoXGE '
        # 暂时没有windows签名证书，有了再删掉改成-CodeSign
        if com.isWindows():
            cmdstr += '-NoCodeSign '
        


        # 工程设置
        cmdstr += f'-ScriptsForProject="{self.uprojectfilePath}" -project="{self.uprojectfilePath}" -noP4 -targetplatform={platform} -UTF8Output -pak '
        
        if not rebuildRes:
            cmdstr += '-skippak '
        # 构建设置
        cmdstr += f'-build -clientconfig={mode} '
        # cook设置
        cmdstr += f'-cook -ue4exe="{self.engine.editorPath}" -CookFlavor={cookFlavor} -allmaps -Compressed  -SkipCookingEditorContent -IterativeCooking ' # -CookOutputDir={cookOutputDir} 
        # 生成平台相关打包工程
        cmdstr += f'-stage ' # -stagingdirectory={stageOutputDir} 
        cmdstr += f'-package '

        cmdstr += f'-ForceFASTBuild -installed '
        if isDis:
            cmdstr += f'-distribution ' # 发布版本(android签名，ios dis签名)

        ext = '-nocompileeditor -prereqs -nodebuginfo -manifests'
        cmdstr += ext
        #  -archive -archivedirectory=E:/tmp/maxsize2 输出产物到指定目录
        #  -target=HitBoxMakerBlueprint 指定目标
        if self.skipGradleBuild:
            self.engine.setSkipGradleBuildFlag()

        self.engine.cmd(cmdstr,getstdout=False)

        if self.skipGradleBuild:
            self.engine.revertSkipGradleBuildFlag()

        pass    
    
    def buildGameModule(self,moduleName,platform,buildMode,manifestPath,rawLogPath,targetType=None,clean=False,onlyRunUHT=False):
        if clean:
            self.clean(moduleName,platform,buildMode)
        # TODO 对应平台自动部署对应的环境

        cmd = f'-Target={moduleName} {platform} {buildMode} -Project="{self.uprojectfilePath}" -Manifest="{manifestPath}" -noxge -NoHotReload -log="{rawLogPath}"'

        if targetType != None:
            cmd += f' -TargetType={targetType}'
        if onlyRunUHT:
            cmd += f' -OnlyRunUHT -ForceHeaderGeneration'

        out,code = self.engine.ubtCmd(cmd,getstdout=False,errException=None)
        return out,code
    
    def clean(self,moduleName,platform,buildMode):
        cmd = f'-clean -Target={moduleName} {platform} {buildMode} -Project="{self.uprojectfilePath}"'
        # -SkipDeploy
        out,code = self.engine.ubtCmd(cmd,getstdout=False,errException=None)

    
    def buildNoPack(self,platform,mode,cookFlavor,isDis=False,noXGE=True):
        cmdstr = ''
        if not com.isWindows():
            cmdstr += 'mono '
        cmdstr = f'{self.engine.UATPath} BuildCookRun '
        cmdstr += self._baseBuildCmd()
        # 不用Incredibuild，让cpu跑满
        if noXGE:
            cmdstr += '-NoXGE '
        # 工程设置
        cmdstr += f'-ScriptsForProject="{self.uprojectfilePath}" -project="{self.uprojectfilePath}" -noP4 -targetplatform={platform} -UTF8Output -pak '
        cmdstr += f'-build -clientconfig={mode} '
        cmdstr += f'-cook -ue4exe={self.engine.editorPath} -CookFlavor={cookFlavor} -allmaps -Compressed -SkipCookingEditorContent -iterativecooking '
        # stagedir = r'E:\stagedir'
        # cmdstr += f'-stage -stagingdirectory="{stagedir}"'
        cmdstr += f'-stage '
        if isDis:
            cmdstr += f'-distribution ' # 发布版本(android签名，ios dis签名)

        self.engine.cmd(cmdstr,getstdout=False)
        pass
    def buildPack(self,platform,mode,cookFlavor,isDis=False,noXGE=True):
        # 打包机：
        # -build -clientconfig={mode}
        # -skipcook -skipstage
        # -package

        # 4.22 bug ERROR: System.IO.DirectoryNotFoundException: 未能找到路径“D:\_workspace\_project\ue4.22\RollingBoll\Binaries\Android\Install_RollingBoll-armv7-es2.bat”的一部分。
        # 在 AndroidPlatform.<>c__DisplayClass24_4.<Package>b__1(UnrealTargetPlatform Target) 位置 D:\Build\++UE4\Sync\Engine\Saved\CsTools\Engine\Source\Programs\AutomationTool\Android\AndroidPlatform.Automation.cs:行号 429
        # fix:
        platform = self.engine._fixPlatformStr(platform)
        Path.ensure_direxsits(os.path.join(self.projectPath,'Binaries',platform))
        cmdstr = ''
        if not com.isWindows():
            cmdstr += 'mono '
        cmdstr = f'{self.engine.UATPath} BuildCookRun '
        cmdstr += self._baseBuildCmd()

        # 工程设置
        cmdstr += f'-ScriptsForProject="{self.uprojectfilePath}" -project="{self.uprojectfilePath}" -noP4 -targetplatform={platform} -UTF8Output -pak'
        if self.skipGradleBuild:
            self.engine.setSkipGradleBuildFlag()
            pass
            # cmdstr += ' -CompileAsDll'
        # 不用Incredibuild，让cpu跑满
        if noXGE:
            cmdstr += '-NoXGE '
        # 纯打代码，与资源无关
        cmdstr += '-skippak '
        # 构建设置
        cmdstr += f'-clientconfig={mode} '
        cmdstr += f'-skipcook -CookFlavor={cookFlavor} '
        cmdstr += f'-stage ' 
        cmdstr += f'-package '

        if isDis:
            cmdstr += f'-distribution ' # 发布版本(android签名，ios dis签名)


        self.engine.cmd(cmdstr,getstdout=False)
        pass


    def _baseBuildCmd(self):
        cmdstr = ''
        if com.isWindows():
            cmdstr += '-NoCodeSign '
        ext = '-nocompileeditor -prereqs -nodebuginfo -manifests '
        cmdstr += ext
        return cmdstr
    @workspace
    def build_CodePakAndPack(self,platform:str,rebuildRes,mode,cookFlavor,isDis=False,useDolphinResUpdate=True,noXGE=True):
        platform = self.engine._fixPlatformStr(platform)

        self.buildNoPack(platform,mode,cookFlavor,isDis=isDis,noXGE=noXGE)

        cwd = os.getcwd()
        if useDolphinResUpdate:
            # TODO 根据包内资源列表生成.res，然后将全部资源打zip上传
            savepath = os.path.join(cwd,'a.ifs')
            if platform == 'Android':
                pakpath = os.path.join(self.savedPath,'StagedBuilds',f'Android_{cookFlavor}',self.projectName,'Content','Paks')
            elif platform == 'Win64':
                pakpath = os.path.join(self.savedPath,'StagedBuilds',f'WindowsNoEditor',self.projectName,'Content','Paks')
            elif platform == 'IOS':
                pakpath = os.path.join(self.savedPath,'StagedBuilds',f'IOS',self.projectName,'Content','Paks')
            else:
                raise Exception(f'不支持平台{platform}')

            srcRespath = os.path.join(cwd,'a.res')
            dstRespath = os.path.join(self.pluginsPath,'ThirdParty','TMSDK','PlatformRes','Android','first_source.ifs.res')
            
            BinManager.IIFSPackager(f'new -zip=zlib -createalways "{savepath}" "{pakpath}"')
            BinManager.IIFSPackager(f'backup -createalways "{savepath}"')
            Path.ensure_pathexsits(srcRespath,dstRespath)


        self.buildPack(platform,mode,cookFlavor,isDis=isDis,noXGE=noXGE)


class UEDataHelper:
    @staticmethod
    def ReadUESerializedData_FString_SerializeAsANSICharArray(fs):
        length = UEDataHelper.ReadUESerializedData_int32(fs)
        str_raw:bytes = fs.read(length)
        for index,sb in enumerate(str_raw):
            if sb == b'\x00'[0]:
                break
        str_raw = str_raw[0:index]
        s = str_raw.decode(encoding='ansi')
        return s
    @staticmethod
    def ReadUESerializedData_int32(fs):
        len_bytes = fs.read(4)
        v = int.from_bytes(len_bytes,byteorder='little',signed=True)
        return v
    @staticmethod
    def ReadUESerializedData_TArrayuint8(fs):
        length = UEDataHelper.ReadUESerializedData_int32(fs)
        data = fs.read(length)
        return data




# "\"D:\\_workspace\\_project\\ue4.26\\fastbuild_cpp\\Source\\fastbuild_cpp\\fastbuild_cppWheelFront.cpp\""

# "/Zc:inline /nologo /Oi /c /Gw /Gy /Zm1000 /wd4819 /D_CRT_STDIO_LEGACY_WIDE_SPECIFIERS=1 /D_SILENCE_STDEXT_HASH_DEPRECATION_WARNINGS=1 /D_DISABLE_EXTENDED_ALIGNED_STORAGE /source-charset:utf-8 /execution-charset:utf-8 /Ob2 /Ox /Ot /GF /errorReport:prompt /D_HAS_EXCEPTIONS=0 /Z7 /MD /bigobj /fp:fast /Zo /Zp8 /we4456 /we4458 /we4459 /wd4463 /wd4244 /wd4838 /I . /I D:\\_workspace\\_project\\ue4.26\\fastbuild_cpp\\Intermediate\\Build\\Win64\\UE4\\Inc\\fastbuild_cpp /I D:\\_workspace\\_project\\ue4.26\\fastbuild_cpp\\Source /I Runtime /I Runtime\\TraceLog\\Public /I Runtime\\Core\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\CoreUObject /I Runtime\\CoreUObject\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\Engine /I Runtime\\Engine\\Classes /I Runtime\\Engine\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\NetCore /I Runtime\\Net /I Runtime\\Net\\Core\\Classes /I Runtime\\Net\\Core\\Public /I Runtime\\ApplicationCore\\Public /I Runtime\\RHI\\Public /I Runtime\\Json\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\SlateCore /I Runtime\\SlateCore\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\InputCore /I Runtime\\InputCore\\Classes /I Runtime\\InputCore\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\Slate /I Runtime\\Slate\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\ImageWrapper /I Runtime\\ImageWrapper\\Public /I Runtime\\Messaging\\Public /I Runtime\\MessagingCommon\\Public /I Runtime\\RenderCore\\Public /I Runtime\\Analytics /I Runtime\\Analytics\\AnalyticsET\\Public /I Runtime\\Analytics\\Analytics\\Public /I Runtime\\Sockets\\Public /I Runtime\\Net\\Common\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\AssetRegistry /I Runtime\\AssetRegistry\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\EngineMessages /I Runtime\\EngineMessages\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\EngineSettings /I Runtime\\EngineSettings\\Classes /I Runtime\\EngineSettings\\Public /I Runtime\\SynthBenchmark\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\Renderer /I Runtime\\Renderer\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\GameplayTags /I Runtime\\GameplayTags\\Classes /I Runtime\\GameplayTags\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\PacketHandler /I Runtime\\PacketHandlers /I Runtime\\PacketHandlers\\PacketHandler\\Classes /I Runtime\\PacketHandlers\\PacketHandler\\Public /I Runtime\\PacketHandlers\\ReliabilityHandlerComponent\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\AudioPlatformConfiguration /I Runtime\\AudioPlatformConfiguration\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\MeshDescription /I Runtime\\MeshDescription\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\StaticMeshDescription /I Runtime\\StaticMeshDescription\\Public /I Runtime\\PakFile\\Public /I Runtime\\RSA\\Public /I Runtime\\NetworkReplayStreaming /I Runtime\\NetworkReplayStreaming\\NetworkReplayStreaming\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\PhysicsCore /I Runtime\\PhysicsCore\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\DeveloperSettings /I Runtime\\DeveloperSettings\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\Chaos /I Runtime\\Experimental /I Runtime\\Experimental\\Chaos\\Public /I Runtime\\Experimental\\ChaosCore\\Public /I ThirdParty\\Intel /I Runtime\\Experimental\\Voronoi\\Public /I ThirdParty /I Runtime\\SignalProcessing\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\AudioExtensions /I Runtime\\AudioExtensions\\Public /I Runtime\\AudioMixerCore\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\PropertyAccess /I Runtime\\PropertyAccess\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\ClothingSystemRuntimeInterface /I Runtime\\ClothingSystemRuntimeInterface\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\AudioMixer /I Runtime\\AudioMixer\\Classes /I Runtime\\AudioMixer\\Public /I Developer /I Developer\\TargetPlatform\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\AnimationCore /I Runtime\\AnimationCore\\Public /I ..\\Plugins\\Runtime\\PhysXVehicles\\Intermediate\\Build\\Win64\\UE4\\Inc\\PhysXVehicles /I ..\\Plugins\\Runtime\\PhysXVehicles\\Source /I ..\\Plugins\\Runtime\\PhysXVehicles\\Source\\PhysXVehicles\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\AnimGraphRuntime /I Runtime\\AnimGraphRuntime\\Public /I ..\\Plugins\\Runtime\\PhysXVehicles\\Source\\ThirdParty /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\HeadMountedDisplay /I Runtime\\HeadMountedDisplay\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\AugmentedReality /I Runtime\\AugmentedReality\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\MRMesh /I Runtime\\MRMesh\\Public /I ThirdParty\\PhysX3\\PxShared\\include /I ThirdParty\\PhysX3\\PxShared\\include\\cudamanager /I ThirdParty\\PhysX3\\PxShared\\include\\filebuf /I ThirdParty\\PhysX3\\PxShared\\include\\foundation /I ThirdParty\\PhysX3\\PxShared\\include\\pvd /I ThirdParty\\PhysX3\\PxShared\\include\\task /I ThirdParty\\PhysX3\\PhysX_3.4\\Include /I ThirdParty\\PhysX3\\PhysX_3.4\\Include\\cooking /I ThirdParty\\PhysX3\\PhysX_3.4\\Include\\common /I ThirdParty\\PhysX3\\PhysX_3.4\\Include\\extensions /I ThirdParty\\PhysX3\\PhysX_3.4\\Include\\geometry /I ThirdParty\\PhysX3\\APEX_1.4\\include /I ThirdParty\\PhysX3\\APEX_1.4\\include\\clothing /I ThirdParty\\PhysX3\\APEX_1.4\\include\\nvparameterized /I ThirdParty\\PhysX3\\APEX_1.4\\include\\legacy /I ThirdParty\\PhysX3\\APEX_1.4\\include\\PhysX3 /I ThirdParty\\PhysX3\\APEX_1.4\\common\\include /I ThirdParty\\PhysX3\\APEX_1.4\\common\\include\\autogen /I ThirdParty\\PhysX3\\APEX_1.4\\framework\\include /I ThirdParty\\PhysX3\\APEX_1.4\\framework\\include\\autogen /I ThirdParty\\PhysX3\\APEX_1.4\\shared\\general\\RenderDebug\\public /I ThirdParty\\PhysX3\\APEX_1.4\\shared\\general\\PairFilter\\include /I ThirdParty\\PhysX3\\APEX_1.4\\shared\\internal\\include /I D:\\software\\VS2019_IDE\\VC\\Tools\\MSVC\\14.28.29333\\INCLUDE /I \"C:\\Program Files (x86)\\Windows Kits\\NETFXSDK\\4.6.2\\include\\um\" /I \"C:\\Program Files (x86)\\Windows Kits\\10\\include\\10.0.18362.0\\ucrt\" /I \"C:\\Program Files (x86)\\Windows Kits\\10\\include\\10.0.18362.0\\shared\" /I \"C:\\Program Files (x86)\\Windows Kits\\10\\include\\10.0.18362.0\\um\" /I \"C:\\Program Files (x86)\\Windows Kits\\10\\include\\10.0.18362.0\\winrt\" /FI\"D:\\_workspace\\_project\\ue4.26\\fastbuild_cpp\\Intermediate\\Build\\Win64\\fastbuild_cpp\\Development\\Engine\\SharedPCH.Engine.ShadowErrors.h\" /Yu\"D:\\_workspace\\_project\\ue4.26\\fastbuild_cpp\\Intermediate\\Build\\Win64\\fastbuild_cpp\\Development\\Engine\\SharedPCH.Engine.ShadowErrors.h\" /Fp\"D:\\_workspace\\_project\\ue4.26\\fastbuild_cpp\\Intermediate\\Build\\Win64\\fastbuild_cpp\\Development\\Engine\\SharedPCH.Engine.ShadowErrors.h.pch\" \"D:\\_workspace\\_project\\ue4.26\\fastbuild_cpp\\Source\\fastbuild_cpp\\fastbuild_cppWheelFront.cpp\" /FI\"D:\\_workspace\\_project\\ue4.26\\fastbuild_cpp\\Intermediate\\Build\\Win64\\UE4\\Development\\fastbuild_cpp\\Definitions.fastbuild_cpp.h\" /Fo\"D:\\_workspace\\_project\\ue4.26\\fastbuild_cpp\\Intermediate\\Build\\Win64\\UE4\\Development\\fastbuild_cpp\\fastbuild_cppWheelFront.cpp.obj\" /TP /GR- /W4"
# "/Zc:inline /nologo /Oi /c /Gw /Gy /Zm1000 /wd4819 /D_CRT_STDIO_LEGACY_WIDE_SPECIFIERS=1 /D_SILENCE_STDEXT_HASH_DEPRECATION_WARNINGS=1 /D_DISABLE_EXTENDED_ALIGNED_STORAGE /source-charset:utf-8 /execution-charset:utf-8 /Ob2 /Ox /Ot /GF /errorReport:prompt /D_HAS_EXCEPTIONS=0 /Z7 /MD /bigobj /fp:fast /Zo /Zp8 /we4456 /we4458 /we4459 /wd4463 /wd4244 /wd4838 /I . /I D:\\_workspace\\_project\\ue4.26\\fastbuild_cpp\\Intermediate\\Build\\Win64\\UE4\\Inc\\fastbuild_cpp /I D:\\_workspace\\_project\\ue4.26\\fastbuild_cpp\\Source /I Runtime /I Runtime\\TraceLog\\Public /I Runtime\\Core\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\CoreUObject /I Runtime\\CoreUObject\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\Engine /I Runtime\\Engine\\Classes /I Runtime\\Engine\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\NetCore /I Runtime\\Net /I Runtime\\Net\\Core\\Classes /I Runtime\\Net\\Core\\Public /I Runtime\\ApplicationCore\\Public /I Runtime\\RHI\\Public /I Runtime\\Json\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\SlateCore /I Runtime\\SlateCore\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\InputCore /I Runtime\\InputCore\\Classes /I Runtime\\InputCore\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\Slate /I Runtime\\Slate\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\ImageWrapper /I Runtime\\ImageWrapper\\Public /I Runtime\\Messaging\\Public /I Runtime\\MessagingCommon\\Public /I Runtime\\RenderCore\\Public /I Runtime\\Analytics /I Runtime\\Analytics\\AnalyticsET\\Public /I Runtime\\Analytics\\Analytics\\Public /I Runtime\\Sockets\\Public /I Runtime\\Net\\Common\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\AssetRegistry /I Runtime\\AssetRegistry\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\EngineMessages /I Runtime\\EngineMessages\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\EngineSettings /I Runtime\\EngineSettings\\Classes /I Runtime\\EngineSettings\\Public /I Runtime\\SynthBenchmark\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\Renderer /I Runtime\\Renderer\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\GameplayTags /I Runtime\\GameplayTags\\Classes /I Runtime\\GameplayTags\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\PacketHandler /I Runtime\\PacketHandlers /I Runtime\\PacketHandlers\\PacketHandler\\Classes /I Runtime\\PacketHandlers\\PacketHandler\\Public /I Runtime\\PacketHandlers\\ReliabilityHandlerComponent\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\AudioPlatformConfiguration /I Runtime\\AudioPlatformConfiguration\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\MeshDescription /I Runtime\\MeshDescription\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\StaticMeshDescription /I Runtime\\StaticMeshDescription\\Public /I Runtime\\PakFile\\Public /I Runtime\\RSA\\Public /I Runtime\\NetworkReplayStreaming /I Runtime\\NetworkReplayStreaming\\NetworkReplayStreaming\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\PhysicsCore /I Runtime\\PhysicsCore\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\DeveloperSettings /I Runtime\\DeveloperSettings\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\Chaos /I Runtime\\Experimental /I Runtime\\Experimental\\Chaos\\Public /I Runtime\\Experimental\\ChaosCore\\Public /I ThirdParty\\Intel /I Runtime\\Experimental\\Voronoi\\Public /I ThirdParty /I Runtime\\SignalProcessing\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\AudioExtensions /I Runtime\\AudioExtensions\\Public /I Runtime\\AudioMixerCore\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\PropertyAccess /I Runtime\\PropertyAccess\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\ClothingSystemRuntimeInterface /I Runtime\\ClothingSystemRuntimeInterface\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\AudioMixer /I Runtime\\AudioMixer\\Classes /I Runtime\\AudioMixer\\Public /I Developer /I Developer\\TargetPlatform\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\AnimationCore /I Runtime\\AnimationCore\\Public /I ..\\Plugins\\Runtime\\PhysXVehicles\\Intermediate\\Build\\Win64\\UE4\\Inc\\PhysXVehicles /I ..\\Plugins\\Runtime\\PhysXVehicles\\Source /I ..\\Plugins\\Runtime\\PhysXVehicles\\Source\\PhysXVehicles\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\AnimGraphRuntime /I Runtime\\AnimGraphRuntime\\Public /I ..\\Plugins\\Runtime\\PhysXVehicles\\Source\\ThirdParty /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\HeadMountedDisplay /I Runtime\\HeadMountedDisplay\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\AugmentedReality /I Runtime\\AugmentedReality\\Public /I ..\\Intermediate\\Build\\Win64\\UE4\\Inc\\MRMesh /I Runtime\\MRMesh\\Public /I ThirdParty\\PhysX3\\PxShared\\include /I ThirdParty\\PhysX3\\PxShared\\include\\cudamanager /I ThirdParty\\PhysX3\\PxShared\\include\\filebuf /I ThirdParty\\PhysX3\\PxShared\\include\\foundation /I ThirdParty\\PhysX3\\PxShared\\include\\pvd /I ThirdParty\\PhysX3\\PxShared\\include\\task /I ThirdParty\\PhysX3\\PhysX_3.4\\Include /I ThirdParty\\PhysX3\\PhysX_3.4\\Include\\cooking /I ThirdParty\\PhysX3\\PhysX_3.4\\Include\\common /I ThirdParty\\PhysX3\\PhysX_3.4\\Include\\extensions /I ThirdParty\\PhysX3\\PhysX_3.4\\Include\\geometry /I ThirdParty\\PhysX3\\APEX_1.4\\include /I ThirdParty\\PhysX3\\APEX_1.4\\include\\clothing /I ThirdParty\\PhysX3\\APEX_1.4\\include\\nvparameterized /I ThirdParty\\PhysX3\\APEX_1.4\\include\\legacy /I ThirdParty\\PhysX3\\APEX_1.4\\include\\PhysX3 /I ThirdParty\\PhysX3\\APEX_1.4\\common\\include /I ThirdParty\\PhysX3\\APEX_1.4\\common\\include\\autogen /I ThirdParty\\PhysX3\\APEX_1.4\\framework\\include /I ThirdParty\\PhysX3\\APEX_1.4\\framework\\include\\autogen /I ThirdParty\\PhysX3\\APEX_1.4\\shared\\general\\RenderDebug\\public /I ThirdParty\\PhysX3\\APEX_1.4\\shared\\general\\PairFilter\\include /I ThirdParty\\PhysX3\\APEX_1.4\\shared\\internal\\include /I D:\\software\\VS2019_IDE\\VC\\Tools\\MSVC\\14.28.29333\\INCLUDE /I \"C:\\Program Files (x86)\\Windows Kits\\NETFXSDK\\4.6.2\\include\\um\" /I \"C:\\Program Files (x86)\\Windows Kits\\10\\include\\10.0.18362.0\\ucrt\" /I \"C:\\Program Files (x86)\\Windows Kits\\10\\include\\10.0.18362.0\\shared\" /I \"C:\\Program Files (x86)\\Windows Kits\\10\\include\\10.0.18362.0\\um\" /I \"C:\\Program Files (x86)\\Windows Kits\\10\\include\\10.0.18362.0\\winrt\" /FI\"D:\\_workspace\\_project\\ue4.26\\fastbuild_cpp\\Intermediate\\Build\\Win64\\fastbuild_cpp\\Development\\Engine\\SharedPCH.Engine.ShadowErrors.h\" /Yu\"D:\\_workspace\\_project\\ue4.26\\fastbuild_cpp\\Intermediate\\Build\\Win64\\fastbuild_cpp\\Development\\Engine\\SharedPCH.Engine.ShadowErrors.h\" /Fp\"D:\\_workspace\\_project\\ue4.26\\fastbuild_cpp\\Intermediate\\Build\\Win64\\fastbuild_cpp\\Development\\Engine\\SharedPCH.Engine.ShadowErrors.h.pch\" FI\"D:\\_workspace\\_project\\ue4.26\\fastbuild_cpp\\Intermediate\\Build\\Win64\\UE4\\Development\\fastbuild_cpp\\Definitions.fastbuild_cpp.h\" /TP /GR- /W4"
# E:\softwave\UE_4.26\Engine\Build\Windows\cl-filter\cl-filter.exe -dependencies="D:\_workspace\_project\ue4.26\fastbuild_cpp\Intermediate\Build\Win64\UE4\Development\fastbuild_cpp\fastbuild_cppHud.cpp.txt" -compiler="D:\software\VS2019_IDE\VC\Tools\MSVC\14.28.29333\bin\HostX64\x64\cl.exe" -timing=E:\softwave\UE_4.26\Engine\Source\timing.txt -showincludes -- "D:\software\VS2019_IDE\VC\Tools\MSVC\14.28.29333\bin\HostX64\x64\cl.exe" @"D:\_workspace\_project\ue4.26\fastbuild_cpp\Intermediate\Build\Win64\UE4\Development\fastbuild_cpp\fastbuild_cppHud.cpp.obj.response"
# E:\softwave\UE_4.26\Engine\Build\Windows\cl-filter\cl-filter.exe -dependencies="D:\_workspace\_project\ue4.26\fastbuild_cpp\Intermediate\Build\Win64\UE4\Development\fastbuild_cpp\fastbuild_cppHud.cpp.txt" -compiler="D:\software\VS2019_IDE\VC\Tools\MSVC\14.28.29333\bin\HostX64\x64\cl.exe" -stderronly -- "D:\software\VS2019_IDE\VC\Tools\MSVC\14.28.29333\bin\HostX64\x64\cl.exe" /Zc:inline /nologo /Oi /c /Gw /Gy /Zm1000 /wd4819 /D_CRT_STDIO_LEGACY_WIDE_SPECIFIERS=1 /D_SILENCE_STDEXT_HASH_DEPRECATION_WARNINGS=1 /D_DISABLE_EXTENDED_ALIGNED_STORAGE /source-charset:utf-8 /execution-charset:utf-8 /Ob2 /Ox /Ot /GF /errorReport:prompt /D_HAS_EXCEPTIONS=0 /Z7 /MD /bigobj /fp:fast /Zo /Zp8 /we4456 /we4458 /we4459 /wd4463 /wd4244 /wd4838 /I . /I D:\_workspace\_project\ue4.26\fastbuild_cpp\Intermediate\Build\Win64\UE4\Inc\fastbuild_cpp /I D:\_workspace\_project\ue4.26\fastbuild_cpp\Source /I Runtime /I Runtime\TraceLog\Public /I Runtime\Core\Public /I ..\Intermediate\Build\Win64\UE4\Inc\CoreUObject /I Runtime\CoreUObject\Public /I ..\Intermediate\Build\Win64\UE4\Inc\Engine /I Runtime\Engine\Classes /I Runtime\Engine\Public /I ..\Intermediate\Build\Win64\UE4\Inc\NetCore /I Runtime\Net /I Runtime\Net\Core\Classes /I Runtime\Net\Core\Public /I Runtime\ApplicationCore\Public /I Runtime\RHI\Public /I Runtime\Json\Public /I ..\Intermediate\Build\Win64\UE4\Inc\SlateCore /I Runtime\SlateCore\Public /I ..\Intermediate\Build\Win64\UE4\Inc\InputCore /I Runtime\InputCore\Classes /I Runtime\InputCore\Public /I ..\Intermediate\Build\Win64\UE4\Inc\Slate /I Runtime\Slate\Public /I ..\Intermediate\Build\Win64\UE4\Inc\ImageWrapper /I Runtime\ImageWrapper\Public /I Runtime\Messaging\Public /I Runtime\MessagingCommon\Public /I Runtime\RenderCore\Public /I Runtime\Analytics /I Runtime\Analytics\AnalyticsET\Public /I Runtime\Analytics\Analytics\Public /I Runtime\Sockets\Public /I Runtime\Net\Common\Public /I ..\Intermediate\Build\Win64\UE4\Inc\AssetRegistry /I Runtime\AssetRegistry\Public /I ..\Intermediate\Build\Win64\UE4\Inc\EngineMessages /I Runtime\EngineMessages\Public /I ..\Intermediate\Build\Win64\UE4\Inc\EngineSettings /I Runtime\EngineSettings\Classes /I Runtime\EngineSettings\Public /I Runtime\SynthBenchmark\Public /I ..\Intermediate\Build\Win64\UE4\Inc\Renderer /I Runtime\Renderer\Public /I ..\Intermediate\Build\Win64\UE4\Inc\GameplayTags /I Runtime\GameplayTags\Classes /I Runtime\GameplayTags\Public /I ..\Intermediate\Build\Win64\UE4\Inc\PacketHandler /I Runtime\PacketHandlers /I Runtime\PacketHandlers\PacketHandler\Classes /I Runtime\PacketHandlers\PacketHandler\Public /I Runtime\PacketHandlers\ReliabilityHandlerComponent\Public /I ..\Intermediate\Build\Win64\UE4\Inc\AudioPlatformConfiguration /I Runtime\AudioPlatformConfiguration\Public /I ..\Intermediate\Build\Win64\UE4\Inc\MeshDescription /I Runtime\MeshDescription\Public /I ..\Intermediate\Build\Win64\UE4\Inc\StaticMeshDescription /I Runtime\StaticMeshDescription\Public /I Runtime\PakFile\Public /I Runtime\RSA\Public /I Runtime\NetworkReplayStreaming /I Runtime\NetworkReplayStreaming\NetworkReplayStreaming\Public /I ..\Intermediate\Build\Win64\UE4\Inc\PhysicsCore /I Runtime\PhysicsCore\Public /I ..\Intermediate\Build\Win64\UE4\Inc\DeveloperSettings /I Runtime\DeveloperSettings\Public /I ..\Intermediate\Build\Win64\UE4\Inc\Chaos /I Runtime\Experimental /I Runtime\Experimental\Chaos\Public /I Runtime\Experimental\ChaosCore\Public /I ThirdParty\Intel /I Runtime\Experimental\Voronoi\Public /I ThirdParty /I Runtime\SignalProcessing\Public /I ..\Intermediate\Build\Win64\UE4\Inc\AudioExtensions /I Runtime\AudioExtensions\Public /I Runtime\AudioMixerCore\Public /I ..\Intermediate\Build\Win64\UE4\Inc\PropertyAccess /I Runtime\PropertyAccess\Public /I ..\Intermediate\Build\Win64\UE4\Inc\ClothingSystemRuntimeInterface /I Runtime\ClothingSystemRuntimeInterface\Public /I ..\Intermediate\Build\Win64\UE4\Inc\AudioMixer /I Runtime\AudioMixer\Classes /I Runtime\AudioMixer\Public /I Developer /I Developer\TargetPlatform\Public /I ..\Intermediate\Build\Win64\UE4\Inc\AnimationCore /I Runtime\AnimationCore\Public /I ..\Plugins\Runtime\PhysXVehicles\Intermediate\Build\Win64\UE4\Inc\PhysXVehicles /I ..\Plugins\Runtime\PhysXVehicles\Source /I ..\Plugins\Runtime\PhysXVehicles\Source\PhysXVehicles\Public /I ..\Intermediate\Build\Win64\UE4\Inc\AnimGraphRuntime /I Runtime\AnimGraphRuntime\Public /I ..\Plugins\Runtime\PhysXVehicles\Source\ThirdParty /I ..\Intermediate\Build\Win64\UE4\Inc\HeadMountedDisplay /I Runtime\HeadMountedDisplay\Public /I ..\Intermediate\Build\Win64\UE4\Inc\AugmentedReality /I Runtime\AugmentedReality\Public /I ..\Intermediate\Build\Win64\UE4\Inc\MRMesh /I Runtime\MRMesh\Public /I ThirdParty\PhysX3\PxShared\include /I ThirdParty\PhysX3\PxShared\include\cudamanager /I ThirdParty\PhysX3\PxShared\include\filebuf /I ThirdParty\PhysX3\PxShared\include\foundation /I ThirdParty\PhysX3\PxShared\include\pvd /I ThirdParty\PhysX3\PxShared\include\task /I ThirdParty\PhysX3\PhysX_3.4\Include /I ThirdParty\PhysX3\PhysX_3.4\Include\cooking /I ThirdParty\PhysX3\PhysX_3.4\Include\common /I ThirdParty\PhysX3\PhysX_3.4\Include\extensions /I ThirdParty\PhysX3\PhysX_3.4\Include\geometry /I ThirdParty\PhysX3\APEX_1.4\include /I ThirdParty\PhysX3\APEX_1.4\include\clothing /I ThirdParty\PhysX3\APEX_1.4\include\nvparameterized /I ThirdParty\PhysX3\APEX_1.4\include\legacy /I ThirdParty\PhysX3\APEX_1.4\include\PhysX3 /I ThirdParty\PhysX3\APEX_1.4\common\include /I ThirdParty\PhysX3\APEX_1.4\common\include\autogen /I ThirdParty\PhysX3\APEX_1.4\framework\include /I ThirdParty\PhysX3\APEX_1.4\framework\include\autogen /I ThirdParty\PhysX3\APEX_1.4\shared\general\RenderDebug\public /I ThirdParty\PhysX3\APEX_1.4\shared\general\PairFilter\include /I ThirdParty\PhysX3\APEX_1.4\shared\internal\include /I D:\software\VS2019_IDE\VC\Tools\MSVC\14.28.29333\INCLUDE /I "C:\Program Files (x86)\Windows Kits\NETFXSDK\4.6.2\include\um" /I "C:\Program Files (x86)\Windows Kits\10\include\10.0.18362.0\ucrt" /I "C:\Program Files (x86)\Windows Kits\10\include\10.0.18362.0\shared" /I "C:\Program Files (x86)\Windows Kits\10\include\10.0.18362.0\um" /I "C:\Program Files (x86)\Windows Kits\10\include\10.0.18362.0\winrt" /FI"D:\_workspace\_project\ue4.26\fastbuild_cpp\Intermediate\Build\Win64\fastbuild_cpp\Development\Engine\SharedPCH.Engine.ShadowErrors.h" /Yu"D:\_workspace\_project\ue4.26\fastbuild_cpp\Intermediate\Build\Win64\fastbuild_cpp\Development\Engine\SharedPCH.Engine.ShadowErrors.h" /Fp"D:\_workspace\_project\ue4.26\fastbuild_cpp\Intermediate\Build\Win64\fastbuild_cpp\Development\Engine\SharedPCH.Engine.ShadowErrors.h.pch" /FI"D:\_workspace\_project\ue4.26\fastbuild_cpp\Intermediate\Build\Win64\UE4\Development\fastbuild_cpp\Definitions.fastbuild_cpp.h" /TP /GR- /W4 /Fo"D:\_workspace\_project\ue4.26\fastbuild_cpp\Intermediate\Build\Win64\UE4\Development\fastbuild_cpp\fastbuild_cppHud.cpp.obj" "D:\_workspace\_project\ue4.26\fastbuild_cpp\Source\fastbuild_cpp\fastbuild_cppHud.cpp" /showIncludes /E

        # windows 调试
        # fastbuild_cpp Win64 Development -Project=D:\_workspace\_project\ue4.26\fastbuild_cpp\fastbuild_cpp.uproject  D:\_workspace\_project\ue4.26\fastbuild_cpp\fastbuild_cpp.uproject -NoUBTMakefiles  -remoteini="D:\_workspace\_project\ue4.26\fastbuild_cpp" -skipdeploy -Manifest=D:\_workspace\_project\ue4.26\fastbuild_cpp\Intermediate\Build\Manifest.xml -NoHotReload -log="C:\Users\tengmu\AppData\Roaming\Unreal Engine\AutomationTool\Logs\E+softwave+UE_4.26\UBT-fastbuild_cpp-Win64-Development.txt"
        #                 16
        # -asdf woshishabi -true="shide" -fqefw asdf
        # -asdf woshishabi -true=shide -fqefw asdf
        # E:\softwave\UE_4.26\Engine\Build\Windows\cl-filter\cl-filter.exe -dependencies="D:\_workspace\_project\ue4.26\fastbuild_cpp\Intermediate\Build\Win64\UE4\Development\fastbuild_cpp\fastbuild_cpp.cpp.txt" -compiler="D:\software\VS2019_IDE\VC\Tools\MSVC\14.28.29333\bin\HostX64\x64\cl.exe" -stderronly -- "D:\software\VS2019_IDE\VC\Tools\MSVC\14.28.29333\bin\HostX64\x64\cl.exe" "D:\_workspace\_project\ue4.26\fastbuild_cpp\Source\fastbuild_cpp\fastbuild_cpp.cpp" /Fo"D:\_workspace\_project\ue4.26\fastbuild_cpp\Intermediate\Build\Win64\UE4\Development\fastbuild_cpp\fastbuild_cpp.cpp.obj" /Fp"D:\_workspace\_project\ue4.26\fastbuild_cpp\Intermediate\Build\Win64\fastbuild_cpp\Development\Engine\SharedPCH.Engine.ShadowErrors.h.pch" /Yu"D:\_workspace\_project\ue4.26\fastbuild_cpp\Intermediate\Build\Win64\fastbuild_cpp\Development\Engine\SharedPCH.Engine.ShadowErrors.h" /FI"D:\_workspace\_project\ue4.26\fastbuild_cpp\Intermediate\Build\Win64\fastbuild_cpp\Development\Engine\SharedPCH.Engine.ShadowErrors.h" /Zc:inline /nologo /Oi /c /Gw /Gy /Zm1000 /wd4819 /D_CRT_STDIO_LEGACY_WIDE_SPECIFIERS=1 /D_SILENCE_STDEXT_HASH_DEPRECATION_WARNINGS=1 /D_DISABLE_EXTENDED_ALIGNED_STORAGE /source-charset:utf-8 /execution-charset:utf-8 /Ob2 /Ox /Ot /GF /errorReport:prompt /D_HAS_EXCEPTIONS=0 /Z7 /MD /bigobj /fp:fast /Zo /Zp8 /we4456 /we4458 /we4459 /wd4463 /wd4244 /wd4838 /I . /I D:\_workspace\_project\ue4.26\fastbuild_cpp\Intermediate\Build\Win64\UE4\Inc\fastbuild_cpp /I D:\_workspace\_project\ue4.26\fastbuild_cpp\Source /I Runtime /I Runtime\TraceLog\Public /I Runtime\Core\Public /I ..\Intermediate\Build\Win64\UE4\Inc\CoreUObject /I Runtime\CoreUObject\Public /I ..\Intermediate\Build\Win64\UE4\Inc\Engine /I Runtime\Engine\Classes /I Runtime\Engine\Public /I ..\Intermediate\Build\Win64\UE4\Inc\NetCore /I Runtime\Net /I Runtime\Net\Core\Classes /I Runtime\Net\Core\Public /I Runtime\ApplicationCore\Public /I Runtime\RHI\Public /I Runtime\Json\Public /I ..\Intermediate\Build\Win64\UE4\Inc\SlateCore /I Runtime\SlateCore\Public /I ..\Intermediate\Build\Win64\UE4\Inc\InputCore /I Runtime\InputCore\Classes /I Runtime\InputCore\Public /I ..\Intermediate\Build\Win64\UE4\Inc\Slate /I Runtime\Slate\Public /I ..\Intermediate\Build\Win64\UE4\Inc\ImageWrapper /I Runtime\ImageWrapper\Public /I Runtime\Messaging\Public /I Runtime\MessagingCommon\Public /I Runtime\RenderCore\Public /I Runtime\Analytics /I Runtime\Analytics\AnalyticsET\Public /I Runtime\Analytics\Analytics\Public /I Runtime\Sockets\Public /I Runtime\Net\Common\Public /I ..\Intermediate\Build\Win64\UE4\Inc\AssetRegistry /I Runtime\AssetRegistry\Public /I ..\Intermediate\Build\Win64\UE4\Inc\EngineMessages /I Runtime\EngineMessages\Public /I ..\Intermediate\Build\Win64\UE4\Inc\EngineSettings /I Runtime\EngineSettings\Classes /I Runtime\EngineSettings\Public /I Runtime\SynthBenchmark\Public /I ..\Intermediate\Build\Win64\UE4\Inc\Renderer /I Runtime\Renderer\Public /I ..\Intermediate\Build\Win64\UE4\Inc\GameplayTags /I Runtime\GameplayTags\Classes /I Runtime\GameplayTags\Public /I ..\Intermediate\Build\Win64\UE4\Inc\PacketHandler /I Runtime\PacketHandlers /I Runtime\PacketHandlers\PacketHandler\Classes /I Runtime\PacketHandlers\PacketHandler\Public /I Runtime\PacketHandlers\ReliabilityHandlerComponent\Public /I ..\Intermediate\Build\Win64\UE4\Inc\AudioPlatformConfiguration /I Runtime\AudioPlatformConfiguration\Public /I ..\Intermediate\Build\Win64\UE4\Inc\MeshDescription /I Runtime\MeshDescription\Public /I ..\Intermediate\Build\Win64\UE4\Inc\StaticMeshDescription /I Runtime\StaticMeshDescription\Public /I Runtime\PakFile\Public /I Runtime\RSA\Public /I Runtime\NetworkReplayStreaming /I Runtime\NetworkReplayStreaming\NetworkReplayStreaming\Public /I ..\Intermediate\Build\Win64\UE4\Inc\PhysicsCore /I Runtime\PhysicsCore\Public /I ..\Intermediate\Build\Win64\UE4\Inc\DeveloperSettings /I Runtime\DeveloperSettings\Public /I ..\Intermediate\Build\Win64\UE4\Inc\Chaos /I Runtime\Experimental /I Runtime\Experimental\Chaos\Public /I Runtime\Experimental\ChaosCore\Public /I ThirdParty\Intel /I Runtime\Experimental\Voronoi\Public /I ThirdParty /I Runtime\SignalProcessing\Public /I ..\Intermediate\Build\Win64\UE4\Inc\AudioExtensions /I Runtime\AudioExtensions\Public /I Runtime\AudioMixerCore\Public /I ..\Intermediate\Build\Win64\UE4\Inc\PropertyAccess /I Runtime\PropertyAccess\Public /I ..\Intermediate\Build\Win64\UE4\Inc\ClothingSystemRuntimeInterface /I Runtime\ClothingSystemRuntimeInterface\Public /I ..\Intermediate\Build\Win64\UE4\Inc\AudioMixer /I Runtime\AudioMixer\Classes /I Runtime\AudioMixer\Public /I Developer /I Developer\TargetPlatform\Public /I ..\Intermediate\Build\Win64\UE4\Inc\AnimationCore /I Runtime\AnimationCore\Public /I ..\Plugins\Runtime\PhysXVehicles\Intermediate\Build\Win64\UE4\Inc\PhysXVehicles /I ..\Plugins\Runtime\PhysXVehicles\Source /I ..\Plugins\Runtime\PhysXVehicles\Source\PhysXVehicles\Public /I ..\Intermediate\Build\Win64\UE4\Inc\AnimGraphRuntime /I Runtime\AnimGraphRuntime\Public /I ..\Plugins\Runtime\PhysXVehicles\Source\ThirdParty /I ..\Intermediate\Build\Win64\UE4\Inc\HeadMountedDisplay /I Runtime\HeadMountedDisplay\Public /I ..\Intermediate\Build\Win64\UE4\Inc\AugmentedReality /I Runtime\AugmentedReality\Public /I ..\Intermediate\Build\Win64\UE4\Inc\MRMesh /I Runtime\MRMesh\Public /I ThirdParty\PhysX3\PxShared\include /I ThirdParty\PhysX3\PxShared\include\cudamanager /I ThirdParty\PhysX3\PxShared\include\filebuf /I ThirdParty\PhysX3\PxShared\include\foundation /I ThirdParty\PhysX3\PxShared\include\pvd /I ThirdParty\PhysX3\PxShared\include\task /I ThirdParty\PhysX3\PhysX_3.4\Include /I ThirdParty\PhysX3\PhysX_3.4\Include\cooking /I ThirdParty\PhysX3\PhysX_3.4\Include\common /I ThirdParty\PhysX3\PhysX_3.4\Include\extensions /I ThirdParty\PhysX3\PhysX_3.4\Include\geometry /I ThirdParty\PhysX3\APEX_1.4\include /I ThirdParty\PhysX3\APEX_1.4\include\clothing /I ThirdParty\PhysX3\APEX_1.4\include\nvparameterized /I ThirdParty\PhysX3\APEX_1.4\include\legacy /I ThirdParty\PhysX3\APEX_1.4\include\PhysX3 /I ThirdParty\PhysX3\APEX_1.4\common\include /I ThirdParty\PhysX3\APEX_1.4\common\include\autogen /I ThirdParty\PhysX3\APEX_1.4\framework\include /I ThirdParty\PhysX3\APEX_1.4\framework\include\autogen /I ThirdParty\PhysX3\APEX_1.4\shared\general\RenderDebug\public /I ThirdParty\PhysX3\APEX_1.4\shared\general\PairFilter\include /I ThirdParty\PhysX3\APEX_1.4\shared\internal\include /I D:\software\VS2019_IDE\VC\Tools\MSVC\14.28.29333\INCLUDE /I "C:\Program Files (x86)\Windows Kits\NETFXSDK\4.6.2\include\um" /I "C:\Program Files (x86)\Windows Kits\10\include\10.0.18362.0\ucrt" /I "C:\Program Files (x86)\Windows Kits\10\include\10.0.18362.0\shared" /I "C:\Program Files (x86)\Windows Kits\10\include\10.0.18362.0\um" /I "C:\Program Files (x86)\Windows Kits\10\include\10.0.18362.0\winrt" /FI"D:\_workspace\_project\ue4.26\fastbuild_cpp\Intermediate\Build\Win64\fastbuild_cpp\Development\Engine\SharedPCH.Engine.ShadowErrors.h" /FI"D:\_workspace\_project\ue4.26\fastbuild_cpp\Intermediate\Build\Win64\UE4\Development\fastbuild_cpp\Definitions.fastbuild_cpp.h" /TP /GR- /W4 /showIncludes /E
