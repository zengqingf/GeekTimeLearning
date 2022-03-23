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


sleeptouch(Template(r"tpl1601199208680.png", record_pos=(0.456, -0.102), resolution=(1440, 810)),10)

exists_and_touch(Template(r"tpl1601199181373.png", target_pos=4, record_pos=(0.006, 0.048), resolution=(1440, 810)))

