# Flatbuffer

## CSharp

[github - flatbuffers](https://github.com/google/flatbuffers)



[flatbuffers in unity sample](http://exiin.com/blog/flatbuffers-for-unity-sample-code/)

[flatbuffers writing a schema](https://google.github.io/flatbuffers/md__schemas.html)

[flatbuffers docu](https://google.github.io/flatbuffers/)

[FlatBuffers for Unity, part 1 (overview)](https://gamedev-wunder9l.blogspot.com/2019/12/flatbuffers-for-unity-part-1-overview.html)

[FlatBuffers for Unity, part 2](https://gamedev-wunder9l.blogspot.com/2019/12/flatbuffers-for-unity-part-2.html)



[Google FlatBuffers使用教程](https://www.coder4.com/archives/4386)





[FlatBuffers初探（C#为例）](https://blog.csdn.net/andyqingliu/article/details/96431623)

``` powershell
#生成目标语言代码
#–csharp指定了编译的目标语言。-o则用于指定输出文件路径，如果没有提供则将当前目录作为输出目录。最后是fbs文件名。
flatc.exe --csharp -o Resource/Sample Sample.fbs

# --gen-onefile 表示将文件中的多个结构体生成到一个代码文件中，如果不加这个参数，那么有几个结构体就会生成几个代码文件.
flatc.exe -n --gen-onefile Sample.fbs
```

``` powershell
#将二进制文件转换成json  AddressBook.ab 这个二进制源文件前面有个空格
flatc.exe --raw-binary -t Sample.fbs -- AddressBook.ab
```

 ``` text
部分参数用法:

flatc [ GENERATOR OPTIONS ] [ -o PATH ] [ -I PATH ] [ -S ] FILES…
[ – FILES…]

定义文件按照顺序被读取和编译,还可以包含其他定义文件和数据(详情 见下面).

-- 表示是定义文件是二进制
任意个定义文件可能生成一个或者多个定义文件,这取决于附加的命令选项
- --cpp,-c :按照定义生成C++头文件
- --java,-j :按照定义生成Java代码
- --csharp,-n :按照定义生成C#代码
- --go,-g :按照定义生成Go代码
- --python,-p :按照定义生成python打底吗
- --javascript,-s :按照定义生成JavaScript
- --php :按照定义生成php
数据序列化格式选项
- --binary,-b :序列化成.bin 后缀的二进制格式,
- --json,-t :序列化成.json 后缀的json格式,
其他选项
- -o PATH :输出搜有生成的文件到Path(绝对路径,或者相对于当前目录)路径,如果省略,Path就是当前目录.路径末尾因该是你的系统分隔符\或者/.
- -I PATH :当遇见include声明,试图读取文件的时候将从此路径按照顺序查找,如果失败,就按照相对路径查找
- -M :打印Make规则到生成文件
- --strict-json :要求生成严格的json文件(名字等字段包含在引号中,table和Vector末尾没有逗号),默认 在required/generated时没有引号,末尾逗号是允许的
---defaults-json :当输出json文件本的时候输出字段等于默认值
---no-prefix :当生成C++头文件时 枚举值不包含枚举类型的前缀
---scoped-enums :使用C++11风格作用域和强类型枚举生成C++,也就意味着 --no-prefix
---no-includes :不生成包含include模式的代码,(依赖C++)
---gen-mutable :为可变的FlatBuffers生成额外的non-const访问器
---gen-onefile :生成一个定义文件(用于C#)

Note:缩写的命令选项会被弃用,尽量用全写选项
 ```





[FlatBuffers vs Protocol Buffers](https://blog.csdn.net/chosen0ne/article/details/43033575)

| 记录个数 | FlatBuffers(序列化总时间/文件大小/反序列化时间) | Protocol Buffers(序列化总时间/文件大小/反序列化时间) |
| -------- | ----------------------------------------------- | ---------------------------------------------------- |
| 10000    | 0.197  /  4.4M  /  0.02                         | 0.166  / 3.0M  / 0.08                                |
| 20000    | 0.383  /  8.8M  /  0.03                         | 0.248  / 6.4M  /  0.168                              |
| 30000    | 0.534  / 13M  /  0.05                           | 0.378  / 10M  / 0.26                                 |
| 100000   | 1.937  /  44M  /  0.144                         | 1.314  /  38M  /  0.8                                |
| 200000   | 3.9  /  88M  /  0.276                           | 2.66  /  67M  /  1.542                               |
| 1000000  | 17.59  / 439M  /  1.396                         | 13.355  /  397M  /  8.012                            |



[flatbuffers - windows 编译好的flatc.exe下载](https://github.com/google/flatbuffers/releases)

[官方 - Building with CMake](https://google.github.io/flatbuffers/flatbuffers_guide_building.html)

``` text
生成flatc 

cmake -G "Unix Makefiles" -DCMAKE_BUILD_TYPE=Release
cmake -G "Visual Studio 10" -DCMAKE_BUILD_TYPE=Release
cmake -G "Xcode" -DCMAKE_BUILD_TYPE=Release
```









---



## JAVA

* Build

  [Android 上的数据格式 FlatBuffers 介绍](https://developer.aliyun.com/article/227805)

  [JSON parsing with FlatBuffers in Android](http://frogermcs.github.io/json-parsing-with-flatbuffers-in-android/)

  

  [flatbuffer java](https://www.jianshu.com/p/3504d4643dba)

  [在Android中使用FlatBuffers](https://blog.csdn.net/tq08g2z/article/details/77311553)

  

  [Flatbuffers使用及其源码浅析](https://romantiskt.github.io/2018/04/18/Flatbuffers/)

  [failed to execute goal org.apache.maven.plugins:maven-gpg-plugin](https://stackoverflow.com/questions/32018765/failed-to-execute-goal-org-apache-maven-pluginsmaven-gpg-plugin)

  ``` text
  FlatBuffers 类库build
  
  1. 安装mvn  (Windows:下载apache-maven-3.6.3-bin，并设置环境变量)
  2. 在flatbuffers源码 根目录下（有pom.xml）执行 mvn install
  3. 如果提示gpg.exe找不到，可以将Git安装目录下的 （例如：D:\programs\Git\usr\bin）配置到环境变量中
  4. 如果提示failed to execute goal org.apache.maven.plugins:maven-gpg-plugin
  	a. 可以选择跳过，在pom.xml中配置
          <plugin>
              <groupId>org.codehaus.mojo</groupId>
              <artifactId>maven-gpg-plugin</artifactId>
              <configuration>
                  <skip>true</skip>
              </configuration>
          </plugin>
      b. 或者生成key （测试阶段未尝试2020.12.29记）
     		Open Git Bash
          gpg --gen-key #use defaults with 4096 as key size
          gpg --list-secret-keys --keyid-format LONG
          gpg --armor --export %the key from above%
          
  5. 生成的jar (如：flatbuffers-java-1.12.0.jar) 目录为
  ```

* Tools

  [json生成java实体类](https://www.sojson.com/json2entity.html)

* 扩展

  [原理 - Improving Facebook’s performance on Android with FlatBuffers](https://engineering.fb.com/2015/07/31/android/improving-facebook-s-performance-on-android-with-flatbuffers/)

  [深入浅出 FlatBuffers 之 Schema](https://halfrost.com/flatbuffers_schema/)

  



---



protocol buffers

[google - pb](https://github.com/protocolbuffers/protobuf)

[pb - guide docu](https://developers.google.com/protocol-buffers/docs/overview)





