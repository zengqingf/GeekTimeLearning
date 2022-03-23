# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace,JsonFile
from comlib.conf.loader import Loader
from comlib.conf.ref import *


from comlib import RecordTool,RecordStruct,SVNManager


import argparse


recordfilepath = os.path.join(workdir,'projconf','record.json')
# recordfilepath = 'record.json'
record = RecordTool(recordfilepath)

plats = ['windows','android','ios']


def run():
    parser = argparse.ArgumentParser()
    parser.add_argument('--action',required=True)
    parser.add_argument('--plat',required=True)
    parser.add_argument('--channel',required=True)
    parser.add_argument('--serverversion',required=True)
    parser.add_argument('--clientversion',required=True)
    parser.add_argument('--verifymd5',required=True)
    parser.add_argument('--versioncode',required=True)
    parser.add_argument('--type',required=True)
    parser.add_argument('--download',required=True)
    args = parser.parse_args(sys.argv[2:])


    if args.action == 'prepublish':
        record.prepublish(args.plat,args.channel,RecordStruct(args.serverversion,args.clientversion,args.verifymd5,
        args.versioncode,args.type,args.download))
    else:
        record.publish(args.plat,args.channel,args.type)
        

def serverversion():
    parser = argparse.ArgumentParser()
    parser.add_argument('--plat',required=True)
    args,unkone = parser.parse_known_args(sys.argv[2:])
    if args.plat == 'all':
        for plat in plats:
            record.setnextserverversion(plat)
    else:
        record.setnextserverversion(args.plat)
    # record.commit(msg=f'{plat}服务器版本号更新')

def versioncode():
    parser = argparse.ArgumentParser()
    parser.add_argument('--plat',required=True)
    parser.add_argument('--channel',required=True)
    args = parser.parse_args(sys.argv[2:])

    def update(plat,channel='all'):
        nextcode = record.getnextversioncode(plat)
        if channel == 'all':
            channels = record.getallchannel(plat)    
            for channel in channels:
                record.setversioncode(plat,channel,nextcode)
        else:
            record.ensure_channelexists(plat,channel)
            record.setversioncode(plat,channel,nextcode)
    if args.plat == 'all':
        if args.channel == 'all':
            for plat in plats:
                update(plat)
        else:
            raise Exception(f'指定type=all时不能指定channel为单一渠道，因为渠道不跨平台')
    else:
        if args.channel == 'all':
            update(args.plat)
        else:
            update(args.plat,channel=args.channel)
                        
if __name__ == "__main__":
    # sys.argv = ['','serverversion','--plat=all','debug']
    SVNManager.upgrade(os.path.join(thisdir,'..'))
    if os.path.exists(os.path.join(workdir,'projconf')):
        SVNManager.upgrade(os.path.join(workdir,'projconf'))
    type = sys.argv[1]
    if type == 'record':
        run()
    elif type == 'serverversion':
        serverversion()
    elif type == 'versioncode':
        versioncode()
    # TODO 记录资源版本号
