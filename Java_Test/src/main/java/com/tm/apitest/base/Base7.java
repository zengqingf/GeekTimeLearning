package com.tm.apitest.base;

public class Base7 {
    /*
    * classpath 是JVM用到的一个环境变量，它用来指示JVM如何搜索class
    *
    *
    * classpath就是一组目录的集合，它设置的搜索路径与操作系统相关
    * windows 例如: C:\work\project1\bin;C:\shared;"D:\My Documents\project1\bin"
    * linux 例如: /usr/shared:/usr/local/bin:/home/liaoxuefeng/bin
    *
    *
    * 强烈不推荐在系统环境变量中设置classpath，那样会污染整个系统环境。在启动JVM时设置classpath才是推荐的做法。实际上就是给java命令传入-classpath或-cp参数：
    * 例如：java -classpath .;C:\work\project1\bin;C:\shared abc.xyz.Hello
    *     或者简写：java -cp .;C:\work\project1\bin;C:\shared abc.xyz.Hello
    *
    *
    * 没有设置系统环境变量，也没有传入-cp参数，那么JVM默认的classpath为.，即当前目录：
    * java abc.xyz.Hello
    *
    *
    * 不要把任何Java核心库添加到classpath中！JVM根本不依赖classpath加载核心库！
    * 注意：JVM自带的标准库rt.jar不要写到classpath中，写了反而会干扰JVM的正常运行。  * java -cp . 完整类名
     *
     *
     *
     *
     *
     * jar包即 zip包
     * 把package组织的目录层级，以及各个目录下的所有文件（包括.class文件和其他文件）都打成一个jar文件
     * 注意压缩包内的目录结构
     * 从包名开始
     * jar包还可以包含一个特殊的/META-INF/MANIFEST.MF文件，MANIFEST.MF是纯文本，可以指定Main-Class和其它信息。
     * JVM会自动读取这个MANIFEST.MF文件，如果存在Main-Class，我们就不必在命令行指定启动的类名”
     * 例如：java -jar hello.jar
     * jar包还可以包含其它jar包，需要在MANIFEST.MF文件里配置classpath
     *
     * Maven工具可以创建jar包
     *
     *
     *
     *
     * 模块 (jmod)
     * 不同于jar只是存放class的容器，模块还关心class之间的依赖
     * 模块支持jar间的依赖关系
     * 把一堆class封装为jar仅仅是一个打包的过程，
     * 而把一堆class封装为模块则不但需要打包，还需要写入依赖关系，并且还可以包含二进制代码（通常是JNI扩展）。
     * 此外，模块支持多版本，即在同一个模块中可以为不同的JVM提供不同的版本
     *
     *
     * 模块操作见 ： ./Java_Test/_command_projects/oop-module/module_test_cmd.bat
     *
     *
     *
    *
    * 推荐执行class方法：
    * 在当前目录执行

    * */
}
