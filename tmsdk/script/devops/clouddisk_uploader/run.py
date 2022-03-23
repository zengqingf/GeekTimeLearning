# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH


from comlib import Baidu
from comlib import HTTPManager
from comlib import workspace
from comlib import com
from comlib.conf.loader import Loader
from comlib.conf.ref import acc
from comlib import Path
from comlib import JsonFile,DictUtil,TMApkManager,TMIPAManager,内网发包群苦工,杨都齐


from baidupcsapi import PCS

import io,pathlib

countpath = os.path.join(thisdir,'count.json')

bdapi = Baidu()



dingname = sys.argv[1]
count = sys.argv[2]
channel = sys.argv[3]
url = sys.argv[4]
filename = url.rsplit('/')[-1]
plat = 'Unkonw'
if filename.endswith('.apk'):
    plat = 'android'
elif filename.endswith('.ipa'):
    plat = 'ios'

@errorcatch
@workspace
def run2():
    titleconf = JsonFile(os.path.join(thisdir,'title.json'))
    nameconf = titleconf.trygetvalue(dingname)

    if nameconf == None:
        # 分享路径
        file_id,path = bdapi.mkdir(f'/游戏更新/{com.timemark}_{dingname}')
        nameconf = {
            '创建时间戳':com.timemark,
            '传包次数':int(count),
            '当前次数':0,
            '分享路径':path,
            '分享路径服务器文件id':file_id
        }
        titleconf.setvalue(dingname,value=nameconf)
    nameconf['当前次数'] += 1
    

    # 下载包
    HTTPManager.download_http(url,filename)
    # 获取包信息
    zipm = com.choose(filename.endswith('.apk'),TMApkManager,TMIPAManager)(filename)
    versioncode,versionname = zipm.get_versioncode_versionname()
    # 生成md5校验文件
    md5 = com.get_md5(filename)
    md5filename = f'md5-{md5}.txt'
    com.savedata(md5,md5filename)
    # baidupcsapi库
    # 包储存路径
    file_id,path = bdapi.mkdir(f'''{nameconf['分享路径']}/{plat}/{channel}/{versionname}''')
    bdapi.upload_normalfile(md5filename,f'''{path}/{md5filename}''')
    if os.path.getsize(filename) >= 2 * 1024 * 1024 * 1024:
        bdapi.upload_superfile(filename,f'''{path}/{filename}''')
    else:
        bdapi.upload_normalfile(filename,f'''{path}/{filename}''')

    
    if nameconf['当前次数'] >= nameconf['传包次数']:
        if not DictUtil.hasKey(nameconf,'分享链接'):
            link,password = bdapi.share([nameconf['分享路径服务器文件id']])
            nameconf['分享链接'] = link
            nameconf['分享链接密码'] = password
        else:
            link = nameconf['分享链接']
            password = nameconf['分享链接密码']
        Ding(timemark=nameconf['创建时间戳'],versioncode=versioncode,versionname=versionname,LANUrl=url,WANUrl=link,password=password)
    titleconf.save(svncommit=False)


def Ding(timemark,versioncode,versionname,LANUrl,WANUrl,password=None):
    '''
    LANUrl : 内网下载\n
    WANUrl : 外网下载\n
    '''
    title = dingname
    content = 内网发包群苦工.markdown_textlevel(内网发包群苦工.markdown_text_weightORitalic(f'{timemark}_{title}','weight'),2)   + '\n'
    content += 内网发包群苦工.markdown_drawline()
    listcontent = com.textwrap.dedent(f'''
    平台:{plat}
    渠道:{channel}
    版本代码:{versioncode}
    版本名:{versionname}
    ''')
    content += 内网发包群苦工.markdown_textlist_2(listcontent) +'\n'
    wanpandesc = f'百度网盘下载-密码：{password}'
    neiwangdesc = f'内网下载'

    btns={
        wanpandesc:内网发包群苦工.getOutUrl(WANUrl),
        neiwangdesc:内网发包群苦工.getOutUrl(LANUrl)
    }
    data = 内网发包群苦工.build_actioncard_mult(title,content,hideAvatar=True,**btns)
    atdata = 内网发包群苦工.build_text(title,杨都齐)
    内网发包群苦工.send(atdata)
    内网发包群苦工.send(data)
    pass


# @errorcatch
# @workspace
# def run_upload():
#     dingname = sys.argv[2]
#     channel = sys.argv[3]
#     url = sys.argv[4]
#     filename = url.rsplit('/')[-1]
#     plat = 'Unkonw'
#     if filename.endswith('.apk'):
#         plat = 'android'
#     elif filename.endswith('.ipa'):
#         plat = 'ios'
    
#     # 下载包
#     HTTPManager.download_http(url,filename)
#     # 生成md5校验文件
#     md5 = com.get_md5(filename)
#     md5filename = f'md5-{md5}.txt'
#     com.savedata(md5,md5filename)
#     # 上传
#     accconf = Loader.load(acc)
#     baidu = BaiduPCS(accconf.baidu_bduss,accconf.baidu_ptoken,accconf.baidu_stoken)
#     fromlist = [filename,md5filename]
#     topath = f'/{dingname}/{plat}/{channel}'
#     baidu.upload(fromlist,topath,parallelCount=8,retryCount=10,miaochuan=False)
#     # 钉钉通知
#     if canDing(dingname):
#         Ding()
#     pass
# def canDing(dingname):
#     jsf = JsonFile(countpath)
#     count,err = jsf.trygetvalue(dingname)
#     if err != None:
#         return True
#     count = int(count) - 1
#     jsf.trysetvalue(count)
#     jsf.save()

#     if count <= 0:
#         return True
#     else:
#         return False

def run_set():
    dingname = sys.argv[2]
    count = sys.argv[3]

    jsf = JsonFile(countpath)
    

if __name__ == "__main__":


    run2()

    # Ding('123','1.2.3.4','http://www.baidu.com','http://www.baidu.com','1234')


    # a = {}
    # DictUtil.setvalue(a,'asdf',value=22)
    # print(a)
    
    # runtype = sys.argv[1]
    # if runtype == 'upload':
    #     run_upload()
    # elif runtype == 'set':
    #     run_set()