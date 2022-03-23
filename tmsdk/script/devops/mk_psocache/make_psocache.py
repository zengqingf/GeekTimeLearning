# -*- coding:utf-8 -*
import sys,os,shutil
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
import argparse
from comlib.comobj import *
from comlib import SVNManager,Path

#TODO先固定路径
UE4Engine_Cmd_Path = 'D:\\UE/UE_4.25/Engine/Binaries/Win64/UE4Editor-Cmd.exe'
GameProject_Path = 'D:\\o_workspace/a8/NextGenActionGame.uproject'
savePsoCaching_Name = "NextGenActionGame_GLSL_ES3_1_ANDROID.stablepc.csv"

ProjectPso_SvnPath = "https://192.168.2.12:8443/svn/Tenmove_Project_A8/trunk/Program/Client/NextGenGame/Build/Android/PipelineCaches"

class PsoObj():
    def __init__(self,args) -> None:
        self.platform = args.platform
        self.localPsoPath = os.path.join(workdir,'psocache')
        self.svnPsofilePath = os.path.join(workdir,'a8pso')
        self.ftpPsoPath = com.get_ftp_tempsavepath(f'pso/{self.platform}/')
        print(f'self.localPsoPath {self.localPsoPath} self.ftpPsoPath {self.ftpPsoPath}')
        pass

    def beforemake(self):
        Path.ensure_dirnewest(self.localPsoPath)
        G_ftp.download(self.localPsoPath,self.ftpPsoPath)
        Path.ensure_svn_pathexsits(self.svnPsofilePath,ProjectPso_SvnPath)
        pass

    def aftermake(self):
        src = os.path.join(self.localPsoPath,savePsoCaching_Name)
        dst = os.path.join(self.svnPsofilePath,savePsoCaching_Name)
        if os.path.exists(src):
            shutil.copy(src,dst)
            SVNManager.commit(dst,'更新PSOCache文件')
        else:
            print('制作PSO缓存失败')
        pass

    def make(self):
        self.beforemake()
        out = com.cmd_subp(f'{UE4Engine_Cmd_Path} {GameProject_Path} -run=ShaderPipelineCacheTools expand {self.localPsoPath}\*.upipelinecache {self.localPsoPath}*.scl.csv {self.localPsoPath}\{savePsoCaching_Name}')
        #self.aftermake()

class PsoObjAndroid(PsoObj):
    def __init__(self,args):
        super().__init__(args)


def TestUpload():
    psopath = os.path.join('C:\\','pso','PipelineCaches')
    #psopath = os.path.join('e:\\','_SDKWorkSpace/NEW_SDK_WorkSpace/script/autotest/client/CollectedPSOs/')
    platform = 'android'
    ftppath = com.get_ftp_tempsavepath(f'pso/{platform}')
    G_ftp.upload(psopath,ftppath)

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    # 打包配置 start------------------------------------
    parser.add_argument('--platform',required=True)

    args = parser.parse_args()
    if args.platform == 'android':
        pso = PsoObjAndroid(args)
    pso.make()

    pass