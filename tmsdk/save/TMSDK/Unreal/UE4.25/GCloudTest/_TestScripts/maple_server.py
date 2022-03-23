# -*- encoding: utf-8 -*-

import requests
import json,random
import time
import urllib
import base64,hmac,hashlib
import socket


def sign():
    pass

def gettree():
    ACCESSKEY = "xhTI7rJnPZDEGgf1SGipBXSGOuEE"
    URI_PREFIX = "/v1/openapi"
    HOST = "open-sh.gcloud.qq.com:8927"
    params = [ ("gameid", "948137280"), ("ts", int(time.time())), ("nonce", random.randint(0, 0x7FFFFFFF)), ("accessid", "uCApkcj7S2WSaWniV4gt5NH1QasE"), ("ProductID", "948137280"), ]
    params.sort(key=lambda x:x[0])
    module = "dir"
    action = 'GetAllPlatform'
    uri = URI_PREFIX + "/" + module + "/" + action
    req_url = "http://" + HOST + uri
    sig = uri + "?"
    first = True
    for key, value in params:
        if first:
            first = False
        else:
            sig += "&"
            sig += key + "=" +  urllib.parse.quote_plus(str(value))
            #sig += key + "=" +  urllib.quote_plus(str(value))
    sig = base64.b64encode(hmac.new(bytes(ACCESSKEY,'utf-8'), bytes(sig,'utf-8'), hashlib.sha1).digest())
    #sig = base64.b64encode(hmac.new(ACCESSKEY, sig, hashlib.sha1).digest())
    print(sig)
    sig = sig.decode().strip().replace("+", "-")
    #sig = sig.replace("+", "-")
    sig = sig.replace("/", "")
    print(sig)
    params.append(("signature", sig))
    print(f'requrl: {req_url} params: {params}')
    r = requests.post(req_url ,data=params)
    print(r.json())

def upserver():
    Host = "101.89.15.219:30000"   #区服数据上报地址（外网）
    time_n = int(time.time())

    #测试请求内容
    params = []
    head = {
            "gameid"       : 948137280,
             "encrypt_type" : 0,
             "client_time"  : time_n,
             "asyncid"      : 1
            }
    body = {
    "gamekey"     : "8ab7d865a7a686ae4c94ff9b3d3fccd4",
    "client_time" : time_n,
    "msg_type"    : "dir_status_report",
    "userdata": {
        "platform_id" : 1,
        "server_id"   : 3,
        "online"      : 99,
        "capacity"    : 100
        }
    }
    params.append(head)
    params.append(body)
    params = json.dumps(params)
    print(params)


    #需要json dumps格式
    #filename = "./info.jsonx"
    #placeholder = "/*client_timestamp_placeholder*/"
    #inplace_str_change(filename, placeholder, str(time_n))
    #with open(filename, "r") as f:
    #    fc = f.read()
    
    #正确请求内容
    request_content = json.dumps(head) + json.dumps(body)
    print(request_content)


    # 创建一个udp套接字
    udp_socket = socket.socket(socket.AF_INET,socket.SOCK_DGRAM)
    # 发送数据，从键盘获取数据
    udp_socket.sendto(request_content.encode("utf-8"), ("101.89.15.219",30000))
    r = udp_socket.recvfrom(1024)
    print(r)
    # 关闭套接字
    udp_socket.close()

'''
文件原地替换内容
目前只支持替换str
'''
def inplace_str_change(filename, old_str, new_str):
    with open(filename, "r") as f:
        fr = f.read()
        if old_str not in fr:
            print('"{old_str}" not found in {filename}, no replace'.format(**locals()))
            return   
    with open(filename, 'wt') as f:
        print('Changing "{old_str}" in {filename}'.format(**locals()))
        fs = fr.replace(old_str, new_str)
        f.write(fs)

if __name__ == "__main__":
    #gettree()
    upserver()