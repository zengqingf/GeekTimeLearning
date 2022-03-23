# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..','..','..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace,TMApkManager,TMIPAManager,ApkInfo,ThreadManager,Task,DictUtil

from models.ensure import Ensure
from models.testhelp import TestHelper,sleeptouch,logplus
from models.pocohelp import *

import re

__author__ = "tengmu"

from airtest.core.api import *
from airtest.core.error import AdbShellError
curDevice = G.DEVICE


auto_setup(__file__)



from poco.drivers.android.uiautomation import AndroidUiautomationPoco
poco = AndroidUiautomationPoco(use_airtest_input=True, screenshot_each_action=False)


packagepath = TestHelper.getPackagePath()

info:ApkInfo = TMApkManager.getapkinfo(packagepath)
deviceinfo = TestHelper.getDeviceInfo()




def install(self:Task):
    Ensure.package_newest(info)
    
    self.quit()
    logplus('adb安装成功')
def isOpenInstaller(patten):
    out = shell('dumpsys activity activities')
    m = re.search(f'''Run #0: ActivityRecord\{{.*{patten}.*\}}''',out)
    if m == None:
        return False
    return True
def getObserve():
    '''
    根据手机厂商和sdkversion分配安装函数
    '''
    def xiaomi(self:Task):
        # 方法1
        if not isOpenInstaller('com\.miui\.permcenter\.install\.AdbInstallActivity'):
            print('loop')
            return
        # 方法2
        # try:
        #     shell('dumpsys activity AdbInstallActivity')
        #     self.quit()
        # except AdbShellError as e:
        #     pass
        # 点击接受
        sleeptouch(Template(r"tpl1600335095471.png", rgb=True, target_pos=8, record_pos=(-0.228, 0.836), resolution=(1080, 2340)))
        self.quit()

    def android(self:Task):
        logplus('observe执行完成')
        self.quit()
    def huawei(self:Task):
        # if not isOpenInstaller('com\.android\.packageinstaller'):
        #     print('loop')
        #     return
        # sleeptouch(Template(r"tpl1604022739269.png", threshold=0.60, rgb=True, target_pos=9, record_pos=(-0.061, 0.641), resolution=(1080, 1920)))
        # # sleeptouch(Template(r"tpl1604022739269.png", threshold=0.60, rgb=True, target_pos=9, record_pos=(-0.061, 0.641), resolution=(1080, 1920)))
        # pos = wait(Template(r"tpl1604022842512.png", threshold=0.60, record_pos=(-0.004, 0.685), resolution=(1080, 1920)),timeout=60)
        # sleeptouch(pos)
        # wait(Template(r"tpl1604022870384.png", rgb=True, record_pos=(-0.006, -0.608), resolution=(1080, 1920)),timeout=120)
        # sleep(5)
        # TestHelper.back(deviceinfo.serialno)
        # 不知道为什么，不弹窗了
        self.quit()
    def oneplus(self:Task):
        self.quit()
    def oppo(self:Task):
        #wait(Template(r"tpl1629190283001.png", record_pos=(0.008, -0.604), resolution=(1080, 2400)),timeout=180)
        poco("com.coloros.safecenter:id/alertTitle").wait_for_appearance(timeout=180)
        #keyevent("t")
        poco("com.coloros.safecenter:id/et_login_passwd_edit").set_text("tm2021")
        #time.sleep(3)
        #sclick(poco("android:id/button1"),5)
        wait(Template(r"tpl1629364293062.png", record_pos=(0.224, -0.11), resolution=(1080, 2400)),timeout=60)
        sleeptouch(Template(r"tpl1629364293062.png", record_pos=(0.224, -0.11), resolution=(1080, 2400)),5)
        sleeptouch(Template(r"tpl1629190359556.png", record_pos=(0.008, 1.008), resolution=(1080, 2400)),10)
        sclick(poco("com.android.packageinstaller:id/done_button"))  
        self.quit()
    enableObserve = locals()
    manufacturer = deviceinfo.manufacturer
    sdkversion = deviceinfo.sdkversion
    match = f'{manufacturer}'
    observe = DictUtil.tryGetValue(enableObserve,match)
    if observe == None:
        raise StopException(f'{match}安装未实现',{'manufacturer':manufacturer,'sdkversion':sdkversion})
    return observe
        
wake()
installtask = ThreadManager.taskGo(install,count=1)[0]
observetask = ThreadManager.taskGo(getObserve(),count=1)[0]

ThreadManager.waitall_tasks([installtask,observetask])

sleep(15)
wake()
sleep(5)

