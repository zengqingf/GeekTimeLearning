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

  

* 通过Safari获取iOS设备UDID

  [通过Safari浏览器获取iOS设备UDID(设备唯一标识符)](http://www.skyfox.org/safari-ios-device-udid.html)

  [github - iOS-UDID-Safari](https://github.com/shaojiankui/iOS-UDID-Safari)

  

  [iPhone XS doesn't have UDID](https://stackoverflow.com/questions/52473290/iphone-xs-doesnt-have-udid/52997294#52997294)

  [在线工具1](https://www.pgyer.com/tools/udid)

  [在线工具2](https://get.udid.io)

  [How to Find Your iPhone or iPad's UDID](https://deciphertools.com/blog/2014_11_19_how_to_find_your_iphone_udid/)

  ```  shell
  system_profiler SPUSBDataType -detailLevel mini | \
  grep -e iPhone -e Serial | \
  sed -En 'N;s/iPhone/&/p'
  
  
  #output:
  2018-10-25 12:57:06.527 system_profiler[23461:6234239] SPUSBDevice: 
  IOCreatePlugInInterfaceForService failed 0xe0003a3e
          iPhone:
            Serial Number: 3aeac....4145
            
  alias fudid="system_profiler SPUSBDataType -detailLevel mini | grep -e iPhone -e Serial | sed -En 'N;s/iPhone/&/p'"
  ```
  
  

* macOs设置开机任务

  [macos设置开机启动任务](https://0clickjacking0.github.io/2020/05/20/macos%E8%AE%BE%E7%BD%AE%E5%BC%80%E6%9C%BA%E5%90%AF%E5%8A%A8%E4%BB%BB%E5%8A%A1/)

  [manpagez: man pages & more man launchd.plist(5)](https://www.manpagez.com/man/5/launchd.plist/)

  ``` tex
  方法一：
  系统偏好-帐户-登录项
  
  方法二：
  launchctl
  
  plist存储目录：
  ~/Library/LaunchAgents 由用户自己定义的任务项
  /Library/LaunchAgents 由管理员为用户定义的任务项
  /Library/LaunchDaemons 由管理员定义的守护进程任务项
  /System/Library/LaunchAgents 由Mac OS X为用户定义的任务项
  /System/Library/LaunchDaemons 由Mac OS X定义的守护进程任务项
  
  常见目录：
  /Library/LaunchDaemons –>只要系统启动了，哪怕用户不登陆系统也会被执行
  /Library/LaunchAgents –>当用户登陆系统后才会被执行
  ```

  ``` xml
  <?xml version="1.0" encoding="UTF-8"?>
  <!DOCTYPE plist PUBLIC -//Apple Computer//DTD PLIST 1.0//EN
  http://www.apple.com/DTDs/PropertyList-1.0.dtd >
  <plist version="1.0">
  <dict>
      <key>Label</key>
      <string>com.example.exampled</string>
      <key>ProgramArguments</key>
      <array>
           <string>exampled</string>
      </array>
      <key>KeepAlive</key>
      <true/>
      <key>RunAtLoad</key>
      <true/>
  </dict>
  </plist>
  ```

  ``` shell
  #plist键值对说明：
  
  #Label（必须）
  #该项服务的名称，且文件名必须和Label一致，比如上述的plist文件，文件名就叫com.example.exampled.plist
  
  #ProgramArguments：指定可执行文件路径及其参数，比如执行ls -a，对应到该配置中，应该写作：
  <key>ProgramArguments</key>
  <array>
       <string>ls</string>         
       <string>-a</string>
  </array>
  
  #RunAtLoad (可选)：标识launchd在加载完该项服务之后立即启动路径指定的可执行文件。默认值为 false,设置为 true 即可实现开机运行脚本文件。
  
  #StartCalendarInterval (可选)
  #该关键字可以用来设置定时执行可执行程序，可使用 Month, Day, Hour, Minute, Second等子关键字，它可以指定脚本在多少月，天，小时，分钟，秒，星期几等时间上执行，若缺少某个关键字则表示任意该时间点，类似于 Unix 的 Crontab 计划任务的设置方式，比如在该例子中设置为每小时的20分的时候执行该命令。
  
  #KeepAlive（可选）是否保持持续运行
  
  #所有key关键字详细使用说明可以在Mac OS X终端下使用命令man launchd.plist查询
  ```

  ``` shell
  #检查语法错误
  plutil local.localhost.startup.plist
  # 加载启动任务
  launchctl load ~/Library/LaunchAgents/example.plist
  # 加载任务, -w选项会在下次登录/重新启动时重新启动。
  launchctl load -w ~/Library/LaunchAgents/example.plist
  #停止
  sudo launchctl stop <path>
  #开始
  sudo launchctl start <path>
  #kill
  sudo launchctl kill <path>
  #卸载
  # 停止并卸载任务。下次登录/重新启动时，任务仍将重新启动。
  launchctl unload ~/Library/LaunchAgents/example.plist
  # 该任务将不会在下次登录/重新启动时重新启动。
  launchctl unload -w ~/Library/LaunchAgents/example.plist
  #查看状态
  launchctl list
  sudo launchctl list（有时候一些需要用sudo才能执行的服务就需要用sudo才能查看）
  #输出具有以下含义：
  #第一个数字是进程的PID，如果它正在运行，如果它不运行，它显示一个’ - ‘。
  #第二个数字是进程的退出代码（如果它已经完成）。如果是负数，则是杀死信号的数量。
  #第三列是进程名称。
  
  #@注意：执行launchctl命令加sudo与不加结果是完全不同的。
  ```

  

* install dmg / pkg on command line

  ``` shell
  cd ~/Desktop
  curl -O http://darwinports.opendarwin.org/downloads/DarwinPorts-1.2-10.4.dmg
  hdiutil attach DarwinPorts-1.2-10.4.dmg
  cd /Volumes/DarwinPorts-1.2/
  sudo installer -pkg DarwinPorts-1.2.pkg -target "/"
  hditutil detach /Volumes/DarwinPorts-1.2/
  ```







---



### Bash Shell

* 快捷键

  移动光标

  - `ctrl+b`: 前移一个字符(backward)
  - `ctrl+f`: 后移一个字符(forward)
  - `alt+b`: 前移一个单词
  - `alt+f`: 后移一个单词
  - `ctrl+a`: 移到行首（a是首字母）
  - `ctrl+e`: 移到行尾（end）
  - `ctrl+xx`: 行首到当前光标替换
  - `ctrl+u`: 清除剪切光标之前的内容

  编辑命令

  - `alt+.`: 粘帖最后一次命令最后的参数（通常用于`mkdir long-long-dir`后, `cd`配合着`alt+.`）
  - `alt+d`: 删除当前光标到临近右边单词开始(delete)
  - `ctrl+u`: 删除光标左边所有
  - `ctrl+h`: 删除光标前一个字符（相当于backspace）
  - `ctrl+d`: 删除光标后一个字符（相当于delete）
  - `ctrl+w`: 删除当前光标到临近左边单词结束(word)
  - `ctrl+k`: 删除光标右边所有
  - `ctrl+y`: 粘贴刚才所删除的字符
  - `ctrl+l`: 清屏
  - `ctrl+shift+c`: 复制（相当于鼠标左键拖拽）
  - `ctrl+shift+v`: 粘贴（相当于鼠标中键）

  其它

  - `ctrl+n`: 下一条命令
  - `ctrl+p`: 上一条命令
  - `alt+n`: 下一条命令（例如输入`ls`, 然后按'alt+n', 就会找到历史记录下的`ls`命令）
  - `alt+p`: 上一条命令（跟`alt+n`相似）
  - `shift+PageUp`: 向上翻页
  - `shift+PageDown`: 向下翻页
  - `ctrl+r`: 进入历史查找命令记录， 输入关键字。 多次按返回下一个匹配项

  zsh

  - `d`: 列出以前的打开的命令
  - `j`: jump到以前某个目录，模糊匹配
  - `!!`: 重复执行最后一条命令

  **Vim**

  移动光标

  - `b`: 向前移动一个单词
  - `w`: 向后移动一个单词

  删除

  - `dw`: 从当前光标开始删除到下一个单词头
  - `de`: 从当前光标开始删除到单词尾

  
