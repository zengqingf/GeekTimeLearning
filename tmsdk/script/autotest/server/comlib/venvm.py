# -*- encoding: utf-8 -*-
from comlib.comobjm import *
thisdir = os.path.abspath(os.path.dirname(__file__))
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib.wraps import workspace
from comlib.conf.loader import Loader
from comlib.conf.ref import *
logger = Log.getLogger('Venv')
from comlib.pathm import Path

import venv
class VirtualEnvManager:
    def __init__(self,vpath) -> None:
        self.vpath = os.path.normpath(vpath)
        self.isCreate = False
        
        if com.isWindows():
            self.binPath = os.path.join(self.vpath,'Scripts')
            self.libPath = os.path.join(self.vpath,'Lib')
            self.pipPath = os.path.join(self.binPath,'pip.exe')
            self.pythonPath = os.path.join(self.binPath,'python.exe')
        else:
            self.binPath = os.path.join(self.vpath,'bin')
            self.libPath = os.path.join(self.vpath,'lib')
            self.pipPath = os.path.join(self.binPath,'pip')
            self.pythonPath = os.path.join(self.binPath,'python')
        self.__init()
    def __init(self):
        if os.path.exists(self.vpath):
            self.isCreate = True

            tmp = glob.glob(os.path.join(self.binPath,'pip3.*'))[0]
            noExtName,ext = os.path.splitext(os.path.basename(tmp))
            if ext != '.exe':
                noExtName += ext

            self.pythonVersion = noExtName.replace('pip','')

            if com.isWindows():
                self.sitePackagePath = os.path.join(self.libPath,'site-packages')
            else:
                self.sitePackagePath = os.path.join(self.libPath,f'python{self.pythonVersion}','site-packages')
    
    def create(self,override=False):
        if not override and os.path.exists(self.vpath):
            return
        Path.ensure_pathnotexsits(self.vpath)
        
        args = [self.vpath]
        venv.main(args)
        # 基础库
        self.installPak('requests')
        # 远程调试库
        self.installPak('ptvsd')

        self.__init()

    def installPak(self,pak):
        com.cmd(f'{self.pipPath} --disable-pip-version-check install -i http://192.168.2.132:5001/repository/pypi-center/simple/ --trusted-host 192.168.2.132 {pak}',errException=Exception('venv pip 安装失败'))
    def installRequirementsFile(self,requirementsFile:str):
        com.cmd(f'{self.pipPath} --disable-pip-version-check install -i http://192.168.2.132:5001/repository/pypi-center/simple/ --trusted-host 192.168.2.132 -r "{requirementsFile}"',errException=Exception(f'venv pip 安装{requirementsFile}失败'))
    



