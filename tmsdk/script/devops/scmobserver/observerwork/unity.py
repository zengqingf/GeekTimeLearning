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

from scmobserver.observer import Observer,UnityObserver
from scmobserver.commethod import CommitWorkHelper

projRoot = os.path.join(workdir,'Program')
isOpendUnity = False


class MetaScan(UnityObserver):
    def __init__(self,unity:TMUnityManager) -> None:
        super().__init__(False,unity)
    def run(self, commitdata: Tuple[LogCommitState, str],revision:int):
        pass
    def afterRun(self, commitdata: Tuple[LogCommitState, str],revision:int):
        com.logout(f'meta检查')
        if not isOpendUnity:
            self.unity.open_close()

        path = self.unity.unityAssetPath
        states = SVNManager.getStates(path)

        suspects = []


        canCommit = []

        for state in states:
            if not state.path.endswith('.meta'):
                continue
            # 文件名大小写比较
            sourfile = state.path[:len(state.path) - len('.meta')]
            

            if state.item == 'unversioned':
                sourfileState = SVNManager.getStates(sourfile,depth='empty',ext='-v')[0]
                # 漏提交meta
                if sourfileState.item == 'normal':
                    # 目录meta和非特殊meta直接提交
                    # 不可直接提交
                    # {'.ttf', '.ogg', '.fbx', '.tga', '.png', '.PNG', '.asset', '.anim', '.WAV', '.TGA', '.dll', '.obj', '.TTF', '.wav', '.psd', '.jpg', '.FBX', '.Ogg'}
                    # 可直接提交
                    # {'', '.txt', '.asset', '.anim', '.cginc', '.prefab', '.skel', '.tpsheet', '.json', '.fnt', '.shadervariants', '.fontsettings', '.controller', '.bytes', '.cs', '.xml', '.mat', '.physicMaterial', '.shader', '.spriteatlas', '.renderTexture', '.r0'}
                    ext = os.path.splitext(sourfileState.path)[1]
                    if os.path.isdir(sourfileState.path) or ext not in ('.ttf', '.ogg', '.fbx', '.tga', '.png', '.PNG', '.asset', '.anim', '.WAV', '.TGA', '.dll', '.obj', '.TTF', '.wav', '.psd', '.jpg', '.FBX', '.Ogg'):
                        canCommit.append((state.path,'漏提交meta'))
                    else:
                        suspects.append((sourfileState.author,state.path,'漏提交meta'))

            elif state.item == 'missing':
                # 漏删meta
                canCommit.append((state.path,'漏删meta'))
                # suspects.append((state.author,state.path,'漏删meta'))
            elif state.item == 'modified':
                # 漏更新meta
                canCommit.append((state.path,'漏更新meta'))
                # suspects.append((state.author,state.path,'漏更新meta'))
        
        needadd = []
        # 检查meta文件名大小写问题
        while True:
            isfind = False
            needremove = set()
            for name,path,msg in suspects:
                count = 0
                for name2,path2,msg2 in suspects:
                    if path.lower() == path2.lower():
                        count += 1
                        if path != path2:
                            needremove.add((name,path,msg))
                            needremove.add((name2,path2,msg2))
                if count == 2:
                    needadd.append((name,path,'meta文件和源文件大小写不同'))
                    isfind = True
                    break
            if isfind:
                com.removeList(suspects,needremove)
            else:
                break
        

        commitPaths = [x[0] for x in canCommit]
        
        wc = SVNManager.get_root(self.unity.projectPath)

        [SVNManager.add(x[0]) for x in canCommit if x[1] == '漏提交meta']
        SVNManager.changelist('MetaCommit',wc,*commitPaths)
        SVNManager.commit_changelist('MetaCommit','需求修改([打包]/[资源标准化]): 自动提交meta',wc)
        
        
        
        robot = Loader.获取客户端维护机器人()
        # robot = Loader.获取脚本调试机器人()

        title = 'Meta自动提交提醒！！！'
        content = ''
        paths = []
        if commitPaths.__len__() > 0:
            for fullpath,msg in canCommit:
                path = com.convertsep2Win32(fullpath.replace(self.unity.projectPath + os.path.sep,''))
                paths.append(path)
                content += f'{path} {msg} \n\n'
            paths.sort()
            # dingFormat(robot,title,content,'MetaScan.txt')






        suspects += needadd

        dataname = 'MetaScan'
        if suspects.__len__() == 0:
            if CommitWorkHelper.isLastUnstable(dataname):
                CommitWorkHelper.resetUnstable(dataname)
                data = robot.build_text('meta嫌疑人已改邪归正')
                robot.send(data)
            return
        
        
        
        # Meta嫌疑人
        # yangduqi 相对路径 漏删meta
        title = 'Meta嫌疑人出现了！'
        content = ''
        paths = []
        for suspect in suspects:
            name = Loader.根据svnname获取中文名(suspect[0])
            path = com.convertsep2Win32(suspect[1].replace(self.unity.projectPath + os.path.sep,''))
            paths.append(path)
            msg = suspect[2]
            content += f'{name} {path} {msg} \n\n'

        data = CommitWorkHelper.getLastUnstableData(dataname)        
        paths.sort()
        if data != None:
            lastPaths = data['paths']
            lastPaths.sort()
            if lastPaths == paths:
                return
        
        CommitWorkHelper.dingFormat(robot,title,content,'MetaScan.txt')
        CommitWorkHelper.setUnstable(dataname,data={'paths':paths})


class CorrectAssetsName(UnityObserver):
    def __init__(self,unity:TMUnityManager) -> None:
        super().__init__(True,unity)
        paths = []
        self.methodname = 'ReplaceAssetsName.CorrectAssetsName'
        

    def run(self,commitdata:List[Tuple[LogCommitState,str]]):
        pass
    
    @workspace
    def afterRun(self, commitdata: Tuple[LogCommitState, str],revision:int):
        # 开关unity让资源自动改名
        self.unity.open_close()
        datalist = []
        paths = []
        # 先检查本地
        # 再修正，根据修正添加
        # 检查本地是否有可提交mat文件，若有则加载旧的和新的比较名字
        respath = os.path.join(self.unity.unityResourcesPath)
        states = SVNManager.getStates(respath)
        for state in states:
            if state.item != 'modified':
                continue
            ext = os.path.splitext(state.path)[1]
            if ext != '.mat':
                continue
            # 过程中被删了，直接跳过这个文件
            if not SVNManager.isExsits(state.path):
                continue
            SVNManager.export_ex(state.path,'old.mat')
            
            self.unity.binary2text('old.mat','old.txt')
            self.unity.binary2text(state.path,'new.txt')
            olddata = com.readall('old.txt')
            newdata = com.readall('new.txt')
            m = re.search('m_Name "(.*?)"',olddata)
            if m == None:
                raise StopException('匹配mat文件内部文件名失败',{'path':state.path})
            oldname = m.group(1)
            m = re.search('m_Name "(.*?)"',newdata)
            if m == None:
                raise StopException('匹配mat文件内部文件名失败',{'path':state.path})
            newname = m.group(1)
            # 只处理名称不同
            if oldname != newname:
                paths.append(state.path)
                localpath = com.abspath2relpath(state.path,self.unity.projectPath)
                winpath = com.convertsep2Win32(localpath)
                datalist.append((winpath,oldname,newname))

        rawdata = self.unity.loadReportData(self.methodname)
        if not com.isNoneOrEmpty(rawdata):
            datalist += com.list2block(rawdata.split('\n'),3)
        if datalist.__len__() == 0:
            return
        
        # 提交
        SVNManager.changelist('CorrectAssetsName',projRoot,*paths)
        SVNManager.commit_changelist('CorrectAssetsName','需求修改[提交监控]/[修正资源名称]):修正资源名称',projRoot)
        # 钉钉发出警告

        robot = Loader.获取客户端维护机器人()
        # robot = Loader.获取脚本调试机器人()
        title = '自动修正资源名称警告！！！'
        content = ''
        for data in datalist:
            content += data[0] + '\n\n'
            content += robot.markdown_drawblock(f'{data[1]} -> {data[2]}')
        
        CommitWorkHelper.dingFormat(robot,title,content,docname='CorrectAssetsName.txt')


