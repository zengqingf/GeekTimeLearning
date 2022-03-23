# -*- encoding: utf-8 -*-
import shutil,re,zipfile
from comlib.comobjm import *
sys.path.append(os.path.abspath(os.path.join(__file__,'..','..')))

from comlib.exception import errorcatch,StopException,LOW,NORMAL,HIGH
from comlib.binm import BinManager


from comlib.conf.loader import Loader
from comlib.conf.ref import *
from comlib.pathm import Path


@errorcatch(HIGH)
class ZipManager(object):
    '''
    doc
    '''
    ZIP_STORED = 0
    ZIP_DEFLATED = 8
    ZIP_BZIP2 = 12
    ZIP_LZMA = 14
    def __init__(self,zippath):
        self.zippath = zippath
        self.tsp = com.timemark
        self.unzippath = ''
        self.zf = None
        # tmp = os.path.splitext(self.zippath)
        # self.unzippath = tmp[0]+'_'+self.tsp
        # self.ext = tmp[1]
        # self.repackpath = self.unzippath + '_repack' + self.ext

        self.clearpathlist = []
    def _setunzippath(self,targetpath=None):
        if targetpath == None:
            tmp = os.path.splitext(self.zippath)
            targetpath = tmp[0]+'_'+self.tsp
        self.unzippath = os.path.abspath(targetpath)
    def _setzippath(self,newzippath):
        self.zippath = os.path.abspath(newzippath)
    def getrepackfile(self):
        tmp = os.path.splitext(self.zippath)
        ext = ''
        if tmp.__len__() == 2:
            ext = tmp[1]
        repackfile = self.unzippath + '_repack' + ext
        return repackfile

    def open(self,mode='r'):
        self.zf = zipfile.ZipFile(self.zippath,mode=mode)
        return self.zf
        
    def unzip(self,targetpath=None):
        self._setunzippath(targetpath)
        com.unzip(self.zippath,self.unzippath)
    def zip(self,method=ZIP_STORED):
        repackfile = self.getrepackfile()

        com.zipall(self.unzippath,repackfile,method)
        self._setzippath(repackfile)
    def clear(self):
        Log.info('[clear]')
        Path.ensure_pathnotexsits(*self.clearpathlist)
    def addclear(self,*path):
        for p in path:
            if p not in self.clearpathlist:
                self.clearpathlist.append(p)
    def getnamelist(self,zippt):
        return zippt.namelist()
    def fileexists(self,zippt,fileinzip):
        namelist = self.getnamelist(zippt)
        if fileinzip in namelist:
            return True
        return False
    def removefile(self,*files):
        '''
        会关闭zipfile 然后用aapt操作
        '''
        if files in (None,[],()):
            return
        self.close()
        out,code = BinManager.aapt(f'remove {self.zippath} ' + ' '.join(files),errException=StopException("zip移出文件失败",locals(),''))
    def isopen(self):
        if self.zf == None:
            return False
        if self.zf.fp == None:
            return False
        return True
    def getfilestream(self,path_in_zip,mode):
        fs = com.getfilestream4zip(self.zippath,path_in_zip,mode)
        return fs
    def getfilestream_pt(self,zippt,path_in_zip,mode):
        fs = zippt.open(path_in_zip,mode.replace('b','').replace('+',''))
        return fs

    def readall(self,path_in_zip,mode='r'):
        '''
        读取zip中的某文件所有数据
        '''
        data = self.getfilestream(path_in_zip,mode).read()
        return data
    def readall_pt(self,zippt,path_in_zip,mode='r'):
        fs = self.getfilestream_pt(zippt,path_in_zip,mode)
        data = fs.read()
        return data
    def write(self,zippt,data,path_in_zip,compress=8):
        zippt.writestr(path_in_zip,data,compress_type=compress)
    def addfile(self,zippt,filepath,path_in_zip,compress=8):
        zippt.write(filepath,path_in_zip,compress_type=compress)
    def close(self):
        if self.isopen():
            self.zf.close()



    @staticmethod
    def ziptarget(tarpaths:list,zippath,compression=zipfile.ZIP_STORED,debug=False):
        """
        指定路径列表压缩
        """
        def tmpiter():
            for tarpath in tarpaths:
                dirname = os.path.dirname(tarpath)
                if os.path.isdir(tarpath):
                    for dirpath, dirnames, filenames in os.walk(tarpath):
                        for fn in filenames:
                            fn = os.path.join(dirpath,fn)
                            yield fn,dirname
                else:
                    yield tarpath,dirname
        def mzip(filelist,zipname,compression=zipfile.ZIP_STORED,debug=False):
            Log.info('[zip] zipname  '+zipname)
            Log.info('[zip] compression  %s'%compression)
            zf = zipfile.ZipFile(zipname,'w',compression=compression)
            for f,basedir in filelist:
                filepathinzip = f.replace(basedir,'').strip(os.path.sep)
                if debug:
                    Log.debug('[zip] %s --> %s'%(f,filepathinzip))
                zf.write(f,filepathinzip)
            zf.close()
        mzip(tmpiter(),zippath,compression=compression,debug=debug)



class ApkInfo():
    def __init__(self,apkpath):
        self.apkpath = apkpath
        self.apkm = TMApkManager(apkpath)
        self.packagename = 'com.hegu.dnl.debug'
        self.appname = 'aldzn'
        self.versioncode,self.versionname = (1,'1.1.1.1')

        if os.path.exists(self.apkpath):
            self.packagename = self.apkm.get_packagename()
            self.appname = self.apkm.get_appname()
            self.versioncode,self.versionname = self.apkm.get_versioncode_versionname()
        
class TMApkInfo(ApkInfo):
    def __init__(self, apkpath):
        super().__init__(apkpath)
        self.usechannel = 'dnfdebug'
        self.usesdk = 'dnfdebug'

        if os.path.exists(self.apkpath):
            self.usechannel = self.apkm.get_sdkconf()
            if self.usechannel == None:
                self.usechannel = 'debug'
            self.usechannel = self.usechannel.lower()
            if self.usechannel == 'none':
                self.usechannel = 'dnfdebug'
            
            self.usesdk = Loader.channel2sdk(self.usechannel)

class Package():
    def __init__(self,assetpath) -> None:
        self.assetpath = assetpath
class TMPackage(Package):
    def __init__(self,assetpath,abpath,sdkconfpath) -> None:
        super().__init__(assetpath)
        self.abpath=abpath
        self.sdkconfpath=sdkconfpath
class PackageManager(ZipManager):
    def __init__(self,zippath) -> None:
        super().__init__(zippath)
        self.package = None

@errorcatch(HIGH)
class ApkManager(PackageManager):
    def __init__(self,apkpath):
        super().__init__(apkpath)
        self.package = Package('assets')
        self.apktoolymlpath = os.path.join(self.unzippath,'apktool.yml')
    @staticmethod
    def getapkinfo(apkpath)->ApkInfo:
        return ApkInfo(apkpath)

    def get_versioncode_versionname(self)->tuple:

        out,code = BinManager.aapt('dump xmltree %s AndroidManifest.xml'%self.zippath)
        # out,code = com.cmd('%s dump xmltree %s AndroidManifest.xml'%(aaptpath,self.zippath))

        m = re.search(r'android:versionCode\(.*\)=\(type .*\)(.*)',out)

        m2 = re.search(r'android:versionName\(.*\)="(.*?)"',out)
        vercode = -1
        vername = -1
        if m != None:
            vercode = eval(m.group(1))
        if m2 != None:
            vername = m2.group(1)
        Log.info(f'[get_versioncode_versionname] versioncode={vercode} versionname={vername}')

        return vercode,vername
    def get_packagename(self):
        r,code = BinManager.aapt(f'dump badging {self.zippath}',filterstr='package:')
        # package: name='com.hegu.dnl.huawei'
        # com.hegu.dnl.oppo.nearme.gamecenter
        m = re.search("package: name='(.*?)'",r)
        if m != None:
            Log.info(f'[get_packagename] {m.group(1)}')
            return m.group(1)
        return None
    def get_appname(self):
        '''
        uses-permission: name='android.permission.READ_EXTERNAL_STORAGE'\n
        application-label:'ALD15'\n
        application-icon-120:'res/mipmap-ldpi/app_icon.png'
        '''
        r,code = BinManager.aapt(f'dump badging {self.zippath}')
        m = re.search("application-label:'(.*?)'",r)
        if m != None:
            # import chardet
            # chardet.detect(m.group(1))
            Log.info(f'[get_appname] appname={m.group(1)}')
            return m.group(1)
        return None
    def get_metadata(self,metaname)->str:
        out,code = BinManager.aapt('dump xmltree %s AndroidManifest.xml'%self.zippath)
        # out,code = com.cmd('%s dump xmltree %s AndroidManifest.xml'%(aaptpath,self.zippath))

        lines = out.split('\n')
        for i in range(0,lines.__len__()):
            if metaname in lines[i]:
                m = re.search('"(.*?)"',lines[i+1])
                if m == None:
                    return
                Log.info(m.group(1))
                return m.group(1)

    def align_apk(self)->str:
        '''
        return outpath
        '''
        # tarsdkversion = f"{tarsdkversion}"
        outpath = self.zippath.replace('.apk','_align.apk')
        # enabletool = [i for i in os.listdir(buildtoolpath) if i.startswith(tarsdkversion)]
        # if enabletool == []:
        #     raise Exception(f"{tarsdkversion}版本工具没找到")
        # enabletool = enabletool[0]
        
        # toolpath = os.path.join(buildtoolpath,enabletool,com.getvalue4plat('zipalign.exe','zipalign'))
        # out,code = com.cmd(f'"{toolpath}" -f -v 4 "{self.zippath}" "{outpath}"',errException=Exception("align失败"))

        out,code = BinManager.zipalign(f'-f -v 4 "{self.zippath}" "{outpath}"',errException=Exception("align失败"))
        self.addclear(outpath)
        self._setzippath(outpath)
        
        return outpath
    
    def unzip(self,targetpath=None):
        # 打个时间戳
        self._setunzippath(targetpath)

        # out,code = com.cmd(f'apktool d {self.zippath} -o {self.unzippath}',errException=Exception("apktool d 失败"))
        BinManager.apktool(f'd {self.zippath} -o {self.unzippath}',errException=Exception("apktool d 失败"))
        self.addclear(self.zippath,self.unzippath)
        
        return self.unzippath
    def zip(self, method=ZipManager.ZIP_STORED):
        repackfile = self.getrepackfile()
        # out,code = com.cmd(f'apktool b {self.unzippath} -o {repackfile}',errException=Exception("apktool b 失败"))
        BinManager.apktool(f'b {self.unzippath} -o {repackfile}',errException=Exception("apktool b 失败"))
        self.addclear(self.unzippath,repackfile)
        self._setzippath(repackfile)
    def removesign(self):
        zf = self.open()
        signfile = []
        for name in zf.namelist():
            if name.startswith('META-INF/'):
                signfile.append(name)
        
        zf.close()

        self.removefile(*signfile)
    def sign(self,keystorepath,v1=True,v2=False):
        keystore = os.path.basename(keystorepath)
        
        conf = Loader.load(android_keystoreconf,keystore)

        signfile = self.zippath.replace('.apk','_signed.apk')
        # 签名
        BinManager.apksigner(f'sign --ks {keystorepath} \
        --v1-signing-enabled {v1.__str__().lower()} --v2-signing-enabled {v2.__str__().lower()} \
        --ks-key-alias {conf.alias} --ks-pass pass:{conf.storepass} --key-pass pass:{conf.keypass} \
        --pass-encoding utf-8 {self.zippath}',getstdout=False,errException=StopException('签名失败',locals(),''))
        #  --out {signfile}
        os.rename(self.zippath,signfile)
        # 验证                        -v 
        BinManager.apksigner(f'verify -Werr --print-certs --min-sdk-version 16 {signfile}',getstdout=False,errException=StopException('签名验证失败',locals(),''))
        
        self.addclear(self.zippath,signfile)
        self._setzippath(signfile)
        return signfile


    def modify_targetSdkVersion(self,targetver)->int:
        oldver, = com.re_replace(r"targetSdkVersion: '(\d{1,2})'",f"targetSdkVersion: '{targetver}'",self.apktoolymlpath)
        if oldver == None:
            raise Exception("targetSdkVersion没找到")
        Log.info(f'[modify_targetSdkVersion] {oldver} -> {targetver}')
        return int(oldver)

    def modify_minSdkVersion(self,targetver)->int:
        oldver, = com.re_replace(r"minSdkVersion: '(\d{1,2})'",f"minSdkVersion: '{targetver}'",self.apktoolymlpath)
        if oldver == None:
            raise Exception("minSdkVersion没找到")
        Log.info(f'[modify_minSdkVersion] {oldver} -> {targetver}')
        return int(oldver)

    def modify_versionCode(self,targetver)->int:
        oldver, = com.re_replace(r"versionCode: '([\d]+)'",f"versionCode: '{targetver}'",self.apktoolymlpath)
        if oldver == None:
            raise Exception("versionCode没找到")
        Log.info(f'[modify_versionCode] {oldver} -> {targetver}')
        return int(oldver)

    def modify_versionName(self,targetvername)->int:
        oldver, = com.re_replace(r"versionName: '(.*)'",f"versionName: '{targetvername}'",self.apktoolymlpath)
        if oldver == None:
            raise Exception("versionName没找到")
        Log.info(f'[modify_versionName] {oldver} -> {targetvername}')
        return oldver
    def add2doNotCompress(self,*abspaths_in_apk:list):
        insertstr = '- '+'- '.join(abspaths_in_apk)
        com.re_replace("doNotCompress:",f"doNotCompress:\n{insertstr}",self.apktoolymlpath)
        Log.info(f'[add2doNotCompress] success')

    def add2lib(self,*path,debug=True):
        libpath = self.getpath_lib()
        if not os.path.exists(libpath):
            
            os.mkdir(libpath)
        com.combinefolder(libpath,*path,debug=debug)
    def add2assets(self,*path,cover=True,debug=False):
        assetspath = self.getpath_assets()
        if not os.path.exists(assetspath):
            os.mkdir(assetspath)
        for p in path:
            basep = os.path.basename(p)
            tarp = os.path.join(assetspath,basep)
            if os.path.exists(tarp):
                if os.path.isfile(tarp):
                    if cover:
                        os.remove()
                        shutil.copy2(p,assetspath)
                elif os.path.isdir(tarp):
                    com.combinefolder(tarp,p,cover=cover,debug=debug)
            else:
                shutil.copytree(p,tarp)
    def getpath_assets(self)->str:
        return os.path.join(self.unzippath,'assets')
    def getpath_lib(self)->str:
        return os.path.join(self.unzippath,'lib')
    def getpath_res(self)->str:
        return os.path.join(self.unzippath,'res')
    def getpaths_mipmap(self)->list:
        return com.listdir_fullpath(self.getpath_res(),lambda x: os.path.basename(x).startswith('mipmap'))
    def getpath_smali(self)->list:
        smalis = filter(lambda x: x.startswith('smali'),os.listdir(self.unzippath))
        fullpath = []
        for s in smalis:
            fullpath.append(os.path.join(self.unzippath,s))
        return fullpath
    def getpath_manifest(self)->str:
        return os.path.join(self.unzippath,'AndroidManifest.xml')
    def getpath_assetbundles(self)->str:
        return os.path.join(self.getpath_assets(),'AssetBundles')



class IPAManager(PackageManager):
    def __init__(self, zippath):
        super().__init__(zippath)

        pt = self.open()
        names = self.getnamelist(pt)
        apppath = None
        for name in names:
            if name.endswith('.app/'):
                apppath = name.rstrip('/')
                break
        self.close()
        
        self.package = Package(f'{apppath}/Data/Raw')
        
        from comlib.thirdparty.bplist import BPListReader
        raw = self.readall(f'{apppath}/Info.plist',mode='rb')
        bplreader = BPListReader(raw)
        try:
            self.plist = bplreader.parse()
        except Exception as ex:
            Log.info("###### plist parse failed !!!")
            
    def get_packagename(self):
        packagename = self.plist['CFBundleName'].decode(encoding='utf-8')
        Log.info(f'[get_packagename] {packagename}')
        return packagename
    def get_versioncode_versionname(self):
        versioncode = self.plist['CFBundleShortVersionString'].decode(encoding='utf-8')
        versionname = self.plist['CFBundleVersion'].decode(encoding='utf-8')
        Log.info(f'[get_versioncode_versionname] versioncode={versioncode} versionname={versionname}')
        return versioncode,versionname
    def get_appname(self):
        appname = self.plist['CFBundleDisplayName']
        Log.info(f'[get_appname] appname={appname}')
        return appname
    def sign(self,bundleid,provision_path,p12filepath,passfilepath):
        # bundleid = self.config_sdk['bundle']
        # provision_path = os.path.expanduser('~')+'/Library/MobileDevice/Provisioning Profiles/%s.mobileprovision'%self.config_sdk['provision']
        # codesing_str = self.config_sdk['codeSign']
        # cmd = com.cmd_builder('sh signer_ios.sh',packpath,bundleid,provision_path,codesing_str)
        from comlib.cerm import PKCS12
        p12 = PKCS12(p12filepath,passfilepath)
        p12.import_mac(provision_path)
        password = com.readall(passfilepath)
        BinManager.ios_resigner(self.zippath,bundleid,provision_path,p12filepath,password,
        getstdout=False,errException=Exception('ios签名失败'),showlog=False)
        

        signfile = self.zippath.replace('.ipa','_signed.ipa')

        self.addclear(self.zippath,signfile)
        self._setzippath(signfile)
        return self.zippath
    

# @errorcatch(HIGH)
class TMApkManager(ApkManager):
    def __init__(self, apkpath):
        super().__init__(apkpath)
        self.package = TMPackage('assets','assets/AssetBundles','assets/sdk.conf')
    @staticmethod
    def getapkinfo(apkpath)->TMApkInfo:
        return TMApkInfo(apkpath)
    def getpath_launchimg(self):
        return os.path.join(self.getpath_res(),'drawable','unity_static_splash.png')
    def getpath_bannerimg(self):
        return os.path.join(self.getpath_res(),'drawable-xhdpi','app_banner.png')
    def sign_auto(self,keystoredir,channel,v1=True,v2=False):
        channel = channel.lower()
        
        keyname,keyconf = Loader.getKeystoreConf(channel)
        curkeystorefile = os.path.join(keystoredir,keyname)
        self.sign(curkeystorefile,v1=v1,v2=v2)
        pass
    def encrypt_with_bangbang_auto(self,secID):
        conf:bangconf = Loader.load(bangconf)
        if conf.usebackground == '10':
            self.encrypt_with_bangbang10(secID)
        elif conf.usebackground == '20':
            self.encrypt_with_bangbang20(secID)
    def encrypt_with_bangbang10(self,secID):
        '''
        return outpath
        '''
        bc = Loader.load(bangconf)
        uploaddir = 'bangup'
        downloaddir = 'bangdown'
        Path.ensure_dirnewest(uploaddir)
        Path.ensure_dirnewest(downloaddir)
        
        uploadzippath = os.path.join(uploaddir,os.path.basename(self.zippath))
        shutil.move(self.zippath,uploadzippath)
        cmd = ' '.join([
        "-i \"%s\"" % (bc.ip + ':' + bc.port),
        "-u \"%s\"" % bc.username,
        "-a \"%s\"" % bc.appkey,
        "-c \"%s\"" % bc.app_password,
        "-t \"%s\"" % secID,
        "-p \"%s\"" % uploaddir,
        "-d \"%s\"" % downloaddir,
        "-f \"%s\"" % bc.reinforce_type,
        "-m 1"  # 不添加时间戳
        ])
        BinManager.bang10(cmd,getstdout=False)

        
        oldsecfilepath = os.path.join(downloaddir,os.listdir(downloaddir)[0])
        
        shutil.move(uploadzippath,self.zippath)
        tsp = com.getlocaltime('-')
        newsecfilepath = self.zippath.replace('.apk',f'_{tsp}_bbid{secID}.apk')
        shutil.move(oldsecfilepath,newsecfilepath)
        Path.ensure_pathnotexsits(uploaddir)
        Path.ensure_pathnotexsits(downloaddir)
        self._setzippath(newsecfilepath)
        
        return newsecfilepath

    def encrypt_with_bangbang20(self,secID):
        '''
        return outpath
        '''

        out,code = BinManager.bang20("--clear",getstdout=False)
        bc = Loader.load(bangconf)
        uploaddir = 'bangup'
        downloaddir = 'bangdown'
        Path.ensure_dirnewest(uploaddir)
        Path.ensure_dirnewest(downloaddir)

        uploadzippath = os.path.join(uploaddir,os.path.basename(self.zippath))
        shutil.move(self.zippath,uploadzippath)
        cmd = ' '.join([
                "-i \"%s\"" % (bc.ip + ':' + bc.port),
                "-u \"%s\"" % bc.username,
                "-c \"%s\"" % bc.password,
                "-s \"%s\"" % secID,
                "-p \"%s\"" % uploaddir,
                "-d \"%s\"" % downloaddir
                ])

        out,code = BinManager.bang20(cmd,getstdout=False)

        oldsecfilepath = os.path.join(downloaddir,os.listdir(downloaddir)[0])
        
        shutil.move(uploadzippath,self.zippath)

        newsecfilepath = self.zippath.replace('.apk',f'_bbid{secID}.apk')
        shutil.move(oldsecfilepath,newsecfilepath)
        Path.ensure_pathnotexsits(uploaddir)
        Path.ensure_pathnotexsits(downloaddir)
        self._setzippath(newsecfilepath)

        return newsecfilepath


    def get_sdkconf(self):
        zf = self.open()
        game = Loader.getgame()
        channel = None
        try:
            if game == 'dnl1.5':
                raise Exception('dnl1.5未实现')
            elif game == 'dnl1.0':
                if self.fileexists(zf,self.package.sdkconfpath):
                    zfs = com.getfilestream4zip(self.zippath,self.package.sdkconfpath,'r')
                    s = zfs.read()
                    channel = s.decode(encoding='utf-8')
        finally:
            zf.close()
        return channel
        
    def get_abmd5(self):
        '''
        return skilldatamd5,tablefbdatamd5,tabledatamd5
        '''

        abpath = self.package.abpath
        skilldatamd5 = ''
        tabledatamd5 = ''
        tablefbdatamd5 = ''
        zf = self.open()

        if self.fileexists(zf,abpath+'/data_skilldata.pck'):
            skilldatamd5 = com.get_md5_fileinzip(self.zippath,abpath+'/data_skilldata.pck')
        if self.fileexists(zf,abpath+'/data_table.pck'):
            tabledatamd5 = com.get_md5_fileinzip(self.zippath,abpath+'/data_table.pck')
        if self.fileexists(zf,abpath+'/data_table_fb.pck'):
            tablefbdatamd5 = com.get_md5_fileinzip(self.zippath,abpath+'/data_table_fb.pck')
        zf.close()
        return skilldatamd5,tablefbdatamd5,tabledatamd5

class TMIPAManager(IPAManager):
    def __init__(self,zippath) -> None:
        super().__init__(zippath)
        pt = self.open()
        names = self.getnamelist(pt)
        apppath:str = None
        for name in names:
            if name.endswith('.app/'):
                apppath = name.rstrip('/')
                break
        self.close()
        self.package = TMPackage(f'{apppath}/Data/Raw',f'{apppath}/Data/Raw/AssetBundles',
        f'{apppath}/Data/Raw/sdk.conf')
    def get_abmd5(self):
        '''
        return skilldatamd5,tablefbdatamd5,tabledatamd5
        '''

        abpath = self.package.abpath
        skilldatamd5 = ''
        tabledatamd5 = ''
        tablefbdatamd5 = ''
        zf = self.open()

        if self.fileexists(zf,abpath+'/data_skilldata.pck'):
            skilldatamd5 = com.get_md5_fileinzip(self.zippath,abpath+'/data_skilldata.pck')
        if self.fileexists(zf,abpath+'/data_table.pck'):
            tabledatamd5 = com.get_md5_fileinzip(self.zippath,abpath+'/data_table.pck')
        if self.fileexists(zf,abpath+'/data_table_fb.pck'):
            tablefbdatamd5 = com.get_md5_fileinzip(self.zippath,abpath+'/data_table_fb.pck')
        zf.close()
        return skilldatamd5,tablefbdatamd5,tabledatamd5


if __name__ == "__main__":
    # zipm = ZipManager(os.path.join('modify','test','app-release_signer.apk'))
    # apkm = ApkManager(os.path.join('modify','test','app-release_signer.apk'))
    # apkm.unzip()
    # apkm.clearpathlist.remove(os.path.join('modify','test','app-release_signer.apk'))

    # try:
    #     assert apkm.modify_minSdkVersion(16) == 21
    #     assert apkm.modify_targetSdkVersion(27) == 28
    #     assert apkm.modify_versionCode(1000) == 1
    #     assert apkm.modify_versionName('1.2.3.4') == '1.0'
    #     print(apkm.getpath_smali())
    #     apkm.add2doNotCompress('assets')
    #     apkm.add2lib(os.path.join('modify','test','lib'),os.path.join('modify','test','lib'))
    #     apkm.add2assets(os.path.join('modify','test','fonts'),os.path.join('modify','test','fonts'))

    # except Exception as e:
    #     # apkm.clear()
    #     raise e
    # apkm.zip()
    # apkm.clear()
    pass