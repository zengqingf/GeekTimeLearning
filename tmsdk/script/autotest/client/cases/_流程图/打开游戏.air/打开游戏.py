# -*- encoding: utf-8 -*-
__author__ = "tengmu"

import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..','..','..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace,TMApkManager,TMIPAManager,ApkInfo,ThreadManager,Task,DictUtil

from models.ensure import Ensure
from models.testhelp import TestHelper,sleeptouch,logplus,assert_stopalltest,exists_and_touch
from models.pocohelp import *


from airtest.core.api import *


from poco.drivers.android.uiautomation import AndroidUiautomationPoco
poco = AndroidUiautomationPoco(use_airtest_input=True, screenshot_each_action=False)

auto_setup(__file__)

info = TMApkManager.getapkinfo(TestHelper.getPackagePath())





deviceinfo = TestHelper.getDeviceInfo()

#同意各种权限
def AllowPermission():
    allowbtn = poco("com.android.permissioncontroller:id/permission_allow_button1")
    time = 0
    while True:
        if time>4:
            break
        if allowbtn.exists():
            sclick(allowbtn,1)
            time = time +1
        else:
            break

def android_0():
    pass
def xiaomi_12():
    while exists_and_touch(Template(r"tpl1602486396688.png", rgb=True, target_pos=8, record_pos=(-0.003, -0.023), resolution=(2340, 1080))):
        pass
def huawei_9_1_0():
    while exists_and_touch(Template(r"tpl1604025765604.png", target_pos=9, record_pos=(-0.017, 0.098), resolution=(1920, 1080)),2):
        pass
def oneplus_max_12():
    while exists_and_touch(Template(r"tpl1629192524706.png", record_pos=(0.002, 0.032), resolution=(3120, 1440)),2):
        pass
    
def oppo_max_12():
    AllowPermission()

quanxian_func = {
    'android': {
        '0-max':android_0
    },
    'xiaomi':{
        '12-max':xiaomi_12
    },
    'huawei':{
        '9.1.0-max':huawei_9_1_0
    },
    'oneplus':{
        '12-max':oneplus_max_12
    },
    'oppo':{
        '12-max':oppo_max_12
    },
    'exp':{
        '1.2.3-2.2.2':None,
        '2.2.3-max':{
            '16-max':lambda : print(1)
        }
    }
}

def calllll(manufacturer,romVersion,androidapi=None):
    romVersionRef = DictUtil.tryGetValue(quanxian_func,manufacturer)
    if romVersionRef == None:
        StopException(f'权限清理未实现',{'manufacturer':manufacturer,'romVersion':romVersion})
    isfind = False
    curRomVersionVal = None
    def isInRange(flagval,val):
        tmp = flagval.split('-')
        low = tmp[0]
        high = tmp[1]
        if com.dotstrcompare(val,low) and (high == 'max' or com.dotstrcompare(high,val)):
            return True
        return False
    for k,v in romVersionRef.items():
        if isInRange(k,romVersion):
            isfind = True
            curRomVersionVal = v
            break
    if not isfind or curRomVersionVal == None:
        raise StopException(f'权限清理未实现{manufacturer} {romVersion}',{})

    if callable(curRomVersionVal):
        curRomVersionVal()
        return
    isfind = False
    curAndroidApiVal = None
    # TODO android api 区分
    curAndroidApiRef = curRomVersionVal
    for k,v in curAndroidApiRef.items():
        if isInRange(k,romVersion):
            isfind = True
            curAndroidApiVal = v
            break
    if not isfind or curAndroidApiVal == None:
        StopException(f'权限清理未实现',{'manufacturer':manufacturer,'androidapi':androidapi})
    curAndroidApiVal()



def clear_quanxian():
    sleep(5)
    calllll(deviceinfo.manufacturer,deviceinfo.romversion,deviceinfo.sdkversion)


start_app(info.packagename)
sleep(5)
package_name, activity_name, pid = G.DEVICE.adb.get_top_activity()
# 闪退确认
#assert_stopalltest(package_name != info.packagename)
# 清理权限
clear_quanxian()


