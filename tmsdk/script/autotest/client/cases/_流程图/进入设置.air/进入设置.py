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


# TODO 根据账号判断
if exists_and_touch(Template(r"tpl1602571671279.png", rgb=True, record_pos=(-0.429, -0.19), resolution=(2340, 1080))):
    pass
elif exists_and_touch(Template(r"tpl1585902010092.png", rgb=True, record_pos=(-0.463, -0.209), resolution=(2160, 1080))):
    pass
elif exists_and_touch(Template(r"tpl1585908567659.png", rgb=True, record_pos=(-0.464, -0.206), resolution=(2160, 1080))):
    pass





