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

poco("beStrong").offspring("icon").click()
list=poco("beStrong").offspring("children").child()
for i in list:
    if not(poco("beStrong").offspring("children").exists()):
        poco("beStrong").offspring("icon").click()
        sleep(1)
    if(i.offspring("icon").exists()):
        i.offspring("icon").click()
        sleep(1)
        GameHelper.closeAny()
    elif(i.offspring("Button").exists()):
        i.offspring("Button").click()
        sleep(1)
        GameHelper.closeAny()