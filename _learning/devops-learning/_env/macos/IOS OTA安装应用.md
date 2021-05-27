

# IOS OTA安装应用

OTA : over-the-air

* link

  [一步一步实现无线安装iOS应用(内网OTA)](https://www.jianshu.com/p/35ca63ec0d8e)

  ``` text
  <a title="iPhone" href="itms-services://?action=download-manifest&url=https://192.168.10.193/installIPA.plist"> Iphone Download</a>
  
  @注意：前缀：itms-services://?action=download-manifest&url=
  前缀带=，即后面的url不能带=,或者需要urlencode转换掉，否则会中断url识别，导致后续点链接无反应问题
  
  plist的访问链接中不能有中文，plist的下载链接中不能有中文
  ```

  标准格式

  ``` xml
  <?xml version="1.0" encoding="UTF-8"?>
  <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
  <plist version="1.0">
      <dict>  
          <key>items</key>
          <array>
              <dict>
                  <key>assets</key>
                  <array>
                      <dict>
                          <key>kind</key>
                          <string>software-package</string>
                          <key>url</key>
                          <string>https://192.168.2.60:58002/temp/ResCombine/a8_dev/ios/code/ios-2021-1.ipa</string>
                      </dict>
                  </array>
                  <key>metadata</key>
                  <dict>
                      <key>bundle-identifier</key>
                      <string>com.tenmove.a8.dev</string>
                      <key>bundle-version</key>
                      <string>1.0.0</string>
                      <key>kind</key>
                      <string>software</string>
                      <key>title</key>
                      <string>IOS安装</string>
                  </dict>
              </dict>
          </array>
      </dict>
  </plist>
  ```





---



### CA证书生成

* link

  [openssl生成CA证书](https://blog.csdn.net/cowbin2012/article/details/100134114)

  [apache目录属性设置](https://www.cnblogs.com/xiaochaohuashengmi/archive/2011/04/24/2026532.html)

  [Mac配置本地https服务](https://www.jianshu.com/p/d22baeae50ea)

  [openssl的介绍和使用](https://segmentfault.com/a/1190000014963014)

  [HTTPS从认识到线上实战全记录](http://blog.haoji.me/https.html)

* 推荐证书生成环境  linux / macOS

  * 测试环境：macOS + XAMPP

    [generate_https_cer.sh](./ca_gen/generate_https_cer.sh)

     [/System/Library/OpenSSL/misc/CA.sh](./ca_gen/CA.sh) 

    ``` text
    遇到的问题：
    
    using configuration from /private/etc/ssl/openssl.cnf
    variable lookup failed for ca::default_ca
    
    解决：拷贝XAMPP中 etc/openssl（链接） 替换  /private/etc/ssl/openssl.cnf  （注意提前备份 /private/etc/ssl/openssl.cnf）
    ```

  * 测试环境：Linux + httpd （apache）

    ``` sh
    #server证书
    
    #生成server私钥  (这样是生成rsa私钥，openssl格式，2048位强度。server.key是密钥文件名)
    openssl genrsa -out server.key 2048
    
    #生成server自签名证书 （req是证书请求的子命令，-sha256表示算法，-x509表示输出证书，-days365 为有效期）
    #需要输入拥有者信息：举例：国家 CN 省份 ZJ 地区 HZ 公司 TM 
    #@注意：Common Name 应该与域名保持一致，如搭建服务器的ip为 192.168.2.60
    openssl req -new -sha256 -x509 -days 365 -key server.key -out server.crt
    
    #CA证书
    
    #生成CA私钥 （这里使用-des3进行加密，需要四位以上密码）
    openssl genrsa -des3 -out ca.key 4096
    
    #生成证书
    openssl req -new -x509 -days 365 -key ca.key -out ca.crt
    
    #生成服务器私钥
    openssl genrsa -out server.key 4096
    
    #生成证书请求文件csr
    openssl req -new -key server.key -out server.csr
    
    #自己作为CA的签发证书（需要输入生成ca.key时设置的密码）
    openssl ca -in server.csr -out server.crt -cert ca.crt -keyfile ca.key -days 365
    
    
    #linux下 执行openssl ca ...  遇到问题
    [CA签名是报的错误及解决方法](https://blog.csdn.net/n_u_l_l_/article/details/103536588)
    1. /etc/pki/CA/private/cakey.pem
    	openssl genrsa 1024 > /etc/pki/CA/private/cakey.pem
    2. /etc/pki/CA/cacert.pem
    	openssl req -new -key /etc/pki/CA/private/cakey.pem -days 3650 -x509 -out /etc/pki/CA/cacert.pem
    3. /etc/pki/CA/index.txt
    	touch /etc/pki/CA/index.txt（为空文件）
    4. /etc/pki/CA/serial
    	echo 01 > /etc/pki/CA/serial
    
    
    #****************************通过上述macOS的shell脚本生成***********************
    #生成目录结构如下：
    #需要将demoCA下的cacert.pem重命名为ca.crt  提供给客户端安装    
    #注意 ca.crt 需要为pem格式
    ```

    ![image-20210518101013144](https://i.loli.net/2021/05/18/AVI1KzSx2a3iRyP.png)

    ``` xml
    <!--</M>linux centos7 yum安装的httpd目录 ： /etc/httpd-->
    
    #配置httpd.conf
    Listen 新的监听端口 （非https）
    DocumentRoot "自定义的服务器根目录"
    <Directory "自定义的服务器根目录">
    	Options Indexes（没有index.html会显示目录内容） FollowSymLinks（目录支持链接） Multiviews（）
        AllowOverride None
        Require all granted
    </Directory>
    
    #配置httpd-ssl.conf (或者ssl.conf)
    Listem 新的监听端口（https）
    <VirtualHost *:新的端口/默认ssl端口 *:433>
    SSLCertificateFile "/xxx/server.crt"   <!--引号是否需要加看情况-->
    SSLCertificateKeyFile "/xxx/server.key"
    SSLCACertificateFile /XXX/ca.crt
    </VirtualHost>
    
    <!--@注意：为了内部测试方便，自定义服务器根目录 至少要开放可读权限给everyone-->
    ```

    