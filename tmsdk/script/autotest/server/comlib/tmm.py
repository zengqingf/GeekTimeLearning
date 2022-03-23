# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com

from comlib.dictm import JsonFile
from comlib.svnm import SVNManager


class Version(object):
    def __init__(self,client=0,server=0,clientCode=1,serverCode=1):
        self.client = int(client)
        self.server = int(server)

        self.serverCode = int(serverCode)
        self.clientCode = int(clientCode)
    def __str__(self):
        return f'{self.serverCode}.{self.server}.{self.clientCode}.{self.client}'
        
def loadversion_str(versionstr):
    tmp = versionstr.split('.')
    client = 0
    clientCode = 1
    server = 0
    serverCode = 1
    if tmp.__len__() == 4:
        client = tmp[3]
        clientCode = tmp[2]
        server = tmp[1]
        serverCode = tmp[0]
    elif tmp.__len__() == 3:
        server = tmp[0]
        clientCode = tmp[1]
        client = tmp[2]
    elif tmp.__len__() ==2:
        server = tmp[0]
        client = tmp[1]
    elif tmp.__len__() == 1:
        client = tmp[0]
    return Version(client,server,clientCode=clientCode,serverCode=serverCode)
def loadversion(client=0,server=0):
    return Version(client,server)

from enum import Enum,unique
from comlib.pathm import Path
@unique
class PackType(Enum):
    fullpack = 0
    hotfixpack = 1

class RecordStruct():
    def __init__(self,serverversion,clientversion,verifymd5,versioncode,type,download=None) -> None:
        # self.serverversion = serverversion
        # self.clienversion = clienversion
        self.version = Version(clientversion,serverversion).__str__()
        self.verifymd5 = verifymd5
        # versioncode 根据 getnextversioncode 赋值
        self.versioncode = versioncode
        self.type = type
        self.download = download


recordfilepath_default = os.path.join(workdir,'projconf','record.json')

class RecordTool():
    def __init__(self,recordfilepath=None) -> None:
        if recordfilepath == None:
            recordfilepath = recordfilepath_default
        Path.ensure_pathexsits_nocopy(recordfilepath)
        self.recordfilepath = recordfilepath
        self.record = JsonFile(self.recordfilepath)
    def commit(self,msg):
        SVNManager.commit(self.recordfilepath,msg)
    def addplat(self,plat):
        content = {
            'serverversion':0
        }
        self.record.setvalue(plat,value=content)
    def addchannel(self,plat,channelname):
        self.ensure_platexists(plat)
        versioncode = self.getversioncode_max(plat)
        content = {
            "online": None,
            "fullpack": None,
            "hotfixpack": None,
            
            "prepublish": {
                "versioncode": versioncode,
                "fullpack": None,
                "hotfixpack": None
            },
            "history": []
        }
        self.record.setvalue(plat,channelname,value=content)
        # if not self.record.haskey(plat,'serverversion'):
        #     self.record.setvalue(plat,'serverversion',value=0)
    def getallchannel(self,plat):
        channels = []
        for channelname,channeldata in self.record.trygetvalue(plat).items():
            if channelname == 'serverversion':
                continue
            channels.append(channelname)
        return channels
    def getallplat(self):
        plats = []
        for plat in self.record.getkeys():
            plats.append(plat)
        return plats
    def getversioncode(self,plat,channel):
        self.ensure_channelexists(plat,channel)
        return self.record.trygetvalue(plat,channel,'prepublish','versioncode')

    def getversioncode_max(self,plat):
        '''
        获取指定平台（android，ios，iosother）最大versioncode
        '''
        channelsdata = self.record.trygetvalue(plat)
        maxversioncode = '1' # 最低是1，0会报错
        if plat == 'ios':
            maxversioncode = '1'
        if channelsdata == None:
            return maxversioncode
        for channelname,channeldata in channelsdata.items():
            if channelname == 'serverversion':
                continue
            cur = channeldata['prepublish']['versioncode']
            if com.dotstrcompare(cur,maxversioncode):
                maxversioncode = cur
        return maxversioncode
    def getnextversioncode(self,plat):
        maxcode = self.getversioncode_max(plat)
        if plat == 'ios':
            # 1.2.3 -> 1.2.4
            # 根据点切割，给最后一位加1，然后再重新组合
            tmp = maxcode.split('.')
            relcode = int(tmp[-1]) + 1
            nextcode = f"{'.'.join(tmp[:tmp.__len__() - 1])}.{relcode}"
        else:
            nextcode = int(maxcode) + 1
        return nextcode.__str__()
    def setversioncode(self,plat,channelname,versioncode):
        self.record.trysetvalue(plat,channelname,'prepublish','versioncode',value=versioncode)
        self.record.save(True)
    def getserverversion(self,plat):
        self.ensure_platexists(plat)
        
        return self.record.trygetvalue(plat,'serverversion')


    def setnextserverversion(self,plat):
        self.ensure_platexists(plat)

        cur = self.record.trygetvalue(plat,'serverversion')
        if cur == None:
            return
        new = cur + 1
        self.record.trysetvalue(plat,'serverversion',value=new)
        self.record.save(True,msg=f'{plat}服务器版本号更新')

    def ensure_channelexists(self,plat,channelname):
        if not self.record.haskey(plat,channelname):
            self.addchannel(plat,channelname)
    def ensure_platexists(self,plat):
        if not self.record.haskey(plat):
            self.addplat(plat)
    def prepublish(self,plat,channelname,data:RecordStruct):
        self.ensure_channelexists(plat,channelname)
        self.record.setvalue(plat,channelname,'prepublish',data.type,value=data.__dict__)

        if data.versioncode != self.record.jsondata[plat][channelname]['prepublish']['versioncode']:
            self.setversioncode(plat,channelname,data.versioncode)

        self.record.save(True,msg=f'{plat}-{channelname}-{data.version}-{data.versioncode}版本预发布')
    def publish(self,plat,channelname,type):
        self.ensure_channelexists(plat,channelname)
        data = self.record.trygetvalue(plat,channelname,'prepublish',type)
        self.record.setvalue(plat,channelname,'prepublish',type,value=None)
        olddata = self.record.trygetvalue(plat,channelname,type)
        self.record.setvalue(plat,channelname,type,value=data)
        self.record.setvalue(plat,channelname,'online',value=data)
        if olddata != None:
            history:list = self.record.jsondata[plat][channelname]['history']
            history.append(olddata)
            com.sort(history,lambda x,y:not com.dotstrcompare(x['version'],y['version']))
        self.record.save(True,msg=f"{plat}-{channelname}-{data['version']}-{data['versioncode']}版本发布")
    def prepublish_easy(self,plat,channel,clientversion,verifymd5,type,download=None):
        serverversion = self.getserverversion(plat)
        versioncode = self.getversioncode(plat,channel)

        self.prepublish(plat,channel,RecordStruct(serverversion,clientversion,verifymd5,versioncode,type,download))
def test():
    from comlib.pathm import Path
    Path.ensure_pathnotexsits('record.json')
    tool = RecordTool('record.json')


    tool.setnextserverversion('android')
    tool.prepublish('android','mg',RecordStruct(tool.getserverversion('android'),'3','md51',tool.getnextversioncode('android'),PackType.fullpack.name))
    tool.publish('android','mg',PackType.fullpack.name)

    tool.setnextserverversion('android')
    tool.prepublish('android','mg',RecordStruct(tool.getserverversion('android'),'2','md52',tool.getnextversioncode('android'),PackType.fullpack.name))
    tool.publish('android','mg',PackType.fullpack.name)

    tool.setnextserverversion('android')
    tool.prepublish('android','mg',RecordStruct(tool.getserverversion('android'),'1','md52',tool.getnextversioncode('android'),PackType.fullpack.name))
    tool.publish('android','mg',PackType.fullpack.name)
    
    tool.setnextserverversion('ios')
    tool.prepublish('ios','appstore',RecordStruct(tool.getserverversion('ios'),'1','md53',tool.getnextversioncode('ios'),PackType.fullpack.name))
    tool.publish('ios','appstore',PackType.fullpack.name)
    
    tool.setnextserverversion('ios')
    tool.prepublish('ios','appstore',RecordStruct(tool.getserverversion('ios'),'13','md53',tool.getnextversioncode('ios'),PackType.fullpack.name))
    tool.publish('ios','appstore',PackType.fullpack.name)
    

    print(tool.getnextversioncode('android'))
    # tool.getnextversioncode('android')
    print(tool.getnextversioncode('ios'))
if __name__ == "__main__":
    test()