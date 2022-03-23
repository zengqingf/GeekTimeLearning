# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace
from comlib.conf.loader import Loader
from comlib.conf.ref import *

from comlib import HTTPManager,TMApkManager,Path

from models.ensure import Ensure
from models.testhelp import TestHelper, adbs

import argparse,multiprocessing,time

import airtest.cli.__main__ as airtestMain

from flowloader import TestCase, TestState, gettestmap,TestEdge,TestPath

from typing import List

from comlib.comobj import *


rawreportlogdir = os.path.join(workdir,'log_raw')
reportlogdir = os.path.join(workdir,'log_report')
subprocesslogdir = os.path.join(workdir,'log_proc')
devicelogdir = os.path.join(workdir,'log_device')
settingjsonfile = os.path.join(thisdir,'settings','configsetting.json')
Path.ensure_dirnewest(rawreportlogdir)
Path.ensure_dirnewest(reportlogdir)
Path.ensure_dirnewest(subprocesslogdir)
Path.ensure_dirnewest(devicelogdir)

caseroot = os.path.join(thisdir,'cases')
packageroot = os.path.join(workdir,'packageroot')
Path.ensure_dirnewest(packageroot)

jenkinsState = 'suc'


def getCasePath(statename,casename):
    airpath = os.path.join(caseroot,statename,casename+'.air')
    if os.path.exists(airpath):
        return airpath
    else:
        raise StopException(f'{airpath}未找到',locals())
class TestTask(object):
    def __init__(self,devicename,serialno,stateName,curTestName,paramfilepath,remoteip='None'):
        self.logcatProc = None

        self.exitcode = None
        self.unstableList = []

        self.devicename = devicename
        self.stateName = stateName
        self.testName = curTestName
        self.paramfilepath = paramfilepath
        self.serialno = serialno
        if remoteip == 'None':
            remoteip = '127.0.0.1'
        self.remoteip = remoteip
        deviceinfo = TestHelper.getDeviceInfo(self.serialno)
        
        self.extmsg = ''
        devurl = f'Android://{self.remoteip}:5037/{self.serialno}?cap_method=JAVACAP&&ori_method=ADBORI' # 127.0.0.1:5037是adb host
        if deviceinfo.sdkversion >= 0:
            devurl += '&&touch_method=ADBTOUCH'
        testcase = getCasePath(self.stateName,self.testName)
        logfilename = f'{com.getlocaltime("_")}-{self.stateName}-{self.testName}'

        self.rawreportlogdir = os.path.join(rawreportlogdir,devicename)
        Path.ensure_direxsits(self.rawreportlogdir)
        self.reportlogpath = os.path.join(self.rawreportlogdir,logfilename)
        # airtest run 'D:\_WorkSpace\_sdk2\DevOps_Scripts\pyscripts\autotest\cases\角色选择\从零转职.air' '--device=Android://127.0.0.1:5037/590250e6?cap_method=JAVACAP&&ori_method=ADBORI&&touch_method=ADBTOUCH'
        self.cmd = f"airtest run '{testcase}' '--device={devurl}' '--log={self.reportlogpath}' '--paramfile={self.paramfilepath}'"
        self.pscmdparames = f"run,'{testcase}','--device={devurl}','--log={self.reportlogpath}','--paramfile={self.paramfilepath}'"

        self.subprocesslogdir = os.path.join(subprocesslogdir,devicename)
        Path.ensure_direxsits(self.subprocesslogdir)
        self.subprocesslogpath = os.path.join(self.subprocesslogdir,f'{logfilename}.txt')
        # windows是stdout和stderr分开的，mac是一起的
        self.subprocesslogpath_stderr = os.path.join(self.subprocesslogdir,f'{logfilename}_stderr.txt')


        self.flag = None
    def setFlag(self,flag):
        if self.flag == None:
            self.flag = flag
            if flag == 'unstable':
                self.extmsg += f'状态:{self.stateName} 用例:{self.testName} 失败 状态:{self.flag}\n'
            elif flag == 'fail':
                self.extmsg += f'状态:{self.stateName} 用例:{self.testName} 彻底失败 状态:{self.flag}\n'
    def run(self):
        # Ensure.app_close('com.netease.nie.yosemite')
        # adbs(self.serialno,f'am force-stop com.netease.nie.yosemite')
        # 用命令行调用是为了存日志  $PSDefaultParameterValues['*:Encoding'] = 'utf8'; | Out-File -Encoding UTF8NoBOM  | out-file -encoding ASCII
        if sys.platform == 'win32':
            subp,code = com.cmd(f'''PowerShell -Command "& {{$proc=Start-Process -FilePath airtest -ArgumentList {self.pscmdparames} -NoNewWindow -Wait -PassThru -RedirectStandardOutput {self.subprocesslogpath} -RedirectStandardError {self.subprocesslogpath_stderr};exit $proc.ExitCode}}"''',getPopen=True)
        else:
            subp,code = com.cmd(self.cmd,logfile=self.subprocesslogpath,getPopen=True)
            
        self.subp = subp
    def isTestEnd(self):
        code = self.subp.poll()
        if code == None:
            return False,-1
        self.exitcode = code
        return True,code
    def isSuccess(self):
        if self.flag == 'suc':
            return True
        return False

class BaseTest(object):
    def __init__(self):
        self.testtype = 'None'

        # 传入测试机型（重复机型暂时不搞） 测试项目
        self.parser = argparse.ArgumentParser()
        self.parser.add_argument('--package_downloadpath',required=True,help='测试包下载地址')
        self.parser.add_argument('--devices',required=True,type=com.str2list,help='测试机型')
        self.parser.add_argument('--paramfile',required=True,help='测试用例传入的参数文件')
        self.parser.add_argument('--remoteip',default='None',help='远程链接ip')

        subp = self.parser.add_subparsers(help='优先度和指定测试')
        subpPriority = subp.add_parser('p')
        subpPriority.add_argument('--priority',required=True,help='测试项目开始状态')
        def priorityfunc(namespace):
            self.testtype='priority'
        subpPriority.set_defaults(func=priorityfunc)
        subpTarget = subp.add_parser('t')
        subpTarget.add_argument('--casestate',required=True,help='测试项目开始状态')
        subpTarget.add_argument('--casename',required=True,type=com.str2Noneable,help='测试项目名称')
        def targetfunc(namespace):
            self.testtype='target'
        subpTarget.set_defaults(func=targetfunc)
        subpTarget = subp.add_parser('single')
        def singlefunc(namespace):
            self.testtype='single'
        subpTarget.set_defaults(func=singlefunc)
    def preinit(self):
        args = self.parser.parse_args()
        args.func(args)
        self.package_downloadpath = args.package_downloadpath
        # self.devices:list = TestHelper.getfilteredtestphones(args.devices)
        self.devices:list = args.devices
        self.remoteip = args.remoteip
        com.dumpfile_json_bykey(settingjsonfile,'remoteip',self.remoteip)
        self.runDevicesData = []
        self.devices_linked = TestHelper.getLinkedDevicesInfo(self.remoteip)

        enabledevices = TestHelper.getEnableDevicesInfo(self.remoteip)
        if enabledevices.__len__() == 0:
            raise Exception('可用设备不足')
        if 'any' in self.devices:
            self.devices = [enabledevices[0]['chinese']]
        elif 'all' in self.devices:
            self.devices = [x['chinese'] for x in enabledevices]
            
        self.paramfile = os.path.join(thisdir,'settings','testfiles',args.paramfile)


        if self.testtype == 'priority':
            self.priority = args.priority
        elif self.testtype == 'target':
            self.casestate = args.casestate
            self.casename = args.casename
        elif self.testtype == 'single':
            pass
        else:
            raise Exception(f'非法类型{self.testtype}')
        
        # 占用测试机
        #self.devices = ['OPPOA92S']
        for devicename in self.devices:
            com.logout(f'[占用测试机] devicename={devicename}')
            devicedata = TestHelper.getDevice(devicename)
            serialno = devicedata['serialno']
            com.logout(f'[占用测试机] device={devicedata}')
            # 检查手机是否连接
            if not self.isLinked(serialno):
                print(f'手机未连接 {devicename}')
                TestHelper.putDevice(devicename)
            else:
                self.runDevicesData.append(devicedata)
        self.package_localpath = HTTPManager.download_http(self.package_downloadpath,os.path.join(packageroot,'test.apk'))
        
    def run(self):
        pass
    def report(self):
        
        for dirpath, dirnames, filenames in os.walk(rawreportlogdir):
            if 'log.txt' not in filenames:
                continue
            tmp = os.path.basename(dirpath).split('-')
            stateName = tmp[-2]
            curTest = tmp[-1]
            outdir = dirpath.replace(rawreportlogdir,reportlogdir,1)
            Path.ensure_direxsits(outdir)
            argv = ['report',getCasePath(stateName,curTest),f'--log_root={dirpath}',f'--export={outdir}',f'--lang=zh']
            airtestMain.main(argv)

    def _runTasks(self,allpath:TestPath,paramfile) -> List[TestTask]:
        tasks = []
        for devicedata in self.runDevicesData:
            devicename = devicedata['chinese']
            # devicedata = TestHelper.getDevice(devicename)
            serialno = devicedata['serialno']
            # 流程图的statename是_流程图
            statename,testname = allpath.registe(devicename)
            
            task = TestTask(devicename,serialno,statename,testname,paramfile,self.remoteip)
            
            # TestHelper.wakeup(task.serialno)
            tasks.append(task)
            # 开启logcat
            proc = TestHelper.openLogcat(serialno,self.remoteip,os.path.join(devicelogdir,devicename)+'.log')
            task.logcatProc = proc
            task.run()
        finishcount = 0
        while finishcount != tasks.__len__():
            time.sleep(1)
            for index,task in enumerate(tasks):
                task:TestTask
                isEnd,code = task.isTestEnd()
                if not isEnd or task.flag == 'fail' or allpath.isFinish(task.devicename):
                    continue
                if code != 0:
                    # TODO 添加不稳定测试
                    flag = checkexitcode(code)
                    if flag == 'skip':
                        task.setFlag('unstable')
                        task.unstableList.append(f'{task.testName}')
                        continue
                    elif flag in ('stop','unkonw'):
                        # 关闭logcat
                        TestHelper.closeLogcat(task.logcatProc)
                        # 测试失败中断
                        finishcount += 1
                        task.setFlag('fail')
                        print('[bugly] 上传数据')
                        # bugly崩溃上传
                        info = TMApkManager.getapkinfo(TestHelper.getPackagePath())
                        time.sleep(3)
                        TestHelper.close(task.serialno,info.packagename)
                        time.sleep(3)
                        TestHelper.open(task.serialno,info.packagename)
                        time.sleep(3)
                        # TestHelper.close(task.serialno,info.packagename)
                        continue
                else:
                    statename,nextTest = allpath.getNext(task.devicename)
                    task.setFlag('suc')
                    if nextTest == None:
                        # 关闭logcat
                        TestHelper.closeLogcat(task.logcatProc)
                        # 测试完成
                        finishcount += 1
                        pass
                    else:
                        time.sleep(1)
                        # 开始下一步测试
                        nextTask = TestTask(task.devicename,task.serialno,statename,nextTest,paramfile,self.remoteip)
                        nextTask.logcatProc = task.logcatProc

                        nextTask.run()
                        tasks[index] = nextTask
        # 清理
        for index,task in enumerate(tasks):
            Ensure.app_close(self.apkinfo,task.serialno)

            # 回收账号
            TestHelper.putAccount(self.apkinfo.usechannel,task.serialno)
            # 回收设备
            TestHelper.putDevice(task.devicename)
        return tasks
    def isLinked(self,serialno):
        for device in self.devices_linked:
            if device['serialno'] == serialno:
                return True
        return False
class ALD10AndroidTest(BaseTest):
    def __init__(self):
        super().__init__()
        self.tasks = []
    def preinit(self):
        super().preinit()
        
        self.apkinfo = TMApkManager.getapkinfo(self.package_localpath)
    @workspace
    def run(self):
        # 手机中文名转序列号
        # 生成测试步骤（图）
        # 等待所有测试完成
        # 生成报告
        self.preinit()
        self.runTasks()
        #self.report()
    def runTasks(self):
        testmap = gettestmap()
        # 跑优先级测试，这里会根据优先级展开整体测试项目
        if self.testtype == 'priority':
            print('runTasks priority')
            allpath = testmap.getAllTestPath(self.priority)
            self.tasks = self._runTasks(allpath,self.paramfile)
        elif self.testtype == 'target':
            allpath = testmap.getTestPath(testmap.getRoot(),self.casestate,self.casename)
            print('runTasks target')
            self.tasks = self._runTasks(allpath,self.paramfile)
        elif self.testtype == 'single':
            # 一次游戏过程只跑一个测试，第一次之后排除安装过程
            isfirst = True
            for id,state in testmap.states.items():
                state:TestState
                name = state.tag
                cases = state.getTestList('single')
                for case in cases:
                    case:TestCase
                    if isfirst:
                        isfirst = False
                        startstate = testmap.getRoot()
                    else:
                        startstate = testmap.getTestState('已安装游戏')
                    allpath = testmap.getTestPath(startstate,name,case.name)
                    self.tasks += self._runTasks(allpath,self.paramfile)
        else:
            raise Exception()
        
        for task in self.tasks:
            pass
        
        


    def report(self):
        super().report()
        # 上传proc和report，清理日志
        # 钉钉发送成功消息，失败则添加失败连接
        
        # 自动化测试报告
        # 测试包：xxx
        # 测试时间：xxxx
        # 测试目的：优先级xx测试/指定测试/single测试
        # 测试结果：成功xxx 失败xxx 不稳定xxx
        # 测试报告：url
        robot = Loader.获取脚本调试机器人()
        robot2 = Loader.获取客户端维护机器人()
        
        title = '自动化测试报告'
        content = f'{robot.markdown_textlevel(title,2)}\n\n'
        content += f'测试包：{robot.markdown_textlink(os.path.basename(self.package_downloadpath),self.package_downloadpath)}\n\n'
        content += f'测试时间：{com.timemark_datetime}\n\n'
        if self.testtype == 'priority':
            content += f'测试目的：优先级{self.priority}测试\n\n'
        elif self.testtype == 'target':
            content += f'测试目的：{self.casestate}-{self.casename}测试\n\n'
        elif self.testtype == 'single':
            content += f'测试目的：single测试\n\n'
        
        suc = com.counter(self.tasks,lambda t: t.flag == 'suc')
        fail = com.counter(self.tasks,lambda t: t.flag == 'fail')
        unstable = com.counter(self.tasks,lambda t: t.flag == 'unstable')
        
        content += f'测试结果：成功{suc} 失败{fail} 不稳定{unstable}\n\n'


        # 写入具体xx机型 失败原因
        reason = ''
        for task in self.tasks:
            if not task.isSuccess():
                tmp = f'{task.devicename}\n'
                tmp += f'{task.extmsg}\n\n'
                reason += tmp
                
        com.savedata(reason,'有问题的测试.txt')
        TMApkManager.ziptarget([subprocesslogdir,reportlogdir,devicelogdir,'有问题的测试.txt'],'upload.zip')
        ftppath = f'{com.get_ftp_logsavepath("autotest")}/{com.timemark}.zip'
        G_ftp.upload('upload.zip',ftppath)
        httppath = G_ftp.ftppath2httppath(ftppath)
        # content += f'''测试报告：{robot.markdown_textlink('报告',httppath)}'''

        data = robot.build_actioncard_mult(title,content,测试报告=httppath)

        lastresult = 'suc'
        lastresultfilepath = os.path.join(workdir,'lastresult.txt')
        if os.path.exists(lastresultfilepath):
            lastresult = com.readall(lastresultfilepath)
        
        global jenkinsState

        if fail != 0:
            jenkinsState = 'fail'
            com.savedata('fail',lastresultfilepath)
            robot.send(data)
            # robot2.send(data)
            # if lastresult == 'suc':
            #     # robot2.send(data)
            #     pass
            # else:
            #     pass

        else:
            jenkinsState = 'suc'
            com.savedata('suc',lastresultfilepath)
            if lastresult == 'fail':
                robot.send(data)
                # robot2.send(data)
            else:
                robot.send(data)

        
        

def checkexitcode(code):
    code = int(code)
    if code == 10:
        return 'skip'
    elif code == 14:
        return 'stop'
    
    return 'unkonw'




if __name__ == "__main__":
    test = ALD10AndroidTest()
    test.run()
    if jenkinsState == 'fail':
        exit(1)
    elif jenkinsState == 'suc':
        exit(0)
    pass
#--package_downloadpath=http://192.168.2.60:58001/package/A8/android/trunk_debug/time=2021-0820-110617_codev=1.2.9.18776_resv=18776_mode=Development_branch=trunk_dis=True_lightmass=SKIP_engine=SourceInstalled_startScene=denglu_job=leijin_logLevel=LOG_ERROR_skillNoCD=False_internalTest=True.apk --devices=OPPOA92S --paramfile=test --remoteip=192.168.4.19 t --casestate=战斗 --casename=制作PSO