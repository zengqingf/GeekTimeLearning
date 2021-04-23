# Linux环境配置

### linux镜像

[Linux系统下载](https://man.linuxde.net/download/)



### android

* adb 

  [centos7安装adb](https://blog.csdn.net/zhesir/article/details/77542859)

* java

  [linux安装jdk环境（多种方式）](https://blog.csdn.net/lyhkmm/article/details/79524712)



---



* 虚拟机

  基于VMware Workstation 15 pro / v.15.5.2

  [linux-centos7-局域网内访问虚拟机](https://blog.csdn.net/Varose/article/details/98791852)

  ``` text
  #ifconfig 获取 虚拟机ip
  #vmware菜单栏 - 编辑 - “虚拟网络编辑器”
  #更改NAT模式的网络设置
  #NAT设置，添加一个映射关系，把虚拟机ip和需要访问的port填入，主机端口可以自定义（不重复）
  #局域网访问：本机ip:主机port
  ```

  ![](https://i.loli.net/2021/04/13/jHny2LgEIdcG7FP.png)

  ![](https://i.loli.net/2021/04/13/42lZWhSGrOKNgmY.png)

  ![](https://i.loli.net/2021/04/13/7b6TSqsNHBh9UDR.png)

  ![](https://i.loli.net/2021/04/13/YSOjKtv9V6aRmAq.png)





---



* linux 常用命令

  ``` shell
  nautilus 直接打开Home
  nautilus . 打开当前文件夹
  nautilus /var/www/aaa/ 打开此文件夹
  
  
  ```

  