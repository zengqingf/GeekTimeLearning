# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,errorcatch_func,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace,Dingrobot,发包群更新机器人,内网发包群苦工,杨都齐,尹宗都,TMApkManager,JenkinsHelper,HTTPManager,FTPManager,Path
from comlib.conf.loader import Loader
from comlib.conf.ref import *

from comlib import JsonFile

import subprocess,zipfile
import json
import os,sys,re,argparse,shutil

apkroot = os.path.join(workdir,'apkroot')
phpdatapath = os.path.join(thisdir,'phpdemo','data.json')
workdatapath = os.path.join(workdir,'data.json')
accpath = os.path.join(thisdir,'acc.json')


def ios_signer(packpath,bundleid,provision_path,codesing_str)->str:
    cmd = com.cmd_builder('sh signer_ios.sh',packpath,bundleid,provision_path,codesing_str)
    com.cmd(cmd,getstdout=False)
    return packpath.replace('.ipa','_signed.ipa')

class LBInterface(object):
    def __init__(self,plat,account,password):
        self.plat = plat
        self.account = account
        self.password = password
        self.valpath = 'val.json'

    def makefenbao(self,zhengbao_filepath,md5):
        fenbao_url = ''
        # php脚本上传整包
        cmd = com.cmd_builder('php phpdemo/demo1.php',self.plat,self.account,self.password,zhengbao_filepath,md5)
        com.cmd(cmd,getstdout=False)
        if not os.path.exists(self.valpath):
            raise Exception('不存在分包信息')
        with open(self.valpath,'r') as f:
            js = json.load(f)
            fenbao_url = js['fenbao_url']
        os.remove(self.valpath)
        return fenbao_url
    def uploadregeng(self,apkpath,ClientChId,packId):
        # ClientChId = self.getClientChId(apkpath)
        force = 1
        install = 1
        jf = JsonFile(os.path.join(thisdir,'lb.json'))
        if ClientChId in jf.trygetvalue('noinstall'):
            install = 0
        # php脚本上传热更包
        cmd = com.cmd_builder(f'php {os.path.join(thisdir,"phpdemo","demo13.php")}','uploadregeng',self.account,self.password,
        ClientChId,packId,apkpath,force.__str__(),install.__str__())
        
        ret,code = com.cmd(cmd,getstdout=False)
        
        jf = JsonFile(phpdatapath)
        val = {
            'subchid': jf.trygetvalue('subchid'),
            'cpmeta': jf.trygetvalue('cpmeta'),
            'chname': jf.trygetvalue('chname')
        }
        Path.ensure_pathnotexsits(phpdatapath)

        if code != 0:
            data = 内网发包群苦工.build_text(f'{apkpath} 上传失败了',杨都齐)
            内网发包群苦工.send(data)
            return None,False
        return val,True
        # "mg_lebian2021_gradletest"
        # rrr = r'py .\lebianuploader.py uploadregeng android rgx d:\_WorkSpace\_sdk\SDK_Projects\2.0\UnitySDK2.0\20191228_150617_AndroidChannelTest_v97.0_MG.apk '
    def predownload(self,m_subchid):
        cmd = com.cmd_builder(f'php {os.path.join(thisdir,"phpdemo","demo13.php")}','predownload',self.account,self.password,m_subchid)
        com.cmd(cmd,getstdout=False,errException=StopException('预下载失败了',locals()))

    def release(self,m_subchid):
        cmd = com.cmd_builder(f'php {os.path.join(thisdir,"phpdemo","demo13.php")}','release',self.account,self.password,m_subchid)
        com.cmd(cmd,getstdout=False,errException=StopException('上线失败了',locals()))

class LBHelper(object):
    def __init__(self):
        self.plat = sys.argv[2]
        self.tp = sys.argv[3]

        # with open('acc.json','r') as f:
        #     js = json.load(f)
        #     self.account = js[self.plat+'_'+self.tp]['account']
        #     self.password = js[self.plat+'_'+self.tp]['password']
        jf = JsonFile(accpath)
        self.account = jf.trygetvalue(self.plat+'_'+self.tp,'account')
        self.password = jf.trygetvalue(self.plat+'_'+self.tp,'password')
        self.lbi = LBInterface(self.plat,self.account,self.password)


    def makefenbao_init(self):

        
        # self.plat = sys.argv[2]
        # self.tp = sys.argv[3]
        self.mode = sys.argv[4]

        self.sdk = sys.argv[5]
        self.zhengbao_url = sys.argv[6]
        self.md5 = sys.argv[7]
        self.next = sys.argv[8]
        self.splitsdklist = sys.argv[9]
        self.encrpy = sys.argv[10]
        self.buildtime_baidu = sys.argv[11]
        self.buildtitledesc_baidu = sys.argv[12]
        self.buildchannelcount_baidu = sys.argv[13]


        # with open('acc.json','r') as f:
        #     js = json.load(f)
        #     self.account = js[self.plat+'_'+self.tp]['account']
        #     self.password = js[self.plat+'_'+self.tp]['password']

        # self.lbi = LBInterface(self.plat,self.account,self.password)

        self.filename = self.zhengbao_url.split('/')[-1]
        self.apkroot = os.path.join(os.getcwd(),'../apkroot') # jenkins工程根目录
        self.androidkeystore = os.path.join(os.getcwd(),'../../keystore/hegu.keystore') # jenkins工程根目录

        self.zhengbao_filepath = os.path.join(self.apkroot,self.filename)
        self.fenbao_filename = self.filename.replace('.apk','_smallpack.apk').replace('.ipa','_smallpack.ipa')
        
        self.fenbao_filepath = os.path.join(self.apkroot,self.fenbao_filename)
        
        self.needsplit=False
        if self.splitsdklist != 'None':
            self.needsplit = True

        com.logout('[apkroot]'+self.apkroot)
        com.logout('[zhengbao_filepath]'+self.zhengbao_filepath)
        com.logout('[fenbao_filepath]'+self.fenbao_filepath)


        if not os.path.exists(self.apkroot):
            com.buildfolder_tree(self.apkroot)

        self.config = {}
    
        with open(os.path.join('..','..','_tool','config.json'),'rb') as f:
            self.config = json.load(f)
            self.config_sdk = self.config['MainScript']['bundleid'][self.sdk]

        # ftp
        if self.mode == 'debug':
            self.ftpconfig = self.config['FTPUpload_Dev']
        else:
            self.ftpconfig = self.config['FTPUpload']

        self.remote_basepath = self.config['FTPUploadRoot'][self.mode][self.plat][self.sdk]
        self.remote_filepath = self.remote_basepath+'/'+self.fenbao_filename

        self.ftp_url = 'http://%s/%s/'%(self.ftpconfig['ip'],self.ftpconfig['type'])+self.remote_filepath


    def makefenbao(self):
        self.makefenbao_init()

        # 下内网整包

        HTTPManager.download_http(self.zhengbao_url,self.zhengbao_filepath)

        if self.md5 == 'None':
            self.md5 = com.get_md5(self.zhengbao_filepath)
            abpath = ''
            if self.plat == 'android':
                abpath = 'assets/AssetBundles'
            elif self.plat == 'ios':
                abpath = 'Payload/test.app/Data/Raw/AssetBundles'
            else:
                raise Exception('非法平台 '+ self.plat)
            self.skilldatamd5 = com.get_md5_fileinzip(self.zhengbao_filepath,abpath+'/data_skilldata.pck')
            self.tabledatamd5 = com.get_md5_fileinzip(self.zhengbao_filepath,abpath+'/data_table.pck')
            self.tablefbdatamd5 = com.get_md5_fileinzip(self.zhengbao_filepath,abpath+'/data_table_fb.pck')
        
        fenbao_url = self.lbi.makefenbao(self.zhengbao_filepath,self.md5)
        # 下乐变分包
        HTTPManager.download_http(fenbao_url,self.fenbao_filepath)
        

        # ios 签名
        if self.plat == 'ios':
            bundleid = self.config_sdk['bundle']
            provision_path = os.path.expanduser('~')+'/Library/MobileDevice/Provisioning Profiles/%s.mobileprovision'%self.config_sdk['provision']
            codesing_str = self.config_sdk['codeSign']

            fenbao_signed_filepath = ios_signer(self.fenbao_filepath,bundleid,provision_path,codesing_str)

            # 上传分包到内网
            mftp = FTPManager(self.ftpconfig['ip'],self.ftpconfig['port'],self.ftpconfig['username'],self.ftpconfig['password'])
            mftp.uploadfile(fenbao_signed_filepath,self.remote_filepath)
            mftp.close()
            os.remove(fenbao_signed_filepath)
        else:
            self._sign_apk_file(self.fenbao_filepath,self.androidkeystore,"123456","123456","hegu")
            # 上传分包到内网
            mftp = FTPManager(self.ftpconfig['ip'],self.ftpconfig['port'],self.ftpconfig['username'],self.ftpconfig['password'])
            mftp.uploadfile(self.fenbao_filepath,self.remote_filepath)
            mftp.close()
            uploadtobaidu = 'true'
            if 'None' == self.buildtime_baidu and 'None' == self.buildtitledesc_baidu:
                uploadtobaidu = 'false'


            if self.next == 'bb':
                cid = 10
                try:
                    cid = self.config_sdk['bangcleSecId']
                except Exception as e:
                    cid = 10

                jenkins_params = JenkinsHelper.buildparams(SDK=self.sdk,Tacticsid=cid,ApkName=self.ftp_url,
                BuildTime=self.buildtime_baidu,BuildTitleDesc=self.buildtitledesc_baidu,
                BuildChannelCount=self.buildchannelcount_baidu,UploadToBaidu=uploadtobaidu)
                JenkinsHelper.build('DNF_Android_BB_Upload_247',jenkins_params)
            elif self.next == 'split':
                m = re.search('1.[0-9]{2}.1.[0-9]{6}',self.filename)
                if m == None:
                    m = re.search('1_[0-9]{2}_1_[0-9]{6}',self.filename)
                    if m == None:
                        raise Exception("包名不存在完整版本号")
                ver = m.group(0).replace('_','.')
                if self.needsplit:
                    # 分包工程
                    jenkins_params = JenkinsHelper.buildparams(SDKList=self.splitsdklist,ApkName=self.ftp_url,VersionName=ver,VersionInGame=ver,
                    EncrpyWithBangbang=self.encrpy,BuildTime=self.buildtime_baidu,BuildTitleDesc=self.buildtitledesc_baidu,
                    BuildChannelCount=self.buildchannelcount_baidu,UploadToBaidu=uploadtobaidu)
                    JenkinsHelper.build('DNF_Android_Split_248',jenkins_params)
                elif self.encrpy == 'true':
                    cid = 10
                    try:
                        cid = self.config_sdk['bangcleSecId']
                    except Exception as e:
                        cid = 10
                    jenkins_params = JenkinsHelper.buildparams(SDK=self.sdk,Tacticsid=cid,ApkName=self.ftp_url,
                    BuildTime=self.buildtime_baidu,BuildTitleDesc=self.buildtitledesc_baidu,
                    BuildChannelCount=self.buildchannelcount_baidu,UploadToBaidu=uploadtobaidu)
                    JenkinsHelper.build('DNF_Android_BB_Upload_247',jenkins_params)
                else:
                    # 通知工程
                    self.Ding()
            else:
                # 非法也通知
                self.Ding()

        # TODO

        # 清理包
        os.remove(self.zhengbao_filepath)
        os.remove(self.fenbao_filepath)
    
    
    def uploadregeng_init(self):
        rawpath = sys.argv[4]
        
        fl = filter(lambda x : x not in (None,''), rawpath.split('\n'))
        self.apkpaths = [p for p in fl]
        print('此次共上传 %s 个包'%self.apkpaths.__len__(),flush=True)
        print('此次共上传 %s 个包'%self.apkpaths.__len__(),flush=True)
        print('此次共上传 %s 个包'%self.apkpaths.__len__(),flush=True)
        print('此次共上传 %s 个包'%self.apkpaths.__len__(),flush=True)
        print('此次共上传 %s 个包'%self.apkpaths.__len__(),flush=True)
        print('此次共上传 %s 个包'%self.apkpaths.__len__(),flush=True)
        pass
    def uploadregeng(self):
        self.uploadregeng_init()
        dingmsg = []
        md5 = []
        
        Path.ensure_dirnewest(apkroot)

        for apk in self.apkpaths:
            com.logout('---------------------------------------------------------')
            com.logout('---------------------------------------------------------')
            com.logout('---------------------------------------------------------')
            com.logout('现在传的是')
            com.logout(apk)

            saveapk = HTTPManager.download_http(apk,apkroot)
            zm = TMApkManager(saveapk)
            
            lb_chid = zm.get_metadata('ClientChId')
            packId = zm.get_packagename()

            sdkconf = zm.get_sdkconf()
            if sdkconf == None:
                sdkconf = lb_chid
            versioncode,versionname = zm.get_versioncode_versionname()
            
            if dingmsg.__len__() == 0:
                dingmsg.append([sdkconf,versioncode,versionname])
            else:
                find = False
                for c in dingmsg:
                    if c[1] == versioncode and c[2] == versionname:
                        c[0] += ' | %s'%sdkconf
                        find = True
                        break
                if not find:
                    dingmsg.append([sdkconf,versioncode,versionname])


            skilldatamd5,tablefbdatamd5,tabledatamd5 = zm.get_abmd5()

            if md5.__len__() == 0:
                md5.append([sdkconf,skilldatamd5,tablefbdatamd5])
            else:
                find = False
                for c in md5:
                    if c[1] == skilldatamd5 and c[2] == tablefbdatamd5:
                        c[0] += ' | %s'%sdkconf
                        find = True
                        break
                if not find:
                    md5.append([sdkconf,skilldatamd5,tablefbdatamd5])
                    
            zm.close()
            ret,isOk = self.lbi.uploadregeng(saveapk,lb_chid,packId)
            if isOk:
                ret['sdk'] = sdkconf
                data = JsonFile(workdatapath)
                data.setvalue(ret['subchid'],value=ret)
                data.save()
        
        content = Dingrobot.markdown_textquote(Dingrobot.markdown_textlevel('乐变上传更新提醒',1))
        content += '------------------------------\n'
        content += Dingrobot.markdown_textlevel('此次上传渠道',3)
        content += '------------------------------\n'
        sortmsg=[]
        for x in dingmsg:
            x[0] = '渠道ID=%s'%Dingrobot.markdown_text_weightORitalic(x[0],'weight')+ '\n'
            x[1] = 'VersionCode=%s'%Dingrobot.markdown_text_weightORitalic(x[1],'weight')+ '\n'
            x[2] = 'VersionName=%s'%Dingrobot.markdown_text_weightORitalic(x[2],'weight')+ '\n'
            # content += Dingrobot.markdown_textquote(Dingrobot.markdown_textlist(*dingmsg))
            sortmsg.append('\n'.join(x) + '\n')

        content += Dingrobot.markdown_textquote(Dingrobot.markdown_textlist(*sortmsg,issort=True))
        content += '------------------------------\n'

        content += Dingrobot.markdown_textlevel('此次MD5',3)
        content += '------------------------------\n'
        for x in md5:
            x[0] = '渠道ID=%s'%Dingrobot.markdown_text_weightORitalic(x[0],'weight') #+ '\n'
            x[1] = 'Skill=%s'%Dingrobot.markdown_text_weightORitalic(x[1],'weight') #+ '\n'
            x[2] = 'FBTab=%s'%Dingrobot.markdown_text_weightORitalic(x[2],'weight')# + '\n'
            # x[3] = 'Table=%s'%Dingrobot.markdown_text_weightORitalic(x[3],'weight')# + '\n'
            content += Dingrobot.markdown_textquote(Dingrobot.markdown_textlist(*x))
            # content += '\n'.join(x)+'\n'
        content += '------------------------------\n'

        data,atdata = Dingrobot.build_markdown('乐变上传更新提醒',content,尹宗都)
        # 内网发包群苦工.send(data)

        robot = Loader.获取发包对接群机器人()
        robot.send(data)
        robot.send(atdata)


        pass
    def predownload(self):
        # $params['m_subchid'] = $ps[0]; //渠道对应的subchid，多个渠道用+号连接 这里在python直接处理好

        data = JsonFile(workdatapath)
        m_subchid = []
        for k,v in data.getkv():
            '''
            subchid : {
                'sdk'
                'subchid'
                'cpmeta'
                'chname'
            }
            '''
            m_subchid.append(v['subchid'])
            pass
        m_subchid_str = '+'.join(m_subchid)
        self.lbi.predownload(m_subchid_str)
    def release(self):
        # $params['m_subchid'] = $ps[0]; //渠道对应的subchid，多个渠道用+号连接 这里在python直接处理好

        data = JsonFile(workdatapath)
        m_subchid = []
        for k,v in data.getkv():
            '''
            subchid : {
                'sdk'
                'subchid'
                'cpmeta'
                'chname'
            }
            '''
            m_subchid.append(v['subchid'])
            pass
        m_subchid_str = '+'.join(m_subchid)
        self.lbi.release(m_subchid_str)
        
        Path.ensure_pathnotexsits(workdatapath)

    def Ding(self):
        jenkins_params = JenkinsHelper.buildparams(Title=self.fenbao_filename,Url=self.ftp_url,Desc=self.fenbao_filename,MD5=self.md5,
        SkillMD5=self.skilldatamd5,TableMD5=self.tabledatamd5,FBTableMD5=self.tablefbdatamd5,
        IOSServerListRoleSave='None',IOSUpdateServerUrl='None')

        JenkinsHelper.build('DNF_Robot_Banma',jenkins_params)
    
    def _sign_apk_file(self, file_path,_keystore_path,_keystore_password,_alias_password,_alias):
        cmd = ' '.join([
                "jarsigner",
                "-digestalg SHA1 -sigalg MD5withRSA",
                "-keystore \"%s\"" % _keystore_path,
                "-storepass \"%s\"" % _keystore_password,
                "-keypass \"%s\"" % _alias_password,
                "-signedjar \"%s\" \"%s\" \"%s\"" % (file_path, file_path, _alias)])
        com.cmd(cmd,getstdout=False)
    # def getClientChId(self,apkpath):
    #     # if sys.platform == 'win32':
    #     #     aaptp = 'bin\\win\\aapt.exe'
    #     # elif sys.platform == 'darwin':
    #     #     aaptp = 'bin/mac/aapt'
    #     #     com.cmd('chmod 755 '+aaptp)
    #     aaptp = com.getbinpath('aapt','..')
    #     out,code = com.cmd('%s dump xmltree %s AndroidManifest.xml'%(aaptp,apkpath))
    #     lines = out.split('\n')
    #     for i in range(0,lines.__len__()):
    #         if 'ClientChId' in lines[i]:
    #             m = re.search('"(.*?)"',lines[i+1])
    #             if m == None:
    #                 return
    #             print(m.group(1))
    #             return m.group(1)


if __name__ == "__main__":
    lbh = LBHelper()
    func = sys.argv[1]
    
    if com.isalpha(func):
        eval('lbh.%s'%func)()
    


    


    
