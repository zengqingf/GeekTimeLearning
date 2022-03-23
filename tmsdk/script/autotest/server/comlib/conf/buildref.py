# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.dirname(__file__)
sys.path.append(os.path.abspath(os.path.join(thisdir,'..','..')))

# 文件可能格式错误
with open(os.path.join(thisdir,'ref.py'),'w') as fs:
    pass

from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com



if __name__ == "__main__":
    refpath = os.path.join(thisdir,'ref.py')
    lines = []
    confdir = os.path.join(thisdir,'conf')
    mainconf = os.path.join(confdir,'mainconf.json')
    mainjs = com.loadfile_json(mainconf)

    for confname,confsetting in mainjs.items():
        tmplines = []
        datastruct = confsetting['DEFAULT']
        datanames = list(datastruct.keys())
        
        classname = confname
        tmplines.append(f'class {classname}():')
        initline = '    def __init__(self'

        for name in datanames:
            initline += f',{name}'
            tmplines.append(f'        self.{name} = {name}')
        initline += '):'

        tmplines.insert(1,initline)
        lines += tmplines
    com.savedata('\n'.join(lines),refpath)
            

