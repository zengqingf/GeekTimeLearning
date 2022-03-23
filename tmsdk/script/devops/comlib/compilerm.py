# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com
from comlib.wraps import workspace
from comlib.conf.loader import Loader
from comlib.conf.ref import *

from typing import Tuple

class CSBuilder:
    def __init__(self,csprojPath) -> None:
        self.csprojPath = csprojPath
        self.name = os.path.basename(self.name)
    def build(self,mode='Debug',rebuild=True) -> Tuple[str,str,int]:
        logfilePath = com.get_logfile_path(f'{self.name}_{mode}')
        if com.isWindows():
            cmd = f'msbuild -version "{self.csprojPath}" /p:WarningLevel=0 /p:Configuration={mode}'
            cmd += ' /t:rebuild' if rebuild else ''

            out,code = com.cmd(cmd,logfile=logfilePath)
            
        else:
            raise Exception('非windows不支持')
        return out,logfilePath,code


class CPPBuilder:
    pass