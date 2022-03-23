# -*- encoding: utf-8 -*-
import sys,os,shutil
sys.path.append(os.path.join(os.getcwd(),'..'))
sys.path.append(os.path.join(os.getcwd(),'.'))

from comlib import FTPManager
from comlib import com

def build_assetsdiff():
    pass

    
def build_hotfix(lastversion,nowversion,*files,plat='a'):
    basepath = '/hotfix/dev'
    zippath = 'asset/{}/zip'
    cp ='' 
    if plat == 'a':
        cp = '/'.join([basepath,'mg',zippath.format('android')])
    elif plat == 'io':
        cp = '/'.join([basepath,zippath.format('ios')])
    else:
        cp = '/'.join([basepath,'zyappstore',plat,zippath.format('ios')])
    com.logout(cp)
    cp = '/'.join([cp,lastversion])

    ftpm = FTPManager('192.168.2.65',21,'ftp','123456')    
    savepath = 'hotfix'
    com.buildfolder(savepath)

    if ftpm.direxsits(cp):
        ftpm.download(savepath,cp,file_filter=lambda x: x.endswith('.zip'))
    else:
        os.mkdir(os.path.join(savepath,lastversion))

    savepath = os.path.join(savepath,lastversion)



    # oldhotfix = os.listdir(savepath)
    # oldhotfix.sort()

    newclientShortVersion = nowversion.split('.')[3]
    newserverShortVersion = nowversion.split('.')[1]
    newversionconfig = versionconfig_format % (newclientShortVersion,newclientShortVersion,newserverShortVersion)
    newversionjson = versionjson_format % (nowversion)

    for zipname in os.listdir(savepath):
        fullpath = os.path.join(savepath,zipname)
        
        newzipname = zipname.replace(lastversion,nowversion)
        newzippath = os.path.join(savepath,newzipname)
        os.rename(fullpath,newzippath)

        com.buildfolder('tmp')
        com.unzip(newzippath,'tmp')
        os.remove(newzippath)
        aspath = os.path.join('tmp','AssetBundles')
        if not os.path.exists(aspath):
            os.mkdir(aspath)
        for ffff in files:
            shutil.copy2(ffff,aspath)
        with open(os.path.join('tmp','version.config'),'w') as fs:
            fs.write(newversionconfig)
        with open(os.path.join('tmp','version.json'),'w') as fs:
            fs.write(newversionjson)
        com.zipall('tmp',newzippath,compression=8)

        build_hotfixtxt(newzippath)
        shutil.rmtree('tmp')

    ftlist = []
    for fl in files:
        ftlist.append((fl,os.path.join('AssetBundles',fl)))
    zipname_format = 'package-{}-{}.zip'
    newversionzipname = zipname_format.format(lastversion,nowversion)
    newversionzippath = os.path.join(savepath,newversionzipname)
    com.zip_addfile(newversionzippath,ftlist)
    com.zip_write(newversionzippath,newversionconfig,'version.config')
    com.zip_write(newversionzippath,newversionjson,'version.json')
    build_hotfixtxt(newversionzippath)

    os.rename(savepath,os.path.join('hotfix',nowversion))


versionconfig_format = '''{
  "clientShortVersion": "%s", 
  "clientVersion": 1, 
  "commitAuthor": "commitAuthor commitAuthor commitAuthor commitAuthor", 
  "commitID": "%s", 
  "commitMessage": "commitMessage commitMessage commitAuthor commitAuthor", 
  "commitTime": 12345678, 
  "serverShortVersion": %s, 
  "serverVersion": 1
}'''
versionjson_format = '''{
  "android": "%s", 
  "ios": "1.0.1.0", 
  "pc": "1.0.1.0"
}'''
def build_hotfixtxt(zippath):
    zipname = os.path.basename(zippath)
    savepath = os.path.dirname(zippath)
    newsize = os.path.getsize(zippath)
    newmd5 = com.get_md5(zippath)
    newtxtname = zipname.replace('.zip','.txt')
    txtcontent = '{},md5:{},{} bytes'
    with open(os.path.join(savepath,newtxtname),'w') as fs:
        fs.write(txtcontent.format(zipname,newmd5,newsize))


if __name__ == "__main__":
    build_hotfix('1.10.10.126320','1.10.10.199555','symurl',plat='zyyz')
    # print(com.get_md5(sys.argv[1]))
    # 860032043037979
    # 860032043037961
    # 869705038543215
    # print(com.getbinpath('aapt'))
    # print(os.path.exists(com.getbinpath('aapt')+'.exe'))
