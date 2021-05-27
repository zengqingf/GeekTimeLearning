# Linux环境配置

### linux镜像

[Linux系统下载](https://man.linuxde.net/download/)

* 系统

  ubuntu （Debian）适合开发环境

  centos（RHEL） 适合稳定运行环境，企业级

  ![](https://i.loli.net/2021/05/08/jk3hSdJbq8FZPLN.jpg)



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






---



* centos

  * yum

    ``` sh
    [如何查看yum 安装的软件路径](https://blog.csdn.net/wd2014610/article/details/79659073)
    #查看yum安装软件路径
    yum install redis
    
    #查找安装包
    rpm -qa|grep redis
    -->redis-3.2.10-2.el7.x86_64
    
    #查找安装包的安装路径
    rpm -ql redis-3.2.10-2.el7.x86_64
    
    
    
    ```

  * ruby

    ``` sh
    # 安装并升级ruby
    yum install ruby
    ruby -v
    
    #添加aliyun镜像
    gem sources -a http://mirrors.aliyun.com/rubygems/ 
    
    #安装ram（ruby version manager）
    gpg --keyserver hkp://keys.gnupg.net --recv-keys 409B6B1796C275462A1703113804BB82D39DC0E3 7D2BAF1CF37B13E2069D6956105BD0E739499BDB
    curl -sSL https://get.rvm.io | bash -s stable
    #更新配置，即时生效
    source /etc/profile.d/rvm.sh
    #查看rvm版本
    rvm -v
    #查看ruby可用版本
    rvm list known
    
    rvm install 2.5
    ruby -v
    
    #如果下载太慢，可以删除默认仓库地址，只保留阿里云镜像（不推荐）
    gem sources --remove https://rubygems.org/
    
    ```

  * 插件

    * 中文转拼音

      [github - chinese_pinyin - flyerhzm](https://github.com/flyerhzm/chinese_pinyin)

      [github - optparse - ruby](https://github.com/ruby/optparse)

      [(shell版)批量自动重命名文件中文转英文](https://blog.csdn.net/hanchaohao2012/article/details/53678319)

      ``` sh
      #2021-05-19
      #环境：ruby >= 2.5   一般都需要更新
      #gem install optparse
      #gem install chinese_pinyin
      #ch2py -h
      ```

      