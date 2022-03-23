
# -*- encoding: utf-8 -*-
import sys,os
from types import resolve_bases
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace
from comlib.conf.loader import Loader
from comlib.conf.ref import *


from comlib import JsonFile
from comlib.clientlib.help import NetHelper

from airtest.core.api import *

import time,re,threading

settingsfolder = os.path.abspath(os.path.join(thisdir,'..','settings'))
flowcasefolder = os.path.abspath(os.path.join(thisdir,'..','cases','_流程图'))
testfilefolder = os.path.join(settingsfolder,'testfiles')
settingjsonfile = os.path.join(settingsfolder,'configsetting.json')




# accountpath = os.path.join(settingsfolder,'account.json')
devicespath = os.path.join(settingsfolder,'devices.json')
# account = JsonFile(accountpath)
devices = JsonFile(devicespath)
testfile = None
net = NetHelper(f'192.168.2.197:5000')

remoteIP = com.loadfile_json_bykey(settingjsonfile,'remoteip')

def sleeptouch(v,sleeptime=None, times=1, **kwargs):
    touch(v,times,**kwargs)
    if sleeptime == None:
        sleeptime = 2
    time.sleep(sleeptime)

def exists_and_touch(img,sleeptime=None, times=1):
    pos = exists(img)
    if pos:
        sleeptouch(pos,sleeptime=sleeptime,times=times)
        return True
    return False
loglock = threading.Lock()
def logplus(desc,**parames):
    '''
    log显示在html报告中
    '''
    loglock.acquire(timeout=5)
    G.LOGGER.log("function", {"name": desc,'call_args': parames, "traceback": ''}, 1)
    loglock.release()



def assert_skipthis(flag):
    '''
    断言失败退出 code 10
    '''
    snapshot(msg=f'断言跳过当前测试 flag={flag}')
    if flag == False:
        sys.exit(10)
def assert_stopalltest(flag):
    '''
    断言失败退出 code 14
    '''
    snapshot(msg=f'断言停止所有测试 flag={flag}')
    if flag == False:
        sys.exit(14)
def adb(serialno,adbcmd):
    return com.cmd(f'adb -s {serialno} {adbcmd}',encoding='utf-8') if 'None' == remoteIP else com.cmd(f'adb -H {remoteIP} -s {serialno} {adbcmd}',encoding='utf-8') 
def adbs(serialno,adbscmd):
    return com.cmd(f'adb -s {serialno} shell "{adbscmd}"',encoding='utf-8') if 'None' == remoteIP else com.cmd(f'adb -H {remoteIP} -s {serialno} shell "{adbscmd}"',encoding='utf-8') 


class DeviceInfo():
    def __init__(self,serialno):
        global remoteIP
        remoteIP = com.loadfile_json_bykey(settingjsonfile,'remoteip')
        self.serialno = serialno
        self.prop = TestHelper.getPhoneProp(serialno)
        self.brand = TestHelper.getBrand(self.serialno,self.prop)
        self.manufacturer = TestHelper.getManufacturer(self.serialno,self.prop)
        self.sdkversion = TestHelper.getSDKVersion(self.serialno,self.prop)
        # 各厂商的rom版本，不同厂商rom版本放的位置是不一样的
        self.romversion = TestHelper.getROMVersion(self.serialno,self.manufacturer,self.prop)
        
class TestHelper(object):
    @staticmethod
    def getPID(serialno,packagename):
        out,code = adbs(serialno,'ps -A -o PID,NAME')
        procDatas = out.splitlines()[1::]
        for procData in procDatas:
            tmp = procData.strip().split(' ')
            pid = tmp[0].strip()
            name = ' '.join(tmp[1::]).strip()
            if name == packagename:
                return pid
        return None

        
    @staticmethod
    def openLogcat(serialno,remoteip,logfilepath,pid=None):
        # 2021-04-08 17:00:48.0000
        curtime = com.getdatetimenow() + '.0000' 

        if remoteip != 'None':
            cmd = f'adb -H {remoteip} -s {serialno} logcat -T "{curtime}"'
        else:
            cmd = f'adb -s {serialno} logcat -T "{curtime}"'
        if pid != None:
            cmd += f' --pid={pid}'
        cmd += f' > {logfilepath}'
        return com.cmd_async(cmd,workdir)
    @staticmethod
    def closeLogcat(proc):
        com.killproc(proc)

    @staticmethod
    def loadparams(args,*key):
        paramfilepath = args.paramfile
        global testfile
        if testfile == None:
            testfile = JsonFile(paramfilepath)
        value = testfile.trygetvalue(*key)
        return value

    @staticmethod
    def wakeup(serialno):
        pass
    @staticmethod
    def back(serialno):
        adbs(serialno,'input keyevent KEYCODE_BACK')
    @staticmethod
    def gotostate(curstatename,targetstatename):
        """
        从当前状态转换到其他状态
        """
        from flowloader import gettestmap
        testmap = gettestmap()
        curstate = testmap.getTestState(curstatename)
        testpath = testmap.getTestPath(curstate,targetstatename)
        for statename,testname in testpath.unpackpaths:
            scriptdir = os.path.join(flowcasefolder,f'{testname}.air')
            scriptpath = os.path.join(scriptdir,f'{testname}.py')
            using(scriptdir)
            code = com.readall(scriptpath)
            exec(code)
    @staticmethod
    def clearinput(count=20):
        keyevent('KEYCODE_MOVE_END')
        while count > 0:
            count -= 1
            keyevent('KEYCODE_DEL')
        time.sleep(3)
    @staticmethod
    def input(textstr,autoenter=True,usetab=True):
        text(textstr,enter=False)
        # mumu模拟器需要先tab才能enter输入
        # if brand == 'android':
        if autoenter:
            if usetab:
                keyevent('KEYCODE_TAB')
            keyevent('KEYCODE_ENTER')
    @staticmethod
    def input_raw(serialno,inputs:str,autoenter=True,usetab=True):
        if sys.version_info >= (3, 7):
            if not inputs.isascii():
                raise Exception('请输入ascii %s'%inputs)
        adbs(serialno,'input text %s'%inputs)
        time.sleep(1)
        if autoenter:
            if usetab:
                keyevent('KEYCODE_TAB')
            keyevent('KEYCODE_ENTER')
    @staticmethod
    def getPackagePath():
        # 用thisdir是因为air脚本跑的时候工作目录是air目录内
        root = os.path.abspath(os.path.join(thisdir,'..','packageroot'))
        if os.path.exists(root):
            names = os.listdir(root)
            if names.__len__() != 0:
                name = names[0]
            else:
                name = 'None.apk'    
        else:
            name = 'None.apk'
        return os.path.join(root,name)
    @staticmethod
    def getLinkedDevicesInfo(remoteip):
        data,isok = net.getdata('autotest/api',{'remoteip':remoteip},method='getLinkedDevicesInfo')
        return data
    @staticmethod
    def getEnableDevicesInfo(remoteip)->list:
        data,isok = net.getdata('autotest/api',{'remoteip':remoteip},method='getEnableDevicesInfo')
        return data
    @staticmethod
    def getDevice(name):
        '''
        从服务器注册设备
        return={
        "serialno":"6EB0217808004166"
        }
        '''
        data,isok = net.getdata('autotest/api',{'name':name},'getDevice')
        if not isok:
            raise Exception(f'注册设备失败{data}')

        return data
    @staticmethod
    def putDevice(name):
        """
        回收设备
        """
        data,isok = net.getdata('autotest/api',{'name':name},'putDevice')
        if not isok:
            raise DingException(f'回收设备失败{data}')
    @staticmethod
    def getAccount(channel,serialno,isNewAccount=True):
        '''
        从服务器拿账号
        return account,password,servertype,servername
        '''
        data,isok = net.getdata('autotest/api',{'channel':channel,'serialno':serialno,'isNewAccount':isNewAccount},'getAccount')
        account = data['account']
        password = data['password']
        servertype = data['servertype']
        servername = data['servername']
        if account == None:
            raise StopException(f'{channel}账号已耗尽',{})
        return account,password,servertype,servername
    @staticmethod
    def putAccount(channel,serialno):
        """
        让服务器回收账号
        """
        data,isok = net.getdata('autotest/api',{'channel':channel,'serialno':serialno},'putAccount')

    # @staticmethod
    # def deviceName2serialno(deviceName):
    #     '''
    #     重名还没解决，暂时这样搞
    #     '''
    #     for serialno,item in devices.getkv():
    #         if item['name'] == deviceName:
    #             return serialno

    # @staticmethod
    # def serialno2devicesName(serialno):
    #     deviceName = devices.trygetvalue(serialno,'name')
    #     return deviceName
            
    @staticmethod
    def getseralnos():
        output,code = com.cmd('adb devices')
        devices = []
        
        for m in output.strip().splitlines()[1::]:
            devices.append(m.split('\t')[0])
        return devices

    # @staticmethod
    # def getfilteredtestphones(testphone):
    #     linked = TestHelper.getseralnos()
    #     linked_phonename = list(map(TestHelper.serialno2devicesName,linked))
    #     r = []
    #     for d in testphone:
    #         if d in linked_phonename:
    #             r.append(d)
    #         else:
    #             print('手机未连接 %s'%d)
    #     return r
    
    @staticmethod
    def open(serialno,packname):
        # monkey -p com.hegu.dnl.huawei -c android.intent.category.LAUNCHER 1
        adbs(serialno,'monkey -p %s -c android.intent.category.LAUNCHER 1'%packname)
    @staticmethod
    def close(serialno,packname):
        adbs(serialno,'am force-stop %s'%packname)
    @staticmethod
    def getDeviceInfo(serialno=None)-> DeviceInfo:
        if serialno == None:
            serialno = G.DEVICE.serialno
        return DeviceInfo(serialno)
    @staticmethod
    def getPhoneProp(serialno)-> str: 
        prop,code = adbs(serialno,'getprop')
        return prop
    @staticmethod
    def getBrand(serialno,prop=None)-> str: 
        if prop == None:
            prop,code = TestHelper.getPhoneProp(serialno)
        m = re.search('\[ro\.product\.brand\]: \[(.*?)\]',prop)
        if m==None:
            m = re.search('\[ro\.product\.system\.brand\]: \[(.*?)\]',prop)
            # m = re.search('\[ro\.product\.model\]: \[(.*?)\]',prop)
        try:
            return m.group(1).lower()
        except:
            return None
    @staticmethod
    def getManufacturer(serialno,prop=None) -> str:
        """
        获取生产商
        """
        if prop == None:
            prop,code = TestHelper.getPhoneProp(serialno)
        m = re.search('\[ro\.product\.manufacturer\]: \[(.*?)\]',prop)
        if m==None:
            m = re.search('\[ro\.product\.vendor\.manufacturer\]: \[(.*?)\]',prop)
            # m = re.search('\[ro\.product\.model\]: \[(.*?)\]',prop)
        try:
            return m.group(1).lower()
        except:
            return None
    @staticmethod
    def getSDKVersion(serialno,prop=None)-> int:
        if prop == None:
            prop,code = TestHelper.getPhoneProp(serialno)
        m = re.search('\[ro\.build\.version\.sdk\]: \[(.*?)\]',prop)
        try:
            return int(m.group(1))
        except:
            return None
    @staticmethod
    def getROMVersion(serialno,manufacturer,prop=None)-> int:
        if prop == None:
            prop,code = TestHelper.getPhoneProp(serialno)
        if manufacturer == 'xiaomi':
            m = re.search('\[ro\.miui\.ui\.version\.name\]: \[(.*?)\]',prop)
            raw = m.group(1) # V12
            version = raw[1::]
        elif manufacturer == 'android':
            version = '12'
        elif manufacturer == 'huawei':
            m = re.search('\[ro\.build\.version\.emui\]: \[EmotionUI_(.*?)\]',prop)
            version = m.group(1) # EmotionUI_x.x.x
        elif manufacturer == 'oneplus':
            version = '12'
        elif manufacturer == 'oppo':
            m = re.search('\[ro\.build\.version\.opporom\]: \[(.*?)\]',prop)
            raw = m.group(1) # V12
            version = raw[1::]
            version = '12'
        else:
            raise Exception(f'{manufacturer} rom版本未编写')
        
        return version
    @staticmethod
    def getDisplayInfo(serialno):
        # cmd : adb shell wm size
        # Physical size: 1440x2560
        # Override size: 1080x1920
        pass
