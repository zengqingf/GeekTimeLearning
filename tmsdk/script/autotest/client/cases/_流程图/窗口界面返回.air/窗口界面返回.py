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

sleeptouch(Template(r"tpl1601198798966.png",threshold=0.6, record_pos=(0.371, -0.178), resolution=(1440, 810)))
