# -*- encoding: utf-8 -*-

from comlib.serverlib.helper import getFailRetContent, getSucRetContent, tryGetData, tryGetFormData
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())

from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace
from comlib.conf.loader import Loader
from comlib.conf.ref import *

from comlib import DictUtil,JsonFile

from flask import Flask,redirect,url_for,request,render_template

import threading,requests
from requests.auth import HTTPBasicAuth


class Api():
    self = None
    @classmethod
    def invoke(cls,methodname,data):
        """
        主运行函数
        """
        if cls.self == None:
            cls.self = cls()
        
        methods = com.getClassMethods(cls)
        return methods[methodname](cls.self,data)


class HTTPAuth(HTTPBasicAuth):
    """Attaches HTTP Basic Authentication to the given Request object."""

    def __call__(self, r):
        r.headers['Authorization'] = f'{self.username} {self.password}'
        return r


class BaseView():
    def __init__(self,app:Flask,viewname):
        self.app = app
        self.viewname = viewname
        print(f'{viewname}初始化成功')
        self.register()
    def apifunc(self):
        """
        api函数
        """
        if request.method == 'GET':
            return self.apiget()
        elif request.method == 'POST':
            return self.apipost()
        
        return ''
    def indexfunc(self):
        '''
        主页函数
        '''
        if request.method == 'GET':
            return self.indexget()
        
        return ''
    def apiget(self):
        return ''
    def apipost(self):
        return ''
    def indexget(self):
        return ''
    def register(self):
        self.app.add_url_rule(f'/{self.viewname}/api/',view_func=self.apifunc,methods=['GET','POST'])
        self.app.add_url_rule(f'/{self.viewname}/index/',view_func=self.indexfunc,methods=['GET'])

views = []
def registview(view):
    views.append(view)
    return view
def initview(app:Flask):
    for view in views:
        view(app)