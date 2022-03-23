# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace
from comlib.conf.loader import Loader
from comlib.conf.ref import *

from comlib import ApkInfo

from models.wraps import platform
from models.testhelp import TestHelper,adbs,adb

from airtest.core.api import *
from airtest.core.android import Android
from airtest.core.error import AdbShellError
device:Android = G.DEVICE

class Ensure(object):
    @platform('android')
    @staticmethod
    def package_notexists(apkinfo:ApkInfo):
        apps = device.adb.list_app(True)
        if apkinfo.packagename in apps:
            device.adb.uninstall_app(apkinfo.packagename)
        # try:
            
        # except AdbShellError:
        #     com.logout(f'[uninstall] {apkinfo.packagename}未安装')
        #     pass
        # pass
    @platform('android')
    @staticmethod
    def package_exists(apkinfo:ApkInfo):
        device.adb.pm_install(apkinfo.apkpath)
        tmpfile = f'/data/local/tmp/{os.path.basename(apkinfo.apkpath)}'
        device.adb.shell(f'rm "{tmpfile}"')
        pass
    @platform('android')
    @staticmethod
    def package_newest(apkinfo:ApkInfo):
        Ensure.package_notexists(apkinfo)
        Ensure.package_exists(apkinfo)
    @platform('android')
    @staticmethod
    def app_close(apkinfo:ApkInfo,serialno):
        adbs(serialno,f'am force-stop {apkinfo.packagename}')
    @platform('android')
    @staticmethod
    def app_open(apkinfo:ApkInfo,serialno):
        adbs(serialno,f'monkey -p {apkinfo.packagename} -c android.intent.category.LAUNCHER 1')
    @platform('android')
    @staticmethod
    def app_newestopen(apkinfo:ApkInfo,serialno):
        Ensure.app_close(apkinfo,serialno)
        time.sleep(0.5)
        Ensure.app_open(apkinfo,serialno)
    
    


    
    
    