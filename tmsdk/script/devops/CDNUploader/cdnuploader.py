# -*- encoding: utf-8 -*-
import sys,os
sys.path.append(os.path.abspath(os.path.join(__file__,'..','..')))





import shutil,subprocess,json
from argparse import ArgumentParser

from comlib import com
from comlib import SSHConnector
from comlib import FTPManager
from comlib import Logging,HTTPManager,Path

ip = '47.96.5.112'
username = 'root'
keyfile = os.path.join(os.path.join(os.getcwd(),'..','..'),'RecordHotfix',r'_cdn_ssh_private_key/20181105/ip_47.96.5.112/pk.txt')
download_tmppath =os.path.join(os.getcwd(),'..','..','tmp')
download_tmppath2 =os.path.join(os.getcwd(),'..','..','tmp2')
hotfixsavepath = os.path.join(os.getcwd(),'..','..','_save')
aliyuntxt = os.path.join(os.getcwd(),'..','..','aliyun')
zp = os.path.join(os.getcwd(),'..','..','zip.json')
vp = os.path.join(os.getcwd(),'..','..','ver.json')
shp = os.path.join(os.getcwd(),'..','..','shp.json')

localtime = com.getlocaltime()

debug = False

def sftp_upload(data):
    '''
    读二维数组
    '''
    if data.__len__() == 0:
        print('上传为空')
        return
    if not debug:
        sftp = SSHConnector(host=ip, username=username, key_filename=keyfile)


    for l2m in data:
        localp = l2m[0]
        remotep = l2m[1]
        if not debug:
            if os.path.isdir(localp):
                rp = sftp.mk_dir(localp, remotep)
                sftp.put_tree(localfile=localp, remotefile=rp)
            if os.path.isfile(localp):
                sftp._sftp_operation(remotefile=remotep,localfile=localp,op='put')
    if not debug:
        sftp.sshclient.close()
    Logging.info("hotfix ftp upload finish")

def sftp_download(data):
    Logging.info("-------------hotfix sftp download start-----------------")
    if data.__len__() == 0:
        print('下载为空')
        return
    com.buildfolder(download_tmppath)
    if not debug:
        sftp = SSHConnector(host=ip, username=username, key_filename=keyfile)


    for l2m in data:
        localp = l2m[0]
        remotep = l2m[1]
        # zip.json里都是文件夹,这里默认是文件夹
        # ["/Volumes/TM148/JenkinsRoot/workspace/DNL_Download_Hotfix_148/_save/2020_0331_113309/zip/android/mg/1.34.1.186870", "/data/wwwroot/static.aldzn.xyimg.net/mg/asset/android/zip"]
        version = os.path.basename(localp) # 1.34.1.186870
        remotep = remotep+'/'+version

        # 用localp是因为越狱的cdn地址是ios不是iosother
        # /data/wwwroot/static.aldzn.xyimg.net/asset/ios/zip
        channel = localp.split(os.path.sep)[-2] # mg
        plat = localp.split(os.path.sep)[-3]
        
        need_download = []
        
        for fn in os.listdir(localp):
            if fn.endswith('.zip'):
                need_download.append(remotep + '/' + fn)
        for ddd in need_download:
            zipp = os.path.join(download_tmppath,plat+'_'+channel+'_'+os.path.basename(ddd))
            if not debug:
                sftp._sftp_operation(remotefile=ddd,localfile=zipp,op='get')
    if not debug:
        sftp.sshclient.close()
    Logging.info("-------------hotfix sftp download finish-----------------")

def getlowver(ver):
    return ver.split('.')[-1]
def getlowver_filename(path, ext):
    return getlowver(os.path.basename(path).replace(ext,'').replace('package-','').split('-')[0])

def findlowver(path):
    lowvfn = ''
    for fn in os.listdir(path):
        if getlowver_filename(fn,'.txt'):
            lowvfn = fn
            break
    f = open(os.path.join(path,lowvfn),'r')
    return getlowver_filename(f.readline().split(',')[0].replace('package-',''),'.zip')

def buildfolder_diff(path):
    
    bfn = os.path.join(path,localtime)
    backup_ver = com.buildfolder(os.path.join(bfn,'backup','online_version_backup'),remove_exists=False)
    cheak_zip = com.buildfolder(os.path.join(bfn,'backup','pre_heat_check'),remove_exists=False)
    verandroid = com.buildfolder(os.path.join(bfn,'version','android'),remove_exists=False)
    verios = com.buildfolder(os.path.join(bfn,'version','ios'),remove_exists=False)
    veriosother = com.buildfolder(os.path.join(bfn,'version','iosother'),remove_exists=False)
    zipandroid = com.buildfolder(os.path.join(bfn,'zip','android'),remove_exists=False)
    zipios = com.buildfolder(os.path.join(bfn,'zip','ios'),remove_exists=False)
    zipiosother = com.buildfolder(os.path.join(bfn,'zip','iosother'),remove_exists=False)


    shutil.copy2(os.path.join(os.getcwd(),'..','..','RecordHotfix','_hotfix_version_zip_backup','online_version_backup','backup-origin-version.sh'),os.path.join(bfn,'backup','online_version_backup'))
    shutil.copy2(os.path.join(os.getcwd(),'passwd.sh'),os.path.join(bfn,'backup','online_version_backup'))
    shutil.copy2(os.path.join(os.getcwd(),'..','..','RecordHotfix','_hotfix_version_zip_backup','online_version_backup','onlineurls'),os.path.join(bfn,'backup','online_version_backup'))
    shutil.copy2(os.path.join(os.getcwd(),'backup-origin-version-zip.sh'),os.path.join(bfn,'backup','pre_heat_check'))

    com.win2unixformat(os.path.join(bfn,'backup','online_version_backup','backup-origin-version.sh'))
    com.win2unixformat(os.path.join(bfn,'backup','online_version_backup','passwd.sh'))
    com.win2unixformat(os.path.join(bfn,'backup','pre_heat_check','backup-origin-version-zip.sh'))

    os.popen('chmod 777 %s' % os.path.join(bfn,'backup','online_version_backup','backup-origin-version.sh'))
    return bfn,verandroid,verios,veriosother,zipandroid,zipios,zipiosother,backup_ver,cheak_zip

def downloadhotfix(ftp,local,localver,remote,remotever,lowv,tarv):
    ext = '.txt'
    def fa(path):
        print("file  "+path)
        l = os.path.splitext(path)

        if l.__len__() > 1 and l[-1] == ext:
            if lowv<=getlowver_filename(path,ext):
                return True
        return False
    def da(path):
        print("dir  "+path)
        return True
    ftp.download(local,remote,fa,da)
    lowv = findlowver(os.path.join(local,tarv))
    print(lowv)
    ext = '.zip'
    ftp.download(local,remote,fa,da)
    # 下版本文件
    ftp.download(localver,remotever)

def gethotfixsavepath():

    if not os.path.exists(hotfixsavepath):
        os.mkdir(hotfixsavepath)
    return hotfixsavepath

def cleardata(path):
    if os.path.exists(path):
        os.remove(path)

def aliyun_push(cdndict,isapply):
    isapply = isapply == 'true'
    AccessKey=''
    AccessKeySecret=''
    remotepath = []
    localpath_raw = []
    hasday2 = False
    for pathref in cdndict:
        remotepath.append(pathref[1].replace('/data/wwwroot/static.aldzn.xyimg.net','http://aldzn.srccwl.com'))
        localpath_raw.append(pathref[0])
    # 去重
    remotepath = list(set(remotepath))
    localpath = list(set(localpath_raw))
    okpath = []
    for i in remotepath:
        for tar in localpath:
            platlocal=os.path.basename(os.path.dirname(os.path.dirname(tar))) # android
            platremote=os.path.basename(os.path.dirname(i))# android
            # 因为iosother在cdn上是ios，所以要加后面判断
            if platremote == platlocal or (platlocal == 'iosother' and platremote == 'ios'):
                # 1.30.1.123456
                verfolder = os.path.basename(tar)
                for fs in os.listdir(tar):
                    okpath.append(com.strjoin('/',i,verfolder,fs))

    print('阿里云预热',flush=True)
    # print(okpath,flush=True)
    # aliyuntxt = r'D:\_WorkSpace\1.0\BuildTools\downloadhotfix\aliyun'

    if not os.path.exists(aliyuntxt):
        os.mkdir(aliyuntxt)

    if isapply:
        afs = os.listdir(aliyuntxt)
        for af in afs:
            afpath = os.path.join(aliyuntxt,af)
            with open(afpath,'r') as fs:
                oldpush = fs.read().split('\n')
                okpath += oldpush
                okpath = list(set(okpath)) # 去重
            
            os.remove(afpath)
    else:
        okpath = list(set(okpath)) # 去重
    okpath.sort()
    # 拆分为500的块
    blocks = [okpath[i:i + 500] for i in range(0, len(okpath), 500)]
    # print(blocks)

    olds = os.listdir(aliyuntxt)
    for old in olds:
        os.remove(os.path.join(aliyuntxt,old))
    for count in range(0,blocks.__len__()):
        with open(os.path.join(aliyuntxt,'%s.txt'%count),'w') as fs:
            fs.write('\n'.join(blocks[count]))

    # 测试新预热生成
    # ["/Volumes/TM148/JenkinsRoot/workspace/DNL_Download_Hotfix_148/_save/2020_0331_113309/zip/android/mg/1.34.1.186870", "/data/wwwroot/static.aldzn.xyimg.net/mg/asset/android/zip"]
    needpush = []
    Path.ensure_dirnewest('txttmp')
    Path.ensure_dirnewest(download_tmppath2)

    for localpath,remotepath in cdndict:
        allfiles = os.listdir(localpath)
        version = os.path.basename(localpath)
        # 用localp是因为越狱的cdn地址是ios不是iosother
        # 例如/data/wwwroot/static.aldzn.xyimg.net/asset/ios/zip
        # /Volumes/TM148/JenkinsRoot/workspace/DNL_Download_Hotfix_148/_save/2020_0331_113309/zip/android/mg/1.34.1.186870
        channel = localpath.split(os.path.sep)[-2] # mg
        plat = localpath.split(os.path.sep)[-3] # android

        for filename in allfiles:
            url = f'''{remotepath.replace('/data/wwwroot/static.aldzn.xyimg.net','http://aldzn.srccwl.com')}/{version}/{filename}'''
            needpush.append(url)
            if filename.endswith('.zip'):
                downloadpath = os.path.join(download_tmppath2,plat+'_'+channel+'_'+filename)
            else:
                downloadpath = os.path.join('txttmp',plat+'_'+channel+'_'+filename)
            HTTPManager.download_http(url,downloadpath)

    com.savedata('\n'.join(needpush),'xinyure.txt')




def dir_listcount(path):
    return os.listdir(path).__len__()
def dir_isempty(path):
    if dir_listcount(path) == 0:
        return True
    return False


def cheakmd5():

    if not os.path.exists(download_tmppath):
        raise Exception('tmp路径不存在')
        
    for fn in os.listdir(download_tmppath):
        # plat+'_'+channel+'_'+os.path.basename(ddd)
        # android_mg_package-1.1.1.11111-1.1.1.22222.zip
        t = fn.split('_')
        plat = t[0]
        channel = t[1]
        verrrrrrr = t[2]
        targetver = verrrrrrr.replace('.zip','').split('-')[-1]
        localzip = os.path.join(savep,localtime,'zip',plat,channel,targetver,verrrrrrr)

        m = com.get_md5(os.path.join(download_tmppath,fn))
        localzipmd5 = com.get_md5(localzip)
        if m == localzipmd5:
            Logging.info(fn + '------>OK')
        else:
            Logging.info(fn + '------>!!!ERROR!!!')
            raise Exception('MD5不一致')


if __name__ == "__main__":

    parse = ArgumentParser()
    parse.add_argument('-afv','--androidfullpkgver',dest='afpv',required=False,default='None')
    parse.add_argument('-atv','--androidtargetver',dest='atv',required=False,default='None')
    parse.add_argument('-anu','--androidneedupload',dest='anu',required=False,default='None')
    
    parse.add_argument('-iofv','--iosotherfullpkgver',dest='iofpv',required=False,default='None')
    parse.add_argument('-iotv','--iosothertargetver',dest='iotv',required=False,default='None')
    parse.add_argument('-ionu','--iosotherneedupload',dest='ionu',required=False,default='None')

    parse.add_argument('-ifv','--iosfullpkgver',dest='ifpv',required=False,default='None')
    parse.add_argument('-itv','--iostargetver',dest='itv',required=False,default='None')
    parse.add_argument('-inu','--iosneedupload',dest='inu',required=False,default='None')

    parse.add_argument('-up','--uploadcdn',dest='upcdn',required=False)
    parse.add_argument('-upt','--uploadcdntype',dest='upcdntype',required=False)
    
    parse.add_argument('-a','--apply',dest='apply',required=True)
    
    # needupload_android = ['sn79-1.35.1.192437-1.35.1.194707','oppo']
    # needupload_iosother = ['xy']
    # needupload_ios = ['zyyz','mgdy-1.20.20.177772-1.20.20.193601']

    # needupload_android = ['mg-1.36.1.198181-1.36.1.199770']

    # needupload_ios = ['zyyz']

    needupload_android = []
    needupload_iosother = []
    needupload_ios = []

    print(sys.argv)
    args = parse.parse_args()
    if args.anu != 'None':
        needupload_android = args.anu.split(',')
    if args.ionu != 'None':
        needupload_iosother = args.ionu.split(',')
    if args.inu != 'None':
        needupload_ios = args.inu.split(',')

    if args.upcdn == 'true':
        if args.upcdntype == 'version':
            # vp = os.path.join(os.path.split(os.getcwd())[0],'ver.json')
            # shp = os.path.join(os.path.split(os.getcwd())[0],'shp.json')

            # 备份
            # shpd = com.loadfile_json(shp)
            subprocess.call('sh %s' % 'ver_bak/backup-origin-version.sh',shell=True,timeout=1200)

            allver = com.loadfile_json(vp)
            uploadver = []

            alluploadchannel = []
            if needupload_android.__len__() != 0:
                alluploadchannel += needupload_android
            if needupload_iosother.__len__() != 0:
                alluploadchannel += needupload_iosother
            if needupload_ios.__len__() != 0:
                alluploadchannel += needupload_ios

            for ver in allver:
                for needupload in alluploadchannel:
                    needupload = needupload.split('-')[0] # 去除 -
                    if needupload == ver[0].split(os.sep)[-2]:
                        uploadver.append(ver)
            for rmver in uploadver:
                allver.remove(rmver)
            
            sftp_upload(uploadver)
            if allver.__len__() != 0:
                com.dumpfile_json(allver,vp) # 保存此次未上传的version
            else:
                cleardata(vp)

            # cleardata(shp)


            exit(0)



    savep = gethotfixsavepath()
    print(savep)

    rootfoldname,verandroid,verios,veriosother,zipandroid,zipios,zipiosother,backup_ver,cheak_zip = buildfolder_diff(savep)
    print(rootfoldname,verandroid,verios,veriosother,zipandroid,zipios,zipiosother)

    pathdata = com.loadfile_json(os.path.join(os.getcwd(),'all.json'))
    ftppath = pathdata['ftppath']
    cdnpath = pathdata['cdnpath']
    zippath_remote_android = pathdata['zippath']['android']
    zippath_remote_ios = pathdata['zippath']['ios']
    zippath_remote_iosother = pathdata['zippath']['iosother']
    verpath_remote = pathdata['verpath']
    channelplat_android = pathdata['channelplat']['android']
    channelplat_iosother = pathdata['channelplat']['iosother']
    channelplat_ios = pathdata['channelplat']['ios']

    l2cdndict=[]
    ver2cdndict=[]


    ftp = FTPManager('192.168.2.65',21,'anonymous','123456')
    remote_cdn = r'/data/wwwroot/static.aldzn.xyimg.net/test1'








    # 拉安卓
    if hasattr(args,'atv') and args.atv != None and args.atv != 'None':
        for uploadchannel in needupload_android:
            if '-' not in uploadchannel:

                fromversion = args.afpv
                toversion = args.atv
            else:
                tmp = uploadchannel.split('-')
                uploadchannel = tmp[0]
                fromversion = tmp[1]
                toversion = tmp[2]

            channelpath = channelplat_android[uploadchannel]

            zipandroid_channel = os.path.join(zipandroid,uploadchannel)

            if uploadchannel == 'mgother915':
                remote_android = com.strjoin('/',ftppath,channelplat_android['mg'],zippath_remote_android,toversion)
                remote_androidver = com.strjoin('/',ftppath,channelplat_android['mg'],verpath_remote)
            else:
                remote_android = com.strjoin('/',ftppath,channelpath,zippath_remote_android,toversion)
                remote_androidver = com.strjoin('/',ftppath,channelpath,verpath_remote)

            lowv_android = getlowver(fromversion)

            l2cdndict.append((os.path.join(zipandroid_channel,toversion),com.strjoin('/',cdnpath,channelpath,zippath_remote_android)))

            ver2cdndict.append((os.path.join(verandroid,uploadchannel,'version.json'),com.strjoin('/',cdnpath,channelpath,verpath_remote)))

            downloadhotfix(ftp,zipandroid_channel,os.path.join(verandroid,uploadchannel,'version.json'),remote_android,remote_androidver,lowv_android,toversion)
    # 拉越狱
    if hasattr(args,'iotv') and args.iotv != None and args.iotv != 'None':
        for uploadchannel in needupload_iosother:
            if '-' not in uploadchannel:

                fromversion = args.iofpv
                toversion = args.iotv
            else:
                tmp = uploadchannel.split('-')
                uploadchannel = tmp[0]
                fromversion = tmp[1]
                toversion = tmp[2]

            channelpath = channelplat_iosother[uploadchannel]

            zipiosother_channel = os.path.join(zipiosother,uploadchannel)


            remote_iosother = com.strjoin('/',ftppath,channelpath,zippath_remote_iosother,toversion)
            remote_iosotherver = com.strjoin('/',ftppath,channelpath,verpath_remote)
            lowv_iosother = getlowver(fromversion)


            l2cdndict.append((os.path.join(zipiosother_channel,toversion),com.strjoin('/',cdnpath,channelpath,zippath_remote_iosother)))

            ver2cdndict.append((os.path.join(veriosother,uploadchannel,'version.json'),com.strjoin('/',cdnpath,channelpath,verpath_remote)))

            downloadhotfix(ftp,zipiosother_channel,os.path.join(veriosother,uploadchannel,'version.json'),remote_iosother,remote_iosotherver,lowv_iosother,toversion)

    # 拉ios
    if hasattr(args,'itv') and args.itv != None and args.itv != 'None':
        for uploadchannel in needupload_ios:
            if '-' not in uploadchannel:

                fromversion = args.ifpv
                toversion = args.itv
            else:
                tmp = uploadchannel.split('-')
                uploadchannel = tmp[0]
                fromversion = tmp[1]
                toversion = tmp[2]

            channelpath = channelplat_ios['appstore'] + '/' + uploadchannel

            zipios_channel = os.path.join(zipios,uploadchannel)

            remote_ios = com.strjoin('/',ftppath,channelpath,zippath_remote_ios,toversion)
            remote_iosver = com.strjoin('/',ftppath,channelpath,verpath_remote)
            lowv_ios = getlowver(fromversion)

            l2cdndict.append((os.path.join(zipios_channel,toversion),com.strjoin('/',cdnpath,channelpath,zippath_remote_ios)))

            ver2cdndict.append((os.path.join(verios,uploadchannel,'version.json'),com.strjoin('/',cdnpath,channelpath,verpath_remote)))


            downloadhotfix(ftp,zipios_channel,os.path.join(verios,uploadchannel,'version.json'),remote_ios,remote_iosver,lowv_ios,toversion)
    
    ftp.close()

    
    com.dumpfile_json(l2cdndict,zp)
    if args.apply == 'false':
        com.dumpfile_json(ver2cdndict,vp)
    else:
        with open(vp,'r+',encoding='utf-8') as f:
            js = json.load(f)
            js += ver2cdndict
            # jsset = set()
            # for tp in js:
            #     jsset.add(tuple(tp))
            # for tp in ver2cdndict:
            #     jsset.add(tuple(tp))
            print("--js--")
            print(js)
            print("--ver2cdndict--")
            f.seek(0)
            f.truncate()
            print(ver2cdndict)
            print("--js--")
            print(js)
            json.dump(js,f,ensure_ascii=False,indent=4)
    com.dumpfile_json((backup_ver,cheak_zip),shp)
    
    # sftp_download(com.loadfile_json(zp))
    # cheakmd5()
    # 一次登陆传输全部zip
    if args.upcdn == 'true':
        if args.upcdntype == 'zip':
            try:
                sftp_upload(com.loadfile_json(zp))
            except Exception as e:

                cleardata(vp)
                cleardata(shp)
                raise e

            sftp_download(com.loadfile_json(zp))
            cheakmd5()

            if args.upcdn == 'true':
                cleardata(zp)

            try:
                aliyun_push(l2cdndict,args.apply)
            except Exception as e:
                import traceback
                print(traceback.format_exc())
        if args.upcdntype == 'version_simple':
            pass
            # try:
            #     sftp_upload(com.loadfile_json(vp))
            #     # aliyun_push(ver2cdndict)

            # except Exception as e:
            #     raise e
            # finally:
            #     cleardata(zp)
            #     cleardata(vp)
            #     cleardata(shp)
        if args.upcdntype == 'zip and version':
            pass

