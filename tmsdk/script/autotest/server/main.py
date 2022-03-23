# -*- encoding: utf-8 -*-

from comlib.serverlib.helper import getFailRetContent, getSucRetContent, tryGetData, tryGetFormData
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())

from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace
from comlib.conf.loader import Loader
from comlib.conf.ref import *

from comlib import DictUtil,JsonFile

from flask import Flask,redirect,url_for,request,render_template

import threading,requests
from requests.auth import HTTPBasicAuth

app = Flask(__name__)
class Api():
    self = None
    @classmethod
    def invoke(cls,methodname,data):
        """
        主运行函数
        """
        if cls.self == None:
            cls.self = cls()
        
        methods = com.getClassMethods(cls)
        return methods[methodname](cls.self,data)


class HTTPAuth(HTTPBasicAuth):
    """Attaches HTTP Basic Authentication to the given Request object."""

    def __call__(self, r):
        r.headers['Authorization'] = f'{self.username} {self.password}'
        return r



class AutoTestApi(Api):
    def __init__(self):
        """
        初始化
        """
        super().__init__()
        self.accountf = JsonFile(os.path.join(thisdir,'data','account.json'))
        self.accountbuffer = {}

        self.accountlock = threading.Lock()

        self.devicef = JsonFile(os.path.join(thisdir,'data','devices.json'))
        self.usingdevice = {}
        


        

    def _getAccount(self,channel):
        self.accountlock.acquire()
        try:
            accounts = self.accountf.trygetvalue(channel)
            if accounts == None or accounts.__len__() == 0:
                return None,None
            tmp = accounts.pop()
        finally:
            self.accountlock.release()
        account = tmp['account']
        password = tmp['password']
        return account,password
    def getAccount(self,data):
        """
        获取账号
        """
        if not DictUtil.checkRequire(data,'channel','serialno'):
            return getFailRetContent(123,'需要提供渠道号')
        channel = data['channel']
        serialno = data['serialno']
        if serialno in self.accountbuffer:
            data = self.accountbuffer[serialno]
        else:
            isNewAccount = DictUtil.tryGetValue(data,'isNewAccount')
            if isNewAccount and channel == 'dnfdebug':
                account,password = (str(com.gettsp()),'1')
            else:
                account,password = self._getAccount(channel)
                isNewAccount = False
            servertype = '内网活动服务器'
            servername = '安卓预发布服务器2'
            servertype = '内网服务器'
            servername = '测试服务器'
            data = {'account':account,'password':password,'servertype':servertype,'servername':servername,'isNewAccount':isNewAccount}
            self.accountbuffer[serialno] = data
        return getSucRetContent(data)
    def _putAccount(self,channel,serialno):
        self.accountlock.acquire()
        try:
            if serialno not in self.accountbuffer:
                return
            data = self.accountbuffer.pop(serialno)
            print(f'[_putAccount]{data}')
            isNewAccount = DictUtil.tryGetValue(data,'isNewAccount')
            if not isNewAccount:
                accounts = self.accountf.trygetvalue(channel)
                accounts.append(data)
        finally:
            self.accountlock.release()
    def putAccount(self,data):
        """
        使用完毕放回账号，
        """
        if not DictUtil.checkRequire(data,'channel','serialno'):
            return getFailRetContent(123,'需要提供渠道号、机器序列号')
        channel = data['channel']
        serialno = data['serialno']
        self._putAccount(channel,serialno)
        return getSucRetContent(None)

    def getDevice(self,data):
        '''
        获取测试机
        '''
        if not DictUtil.checkRequire(data,'name'):
            return getFailRetContent(123,'机器序列号')
        name = data['name']

        device,err = self._getdevice(name)
        if err != None:
            return err
        usestf = device['usestf']
        serialno = device['serialno']

        err = self._ensure_devicenotusing(name,usestf)
        if err != None:
            return err        

        if usestf:
            err = STFManager.registdevice(serialno)
            if err != None:
                return err

        self.usingdevice[name] = device
        return getSucRetContent(device)

    def putDevice(self,data):
        '''
        放回测试机
        '''
        if not DictUtil.checkRequire(data,'name'):
            return getFailRetContent(123,'机器序列号')
        name = data['name']
        
        device,err = self._getdevice(name)
        if err != None:
            return err
        usestf = device['usestf']
        serialno = device['serialno']

        err = self._ensure_deviceusing(name,usestf)
        if err != None:
            return err        
        if usestf:
            err = STFManager.unregistdevice(serialno)
            if err != None:
                return err

        self.usingdevice.pop(name)
        # TODO 超时就强制回收
        return getSucRetContent({})
        
        
    def _getLinkedDevicesInfo(self,data):
        remoteip = 'None'
        if DictUtil.checkRequire(data,'remoteip'):
            remoteip = data['remoteip']
        output,code = com.cmd(f'adb devices') if remoteip == 'None'  else com.cmd(f'adb -H {remoteip} devices')
        devices = []
        
        for m in output.strip().splitlines()[1::]:
            devices.append(m.split('\t')[0])
        # return devices

        # 过滤
        datas = []
        for k,v in self.devicef.getkv():
            if v['serialno'] in devices:
                datas.append(v)
        return datas
    def getLinkedDevicesInfo(self,data):
        datas = self._getLinkedDevicesInfo(data)
        return getSucRetContent(datas)
    def getEnableDevicesInfo(self,data):
        linked = self._getLinkedDevicesInfo(data)

        for k,v in self.usingdevice.items():
            linked.remove(v)

        return getSucRetContent(linked)
            
    def getReportSummary(self,data):
        if not DictUtil.checkRequire(data,'startid','count'):
            return getFailRetContent(123,'开始的报告id','返回的报告数量')
        startid = data['startid']
        count = data['count']

        ret = [
            {
                'time':'20201210',
                'packageurl':'http://www.baidu.com',
                'devices':'测试机',
                'target':'测试',
                'suc_count':1,
                'fail_count':1,
                'unstable_count':1,
                'reporturl':'http://www.baidu.com'
            }
        ]*int(count)
        
        return getSucRetContent(ret)
    def getReportInfo(self,data):
        if not DictUtil.checkRequire(data,'id'):
            return getFailRetContent(123,'开始的报告id','返回的报告数量')
        id = data['id']
        ret = {
            'summary':{
                'time':'20201210',
                'packageurl':'http://www.baidu.com',
                'devices':'测试机',
                'target':'测试',
                'suc_count':1,
                'fail_count':1,
                'unstable_count':1,
                'reporturl':'http://www.baidu.com'
            },
            'suc':[{
                'device':'成功机器',
                'target':'测试目标',
                'proc_reporturl':'http://www.baidu.com',
                'airtest_reporturl':'http://www.baidu.com'
            }],
            'fail':[{
                'device':'成功机器',
                'target':'测试目标',
                'proc_reporturl':'http://www.baidu.com',
                'airtest_reporturl':'http://www.baidu.com'
            }],
            'unstable':[{
                'device':'成功机器',
                'target':'测试目标',
                'proc_reporturl':'http://www.baidu.com',
                'airtest_reporturl':'http://www.baidu.com'
            }]
        }
        return getSucRetContent(ret)
    def _name2serialno(self,name):
        device,err = self._getdevice(name)
        if err != None:
            return None,err
        return device['serialno'],None

    def _ensure_devicenotusing(self,name,usestf):
        if name in self.usingdevice:
            return getFailRetContent(123,f'{name} 在使用中')
        serialno,err = self._name2serialno(name)
        if err != None:
            return err
        if usestf:
            isusing,err = STFManager.isusing(serialno)
            if err != None:
                return err
            if isusing:
                return getFailRetContent(123,f'{name} 被STF手动占用中')
        return None
    
    def _ensure_deviceusing(self,name,usestf):
        if name not in self.usingdevice:
            return getFailRetContent(123,f'{name} 未在使用中')
        serialno,err = self._name2serialno(name)
        if err != None:
            return err

        if usestf:
            isusing,err = STFManager.isusing(serialno)
            if err != None:
                return err
            if not isusing:
                return getFailRetContent(123,f'{name} STF空闲中')
        return None
    def _getdevice(self,name):
        device = self.devicef.trygetvalue(name)
        if device == None:
            return None,getFailRetContent(123,f'{name} 不存在')
        return device,None

STFURL = 'http://192.168.2.132:7100'



class STFManager():
    auth=HTTPAuth('Bearer','7c82f220245449b89db202b520e5a12de72cb3f8e3cb4dc9bd752a56dfb0f203')
    STFEnable = False
    @staticmethod
    def isusing(serial):
        if STFManager.STFEnable == False:
            return False,None

        deviceinfo_raw = requests.get(f'{STFURL}/api/v1/devices/{serial}',auth=STFManager.auth)
        deviceinfo = deviceinfo_raw.json()
        err = STFManager._checkdata(deviceinfo,123,f'{serial} 设备数据获取失败')
        if err != None:
            return False, err
        return deviceinfo['device']['using'],None
    @staticmethod
    def registdevice(serial):
        if STFManager.STFEnable == False:
            return None

        data = requests.post(f'{STFURL}/api/v1/user/devices',json={'serial':f'{serial}'},auth=STFManager.auth)
        return STFManager._checkdata(data.json(),123,f'{serial} STF占用失败')

    @staticmethod
    def unregistdevice(serial):
        if STFManager.STFEnable == False:
            return None
            
        data = requests.delete(f'{STFURL}/api/v1/user/devices/{serial}',auth=STFManager.auth)
        return STFManager._checkdata(data.json(),123,f'{serial} STF解除占用失败')
    @staticmethod
    def _checkdata(data:dict,errcode,msg):
        if data['success'] == False:
            return getFailRetContent(errcode,f"{msg}\nSTF MSG: {data['description']}")
        return None

try:
    req = requests.get(STFURL,timeout=1)
    STFManager.STFEnable = True
except:
    STFManager.STFEnable = False


class BaseView():
    def __init__(self,viewname):
        self.viewname = viewname
        print(f'{viewname}初始化成功')
        self.register()
    def apifunc(self):
        """
        api函数
        """
        if request.method == 'GET':
            return self.apiget()
        elif request.method == 'POST':
            return self.apipost()
        
        return None
    def indexfunc(self):
        '''
        主页函数
        '''
        if request.method == 'GET':
            return self.indexget()
        
        return None
    def apiget(self):
        return None
    def apipost(self):
        return None
    def indexget(self):
        return None
    def register(self):
        app.add_url_rule(f'/{self.viewname}/api/',view_func=self.apifunc,methods=['GET','POST'])
        app.add_url_rule(f'/{self.viewname}/index/',view_func=self.indexfunc,methods=['GET'])

views = []
def registview(view):
    views.append(view)
    return view
@registview
class AutoTest(BaseView):
    def __init__(self):
        """
        自动化测试路由
        """
        super().__init__('autotest')
    def apiget(self):
        method,data = tryGetData(request)
        print(data)
        content = AutoTestApi.invoke(method,data)
        return content
    def apipost(self):
        return self.apiget()
    def indexget(self):
        return render_template(f'index_{self.viewname}.html')
    def infofunc(self):
        # data = tryGetFormData(request)
        return render_template(f'autotest_info.html')
    def register(self):
        super().register()
        app.add_url_rule(f'/{self.viewname}/info/',view_func=self.infofunc,methods=['GET'])


def init():
    for view in views:
        view()
    app.run(com.ip, 5000,debug=True)
if __name__ == "__main__":
    init()
    pass