# -*- encoding: utf-8 -*-
__author__ = "tengmu"


import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..','..','..')))
from comlib.exception import errorcatch,errorcatch_func,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace,TMApkManager,TMIPAManager,ApkInfo,ThreadManager,Task,DictUtil

from models.ensure import Ensure
from models.testhelp import TestHelper,sleeptouch,logplus,exists_and_touch


from airtest.core.api import *



auto_setup(__file__)

info = TMApkManager.getapkinfo(TestHelper.getPackagePath())

deviceinfo = TestHelper.getDeviceInfo()

def clear_gonggao():
    while exists(Template(r"tpl1578024234315.png", rgb=True, target_pos=6, record_pos=(-0.01, 0.169), resolution=(2160, 1080))):
        # 可以截图
        sleeptouch(Template(r"tpl1578024234315.png", rgb=True, target_pos=6, record_pos=(-0.01, 0.169), resolution=(2160, 1080)),6)
    # 公告
    assert_exists(Template(r"tpl1578024398833.png", rgb=True, threshold=0.8500000000000001, record_pos=(-0.0, 0.17), resolution=(2160, 1080)),'登陆成功')
    sleeptouch(Template(r"tpl1578024398833.png", rgb=True, threshold=0.8500000000000001, record_pos=(-0.0, 0.17), resolution=(2160, 1080)))

@errorcatch_func(HIGH)
def _call_local(brand,localfuncs,err,romversion=None,sdkversion=None):
    ok = False
    funcname = brand
    for funcname in localfuncs:
        if '_' in funcname:
            tmp:list = funcname.split('_')
            targetBrand = tmp.pop(0)
            minROMVersion = float(tmp.pop(0))
            maxROMVersion = 1000.0
            if tmp.__len__() != 0:
                maxROMVersion = float(tmp.pop(0))
            minSDKVersion = -1.0
            maxSDKVersion = 1000.0
            if tmp.__len__() != 0:
                minSDKVersion = float(tmp.pop(0))
            if tmp.__len__() != 0:
                maxSDKVersion = float(tmp.pop(0))

            if targetBrand != brand:
                continue
            if romversion != None:
                if minROMVersion <= float(romversion) <= maxROMVersion:
                    pass
                else:
                    continue
            if sdkversion != None:
                if minSDKVersion <= float(sdkversion) <= maxSDKVersion:
                    pass
                else:
                    continue
            ok = True
            break

    if ok:
        func = DictUtil.tryGetValue(localfuncs,funcname)
    else:
        func = DictUtil.tryGetValue(localfuncs,brand)
    if func != None:
        func()
    else:
        raise err



def android_0():
    pass
def xiaomi_12():
    while exists_and_touch(Template(r"tpl1602486396688.png", rgb=True, target_pos=8, record_pos=(-0.003, -0.023), resolution=(2340, 1080))):
        pass
def huawei_9_1_0():
    while exists_and_touch(Template(r"tpl1604025765604.png", target_pos=9, record_pos=(-0.017, 0.098), resolution=(1920, 1080)),2):
        pass

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


def login():
    # local环境内不准用下划线_
    account,password,servertype,servername = TestHelper.getAccount(info.usechannel,deviceinfo.serialno,False)
    def dnfdebug():
        clear_gonggao()
        GameHelper.login(servertype,servername,account)
        
        # # 先选服务器，不然不容易试别
        # sleeptouch(Template(r"tpl1599443393073.png", threshold=0.65, rgb=True, record_pos=(0.074, -0.008), resolution=(2340, 1080)))
        # swipe(Template(r"tpl1585310800680.png", record_pos=(-0.269, -0.173), resolution=(2160, 1080)),Template(r"tpl1599443460201.png", rgb=True, record_pos=(-0.28, 0.21), resolution=(2340, 1080)), vector=[0.0737, 0.8365])
        # sleep(3)
        # if not exists(Template(r"tpl1585311632896.png", threshold=0.9000000000000001, rgb=True, record_pos=(-0.267, -0.126), resolution=(2160, 1080))):
        #     sleeptouch(Template(r"tpl1585311612864.png", threshold=0.7, rgb=True, record_pos=(-0.269, -0.126), resolution=(2160, 1080)),3)
        # # sleeptouch(Template(r"tpl1585311661793.png", rgb=False, target_pos=6, record_pos=(-0.187, -0.124), resolution=(2160, 1080)),3)
        # sleeptouch(Template(r"tpl1598953340167.png", target_pos=9, record_pos=(-0.174, -0.087), resolution=(2340, 1080)),3)
        # sleeptouch(Template(r"tpl1599443790426.png", threshold=0.85, rgb=True, target_pos=8, record_pos=(0.001, 0.009), resolution=(2340, 1080)))
        # TestHelper.clearinput()
        # TestHelper.input_raw(deviceinfo.serialno,account)
        # time.sleep(1)
        
        # sleeptouch(Template(r"tpl1602489554332.png", rgb=True, target_pos=8, record_pos=(0.001, 0.027), resolution=(2340, 1080)))
        # # 小米安全键盘会导致暂时中断，下面输入需要用纯adb操作
        # TestHelper.clearinput()
        # TestHelper.input_raw(deviceinfo.serialno,password)
        # time.sleep(1)
        # # 小米安全键盘导致的中断在下面touch中恢复
        # touch_login_entergame()

    def mg():
        # mgsdk操作
        if exists_and_touch(Template(r"tpl1578045236088.png", threshold=0.8, rgb=True, target_pos=8, record_pos=(-0.007, 0.086), resolution=(2160, 1080)),1):
            pass
        elif exists_and_touch(Template(r"tpl1578129362900.png", threshold=0.7, rgb=True, target_pos=8, record_pos=(-0.0, 0.081), resolution=(2160, 1080))):
            pass

        if exists_and_touch(Template(r"tpl1578045068093.png", record_pos=(0.084, 0.054), resolution=(2160, 1080))):
            sleeptouch(Template(r"tpl1585617611017.png", threshold=0.7, rgb=True, target_pos=1, record_pos=(0.116, 0.049), resolution=(2160, 1080)))

            TestHelper.clearinput()
            TestHelper.input_raw(deviceinfo.serialno,account,autoenter=False,usetab=False)
            TestHelper.back(deviceinfo.serialno)
            time.sleep(1)
            while not exists(Template(r"tpl1585619869550.png", rgb=True, target_pos=1, record_pos=(0.119, 0.074), resolution=(2160, 1080))):
                TestHelper.back(deviceinfo.serialno)
                time.sleep(3)

            sleeptouch(Template(r"tpl1585619869550.png", rgb=True, target_pos=1, record_pos=(0.119, 0.074), resolution=(2160, 1080)),3)


            TestHelper.clearinput()
            TestHelper.input_raw(deviceinfo.serialno,password,autoenter=False,usetab=False)
            TestHelper.back(deviceinfo.serialno)
            time.sleep(1)
            sleeptouch(Template(r"tpl1578122772466.png", threshold=0.5, rgb=True, record_pos=(0.003, 0.085), resolution=(2160, 1080)),3)
        time.sleep(5)
        
        clear_gonggao()
        
        sleeptouch(Template(r"tpl1599443393073.png", rgb=True, record_pos=(0.074, -0.008), resolution=(2340, 1080)),3)
        if not exists(Template(r"tpl1585908120201.png", rgb=True, target_pos=8, record_pos=(-0.264, -0.138), resolution=(2160, 1080))):
            sleeptouch(Template(r"tpl1585908198239.png", target_pos=2, record_pos=(-0.261, 0.062), resolution=(2160, 1080)),3)
        sleeptouch(Template(r"tpl1592896923075.png", rgb=True, target_pos=7, record_pos=(0.102, -0.138), resolution=(2160, 1080)),3)
        touch_login_entergame()
        
    
    match = f'{info.usesdk}'
    _call_local(match,locals(),
    StopException(f'{match}登陆未实现',{'brand':deviceinfo.brand,'sdkversion':deviceinfo.sdkversion}))
def touch_login_entergame():
    sleeptouch(Template(r"tpl1599443974404.png", threshold=0.75, rgb=True, record_pos=(0.002, 0.138), resolution=(2340, 1080)))
    #超时时间15s
    sleep(17)
    assert_not_exists(Template(r"tpl1585900745700.png", record_pos=(0.003, 0.004), resolution=(2160, 1080)), "开发服务器不在维护")
    
    # 保证处于无干扰状态
    if exists(Template(r"tpl1585312556435.png", threshold=0.8, rgb=True, target_pos=4, record_pos=(0.011, 0.081), resolution=(2160, 1080))):
        sleeptouch(Template(r"tpl1585312556435.png", threshold=0.8, rgb=True, target_pos=4, record_pos=(0.011, 0.081), resolution=(2160, 1080)))
    
    pass


# 根据手机型号清除权限提醒
clear_quanxian()
# 加载流程，暂时不考虑乐变和热更
# sleep(10)
# out,code=com.cmd('adb forward --list')
# print(out)
sleep(20)
# 必须等游戏初始化完成，不然不会和unity poco建立连接
from models.pocohelp import *
# 根据包的sdk进行登陆
login()





























