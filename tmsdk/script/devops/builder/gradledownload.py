# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace
from comlib.conf.loader import Loader
from comlib.conf.ref import *

from comlib import XMLFile,HTTPManager,Path

import requests

if __name__ == "__main__":
    res = requests.get('https://services.gradle.org/distributions/')
    # print(res.content)
    data = res.content.decode(encoding='utf-8')
    # data = com.readall('t.xml')
    import re
    ms = re.findall('href="(/distributions/.*?)"',data)
    savepath = os.path.join(thisdir,'gradle')
    Path.ensure_dirnewest(savepath)
    sss = []
    for m in ms:
        url = f'https://services.gradle.org{m}'
        sss.append(url)
        # HTTPManager.download_http(url,savepath)
        # print(url)
    for s in sss:
        HTTPManager.download_http(s,savepath)
        # print(s)
    print('okokokokokokokokok')
    # print(1)
    # com.savedata(data,'t.xml')
    # xmlf = XMLFile(data)
    # htmle = xmlf.findpath(xmlf.root,'body->div|id=contents->ul|class=items')
    # print(1)
    pass