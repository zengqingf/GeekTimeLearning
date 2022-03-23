# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.dirname(__file__)
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH

from comlib import com
from comlib.dictm import DictUtil
from comlib.binm import BinManager
# import baidupcsapi
from baidupcsapi import PCS

from comlib.conf.loader import Loader


import io

class Baidu():
    def __init__(self):
        from comlib.conf.ref import baiduacc
        self.accconf = Loader.load(baiduacc)
        self.bdapi = PCS(self.accconf.baidu_bduss,self.accconf.baidu_stoken)

    def share(self,file_ids:list):
        import random,string
        password = "".join(random.sample(string.ascii_letters + string.digits, 4))
        ret = self.bdapi.share(file_ids,pwd=password)
        ret_json = ret.json()
        link = ret_json['link']
        return link,password
    def mkdir(self,remotedir):
        '''
        目录存在时会新建一个名为 目标目录名_20200814_174713 的目录\n
        允许多重目录建立\n
        return file_id,path \n
        req ret {'fs_id': 661954438774102, 'path': '/test_20200814_174713', 'ctime': 1597398433, 'mtime': 1597398433, 'status': 0, 'isdir': 1, 'errno': 0, 'name': '/test_20200814_174713', 'category': 6}
        '''
        ret = self.bdapi.mkdir(remotedir)
        ret_json = ret.json()
        file_id = ret_json['fs_id']
        path = ret_json['path']
        return file_id,path

    def upload_normalfile(self,localfile,remotefile):
        com.logout(f'[upload] {localfile} => {remotefile}')
        filename = os.path.basename(remotefile)
        dirpath = os.path.dirname(remotefile)
        with open(localfile,'rb') as fs:
            ret = self.bdapi.upload(dirpath,fs,filename)
            print(ret.json())
    def upload_superfile(self,localfile,remotefile):
        '''
        普通用户分片大小4mb 最多分片1024个
        会员分片10mb 最多分片1024个
        超级会员分片20mb 最多分片1024个
        '''
        com.logout(f'[upload superfile] {localfile} => {remotefile}')
        size = 20 * 1024 * 1024
        md5list = []
        with open(localfile,'rb') as fs:
            chunk = fs.read(size)
            while chunk.__len__() != 0:
                filelike = io.BytesIO(chunk)
                ret = self.bdapi.upload_tmpfile(filelike)
                md5 = ret.json()['md5']
                md5list.append(md5)
                chunk = fs.read(size)
                
            ret = self.bdapi.upload_superfile(remotefile,md5list)
            ret_json = ret.json()
            if DictUtil.hasKey(ret_json,'block_list'):
                ret_json.pop('block_list')
            print(ret_json)
                

    def list_files(self,path):
        '''
        "list": [
            {
                "server_mtime": 1514415738,
                "privacy": 0,
                "category": 6,
                "unlist": 0,
                "isdir": 1,
                "server_atime": 0,
                "server_ctime": 1514415738,
                "wpfile": 0,
                "local_mtime": 1514415738,
                "size": 0,
                "share": 0,
                "server_filename": "游戏更新",
                "path": "/游戏更新",
                "local_ctime": 1514415738,
                "oper_id": 1169885470,
                "fs_id": 578709214009912
            }
        '''
        ret = self.bdapi.list_files(path)
        import json
        tmp = json.loads(ret.content.decode(encoding='utf-8'))
        com.dumpfile_json(tmp,'list.json')

# class Baidu(object):
#     def __init__(self):
#         self.bduss = bduss
#         self.ptoken = ptoken
#         self.stoken = stoken
#         self.haslogin = False

#     def checklogin(self):
#         if self.haslogin:
#             return
#         out,code = BinManager.baidupcs_go('who')
        
#         if 'uid: 0' in out:
#             self.login()
#             self.haslogin = True

#     def login(self):
#         BinManager.baidupcs_go(f'login --bduss {self.bduss} --ptoken {self.ptoken} --stoken {self.stoken}',getstdout=False,errException=StopException('百度盘登陆失败',locals(),''))

#         # ZpLWxJZks2MEhmT0psQ241NGtvYWNrejZsMHJ4dTdvMzM4UGU3b253VTZXQzFmSVFBQUFBJCQAAAAAAAAAAAEAAACoIz8mZWFydGhzODY2AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADrLBV86ywVfRj
#     def upload(self,frompath:list,topath,parallelCount=0,retryCount=3,miaochuan=True):
#         self.checklogin()
#         if not miaochuan:
#             miaochuan = '--norapid'
#         else:
#             miaochuan = ''
#         # 小于2G文件在最快pcs服务器下不使用分片能快2倍以上速度
#         splitopt = '--nosplit'
#         for path in frompath:
#             if os.path.getsize(path) >= 1024 * 1024 * 1024 * 2: # 2G文件
#                 splitopt = ''
            
#         fromstr = ' '.join(frompath)
#         BinManager.baidupcs_go(f'upload -p {parallelCount} --retry {retryCount} {miaochuan} {splitopt} {fromstr} {topath}'
#         ,getstdout=False,errException=StopException('百度盘上传失败',locals(),''))
