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


import re

from typing import List,Type,TypeVar,Tuple

from scmobserver.observer import Observer,UnityObserver,ObserverState
from scmobserver.commethod import CommitWorkHelper,CommitWorkData,ObserverResult,ObserverResultCtl,findSuspects

from comlib import Jenkins,Jenkins_2_249_2_Manager
from comlib import XMLFile




class PackageApplication(Observer):
    '''
    使用jenkins进行打包测试
    '''
    def __init__(self, workArgs, workMechine, data: CommitWorkData,dependences) -> None:
        super().__init__(workArgs, workMechine, data,dependences)
        # 打包，用默认参数，只指定通用参数，例如平台、宏（失败提醒，发大群）
        self.jobname = self.workArgs[0]
        self.platform = self.workArgs[1]
        self.macro = self.workArgs[2]

        self.jenkins = Jenkins(Jenkins_2_249_2_Manager())
    def canRun(self, commitdata: List[Tuple[LogCommitState, str]], revision: int):
        # TODO 处理文件夹
        for commitstate,commitfilepath in commitdata:
            ext = os.path.splitext(commitfilepath)[-1]
            # com.logout(f'[canRun] {commitfilepath}  {ext}')
            
            if ext not in ('.target','.modules', # UE4通用
            '.dll','.pdb', # win64
            '.so', # android
            'dylib', # mac
            '.a'): # ios
                com.logout(f'[canRun] PackageApplication {True} {commitfilepath}  {ext}')

                return True
        com.logout(f'[canRun] PackageApplication {False}')
        return False

    def run(self, commitdata: List[Tuple[LogCommitState, str]], revision: int):
        self.state = ObserverState.RUNNING
        self.buildnum = self.jenkins.buildJob_parames(self.jobname,platform=self.platform,mode='Development',macro=self.macro,svnVersion=revision)
        content,code = self.jenkins.getDownloadHTMLContent(self.jobname,self.buildnum)
        com.logout(f'[PackageApplication] buildnum={self.buildnum} jobname={self.jobname} code={code}')

        com.logout(content)
        m = re.search('<a href="(.*?)">',content)
        self.packageUrl = m.group(1)
        res = ObserverResult(['PackageApplication'],'PackageApplication')
        res.sucFlag = lambda : code == 200

        if code == 200:
            self.state = ObserverState.SUCCESS
        else:
            self.state = ObserverState.FAIL
        def failNotification(o):
            pass
        def successNotification(o):
            pass
        res.failNotification = failNotification
        res.successNotification = successNotification



        ObserverResultCtl.run(res)


class OpenApplication(Observer):
    '''
    使用airtest进行app打开测试
    '''
    def __init__(self, workArgs, workMechine, data: CommitWorkData,dependences:List[PackageApplication]) -> None:
        super().__init__(workArgs, workMechine, data,dependences)
        self.autotestJobName = self.workArgs[0]
        # 依赖PackageApplication
        # 根据PackageApplication获取包下载地址
        self.autotestDir = os.path.join(workdir,'autotest')
        self.jenkins = Jenkins(Jenkins_2_249_2_Manager())


    def run(self, commitdata: List[Tuple[LogCommitState, str]], revision: int):
        # 拉取airtest工程
        # Path.ensure_svn_pathexsits(self.autotestDir,'svn://192.168.2.177/sdk/script/autotest/client')
        # 运行main.py
        # cmd = com.getvalue4plat('py','python3')
        # cmd += f'{cmd} {os.path.join(self.autotestDir,"main.py")} --package_downloadpath={self.dependence.packageUrl} --devices=华为荣耀V9 --paramfile=test t --casestate=打开游戏 --casename=None '
        # out,code = com.cmd(cmd,getstdout=False)
        # if code != 0:
        # 这里也用jenkins
        self.state = ObserverState.RUNNING
        buildnum = self.jenkins.buildJob_parames(self.autotestJobName,package_downloadpath=self.dependences[0].packageUrl,devices='华为荣耀V9',casestate='登陆',casename='进入游戏测试')
        
        state = self.jenkins.getBuildState(self.autotestJobName,buildnum)
        res = ObserverResult(['OpenApplication'],'OpenApplication')

        bot = Loader.获取脚本调试机器人()

        res.sucFlag = lambda : state == 'SUCCESS'

        def findSuspectsFunc(commitstate,commitfilepath):
            return commitstate.parent.author 

        def failNotification(o):
            suspects = findSuspects(commitdata,findSuspectsFunc)
            self.state = ObserverState.FAIL
            bot.send(bot.build_text(f'游戏运行失败嫌疑人：{"|".join(suspects)}'))

        def successNotification(o):
            self.state = ObserverState.SUCCESS
            bot.send(bot.build_text(f'游戏运行成功'))

        res.failNotification = failNotification
        res.successNotification = successNotification
        ObserverResultCtl.run(res)

        if self.state == ObserverState.RUNNING:
            self.state = ObserverState.SUCCESS


        # 上传符号表
        # self.jenkins.buildJob_parames('Client_Tool_Temp/SymbolUpload_win10')
