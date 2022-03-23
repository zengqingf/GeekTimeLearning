# -*- encoding: utf-8 -*-
import os,sys
thisdir = os.path.abspath(os.path.dirname(__file__))
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.comobj import *

from comlib import DockerManager,VirtualEnvManager
from argparse import ArgumentParser


ftpArchiveFileFormat = '/client_ftp/DevOps/codechecker/analyzeEnv/codechecker-{branchName}.tar.gz'

@workspace
def buildCodeChecker_AnalyzeSide(args):
    branchName = args.branch

    sourceSavePath = 'codechecker'
    # GitManager.clone('http://192.168.2.110/jenkins/codechecker.git',sourceSavePath)

    # 从setup.py拿，然后添加第四位版本号 e.g. version="6.16.0"
    version = '6.17.0'
    archiveName = f'codechecker-{version}.tar.gz'

    # build镜像
    imgName = 'codechecker_analyze_build_env'
    containerName = 'codechecker_analyze_build_env_1'
    try:
        imgCid = DockerManager.BuildImage(os.path.join(thisdir,'Dockerfile.BuildEnv'),imgName,argkv={'CC_BRANCH':branchName})
        # 创建容器
        containerCid = DockerManager.CreateContainer(imgName,containerName)
        # 放入codechecker
        # DockerManager.Copy(sourceSavePath,f'{containerName}:/codechecker')
        # make dist
        # DockerManager.ExecInContainer('/bin/sh -c "cd /codechecker && make dist"',containerName)
        # 拿出/codechecker/dist/xxx
        DockerManager.Copy(f'{containerName}:/codechecker/dist/{archiveName}','.')
        # 上传ftp
        G_ftp.upload(archiveName,ftpArchiveFileFormat.format(branchName=branchName),overwrite=True)
    finally:
        DockerManager.RemoveContainer(containerName)
        DockerManager.RemoveImage(imgName)


def installCodeChecker(args):
    branchName = args.branch
    vpath = os.path.join(workdir,f'venv-{branchName}')
    vem = VirtualEnvManager(vpath)
    vem.create()
    httpArchiveFile = G_ftp.ftppath2httppath(ftpArchiveFileFormat.format(branchName=branchName))
    # 先用他们的东西测一下
    # vem.installPak(httpArchiveFile)
    # 脚本运行环境
    vem.installPak('codechecker')
    vem.installPak('thrift')
    vem.installPak('requests')
    # BUGFIX：去除store的多进程操作
    storeFilePath = os.path.join(vem.sitePackagePath,'codechecker_client','cmd','store.py')
    com.replace_filecontent(storeFilePath,'executor.map','map')


def main(args_raw=None):
    parser = ArgumentParser()
    subparser = parser.add_subparsers(title='subparsers')
    subparser_build = subparser.add_parser('build')
    subparser_build.add_argument('--branch',default='master',help='使用的指定分支的包进行打包')
    subparser_build.set_defaults(func=buildCodeChecker_AnalyzeSide)




    subparser_install = subparser.add_parser('install')
    subparser_install.add_argument('--branch',default='master',help='使用的指定分支的包进行安装')
    subparser_install.set_defaults(func=installCodeChecker)

    args = parser.parse_args(args_raw)
    args.func(args)
    pass


if __name__ == '__main__':
    main()
