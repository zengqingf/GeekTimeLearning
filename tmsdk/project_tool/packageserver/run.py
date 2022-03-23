# -*- encoding: utf-8 -*-
from comlib.serverlib.helper import getFailRetContent, getSucRetContent, tryGetData, tryGetFormData
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())

from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace,Dingrobot
from comlib.conf.loader import Loader
from comlib.conf.ref import *

from comlib import DictUtil,JsonFile

from flask import Flask,redirect,url_for,request,render_template

import threading,requests
from requests.auth import HTTPBasicAuth


from comlib.serverlib.interface import Api,BaseView,registview,initview
from comlib.comobj import G_ftp,G_ip

from typing import Tuple,List

class PackageApi(Api):
    def __init__(self) -> None:
        super().__init__()


def getFiles(dir,count=20):
    files = os.listdir(dir) 
    files.sort()
    files.reverse()
    files = files[0:count]
    return files

@registview
class PackageView(BaseView):
    def __init__(self, app):
        super().__init__(app, 'pkg')
    def apipost(self):
        # method,data = tryGetData(request)
        # return PackageApi.invoke(method,data)
        pass
    def indexget(self):
        domain = f'https://192.168.2.60:58002'
        gamename = Loader.getgame()
        packagePath = com.get_ftp_savepath_base()
        httpPackagePathBase = com.get_http_savepath_base()

        packagePath_NFSPath = com.convertsep(packagePath)
        if com.isWindows():
            packagePath_NFSPath = f'\\\\192.168.2.60{packagePath_NFSPath}'

        packageBlocks:List[Tuple[str,str,List[Tuple[str,str]]]] = []
        gameDir = packagePath_NFSPath
        plats = os.listdir(gameDir)
        for plat in plats:
            platDir = os.path.join(gameDir,plat)
            packageTypeNames = os.listdir(platDir)
            count = 20 if plat.lower() != 'ios' else 40
            for packageTypeName in packageTypeNames:
                packageTypeDir = os.path.join(platDir,packageTypeName)
                files = getFiles(packageTypeDir,count)
                if plat.lower() == 'ios':
                    ipaUrlBase = f'{domain}/{httpPackagePathBase}/{plat}/{packageTypeName}'

                    ipaFiles = filter(lambda x: x.endswith('.ipa'),files)
                    urlblocks_ipa = []
                    for ipaFile in ipaFiles:
                        if os.path.exists(os.path.join(packageTypeDir,ipaFile + '.plist')):
                            urlblocks_ipa.append((f'itms-services://?action=download-manifest&url={com.urlencode(f"{ipaUrlBase}/{ipaFile}.plist")}',ipaFile))
                        else:
                            urlblocks_ipa.append((f'{ipaUrlBase}/{ipaFile}',ipaFile))
                    packageBlocks.append((plat,packageTypeName,urlblocks_ipa))

                else:
                    urlBlocks = [(f'{domain}/{httpPackagePathBase}/{plat}/{packageTypeName}/{file}',file) for file in files]
                    packageBlocks.append((plat,packageTypeName,urlBlocks))

        return render_template('index.html',game=gamename,packageBlocks=packageBlocks)


app = Flask(__name__)
if __name__ == '__main__':
    initview(app)
    app.run(G_ip, 5001,debug=True)
