
# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,errorcatch_func,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com
from comlib.wraps import workspace
from comlib.conf.loader import Loader
from comlib.conf.ref import *
from comlib.threadm import ThreadManager

from comlib.dictm import JsonFile
from comlib.binm import BinManager
from comlib.webhelper import httphelper
from comlib.com import cmd,cmd_builder

import socket,json,requests,re
from requests.auth import HTTPBasicAuth

from comlib.comobjm import *
ln = 'jenkins'

defaulthost=('192.168.2.65',8080)
jenkinscmd = BinManager.jenkins10








@errorcatch(HIGH)
class JenkinsHelper(object):
    @staticmethod
    def getbuildjson(buildname,jobid=None,host=('192.168.2.65',8080))->str:
        buildname = buildname.replace('/','/job/')
        httpsk = socket.socket(family=socket.AF_INET,type=socket.SOCK_STREAM)
        httpsk.connect(host)
        req = ''
        if jobid == None:
            req = httphelper.req_get_build('/job/%s/api/json'%buildname,'%s:%s'%host,iskeepalive=True)
        else:
            req = httphelper.req_get_build('/job/%s/%s/api/json'%(buildname,jobid),'%s:%s'%host,iskeepalive=True)
        httpsk.send(req)

        hd = httphelper.getheader(httpsk).decode(encoding='utf-8')

        length = httphelper.getcontent_length(hd)
        js = ''
        if length != None:
            js = httpsk.recv(length,socket.MSG_WAITALL).decode(encoding='utf-8')
        else:
            # chunk
            print('!!!!!!!!!chunkchunkchunkchunkchunkchunkchunk!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!')
            print(buildname)
            print(hd)
            
            jsb = b''
            size = httphelper.getfirstpacketsize(httpsk)
            while size != 0:
                jsb += httpsk.recv(size,socket.MSG_WAITALL)
                size = httphelper.getpacketsize(httpsk)
            js = jsb.decode(encoding='utf-8')

        return js

    @staticmethod
    def buildparams_webjs(js_str)->str:
        d = json.loads(js_str)
        l = d['actions'][0]['parameters']
        ret = ''
        paraformat = ' -p {}="{}"'
        for p in l:
            ret += paraformat.format(p['name'],p['value'])
        return ret
    @staticmethod
    def buildparams(**params)->str:
        ret = ''
        paraformat = ' -p {}="{}"'

        for k,v in params.items():
            ret += paraformat.format(k,v)
        return ret

    @staticmethod
    def build(buildname,params=None,host=None):
        if params == None:
            params = ''
        if host == None:
            host = defaulthost
            
        cmdp = ' '.join([
            "-s http://%s:%s/ build"%host,
            "\"%s\"" % buildname,
            "%s" % params,
            "--username \"jenkins\" --password \"123456\""])
        jenkinscmd(cmdp,getstdout=False)




class BuildCallBack():
    def __init__(self,subp):
        self.subp = subp
    def waitBuildNum(self):
        buildNum = None
        def func():
            startout = self.subp.stdout.readline()
            # Started DNL1.5-主干 ? __流水线 ? Test_1 #40
            m = re.match(r'^Started.*#([\d]*)$',startout)
            nonlocal buildNum
            buildNum = m.group(1)
        thds = ThreadManager.go(func,count=1)
        ThreadManager.waitall(thds)
        return buildNum
    def waitBuildFinish(self):
        # Completed DNL1.5-主干 ? __流水线 ? Test_1 #40 : SUCCESS
        self.subp.wait()

class JenkinsApi():
    def __init__(self,username,token,domain,clifilename) -> None:
        self.username = username
        self.token = token
        self.domain = domain
        self.clifilename = clifilename
    def getRoot(self):
        return f'http://{self.domain}'
    def request_get(self,route,**parames):
        url = f'{self.getRoot()}/{route}'
        return requests.get(url,auth=HTTPBasicAuth(self.username,self.token),**parames)
    def request_post(self,route,**parames):
        data = requests.post(f'{self.getRoot()}/{route}',auth=HTTPBasicAuth(self.username,self.token),**parames)
        body = data.content
        return data
    def getCrumb(self):
        """
        获取跨域请求验证字符串
        """
        rawdata = self.request_get('crumbIssuer/api/json')
        data = rawdata.json()
        # {"_class":"hudson.security.csrf.DefaultCrumbIssuer","crumb":"2a20469dde319cda23fd12ec8d9bea03","crumbRequestField":"Jenkins-Crumb"}
        crumb = data['crumb']
        return crumb
    def jobname2route(self,jobname:str):
        """
        将jobname转换成路由
        """
        route = jobname.strip('/').replace('/','/job/')
        return route
    def getJobRoute(self,jobname):
        return f'job/{self.jobname2route(jobname)}'
    def getJobBuildDataRoute(self,jobname,buildnum):
        return f'{self.getJobRoute(jobname)}/{buildnum}/api/json'
    def getJobBuildData(self,jobname,buildnum):
        res = self.request_get(self.getJobBuildDataRoute(jobname,buildnum))
        return res.content.decode(encoding='utf-8')
        
    def copyJob(self,srcjobname,dstjobname):
        """
        拷贝job
        """
        pass
    
    def cloneJob(self,jobname,profile):
        """
        根据job描述文件进行克隆
        """
        pass

    def buildJob_parames(self,jobname,waitFinish=True,waitBuildId=True,**parames):
        '''
        根据参数构建
        '''
        pass
    def buildJob_profile(self,jobname,profile,waitFinish=True):
        """
        根据json描述文件构建
        """
        pass
    def deleteJobs(self,*jobnames):
        '''
        根据job名称删除
        '''
        pass
    def updateJob(self,jobname,profile):
        """
        根据job描述文件对已存在工程进行更新
        """
        pass
    def dumpJob(self,jobname,savepath=None):
        '''
        根据job名dump job的xml描述文件
        '''
        pass
    def getBuildState(self,jobname,buildnum):
        data = self.getJobBuildData(jobname,buildnum)
        jf = JsonFile(jsondata_default=data)
        state = jf.trygetvalue('result')

        return state

class Jenkins_2_249_2_Manager(JenkinsApi):
    def __init__(self) -> None:
        clifilename = 'Jenkins_2_249_2.jar'
        confpath = os.path.join(BinManager.get_conf_dir(clifilename),'config.json')
        jf = JsonFile(confpath)
        super().__init__(jf.trygetvalue('username'),jf.trygetvalue('token'),jf.trygetvalue('domain'),clifilename)
    def cmd(self,cmd,usePopen=False,**kv):
        """
        运行
        """
        com.logout(f'[Jenkins] {cmd}')
        wrapedcmd = f'-s {self.getRoot()} -auth {self.username}:{self.token} -webSocket {cmd}'
        return BinManager.JenkinsCli(self.clifilename,wrapedcmd,usePopen=usePopen,showlog=Loader.isdebug(),errException=StopException('jenkins命令执行失败',locals()),**kv)

    def copyJob(self,srcjobname,dstjobname):
        """
        拷贝job
        """
        self.cmd(f'copy-job "{srcjobname}" "{dstjobname}"')
    def cloneJob(self,jobname,profile):
        """
        根据job描述文件进行克隆
        """
        # data = com.readall(profile)
        
        with open(profile,'r') as fs:
            fs.seek(fs.tell())
            fs.flush()
            subp,code = self.cmd(f'create-job {jobname}',usePopen=True,stdin=fs)
            out = subp.communicate()
            Log.info(f'[cloneJob] {out}',ln)
            subp.wait()
        # subp,code = self.cmd(f'create-job {jobname}',getPopen=True)
        # out = subp.communicate()
        # subp.wait()
        # print(f'[cloneJob] {out}')
        

    def buildJob_parames(self,jobname,waitFinish=True,waitBuildId=True,**parames):
        '''
        根据参数构建
        '''
        opt = ''
        if waitFinish:
            opt += '-s '
        pstr = ''
        for k,v in parames.items():
            pstr += f'-p {k}="{v}" '
        
        subp,code = self.cmd(f'build {jobname} {opt} {pstr}',getPopen=True)
        cb = BuildCallBack(subp)
        if waitFinish and waitBuildId:
            buildNum = cb.waitBuildNum()
            cb.waitBuildFinish()
            return buildNum
        if not waitFinish:
            subp.wait()
        return cb
        
    def buildJob_profile(self,jobname,profile,waitFinish=True,waitBuildId=True):
        """
        根据json描述文件构建
        """
        jf = JsonFile(profile)
        return self.buildJob_parames(jobname,waitFinish=waitFinish,waitBuildId=waitBuildId,**jf.jsondata)

    def deleteJobs(self,*jobnames):
        self.cmd(f'delete-job {com.safepath(*jobnames)}')

    def updateJob(self,jobname,profile):
        """
        根据job描述文件进行更新
        """
        data = com.readall(profile)
        subp,code = self.cmd(f'update-job {jobname}',getPopen=True)
        out = subp.communicate(data)
        subp.wait()
    def dumpJob(self,jobname,savepath=None):
        '''
        根据job名dump job的xml描述文件
        '''
        data,code = self.cmd(f'get-job {jobname}')
        if savepath != None:
            com.savedata(data,savepath)
        return data


class Jenkins_2_289_2_Manager(JenkinsApi):
    def __init__(self) -> None:
        clifilename = 'Jenkins_2_289_2.jar'
        confpath = os.path.join(BinManager.get_conf_dir(clifilename),'config.json')
        jf = JsonFile(confpath)
        super().__init__(jf.trygetvalue('username'),jf.trygetvalue('token'),jf.trygetvalue('domain'),clifilename)
    def cmd(self,cmd,usePopen=False,**kv):
        """
        运行
        """
        com.logout(f'[Jenkins] {cmd}')
        wrapedcmd = f'-s {self.getRoot()} -auth {self.username}:{self.token} -webSocket {cmd}'
        return BinManager.JenkinsCli(self.clifilename,wrapedcmd,usePopen=usePopen,showlog=Loader.isdebug(),errException=StopException('jenkins命令执行失败',locals()),**kv)

    def copyJob(self,srcjobname,dstjobname):
        """
        拷贝job
        """
        self.cmd(f'copy-job "{srcjobname}" "{dstjobname}"')
    def cloneJob(self,jobname,profile):
        """
        根据job描述文件进行克隆
        """
        # data = com.readall(profile)
        
        with open(profile,'r') as fs:
            fs.seek(fs.tell())
            fs.flush()
            subp,code = self.cmd(f'create-job {jobname}',usePopen=True,stdin=fs)
            out = subp.communicate()
            Log.info(f'[cloneJob] {out}',ln)
            subp.wait()
        # subp,code = self.cmd(f'create-job {jobname}',getPopen=True)
        # out = subp.communicate()
        # subp.wait()
        # print(f'[cloneJob] {out}')
        

    def buildJob_parames(self,jobname,waitFinish=True,waitBuildId=True,**parames):
        '''
        根据参数构建
        '''
        opt = ''
        if waitFinish:
            opt += '-s '
        pstr = ''
        for k,v in parames.items():
            pstr += f'-p {k}="{v}" '
        
        subp,code = self.cmd(f'build {jobname} {opt} {pstr}',getPopen=True)
        cb = BuildCallBack(subp)
        if waitFinish and waitBuildId:
            buildNum = cb.waitBuildNum()
            cb.waitBuildFinish()
            return buildNum
        if not waitFinish:
            subp.wait()
        return cb
        
    def buildJob_profile(self,jobname,profile,waitFinish=True,waitBuildId=True):
        """
        根据json描述文件构建
        """
        jf = JsonFile(profile)
        return self.buildJob_parames(jobname,waitFinish=waitFinish,waitBuildId=waitBuildId,**jf.jsondata)

    def deleteJobs(self,*jobnames):
        self.cmd(f'delete-job {com.safepath(*jobnames)}')

    def updateJob(self,jobname,profile):
        """
        根据job描述文件进行更新
        """
        data = com.readall(profile)
        subp,code = self.cmd(f'update-job {jobname}',getPopen=True)
        out = subp.communicate(data)
        subp.wait()
    def dumpJob(self,jobname,savepath=None):
        '''
        根据job名dump job的xml描述文件
        '''
        data,code = self.cmd(f'get-job {jobname}')
        if savepath != None:
            com.savedata(data,savepath)
        return data


class Jenkins(object):
    def __init__(self,manager:JenkinsApi) -> None:
        self.manager = manager
        pass

    def copyJob(self,srcjobname,dstjobname):
        """
        拷贝job
        """
        return self.manager.copyJob(srcjobname,dstjobname)
    def cloneJob(self,jobname,profile):
        """
        根据job描述文件进行克隆
        """
        return self.manager.cloneJob(jobname,profile)
    def buildJob_parames(self,jobname,waitFinish=True,waitBuildId=True,**parames):
        '''
        根据参数构建
        '''
        return self.manager.buildJob_parames(jobname,waitFinish=waitFinish,waitBuildId=waitBuildId,**parames)
    def buildJob_profile(self,jobname,profile,waitFinish=True):
        """
        根据json描述文件构建
        """
        return self.manager.buildJob_profile(jobname,profile,waitFinish=waitFinish)
    def deleteJobs(self,*jobnames):
        return self.manager.deleteJobs(*jobnames)
    def updateJob(self,jobname,profile):
        return self.manager.updateJob(jobname,profile)
    def dumpJob(self,jobname,savepath=None):
        return self.manager.dumpJob(jobname,savepath)

    def getBuildState(self,jobname,buildnum):
        return self.manager.getBuildState(jobname,buildnum)

    def getDownloadHTMLContent(self,jobname,jobnum):
        route = 'job/'+self.manager.jobname2route(jobname)
        res = self.manager.request_get(f'{route}/{jobnum}/artifact/download.html')

        return res.content.decode(encoding='utf-8'),res.status_code



def test():
    ja = Jenkins_2_249_2_Manager()
    j = Jenkins(ja)
    j.copyJob('DNL1_5_Dev/Client/Build_Test_Code','DNL1_5_Dev/Client/Build_Test_Code_copy')
    j.cloneJob('DNL1_5_Dev/Client/Build_Test_copy2',r'C:\Users\tengmu\Downloads\config.xml')
    
    # data = j.buildJob_parames('DNL1_5_Dev/_Pipeline/Test_1',waitFinish=False,waitBuildId=False,asdf=123,bbb=2222)
    # print(data)
    # num = data.waitBuildNum()
    # print(num)
    # data.waitBuildFinish()
    # print('finish')
    # j.dumpJob('DNL1_5_Dev/_Pipeline/Test_1','save.xml')
    # j.updateJob('DNL1_5_Dev/_Pipeline/Test_1','save.xml')
    # j.deleteJobs('DNL1_5_Dev/Client/Build_Test_Code_copy')
    # j.deleteJobs('DNL1_5_Dev/Client/Build_Test_copy2')

if __name__ == "__main__":
    test()