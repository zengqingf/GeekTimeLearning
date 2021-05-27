#########################################################################
# File Name: generate_https_cer.sh
# Author: etond
# mail: monkey_tv@126.com
# Created Time: Sat Jan 23 11:14:10 2016
#########################################################################
#!/bin/bash

#openssl genrsa -des3 -out private.key 2048 
#openssl req -new -key private.key -out server.csr 
#openssl rsa -in private.key -out server.key
#sudo openssl x509 -req -in server.csr -out server.crt -signkey server.key -days 3650

mkdir ca
cd ca
cp /System/Library/OpenSSL/misc/CA.sh CA.sh
./CA.sh -newca

echo '********CA*********'

#生成私钥
sudo openssl genrsa -des3 -out private.key 2048

echo '********private.key*********'
#生成证书请求
sudo openssl req -new -key private.key -out server.csr
echo '********server.csr*********'
#生成服务器的私钥，去除密钥口令
sudo openssl rsa -in private.key -out server.key
echo '********server.key*********'
#使用CA进行签名，生成server.crt
cp server.csr newreq.pem
./CA.sh -sign
mv newcert.pem server.crt
echo '********server.crt*********'
#或者上面三步都不需要，直接使用下面一步
#openssl ca -in server.csr -out server.crt
