# -*- encoding: utf-8 -*-
__author__ = "tengmu"

import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..','..','..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace,TMApkManager,TMIPAManager,ApkInfo,ThreadManager,Task,DictUtil

from models.ensure import Ensure
from models.testhelp import TestHelper,sleeptouch,logplus,exists_and_touch


from airtest.core.api import *

auto_setup(__file__)


def clear_activty():
    if exists_and_touch(Template(r"tpl1585901420268.png", rgb=True, record_pos=(0.0, 0.201), resolution=(2160, 1080))):
        pass

    while True:
        if exists_and_touch(Template(r"tpl1585901537374.png", threshold=0.6000000000000001, record_pos=(0.334, -0.158), resolution=(2160, 1080)),3):
            pass
        elif exists_and_touch(Template(r"tpl1585901537375.png", threshold=0.6000000000000001, record_pos=(0.334, -0.158), resolution=(2160, 1080)),3):
            pass
        elif exists_and_touch(Template(r"tpl1585901537376.png", threshold=0.7, record_pos=(0.334, -0.158), resolution=(2160, 1080)),3):
            pass
        else:
            break

            
            
count = 0
while count<3:
    count += 1
    sleeptouch(Template(r"tpl1585312665120.png", rgb=True, record_pos=(-0.001, 0.212), resolution=(2160, 1080)))
    sleep(10)
    # wait(Template(r"tpl1601285305769.png", record_pos=(-0.368, 0.173), resolution=(1440, 810)))
    # 保证处于无干扰状态
    clear_activty()
    if exists_and_touch(Template(r"tpl1585902010092.png", rgb=True, record_pos=(-0.463, -0.209), resolution=(2160, 1080))):
        pass
    elif exists_and_touch(Template(r"tpl1585908567659.png", record_pos=(-0.464, -0.206), resolution=(2160, 1080))):
        pass

    sleeptouch(Template(r"tpl1585902055158.png", record_pos=(0.169, 0.172), resolution=(2160, 1080)))
    sleep(5)




