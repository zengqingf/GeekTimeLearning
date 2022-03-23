# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com
from comlib.conf.loader import Loader
from comlib.conf.ref import *

from comlib.wraps import workspace
from comlib.pathm import Path
# from unityparser import UnityDocument

import time


@errorcatch(HIGH)
class UnityManager(object):
    def __init__(self,unitypath,projectPath,timemark=None):
        self.unitypath = unitypath
        self.projectPath = projectPath
        self.timemark = com.timemark
        if timemark != None:
            self.timemark = timemark
        
        self.projectsettings_dirpath = os.path.join(self.projectPath,'ProjectSettings')
        self.projectsettings_filepath = os.path.join(self.projectsettings_dirpath,'ProjectSettings.asset')

        self.unityAssetPath = os.path.join(self.projectPath,'Assets')
        self.unityResourcesPath = os.path.join(self.unityAssetPath,'Resources')
        self.unityStreamingAssetsPath = os.path.join(self.unityAssetPath,'StreamingAssets')
        self.unityABSavePath = os.path.join(self.unityStreamingAssetsPath,'AssetBundles')
        self.pluginsPath = os.path.join(self.unityAssetPath,'Plugins')

        self.unityLibrary = os.path.join(self.projectPath,'Library')
        self.unityLibraryAsm = os.path.join(self.unityLibrary,'ScriptAssemblies')

        if sys.platform == 'darwin':
            self.exepath = os.path.join(unitypath,'Unity.app/Contents/MacOS/Unity')
            # unity自带工具
            self.toolspath = f'{self.unitypath}/Unity.app/Contents/Tools'
            self.binary2textPath = f'{self.toolspath}/binary2text'
            
            # 修复权限问题
            SharedPrecompiledHeadersPath = os.path.join(self.unitypath,'PlaybackEngines/iOSSupport/Trampoline/build/SharedPrecompiledHeaders')
            if os.path.exists(SharedPrecompiledHeadersPath):
                com.cmd(f'echo 123456 | sudo -S chmod 755 {SharedPrecompiledHeadersPath}',errException=Exception(f'修复{SharedPrecompiledHeadersPath}权限失败'),showlog=False)
        else:
            self.exepath = os.path.join(unitypath,'Editor\\Unity.exe')
            # unity自带工具
            self.toolspath = f'{self.unitypath}\\Editor\\Data\\Tools'
            self.binary2textPath = f'{self.toolspath}\\binary2text.exe'

    def binary2text(self,binaryfile,savepath):
        com.cmd(f'{self.binary2textPath} {binaryfile} {savepath}',errException=StopException('unity二进制文字转文本文件失败',locals()))
        
    def _get_cmd(self,executeMethod,batchmode=True,logfile=None,isquit=True,**parames)->str:
        if logfile == None:
            logfile = self._get_logfile_path(executeMethod+'.txt')

        # executeMethod_safe = com.safepath(executeMethod,*parames)
        paramesstr = ''
        for k,v in parames.items():
            paramesstr += f''' "--{k}={v}"'''
        execOpt = ''
        if executeMethod not in (None,''):
            execOpt = f'-executeMethod {executeMethod} {paramesstr}'
        cmd = ' '.join([
            '"%s"'%self.exepath,
            '-projectPath "%s"'%self.projectPath,
            '%s'%execOpt,
            '-logfile "%s"'%logfile
            
        ])        
        if batchmode:
            cmd += ' -batchmode'
        if isquit:
            cmd += ' -quit'
        
        return cmd
    def plat2UnityPlat(self,plat):
        if plat == "mac":
            pl = "OSXUniversal"
        elif plat == "win":
            pl = "Win64"

        return plat
    def set_BuildTarget(self,pyPlat):
        unityPlat = self.plat2UnityPlat(pyPlat)
        logfile = self._get_logfile_path(f'switch2{pyPlat}.txt')
        cmd = self._get_cmd(None,logfile=logfile)
        cmd += f' -buildTarget {unityPlat}'
        com.cmd(cmd,errException=Exception("set_BuildTarget执行失败"))
    def _get_logfile_path(self,filename):
        return com.get_logfile_path(filename)
    
    def run_cmd(self,executeMethod,batchmode=True,isquit=True,errException=None,**parames):
        '''
        适合用于不终止的自动化工具\n
        不合适用在打包工程，因为被强制终止后文件锁不会消除
        '''
        # @projlock(self.projectPath)
        def run():
            logfile = self._get_logfile_path(executeMethod+'.txt')
            cmd = self._get_cmd(executeMethod,logfile=logfile,isquit=isquit,**parames)
            out,code = com.cmd(cmd,errException=errException)
            return out,code
        time.sleep(3)
        return run()
    def run_cmd_nolock(self,executeMethod,batchmode=True,isquit=True,errException=None,**parames):
        time.sleep(3)
        logfile = self._get_logfile_path(executeMethod+'.txt')
        cmd = self._get_cmd(executeMethod,batchmode=batchmode,logfile=logfile,isquit=isquit,**parames)
        if isquit:
            out,code = com.cmd(cmd,errException=errException)
        else:
            out,code = com.cmd(cmd,getPopen=True,errException=errException)
        return out,code
    def open_close(self,errException=None):
        # @projlock(self.projectPath)
        def run():
            logfile = self._get_logfile_path('open_close.txt')
            cmd = self._get_cmd('',logfile=logfile,isquit=True)
            out,code = com.cmd(cmd,errException=errException)
            return out,code
        time.sleep(3)
        return run()


@errorcatch(HIGH)
class TMUnityManager(UnityManager):
    def __init__(self,unitypath,projectPath,timemark=None):
        super().__init__(unitypath,projectPath,timemark)
    def UWA_projscan(self)->str:
        '''
        非堵塞，需要多线程监视，完成后会在工程目录产生名为ok的文件，需要监视这个文件生成
        subp子进程,code状态码,scanResFolder生成路径
        无  -batchmode -quit
        ${UnityPath} -projectPath ${ProjectPath} -executeMethod UwaProjScan.MainScan.DoTest 
        -logFile
        ${LogFile} –quit
        '''
        subp,code = self.run_cmd_nolock('UwaProjScan.MainScan.DoTest',batchmode=False,isquit=False,errException=Exception("UWA_projscan执行失败"))
        # logfile = self._get_logfile_path('UwaProjScan.MainScan.DoTest')
        # cmd = self._get_cmd('UwaProjScan.MainScan.DoTest',batchmode=False,logfile=logfile,isquit=False)
        
        # out,code = com.cmd(cmd,errException=Exception("UWA_projscan执行失败"))

        scanResFolder = os.path.join(self.projectPath,'UwaScan')
        return subp,code,scanResFolder


    def Conf_UWA(self,directMode,directMonoManual):
        self.run_cmd_nolock('BuildPlayer.NewOpenMainSceneAddUWAPrefab',errException=StopException('配置uwa失败',locals()),
        directMode=directMode,directMonoManual=directMonoManual)
    def Set_Config(self,buildTarget,isUnityDev,isIL2CPP,isConnectProfiler,macro):
        self.set_BuildTarget(buildTarget)
        
        self.run_cmd_nolock('BuildPlayer.ConfigAll',errException=StopException('配置unity工程失败',locals()),
        isUnityDev=isUnityDev,isIL2CPP=isIL2CPP,isConnectProfiler=isConnectProfiler,macro=macro)

    def Generate_CSProj(self):
        out,code = self.run_cmd_nolock('BuildPlayer.OpenCSharpProject')
        csprojpaths = list(map(lambda x:os.path.join(self.projectPath,x),filter(lambda x:x.endswith('.csproj'),os.listdir(self.projectPath))))
        return csprojpaths,code
    def build(self,buildTarget,isUnityDev,isIL2CPP,isConnectProfiler,macro,outpath,**extparames):
        '''
        outpath不带后缀，后缀会根据平台自动加的
        '''
        if buildTarget == 'android':
            outpath += '.apk'
        elif buildTarget == 'ios':
            pass
        elif buildTarget == 'win':
            outpath += '.exe'
        elif buildTarget == 'android':
            outpath += '.app'
        else:
            raise StopException('平台不支持',locals())
        # 先清dll，不然jenkins取消导致dll不完整导致命令执行失败
        Path.ensure_pathnotexsits(self.unityLibraryAsm)
        plat = self.plat2UnityPlat(buildTarget)
        Path.ensure_pathnotexsits(os.path.join(self.projectPath,'Library','ScriptAssemblies'))
        self.set_BuildTarget(plat)
        self.run_cmd_nolock('BuildPlayer.NewBuild',errException=Exception('build执行失败'),
        plat=plat,isUnityDev=isUnityDev,isIL2CPP=isIL2CPP,isConnectProfiler=isConnectProfiler,macro=macro,outpath=outpath,**extparames)
    def buildRes(self,buildTarget,isUnityDev,isIL2CPP,isConnectProfiler,macro,outpath,**extparames):
        '''
        outpath不带后缀，后缀会根据平台自动加的
        '''
        if buildTarget == 'android':
            outpath += '.apk'
        elif buildTarget == 'ios':
            pass
        elif buildTarget == 'win':
            outpath += '.exe'
        elif buildTarget == 'android':
            outpath += '.app'
        else:
            raise StopException('平台不支持',locals())
        # 先清dll，不然jenkins取消导致dll不完整导致命令执行失败
        Path.ensure_pathnotexsits(self.unityLibraryAsm)
        plat = self.plat2UnityPlat(buildTarget)
        Path.ensure_pathnotexsits(os.path.join(self.projectPath,'Library','ScriptAssemblies'))
        self.set_BuildTarget(plat)
        self.run_cmd_nolock('BuildPlayer.NewBuildAssets',errException=Exception('build执行失败'),
        plat=plat,isUnityDev=isUnityDev,isIL2CPP=isIL2CPP,isConnectProfiler=isConnectProfiler,macro=macro,outpath=outpath,**extparames)

    def buildAB(self,buildTarget):
        plat = self.plat2UnityPlat(buildTarget)
        # self.set_BuildTarget(plat)

        self.run_cmd_nolock('BuildPlayer.NewBuildAB',errException=Exception('buildAB执行失败'),
        plat=plat)
    def loadReportData(self,methodname,clean=True):
        reportdir = os.path.join(self.projectPath,methodname)
        reportfile = os.path.join(reportdir,'report.txt')
        data = ''
        if os.path.exists(reportfile):
            data = com.readall(reportfile)
            if clean:
                Path.ensure_pathnotexsits(reportdir)

        return data


if __name__ == "__main__":
    print(UnityManager('','','213')._get_cmd('UwaProjScan.MainScan.DoTest','wos',isquit=True))
