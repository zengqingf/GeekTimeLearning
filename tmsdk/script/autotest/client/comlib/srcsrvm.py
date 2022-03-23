# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com
from comlib.conf.loader import Loader
from comlib.conf.ref import *

from comlib.gitm import GitlabManager,GitManager
from comlib.wraps import workspace

from enum import Enum,unique

from typing import List,Tuple

@unique
class SrcsrvSupportSCM(Enum):
    Subversion=0
    GitLab=1
    Perforce=2
    

class Srcsrv_SCM:
    def getName(self)->str:
        '''VERCTRL的值，版本控制软件名'''
        pass
    def getLocalRelFilePath(self)->str:
        '''EXTRACT_TARGET的值，下载文件的本地相对路径'''
        pass
    def getDownloadFileCmd(self)->str:
        '''EXTRACT_CMD的值，下载仓库内代码文件命令'''
        pass
    def getFileVarStrBuild(self,localCodeFilePath,codeFileRelPathInRepo,revision)->str:
        '''{localCodeFilePath}*{var2}*{var3}*...的值字符串生成'''
        pass
    def checkFileInRepo(self,localFilePath)->str:
        '''检查文件是否在仓库里'''
        pass
    def getRelFilePath(self,localFilePath)->str:
        '''绝对路径转仓库相对路径'''
        pass
    def getRevision(self)->str:
        '''获取版本号'''
        pass
class Srcsrv_GitLab(Srcsrv_SCM):
    def __init__(self,workspaceDir,token:str,publicTokenSVNUrl) -> None:
        self.gitm = GitManager(workspaceDir)
        gitUrl = self.gitm.getRepoUrl()
        self.gitlabm = GitlabManager(gitUrl,token)
        self.publicTokenSVNUrl = publicTokenSVNUrl
    def getName(self)->str:
        return 'GitLab'
    def getLocalRelFilePath(self)->str:
        return '%targ%\\%var4%\\%fnbksl%(%var3%)'
    def getDownloadFileCmd(self)->str:
        apiUrl_base = self.gitlabm.getApiUrl('/projects/:id/repository/files').replace('%','%%')
        apiUrl = apiUrl_base + '/%var2%/raw?ref=%var4%'
        cmd = f'''cmd /c "for /f "delims=" %%i in ('svn cat {self.publicTokenSVNUrl}') do ( curl -s --header "PRIVATE-TOKEN: %%i" "{apiUrl}" -o "%EXTRACT_TARGET%" )"'''
        return cmd
    def getFileVarStrBuild(self,localCodeFilePath,codeFileRelPathInRepo,revision)->str:
        codeFileRelPathInRepo = com.convertsep2Unix(codeFileRelPathInRepo)
        codeFileRelPathInRepo_encode = com.urlencode(codeFileRelPathInRepo,plus=True).replace("%","%%")
        return f'{localCodeFilePath}*{codeFileRelPathInRepo_encode}*{codeFileRelPathInRepo}*{revision}'
    def checkFileInRepo(self,localFilePath)->str:
        return self.gitm.isFileExists(localFilePath)
    def getRelFilePath(self,localFilePath:str)->str:
        return localFilePath.replace(self.gitm.workspace + os.sep,'',1)
    def getRevision(self) -> str:
        return self.gitm.getRevision()

class SrcsrvManager:
    def __init__(self,scm:Srcsrv_SCM,windowsSDKPath) -> None:
        self.scm = scm
        self.windowsSDKPath = windowsSDKPath
        arch = 'x64'
        self.debuggerBinDir = os.path.join(windowsSDKPath,'Debuggers',arch)
        self.dbhExe = os.path.join(self.debuggerBinDir,'dbh.exe')
        self.pdbstrExe = os.path.join(self.debuggerBinDir,'srcsrv','pdbstr.exe')

        # 用于已经确认在仓库里的文件
        self.repoFileDict = {}
        self.fileNotInRepoDict = set()
        self.revision = ''

    @workspace
    def modifyPDB(self,pdbPaths:List[str]):
        for pdbPath in pdbPaths:
            data,code = com.cmd(f'"{self.dbhExe}" "{pdbPath}" r',errException=Exception('获取pdb源代码路径失败'))
            filepaths = data.splitlines()
            varStrList:List[str] = []
            for filepath in filepaths:
                relFilePath = ''
                if com.isNoneOrEmpty(filepath) or filepath in self.fileNotInRepoDict:
                    continue
                if not os.path.exists(filepath):
                    continue
                if filepath in self.repoFileDict:
                    relFilePath = self.repoFileDict[filepath]
                else:
                    if not self.scm.checkFileInRepo(filepath):
                        self.fileNotInRepoDict.add(filepath)
                        continue
                    relFilePath = self.scm.getRelFilePath(filepath)
                    self.repoFileDict[filepath] = relFilePath
                    if com.isNoneOrEmpty(self.revision):
                        self.revision = self.scm.getRevision()


                varStrList.append(self.scm.getFileVarStrBuild(filepath,relFilePath,self.revision))

                
            if varStrList.__len__() == 0:
                continue

            varStrs = '\t\n'.join(varStrList)

            srcsrvFileContent = com.textwrap.dedent(f'''            SRCSRV: ini ------------------------------------------------
            VERSION=1
            INDEXVERSION=2
            VERCTRL={self.scm.getName()}
            DATETIME={com.getdatetimenow()}
            SRCSRV: variables ------------------------------------------
            EXTRACT_TARGET={self.scm.getLocalRelFilePath()}
            EXTRACT_CMD={self.scm.getDownloadFileCmd()}
            SRCSRVTRG=%EXTRACT_TARGET%
            SRCSRVCMD=%EXTRACT_CMD%
            SRCSRV: source files ---------------------------------------
            {varStrs}
            SRCSRV: end ------------------------------------------------''')
            srcsrvTmpFilePath = 'srcsrv.ini'
            com.savedata(srcsrvFileContent,srcsrvTmpFilePath)

            pdbstrCmd = f'"{self.pdbstrExe}" -w -p:\"{pdbPath}\" -i:\"{srcsrvTmpFilePath}\" -s:srcsrv'
            com.cmd(pdbstrCmd,errException=Exception('pdbstr命令执行失败'))


class SymbolStore:
    def __init__(self,storePath,windowsSDKPath) -> None:
        self.storePath = storePath
        self.storePath = r'\\192.168.2.60\TM_Cache\symbolserver'

        self.windowsSDKPath = windowsSDKPath
        arch = 'x64'
        self.debuggerBinDir = os.path.join(windowsSDKPath,'Debuggers',arch)
        self.symstoreExe = os.path.join(self.debuggerBinDir,'symstore.exe')
        self.historyFilePath = os.path.join(self.storePath,'000Admin','history.txt')

        pass

    def store(self,pdbFilePaths:List[str],productName,versionName,commentMsg):
        responseFilePath = 'pdbs.txt'
        com.savelines(pdbFilePaths,responseFilePath)

        # "C:\Program Files (x86)\Windows Kits\10\Debuggers\x64\symstore.exe" add /s \\192.168.2.60\TM_Cache\symbolserver /f E:\tttt\srcsrvtest\SrcsrvTest\x64\Release\SrcsrvTest.pdb /t "Product Name" /v "Version Name" /c "Comment Msg" /compress
        cmd = f'"{self.symstoreExe}" add /s "{self.storePath}" /f @"{responseFilePath}" /t "{productName}" /v "{versionName}" /c ""{commentMsg}"" /compress'
        com.cmd(cmd,getstdout=False,errException=Exception('符号储存失败'))

        pass
    def delete(self,targetProductName,targetVersionName,deleteSaveId=False):
        # TODO 在history.txt里找
        historyRawData = com.readall(self.historyFilePath)
        historyDatas = historyRawData.splitlines()

        ids = []
        for historyData in historyDatas:
            tmp = historyData.strip(',').split(',')
            if tmp.__len__() == 3:
                # del
                id,action,delId = tmp
                continue
            elif tmp.__len__() == 8:
                # add
                id,action,pathType,date,time,productName,versionName,commentMsg = tmp
                if productName == f'"{targetProductName}"' and versionName == f'"{targetVersionName}"':
                    ids.append(id)
            else:
                com.logout(f'未知操作 {historyData}')
        if ids.__len__() > 1 and deleteSaveId == False:
            raise Exception(f'add 事务存在重名，ids = {ids}，productName={targetProductName},versionName={targetVersionName}')
        elif ids.__len__() == 0:
            raise Exception(f'add 事务不存在，productName={targetProductName},versionName={targetVersionName}')
            
        for id in ids:
            cmd = f'"{self.symstoreExe}" del /s "{self.storePath}" /i {id}'
            com.cmd(cmd,getstdout=False,errException=Exception(f'删除事务失败，id={id},productName={targetProductName},versionName={targetVersionName}'))