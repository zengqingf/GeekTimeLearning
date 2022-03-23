# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace
from comlib.conf.loader import Loader
from comlib.conf.ref import *

from comlib import DictUtil

import json

def tryGetData(request):
    js = {}
    
    if request.args.__len__() != 0:
        js = request.args.to_dict()
    elif request.form.__len__() != 0:
        js = request.form.to_dict()
    elif request.json != None:
        js = request.json
    elif request.data != None:
        pass
    elif request.files.__len__() != 0:
        pass
    data = DictUtil.tryGetValue(js,'data')
    method = DictUtil.tryGetValue(js,'method')
    print(f'[getdata] [method]={method} [data]={data}')
    return method,data
def tryGetFormData(request):
    js = request.form.to_dict()
    print(f'[getformdata] [data]={js}')
    return js
    
def getSucRetContent(data):
    content = {
        'code': 200,
        'data':data
    }
    senddata = json.dumps(content)
    print(f'[send]{senddata}')
    return senddata
def getFailRetContent(code,msg):
    content = {
        'code':code,
        'msg':msg
    }
    return json.dumps(content)