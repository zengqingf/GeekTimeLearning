# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..','..','..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace,TMApkManager,TMIPAManager,ApkInfo
from models.testhelp import TestHelper,sleeptouch,logplus,exists_and_touch
curDevice = G.DEVICE
curSerialno = G.DEVICE.serialno


__author__ = "tengmu"



from models.pocohelp import *

info = TMApkManager.getapkinfo(TestHelper.getPackagePath())



deviceinfo = TestHelper.getDeviceInfo()

auto_setup(__file__)


package=info.packagename
account="2020101903"
account,password,server_type,server_name = TestHelper.getAccount(info.usesdk,deviceinfo.serialno)

GameHelper.inputGM("!!uplevel num=65")
# GameHelper.rlGame(info)
Ensure.app_close(info)
TestHelper.gotostate('已安装游戏','角色选择')
# GameHelper.login(server_type,server_name,account)
GameHelper.choseRole(0)
GameHelper.closeAllAds()
sleep(5)
GameHelper.inputGM("!!fatask")
Ensure.app_close(info)
TestHelper.gotostate('已安装游戏','角色选择')
# GameHelper.rlGame(package)
# GameHelper.login(server_type,server_name,account)
GameHelper.choseRole(0)
GameHelper.closeAllAds()
sleep(10)
GameHelper.inputGM("!!clearitems type=all")
sleep(30)
GameHelper.inputGM("!!additem id=165890039")
sleep(30)
GameHelper.inputGM("!!additem id=200000004 num=999")
sleep(30)
GameHelper.inputGM("!!opendungeon")
GameHelper.autoSkill()
GameHelper.betterEquip()
GameHelper.challenge("深渊地下城","幻影深渊","普通")
# 神秘商店
for i in "0123456789":
    GameHelper.chapterDungeon(int(i))

# 只测这个脚本要加，测完了注释掉
# TestHelper.putAccount(info.usesdk,deviceinfo.serialno)





