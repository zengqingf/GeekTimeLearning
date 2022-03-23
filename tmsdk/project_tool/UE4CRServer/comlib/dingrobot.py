# -*- encoding: utf-8 -*-
import sys,os,json
sys.path.append(os.path.abspath(os.path.join(__file__,'..','..')))

from comlib.com import logout,cmd,cmd_builder
from comlib.httpm import HTTPManager

from comlib.exception import errorcatch,LOW,NORMAL,HIGH


import requests

import time
import hmac
import hashlib
import base64
from urllib import parse


# @errorcatch(HIGH)
# https://ding-doc.dingtalk.com/doc#/serverapi3/iydd5h/RkjDb
class Dingrobot(object):
    format_url="dingtalk://dingtalkclient/page/link?url={}"
    # &pc_slide=false
    def __init__(self,webhook,secret_key):
        self.webhook = webhook
        self.secret_key = secret_key

        self.url_format='%s&timestamp=%s&sign=%s'
        # '-H "Content-Type: application/json;charset=UTF-8" '\
        # "-d '%s'"
        
    def sign(self):
        timestamp = HTTPManager.gettsp_BeiJin()
        secret = self.secret_key
        secret_enc = secret.encode('utf-8')
        string_to_sign = '{}\n{}'.format(timestamp, secret)
        string_to_sign_enc = string_to_sign.encode('utf-8')
        hmac_code = hmac.new(secret_enc, string_to_sign_enc, digestmod=hashlib.sha256).digest()
        sign = parse.quote_plus(base64.b64encode(hmac_code))
        # logout(timestamp)
        # logout(sign)
        return timestamp,sign
    def send(self,data):
        from comlib.conf.loader import Loader
        from comlib.conf.ref import globalconf
        from comlib.comobj import G_ip
        gc = Loader.load(globalconf,use_defalt=True)
        if gc.dingserver not in (None,'') and not gc.dingserver.startswith(G_ip):
            from comlib.clientlib.help import NetHelper
            net = NetHelper(gc.dingserver)
            ret,isOk = net.postdata('dingding/api',{'webhook':self.webhook,'secret_key':self.secret_key,'msg':data},'sendmsg')
        else:
            # logout(data)
            timestamp,sign = self.sign()
            header = {'Content-Type': 'application/json;charset=UTF-8'}

            url = self.url_format%(self.webhook,timestamp,sign)
            # ip 106.11.43.160:443
            # ssh -N -L 50000:106.11.43.160:443 hegu@192.168.2.148
            # import subprocess
            # p = subprocess.Popen('ssh -N -L 50000:106.11.43.160:443 hegu@192.168.2.148')
            # url = url.replace('oapi.dingtalk.com/robot/send','106.11.6.1:443')

            res = requests.post(url,data=data,headers=header)
            # print(res.text)
            # print(res.url)
    @staticmethod
    def build_text(content,*atMobiles,isAtAll=False) ->str:
        '''
        @只会在这里生效
        '''
        strbuilder = {
            "msgtype": "text", 
            "text": {
                "content": content
            }, 
            "at": {
                "atMobiles": atMobiles, 
                "isAtAll": isAtAll
            }
        }
        return json.dumps(strbuilder)
    @staticmethod
    def build_link(title,text,messageUrl,picUrl=None) ->str:
        if picUrl == None:
            picUrl = 'https://dss0.bdstatic.com/-0U0bnSm1A5BphGlnYG/tam-ogel/a76456f4d38dd624f0eb085745ea60f0_121_121.jpg'
        strbuilder = {
            "msgtype": "link", 
            "link": {
                "title": title, 
                "text": text, 
                "messageUrl": messageUrl,
                "picUrl": picUrl,
            }
        }
        return json.dumps(strbuilder)
    @staticmethod
    def build_markdown(title,text_markdown,*atMobiles,isAtAll=False) ->str:
        '''
        添加@的时候return content,json.dumps(strbuilder)
        无@ return json.dumps(strbuilder)
        '''
        # ！！！注意：\\_会被钉钉转化成_，要显示\_必须得变成\\\\_！！！
        text_markdown = Dingrobot.std_markdowncontent(text_markdown)
        strbuilder = {
            "msgtype": "markdown",
            "markdown": {
                "title":title,
                "text": text_markdown
            },
            "at": {
                "atMobiles": atMobiles, 
                "isAtAll": isAtAll
            }
        }
        if atMobiles.__len__() != 0 or isAtAll == True:
            content = Dingrobot.build_text(title,*atMobiles,isAtAll=isAtAll)
            return content,json.dumps(strbuilder)
        return json.dumps(strbuilder)
    @staticmethod
    def build_actioncard_single(title,text_markdown,singleTitle,singleURL,hideAvatar=False,btnOrientation=False) ->str:
        text_markdown = Dingrobot.std_markdowncontent(text_markdown)
        strbuilder = {
            "msgtype": "actionCard",
            "actionCard": {
                "title": title, 
                "text": text_markdown, 
                "hideAvatar": int(hideAvatar), 
                "btnOrientation": int(btnOrientation), 
                "singleTitle" : singleTitle,
                "singleURL" : singleURL
            }
        }
        return json.dumps(strbuilder)
    @staticmethod
    def build_actioncard_mult(title,text_markdown,hideAvatar=False,btnOrientation=False,**btscontent) ->str:
        text_markdown = Dingrobot.std_markdowncontent(text_markdown)
        btns = []
        
        for k,v in btscontent.items():
            btns.append({'title':k,'actionURL':Dingrobot.getOutUrl(v)})
        strbuilder = {
            "msgtype": "actionCard",
            "actionCard": {
                "title": title, 
                "text": text_markdown, 
                "hideAvatar": int(hideAvatar), 
                "btnOrientation": int(btnOrientation), 
                "btns": btns
            }
        }
        return json.dumps(strbuilder)
    @staticmethod
    def build_feedcard(*links) ->str:
        strbuilder = {
            "msgtype": "feedCard",
            "feedCard": {
                "links": links
            }
        }
        return json.dumps(strbuilder)
    @staticmethod
    def feedcard_linkbuilder(title,messageURL,picURL) ->dict:
        return {'title':title,'messageURL':messageURL,'picURL':picURL}
    @staticmethod
    def markdown_textlevel(text:str,level:int) ->str:
        return '#'*level+' '+text+'\n'
    @staticmethod
    def markdown_textquote(text:str) ->str:
        text = text.strip().replace('\n','\n> ')
        return '> '+text+ '\n'
    @staticmethod
    def markdown_text_weightORitalic(text:str,style:str) ->str:
        if text in (None,''):
            text = 'None'
        if style == 'weight':
            return '**%s**'%text
        else:
            return '*%s*'%text
    @staticmethod
    def markdown_textlink(text:str,link:str) ->str:
        return '[%s](%s)'%(text,link)
    @staticmethod
    def markdown_textimg(text:str,imglink:str) ->str:
        return '![%s](%s)'%(text,imglink)
    @staticmethod
    def markdown_textlist(*texts,issort=False) ->str:
        texts = list(filter(lambda x : x not in (None,''),texts))
        out = ''
        if issort:
            index = 1
            outtexts = []
            for t in texts:
                outtexts.append(index.__str__() + '. '+t)
                index += 1
            out = '\n'.join(outtexts)
        else:
            out = '- ' + '\n- '.join(texts)
        return out + '\n'
    @staticmethod
    def markdown_textlist_2(texts:str,issort=False) ->str:
        texts = texts.split('\n')
        return Dingrobot.markdown_textlist(*texts,issort=issort)
    
    @staticmethod
    def markdown_drawline() ->str:
        return '------------------------------\n'
    @staticmethod
    def markdown_drawblock(*texts):
        '''
        内部不支持其他markdown
        '''
        return '\n\n    '+'    '.join(texts) + '    \n\n' #4个空格
    @staticmethod
    def getOutUrl(url):
        fmt = "dingtalk://dingtalkclient/page/link?url={}"
        url = parse.quote(url)
        return fmt.format(url)

    @staticmethod
    def std_markdowncontent(text_markdown):
        text_markdown = text_markdown.replace('\\_','\\\\_')

        return text_markdown
发包群更新机器人 = Dingrobot('https://oapi.dingtalk.com/robot/send?access_token=4f15526b3350cc53cfcbcdf8fbb76548a30912c3b8c6bc24ff1a407d11b9de4f',
'SEC3546324653891befc73616a1d699a384e68ae4a2defc983f33146fd80804c263')
内网发包群苦工 = Dingrobot('https://oapi.dingtalk.com/robot/send?access_token=b66ecc61ee2d2d38bdb00a6f52747aa8465edcdf6118f8ab3d1047d03b38a33e',
'SECab7597a071226a83d4ad4df64bbc81d7d5b032edd7dcd23fd501c4b7dfcafb8c')
一点五优化群优化机器人 = Dingrobot('https://oapi.dingtalk.com/robot/send?access_token=76f48b3f6654d831bdfbf5568ca67f2c3328b54ca52be4ababac67e54e786aa6',
'SECdbcb05f11f19d302bf21bf7e8e8681f007f234f6957aeb3a4e84cfba41d61fa9')
一点零注意了机器人 = Dingrobot('https://oapi.dingtalk.com/robot/send?access_token=c29f463dda94bb317ea97ef37ddaf50ccc42a181ddc5cf50cda95e84f102e7ae',
'SECad2832d42bc61e613c617ada84a78ea85015ef3091a789dd3b4ff32e3000285c')
一点五包来了机器人 = Dingrobot('https://oapi.dingtalk.com/robot/send?access_token=e2c3c6a4009351218232688c9976ea2dba3bf6b5421b7727a2e758c5afa18f81',
'SECda9ca91b4db028d1ebd97f21c2fe04de0add2ff46f368d93ce50e563b761b2aa')
一点五注意了机器人 = Dingrobot('https://oapi.dingtalk.com/robot/send?access_token=b3e49d3d2f9f987f2ec9ce9d0e6ff03e344fd7fc53c936a7a94d663525bdf629',
'SEC45884e44609fd8555bb0adbe3da3aa769aa59e9445658abdcac4d37eeade36f8')

杨都齐="13588189160"
尹宗都="17682316225"

if __name__ == "__main__":
    robotName_encode = sys.argv[1]
    robotName = ''.join(list(map(chr,map(int,robotName_encode.split('-')))))
    print(robotName)
    
    if not robotName in globals().keys():
        print(globals())
        print(f'{robotName}不存在')
        exit(1)
    robot = globals()[robotName]
    data_encode = sys.argv[2]
    
    data = bytes(map(int,data_encode.split('-'))).decode(encoding='ascii')
    robot.send(data)
    pass