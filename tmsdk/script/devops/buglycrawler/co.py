# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com


from comlib.comobj import *
G_ftp.upload(os.path.join(thisdir,'index.html'),'/DevOps/BuglyData/index.html',showlog=True,overwrite=True)

