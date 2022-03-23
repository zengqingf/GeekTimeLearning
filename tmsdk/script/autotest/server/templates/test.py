# -*- encoding: utf-8 -*-
from os import name
from os.path import join
from sys import path










from flowloader import gettestmap
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH, test
from comlib import com,workspace,ZipManager,DictUtil
from comlib.conf.loader import Loader
from comlib.conf.ref import *

from comlib import XMLFile

import argparse

from models.testhelp import TestHelper,adbs

# print(TestHelper.getfilteredtestphones(['测试机','mumu模拟器_win']))
from xml.dom.minidom import parse
import xml.dom.minidom

from poco.drivers.unity3d import UnityPoco
from poco.proxy import UIObjectProxy
from poco.utils.query_util import build_query

import re
import time,textwrap

# import cv2
# print(cv2.__version__)
from comlib import HTTPManager
import requests
from models.testhelp import TestHelper
import argparse

import airtest

from jinja2 import Environment,FileSystemLoader

env = Environment(loader=FileSystemLoader(os.getcwd()),
trim_blocks=True)
summary = {
    'start_all':111,
    'time':3929,
    'success':80,
    'unstable':10,
    'count':100,
    'result':[
        {
            'tests':{
                '设备1':{
                    'status':'success',
                    'chinese':'成功',
                    'path':'http://www.baidu.com'
                }
            },
            'script':'测试脚本'
        },
        {
            'tests':{
                '设备2':{
                    'status':'failed',
                    'chinese':'失败',
                    'path':'http://www.baidu.com'
                }
            },
            'script':'测试脚本2'
        },
        {
            'tests':{
                '设备3':{
                    'status':'unstable',
                    'chinese':'不稳定',
                    'path':'http://www.baidu.com'
                }
            },
            'script':'测试脚本3'
        }
    ]
}
html = env.get_template('report_tpl.html').render(data=summary)
with open("report.html", "w", encoding="utf-8") as f:
    f.write(html)






# print(TestHelper.getEnableDevicesInfo())

# C:\Users\tengmu\AppData\Local\Programs\Python\Python39\lib\site-packages
# paser = argparse.ArgumentParser()
# paser.add_argument('--devices',nargs='+')
# args = paser.parse_args()
# print(args.devices)
# print(TestHelper.getDevice('华为荣耀V9'))
# print(TestHelper.putDevice('华为荣耀V9'))

# TestHelper.putDevice('590250e6')
# TestHelper.putDevice('6EB0217808004166')


# res = requests.get('https://www.tsa.cn/time.jspx') # 国家授时中心标准时间
# print(res.content)
# detector = cv2.xfeatures2d.SIFT_create(edgeThreshold=10)
# detector = cv2.xfeatures2d.SURF_create(self.HESSIAN_THRESHOLD, upright=self.UPRIGHT)
# detector = cv2.xfeatures2d.SIFT_create(edgeThreshold=10)
# detector = cv2.xfeatures2d.SIFT_create(edgeThreshold=10)
# detector = cv2.xfeatures2d.SIFT_create(edgeThreshold=10)
# print(textwrap.dedent('''
#     def asdf():
#         adfdew
#         afasdf
# '''))
# 192.168.2.228
# 192.168.2.168
# 192.168.2.145
# 192.168.2.146
# 192.168.2.247

# 192.168.3.198







# exec(compile('print(__file__)',filename=r'D:\_WorkSpace\_sdk2\DevOps_Scripts\pyscripts\autotest\test2.py',mode='exec'))
# time.sleep(100)


# quanxian_func = {
#     'android': {
#         '0-max':111
#     },
#     'xiaomi':{
#         '12-max':222
#     },
#     'huawei':{
#         '9.1.0-max':333
#     },
#     'exp':{
#         '1.2.3-2.2.2':None,
#         '2.2.3-max':{
#             '16-max':lambda : print(1)
#         }
#     }
# }
# def calllll(manufacturer,romVersion,androidapi=None):
#     romVersionRef = DictUtil.tryGetValue(quanxian_func,manufacturer)
#     if romVersionRef == None:
#         StopException(f'权限清理未实现',{'manufacturer':manufacturer,'romVersion':romVersion})
#     isfind = False
#     curRomVersionVal = None
#     def isInRange(flagval,val):
#         tmp = flagval.split('-')
#         low = tmp[0]
#         high = tmp[1]
#         if com.dotstrcompare(val,low) and (high == 'max' or com.dotstrcompare(high,val)):
#             return True
#         return False
#     for k,v in romVersionRef.items():
#         if isInRange(k,romVersion):
#             isfind = True
#             curRomVersionVal = v
#             break
#     if not isfind or curRomVersionVal == None:
#         raise StopException(f'权限清理未实现{manufacturer} {romVersion}',{})

#     if callable(curRomVersionVal):
#         curRomVersionVal()
#         return
#     isfind = False
#     curAndroidApiVal = None
#     # TODO android api 区分
#     curAndroidApiRef = curRomVersionVal
#     for k,v in curAndroidApiRef.items():
#         if isInRange(k,romVersion):
#             isfind = True
#             curAndroidApiVal = v
#             break
#     if not isfind or curAndroidApiVal == None:
#         StopException(f'权限清理未实现',{'manufacturer':manufacturer,'androidapi':androidapi})
#     curAndroidApiVal()
# calllll('huawei','9.1.0')

# d = com.readall('open2.txt')
# d,code = adbs('6EB0217808004166','dumpsys activity activities')
# m = re.search('''Run #0: ActivityRecord\{.*com\.android\.packageinstaller/\..*\}''',d)
# print(m)


# raise StopException('配置不存在',{'tpname':1,'keys':2})
# deviceinfo = TestHelper.getDeviceInfo('6EB0217808004166')
# print(1)
# from m1 import *
# import m1
# print(m1.ins.name)
# m1.reset()
# print(m1.ins.name)


# poco = UnityPoco()
# proxy = UIObjectProxy(poco)
# def selectall(poco,*nodenames):
#     """
#     一次搜索多个节点
#     """
#     selector = poco.agent.hierarchy.selector
#     root = selector.getRoot()
#     res = []
#     for name in nodenames:
#         query = build_query(name)
#         nodes = selector.selectImpl(query, False, root, 9999, True, True)
#         res.append(nodes)
#     return res
# def getAttr(poco,node,attrname):
#     """
#     获取attr属性
#     """
#     return poco.agent.hierarchy.getAttr(node,attrname)
# TestHelper.putAccount('dnfdebug','127.0.0.1:7555')
# def getChild_tree(poco,parentname,*childs):
#     parentquery = build_query(parentname)
#     query = parentquery
#     for child in childs:
#         childquery = build_query(child)
#         query = ('/'(parentquery,childquery))
#     p = UIObjectProxy(poco)
#     p.query = query
#     return p
    


# ZipManager.ziptarget([os.path.join(workdir,'projconf'),os.path.join(workdir,'flowloader.py')],'test.zip')
# print(TestHelper.getAccount('mg'))
# fp = r'D:\_WorkSpace\_sdk2\DevOps_Scripts\pyscripts\autotest\flow.drawio'
# m = gettestmap()
# print(1)
# document = xml.dom.minidom.parse(fp)
# collection = document.documentElement
# diagram = collection.getElementsByTagName('diagram')[0]
# mxGraphModel = diagram.getElementsByTagName('mxGraphModel')[0]
# root = mxGraphModel.getElementsByTagName('root')[0]
# mxCells = root.getElementsByTagName('mxCell')
# for c in mxCells:
#     print(c.getAttribute('aaaa'))

# aaa = TestHelper.getAccount('dnfdebug')
# print(aaa)

# xmlf = XMLFile(r'settings\acc.xml')
# e = xmlf.findall(xmlf.root,'asdfas')
# for eee in e:
#     print(eee)


# class ASD():
#     name = 1
#     @classmethod
#     def aaa(cls,a):
#         """
#         docstring
#         """
#         print(cls)
#         print(dir(cls))
#         print(a)


# ASD.aaa(1)



# out,code = com.cmd('adb -s 127.0.0.1:7555 shell am force-stop com.hegu.d')
# print(code)
# out,code = com.cmd('''PowerShell -Command "& {$proc=Start-Process -FilePath airtest -ArgumentList run -NoNewWindow -Wait -PassThru;exit $proc.ExitCode}"''',getstdout=False)
# # out,code = com.cmd('''PowerShell -Command "asdf"''',getstdout=False)
# # out,code = com.cmd('''PowerShell -Command "$asd=1;echo $asd"''',getstdout=False)
# # print(out)
# print('---------------------------------------')
# print(code)

# out,code = com.cmd('airtest run')
# print(out)
# print(code)