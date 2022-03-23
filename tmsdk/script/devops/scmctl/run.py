# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace,SVNManager,Path,GitManager,GitlabManager,SecretManager
from comlib.conf.loader import Loader
from comlib.conf.ref import *
from comlib import SCMWorkflow,SVNWorkflow,GitWorkflow,SVNManager,RecordTool,Version

from argparse import ArgumentParser

def run():
    parser = ArgumentParser()
    # -----------------------------------发布参数开始
    # 发布版本信息后面自动生成，不需要添加参数
    parser.add_argument('--platform',required=True)
    # parser.add_argument('--channel',required=True)

    parser.add_argument('--projectTrunkName',required=True,default='trunk',help='要发布的项目分支名称')
    parser.add_argument('--engineTrunkName',required=True,default='trunk',help='要发布的引擎分支名称')
    parser.add_argument('--extName',required=True,type=com.str2Noneable,help='发布的目标分支额外名称')
    parser.add_argument('--releaseMode',required=True,default='toRelease',help='发布模式，从开发分支到发布分支还是从发布分支到线上分支')

    parser.add_argument('--releaseMessage',required=True,default='No Release Message !',help='发布信息例如：2021/xx/xx v1')
    parser.add_argument('--releaseDescribe',required=True,default='No Release Describe !',help='发布描述，用来搜索的，例如：2021/xx/xx包')

    # 非必要的，用于手动指定版本号而不是从仓库获取，通常用于流水线流程中，手动执行不要填
    parser.add_argument('--revision',required=False,default=None,type=com.str2Noneable)

    # -----------------------------------发布参数结束 
    args = parser.parse_args()
    # -----------------------------------发布相关开始
    
    platform = args.platform
    # channel = args.channel
    projectTrunkName = args.projectTrunkName
    engineTrunkName = args.engineTrunkName
    # toBranchName = args.toBranchName
    releaseMode = args.releaseMode
    extName = args.extName

    gameName = Loader.getgame()
    bc = Loader.load(projectconf)
    ec = Loader.load(engineconf,use_defalt=True)

    # 目标url格式 {rootUrl}/[release|online]/{projectTrunkName|engineTrunkName}_{gameName}_{platform}_{extName}
    finalExtBranchName = SVNWorkflow.buildExtBranchName(gameName,platform,extName)

    workflow_engineSource = GitWorkflow(ec.srcRepoUrl,engineTrunkName,finalExtBranchName)
    workflow_engineBin = SVNWorkflow(ec.binRepoUrl,engineTrunkName,finalExtBranchName)
    workflow_project = SVNWorkflow(bc.reporoot,projectTrunkName,finalExtBranchName)

    if not workflow_engineSource.repoCheck():
        raise Exception('git分支不存在')
        # workflow_engineSource.repoFrameworkBuild()
    if not workflow_engineBin.repoCheck():
        workflow_engineBin.repoFrameworkBuild()
    if not workflow_project.repoCheck():
        workflow_project.repoFrameworkBuild()

    # 提升versioncode，提升所需大版本号 1.x.1.xxxx -> 1.x.2.xxxx
    recordTool = RecordTool()
    
    projectRevision = args.revision
    if projectRevision =='HEAD' or com.isNoneOrEmpty(projectRevision):
        projectRevision = SVNManager.version_top(f'{workflow_project.rootUrl}/{projectTrunkName}')
    # 根据最大版本号打标签，后续也会根据最大版本号进行同步提升，所以没啥问题
    version = Version(client=projectRevision,clientCode=recordTool.getversioncode_max(platform))
    releaseMessage = f'版本{version}，{args.releaseMessage}'


    if releaseMode == 'toRelease':
        releaseDescribe = f'版本{version}，project svn路径 {workflow_project.releaseBranchUrl}，engineBin svn路径 {workflow_engineBin.releaseBranchUrl}，{args.releaseDescribe}'
        workflow_project.trunk2Release(version.__str__(),releaseMessage,releaseDescribe)
        workflow_engineBin.trunk2Release(version.__str__(),releaseMessage,releaseDescribe)
        # workflow_engineSource.trunk2Release(version.__str__(),releaseMessage,releaseDescribe)
    else:
        releaseDescribe = f'版本{version}，project svn路径 {workflow_project.onlineBranchUrl}，engineBin svn路径 {workflow_engineBin.onlineBranchUrl}，{args.releaseDescribe}'
        workflow_project.release2Online(version.__str__(),releaseMessage,releaseDescribe)
        workflow_engineBin.release2Online(version.__str__(),releaseMessage,releaseDescribe)
        # workflow_engineSource.release2Online(version.__str__(),releaseMessage,releaseDescribe)


    # -----------------------------------发布相关结束 

if __name__ == '__main__':
    # sys.argv += ['--platform=android','--channel=debug','--fromBranchName=trunk','--toBranchName=A8_Android','--releaseMode=toOnline','--releaseMessage=发布信息','--releaseDescribe=发布描述']
    run()