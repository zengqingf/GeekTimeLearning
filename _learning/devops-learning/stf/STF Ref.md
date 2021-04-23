# STF Ref

### 基础

* 介绍

  [8分钟带你了解 OpenSTF](https://segmentfault.com/a/1190000023580853)

  ``` text
  STF（Smartphone Test Farm）是一个开源的web架构应用
  ```

  * stf架构

    <img src="https://i.loli.net/2021/04/13/cudBVGMP3OyfkgK.png" style="zoom:150%;" />

    <img src="https://i.loli.net/2021/04/13/xckIzeE16mu9yRM.png" style="zoom:150%;" />、

  * 可能的坑

    ``` text
    node.js只支持8.x版本的 ，所以可以用nvm来管理node.js的版本，安装多个node.js
    设备一直显示disconnect或者preparing，之后disconnect，这是他们的一个bug，当数据线很差当时候会出现。换一条数据线。
    小米设备需要sim卡打开通过usb安装的权限才可以
    如果在docker上安装，最好是linux系统，mac上可以装一个linux虚机来安装
    ```



* 部署方式

  [自动化测试：如何在CentOS7下通过Docker方式搭建OpenStf环境？](https://zhuanlan.zhihu.com/p/363567066)

  ``` text
  1、主节点部署STF服务并允许远程访问API，子节点只需部署ADB，保证ADB 5037端口对外暴露即可。这种方案的优点是部署简单，而且子节点无需部署STF服务。缺点就是子节点新增机器需要在主节点重新运行STF命令以识别子节点上新增的机器。
  
  2、主节点和子节点都部署STF服务并允许远程访问API。然后主节点连接子节点。这种方案的优点是子节点新增设备可以自动识别。缺点就是子节点也需要部署STF服务。
  ```




---



### 实践

* centos7  + docker v3:20.10.6 +  stf 搭建 （**更新于2021/04/13**）

  [使用docker部署STF服务（CentOS环境）](https://www.jianshu.com/p/10bdf33d2c64)

  [CentOS下通过docker方式搭建OpenSTF](https://blog.csdn.net/FloatDreamed/article/details/103809814)

  * 拉取镜像

    ``` shell
    # 迁移from openstf to devicefarmer
    #sudo docker pull openstf/stf:latest   
    sudo docker pull devicefarmer/stf:latest
    
    #adb镜像
    sudo docker pull sorccu/adb:latest
    #rethinkdb镜像
    sudo docker pull rethinkdb:latest
    
    #ambassador镜像
    # 迁移from openstf to devicefarmer
    #sudo docker pull openstf/ambassador:latest
    sudo docker pull devicefarmer/ambassador:latest
    
    #nginx 反向代理镜像
    sudo docker pull nginx:latest
    ```

  * 启动容器

    ``` shell
    #启动rethinkdb
    sudo docker run -d --name rethinkdb -v /srv/rethinkdb:/data --net host rethinkdb rethinkdb --bind all --cache-size 8192 --http-port 8090
    #如果未成功启动(添加--privileged=true)
    docker run -d --name rethinkdb --privileged=true -v /srv/rethinkdb:/data --net host rethinkdb rethinkdb --bind all --cache-size 8192 --http-port 8090
    
    #启动adb service
    #注意：如果stf的宿主机在虚拟机上，为了让主机连接的设备能识别到，宿主机的adb不需要启动了
    sudo docker run -d --name adbd --privileged -v /dev/bus/usb:/dev/bus/usb --net host sorccu/adb:latest
    
    #启动stf 
    sudo docker run -d --name stf --net host devicefarmer/stf stf local --public-ip 本机局域网ip --allow-remote
    #如果未成功启动(添加--privileged=true)
    docker run -d --name stf --privileged=true --net host devicefarmer/stf stf local --public-ip 本机局域网ip --allow-remote
    #添加其他主机上的设备连接 (虚拟机和主机 adb共享可用)
    docker run -d --name stf --privileged=true --net host devicefarmer/stf stf local --public-ip 本机局域网ip --adb-host 其他机器ip --adb-port 5037  --allow-remote
    
    
    #也可以
    #进入容器并启动相关
    
    
    #查看stf docker 进程号
    docker ps -a 
    docker exec -it stf进程号 /bin/sh
    #容器内部执行命令
    stf local --public-ip 本机IP --allow-remote
    #容器内部 通过adb host连接其他主机上的设备
    #命令可以从stf启动后的日志中获取
    #日志关键字是  stf provider .....
    
    ### 推荐方法：进入stf容器再处理 ###
    #例子：
    #下面命令不可行
    docker ps -a 
    docker exec -it stf容器ID /bin/sh
    #查看主机名
    hostname
    # linux centos虚拟机
    stf provider --name localhost.localdomain --min-port 7400 --max-port 7700 --connect-sub tcp://127.0.0.1:7114 --connect-push tcp://127.0.0.1:7116 --group-timeout 900 --public-ip 192.168.2.132 --storage-url http://192.168.2.132:7100/ --adb-host (目标主机IP) --adb-port 5037 --vnc-initial-size 600x800 --mute-master never --allow-remote
    ```

  * 虚拟机（防火墙等）相关

    ``` shell
    #查看防火墙状态
    systemctl status firewalld
    
    #临时打开 reboot后失效 
    systemctl start firewalld.service  /  systemctl start firewalld
    
    #临时关闭 reboot后失效 
    systemctl stop firewalld.service  /  systemctl stop firewalld
    
    #永久开启 设置后reboot后生效
    systemctl enable firewalld
    
    
    
    #可能需要放开7100端口
    firewall-cmd --query-port=7100/tcp
    #开放7100端口
    firewall-cmd --permanent --zone=public --add-port=7100/tcp
    #重载
    firewall-cmd --reload
    
    ```

    

    ``` text
    #虚拟机-编辑-虚拟网络编辑器-（VMnet8 NAT模式）-NAT模式设置 - 添加虚拟机ip和port映射
    
    #如果虚拟机（作为master）,主机无法访问虚拟机中的stf网址时，并且虚拟机网络模式是 NAT
    #主机ping虚拟机
    #ping不通的话
    #可以查看主机 网络适配器中的 ”VMware Virtual Ethernet Adapter for VMnet8“是否被禁了
    ```

    

    

  * 整合脚本

    stf-setup-run-linux-centos7.sh

    ``` shell
    #!/bin/bash
     
    #IP
    #IP=`ip addr | grep 'state UP' -A2 | tail -n1 | awk '{print$2}' | awk -F "/" '{print $1}'`
    IP=`hostname -I | awk '{print $1}'`
    echo $IP
    
    #检查docker是否安装
    systemctl start docker
    
    if [ $? != 0 ]; then
        #docker安装-安装软件包
        yum install -y yum-utils device-mapper-persistent-data lvm2
    
        #docker安装-设置稳定的仓库
        yum-config-manager --add-repo https://download.docker.com/linux/centos/docker-ce.repo
    
        #docker安装---列出并排序存储库中可用的docker版本
        yum list docker-ce --showduplicates | sort -r
    
        #docker安装---通过其完整的软件包名称安装特定版本
        echo "Please enter the version number of docker(e.g. 19.03.5):"
        read version
        echo "准备安装docker.........."
        yum install docker-ce-$version docker-ce-cli-$version containerd.io
        echo "docker安装完毕.........."
    
        #docker安装---查看当前docker的版本信息
        echo "---------- docker versionInfo ----------"
        docker version
        echo "----------//docker versionInfo ----------"
    else 
    	echo "docker已安装.........."
    fi
     
    #docker安装---启动docker
    systemctl start docker
    echo "docker已启动.........."
     
    function dockerpull_by_nametag_with_checkexist()
    {
    	image_name_tag=$1
    	image_id=`docker images -q $image_name_tag`
    	if [ x"$image_id" == x"" ]; then
    		echo "拉取$image_name_tag镜像......"
    		docker pull $image_name_tag
    	else 
    		echo "$image_name_tag镜像已存在......"
    	fi
    }
    
    #拉取devicefarmer stf镜像
    dockerpull_by_nametag_with_checkexist devicefarmer/stf:latest
     
    #拉取adb镜像
    dockerpull_by_nametag_with_checkexist sorccu/adb:latest
     
    #拉取ambassador镜像
    dockerpull_by_nametag_with_checkexist devicefarmer/ambassador:latest
     
    #拉取rethinkdb数据库镜像
    dockerpull_by_nametag_with_checkexist rethinkdb:latest
     
    #拉取nginx镜像
    dockerpull_by_nametag_with_checkexist nginx:latest
    
    function check_docker_run_status_by_containername()
    {
    	container_name=$1
    	echo "检查$container_name容器启动状态......"
    	container_run_id=`docker ps --filter name=$container_name -q`
    	if [ x"$container_run_id" == xvi "" ]; then
    		echo "$container_name容器未启动......"
    		
    		echo "检查$container_name容器ID挂载状态......"
    		container_exist_id=`docker ps -a --filter name=$container_name -q`
    		if [ x"$container_exist_id" != x"" ]; then
    			echo "$container_name容器ID已挂载 准备删除......"
    			docker rm $container_exist_id
    		else
    			echo "$container_name容器ID未挂载......"
    		fi
    		
    		return 1
    	else
    		echo "$container_name容器已启动......"
    		return 0
    	fi
    }
    
    check_docker_run_status_by_containername rethinkdb
    if [ $? == 1 ]; then
    #启动数据库
    echo "正在启动数据库......"
    docker run -d --name rethinkdb -v /srv/rethinkdb:/data --net host rethinkdb rethinkdb --bind all --cache-size 8192 --http-port 8090
    echo "数据库启动完成......."
    fi
     
    #注意：只作为服务器不需要启动
    #启动adb service 
    #echo "正在启动adb......."
    #docker run -d --name adbd --privileged -v /dev/bus/usb:/dev/bus/usb/ --net host sorccu/adb:latest
    #echo "adb启动完成......."
    
    check_docker_run_status_by_containername stf
    if [ $? == 1 ]; then
    #启动stf
    echo "正在启动stf......."
    docker run -d --name stf --privileged=true --net host devicefarmer/stf stf local --public-ip $IP --allow-remote
    echo "stf启动完成......."
    fi
    
    # 展示已启动的容器
    docker ps
    ```

    

    stf-restart.sh

    ``` shell
    #!/bin/bash
     
    #注意：只作为服务器不需要启动
    #echo "`docker start adbd`已启动......."
    
    echo "`docker stop rethinkdb`已关闭......"
    echo "`docker start rethinkdb`已启动......."
    
    echo "`docker stop stf`已关闭......"
    echo "`docker start stf`已启动......."
     
    # 展示已启动的容器
    docker ps
    ```

    

    stf-reset.sh

    ``` shell
    #!/bin/bash
    
    echo "准备关闭stf provider 端口 5037"
    stf_port5037_pid=`ps -ef | grep 5037 | grep stf | awk '{print $2}'`
    echo "`kill -s 9 ${stf_port5037_pid}`占用端口5037的进程${stf_port5037_pid}已关闭......"
    
    echo "`docker stop stf`已关闭......"
    stf_contain_id=`docker ps -a --filter name=stf -q`
    echo "`docker rm $stf_contain_id`容器已删除"
    
    if [ ! -f "./stf-setup-run-linux-centos7.sh" ]; then
    	echo "找不到stf重新启动脚本"
    	exit 1
    fi
if [ ! -x "./stf-setup-run-linux-centos7.sh" ]; then
       chmod +x "./stf-setup-run-linux-centos7.sh"
fi
    sh stf-setup-run-linux-centos7.sh
```
    
    
    
    stf-reinstall.sh
    
    ``` shell
    i#!/bin/bash
    
    echo "准备关闭stf provider 端口 5037"
    stf_port5037_pid=`ps -ef | grep 5037 | grep stf | awk '{print $2}'`
    echo "`kill -s 9 ${stf_port5037_pid}`占用端口5037的进程${stf_port5037_pid}已关闭......"
    
    echo "`docker stop stf`已关闭......"
    stf_contain_id=`docker ps -a --filter name=stf -q`
    echo "`docker rm $stf_contain_id`容器已删除"
    
    stf_image_id=`docker images | grep devicefarmer/stf | awk '{print $3}'`
    echo "`docker rmi $stf_image_id`镜像已删除"

    if [ ! -f "./stf-setup-run-linux-centos7.sh" ]; then
    	echo "找不到stf重新启动脚本"
    	exit 1
    fi
    if [ ! -x "./stf-setup-run-linux-centos7.sh" ]; then
       chmod +x "./stf-setup-run-linux-centos7.sh"
    fi
    sh stf-setup-run-linux-centos7.sh
    ```
    
    
    
    
    
    stf-provider-add-device-host-ip.sh
    
    ``` shell
    #!/bin/bash
    
    IP=`hostname -I | awk '{print $1}'`
    echo $IP
    
    echo "开启STF远程主机连接设备......"
    echo "如果连接成功，不要关闭这个命令窗口"
    TargetIP=$1
    if [ x"$TargetIP" == x"" ]; then
    	echo "没有输入目标主机IP"
    	exit 1
    fi
    echo "目标主机IP: ${TargetIP}, Port: 5037"
    stf_contain_id=`docker ps --filter name=stf -q`
    if [ x"$stf_contain_id" == x"" ]; then
    	echo "stf没有启动"
    	exit 1
    fi
    docker exec -it ${stf_contain_id} /bin/sh -c "stf provider --name `hostname` --min-port 7400 --max-port 7700 --connect-sub tcp://127.0.0.1:7114 --connect-push tcp://127.0.0.1:7116 --group-timeout 900 --public-ip $IP --storage-url http://$IP:7100/ --adb-host ${TargetIP} --adb-port 5037 --vnc-initial-size 600x800 --mute-master never --allow-remote"
    ```
    
    





* macos (待写 系统版本) + brew + stf搭建  （**更新于2021/04/13**）

  ``` shell
  #安装node 8  
  node -v #查看版本号
  
  #android 环境 
  #安装adb
  
  #其他依赖库
  brew install rethinkdb graphicsmagick zeromq protobuf yasm pkg-config
  
  #源码（建议github copy ： git@github.com:DeviceFarmer/stf.git）  
  
  #启动
  rethindb 
  #启动stf服务
  stf local --public-ip <本机ip> --allow-remote
  ```



* ubuntu 18 + docker compose + stf搭建 

  [STF 使用 Docker Compose 部署 OpenSTF](https://testerhome.com/topics/17233)



* 连接其他操作系统设备

  [STF 手机设备管理平台------ 连接其它操作系统上的安卓设备实操介绍](https://blog.csdn.net/xl_lx/article/details/79445862)

  [Centos7中docker的stf 连接 局域网中 windows下的Android机](https://blog.csdn.net/shylcok/article/details/107803797)

  ``` shell
  #本机开放端口 等待连接在其他主机上的设备连接到本机stf
  #其他主机也开放如下端口
  # adb version > 1.0.32   otherwise use adb -a -P 5037 fork-server server
  adb nodaemon server -a -P 5037
  
  
  ############### WIN ##################
  #乱码问题
  #PS C:\Users\tenmove\Desktop> adb nodaemon server -a -P 5037
  #adb.exe F 04-15 15:54:55 55168 53460 main.cpp:153] could not install *smartsocket* listener: cannot bind to 0.0.0.0:5037: 閫氬父姣忎釜濂楁帴瀛楀湴鍧€(鍗忚/缃戠粶鍦板潃/绔彛)鍙厑璁镐娇鐢ㄤ竴娆°€?(10048)
  
  #检查端口被占用
  netstat -aon | findstr 5037
   #例如输出>> TCP 127.0.0.1:5037 xxx LISTENING 3364
  #端口如果被占用时
  taskkill /pid 3364 /f
  ######################################
  
  
  #配置adb host连接其他主机ip
  stf provider --name XXXMac-mini.local --min-port 7400 --max-port 7700 --connect-sub tcp://127.0.0.1:7114 --connect-push tcp://127.0.0.1:7116 --group-timeout 20000 --public-ip (本机IP) --storage-url http://（本机IP）:7100/ --adb-host (对方电脑IP) --adb-port 5037 --vnc-initial-size 600x800 --allow-remote
  
  #如果其他主机也安装了stf  
  #可以省略 --adb host 对方电脑IP 这一参数
  stf provider --name XXXMac-mini.local --min-port 7400 --max-port 7700 --connect-sub tcp://127.0.0.1:7114 --connect-push tcp://127.0.0.1:7116 --group-timeout 20000 --public-ip (本机IP) --storage-url http://(本机IP):7100/ --vnc-initial-size 600x800 --allow-remote
  
  
  #如果主机系统是安装在虚拟机上的  连接设备时，需要连接到虚拟机所在的主机上
  #可以adb devices检查设备连接情况
  ```

  ``` text
  adb 报错：
  INF/provider 46 [*] Receiving input from "tcp://127.0.0.1:7114"
  Unhandled rejection Error: spawn adb ENOENT
  
  解决：
  1.docker stop stf
  2.ps -ef | grep 5037   （需要删除宿主机的5037端口，即宿主机上不需要启动 adb nodaemon server -a -P 5037 ）
    kill -s 9 PID    
  3.adb -a -P 5037 fork-server server  //  adb nodaemon server -a -P 5037
  4.docker run -d --name stf --privileged=true --net host devicefarmer/stf stf local --public-ip 192.168.182.129 --adb-host 192.168.4.92 --adb-port 5037 --allow-remote
  
  ```

  



* 二次开发 + 发布docker

  [STF STF 正式环境 docker 化集群部署](https://testerhome.com/topics/12755)

  [移动设备管理平台的搭建（基于STF/ATXServer2）](https://segmentfault.com/a/1190000038807238)

  ``` text
  推荐使用  ATXServer2
  基于ATXServer2搭建移动设备管理平台
  基于python3
还支持IOS
  https://github.com/openatx/atxserver2
  ```
  
  



---



### 问题

* 运行时问题

  * 小米设备连接报错

    ``` text
    尝试解决小米10 minicap报错的问题 ---> aosp问题 替换minicap.so
    ```

    [LG G7 ThinQ update(android9-sdk28) issue. minicap aborted.](https://github.com/openstf/minicap/issues/169)

    [Vector<> have different types XI AOMI MI_10 MIUI_12 Android 10 (SDK 29) #6](https://github.com/DeviceFarmer/minicap/issues/6)

    [Vector<> have different types —— XIAOMI MI_8 Android 9 (sdk 28) #2](https://github.com/DeviceFarmer/minicap/issues/2)

    [github - MinicapBinaries - varundtsfi](https://github.com/varundtsfi)
    
  * connect-sub 和 connect-push 没有配置为localhost ip
  
    ``` text
    INF/provider 336 [*] Providing all 0 of 1 device(s); ignoring "XXXXXXXXXXXXXXXXXXXXXXXX"
    ```
  
    ``` shell
    stf provider --name XXXMac-mini.local --min-port 7400 --max-port 7700 --connect-sub tcp://127.0.0.1:7114 --connect-push tcp://127.0.0.1:7116 --group-timeout 20000 --public-ip (本机IP) --storage-url http://(本机IP):7100/ --vnc-initial-size 600x800 --allow-remote
    
    #--connect-sub tcp://127.0.0.1:7114 --connect-push tcp://127.0.0.1:7116  不能写成（本机ip）
    ```
  
  * slave启动adb server 5037 连接上device，master中开启provider能成功，但是在网页上能显示连接上的设备，但是设备黑屏，没图像
  
    ``` text
    2021-04-16T06:04:11.224Z INF/device:plugins:touch 358 [PKT0220804001995] minitouch says: "open: Permission denied"
    2021-04-16T06:04:11.225Z INF/device:plugins:touch 358 [PKT0220804001995] minitouch says: "Unable to open device /dev/input/event3 for inspectionopen: Permission denied"
    2021-04-16T06:04:11.226Z INF/device:plugins:touch 358 [PKT0220804001995] minitouch says: "Unable to open device /dev/input/mouse0 for inspectionopen: Permission denied"
    2021-04-16T06:04:11.228Z INF/device:plugins:touch 358 [PKT0220804001995] minitouch says: "Unable to open device /dev/input/mice for inspectionopen: Permission denied"
    2021-04-16T06:04:11.229Z INF/device:plugins:touch 358 [PKT0220804001995] minitouch says: "Unable to open device /dev/input/event2 for inspectionopen: Permission denied"
    2021-04-16T06:04:11.230Z INF/device:plugins:touch 358 [PKT0220804001995] minitouch says: "Unable to open device /dev/input/event0 for inspectionopen: Permission denied"
    2021-04-16T06:04:11.231Z INF/device:plugins:touch 358 [PKT0220804001995] minitouch says: "Unable to open device /dev/input/event1 for inspectionopen: Permission denied"
    2021-04-16T06:04:11.233Z INF/device:plugins:touch 358 [PKT0220804001995] minitouch says: "Unable to open device /dev/input/event5 for inspectionopen: Permission denied"
    2021-04-16T06:04:11.234Z INF/device:plugins:touch 358 [PKT0220804001995] minitouch says: "Unable to open device /dev/input/event4 for inspectionUnable to find a suitable touch device"
    
    注意：上述报错可以认为是正常日志
    
    因为当能显示设备图像时，日志会输出 minitouch says: "正常信息..."
    ```
  
    ``` shell
    #不能输出正常信息的原因可能是：
    stf provider --name XXXMac-mini.local --min-port 7400 --max-port 7700 --connect-sub tcp://127.0.0.1:7114 --connect-push tcp://127.0.0.1:7116 --group-timeout 20000 --public-ip (本机IP) --storage-url http://(本机IP):7100/ --vnc-initial-size 600x800 --allow-remote
    
    #本机IP没有设置！！！
    #所以说执行shell命令时，最好打印出命令执行的语句  
    
    #使用 set -v   ||    set -o verbose
    #使用 #!/bin/bash -v
    
    #使用 set -x   
    #使用 #!/bin/bash -x
    
    #使用 bash -v  script.sh
    #使用 bash -x script.sh
    ```
  
    



* 环境配置问题

  ``` text
  Q: 执行 idevicescreenshot -u UUID UUID.img 时 报错 Xcode error “Could not find Developer Disk Image” ...
  
  A: 1. 可能是设备升级后 Xcode版本不支持
  /Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/DeviceSupport  
  即使下载对应版本的support 放入上述路径 也不行 需要安装对应版本的xcode
  ```




---



