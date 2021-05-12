# C++编译



* 资源

  [CMake download](https://cmake.org/download/)

  [CodeLite download](https://downloads.codelite.org/)





---



### centos7环境搭建

``` shell
#env：centos 7 64bit   su ==> root
#cmake 3.20.2
#virtualbox : VMware® Workstation 15 Pro

#1
yum install -y gcc gcc-c++ make automake

#2
yum remove cmake

#3  cmake
yum install -y wget
wget https://github.com/Kitware/CMake/releases/download/v3.20.2/cmake-3.20.2.tar.gz
#unable to establish SSH connection
#先下载到本地 再上传

#4
tar -zxvf cmake-3.20.2.tar.gz -C /tmp
cd /root/tmp/cmake-3.20.2

#5
yum install -y openssl openssl-devel

#6
./bootstrap
make (gmake)    # /usr/bin/gmake -> make
make (gmake) install 



#1 codelite
wget https://github.com/eranif/codelite/archive/15.0.tar.gz
#先下载到本地 再上传
tar -zxvf codelite-15.0.tar.gz -C /tmp

vi BuildInfo.txt
---> Linux/FreeBSD:  wxWidgets 2.9.5 or later  gtk2 development package  cmake

#2
yum install -y gtk2*

#3 过期了！
wget http://sourceforge.net/projects/wxwindows/files/2.9.5/wxWidgets-2.9.5.tar.bz2
tar -jxf wxWidgets-2.9.5.tar.bz2 -C /tmp
cd /tmp/wxWidgets-2.9.5
#需要卸载老版本
rm -rf /usr/bin/wx*
rm -rf /usr/include/wx*
rm -rf /usr/lib/wx
rm -rf /usr/lib/libwx*
rm -rf /usr/local/bin/wx*
rm -rf /usr/local/include/wx*
rm -rf /usr/local/lib/wx
rm -rf /usr/local/lib/libwx*
ldconfig


#3
wget https://github-releases.githubusercontent.com/1764646/3647d400-9bf2-11eb-9f09-9c4043b2b1ca?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAIWNJYAX4CSVEH53A%2F20210508%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20210508T091352Z&X-Amz-Expires=300&X-Amz-Signature=e69bb0496f3654523c26872d32d56aee0d10876911ceec4d159982db526d1263&X-Amz-SignedHeaders=host&actor_id=11548926&key_id=0&repo_id=1764646&response-content-disposition=attachment%3B%20filename%3DwxWidgets-3.1.5.tar.bz2&response-content-type=application%2Foctet-stream

tar -jxf wxWidgets-3.1.5.tar.bz2 -C /tmp
cd /tmp/wxWidgets-3.1.5

#4
./configure #(--enable-unicode --enable-optimise)
make
make install
#5
#提示需要配置环境变量 LD_LIBRARY_PATH
ldconfig
./wx-config --libs
#输出...
#-L/tmp/wxWidgets-2.9.5/lib -pthread   -Wl,-rpath,/tmp/wxWidgets-2.9.5/lib -lwx_gtk2u_xrc-2.9 -lwx_gtk2u_html-2.9 -lwx_gtk2u_qa-2.9 -lwx_gtk2u_adv-2.9 -lwx_gtk2u_core-2.9 -lwx_baseu_xml-2.9 -lwx_baseu_net-2.9 -lwx_baseu-2.9

#6
vim /etc/profile
#添加 export LD_LIBRARY_PATH="填入输出..."
export LD_LIBRARY_PATH="..."
export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:/usr/local/lib
source /etc/profile


#1
cd codelite-15.0
mkdir build-release
cd build-release

#2
#需要加../  找CMakeList.txt
cmake ../ -G "Unix Makefiles" -DCMAKE_BUILD_TYPE=Release
#error: 缺少libsqlite3-dev package
yum list | grep sqlite

#3
yum install -y sqlite-devel.x86_64

#4
#requires at lease wxWidgets-3.0.0
#需要清理已经安装的wx... 清理方法如上

#5
#安装libssh
wget https://www.libssh.org/files/0.9/libssh-0.9.5.tar.xz
cd libssh-0.9.5
mkdir build
cd build
cmake ../ -DCMAKE_BUILD_TYPE=Debug
make
make install

#6 
#重新执行codelite的
cmake ../ -G "Unix Makefiles" -DCMAKE_BUILD_TYPE=Release
make
make install

#for libssh.so 
yum install clang
yum install libssh

#test
codelite
#end...
```









---



### 编译器

* gcc vs g++

  ``` shell
  gcc -E test2.c -o test2.i    #预编译  .c文件 -> .i文件  
  							#1. 去掉#开头程序，保留#pragma
  							#2. 宏替换
  							#3. 去注释
  							#4. 头文件包含
  							#5. 添加行号
  							
  gcc -S test2.i -o test2.s    #编译   .i文件 ->  .s文件
  							#1. 语法分析
  							#2. 词法分析
  							#3. 生成相应的汇编代码
  							
  gcc -c test2.s -o test2.o    #汇编   .s文件 -> .o文件
  							#1. 将汇编代码转换成相应的机器语言
  					
  gcc test2.o -o test2		 #链接   .o文件 ->  可执行程序
  							#1. 通过调用链接器ld来链接程序运行需要的一大堆目标文件，以及所依赖的其他库文件，最后生成可执行文件
  							
  							
  对于简单的编译c文件
  可以直接执行
  gcc test2.c -o test2_
  						
  对于C文件gcc和g++所做的事情确实是一样的，g++在编译C文件时调用了gcc
  
  gcc和g++的区别主要是在对cpp文件的编译和链接过程中，因为cpp和c文件中库文件的命名方式不同，那为什么g++既可以编译C又可以编译C++呢，这时因为g++在内部做了处理，默认编译C++程序，但如果遇到C程序，它会直接调用gcc去编译.
  
  
  编译cpp文件
  gcc无法编译cpp
  分析：
  在预处理阶段 gcc和g++都能正常预处理，处理方式相同
  在编译阶段，gcc无法自动和c++库进行连接，会提示库函数没有申明的错误
  在汇编阶段，gcc和g++处理方式应该相同
  在链接阶段，gcc无法将库文件和.o后缀文件关联，生成可以执行程序
  ```
  
  