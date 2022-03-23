# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace
from comlib.conf.loader import Loader
from comlib.conf.ref import *

from builder.build import argparse,UE4AndroidBuilder,UE4IOSBuilder,UE4WindowsBuilder,LightmassQuality
from comlib import UE4Engine




if __name__ == "__main__":
    # argv = ['--platform=android','--mode=Development','--cookFlavor=ASTC','--level=0','--branchDesc=release','--svnVersion=HEAD',
    # '--useEngine=SourceInstalled','--lightmass_quality=SKIP','--branchExtName=Beta',
    # '--isA8Project=false','--forceSvnUrl=svn://192.168.2.177/sdk/test/ue4buildtest']
    parser = argparse.ArgumentParser()
    # 打包配置 start------------------------------------
    parser.add_argument('--platform',required=True)
    parser.add_argument('--mode',required=True)
    parser.add_argument('--cookFlavor',required=True)
    parser.add_argument('--lightmass_quality',required=True,type=str)
    parser.add_argument('--level',required=True,type=int)
    parser.add_argument('--branchDesc',required=True)
    parser.add_argument('--svnVersion',required=True)
    parser.add_argument('--useEngine',required=True)
    # 配置体验服之类分支名字
    parser.add_argument('--branchExtName',required=False,default=None,type=com.str2Noneable)
    parser.add_argument('--bPackageDataInsideApk',required=False,default=True,type=com.str2boolean)
    # dis 发布版本，dis签名
    parser.add_argument('--distribution',required=False,default=False,type=com.str2boolean)
    #编译宏
    parser.add_argument('--macro', required=False,default=[],type=com.str2list)



    parser.add_argument('--isA8Project', required=False,default=True,type=com.str2boolean)
    parser.add_argument('--forceSvnUrl', required=False,default=None,type=com.str2Noneable)

    parser.add_argument('--isMakePSOCache', required=False,default=False,type=com.str2boolean)


    # 打包配置 end------------------------------------

    # 游戏配置 start------------------------------------
    parser.add_argument('--startMode',required=False,default='NotSet')
    parser.add_argument('--jobType',required=False,default='NotSet')
    parser.add_argument('--battleRecord',required=False,default=False,type=com.str2boolean)
    parser.add_argument('--logLevel',required=False,default='NotSet')
    parser.add_argument('--skillNoCD',required=False,default=False,type=com.str2boolean)
    parser.add_argument('--useBattlePreload',required=False,default=False,type=com.str2boolean)
    parser.add_argument('--internalTest',required=False,default=False,type=com.str2boolean)
    

    parser.add_argument('--userBulidPhoneNum',required=False,default='None')
    parser.add_argument('--userBuildComment', required=False,default='',type=com.filterStr)

    # 游戏配置 end------------------------------------

    # args = parser.parse_args(argv)
    args = parser.parse_args()


    if args.platform == 'android':
        builder = UE4AndroidBuilder(args)
    elif args.platform == 'ios':
        builder = UE4IOSBuilder(args)
    elif args.platform == 'windows':
        args.platform = 'Win64'
        builder = UE4WindowsBuilder(args)
    else:
        raise Exception(f'不支持平台{args.platform}')
    builder.build()
    pass