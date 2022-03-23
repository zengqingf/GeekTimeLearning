# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.dirname(__file__)
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..','..')))
from comlib.exception import errorcatch,errorcatch_func,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com


from comlib.dictm import DictUtil, JsonFile

from comlib.conf.ref import *


from comlib.ftpm import FTPManager

import copy

from typing import TypeVar,Type

# https://docs.python.org/zh-cn/3/library/typing.html#module-typing
T = TypeVar('T')      # Declare type variable


settingbuffer = {}

buffer = {}


# 电话映射 = JsonFile(os.path.join(thisdir,'conf','电话.json'))



class Loader(object):
    @staticmethod
    def str2robot(rs):
        from comlib.dingrobot import Dingrobot  
        conf = Loader.load(dingrobotconf,rs,use_defalt=True)
        bot = Dingrobot(conf.webhook,conf.key)
        return bot
    @staticmethod
    def 获取ftp实例()->FTPManager:
        from comlib.secm import SecretManager
        conf = Loader.load(ftpconf,use_defalt=True)
        return FTPManager(conf.host,conf.port,SecretManager.getSecData('ftp','username'),
        SecretManager.getSecData('ftp','password'),conf.httpport,conf.httproot)
    
    @staticmethod
    def 获取登陆信息(ip=None,unityversion=None):
        '''
        return user,password
        '''
        from comlib.comobj import G_ip,G_username
        if ip == None:
            ip = G_ip
        data:dict = Loader.load(envconf,ip)
        
        if data == None:
            raise Exception(f'{ip}未配置打包机信息')
        password = ''
        if unityversion == None:
            # 默认已经登陆
            for k,v in data.items():
                v:envconf
                if v.user == G_username:
                    unityversion = k
                    break
            if unityversion == None:
                raise Exception(f'用户名{G_username}未配置打包机信息配置文件的unity keyname')
        
        password = data[unityversion].password
        if password == '':
            from comlib.secm import SecretManager
            password = SecretManager.getSecData('buildmechine','password')

        return G_username,password

    @staticmethod
    def 获取引擎路径():
        '''
        当前用户名的引擎路径
        '''
        return Loader.getenvconf().enginepath
    @staticmethod
    def 获取相对客户端工程路径(sep=os.path.sep):
        '''
        Program
        '''
        conf = Loader.load(projectconf)
        return com.convertsep2Target(conf.programpath,sep)
    @staticmethod
    def 获取相对客户端游戏工程路径(sep=os.path.sep):
        '''
        Program/Client
        '''
        conf = Loader.load(projectconf)
        return com.convertsep2Target(conf.projectpath,sep)
    @staticmethod
    def 获取主干工程svn地址():
        '''
        .../branches/RELEASE_version_ANDROID_CORE_BANQUAN_2020/Program
        '''
        return com.strjoin('/',Loader.获取主干svn地址(),Loader.获取相对客户端工程路径(sep="/"))
    @staticmethod
    def 获取发布分支工程svn地址():
        return com.strjoin('/',Loader.获取发布分支svn地址(),Loader.获取相对客户端工程路径(sep="/"))
    @staticmethod
    def 获取发布分支游戏工程svn地址():
        return com.strjoin('/',Loader.获取发布分支svn地址(),Loader.获取相对客户端游戏工程路径(sep="/"))
    @staticmethod
    def 根据svnname获取中文名(svnname):
        '''
        找不到返回svnname
        '''
        电话映射:dict = Loader.load(memberconf)
        for 中文名,数据 in 电话映射.items():
            数据:memberconf
            if 数据.svnname == svnname:
                return 中文名
        return svnname
    @staticmethod
    def 根据svnname获取电话(svnname):
        电话映射:dict = Loader.load(memberconf)
        for 中文名,数据 in 电话映射.items():
            数据:memberconf
            if 数据.svnname == svnname:
                return 数据.phone
    @staticmethod
    def 根据中文名获取电话(中文名):
        电话映射:dict = Loader.load(memberconf)
        data = DictUtil.tryGetValue(电话映射,中文名)
        if data != None:
            return data['phone']
        return None
    @staticmethod
    def 获取发包对接群机器人():
        robotconf = Loader.load(dingrobotsendconf,use_defalt=True)
        return Loader.str2robot(robotconf.发包对接群)
    @staticmethod
    def 获取客户端维护机器人():
        robotconf = Loader.load(dingrobotsendconf,use_defalt=True)
        return Loader.str2robot(robotconf.客户端维护)
    @staticmethod
    def 获取脚本调试机器人():
        robotconf = Loader.load(dingrobotsendconf,use_defalt=True)
        return Loader.str2robot(robotconf.脚本调试群)
    @staticmethod
    def 获取出包机器人():
        robotconf = Loader.load(dingrobotsendconf,use_defalt=True)
        return Loader.str2robot(robotconf.出包群)
    @staticmethod
    def isReleaseBranchDesc(branchDesc):
        if 'release' == branchDesc:
            return True
        return False
    # @staticmethod
    # def getsdksvnurl(chinese):
    #     branchurl = Loader.getbranchdesc_chinese2url(chinese)
    #     branchdesconf:branchdesc = Loader.load(branchdesc)
    #     # 当前分支sdk目录url
    #     sdkrooturl = Loader.getsvnurl(branchurl,)
    #     return sdkrooturl
    @staticmethod
    def getsvnurl(rooturl,rel_path):
        return f'{rooturl}/{rel_path}'
    @staticmethod
    def getKeystoreConf(channel,use_defalt=True):
        cconf = Loader.load(channelconf,channel,use_defalt=use_defalt)
        keyconf = Loader.load(android_keystoreconf,cconf.keystorename,use_defalt=True)
        return cconf.keystorename,keyconf
    @staticmethod
    def getenvconf(enginename=None):
        '''
        enginename==None时会自动从globalconf中加载
        '''
        from comlib.comobj import G_ip,G_username
        if enginename == None:
            gconf = Loader.load(globalconf,use_defalt=True)
            enginename = gconf.engine
        # 使用当前用户名的配置
        if com.isNoneOrEmpty(enginename):
            enginename = G_username
        env = Loader.load(envconf,G_ip,enginename)
        return env
    # @errorcatch_func(HIGH) pyinstaller打包后会出现死循环
    @staticmethod
    def _getconf(tp:Type[T],*keys,use_defalt=False) -> T:
        '''
        dict 根据深度转 object
        '''
        
        def unpack(curtp,cur,defaultdata,maxdeepth,curdeepth):
            if maxdeepth == curdeepth:
                combinedata = DictUtil.combine(cur,defaultdata)
                struct = curtp(**combinedata)
                return struct
            else:
                for k,v in cur.items():
                    cur[k] = unpack(curtp,v,defaultdata,maxdeepth,curdeepth + 1)
                return cur
        global settingbuffer,buffer

        def loadprojdata(extfilename):
            confpath = os.path.join(workdir,'projconf',f'{extfilename}.json')
            data = {}
            if not os.path.exists(confpath):
                curthisdir = thisdir
                if com.isPacked() and com.isPackedSingleFile():
                    curthisdir = com.convertMEIPASS2localpath_packedSingleFile(__file__)
                confpath = os.path.join(curthisdir,'..','..','projconf',f'{extfilename}.json')
            if os.path.exists(confpath):
                data = com.loadfile_json(confpath)
            return data
        def loadglobaldata(extfilename):
            curthisdir = thisdir
            if com.isPacked() and com.isPackedSingleFile():
                curthisdir = com.convertMEIPASS2localpath_packedSingleFile(__file__)
            data = com.loadfile_json(os.path.join(curthisdir,'conf',f'{extfilename}.json'))
            return data
        # 初始化
        if settingbuffer == {}:
            import comlib.conf.ref as confref
            settingbuffer = loadglobaldata('mainconf')
            buffer = loadprojdata('mainconf')
            # 合并两个配置
            if buffer != {}:
                buffer['channelconf'] = loadprojdata('channelconf')
                buffer['ioschannelconf'] = loadprojdata('ioschannelconf')

            buffer['envconf'] = loadglobaldata('envconf')
            buffer['dingrobotconf'] = loadglobaldata('dingrobotconf')
            buffer['memberconf'] = loadglobaldata('memberconf')


            for confname,confsetting in settingbuffer.items():
                defaultdata = confsetting['DEFAULT']
                depth = confsetting['DEPTH']
                tt = confref.__dict__[confname]

                # 实例化DEFAULT
                settingbuffer[confname]['DEFAULT'] = confref.__dict__[confname](**defaultdata)
                curtp = confref.__dict__[confname]
                if confname in buffer:
                    # 实例化配置
                    buffer[confname] = unpack(curtp,buffer[confname],defaultdata,depth,1)

        tpname = tp.__name__
        data = DictUtil.tryGetValue(buffer,tpname)
        defaultdata = DictUtil.tryGetValue(settingbuffer,tpname,'DEFAULT')
        if data != None and keys.__len__() != 0:
            data = DictUtil.tryGetValue(data,*keys)
        if data == None:
            if use_defalt:
                data = defaultdata
            else:
                raise StopException(f"配置不存在 tpname={tpname} keys={keys}")

        return data
    @staticmethod
    def load(tp:Type[T],*keys,use_defalt=False) -> T:
        '''
        key大部分是游戏标识
        use_defalt=True的时候key不存在会使用DEFAULT当key
        '''
        data = Loader._getconf(tp,*keys,use_defalt=use_defalt)
        # if keys.__len__() != 0 and isinstance(data,):
        #     data = DictUtil.tryGetValue(data,*keys)
        #     if data == None:
        #         data = defaultdata
        return data

    @staticmethod
    def loadall(tp)->dict:
        pass
        # js = Loader._getconf(tp)

        # return js
    @staticmethod
    def getgame():
        projconf = Loader.load(globalconf)
        return projconf.game
    @staticmethod
    def isdebug():
        projconf = Loader.load(globalconf,use_defalt=True)
        return projconf.debug
    @staticmethod
    def getglobalconf():
        conf = Loader.load(globalconf,use_defalt=True)
        return conf
    
        
    @staticmethod
    def channel2sdk(channel):
        conf:channelconf = Loader.load(channelconf,channel)
        return conf.usesdk
        


if __name__ == "__main__":
    # data1 = Loader.load(channelconf,'mg')
    # data2 = Loader.load(channelconf,'mgggg',use_defalt=True)
    # aaa = Loader.getKeystoreConf('mg',use_defalt=True)
    # aa = Loader.loadall(channelconf)
    print(1)
    # a = Loader.load(bangconf,Loader.getgame())
    # import comlib.conf.ref as confref
    # import inspect
    # print(inspect.getmembers(sys.modules['comlib.conf.ref'], inspect.isclass))
    # print(confref.__dict__)
    pass
