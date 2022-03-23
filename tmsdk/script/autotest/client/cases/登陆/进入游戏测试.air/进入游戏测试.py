# -*- encoding: utf-8 -*-
__author__ = "tengmu"

import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..','..','..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace,TMApkManager,TMIPAManager,ApkInfo,ThreadManager,Task,DictUtil

from models.ensure import Ensure
from models.testhelp import TestHelper,sleeptouch,logplus,assert_stopalltest


from airtest.core.api import *

auto_setup(__file__)

# info = TMApkManager.getapkinfo(TestHelper.getPackagePath())
deviceinfo = TestHelper.getDeviceInfo()
info = TMApkManager.getapkinfo(TestHelper.getPackagePath())

package_name, activity_name, pid = G.DEVICE.adb.get_top_activity()
# 引擎闪退确认
logplus(f'package_name={package_name} info.packagename={info.packagename}')
assert_stopalltest(package_name == info.packagename)


sleep(3)
keyevent('BACK')
# TestHelper.back(deviceinfo.serialno)
sleep(3)
keyevent('BACK')
# TestHelper.back(deviceinfo.serialno)
sleep(3)
# 卡死测试
assert_not_exists(Template(r"tpl1618364020907.png", rgb=True, threshold=0.85, record_pos=(-0.001, -0.006), resolution=(1920, 1080)),'断言进入游戏')

package_name, activity_name, pid = G.DEVICE.adb.get_top_activity()
# 游戏流程闪退确认
assert_stopalltest(package_name == info.packagename)


assert_not_exists(Template(r"tpl1618466483821.png", rgb=True,threshold=0.90, record_pos=(0.182, -0.001), resolution=(1920, 1080)))
