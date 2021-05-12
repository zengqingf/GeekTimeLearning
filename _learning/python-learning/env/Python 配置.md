# Python 配置

* 安装

  * linux

    [Linux安装python3.6](https://www.cnblogs.com/kimyeee/p/7250560.html)

    [Linux下安装Python](https://blog.csdn.net/gdkyxy2013/article/details/79457590)

    ``` shell
    #切换到root用户下
    sudo su
    
    #安装python依赖的其他模块
    
    #测试通过的
    yum -y install zlib-devel bzip2-devel openssl-static openssl-devel ncurses-devel sqlite-devel readline-devel tk-devel gdbm-devel db4-devel libpcap-devel xz-devel \ 
    libffi-devel #（python 3.7 up need）
    
    #另外的
    yum -y install zlib zlib-devel
    yum -y install bzip2 bzip2-devel
    yum -y install ncurses ncurses-devel
    yum -y install readline readline-devel
    yum -y install openssl openssl-devel
    yum -y install openssl-static
    yum -y install xz lzma xz-devel
    yum -y install sqlite sqlite-devel
    yum -y install gdbm gdbm-devel
    yum -y install tk tk-devel
    yum install gcc
    
    
    #下载python源码   后缀 tgz
    #上传到远端虚拟机中
    # 或者直接下载：wget https://www.python.org/ftp/python/3.6.1/Python-3.6.1.tgz
    
    #创建安装目录
    mkdir -p /usr/local/python3
    
    #解压tgz
    tar -zxf Python-xxx.tgz
    cd Python-xxx
    ./configure --prefix=/usr/local/python3
    make && make install
    
    #创建软链接
    ln -s /usr/local/python3/bin/python3 /usr/bin/python3
    
    #加入PATH
    # vim ~/.bash_profile
    # .bash_profile
    # Get the aliases and functions
    if [ -f ~/.bashrc ]; then
    . ~/.bashrc
    fi
    # User specific environment and startup programs
    PATH=$PATH:$HOME/bin:/usr/local/python3/bin
    export PATH
    
    source ~/.bash_profile
    
    #检查版本
    python -V
    pip3 -V
    
    #补充 创建pip3软链接
    ln -s /usr/local/python3/bin/pip3 /usr/bin/pip3
    ```

    







---

* 问题

  * 问题1

    Q：

    ``` text
    > pip2
    Fatal error in launcher: Unable to create process using '"c:\python27\python.exe"  "C:\Python27\Scripts\pip2.exe" ': ???????????
    > pip3
    Fatal error in launcher: Unable to create process using '"c:\python38\python3.exe"  "C:\Python38\Scripts\pip3.exe" ': ???????????
    
    ```

    A：

    ``` text
    Use 010 editor, edit C: \ Python27 \ Scripts \ pip2.exe,
    
    Amended as follows catalog program name
    ```

    ![](https://i.loli.net/2021/05/07/6Id25VMy9KqsfmU.png)

    ![](https://i.loli.net/2021/05/07/kDmCEV45cplXA8a.png)







