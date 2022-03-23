# -*- coding:utf-8 -*-

import os,re,sys
sys.path.append(os.path.abspath(os.path.join(__file__,'..','..')))
from comlib import com
import urllib.request as rq
import requests
from comlib.com import logout
from comlib.exception import errorcatch,LOW,NORMAL,HIGH


import datetime


@errorcatch(HIGH)
class HTTPManager(object):
    @staticmethod
    def getUrlFileName(url):
        return url.split('/')[-1]
    @staticmethod
    def downloadfile(local,url):
        '''
            不需要模块，只能下载小文件
        '''
        logout('开始下载 %s \n到 %s'%(url,local))
        rq.urlretrieve(url,local)
        # chunk = f.read()
        
        # with open(local,"wb") as f:
        #     while chunk != b'':
        #         f.write(chunk)
        #         chunk = f.read()
    @staticmethod
    def download_http(url,filepath):
        '''
            下载大文件,需要模块requests
        '''
        if os.path.isdir(filepath):
            filepath = os.path.join(filepath, url.split('/')[-1])
        r = requests.get(url, stream=True, verify=False)
        f = open(filepath, "wb")
        logout('开始下载 %s \n到 %s'%(url,filepath))
        for chunk in r.iter_content(chunk_size=2048):
            if chunk:
                f.write(chunk)
        f.close()
        logout('下载完成！！！')
        return filepath
    @staticmethod
    def gettsp_BeiJin():
        tsp = com.gettsp()
        try:
            res = requests.get('https://www.tsa.cn/time.jspx') # 国家授时中心标准时间
            if res.ok:
                tsp = res.content.decode(encoding='utf8')
            else:
                logout("https://www.tsa.cn/time.jspx 请求失败 %s"%res.reason)
        except Exception as e:
            pass
        finally:
            return tsp
    @staticmethod
    def getwebtime(divide='_')->str:
        tsp = HTTPManager.gettsp_BeiJin()
        date = datetime.fromtimestamp(int(tsp) / 1000)
        return '%d%s%02d%02d%s%02d%02d%02d' % (date.year,divide,date.month,date.day,divide,date.hour,date.minute,date.second)

    @staticmethod
    def getbody(url,header=None,decode=None):
        # res = requests.get(url,headers=header)

        if header != None:
            res = requests.get(url,headers=header)
        else:
            res = requests.get(url)
        if res.ok:
            if decode == 'json':
                return res.json()
            else:
                return res.content.decode(res.encoding)
            
        else:
            logout(url+" 请求失败 %s"%res.reason)


