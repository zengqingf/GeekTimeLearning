# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace
from comlib.conf.loader import Loader
from comlib.conf.ref import *

from comlib.svnm import SVNManager,LogCommitState
from comlib.dictm import JsonFile,DictUtil


import re

from typing import List,Type,TypeVar,Tuple




from scmobserver.observer import Observer,UnityObserver,ObserverState
from scmobserver.commethod import CommitWorkHelper,CommitWorkData
import argparse

# T = TypeVar('T')
# 每次提交后运行脚本

# 根目录是 svn分支的工程目录

# 根据配置读取每个脚本需要监控的目录，根据当前提交文件所在目录运行对应监控脚本

# 监控路径
# 执行操作


isOpendUnity = False


# TODO SVN GIT 统一，Unity UE4 等多种类型分类
class SVNCommitWork():
    def __init__(self,commitWorkFilePath) -> None:
        commitWorkFile = JsonFile(commitWorkFilePath)
        self.enginename = commitWorkFile.trygetvalue('engine').lower()
        


        self.rootDir = commitWorkFile.trygetvalue('rootDir')
        self.svnurl = commitWorkFile.trygetvalue('svnurl')
        self.vaildWorks = commitWorkFile.trygetvalue('vaildWorks')

        # 更新分支
        # self.clientPath = os.path.join(projRoot,'Client')
        # self.assetsPath = os.path.join(self.clientPath,'Assets')
        self.projRoot = os.path.join(workdir,os.path.basename(self.svnurl))

        self.enginInterface:EngineInterface = None

        if self.enginename == 'ue4':
            self.enginInterface = UE4(os.path.join(workdir,Loader.获取相对客户端游戏工程路径()))
        else:
            raise Exception(f'未知引擎{self.enginename}')



        # Path.ensure_svn_pathexsits(projRoot,self.svnurl)
        SVNManager.upgrade(self.projRoot)
        self.curRevision = int(SVNManager.version(self.projRoot))

        # self.unity = TMUnityManager(Loader.获取Unity路径(),self.clientPath)
        
    def run(self):
        self.initObserver()

        curRevision = self.curRevision

        asdf:List[Tuple[Observer,List[str],List[Tuple[LogCommitState,str]]]] = []
        for observer,observePaths in self.observerlist:
            # 根据上一次运行和当前运行之间的svn版本获取提交列表
            # 根据提交列表获取提交路径
            # 根据提交路径运行对应监控脚本
            lastRevision = CommitWorkHelper.getSuccessRevision(observer.__class__.__name__)
            # 不包含上次版本，因为上次版本已经在上次检查过了
            logs = SVNManager.getlog(lastRevision,curRevision,self.svnurl,containLastRivision=False)


            # 同文件去重
            orderCommit = {}
            # orderCommit = []
            for log in logs:
                for commitState in log.commitStates:
                    for observePath in observePaths:
                        fullpath = com.strjoin('/',self.rootDir,observePath)
                        if commitState.path.startswith(fullpath):
                            orderCommit[commitState.path] = commitState
                            # orderCommit.append((commitState,commitState.path))
            orderCommitList = []
            for k,v in orderCommit.items():
                orderCommitList.append((v,k))
            asdf.append((observer,observePaths,orderCommitList))


        def isDependenceSuccess(dependences):
            for dependence in dependences:
                if dependence.state != ObserverState.SUCCESS:
                    return False
            return True
        def isDependenceFail(dependences):
            for dependence in dependences:
                if dependence.state == ObserverState.FAIL:
                    return True
            return False
        def isDependenceSkip(dependences):
            for dependence in dependences:
                if dependence.state == ObserverState.NOTSTART:
                    return True
            return False
        finishCount = 0
        suc = True
        while finishCount < asdf.__len__():
            # 根据指定目录的提交情况运行监控
            for observer,observePaths,orderCommit in asdf:
                # 跳过正在运行或者运行完成的
                if observer.state != ObserverState.NOTSTART:
                    continue
                # 依赖处理
                if observer.dependences != []:
                    for dep in observer.dependences:
                        com.logout(f'[依赖] {observer.__class__.__name__}=>{dep} state={dep.state}')
                    # 如果依赖存在失败
                    if isDependenceFail(observer.dependences):
                        suc = False
                    if isDependenceFail(observer.dependences) or isDependenceSkip(observer.dependences):
                        # 跳过该监控并设置为FAIL
                        com.logout(f'[依赖运行失败] 跳过{observer.__class__.__name__}')
                        observer.state = ObserverState.FAIL
                        finishCount += 1
                        break
                    # 如果依赖全部成功则继续流程
                    
                observer.mustDo(orderCommit,curRevision)

                lastRevision = CommitWorkHelper.getSuccessRevision(observer.__class__.__name__)

                com.logout(f'[监控] {observer.__class__.__name__}')
                com.logout(f'orderCommit.__len__() == {orderCommit.__len__() == 0}')
                com.logout(f'lastRevision == curRevision {lastRevision == curRevision}')
                com.logout(f'not observer.canRun(orderCommit,curRevision) {not observer.canRun(orderCommit,curRevision)}')
                if observer.isForceRun():
                    com.logout(f'[ForceRun] {observer.__class__.__name__}')
                    observer.run(orderCommit,curRevision)
                elif orderCommit.__len__() == 0 or lastRevision == curRevision or not observer.canRun(orderCommit,curRevision):
                    pass
                else:
                    observer.run(orderCommit,curRevision)
                finishCount += 1
        com.logout(f'------------------------------------------------------')
        com.logout(f'---------------------afterRun-------------------------')
        com.logout(f'------------------------------------------------------')

        for observer,observePaths,orderCommit in asdf:
            if observer.isForceRun():
                observer.afterRun(orderCommit,curRevision)
            elif orderCommit.__len__() == 0 or lastRevision == curRevision or not observer.canRun(orderCommit,curRevision):
                continue
            else:
                observer.afterRun(orderCommit,curRevision)

            if not CommitWorkHelper.isNowUnstable(observer.__class__.__name__):
                CommitWorkHelper.setSuccess(observer.__class__.__name__,self.curRevision)
        return suc
    def initObserver(self):
        '''
        初始化监控脚本
        '''
        self.observerlist:List[Tuple[Observer,List[str]]] = []
        import importlib
        platformModel = importlib.import_module(f'scmobserver.observerwork.{self.enginename}')
        commonModel = importlib.import_module(f'scmobserver.observerwork.common')
        # platformModel = importlib.import_module(f'scmobserver.observerwork.{self.}')
        baseObserverModel = importlib.import_module('scmobserver.observer')
        excludeTypeName = []
        vaildWorkTypeDict = {}
        vaildWorkTypeDict_common = {}
        for k,v in baseObserverModel.__dict__.items():
            if type(v).__name__ == 'type' and issubclass(v,Observer):
                excludeTypeName.append(k)

        for k,v in platformModel.__dict__.items():
            if type(v).__name__ == 'type' and (k not in excludeTypeName) and issubclass(v,Observer):
                    vaildWorkTypeDict[k] = v
        for k,v in commonModel.__dict__.items():
            if type(v).__name__ == 'type' and (k not in excludeTypeName) and issubclass(v,Observer):
                    vaildWorkTypeDict_common[k] = v

        vaildTypes = set()
        for vaildWork in self.vaildWorks:
            vaildTypes.add(vaildWork['workType'])

        argsparser = argparse.ArgumentParser()
        for vaildType in vaildTypes:
            argsparser.add_argument(f'--{vaildType}',required=False)
        args,unkonwList = argsparser.parse_known_args()

        # 按机子ip进行分组
        # 分组后根据进行依赖添加    
        works = []
        from comlib.comobj import G_ip
        for vaildWork in self.vaildWorks:
            if G_ip == vaildWork['workMechine']:
                works.append(vaildWork)

        refdict = {}
        for work in works:
            workType = work['workType']
            workArgs = work['workArgs']
            workMechine = work['workMechine']
            orderPaths = work['orderPaths']
            dependences = work['dependences']

            cmdArgs = getattr(args,workType,None)
            data = CommitWorkData(self.enginename,self.projRoot,self.svnurl,self.curRevision,cmdArgs)
            if workType in vaildWorkTypeDict:
                workIns = self.enginInterface.observerWorkInit(vaildWorkTypeDict[workType],workArgs,workMechine,data,dependences)
            elif workType in vaildWorkTypeDict_common:
                workIns = vaildWorkTypeDict_common[workType](workArgs,workMechine,data,dependences)


            self.observerlist.append((workIns,orderPaths))
            refdict[workType] = workIns
        # 依赖从字符串转为实例
        for workIns,orderPaths in self.observerlist:
            for index,dependence in enumerate(workIns.dependences):
                dependenceIns = DictUtil.tryGetValue(refdict,dependence)
                if dependenceIns == None:
                    raise Exception(f'监控依赖{dependence}未在本机{G_ip}中找到')
                workIns.dependences[index] = dependenceIns
                



class EngineInterface():
    def __init__(self,projPath) -> None:
        self.projPath = projPath
    def observerWorkInit(self,workCls,workArgs,workMechine,data:CommitWorkData,dependences):
        pass

class UE4(EngineInterface):
    def __init__(self, projPath) -> None:
        super().__init__(projPath)


    def observerWorkInit(self,workCls,workArgs,workMechine,data:CommitWorkData,dependences):
        from comlib import UE4Project,UE4Engine,Path

        if workMechine == '192.168.2.200':
            enginePath = Loader.获取引擎路径()
            com.logout(f'enginePath={enginePath} projPath={self.projPath}')
            self.proj = UE4Project(enginePath,self.projPath)
        else:
            enginePath,engineSVNPath = UE4Engine.getBinEngineTrunkSVNPathAndWCPath()
            Path.ensure_svn_pathexsits(enginePath,engineSVNPath)
            self.proj = UE4Project(enginePath,self.projPath)


        return workCls(self.proj,workArgs,workMechine,data,dependences)
    


if __name__ == "__main__":
    # commitWorkFilePath = sys.argv[1]
    commitWorkFilePath = os.path.join(thisdir,'workconf','NextGenGame.json')
    cw = SVNCommitWork(commitWorkFilePath)
    suc = cw.run()
    if suc == False:
        exit(1)
