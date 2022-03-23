# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,errorcatch_func,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com

from comlib import Path,JsonFile,TMUnityManager,内网发包群苦工,SVNManager ,一点零注意了机器人
from comlib.conf.loader import Loader
from comlib import HTTPManager,ThreadManager,SVNManager,DictUtil
from comlib import projlock,workspace

from comlib.comobj import *

# 一点零注意了机器人 = 内网发包群苦工
tsp = HTTPManager.gettsp_BeiJin()
tsp_readable = com.tsp2readable(tsp)
tsp_datetime = com.tsp2datetime(tsp)

@errorcatch(HIGH)
class BaseToolUnit(object):
    def __init__(self,projpath,unitypath,chinese,conf):
        super().__init__()
        self.projpath = projpath
        self.unitypath = unitypath
        self.chinese = chinese
        self.weekday = list(map(lambda x: int(x),conf['weekday']))
        self.method = conf['fullmethodname']
        self.show_report = conf['show_report']

        self.isOpen = conf['open']

        self.rel_artifact_path = self.method
        self.rel_reportfile_path = os.path.join(self.method,'report.txt')
        self.artifact_path = os.path.join(self.projpath,self.method)
        self.reportfile_path = os.path.join(self.projpath,self.method,'report.txt')
        

        self.abs_artifact_path = os.path.join(self.projpath,self.artifact_path)
        self.abs_reportfile_path = os.path.join(self.projpath,self.reportfile_path)
        # self.jenkinswork_path = os.path.join(projpath,'jenkinswork.json')
        self.unity = TMUnityManager(self.unitypath,self.projpath,tsp_readable)

        self.ftppath = f'/DevOps/ClientToolArtifact/{self.getBranchDesc()}/{self.method}/{tsp_readable}'
    def canRun(self):
        if not self.isOpen:
            print(f'{self.chinese}开关已关闭')
            return False

        nowweekday = tsp_datetime.isocalendar()[2]
        if nowweekday not in self.weekday:
            print(f'今天星期{nowweekday}，{self.chinese}{self.method}不运行，运行时间是{self.weekday}')
            return False
        return True
    def run(self):
        if not self.canRun():
            return
        
        out,code = self.unity.run_cmd(self.method)
        if code != 0:
            raise DingException(f'运行{self.method}失败',locals())

        self.afterRun()
        
        self._upload()
        self._ding()
        self._clear()
    def afterRun(self):
        """
        c#代码运行之后执行
        """
        pass
    def _upload(self):
        if not os.path.exists(self.abs_artifact_path):
            raise DingException(f'{self.abs_artifact_path}路径不存在',locals())
        G_ftp.upload(self.abs_artifact_path,self.ftppath,showlog=True)
    def getBranchDesc(self):
        return '-'.join([
            Loader.getgame(),
            SVNManager.get_branch_Desc(self.projpath)
        ])
    def _ding(self):
        remote_reportfile_path = '/'.join([
            self.ftppath,
            self.rel_reportfile_path
        ])
        allstr = com.readall(self.abs_reportfile_path)
        if allstr == 'ok':
            data = 一点零注意了机器人.build_text(f'{self.chinese}{self.method}执行完毕\n\n无异常')
        else:
            if self.show_report == 'true':
                data = 一点零注意了机器人.build_text(f'{self.chinese}{self.method}执行完毕，有异常\n\n{allstr}')
            else:
                url = f'http://192.168.2.147/dnl{remote_reportfile_path}'
                data = 一点零注意了机器人.build_link(self.chinese,f'{self.chinese}{self.method}执行完毕',url)
        一点零注意了机器人.send(data)
    def _clear(self):
        Path.ensure_pathnotexsits(self.abs_artifact_path)
class ToolUnit_UwaProjScan(BaseToolUnit):
    def __init__(self, projpath, unitypath,method,conf):
        super().__init__(projpath, unitypath,method,conf)
    def run(self):
        pass
    def upload(self):
        pass
    def ding(self):
        pass
class SVNState(object):
    def __init__(self,stateStr:str):
        super().__init__()
        infoList = list(filter(lambda x: x != '',stateStr[1::].split(' ')))
        if stateStr[0] == '?':
            self.state = 'noVersion'
        elif stateStr[0] == 'M':
            self.state = 'modify'
        else:
            self.state = 'nothing'
        if self.state != 'noVersion':    
            self.p1 = infoList[0]
            self.p2 = infoList[1]
            self.who = infoList[2]
            self.path = ' '.join(infoList[3::])
        else:
            self.path = ' '.join(infoList[0::])
            
        self.isPathContainSpace = False
        if ' ' in self.path:
            self.isPathContainSpace = True

        

class ToolUnit_meta文件补充提交提醒(BaseToolUnit):
    def __init__(self, projpath, unitypath,method,conf):
        super().__init__(projpath, unitypath,method,conf)
        
        self.ftppath = f'/DevOps/ClientToolArtifact/{self.getBranchDesc()}/metaScan/{tsp_readable}/report.txt'
        self.path = map(lambda x: os.path.join(self.projpath,com.convertsep(x)), conf['path'])
        self.isOk = False
        self.warningfile = []

    def run(self):
        if not self.canRun():
            return

        out,code = self.unity.open_close()
        filestate = []
        
        for p in self.path:
            filestate += self.getMetaState(p)
        if filestate.__len__() == 0:
            self.dingSuc()
            return
        
        filestate.sort(key=lambda x: x.state)
        with open('report.txt','w',encoding='gbk') as fs:
            for state in filestate:
                if state.state == 'modify':
                    suspect = state.who
                    fs.write(f'犯罪嫌疑人:{suspect.rjust(15)}     罪行:{"没更新meta".rjust(15)}     罪证:{self.getRelPath(state.path)}\n')
                elif state.state == 'noVersion':
                    sourceFile = self.getMetaSourceFile(state.path)
                    out,code = com.cmd(f'svn status --depth empty -v {sourceFile}')
                    out = out.rstrip()
                    suspect = SVNState(out).who
                    fs.write(f'犯罪嫌疑人:{suspect.rjust(15)}     罪行:{"没提交meta".rjust(15)}     罪证:{self.getRelPath(state.path)}\n')
                if state.isPathContainSpace:
                    fs.write(f'犯罪嫌疑人:{suspect.rjust(15)}     罪行:{"文件名带空格".rjust(15)}     罪证:{self.getRelPath(state.path)}\n')
        self.upload()
        self.dingFail()
    def getRelPath(self,absPath:str):
        return absPath.replace(self.projpath + os.path.sep,'')
    def upload(self):
        G_ftp.upload('report.txt',self.ftppath,showlog=True)
    def dingSuc(self):
        data = 一点零注意了机器人.build_text(f'{self.chinese}{self.method}执行完毕\n\n无异常')
        一点零注意了机器人.send(data)
    def dingFail(self):
        if self.show_report == 'true':
            with open('report.txt','w',encoding='gbk') as fs:
                allstr = fs.read()
            data = 一点零注意了机器人.build_text(f'{self.chinese}{self.method}执行完毕，有异常\n\n{allstr}')
        else:
            url = f'http://192.168.2.147/dnl{self.ftppath}'
            data = 一点零注意了机器人.build_link(self.chinese,f'{self.chinese}{self.method}执行完毕',url)
        一点零注意了机器人.send(data)
    def getMetaState(self,path):
        '''
        meta状态: noversion,update
        '''
        out,code = com.cmd(f'svn status -v {path}')
        out = out.rstrip()
        filestateStr = out.split(com.getcmddivide())

        metafilestate = filter(lambda x : x.endswith('.meta'),filestateStr)
        metafilestate = map(lambda x: SVNState(x),metafilestate)
        metafilestate = filter(lambda x: x.state != 'nothing',metafilestate)

        ok = []
        for x in metafilestate:
            sourceFile = self.getMetaSourceFile(x.path)
            if self.isInSvn_file(sourceFile):
                ok.append(x)

        return ok

    def isInSvn_file(self,filepath):
        out,code = com.cmd(f'svn status --depth empty -v "{filepath}"')
        out = out.rstrip()
        state = SVNState(out)
        if state.state == 'noVersion':
            return False
        return True
    def getMetaSourceFile(self,metafile):
        sourceFile = os.path.splitext(metafile)[0]
        return sourceFile

class ToolUnit_修改资源名称(BaseToolUnit):
    def afterRun(self):
        pass


def RunSpecial(projpath,unitypath,chinese,conf):
    cls = DictUtil.tryGetValue(globals(),f'ToolUnit_{chinese}')
    if cls == None:
        return False

    unit = cls(projpath,unitypath,chinese,conf)
    unit.run()
    return True

@errorcatch_func
def runall():
    projpath = sys.argv[1]
    unitypath = sys.argv[2]

    rootsvnpath = SVNManager.get_root(projpath)
    SVNManager.update_safe(rootsvnpath)

    jenkinswork_path = os.path.join(projpath,'jenkinswork.json')
    jsf = JsonFile(jenkinswork_path)
    
    for chinese,conf in jsf.getkv():
        if not RunSpecial(projpath,unitypath,chinese,conf):
            unit = BaseToolUnit(projpath,unitypath,chinese,conf)
            unit.run()


if __name__ == "__main__":
    runall()

    pass


