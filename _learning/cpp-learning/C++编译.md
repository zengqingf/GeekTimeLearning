# C++编译



* 资源

  [CMake download](https://cmake.org/download/)

  [CodeLite download](https://downloads.codelite.org/)



---



* 编译过程

  * 预编译Preprocessing

    ``` text
    在实际编译工作开始之前，预处理器指令指示编译器对源码进行临时扩充，以为之后的步骤做好准备。
    
    在 C++ 中，预处理器指令以 # 号开头，比如 #include、#define 和 #if 等。在这一阶段，编译器逐个处理 C++ 源码文件。对于 #define 指令，编译器将源码中的宏替换成宏定义中的内容；对于 #if、#ifdef 和 #ifndef 指令，编译器将有选择地跳过或选中部分源代码；而对于 #include 指令，编译器将把对应的库的源码插入到当前源代码中——这通常是一些通用的声明。被 #include 指令引入的头文件（ .h ）往往会包含大量的代码，你引入的越多，最后生成的预编译文件就越大。总的来说，预编译过的文件会比原来的 C++ 源码更大一些。
    
    通过上面这些替换和插入操作，预处理器产生的是被合为一体的输出文件。预处理器还会在代码中插入记号，使编译器能分辨出每一行来自哪个文件，以便在调试过程中能生成对应的错误信息。在开发调试你的 C++ 程序时，这些错误信息能给你很多帮助。
    ```

    

  * 编译Compilation & 汇编assembly

    ``` text
    在这一阶段，编译器通过两个连续的步骤，将预处理器产生的代码编译成目标文件（object file）。
    
    首先，编译器将去除了预编处理器指令的纯 C++ 代码编译成底层汇编代码。在这一步中，编译器会对代码进行检查优化，指出语法错误、重载决议错误及其他各种编译错误。在 C++ 中，如果一个对象只声明，不进行定义，编译器仍然可以从源代码产生目标文件——因为这个对象也可以指向某些当前代码中还未定义的标识符。
    
    其次，汇编器将上一步生成的汇编代码逐行转换成字节码（也就是机器码）。实际上，如果你希望把代码的不同部分分开编译的话，编译过程在这一步之后就可以停止了。这一步生成的目标文件可以被放在被称为静态库的包中，以备后续使用——也就是说，如果你只修改了一个文件，你并不需要重新编译整个项目的源代码。
    ```

    

  * 链接Linking

    ```text
    链接器利用编译器产生的目标文件，生成最终结果。
    
    在这一阶段，编译器将把上一阶段中编译器产生的各种目标文件链接起来，将未定义标识符的引用全部替换成它们对应的正确地址。没有把目标文件链接起来，就无法生成能够正常工作的程序——就像一页没有页码的目录一样，没什么用处。完成链接工作之后，链接器根据编译目的不同，把链接的结果生成为一个动态链接库，或是一个可执行文件。
    
    链接的过程也会抛出各种异常，通常是重复定义或者缺失定义等错误。不只是没进行定义的情况，如果你忘记将对某个库或是目标文件的引用导入进来，让链接器能找到定义的话，也会发生这类错误。重复定义则刚好相反，当有两个库或目标文件中含有对同一个标识符的定义时，就可能出现重复定义错误。
    ```

  * 补充

    * gcc

      ![](https://i.loli.net/2021/03/15/u2yoxAvkbiJq5MQ.png)

    * gcc编译c++

      

* 编译器

  * windows

    * Cygwin

      linux工具集，包括gcc；可以用Cygwin运行gcc或者clang，但是生成的代码需要Cygwin才能运行；可以使windows下调用unix-like的系统函数（如进程函数）

    * MinGW/MinGW-w64

      不依赖Cygwin，可以生成原生再windows上运行的可执行程序

    * Visual Studio IDE (MSVC)

    * Intel C++

  * linux

    * gcc编译器工具集，包含在linux发行版的软件包仓库里

  * macos

    * clang是默认编译器，随xcode命令行工具一起安装

  * 在线

    [在线编译器列表](https://arnemertz.github.io/online-compilers/)











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
  
  
  编译步骤，大体可分为：
  1、预处理,生成 .i 的文件[预处理器cpp]
  2、将预处理后的文件转换成汇编语言, 生成文件 .s [编译器egcs]
  3、有汇编变为目标代码(机器代码)生成 .o 的文件[汇编器as]
  4、连接目标代码, 生成可执行程序 [链接器ld]
  ```
  



* gcc、g++编译参数

  [GCC 参数详解](https://www.runoob.com/w3cnote/gcc-parameter-detail.html)

  [gcc/g++常用编译选项和gdb常用调试命令](https://andrewpqc.github.io/2018/11/25/gcc-and-gdb/)

  | gcc/g++编译参数 | 作用                                                         |
  | --------------- | ------------------------------------------------------------ |
  | -c              | 生成目标文件（obj文件），但不进行链接（只激活预处理，编译，汇编） |
  | -C              | 预处理时补删除注释信息，一般和-E一起使用                     |
  | -M              | 生成文件关联信息，包含目标文件所依赖的所有源代码             |
  | -S              | 生成汇编代码（只激活预处理和编译）                           |
  | -E              | 预处理，不生成文件，需要主动指定到输出文件里（使用>）        |
  | -o              | 指定生成文件的文件名（默认为.out，可以根据系统改）           |
  | -g              | 在目标文件中添加调试信息，便于gdb调试或objdump反汇编         |
  | -Wall           | 显示所有的警告信息(建议使用)                                 |
  | -Werror         | 视警告为错误，出现警告即放弃编译                             |
  | -w              | 不显示任何警告信息(不建议使用)                               |
  | -v              | 显示编译步骤                                                 |
  | -On             | (n=0,1,2,3) 设置编译器优化等级，O0为不优化，O3为最高等级优化，O1为默认优化等级 |
  | -L              | 指定库文件的搜索目录                                         |
  | -l              | (小写的L)链接某一库                                          |
  | -I              | (大写的i)指定头文件路径                                      |
  | -D              | 定义宏，例如-DAAA=1,-DBBBB                                   |
  | -U              | 取消宏定义，例如-UAAA                                        |
  | -static         | 禁止使用共享链接，如当存在多份dll，导致找不到程序输入点时，可以添加 |
  | -shared         | 生成共享目标文件，通常用于建立共享库                         |



* gdb调试命令

  ``` shell
  #必须在编译时加上调试信息，也即是加上-g选项，才能被gdb调试
  gcc -g example.c -o example
  
  #调试可执行文件, 进入gdb调试命令行
  gdb example
  #或者分两步
  gdb
  file example
  
  #查看源码，默认显示10行，按回车显示后10行，直至文件末尾
  l
  list
  
  #打断点
  b （函数名、行号、文件名：行号、行号 if条件）
  break
  
  #查看断点
  info break
  
  #禁用、解禁断点
  disable <break number>
  enable <break number>
  
  #删除
  delete <break number>
  
  #启动程序
  r
  run
  
  #查看变量的值
  p <variable name> （变量名）
  print <variable name> 
  
  #单步执行
  n
  next
  
  #跳入跳出函数
  s    #跳入函数需要在函数中预先打断点
  step    
  finish  #跳出
  
  #监控变量
  watch <variable name> 
  
  #显示变量的值
  display <variable name>
  
  #进入shell
  shell 
  exit #离开shell，回到gdb
  ```

  





---



* volatile关键字的作用

  [C语言中volatile关键字的作用](https://blog.csdn.net/tigerjibo/article/details/7427366)

  * 编译器优化相关

    ``` tex
    硬件层优化：
    由于内存访问速度远不及CPU处理速度，为提高机器整体性能，在硬件上引入硬件高速缓存Cache，加速对内存的访问。
    另外在现代CPU中指令的执行并不一定严格按照顺序执行，没有相关性的指令可以乱序执行，以充分利用CPU的指令流水线，提高执行速度。
    
    软件层优化：
    在编写代码时由程序员优化
    由编译器进行优化
    
    编译器优化常用的方法有：将内存变量缓存到寄存器；调整指令顺序充分利用CPU指令流水线，常见的是重新排序读写指令。
    
    由编译器优化或者硬件重新排序引起的问题的解决办法是在从硬件（或者其他处理器）的角度看必须以特定顺序执行的操作之间设置内存屏障（memory barrier）
    
    linux提供了 void Barrier(void) 通知编译器插入一个内存屏障，但对硬件无效，编译后的代码会把当前CPU寄存器中的所有修改过的数值存入内存，需要这些数据的时候再重新从内存中读出。
    
    
    编译器优化
    在本次线程中，当读取一个变量时，为提高存取速度，编译器优化时有时会先把变量读取到一个寄存器中
    之后，若再取变量值时，就直接从寄存器中取值
    当变量值在本线程中改变时，会同时把变量新值copy到寄存器中，保持一致
    当变量值被其他线程改变时，该寄存器的值不会改变，从而导致应用程序读取的值和实际的值不一致了
    当变量值被其他线程改变时，原变量的值不会改变，从而导致应用程序读取的值和实际的值不一致了
    ```

  * volatile作用简述

    ``` tex
    volatile 可解释为 直接存取原始内存地址
    
    volatile的本意是“易变的” 因为访问寄存器要比访问内存单元快的多,所以编译器一般都会作减少存取内存的优化，但有可能会读脏数据。
    当要求使用volatile声明变量值的时候，系统总是重新从它所在的内存读取数据，即使它前面的指令刚刚从该处读取过数据。
    精确地说就是，遇到这个关键字声明的变量，编译器对访问该变量的代码就不再进行优化，从而可以提供对特殊地址的稳定访问；
    如果不使用valatile，则编译器将对所声明的语句进行优化。
    （简洁的说就是：
    volatile关键词影响编译器编译的结果，用volatile声明的变量表示该变量随时可能发生变化，与该变量有关的运算，不要进行编译优化，以免出错）
    ```

    * 示例1

      ``` c
      //告诉编译器不做任何优化
      int *ip = ...;//地址
      *ip = 1; //第一个指令
      *ip = 2; //第二个指令
      //compiler优化后
      int *ip = ...;
      *ip = 2;
      //第一个指令丢失
      //如果用volatile，compiler不会做任何优化，保持程序的原意
      volatile int *ip = ...;
      *ip = 1;
      *ip = 2;
      ```

    * 示例2

      ``` c
      //volatile定义的变量会在程序外被改变，每次都必须从内存中读取，而不能重复使用放在cache或寄存器中的备份
      volatile char a;
      a = 0;
      while(!a) {
          //do sth
      }
      doOther();
      //如果没有volatile  doOther()不会被执行
      ```

    * 使用场景1

      中断服务程序中修改的供其他程序检测的变量  

      ``` c
      static int i = 0;
      int main(void)
      {
          while(1) {
             if(i)
                 dosth();
          }
      }
      
      /* interrupt service routine */
      void ISR_2(void)
      {
          i = 1;
      }
      
      /*
      只会执行一次从i到寄存器得读操作，然后每次判断只会使用寄存器中得i值（副本）
      如果变量i加上volatile，则编译器保证对此变量读写不会进行优化（保证每次直接从内存中读取）
      */
      ```

    * 使用场景2

      ``` tex
      多任务环境下 各任务间共享得标志位 应该加 volatile
      ```

    * 使用场景3

      ``` tex
      存储器映射的硬件寄存器通常也要加voliate，因为每次对它的读写都可能有不同意义
      ```

      ``` c
      int *output = (unsigned int *)0xff800000;  //定义一个IO端口
      int init(void)
      {
          int i;
          for(i = 0; i< 10; i++) {
              *output = i;
          }
      }
      /*
      经过编译器优化后，编译器会忽略前面的循环，最终结果相当于 *output = 9
      */
      //调整为
      volatile int *output = (volatile unsigned int *)0xff800000; //定义了一个I/O端口
      ```

    * 1、2、3小结

      ``` tex
      上述几种场景还需要考虑数据的完整性（相互关联的几个标志读了一半被打断了重写）
      1：通过关中断实现
      2：禁止任务调度
      3：依靠硬件的良好设计
      ```

  * volatile Q&A

    ``` tex
    Q: 一个参数既可以是const也可以是volatile
    A: 可以，只读的状态寄存器， volatile表示它可能被意想不到的改变；const表示程序不应该去修改它
    
    Q: 一个指针可以是volatile
    A：可以
    ```

  * 问题实例1

    ``` c
    int square(volatile int *ptr)
    {
        return (*ptr) * (*ptr);
    }
    
    //==>
    int square(volatile int *ptr)
    {
        int a, b;
        a = *ptr;
        b = *ptr;
        return a * b;  //a 和 b可能不一致
    }
    
    //正确写法
    int square(volatile int *ptr)
    {
        int a;
        a = *ptr;
        return a * a;
    }
    ```

    

    