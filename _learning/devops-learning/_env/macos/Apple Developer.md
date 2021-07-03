# Apple Developer

* 证书

  [Apple PKI](https://www.apple.com/certificateauthority/)

  ![](https://i.loli.net/2021/05/10/2PoCgAOci8EQb6H.jpg)

  **测试时间2021-5-10**

  [Apple开发证书和发布证书不受信任问题](https://blog.csdn.net/Dancen/article/details/114406431)

  ``` text
  mac 钥匙串中新增的证书 显示不受信任
  
  目前的Apple全球开发者关系认证中间证书WWDRCA（也就是我们上面下载的那个）将于2023年2月7日到期。Apple发布了新的WWDRCA，更新后的证书过期时间为2030年2月20日，新证书将用于签署2021年1月28日后为苹果开发者项目颁发的新软件签名证书。
  
  解决：
  安装WWDR3 （AppleWWDRCAG3.cer）
  ```

  [WDRC 证书一定要在系统钥匙串](https://juejin.cn/post/6844903728324018190)

  ``` text
  q:error: Signing certificate is invalid. Signing certificate "iPhone Distribution: ******", serial number "******", is not valid for code signing. It may have been revoked or expired. (in target '*****')
  
  解决：
  保留登录的 AppleWWDRCAG.cer 
  删原先的 系统的 AppleWWDRCAG.cer 
  安装 AppleWWDRCAG.cer 到系统
  新下载的 AppleWWDRCAG3.cer 安装到登录和系统
  
  注意：
  1. 证书过期后添加 需要删掉过期的
  2. 证书需要移动位置（如从登录移动到系统，需要删掉原来的，重新安装）
  ```

  

* iOS超级签名

  [IOS 超级签名实现 (linux 版本)](http://events.jianshu.io/p/ea9896b68a05)

  [iOS ipa超级签名](https://shirojin.github.io/2020/04/04/iOS%E8%B6%85%E7%BA%A7%E7%AD%BE%E5%90%8D/)

  [add-device-and-generate-profile.rb](./reg_uuid/add-device-and-generate-profile.rb)

  [register-device.sh](./reg_uuid/register-device.sh)





---



* macOs BigSur 证书不可信 (certificate is not trusted)

  ``` tex
  https://developer.apple.com/de/support/expiration/ should be the answer.
  Just install the certificate manually or upgrade to Xcode 11.4.1 or later. 
  After upgrading to Xcode >= 11.4.1 I had to open a Xcode project and had to wait few seconds. 
  Afterwards the new Apple Worldwide Developer Relations Intermediate Certificate automatically has been installed.
  ```

  * 问题1：（环境：Big Sur系统、Xcode12.5）
  
    ``` tex
        Signing Identity:     "iPhone Developer: XXX"
        Provisioning Profile: "TM-V1-Dev-Provisioning-Profile"
                              (AAAAA-BB-CC-DD-EEEE)
      
            /usr/bin/codesign --force --sign "certificate UID" --entitlements /Users/<NAME>/UE4/Builds/DESKTOP-VVN2869/C/jwk/A8_Trunk_IOS/trunk/Client/NextGenGame/Intermediate/ProjectFilesIOS/build/NextGenActionGame.build/Development-iphoneos/NextGenActionGame.build/HitBoxMakerBlueprint.app.xcent --timestamp=none /Users/tenmove/UE4/Builds/DESKTOP-VVN2869/C/jwk/A8_Trunk_IOS/trunk/Client/NextGenGame/Binaries/IOS/Payload/HitBoxMakerBlueprint.app
        Warning: unable to build chain to self-signed root for signer "iPhone Developer: XXX"
        /Users/tenmove/UE4/Builds/DESKTOP-VVN2869/C/jwk/A8_Trunk_IOS/trunk/Client/NextGenGame/Binaries/IOS/Payload/HitBoxMakerBlueprint.app: errSecInternalComponent
        Command /usr/bin/codesign failed with exit code 1
      
        ** BUILD FAILED **
    ```
  
    解决1：
  
    [Resolving iOS Code Signing Failures due to WWDR Intermediate Certificate Expiration](https://support.circleci.com/hc/en-us/articles/360052459251-Resolving-Enterprise-iOS-Code-Signing-Failures-due-to-WWDR-Intermediate-Certificate-Expiration)
  
    ``` sh
    - run: curl https://www.apple.com/certificateauthority/AppleWWDRCAG3.cer -o AppleWWDRCAG3.cer
    - run: sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain AppleWWDRCAG3.cer
    ```
  
    ![](https://raw.githubusercontent.com/MJX1010/PicGoRepo/main/img/20210623104054.jpg?token=ACYDR7SGNUC42K7DCE3JVGDA2KPXI)
  
    ``` tex
    安装后钥匙串界面一开始未not trust，需要执行一次codesign 会提示errSec... 回到钥匙串界面就会刷新为valid
    ```
  
    
  
    



* iOS 14.6无法安装 ipa，提示此App的开发者需要更新App以在此iOS版本上正常工作

  [最新消息！IOS14.2 beta2 最新的代码签名格式！！](https://zhuanlan.zhihu.com/p/324009534)

  ``` text
  问题：2021-06-15
  	1.ipa打包环境还是老的系统和Xcode版本：
      解决：升级到macOs Big Sur并且升级Xcode版本到12.5
      							
      2. 使用UE4.25.4默认codesign签名一次，不行
      解决：使用乐变重签名工具重新签名
      
  扩展：
  	1.获取codesign identity
  	security find-identity -v -p codesigning
  	#output: 482348ADF834384843884934734 "iPhone Developer: xxx (yyy)"
  			 ...
  		
  	2.codesign重签名
  	/usr/bin/codesign -s "codesign identity" -f --preserve-metadata /xx/YY.app
  	
  	3.codesign获取app是否具有新签名
  	/usr/bin/codesign -dv /xx/YY.app
  	#output: CodeDirectory v=20200...
  	
  问题：
  	1.macOs BigSur 无法建立/data目录  提示没权限 即使sudo mount -uw / 也不行
  		先在home目录下创建一个可以读写的目录，例如/Users/zc/data
  		sudo vim /etc/synthetic.conf在synthetic.conf文件中添加一行
  		（注意：/Users/zc/data是你自己创建的可读写的目录，可以自定义。用来做为/data实际存储的目录。
  		重启后会创建一个/data的软链接，指向/Users/zc/data)
  ```

  

