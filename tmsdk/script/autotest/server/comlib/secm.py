# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
# from comlib import com,workspace,SVNManager,JsonFile,Path
from comlib.dictm import JsonFile
from comlib.svnm import SVNManager
from comlib.pathm import Path
from comlib import com
from comlib.conf.loader import Loader
from comlib.conf.ref import *

import atexit

secDir = os.path.join(com.gethomepath(),'sec')
Path.ensure_direxsits(secDir)

secData:JsonFile = None

class SecretManager:
    @staticmethod
    def getSSHPrivateKey(ip):
        return SecretManager.getSecKey('ssh',ip)
    @staticmethod
    def getSecData(*keys):
        global secData
        if secData == None:
            jsondata = SVNManager.cat('svn://192.168.2.177/sdk/sec/password/pwd.json')
            secData = JsonFile(jsondata_default=jsondata)
        if keys.__len__() == 0:
            return secData
        else:
            return secData.trygetvalue(*keys)

    @staticmethod
    def getSecKey(keyType,keyName):
        savepath = os.path.join(secDir,keyName)
        SVNManager.export(f'svn://192.168.2.177/sdk/sec/key/{keyType}/{keyName}',savepath)
        return savepath
    @staticmethod
    def getSecKeyConf(keyType,keyName):
        jsondata = SVNManager.cat(f'svn://192.168.2.177/sdk/sec/key/{keyType}/{keyName}.json')
        return JsonFile(jsondata_default=jsondata)



