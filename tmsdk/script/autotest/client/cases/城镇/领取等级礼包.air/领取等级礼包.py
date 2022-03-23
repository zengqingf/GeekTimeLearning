# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..','..','..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace,TMApkManager,TMIPAManager,ApkInfo
from models.testhelp import TestHelper,sleeptouch,logplus,exists_and_touch

from models.pocohelp import *
curDevice = G.DEVICE
curSerialno = G.DEVICE.serialno




GameHelper.closeNewItemTips()

if(poco("topleft2").offspring("uplevelgift").child("Button").exists()):
    poco("topleft2").offspring("uplevelgift").child("Button").click()
else:
    print("已经领取完了")
i=True
while True:
    gifts=poco("UI2DRoot").offspring("LevelGift(Clone)").offspring("Middle").child()
    for gift in gifts:
        if(gift.offspring("receive").exists()):
            gift.offspring("receive").click()
        if not(gift.offspring("TextTitle").exists()):
            continue
        if(gift.offspring("TextTitle").get_text()=="65级礼包"):
            print(gift.offspring("TextTitle").get_text())
            i=False
    if (i):        
        poco("UI2DRoot").offspring("next").click()
    else:
        GameHelper.closeAny()
        break

