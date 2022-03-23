# -*- coding=utf-8 -*-


import os,sys
import socket
import json
import time
import threading
import re
import zipfile
from datetime import datetime
import shutil
sys.path.append(os.path.abspath(os.path.join(__file__,'..','..')))

import subprocess

from comlib.exception import errorcatch,LOW,NORMAL,HIGH

'''
已废弃
'''


def buildfolder_tree(leafpath,exfunc,mdfunc):
    q = []
    pt = leafpath
    
    while not exfunc(pt):
        q.append(pt)
        if pt.find(os.path.sep) != -1:
            pt = pt.replace(os.path.sep+os.path.basename(pt),'')
        else:
            pt = '.'
            break
    while q.__len__() != 0:
        mdfunc(q.pop())
    return leafpath

@errorcatch(HIGH)
class wait(object):
    index = 0
    isbytes = False
    
    def __init__(self,waits,overcount=5000):
        self.waits = waits
        if isinstance(waits,bytes):
            self.isbytes = True
        self.overcount = overcount

    def waitfor(self,cur):
        if self.isbytes:
            cur = cur[0]
        self.overcount -= 1
        if self.overcount <= 0:
            return True
        if self.waits[self.index] == cur:
            self.index += 1
            if self.index == self.waits.__len__():
                return True
        elif self.waits[self.index] == b'.'[0]:
            if self.waits[self.index+1] == cur:
                self.index += 2
                if self.index == self.waits.__len__():
                    return True
            return False
        else:
            self.index = 0
        return False

@errorcatch(HIGH)
class httphelper(object):
    req_get_format = 'GET {} HTTP/1.1\r\nHost: {}\r\nConnection:{}\r\n\r\n'#\r\nConnection:keep-alive

    headerend = b'\r\n\r\n'
    headersplit = b'\r\n'
    packetsplit = b'\r\n.\r\n'#.代表任意长度字符
    @property
    def __headerend(self):
        return httphelper.req_get_format
    @property
    def __headerend(self):
        return httphelper.headerend
    @property
    def __headersplit(self):
        return httphelper.headersplit   
    @property
    def __packetsplit(self):
        return httphelper.packetsplit   
    @staticmethod
    def req_get_build(path,host,iskeepalive=False):
        if iskeepalive:
            req = httphelper.req_get_format.format(path,host,'keep-alive').encode('utf-8')    
        else:
            req = httphelper.req_get_format.format(path,host,'close').encode('utf-8')  
        return req
    @staticmethod
    def getdata(waits,sk,withwaits=True,timeout=5):
        w = wait(waits,1000)
        data=b''
        try:
            while True:
                
                t = sockethelper.recvtimer(sk,1,timeout)
                if t == b'':#超时暂时这么写
                    return b''
                data += t
                if w.waitfor(t):
                    break
            if not withwaits:
                return data.replace(waits,b'')
        except Exception as e:
            print(e)
            return b''
        return data
    @staticmethod
    def getheader(sk):
        header = httphelper.getdata(httphelper.headerend,sk)
        # print("头部字节数%s"%header.__len__())
        return header
    @staticmethod
    def getcontent_length(header):
        m = re.search('Content-Length:(.*?)\r\n',header,re.M|re.S)
        if m != None:
            return int(m.group(1).strip())
        return None # chunk模式
    @staticmethod
    def getfirstpacketsize(sk,encoding='utf-8'):
        packetsize = httphelper.getdata(httphelper.headersplit,sk)
        cleardata = packetsize.strip()
        if cleardata in (b'',b'0'):#数据包结束标识符
            end = sk.recv(2,socket.MSG_WAITALL)#接受结束符
            return 0
        size = cleardata.decode(encoding='utf-8')
        return eval('0x'+size)

    @staticmethod
    def getpacketsize(sk,encoding='utf-8'):
        packetsize = httphelper.getdata(httphelper.packetsplit,sk)
        cleardata = packetsize.strip()
        if cleardata in (b'',b'0'):#数据包结束标识符
            end = sk.recv(2,socket.MSG_WAITALL)#接受结束符
            return 0
        size = cleardata.decode(encoding='utf-8')

        return eval('0x'+size)
        
@errorcatch(HIGH)
class sockethelper(object):
    msgend = b'\r\n'
    @property
    def __msgend(self):
        return sockethelper.msgend   
    @staticmethod
    def sendmsg(sk,msg):
        if isinstance(msg,str):
            msgb = msg.encode(encoding='utf-8') + b'\r\n\r\n'
        elif isinstance(msg,list):
            msgb = b''
            for m in msg:
                msgb += m.encode(encoding='utf-8') + b'\r\n'
            msgb += b'\r\n'
        elif isinstance(msg,bytes):
            msgb = msg + b'\r\n\r\n'
        # elif isinstance(msg,unicode):
        #     msgb = msg.encode(encoding='utf-8') + b'\r\n\r\n'
        else:
            print('unsuppot instance')
            return
        try:
            if sk != None:
                sk.send(msgb)
                print('发送成功 %s'%msgb)
        except Exception as e:
            print('发送失败')
            print(e)
    @staticmethod
    def recvmsg(sk,withwaits=False,timeout=5):
        msg = []
        
        m = httphelper.getdata(httphelper.headersplit,sk,withwaits=withwaits,timeout=timeout)
        if not withwaits:
            while m != b'':
                msg.append(m.decode(encoding='utf-8'))
                m = httphelper.getdata(httphelper.headersplit,sk,withwaits=withwaits,timeout=timeout)
        else:
            while m != httphelper.headersplit:
                msg.append(m.decode(encoding='utf-8'))
                m = httphelper.getdata(httphelper.headersplit,sk,withwaits=withwaits,timeout=timeout)
        if msg.__len__() == 0:
            return ''
        elif msg.__len__() == 1:
            # print("接收%s"%msg[0])
            return msg[0]
        else:
            print("接收")
            print(msg)
            return msg
    @staticmethod
    def recvfile(sk,path):
        if os.path.exists(path):
            os.remove(path)

        sizeb = httphelper.getdata(httphelper.headersplit,sk,withwaits=False,timeout=None)
        print(sizeb)
        size = int(sizeb.decode(encoding='utf-8'))
        print("接收文件大小%s"%size)
        f = open(path,'wb')
        chunk = b''
        while size > 0 :
            if size < 1024:
                chunk = sockethelper.recvtimer(sk,size)
            else:
                chunk = sockethelper.recvtimer(sk,1024)
            size -= chunk.__len__()
            f.write(chunk)
        
        f.close()
        
    @staticmethod
    def sendfile(sk,path,block=True):
        if block:
            sockethelper._sendfile(sk,path,block)
        else:
            thd = threading.Thread(target=sockethelper._sendfile,args=(sk,path,block))
            return thd
    @staticmethod
    def _sendfile(sk,path,block):
        
        size = os.path.getsize(path)
        print("传输文件大小%s"%size)
        f = open(path,'rb')
        
        sk.send(str(size).encode('utf-8')+b'\r\n')
        if hasattr(sk,'sendfile') and not block:
            sk.sendfile(f)
        else:
            b = f.read(1024*8)
            while b.__len__() != 0:
                sk.sendall(b)
                b = f.read(1024*8)

        f.close()
    @staticmethod
    def recvtimer(sk,length,timeout=None):
        data = b''
        if timeout == None:
            while data.__len__() < length:
                try:
                    tmp = sk.recv(length)
                    data += tmp
                except:
                    pass
            return data
        t = timeout
        while t > 0:
            start = time.time()
            try:
                tmp = sk.recv(length)
                if tmp.__len__() > 0:
                    data += tmp
                    t = timeout
                    if data.__len__() == length:
                        return data
            except:
                pass
            finally:
                end = time.time()
                t -= end - start
        print("连接超时")
        return b''
    @staticmethod
    def waitallthd(thds):
        if thds != None:
            for t in thds:
                if t != None:
                    t.join()


class slave(object):
    def __init__(self,game,platform,branch,ver,buildname):
        self.game = game
        self.platform = platform
        self.branch = branch
        self.ver = ver
        self.buildname = buildname
        

    def connectmaster(self):
        try:
            sk = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
            sk.connect(('192.168.2.75',5000))
        except Exception as e:
            print(e)
        return sk
        
    def hello(self):
        sk = self.connectmaster()
        sockethelper.sendmsg(sk,'{"func":"hello","param":{"game":"%s","platform":"%s","branch":"%s","ver":"%s","buildname":"%s"}}'%(self.game,self.platform,self.branch,self.ver,self.buildname))

    def saveab(self,abp):
        s = self.connectmaster()

        sockethelper.sendmsg(s,'{"func":"saveab","param":{"game":"%s","platform":"%s","branch":"%s","ver":"%s","buildname":"%s"}}'%(self.game,self.platform,self.branch,self.ver,self.buildname))
        js = sockethelper.recvmsg(s)
        if js == 'nobuilding':
            return
        try:
            reqlist = json.loads(js)
        except Exception as e:
            print('存ab json出错了')
            print(js)
            print(e)

            
        thds = []
        isquit = False
        aplock = threading.Lock()
        tmp = {'reqlist':reqlist,'isquit':isquit,'aplock':aplock}
        def recvlink():
            while not tmp['isquit']:
                print("等待消息")
                js = sockethelper.recvmsg(s)
                if js == 'end':
                    print('结束')
                    return
                tmp['aplock'].acquire()
                if js not in (None,''):
                    tmp['reqlist'].append(json.loads(js))
                tmp['aplock'].release()
                print("loop   %s"%tmp['isquit'])

        t = threading.Thread(target=recvlink)
        t.setDaemon(True)
        t.start()
        thds.append(t)
        def getreq(reqlist,aplock):
            aplock.acquire()
            i = reqlist.pop(0)
            aplock.release()
            return i
        while reqlist.__len__() != 0:
            req = getreq(reqlist,aplock)
            tcpsocket = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
            tcpsocket.connect(tuple(req))
            t = sockethelper.sendfile(tcpsocket,abp,block=True)
            if t != None:
                t.setDaemon(True)
                thds.append(t)
            
        tmp['isquit'] = True
        thds.append(sockethelper.sendfile(s,abp))
        print("end")
        return thds

    def buildab(self,mode):
        sk = self.connectmaster()
        sockethelper.sendmsg(sk,'{"func":"buildab","param":{"game":"%s","platform":"%s","branch":"%s","ver":"%s","mode":"%s"}}'%(self.game,self.platform,self.branch,self.ver,mode))
        msg = sockethelper.recvmsg(sk)
        print(msg)
        
    def buildcode(self):
        sk = self.connectmaster()
        sockethelper.sendmsg(sk,'{"func":"buildcode","param":{"game":"%s","platform":"%s","branch":"%s","ver":"%s"}}'%(self.game,self.platform,self.branch,self.ver))
        msg = sockethelper.recvmsg(sk)
        print(msg)
        
    def getab(self,mode,abp,block=False):

        if block:
            self._getab(mode,abp)
        else:
            t = threading.Thread(target=self._getab,args=(mode,abp))
            t.setDaemon(True)
            t.start()
            return t
            
    def _getab(self,mode,abp):
        def trybind(sk,host,portstart,count):
            try:
                sk.bind((host,portstart))
                return portstart
            except Exception as e:
                count -= 1
                portstart += 1
                return trybind(sk,host,portstart,count)

        sk = self.connectmaster()

        host = socket.gethostbyname(socket.gethostname())
        ss = socket.socket()
        port = trybind(ss,host,6000,100)
        print('绑定端口 %s'%port)
        sockethelper.sendmsg(sk,'{"func":"getab","param":{"game":"%s","platform":"%s","branch":"%s","ver":"%s","mode":"%s","host":"%s","port":"%s","buildname":"%s"}}'%(self.game,self.platform,self.branch,self.ver,mode,host,port,self.buildname))
        msg = sockethelper.recvmsg(sk)
        print(msg)
        try:
            if msg == 'fail':
                sk.close()
                ss.close()
                raise Exception('!!!FAIL TO GET ASSETSBUNDLE FROM CENTER SERVER!!!')
            elif msg == 'ok':
                print('从仓库拿ab %s' %self.ver)
                ss.close()
                sockethelper.recvfile(sk,abp)
            else:
                ss.listen(1)
                print('开始监听')
                client,addr = ss.accept()
                print('接收文件')
                sockethelper.recvfile(client,abp)
        except Exception as e:
            raise e
        finally:
            sk.close()
            ss.close()
    @staticmethod
    def createzip(paths,zipname):
        
        z = zipfile.ZipFile(zipname,mode='w')
        for f in paths:
            z.write(f,os.path.split(f)[1])
        z.close()
    @staticmethod
    def listdir_full(dirp):
        return map(lambda p: os.path.join(dirp,p), os.listdir(dirp))
    @staticmethod
    def unzip(frm,to):
        zip_file = zipfile.ZipFile(frm)
        if os.path.exists(to):
            shutil.rmtree(to)
            os.mkdir(to)
        else:
            os.mkdir(to)
        for names in zip_file.namelist():
            zip_file.extract(names,to)
        zip_file.close()
        
    def buildbb(self):
        sk = self.connectmaster()
        sockethelper.sendmsg(sk,'{"func":"buildbb","param":{"game":"%s","platform":"%s","branch":"%s","ver":"%s"}}'%(self.game,self.platform,self.branch,self.ver))
        scpcmd = sockethelper.recvmsg(sk)
        print(scpcmd)
        return scpcmd



