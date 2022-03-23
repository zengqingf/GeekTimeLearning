# -*- coding:utf-8 -*-

import os,sys,ftplib
sys.path.append(os.path.abspath(os.path.join(__file__,'..','..')))

from comlib.com import logout,buildfolder_tree
from comlib.exception import errorcatch,LOW,NORMAL,HIGH,DingException



@errorcatch(HIGH)
class FTPManager(object):
    
    def __init__(self,host,port,user,password,httpport=80,httproot=''):
        self.ftp = ftplib.FTP()
        # 允许使用中文
        self.ftp.encoding='gbk'

        self.host = host
        self.port = port
        self.httpport = httpport
        self.httproot = httproot

        self.user = user
        self.password = password
        self.isconnect = False

    def ftppath2httppath(self,ftppath:str):
        return f'http://{self.host}:{self.httpport}{ftppath.replace(f"/{self.httproot}","",1)}'

    def ensure_connect(self):
        if self.isconnect == False:
            self.connect()
            self.isconnect = True
    def connect(self):
        try:
            if isinstance(self.port,str):
                self.port = int(self.port)
            self.ftp.connect(host=self.host,port=self.port)
        except Exception as e:
            logout("CONNENT FAIL : %s " % e)
        else:
            try:
                if isinstance(self.password,int):
                    self.password = str(self.password)
                self.ftp.login(user=self.user,passwd=self.password)
            except Exception as e2:
                logout('LOGIN FAIL : %s' % e2)
                raise e2
    def download(self,local,remote,file_filter=None,dir_filter=None):
        self.ensure_connect()
        
        ptype = self.pathtype(remote)
        if ptype == 'file':
            self.downloadfile(local,remote)
        elif ptype == 'dir':
            localfoldername = os.path.join(local,os.path.basename(remote))
            self.mkdir_local(localfoldername)
                
            def dira(self2,path,file_action,dir_action):
                if dir_filter == None:
                    self.mkdir_local(self.ftpsep2local(os.path.join(localfoldername,path.replace(remote+'/','',1))))
                elif dir_filter(path):
                    self.mkdir_local(self.ftpsep2local(os.path.join(localfoldername,path.replace(remote+'/','',1))))
            def filea(self2,path,file_action,dir_action):
                if file_filter == None:
                    self.downloadfile(self.ftpsep2local(os.path.join(localfoldername,path.replace(remote+'/','',1))),path)
                elif file_filter(path):
                    self.downloadfile(self.ftpsep2local(os.path.join(localfoldername,path.replace(remote+'/','',1))),path)
            self.rangefirstsearch(remote,filea,dira)
    def ftpsep2local(self,path):
        return path.replace('/',os.path.sep)
    def downloadfile(self,local,remote):
        self.ensure_connect()

        dirname = os.path.dirname(local)
        if dirname not in ('',None) and not os.path.exists(dirname):
            os.makedirs(dirname)

        f = open(local,'wb')
        try:
            logout("--------------DOWNLOADING : %s--------------"%remote)

            self.ftp.retrbinary('RETR ' + remote, f.write)
        except Exception as e:
            logout(e)
            raise e
        finally:
            f.close()
            logout('FINISH')
        
    def mkdir_local(self,path):
        self.ensure_connect()

        logout('mk '+path)
        if not os.path.exists(path):
            os.makedirs(path)
    def direxsits(self,remote):
        self.ensure_connect()
        
        b = False
        try:
            self.ftp.cwd(remote)
        except ftplib.error_perm:
            b=False
        except Exception as e:
            logout(e)
            raise e
        else:
            b=True
        finally:
            self.ftp.cwd('/')
            return b
    def exists(self,remote):
        dirname = os.path.dirname(remote)
        basename = os.path.basename(remote)
        if not self.direxsits(dirname):
            return False
        
        allfiles = self.listdir(dirname)
        if basename in allfiles:
            return True
        return False
            
    def upload(self,local,remote,showlog=True,overwrite=False):
        self.ensure_connect()

        if not os.path.exists(local):
            logout('%s 不存在'%local)
            return
        if os.path.isfile(local):
            self.uploadfile(local,remote,showlog=showlog,overwrite=overwrite)
        elif os.path.isdir(local):
            pss = os.path.split(local)
            headname = pss[0]
            tailname = pss[1]
            
            # buildfolder_tree(remote,self.direxsits,self.mkd_safe,sep='/')
            self.makedirs(remote)
            
            for dirpath,dirnames,filenames in os.walk(local):
                repsss = dirpath[(headname+os.path.sep).__len__():]
                
                self.mkd_safe(remote+'/'+repsss.replace(os.path.sep,'/'))
                for d in dirnames:
                    self.mkd_safe(remote+'/'+repsss.replace(os.path.sep,'/') + '/'+d)
                for f in filenames:
                    self.uploadfile(os.path.join(dirpath,f),remote+'/'+repsss.replace(os.path.sep,'/')+'/'+f,showlog=showlog,overwrite=overwrite)
    def delete(self,remote,showlog=True):
        self.ensure_connect()
        if showlog:
            logout(f'[ftp] delete => {remote}')
        self.ftp.delete(remote)
    
    def deletedir(self,remote,showlog=True):
        self.ensure_connect()
        try:
            dir_res = []
            try:
                self.ftp.cwd(remote)
            except Exception as e:
                logout('进入ftp目录失败')
                logout(e)
            self.ftp.dir('.',dir_res.append)
            for i in dir_res:
                if i.startswith('d'):
                    dirName = i.split(" ")[-1]
                    self.deletedir(remote+'/'+dirName)
                    self.ftp.cwd('..')
                    if showlog:
                        logout(f'[ftp] delete dir => {dirName}')
                    self.ftp.rmd(remote+'/'+dirName)
                else:
                    filelist = self.ftp.nlst(remote)
                    for f in filelist:
                        self.delete(f)
        except Exception as e:
            logout('delete 目录失败')
            logout(e)

    def uploadfile(self,local,remote,showlog=True,overwrite=False):
        self.ensure_connect()

        f = open(local, "rb")
        try:
            self.makedirs(os.path.split(remote)[0])
            if overwrite and self.exists(remote):
                self.delete(remote,showlog=showlog)
            if showlog:
                logout("UPLOAD : %s 到 %s"%(local,remote))
            self.ftp.storbinary("STOR "+remote, f)
        except Exception as e:
            logout('上传失败')
            logout(e)
        finally:
            f.close()
            if showlog:
                logout('FINISH')
        
    def mkd_safe(self,ftppath):
        self.ensure_connect()

        try:
            self.ftp.cwd(ftppath)
        except ftplib.error_perm:
            try:
                self.ftp.mkd(ftppath)
            except ftplib.error_perm:
                logout("Change directory failed!")
        finally:
            self.ftp.cwd('/')
    def makedirs(self,ftppath):
        buildfolder_tree(ftppath,self.direxsits,self.mkd_safe,sep='/')

    def pathtype(self,path):
        self.ensure_connect()

        l = self.getcontentfullnames(path)
        if l.__len__() == 1 and l[0] == path:
            return 'file'
        else:
            return 'dir'
    def close(self):
        self.isconnect = False

        self.ftp.close()
    def getcontentfullinfos(self,remote):
        self.ensure_connect()

        ftp = self.ftp
        return ftp.mlsd(remote)
    def getcontentfullnames(self,remote):
        '''
        ftp.nlst
        '''
        self.ensure_connect()

        ftp = self.ftp
        return ftp.nlst(remote) 
    def listdir(self,remote):
        return list(map(lambda x: os.path.basename(x),self.getcontentfullnames(remote)))
    def getcontentbasenames(self,remote):
        return map(lambda x: x.replace(remote+'/',''),self.getcontentfullnames(remote))
    def deepfirstsearch(self,path,file_action,dir_action=None):
        fl = self.getcontentfullinfos(path)

        for nfl in fl:
            if nfl[1]['type'] == 'dir':
                if dir_action != None:
                    dir_action(self,path + '/' + nfl[0],file_action,dir_action)
                self.deepfirstsearch(path + '/' + nfl[0],file_action,dir_action)
            elif nfl[1]['type'] == 'file':
                if file_action != None:
                    file_action(self,path + '/' + nfl[0],file_action,file_action)

    def rangefirstsearch(self,path,file_action,dir_action):
        folders = []
        def se(path,file_action,dir_action):
            fl = self.getcontentfullinfos(path)
            for nfl in fl:
                if nfl[1]['type'] == 'dir':
                    if dir_action != None:
                        dir_action(self,path + '/' + nfl[0],file_action,dir_action)
                    folders.append(path + '/' + nfl[0])
                elif nfl[1]['type'] == 'file':
                    if file_action != None:
                        file_action(self,path + '/' + nfl[0],file_action,file_action)
        
        se(path,file_action,dir_action)
        while folders.__len__() != 0:
            self.rangefirstsearch(folders.pop(0),file_action,dir_action)

    def searchfomat(self):
        pass



# ftp147 = FTPManager('192.168.2.147',21,'ftp','123456')