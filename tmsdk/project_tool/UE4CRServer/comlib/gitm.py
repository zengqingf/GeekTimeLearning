# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com
from comlib.conf.loader import Loader
from comlib.conf.ref import *
from comlib.binm import BinManager



import requests,re

from urllib3.util import parse_url
from typing import List,Tuple
from comlib.logm import Log
ln = 'Git'


class GitState():
    def __init__(self,stateStrInfo:str) -> None:
        # ?? 未纳入版本控制
        # M  暂存区和仓库存在差异
        #  M 工作区和暂存区存在差异
        # MM 暂存区和仓库存在差异，工作区和暂存区存在差异
        # A  添加入暂存区，工作区和暂存区无差异
        # AM 添加入暂存区，工作区和暂存区有差异
        # AD 添加入暂存区，工作区内被删除
        #  D 工作区内被删除
        # D  缓存区内被删除

        stateStrInfo = stateStrInfo.strip()
        workcloneflag = stateStrInfo[0]
        bufferflag = stateStrInfo[1]
        if workcloneflag == 'M' or bufferflag == 'M':
            self.state = 'modify'
        elif workcloneflag == 'A':
            self.state = 'add'
        elif workcloneflag == '?':
            self.state = 'noVersion'
        elif workcloneflag == 'D':
            self.state = 'delete'
        else:
            raise Exception(f'self.state未赋值 info   {stateStrInfo}')
        
        self.path = stateStrInfo[2::]

class GitLog():
    def __init__(self,files,revision,author,commiter,msg,branchName) -> None:
        self.files = files
        self.revision = revision
        self.author = author
        self.commiter = commiter
        self.msg = msg
        self.branchName = branchName
        







isGitInit = False
account_git = ''
password_git = ''
if not isGitInit:
    from comlib.secm import SecretManager
    # SecretManager.getSecKey('ssh','gitlab_jenkins')
    account_git = SecretManager.getSecData('gitlab_jenkins','account')
    password_git = SecretManager.getSecData('gitlab_jenkins','password')



def basecmd_G(cmd,workspace=None,errException=None,**kv):
    cmd = f'git {cmd}'
    if workspace == None:
        workspace = workdir
    # 屏蔽auth字段
    import re
    m = re.search('//(.*:.*)@',cmd)
    cmd_sec = cmd
    if m != None:
        auth = m.group(1)
        cmd_sec = cmd.replace(auth,'***:***')
    Log.info(f'{cmd_sec}',ln)
    return com.cmd(cmd,errException=errException,showlog=False,cwd=workspace,encoding='utf-8',**kv)






class GitManager:
    def __init__(self,workspace) -> None:
        self.workspace = workspace
        self.dotGitDir = os.path.join(self.workspace,'.git')
        self.configFilePath = os.path.join(self.dotGitDir,'config')

        # 设置配置
        # from comlib.pathm import Path

        # confDir = BinManager.get_conf_dir('git')
        # Path.ensure_pathnewest(os.path.join(confDir,'config.cfg'),self.configFilePath)


        
        pass
    def basecmd(self,cmd,errException=None,**kv):
        return basecmd_G(cmd,errException=errException,workspace=self.workspace,**kv)
    @staticmethod
    def clone(url,savePath,branchName='master'):
        wks = os.getcwd()
        out,code = basecmd_G(f'clone --branch {branchName} "{url}" "{savePath}"',workspace=wks,errException=f'拉取{url}:{branchName}到{savePath}失败')
        pass
    @staticmethod
    def isBranchVaild(url,branchName):
        url = com.addAuth2URL(account_git,password_git,url)
        cmd = f'ls-remote --exit-code {url} {branchName}'
        out,code = basecmd_G(cmd)
        return code == 0
    def getRevision(self,branch='HEAD'):
        cmd = f'rev-parse {branch}'
        out,code = self.basecmd(cmd,errException=Exception('获取版本号失败'))
        revision = out.strip()
        return revision
        
    def getState(self,relPath)->List[GitState]:
        '''不支持空文件夹'''
        cmd = f'status -s --untracked-files "{relPath}"'
        out,code = self.basecmd(cmd,errException=Exception(f'获取{relPath}状态失败'),encoding='utf-8')
        lines = com.remove_space(out.splitlines())
        status:List[GitState] = []
        for line in lines:
            status.append(GitState(line.strip()))
        return status
    def getLog(self,relPath,count=1,errException=Exception("获取日志失败"))->List[GitLog]:
        logs:List[GitLog] = []

        # --name-only 输出改动文件
        out,code = self.basecmd(f'log -{count} --pretty=format:"^%H$%T$%an$%cn$%s$%D" --name-only "{relPath}"',errException=errException)
        if code != 0:
            return logs
        logdatas = out.split('^')
        for logdata in logdatas:
            if com.isNoneOrEmpty(logdata):
                continue
            tmp = logdata.splitlines()
            data = tmp[0]
            files = tmp[1::]
            tmp = data.split('$')
            revision = tmp[0]
            treerevision = tmp[1]
            author = tmp[2]
            commiter = tmp[3]
            msg = tmp[4]
            # branchName = re.search(r'\((.*?), .*?/(.*)\)',tmp[5]).group(2) # (HEAD, origin/UE4.25_Master)
            branchName = tmp[5].split(', ')[-1].strip() 
            if '/' in branchName:
                branchName = branchName.split('/')[-1]
            logs.append(GitLog(files,revision,author,commiter,msg,branchName)) #HEAD, origin/UE4.25_Master, UE4.25_Master
        return logs    
    def getBranchName(self):
        log = self.getLog('.',errException=Exception('获取分支名失败了'))[0]
        return log.branchName

    def isFileExists(self,fileRelPath):
        logs = self.getLog(fileRelPath,errException=None)
        if logs.__len__() == 0:
            return False
        return True
    def getRepoUrl(self):
        out,code = self.basecmd(f'remote -v',errException=Exception('获取远程仓库url失败'))
        # origin\thttp://192.168.2.110/jenkins/srcsrvtest.git (fetch)
        # origin\thttp://192.168.2.110/jenkins/srcsrvtest.git (push)
        tmp = re.findall(r'\t(.*) ',out)
        if tmp.__len__() == 2:
            fetchUrl,pushUrl = tmp
        elif tmp.__len__() == 1:
            fetchUrl,pushUrl = tmp+[None]
        else:
            raise Exception(f'|{out}| fetchUrl不存在')
        if fetchUrl != pushUrl:
            com.logout(f'[Warning] git URL不一致，fetchUrl={fetchUrl} pushUrl={pushUrl}')
            
        return fetchUrl

class GitlabManager:
    def __init__(self,gitUrl:str,token:str) -> None:
        
        if gitUrl.startswith('http'):
            # http://192.168.2.110/tm_engine/unrealengine.git
            urlStruct = parse_url(gitUrl)

            self.domain = urlStruct.netloc # {ip}:{port}
            self.scheme = urlStruct.scheme # http/https
            route = urlStruct.request_uri # /{groupName}/{projectName}.git
        elif gitUrl.startswith('git'):
            # git@192.168.2.110:tm_engine/unrealengine.git
            tmp = gitUrl.split('@')[1]
            tmp2 = tmp.rsplit(':',1)
            self.domain = tmp2[0]
            self.scheme = 'http'
            route = '/'+tmp2[1]

        projectIdOrProjectPath = route[1:route.__len__() - 4] # {groupName}/{projectName}
        self.projectIdOrProjectPath = com.urlencode(projectIdOrProjectPath,plus=True)
        self.token = token

    def getApiUrl(self,routePath:str):
        routePath = routePath.strip('/')
        routePath = routePath.replace(':id',self.projectIdOrProjectPath)

        url = f'{self.scheme}://{self.domain}/api/v4/{routePath}'
        return url
    
    def post(self,routePath,err=None,**args):
        header = {
            'PRIVATE-TOKEN':self.token
        }
        res = requests.post(self.getApiUrl(routePath),headers=header,params=args)
        data = res.text
        if err != None and not res.ok:
            raise err
        return data,res.ok
        

    def get(self,routePath,err=None,**args):
        header = {
            'PRIVATE-TOKEN':self.token
        }
        res = requests.get(self.getApiUrl(routePath),headers=header,params=args)
        data = res.text
        if err != None and not res.ok:
            raise err
        return data,res.ok
    def delete(self,routePath,err=None,**args):
        header = {
            'PRIVATE-TOKEN':self.token
        }
        res = requests.delete(self.getApiUrl(routePath),headers=header,params=args)
        data = res.text
        if err != None and not res.ok:
            raise err
        return data,res.ok
    
    def tag(self,name,ref,msg,releaseDesc):
        self.post('/projects/:id/repository/tags',tag_name=name,ref=ref,message=msg,release_description=releaseDesc,err=Exception(f'tag失败,name={name} ref={ref} msg={msg}'))
    def deleteTag(self,name):
        self.delete(f'/projects/:id/repository/tags/{name}',err=Exception(f'delete tag失败,name={name}'))
    





