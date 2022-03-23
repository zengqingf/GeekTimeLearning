# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace
from comlib.conf.loader import Loader
from comlib.conf.ref import *


from comlib.exception import errorcatch,errorcatch_func,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace,SVNManager,Path,TMUnityManager,SVNState,LogCommitState,JsonFile
from comlib.conf.loader import Loader
from comlib.conf.ref import *
from comlib.ue4m import UE4Project


import re

from typing import List,Type,TypeVar,Tuple
from scmobserver.commethod import CommitWorkHelper,CommitWorkData
from enum import Enum,unique
@unique
class ObserverState(Enum):
    FAIL=0 # 失败
    SUCCESS=1 # 成功
    UNSTABLE=2 # 不稳定
    NOTSTART=3 # 未开始
    RUNNING=4 # 正在运行

class Observer():
    def __init__(self,workArgs:list,workMechine,data:CommitWorkData,dependences) -> None:
        self.methodname = None
        self.workArgs = workArgs
        self.workMechine = workMechine
        self.data = data
        self.dependences = dependences
        self.state = ObserverState.NOTSTART
        
        pass
    def mustDo(self,commitdata:List[Tuple[LogCommitState,str]],revision:int):
        pass
    def isForceRun(self):
        return False
    def canRun(self,commitdata:List[Tuple[LogCommitState,str]],revision:int):
        return True
    def run(self,commitdata:List[Tuple[LogCommitState,str]],revision:int):
        '''
        commitdata必定不为空
        '''
        pass
    
    def afterRun(self,commitdata:List[Tuple[LogCommitState,str]],revision:int):
        '''
        commitdata必定不为空
        '''
        pass
class UnityObserver(Observer):
    def __init__(self,isUnityWork,unity:TMUnityManager,workArgs,workMechine,data:CommitWorkData,dependences) -> None:
        super().__init__(workArgs,workMechine,data,dependences)
        self.unity = unity
        self.isUnityWork = isUnityWork
class UE4Observer(Observer):
    def __init__(self,proj:UE4Project,workArgs,workMechine,data:CommitWorkData,dependences) -> None:
        super().__init__(workArgs,workMechine,data,dependences)
        self.proj = proj
        self.engine = proj.engine
        self.ubt = self.engine.UBTPath
