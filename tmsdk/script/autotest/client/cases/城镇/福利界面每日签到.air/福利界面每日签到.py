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

sclick(poco("activeFuli"),2)
if poco("Content").offspring("btnSignIn").exists():
    sclick(poco("Content").offspring("btnSignIn"),2)
    # assert_skipthis(not poco("Content").exists())

sclick(poco(text="点击空白位置关闭"),2)

GameHelper.closeAny()


