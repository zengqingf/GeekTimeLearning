# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace
from comlib.conf.loader import Loader
from comlib.conf.ref import *

from builder.build import argparse,UE4AndroidBuilder,LightmassQuality
from comlib import UE4Engine,SVNManager,Path,Srcsrv_GitLab,SrcsrvManager,SymbolStore,SecretManager,SVNWorkflow
from comlib import GitManager
import glob



def buildEngine():
    # 简单写一下先

    parser = argparse.ArgumentParser()
    # 打包配置
    parser.add_argument('--enginePath',required=True)

    args = parser.parse_args()
    enginePath = args.enginePath

    # 本地拷贝的位置，选在引擎同磁盘，防止出现磁盘之间数据拷贝
    if com.isWindows():
        engineSavePath = f'{os.path.splitdrive(enginePath)[0]}\\UE'
    else:
        raise Exception(f'目前只支持在windows打包')
    
    git = GitManager(enginePath)
    branchName = git.getBranchName()
    ec = Loader.load(engineconf,use_defalt=True)
    engineWorkflow = SVNWorkflow(ec.binRepoUrl,branchName,None)

    workingCopyDir = UE4Engine.getEngineWCPath(branchName,True,engineSavePath)
    # 轻量级引擎不能打包
    workingCopyDir_light = UE4Engine.getLightEngineWCPath(branchName,engineSavePath)
    
    # 这里已经由jenkins拉好处理完成了，
    engine = UE4Engine(enginePath)
    engine.allowFASTBuild = False

    engineVersion = engine.engineVersion
    # 引擎要生成2份,一份轻量级引擎,给美术策划用,一份预编译引擎,给程序用,这两份都得提交svn,引擎组用纯源码本地编译的引擎
    svnPath_installed = UE4Engine.getEngineSVNPath(engineWorkflow.trunkUrl,installed=True)
    svnPath_light = UE4Engine.getEngineSVNPath(engineWorkflow.trunkUrl,installed=False)

    if not SVNManager.isInversion(svnPath_installed):
        SVNManager.mkdir(svnPath_installed)
    if not SVNManager.isInversion(svnPath_light):
        SVNManager.mkdir(svnPath_light)

    # 拉取引擎的工作拷贝
    Path.ensure_svn_pathexsits(workingCopyDir,svnPath_installed)
    Path.ensure_svn_pathexsits(workingCopyDir_light,svnPath_light)
    
    # 检查本地的引擎拷贝
    
    # android 环境 ANDROID_HOME NDKROOT
    env = Loader.getenvconf()
    os.putenv('ANDROID_HOME',env.androidsdkpath)
    os.putenv('NDKROOT',env.ndkpath)

    # ios 环境
    # engine = UE4Engine(r'D:\TM_UE4.25_GIT')
    # com.tm.a8.test这个是随便写的
    c = ioschannelconf('','com.tm.a8.test','com.tm.a8.test','a8_debug_dev')
    engine.SetIOSEnv(c)

    # 创建引擎标识符
    EngineAssociation = f'TM{engineVersion}'
    # 凑合一下
    out,code = com.cmd(f'git log -1 --pretty=format:"%H"',cwd=enginePath,getstdout=True)
    gitVersion = out.strip()


    # graph打包
    graphTarget = 'Copy To Staging Directory'
    # scriptPath = 'Engine/Build/Graph/Examples/BuildEditorAndTools.xml'
    scriptPath = 'Engine/Build/InstalledEngineBuild.xml'
    # out,code = engine.runUATCmd(f'BuildGraph -Script={scriptPath} -Target="{graphTarget}" -set:EditorTarget=UE4Editor')
    
    # out,code = engine.runUATCmd(f'BuildGraph -Script={scriptPath} -Target="Make Installed Build Win64" \
    # -set:LocalInstalledDirRoot="{workingCopyDir}" -set:LocalBaseInstalledDirRoot="{workingCopyDir_light}" \
    # -set:WithIOS=true \
    # -set:WithDDC=false -set:WithWin64=false -set:WithAndroid=false -set:WithWin32=false -set:WithTVOS=false -set:WithLinux=false -set:WithLinuxAArch64=false \
    # -set:WithLumin=false -set:WithLuminMac=false -set:WithHoloLens=false -set:WithFullDebugInfo=false \
    # -set:VS2019=true -buildmachine -nop4 -noxge')
    #  -clean

    out,code = engine.runUATCmd(f'BuildGraph -Script={scriptPath} -Target="Make Installed Build Win64" \
    -set:LocalInstalledDirRoot="{workingCopyDir}" -set:LocalBaseInstalledDirRoot="{workingCopyDir_light}" \
    -set:WithAndroid=true -set:WithIOS=true -set:WithClient=true -set:WithServer=true \
    -set:WithWin32=false -set:WithTVOS=false -set:WithLinux=true -set:WithLinuxAArch64=false \
    -set:WithLumin=false -set:WithLuminMac=false -set:WithHoloLens=false -set:WithFullDebugInfo=false \
    -set:VS2019=true -buildmachine -clean -nop4 -noxge')

    # -set:VS2019=true 默认是VS2017

    matchpatten = os.path.join(workingCopyDir,'Engine','**','*.sh')
    matchpatten_light = os.path.join(workingCopyDir_light,'Engine','**','*.sh')
    shellFiles = glob.glob(matchpatten,recursive=True)
    shellFiles_light = glob.glob(matchpatten_light,recursive=True)

    # 创建引擎注册文件
    content = com.textwrap.dedent(f'''
    @echo off
    REG ADD "HKEY_CURRENT_USER\\SOFTWARE\\Epic Games\\Unreal Engine\\Builds" /f /v {EngineAssociation} /t REG_SZ /d %~dp0%''')
    com.savedata(content,f'{workingCopyDir}\\双击注册安装版引擎.bat')
    
    SVNManager.add(workingCopyDir)
    # 添加sh文件的可执行性
    SVNManager.setExecutable(*shellFiles)
    SVNManager.commit(workingCopyDir,f'[机器人] 提交{EngineAssociation}安装版引擎,git版本:{gitVersion}',deleteMissing=True)


    # 轻量级引擎提交
    content = com.textwrap.dedent(f'''
    @echo off
    REG ADD "HKEY_CURRENT_USER\\SOFTWARE\\Epic Games\\Unreal Engine\\Builds" /f /v {EngineAssociation}L /t REG_SZ /d %~dp0%''')
    com.savedata(content,f'{workingCopyDir_light}\\双击注册轻量级引擎.bat')
    
    SVNManager.add(workingCopyDir_light)
    # 添加sh文件的可执行性
    SVNManager.setExecutable(*shellFiles_light)
    SVNManager.commit(workingCopyDir_light,f'[机器人] 提交{EngineAssociation}轻量级引擎,git版本:{gitVersion}',deleteMissing=True)


    bot = Loader.获取出包机器人()
    data = bot.build_text(f'虚幻{EngineAssociation}引擎，git版本{gitVersion}已完成svn提交')
    bot.send(data)
    





    SrcAndSymbolServer(enginePath,EngineAssociation,gitVersion,f'藤木虚幻引擎，git版本号{gitVersion}')
    data = bot.build_text(f'虚幻{EngineAssociation}引擎，git版本{gitVersion}已发布',isAtAll=True)
    bot.send(data)



    



def SrcAndSymbolServer(enginePath,productName, versionName, commentMsg):
    try:
        windowsSDKPath = r'C:\Program Files (x86)\Windows Kits\10'
        # 源服务器 pdb格式的应该都没问题
        com.logout(f'[Srcsrv] 源服务器创建中')
        pdbFilePath_glob = os.path.join(enginePath,'Engine','**','Binaries','**','*.pdb')
        pdbFilePaths = glob.glob(pdbFilePath_glob,recursive=True)
        token = SecretManager.getSecData('gitlab_jenkins','token')
        srcsrv_scm = Srcsrv_GitLab(enginePath,token,'https://192.168.2.12:8443/svn/Tenmove_Project_A8/automationData/srcsrvToken.txt')
        srcsrvm = SrcsrvManager(srcsrv_scm,windowsSDKPath)
        srcsrvm.modifyPDB(pdbFilePaths)
        # 符号服务器
        com.logout(f'[Symbol Server] 符号服务器创建中')
        symStore = SymbolStore(None,windowsSDKPath)
        symStore.store(pdbFilePaths,productName,versionName,commentMsg)
    except Exception as e:
        import traceback
        traceback.print_exc()
        com.logout(f'[server] 服务器创建失败')
if __name__ == "__main__":
    buildEngine()
    # SrcAndSymbolServer()
    pass