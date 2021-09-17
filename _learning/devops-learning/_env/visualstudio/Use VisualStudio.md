# Visual Studio使用

**基于VS 2017**

* 设置#include中的头文件或者库文件的路径

  ``` tex
  需要引入msgpack-c-cpp_master
  将下图中的目录放到工程目录下，将需要的文件包含到项目里
  
  打开菜单栏“项目-xxx（项目名）属性---配置属性---C++---常规---附加包含目录”
  选择到指向include的文件夹
  保存
  ```

  ![](https://raw.githubusercontent.com/MJX1010/PicGoRepo/main/img/20210731163202.jpg)

  ``` tex
  添加libs文件目录和lib文件名
  
  方法1
  打开菜单栏“项目-xxx（项目名）属性---配置属性---链接器---常规---附加库目录” 
  加入链接库路径
  
  打开菜单栏“项目-xxx（项目名）属性---配置属性---链接器---输入---附加依赖项” 
  添加链接库名字 xxx.lib
  
  
  方法2
  VS导入一个库文件的方法
  #pragma comment(lib, “xxx.lib”)
  #pragma comment(lib, “绝对路径/xxx.lib”)
  ===>
  #pragma仅仅影响编译时的link，运行时只需要保证exe目录下（或者系统目录下或者PATH变量中都有lib对应的dll）exe即能运行
  ```






---



### 辅助

* 添加头部注释

  ``` tex
  \Common7\IDE\ItemTemplates\CSharp\目录（WinForm/Web/...）
  
  $time$ 日期
  $year$ 年份
  $clrversion$ CLR版本
  $GUID$ 用于替换项目文件中的项目 GUID 的 GUID。最多可以指定 10 个唯一的 GUID（例如，guid1)）。
  $itemname$ 用户在对话框中提供的名称。
  $machinename$ 当前的计算机名称（例如，Computer01）。
  $projectname$ 用户在对话框中提供的名称。
  $registeredorganization$ HKLM\Software\Microsoft\Windows NT\CurrentVersion\RegisteredOrganization 中的注册表项值。
  $rootnamespace$ 当前项目的根命名空间。此参数用于替换正向项目中添加的项中的命名空间。
  $safeitemname$ 用户在“添加新项”对话框中提供的名称，名称中移除了所有不安全的字符和空格。
  $safeprojectname$ 用户在“新建项目”对话框中提供的名称，名称中移除了所有不安全的字符和空格。
  $time$ 以 DD/MM/YYYY 00:00:00 格式表示的当前时间。
  $userdomain$ 当前的用户域。
  $username$ 当前的用户名。
  ```

  