# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com
from comlib.conf.loader import Loader
from comlib.conf.ref import *
from comlib.svnm import SVNManager
from comlib.pathm import Path
from comlib.gitm import GitManager,GitlabManager
from comlib.secm import SecretManager


class SCMWorkflow:
    def __init__(self,rootUrl,branchName,extBranchName) -> None:
        self.rootUrl= rootUrl
        self.branchName = branchName
        self.extBranchName = extBranchName
        pass
    def repoCheck(self):
        pass
    def repoFrameworkBuild(self):
        pass
    def trunk2Release(self,version,msg,releaseDesc):
        pass
    def release2Online(self,version,msg,releaseDesc):
        '''
        发布release版本到指定分支
        '''
        pass

class SVNWorkflow(SCMWorkflow):
    def __init__(self, rootUrl,branchName,extBranchName=None) -> None:
        '''
        主干必须放根目录！！！，extBranchName的意义是为了支持体验服、线上服等多发布分支同时开发
        '''
        super().__init__(rootUrl,branchName,extBranchName)
        if '/' in branchName:
            raise Exception(f'！！！！！！主干不在根目录！！！！！！')

        self.trunkUrl = f'{rootUrl}/{branchName}'
        
        self.branchesUrl = f'{rootUrl}/branches'
        self.releaseRelUrl = 'release'
        self.onlineRelUrl = 'online'
        self.tagsRelUrl = 'tags'
        self.releaseUrl = f'{rootUrl}/{self.releaseRelUrl}'
        self.onlineUrl = f'{rootUrl}/{self.onlineRelUrl}'
        self.tagsUrl = f'{rootUrl}/{self.tagsRelUrl}'
        # branchName这个分支名是没加描述头的
        # 这个是加描述头的
        self.finalBranchName = self.branchName
        if not com.isNoneOrEmpty(self.extBranchName):
            self.finalBranchName = f'{self.branchName}_{self.extBranchName}'
        self.releaseBranchUrl = f'{self.releaseUrl}/{self.finalBranchName}'
        self.releaseBranchRelUrl = f'{self.releaseRelUrl}/{self.finalBranchName}'
        self.onlineBranchUrl = f'{self.onlineUrl}/{self.finalBranchName}'
        self.onlineBranchRelUrl = f'{self.onlineRelUrl}/{self.finalBranchName}'

    @staticmethod
    def buildExtBranchName(gameName,platform,extName=None):
        '''
        目标url格式 {rootUrl}/[release|online]/{projectTrunkName|engineTrunkName}_{gameName}_{platform}_{extName}
        '''
        finalExtBranchName = f'{gameName}_{platform}'
        if extName != None:
            finalExtBranchName = f'{finalExtBranchName}_{extName}'
        return finalExtBranchName
    def repoCheck(self):
        return SVNManager.isExsits(self.branchesUrl) and SVNManager.isExsits(self.releaseUrl) and SVNManager.isExsits(self.onlineUrl) and SVNManager.isExsits(self.tagsUrl)
    def repoFrameworkBuild(self):
        if not SVNManager.isExsits(self.branchesUrl):
            SVNManager.mkdir(self.branchesUrl,'创建branches目录')
        if not SVNManager.isExsits(self.releaseUrl):
            SVNManager.mkdir(self.releaseUrl,'创建release目录')
        if not SVNManager.isExsits(self.onlineUrl):
            SVNManager.mkdir(self.onlineUrl,'创建online目录')
        if not SVNManager.isExsits(self.tagsUrl):
            SVNManager.mkdir(self.tagsUrl,'创建tags目录')
    def trunk2Release(self,version,msg,releaseDesc):
        self.cover(self.branchName,self.releaseBranchRelUrl)

    def release2Online(self,version,msg,releaseDesc):
        self.cover(self.releaseBranchRelUrl,self.onlineBranchRelUrl)





    def cover(self,fromRelUrl,toRelUrl):
        fromUrl = f'{self.rootUrl}/{fromRelUrl}'
        toUrl = f'{self.rootUrl}/{toRelUrl}'
        fromVersion = SVNManager.version(fromUrl)

        if SVNManager.isExsits(toUrl):
            toVersion = SVNManager.version(toUrl)
            # 备份到tags
            message = f'需求修改([svn]/[备份]): 因{fromRelUrl}版本号{fromVersion}覆盖操作，从{toRelUrl}版本号{toVersion}备份'

            backupUrl = f'{self.tagsUrl}/{toRelUrl.replace("/","-")}#{com.timemark}_R{fromVersion}》{toVersion}'
            SVNManager.copy(toUrl,backupUrl,toVersion,message)
            # 移除旧分支
            message = f'需求修改([svn]/[删除]): 因从{fromRelUrl}版本号{fromVersion}对{toRelUrl}版本号{toVersion}进行覆盖操作，删除旧分支，备份地址{backupUrl}'
            SVNManager.remove(toUrl,message)
            # 覆盖
            message = f'需求修改([svn]/[覆盖]): 从{fromRelUrl}版本号{fromVersion}对{toVersion}版本号{toVersion}进行覆盖操作，备份地址{backupUrl}'
            SVNManager.copy(fromUrl,toUrl,fromVersion,message)
        else:
            # 新建
            message = f'需求修改([svn]/[拷贝]): 从{fromRelUrl}版本号{fromVersion}拷贝到{toUrl}'
            SVNManager.copy(fromUrl,toUrl,fromVersion,message)


        
class GitWorkflow(SCMWorkflow):
    '''
    使用Gitlab API进行操作！！！
    '''
    def __init__(self, rootUrl,branchName,extBranchName=None) -> None:
        super().__init__(rootUrl,branchName,extBranchName=None)
        token = SecretManager.getSecData().trygetvalue('gitlab_jenkins','token')
        self.gitlab = GitlabManager(rootUrl,token)


    def repoCheck(self):
        return GitManager.isBranchVaild(self.rootUrl,self.branchName)

    def repoFrameworkBuild(self):
        pass
    def trunk2Release(self,version,msg,releaseDesc):
        self.gitlab.tag(version,self.branchName,msg,releaseDesc)
        
    def release2Online(self,version,msg,releaseDesc):
        '''
        发布release版本到指定分支
        '''
        self.gitlab.tag(version,self.branchName,msg,releaseDesc)