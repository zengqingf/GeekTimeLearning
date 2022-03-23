# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace
from comlib.conf.loader import Loader
from comlib.conf.ref import *

from comlib.wraps import call
from comlib import DictUtil
from comlib import ApkInfo

from functools import wraps


def platform(*okplats):
    def innerwrap(func):
        precalls = []
        for okplat in okplats:
            if okplat == 'android':
                def android_wrap(packageinfo:ApkInfo):
                    assert packageinfo.apkpath.endswith('.apk')
                precalls.append(android_wrap)
            elif okplat == 'ios':
                def ios_wrap(packageinfo):
                    assert packageinfo.endswith('.ipa')

                precalls.append(ios_wrap)
            else:
                raise Exception(f'不支持平台{okplat}')
        @wraps(func)
        def desc(*a,**kv):
            packageinfo = a[0] if a.__len__() != 0 else ''
            packageinfo = kv['packageinfo'] if 'packageinfo' in kv else packageinfo
            packageinfo = kv['apkinfo'    ] if 'apkinfo'     in kv else packageinfo
            packageinfo = kv['ipainfo'    ] if 'ipainfo'     in kv else packageinfo
            if packageinfo == '':
                raise Exception('少info参数了')
            for precall in precalls:
                precall(packageinfo)
            call(func,*a,**kv)
        return desc
    return innerwrap

