# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.dirname(__file__)
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH

from comlib import com
from comlib import TMApkManager
from comlib import HTTPManager
from comlib import workspace
from comlib import SVNManager
from comlib import Path

from comlib.conf.ref import scmpath_devops
from comlib.conf.loader import Loader
from comlib.comobj import *
workdir = os.getcwd()



@errorcatch
@workspace
def run():
    if sys.argv[1::].__len__() != 5:
        raise StopException('参数不足',sys.argv,'')
    

    plat = sys.argv[1]
    mode = sys.argv[2]
    channel = sys.argv[3]
    packurl = sys.argv[4]
    secID = sys.argv[5]

    scmpath_devopsconf = Loader.load(scmpath_devops)

    keystorepath = os.path.join(workdir,'keystore')
    Path.ensure_svn_pathexsits(keystorepath,scmpath_devopsconf.keystore['url'])

    tmp = packurl.rsplit('/',1)
    baseurl = tmp[0]
    oldfilename = tmp[-1]

    
    HTTPManager.download_http(packurl,oldfilename)
    apk = TMApkManager(oldfilename)
    # 额外功能
    # # 添加ab
    # addab(apk.zippath)
    # 加固
    apk.encrypt_with_bangbang_auto(secID)
    # 对齐
    apk.align_apk()
    # 签名
    apk.sign_auto(keystorepath,channel)
    # 上传
    remotedir = com.get_ftp_savepath(plat,mode,channel)
    remotefile = f'{remotedir}/{apk.zippath}'
    G_ftp.uploadfile(apk.zippath,remotefile)
    # 创建html
    hosturl = baseurl.split('/__zip/')[0]
    com.creat_download_html(f'{hosturl}/{remotefile}',workdir)
    # # 删除所有包
    # apk.clear()

def addab(apkpath):
    import zipfile
    zf = zipfile.ZipFile(apkpath,'a')
    p = r'E:\_Download\signtest\AssetBundles'
    for f in os.listdir(p):
        zf.write(os.path.join(p,f),f'assets/AssetBundles/{f}',compress_type=zipfile.ZIP_STORED)

    zf.close()
if __name__ == "__main__":
    run()
