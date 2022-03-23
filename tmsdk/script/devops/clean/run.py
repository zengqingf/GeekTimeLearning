# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace
from comlib.conf.loader import Loader
from comlib.conf.ref import *

import zlib
from io import BytesIO
from typing import List

import shutil

from comlib.comobj import *

paths = [
    r'\\192.168.2.60\client_ftp\package\A8\ios\trunk_debug',
    r'\\192.168.2.60\client_ftp\package\A8\android\trunk_debug'
]

def clean(path,day=15):
    files = com.listdir_fullpath(path)
    tsp = com.gettsp()

    # 1623297107.9164455
    # 1627980676411
    for file in files:
        time = os.path.getmtime(file)
        deltaDay = int((tsp - com.pythonTsp2unixTsp(time)) / 1000 / 60 / 60 / 24)
        if deltaDay > day:
            Log.info(f'[remove] {file}')
            os.remove(file)
def main():
    for p in paths:
        clean(p)
    




if __name__ == '__main__':
    main()