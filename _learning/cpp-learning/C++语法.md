# C++语法

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

* forward declaration of class 类的前向声明

  ``` text
  可以声明一个类而不定义它
     class Screen;//declaration of the Screen class
     这个声明,有时候被称为前向声明(forward declaration),在程序中引入了类类型的Screen.在声明之后,定义之前,类Screen是一个不完全类型(incompete type),即已知Screen是一个类型,但不知道包含哪些成员.
     不完全类型只能以有限方式使用,不能定义该类型的对象,不完全类型只能用于定义指向该类型的指针及引用,或者用于声明(而不是定义)使用该类型作为形参类型或返回类型的函数.
     
     使用前置声明是因为不想引入该类的头文件，但是前置声明是有限制的，只能用来定义前置声明类的指针或者引用。如果继承了该类，则不能使用前置声明，因为继承是需要了解类的内部结构的
  ```

  ``` c++
  //e.g.
  #ifndef FIRSTPAGE_H
  #define FIRSTPAGE_H
  
  #include "ui_dialog.h"
  //#include <QWizardPage>
  class QWizardPage;              //报错了
  
  class FirstPage : public Ui::Dialog, public QWizardPage
  {
  public:
      FirstPage();
  };
  #endif // FIRSTPAGE_H
  ```

  ``` text
  类似的报错信息：
  Syntax error missing ; before *
  ```
  
  

* 数组

* static_cast     vs.     dynamic_cast      vs.     reinterpret_cast     vs.    const_cast 