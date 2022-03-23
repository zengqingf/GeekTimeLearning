# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..','..','..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace,TMApkManager,TMIPAManager,ApkInfo
from models.testhelp import TestHelper,sleeptouch,logplus,exists_and_touch
from models.testhelp import assert_skipthis,assert_stopalltest

from models.pocohelp import *
curDevice = G.DEVICE
curSerialno = G.DEVICE.serialno




GameHelper.closeNewItemTips()

if(poco("topleft2").offspring("onlinegift").child("Button").exists()):
    poco("topleft2").offspring("onlinegift").child("Button").click()
    gifts=poco("UI2DRoot").offspring("OnlineGift(Clone)").offspring("Middle").child()
    for gift in gifts:
        if(not gift.offspring("accomplish").exists() and not gift.offspring("uncomplete").exists()):
            gift.offspring("receive").click()
            sleep(1)
            assert_skipthis(gift.offspring("accomplish").exists() == True)
    GameHelper.closeAny()    
else:
    snapshot(msg="在线奖励已经领取完了")





