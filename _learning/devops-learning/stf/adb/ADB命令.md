# ADB命令

link:

[Android 调试桥 (adb)](https://developer.android.com/studio/command-line/adb#howadbworks)



* USB-ADB调试

  ``` shell
  adb kill-server  #kill the server if it is running
  
  #启动守护进程
  #-a - listen on all network interfaces, not just localhost - 侦听所有网络接口，而不仅仅是本地主机
  adb nodaemon server -a -P 5037 
  #启动守护进程前先 kill-server
  
  # 访问守护进程 执行命令
  #-H - name of adb server host [default=localhost]
  adb -H 192.168.*.*** devices
  adb -H 192.168.*.*** shell
  # notice: 互相连接的两台设备 adb版本号也要互相匹配
  ```
  
  ``` powershell
  @echo off
  REM 更改当前目录为批处理本身的目录
  cd /d %~dp0
  
  REM adb version > 1.0.32   otherwise use adb -a -P 5037 fork-server server
  
  for /F "tokens=5 delims= " %%a in ('netstat -aon ^| findstr 5037 ^| findstr LISTENING') do ( 
  	echo port occupied : %%a
  	taskkill /pid %%a /f /t
  )
  adb devices
  adb kill-server
  adb nodaemon server -a -P 5037
  pause
  ```
  
  



* adb安装卸载

    ``` shell
    #指定设备安装
    adb devices
    adb -s (deviceid) install xxx.apk
    
    #安装获取进度
    adb push apk.apk /data/local/tmp
    adb shell pm install -r /data/local/tmp/apk.apk
    adb shell rm /data/local/tmp/apk.apk
    ```

  ``` shell
  #安装获取进度
  @echo off
  set apkname=%1%
   
  ::传入apk路径
  echo 您输入了参数:%apkname% 
   
  Set filename=%apkname%
  set filename=%~nx1
  echo 文件名为:%filename%
  set folder=%~dp1
  echo 路径为:%folder%
  
  adb push %apkname% /data/local/tmp
  echo 复制到设备完成！***开始安装,耐心等待***
  adb shell pm install -r /data/local/tmp/%filename%
  echo ***安装完成***
  adb shell rm /data/local/tmp/%filename%
  echo ***删除临时文件***
  ```

  

* adb connect wifi

  [ADB over Wi-Fi](https://help.famoco.com/developers/dev-env/adb-over-wifi/)
  
  ``` text
  Connect the device and the computer to the same Wi-Fi network
  
  Plug the device to the computer with a USB cable to configure the connection
  
  On the computer command line type: adb tcpip 5555
  
  On the computer command line type: adb shell ip addr show wlan0 and copy the IP address after the "inet" until the "/". You can also go inside the Settings of the device to retrieve the IP address in Settings → About → Status.
  
  On the computer command line type: adb connect ip-address-of-device:5555
  
  You can disconnect the USB cable from the device and check with adb devices that the device is still detected.
  ```
  
  



---



* 问题：adb使用时出现unanthorized问题

  [adb使用时出现unanthorized问题](https://www.cnblogs.com/yejintianming00/p/9339020.html)

  ``` text
  ADB 启动时，adb devices出现unanthorized问题。
  
  检查USB调试是否开启。
  重新拔插USB数据线是否有授权提示
  重启adb ：adb kill-server和adb start-server
  
  
  补充方法：
  
  删除C:\Users\你电脑的用户名\.android下的adbkey和adbkey.pub
  
  关闭USB调试
  
  重新打开一个DOS命令窗口，进入ADB中
  
  先adb kill-server关闭adb驱动，然后输入adb start-server打开adb服务
  
  插入USB数据线，打开USB调试，出现以下图片，只有出现了第二张图片中的序号，adb才可以配对成功。点击确定，公钥配对成功，只要出现了第二个图片，不管配不配对，本地C:\Users\你电脑的用户名\.android都会新出现两个文件adbkey和adbkey.pub
  
  输入adb devices,成功打开adb服务
  
  
  原理：
  当我们在window电脑上启动adb.exe进程时，adb会在本地生成一堆adbkey（私钥）和adbkey.pub（公钥）；根据弹框提示"The computer's RSA key fingerprint is:xxxx"，可以看出是一对RSA算法的密钥，其中公钥是用来发送给手机的；当执行"adb shell"时，adb.exe会将当前电脑PC的公钥（或者公钥）的hash值（fingerprint）发送给Android设备；这是，如果android上已经保存了这台PC的公钥，则匹配出对应的公钥进行认证，建立adb连接；如果android上没有保存这台PC的公钥，则会弹出提示框，让你确认是否允许这台机器进行adb连接，当你点击了允许授权之后，android就会保存了这台PC的adbkey.pub(公钥)
  
  
  
  Adbkey和adbkey.pub的存储位置：
  
  window系统，当我们首次启动adb.exe时，会在C盘的当前用户的目录下生成一个".android"目录，其中adbkey与adbkey.pub就在这个目录下；（adb.exe会在启动时读取这两个文件（如果没有生成就重新生成），所以如果你要是删除或者修改了这两个文件后，必须关闭adb.exe进程，重启之后才能生效）；
  
  android系统上,PC的公钥被保存在一个文件中"/data/misc/adb/adb_keys"
  ```



* 问题：usb hub 供电不足

  ``` text
  windows:
  设备管理器 - 通用串行总线控制器 - USB Root Hub属性 - 电源管理 - 取消勾选 允许计算机关闭这个设备以节约电源
  ```



* 问题：adb device offline with ADB wireless

  ``` tex
  adb devices
  List of devices attached
  192.168.1.2:5555        offline
  ```

  ``` tex
  Settings -> Developer options -> Revoke USB debugging authorizations (clear the list of authorized PCs).
  Set USB Debugging OFF.
  In Terminal write : adb kill-server
  Then : adb start-server
  Then : adb connect xx.xx.xx.xx:5555 (the devices ip), it should say unable to connect.
  Now turn ON USB debugging again and type the adb connect xx.xx.xx.xx:5555 again.
  It should now ask for authorization and you are back online without needing to connect cable to USB, only wifi used.
  ```
  
  ``` shell
  #安卓设备和pc连接到可以ping通的网络环境下
  #安卓设备ip查看： 设置-关于本机-状态信息-ip地址（获取内网ip 192.168.x.x）
  adb shell
  setprop service.adb.tcp.port 5555
  stop adb
  start adb
  exit
  adb connect 192.168.x.x:5555
  ```
  
  

* 问题：unable to connect to 192.168.x.x:5555: cannot connect to 192.168.x.x:5555: 由于连接方在一段时间后没有正确答复或连接 的主机没有反应，连接尝试失败。 (10060)

  ``` tex
  设备wifi和电脑不在同一个网段
  
  可以电脑ping一下设备wifi的ip
  ```

  

* 问题：unable to connect to 192.168.x.x:5555: cannot connect to 192.168.x.x:5555: 由于目标计算机积极拒绝，无法连接。 (10061)

  ``` shell
  adb -s 设备id usb   #切换到USB模式
  adb kill-server
  adb -s 设备id tcpip 5555  #切换到wifi无线调试模式
  adb -s 设备id connect 192.168.x.x:5555
  
  #adb -s 设备id disconnect 192.168.x.x:5555  断开连接
  ```

  
