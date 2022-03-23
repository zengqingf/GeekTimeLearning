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


import zlib
from io import BytesIO
from typing import List
from comlib import UEDataHelper,Path

class FCompressedHeader:
    def __init__(self,DirectoryName,FileName,UncompressedSize,FileCount) -> None:
        self.DirectoryName = DirectoryName
        self.FileName = FileName
        self.UncompressedSize = UncompressedSize
        self.FileCount = FileCount
        print(f'DirectoryName name = {DirectoryName} FileName = {FileName} UncompressedSize = {UncompressedSize} FileCount = {FileCount}')

    @classmethod
    def DeSerialized(cls,fs):
        DirectoryName = UEDataHelper.ReadUESerializedData_FString_SerializeAsANSICharArray(fs)
        FileName = UEDataHelper.ReadUESerializedData_FString_SerializeAsANSICharArray(fs)
        UncompressedSize = UEDataHelper.ReadUESerializedData_int32(fs)
        FileCount = UEDataHelper.ReadUESerializedData_int32(fs)
        return cls(DirectoryName,FileName,UncompressedSize,FileCount)

class FCompressedCrashFile:
    def __init__(self,CurrentFileIndex,Filename,Filedata) -> None:
        self.CurrentFileIndex = CurrentFileIndex
        self.Filename = Filename
        self.Filedata = Filedata

        pass
    @classmethod
    def DeSerialized(cls,fs):
        CurrentFileIndex = UEDataHelper.ReadUESerializedData_int32(fs)
        Filename = UEDataHelper.ReadUESerializedData_FString_SerializeAsANSICharArray(fs)
        Filedata = UEDataHelper.ReadUESerializedData_TArrayuint8(fs)

        return cls(CurrentFileIndex,Filename,Filedata)




class PackageApi(Api):
    def __init__(self) -> None:
        super().__init__()



crashSaveRoot = os.path.join(workdir,'crashSave')
def getCrashSaveDir():
    return os.path.join(crashSaveRoot,com.getdatetimenow_full())


@registview
class CrashReporterView(BaseView):
    def __init__(self, app):
        super().__init__(app, 'CrashReporter')
    def register(self):
        super().register()
    
    def apiget(self):

        return super().apiget()
    def apipost(self):
        args = request.args
        AppID = args['AppID']
        AppVersion = args['AppVersion']
        AppEnvironment = args['AppEnvironment']
        UploadType = args['UploadType']
        UserID:str = args['UserID']
        tmp = UserID.split('|')
        LoginId = tmp[0]
        EpicAccountId = tmp[1]
        OperatingSystemId = tmp[2]
        # svn用户名
        who = ''
        if tmp.__len__() > 3:
            who = tmp[3]
        conf = {
            'AppID' : AppID,
            'AppVersion' : AppVersion,
            'AppEnvironment' : AppEnvironment,
            'UploadType' : UploadType,
            'UserID' : UserID,
            'LoginId' : LoginId,
            'EpicAccountId' : EpicAccountId,
            'OperatingSystemId' : OperatingSystemId,
            'who' : who,

        }


        decompressData = zlib.decompress(request.data)

        decompressDataIO = BytesIO(decompressData)
        OptionalHeader = FCompressedHeader.DeSerialized(decompressDataIO)
        saveDir = os.path.join(getCrashSaveDir(),OptionalHeader.DirectoryName)
        Path.ensure_direxsits(saveDir)

        files:List[FCompressedCrashFile] = []
        for i in range(0,OptionalHeader.FileCount):
            f = FCompressedCrashFile.DeSerialized(decompressDataIO)
            fileSavePath = os.path.join(saveDir,f.Filename)
            com.savedata(f.Filedata,fileSavePath,'wb')

            files.append(f)
            
        OptionalFooter = FCompressedHeader.DeSerialized(decompressDataIO)
        data = decompressDataIO.read()
        if data != b'':
            com.logout(f'数据结构不匹配导致data剩余！！！ {com.getdatetimenow_full()}')



        return b''

app = Flask(__name__)
if __name__ == '__main__':
    initview(app)
    app.run(G_ip, 5002,debug=False)
