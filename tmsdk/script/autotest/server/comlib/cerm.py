# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,errorcatch_func,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com
from comlib.conf.loader import Loader
from comlib.conf.ref import *
from comlib.binm import BinManager
from comlib.xmlm import XMLFile


import OpenSSL
import re

from comlib.comobjm import *

class PKCS12():
    def __init__(self,filepath,passfile) -> None:
        self.filepath = filepath
        self.passfile = passfile
        self.password = com.readall(passfile)
    def load(self):
        data = open(self.filepath,'rb').read()
        
        self.p12 = OpenSSL.crypto.load_pkcs12(data,self.password.encode())

        self.cer = self.p12.get_certificate()     # (signed) certificate object
        self.pkey = self.p12.get_privatekey()      # private key.
        self.ca_cer = self.p12.get_ca_certificates() # ca chain.

        self.has_expired = self.cer.has_expired() # 是否过期

        subject = self.cer.get_subject()
        s_components = subject.get_components()

        self.subject_userid = None
        self.subject_username = None
        self.subject_companyid = None
        self.subject_companyname = None
        self.subject_country = None
        for key, value in s_components:
            value = value.decode()
            if key == b'UID':
                self.subject_userid = value
            if key == b'CN':
                self.subject_username = value
            if key == b'OU':
                self.subject_companyid = value
            if key == b'O':
                self.subject_companyname = value
            if key == b'C':
                self.subject_country = value
            
        issuer = self.cer.get_issuer()
        
        # self.issuer_userid = None
        self.issuer_username = None
        self.issuer_companyid = None
        self.issuer_companyname = None
        self.issuer_country = None
        
        for key,value in issuer.get_components():
            value = value.decode()
            # if key == b'UID':
            #     self.issuer_companyid = value
            if key == b'CN':
                self.issuer_username = value
            if key == b'OU':
                self.issuer_companyid = value
            if key == b'O':
                self.issuer_companyname = value
            if key == b'C':
                self.issuer_country = value
        Log.debug(com.textwrap.dedent(f'''
        subject_userid={self.subject_userid}
        subject_username={self.subject_username}
        subject_companyid={self.subject_companyid}
        subject_companyname={self.subject_companyname}
        subject_username={self.subject_country}

        issuer_username={self.issuer_username}
        issuer_companyid={self.issuer_companyid}
        issuer_companyname={self.issuer_companyname}
        issuer_country={self.issuer_country}

        '''))

    def import_mac(self,mobileprovisionfile):
        '''
        (mac)导入p12证书
        '''
        
        user,loginpassword = Loader.获取登陆信息()
        BinManager.importp12(self.filepath,self.password,mobileprovisionfile,loginpassword,errException=Exception('导入p12证书失败'))
        
    def delete_mac(self,mobileprovisionfile,loginpassfile):
        '''
        (mac)删除p12
        '''
        pass
    @staticmethod
    def getUUID_mpfile(mobileprovisionfile):
        # mp文件data块解析
        # MobileProvisionCertificateName=`openssl smime -inform der -verify -noverify -in "$2" | \
        # xmlstarlet sel -t -v "/plist/dict/key[. = 'DeveloperCertificates']/following-sibling::array[1]/data[1]" | \
        # awk '{print $1}' | sed '/^$/d' | base64 -d | openssl x509 -subject -inform der | head -n 1 | \
        # sed 's/\(.*\)\/CN=\(.*\)\/OU=\(.*\)/\2/g'`
        out,code = com.cmd(f'openssl smime -inform der -verify -noverify -in "{mobileprovisionfile}"',getstdout=True,errException=Exception('解析mp文件失败'))
        
        xmlf = XMLFile(out)
        dicte = xmlf.find(xmlf.root,'dict')
        isfind = False
        uuid = None
        for sube in dicte.iter():
            if isfind:
                uuid = sube.text
                break
            if sube.text == 'UUID':
                isfind = True
        return uuid
    def getfingerprint_sha1(self):
        '''
        获取p12 sha1指纹
        '''
        # 获取p12 sha1指纹
        # openssl pkcs12 -in xxx -nodes -passin pass:"" | openssl x509 -noout -fingerprint

        # MAC verified OK
        # SHA1 Fingerprint=a1821ae72a229eac73969d50337ce60e8ceedfe3
        out,code = com.cmd(f'openssl pkcs12 -in {self.filepath} -nodes -passin pass:"{self.password}" | openssl x509 -noout -fingerprint',getstdout=True,errException=Exception('获取p12 sha1指纹失败'))
        m = re.search('Fingerprint=(.*)',out)
        sha1:str = m.group(1).replace(':','')
        return sha1

# print(PKCS12.getUUID_mpfile(''))
# p = r'E:\__Project\1.5\buildtool\keystore\_ios_appstore\channels\_tengmu\dev\tengmuV1-20200420.p12'
# data = open(p,'rb').read()
# p12 = OpenSSL.crypto.load_pkcs12(data,'')

# cer = p12.get_certificate()     # (signed) certificate object
# pkey = p12.get_privatekey()      # private key.
# ca_cer = p12.get_ca_certificates() # ca chain.
# print(cer, pkey, ca_cer)
 
# print('版本', cer.get_version())
# print('签名算法', cer.get_signature_algorithm())
# print('序列号：', cer.get_serial_number())
# print('证书是否过期：', cer.has_expired())
# print('在此之前无效：', cer.get_notBefore())
# print('在此之后无效', cer.get_notAfter())
 
 
 
# #主题名称
# subject = cer.get_subject()
# s_components = subject.get_components()
# print(s_components)
 
# key_dict = {'UID': '用户 ID',
# 			'CN': '常用名称',
# 			'OU': '组织单位',
# 			'O': '组织',
# 			'C': '国家或地区'
# 			}


# name = None
# for key, value in s_components:
#     print(key, value)
#     if key == b'CN':
#         name = value.decode()
#     print(key_dict.get(key.decode(), key))
 
# #签发者名称
# suer = cer.get_issuer()
# print(suer.get_components())
 
# #证书扩展信息
# print('扩展数：', cer.get_extension_count())
# print('扩展1：', cer.get_extension(0))
# print(name)
