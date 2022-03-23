# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace

import requests,json


class NetHelper():
    def __init__(self,domain):
        """
        domain:域名
        """
        self.domain = domain
    def getSendJsonData(self,rawdata,method=None):
        data = {'data':rawdata}
        if method != None:
            data['method'] = method
        return data
    def postdata(self,route,jsondata,method=None):
        data = self.getSendJsonData(jsondata,method)
        url = f'http://{self.domain}/{route}/'

        ret = requests.post(url,json=data)
        if ret.ok:
            data = ret.json()
            if not self.isOk(data):
                return data['msg'],False
            return self.getRetData(data),True
        return None,False
    def getdata(self,route,jsondata=None,method=None):
        data = self.getSendJsonData(jsondata,method)
        url = f'http://{self.domain}/{route}/'

        # json标志是http头会加json标志
        # data标志是http头会加form标志
        ret = requests.get(url,json=data)
        if ret.ok:
            data = ret.json()
            if not self.isOk(data):
                return data['msg'],False
            return self.getRetData(data),True
        return None,False
    def isOk(self,data):
        if data['code'] != 200:
            return False
        return True
    def getRetData(self,data):
        return data['data']