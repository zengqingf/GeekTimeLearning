# -*- coding: utf-8 -*-
'''
this script running with paramiko
'''


import os
import sys
sys.path.append(os.path.abspath(os.path.join(__file__,'..','..')))




import socket,json
import paramiko
import traceback

from ftplib import FTP

from paramiko import SSHException, AuthenticationException
from comlib import com

# from com.Log import Logging
from comlib import Logging
from comlib.exception import errorcatch,LOW,NORMAL,HIGH

@errorcatch(HIGH)
class SSHConnector(object):
    def __init__(self, host=None, port=None, username=None, password=None, key_filename=None,
                 known_hosts_file_path=None):
        self.host = host
        self.port = port
        self.username = username
        self.password = password
        self.key_filename = key_filename
        self.knownhost = known_hosts_file_path
        self.hostkeys = self._load_host_keys()
        
        self.sshclient = self._connect()
        if self.sshclient:
            self.tranport = self.sshclient.get_transport()
            self.session = self.tranport.open_session()
        #self.sftpclient = self._init_sftp_client()

    def _connect(self):
        try:
            client = paramiko.SSHClient()
            client.set_missing_host_key_policy(paramiko.AutoAddPolicy())
            if self.key_filename:
                if check_maxos_enviroment:
                    client.connect(hostname=self.host, username=self.username, key_filename=self.key_filename)
                else:
                    client.connect(hostname=self.host, port=self.port, username=self.username,
                                   password=self.password, key_filename=self.key_filename)
            else:
                client.connect(hostname=self.host, port=self.port, username=self.username,
                               password=self.password)
        except AuthenticationException as e:
            Logging.error(e)
        except SSHException as e:
            Logging.error(e)
        return client


    def _load_host_keys(self):
        host_keys = {}
        if self.knownhost:
            try:
                host_keys = paramiko.util.load_host_keys(self.knownhost)
            except IOError as e:
                try:
                    host_keys = paramiko.util.load_host_keys(self.knownhost)
                except IOError as e:
                    Logging.error("Unable to open host keys file:%s" % self.knownhost)
        return host_keys

    def _init_sftp_client(self):
        if self.sshclient:
            return self.sshclient.open_sftp()
        if self.host in self.host:
            hostkeytype = self.hostkeys[self.host].keys()[0]
            hostkey = self.hostkeys[self.host][hostkeytype]
            hostfqdn = socket.getfqdn(self.host)
            Logging.info('Using host key of type %s' % hostkeytype)
            try:
                t = paramiko.Transport((self.host, self.port))
                t.connect(hostkey=hostkey, username=self.username, password=self.password,
                gss_host=hostfqdn, gss_auth=True, gss_kex=True)
                sftp = paramiko.SFTPClient.from_transport(t, max_packet_size=1000 * 1000 * 10)
                return sftp
            except SSHException as e:
                traceback.print_exc()
                t.close()
                sys.exit(1)

    def execute_command(self, command="ls -l", timeout=3000):
        stdin, stdout, stderr = self.session.exec_command(command)
        if stderr:
            raise SSHException(stderr)
        self.sshclient.close()
        return stdout

    def _sftp_operation(self, remotefile=None, localfile=None, op=None):
        """do some sftp operations like 'get', 'put', 'read', 'write' """
        sftp = self.sshclient.open_sftp()
        if op == "get":
            Logging.info("[get] %s-->%s" % (remotefile, localfile))
            sftp.get(remotefile, localfile)
        elif op == "put":
            Logging.info("[put] %s-->%s" % (localfile, remotefile))
            sftp.put(localfile, remotefile)
        elif op == "mkdir":
            Logging.info("[mkdir] %s" % remotefile)
            sftp.mkdir(remotefile)
        elif op == "listdir":
            Logging.info("[listdir] %s" % remotefile)
            return sftp.listdir(remotefile)
        sftp.close()

    def sftp_get(self, remotefile=None, destfile=None):
        """upload file to remote server"""
        self._sftp_operation(remotefile, destfile, "get")

    def sftp_put(self, localfile=None, remotefile=None):
        """download file from remote server"""
        self._sftp_operation(remotefile, localfile, "put")

    def put_tree(self, localfile=None, remotefile=None):
        names = os.listdir(localfile)

        errors = []

        for name in names:
            if name.startswith("."):
                continue

            srcname = os.path.join(localfile, name)
            dstname = os.path.join(remotefile, name)
            try:
                if os.path.isdir(srcname):
                    self.mk_dir(localfile=srcname,remotefile=remotefile)
                    self.put_tree(srcname, dstname)
                else:
                    self._sftp_operation(remotefile=dstname, localfile=srcname, op="put")
            except (IOError, os.error) as why:
                errors.append((srcname, dstname, str(why)))

        if errors:
            Logging.error(errors)

    def mk_dir(self,localfile=None, remotefile=None):
        remotelist = self._sftp_operation(remotefile=remotefile, op="listdir")
        filename = self.splitpath(localpath=localfile)[1]
        for dir in remotelist:
            if filename == dir:
                return os.path.join(remotefile, filename)
        self._sftp_operation(remotefile=os.path.join(remotefile, filename), op="mkdir")
        return os.path.join(remotefile, filename)

    def splitpath(self, localpath):
        position = localpath.rfind('/')
        return (localpath[:position + 1], localpath[position + 1:])

def check_maxos_enviroment():
    if "darwin" != sys.platform:
        return False
    return True

def __test_ftp_upload():
    sftp = SSHConnector(host="47.96.5.112", username="root",
                        key_filename="/work/BuildTools/RecordHotfix/_cdn_ssh_private_key/20181105/ip_47.96.5.112/pk.txt")
    remotefile = sftp.mk_dir("/work/1.22.1.117523", "/data/wwwroot/static.aldzn.xyimg.net/test1")
    sftp.put_tree("/work/1.22.1.117523", remotefile)
    sftp.sshclient.close()


def dir_listcount(path):
    return os.listdir(path).__len__()
def dir_isempty(path):
    if dir_listcount(path) == 0:
        return True
    return False










def save(path,**kv):
    f = open(path,'w',encoding='utf-8')
    json.dump(kv,f)
    f.close()
def cleardata(path):
    os.remove(path)
def getdata(path):
    f = open(path,'r',encoding='utf-8')
    s = json.load(f)
    f.close()
    return s
# def buildfolder_tree(leafpath,exfunc=os.path.exists,mdfunc=os.mkdir,sep=os.path.sep):
#     q = []
#     pt = leafpath
    
#     while not exfunc(pt):
#         q.append(pt)
#         if pt.find(sep) != -1:
#             pt = pt.replace(sep+os.path.basename(pt),'')
#         else:
#             pt = '.'
#             break
#     while q.__len__() != 0:
#         mdfunc(q.pop())
#     return leafpath

# def buildfolder(*ps):
#     import shutil
#     p = os.path.sep.join(ps)
#     print(ps)
#     print('------------------------'+p)

#     if os.path.exists(p):
#         shutil.rmtree(p)
#     buildfolder_tree(p)

# def ftp_upload(host=None, username=None,key_filename=None, localfile=None, remotefile=None):
if __name__ == "__main__":
    #__test_ftp_upload()
    # ftp_upload(sys.argv[1], sys.argv[2], sys.argv[3], sys.argv[4], sys.argv[5])
    
    # if sys.argv[5] == 'upload':
    #     # raise Exception('测试用，防止出问题')
    #     ftp_upload(sys.argv[1], sys.argv[2], sys.argv[3],sys.argv[4])
    # else:
    #     ftp_download(sys.argv[1], sys.argv[2], sys.argv[3],sys.argv[4])
    exit()