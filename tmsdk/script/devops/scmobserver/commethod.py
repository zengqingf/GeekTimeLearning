# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace,LogCommitState
from comlib.conf.loader import Loader
from comlib.conf.ref import *


from comlib.dictm import JsonFile,DictUtil
from comlib.pathm import Path

from typing import List,Tuple

unstablefile = os.path.join(workdir,'unstable')
successfile = os.path.join(workdir,'success')


def svnRepoFilePath2LocalFilePath(svnurl:str,svnRepoFilePath,checkOutDirRoot):
    bdesc = Loader.load(projectconf)
    # https://192.168.2.12:8443/svn/Tenmove_Project_A8/trunk => /trunk
    pathInRepo = svnurl.replace(bdesc.reporoot,'',1)
    # /trunk/Program/Client/NextGenGame => c:\xxxx\xxxx\xx\Program\Client\NextGenGame
    svnLocalFilePath = com.convertsep(svnRepoFilePath.replace(pathInRepo,checkOutDirRoot,1))
    return svnLocalFilePath
                
def findSuspects(commitdata:List[Tuple[LogCommitState,str]],findSuspectsFunction=None):
    suspects = set()
    maybeSuspects = set()
    for commitstate,commitfilepath in commitdata:
        if findSuspectsFunction != None:
            suspect = findSuspectsFunction(commitstate,commitfilepath)
        else:
            suspect = commitstate.parent.author
        if not com.isNoneOrEmpty(suspect):
            suspects.add(suspect)

        maybeSuspects.add(commitstate.parent.author)
        pass
    com.logout('----------------------------------------------------')
    com.logout(f'[suspects] {suspects}')
    com.logout(f'[maybeSuspects] {maybeSuspects}')
    com.logout('----------------------------------------------------')
    # 没在提交列表中找到报错文件，直接根据提交区间里的提交人发提醒
    if suspects.__len__() == 0:
        suspects = maybeSuspects
    if 'jenkins' in suspects:
        suspects.remove('jenkins')
    return suspects

    
class ObserverResult():
    def __init__(self,dataKeysPath:List[str],observerName) -> None:
        self.dataKeysPath = dataKeysPath
        self.observerName = observerName

    def sucFlag(self):
        return True
    def successNotification(self):
        pass
    def failNotification(self):
        pass


class ObserverResultCtl():
    @staticmethod
    def run(interface:ObserverResult,unstableData={'fail':'fail'},successData={}):
        if not interface.sucFlag():
            com.logout(f'[fail]')
            if not CommitWorkHelper.isLastUnstable(*interface.dataKeysPath):
                com.logout(f'[call failNotification] suc2fail')
                # 通知
                interface.failNotification(interface)
                CommitWorkHelper.setUnstable(*interface.dataKeysPath,'data',data=unstableData)
            else:
                lastUnstableData = CommitWorkHelper.getLastUnstableData(*interface.dataKeysPath,'data')
                # 上次数据对比
                if DictUtil.isSame(lastUnstableData,unstableData):
                    # 相同则不通知
                    pass
                else:
                    com.logout(f'[call failNotification] fail2newfail')
                    # 不同则通知
                    interface.failNotification(interface)
                    
        else:
            com.logout(f'[suc]')
            if CommitWorkHelper.isLastUnstable(*interface.dataKeysPath):
                com.logout(f'[call failNotification] fail2suc')
                interface.successNotification(interface)
                CommitWorkHelper.setSuccessData(*interface.dataKeysPath,'data',data=successData)
                CommitWorkHelper.resetUnstable(*interface.dataKeysPath)













class CommitWorkData:
    def __init__(self,enginename:str,projRoot:str,svnurl:str,curRevision:int,cmdArgs:str) -> None:
        self.enginename = enginename
        self.projRoot = projRoot
        self.svnurl = svnurl
        self.curRevision = curRevision
        self.cmdArgs = None
        if cmdArgs != None:
            self.cmdArgs = cmdArgs.split(',')

class CommitWorkHelper():
    @staticmethod
    def setSuccess(workType,revision):
        com.logout(f'[设置success] workType={workType} revision={revision}')
        jf = JsonFile(successfile)
        # jf.trysetvalue(workType,'revision',value=revision)
        jf.setvalue(workType,'revision',value=revision)
        jf.save()
    @staticmethod
    def setSuccessData(*keys,data=None):
        jf = JsonFile(successfile)
        jf.setvalue(*keys,value=data)
        jf.save()
    @staticmethod
    def setSuccessUniqueData(*rootkeyPath,key=None,data=None):
        jf = JsonFile(successfile)
        jf.setvalue(*rootkeyPath,'data',key,value=data)
        jf.save()
    # @staticmethod
    # def setSuccessData(workType,key,value):
    #     jf = JsonFile(successfile)
    #     jf.setvalue(workType,'data',key,value=value)
    #     jf.save()
    @staticmethod
    def getSuccessRevision(workType):
        if os.path.exists(successfile):
            jf = JsonFile(successfile)
            revision = jf.trygetvalue(workType,'revision')
            if revision == None:
                return 0
            return revision
        else:
            return 0
    @staticmethod
    def getSuccessData(workType,key,default=''):
        if os.path.exists(successfile):
            jf = JsonFile(successfile)
            v = jf.trygetvalue(workType,'data',key)
            if v == None:
                return default
            return v
        else:
            return default
    
    @staticmethod
    def setUnstable(*keys,data=None):
        if not os.path.exists(unstablefile):
            com.dumpfile_json({},unstablefile)
        jf = JsonFile(unstablefile)
        if data == None:
            data = True
        jf.setvalue(*keys,value=data)
        jf.save()
    @staticmethod
    def getLastUnstableData(*keys):
        if not os.path.exists(unstablefile):
            return None
        jf = JsonFile(unstablefile)
        data = jf.trygetvalue(*keys)
        return data
    @staticmethod
    def resetUnstable(*keys):
        if not os.path.exists(unstablefile):
            return

        jf = JsonFile(unstablefile)
        isUnstable = jf.trygetvalue(*keys)
        if isUnstable:
            jf.remove(*keys)
        jf.clearEmpty()
        jf.save()
        if jf.__len__() == 0:
            Path.ensure_pathnotexsits(unstablefile)

    @staticmethod
    def isLastUnstable(*keys):
        if not os.path.exists(unstablefile):
            return False
        jf = JsonFile(unstablefile)
        if jf.haskey(*keys):
            return True
        return False
    @staticmethod
    def isNowUnstable(workType:str):
        jf = JsonFile(unstablefile)
        jf.clearEmpty()
        val = jf.trygetvalue(workType)
        if val == None:
            return False
        return True
        # if os.path.exists(unstablefile):
        #     return True
        # return False
    @staticmethod
    def dingFormat(robot,title,markdowncontent,docname=None):
        content = robot.markdown_textlevel(title,2) +'\n\n'
        content += robot.markdown_drawline()
        content += markdowncontent
        content += robot.markdown_drawline()
        content += robot.markdown_textlevel('维护客户端环境，人人有责',3)
        if docname != None:
            # url = f'http://192.168.2.147/dnl/DevOps/WhyItHappend/{docname}'
            url = f'http://www.baidu.com'
            data = robot.build_actioncard_mult(title,content,为什么会发生这个=url)
        else:
            data = robot.build_markdown(title,content)
        robot.send(data)
    # @staticmethod
    # def isNowSuccess():
    #     if os.path.exists(unstablefile):
    #         return False
    #     return True
