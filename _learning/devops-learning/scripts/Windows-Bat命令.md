# Windows-Bat命令

* 目录

  ``` sh
  # use REM --> #
  # 当前盘符：%~d0
  # 当前路径：%cd%
  # 当前执行命令行：%0
  # 当前bat文件路径：%~dp0
  # 当前bat文件短路径：%~sdp0
  
  # 调用.bat当前目录下的SQL文件
  @echo off 
  set filepath=%cd%
  echo 创建中间表开始
  sqlplus GXGXH/GXGXH @%filepath%\createTable.sql 
  echo 创建中间表成功
  pause>nul
  
  @echo off 
  echo 创建中间表开始
  sqlplus GXGXH/GXGXH @%cd%\createTable.sql 
  echo 创建中间表成功
  pause>nul
  
  
  # dir
  # 例 批处理更新svn  ---> 可以用 "svn info --show-item wc-root" 获取.svn所在节点
  @echo off
  #首先进入到所有SVN目录所处的顶层路径
  D:&&cd D:\TOP_SVN_DIR
  #开始执行查找 -> 遍历 -> 更新
  for /f "delims=" %%i in ('dir /adh/b/s/on .svn') do echo %%i&&cd %%i&&cd ../&&svn up
  pause
  ```

  

* if语句

  ``` sh
  # 用于for循环
  
  # 例 批处理删除.svn文件夹
  @echo off
  echo 正在清理SVN文件，请稍候...... 
  for /r . %%a in (.) do @if exist "%%a\.svn" rd /s /q "%%a\.svn" 
  echo 清理完毕！！！ 
  pause
  
  ```



* 回显

  ``` sh
  # echo off vs. echo on vs. @echo off vs. @echo on
  
  # @echo off ：表示在批处理文件执行过程中，只显示结果，而不显示执行的命令（包括本条命令）
  # echo off : 表示在批处理文件执行过程中，只显示结果，而不显示执行的命令（本条命令显示）
  # echo on : 表示在批处理文件执行过程中，显示执行的命令和结果
  # @echo on : 表示在批处理文件执行过程中，显示执行的命令和结果（不显示本条命令）
  
  
  
  ```

  

* 注释

  ``` sh
  # :: vs. REM
  # 1. 被 @echo on 打开时 REM注释的内容会被显示出来 ！  需要使用@REM 
  
  # rem是一条命令，在运行的时候相当于把rem本身及其后面的内容置空。既然它是一条命令，就必须处于单独的一行或者有
  # 类似“&”的连接符号连接。
  
  # 批处理遇到以冒号“:”开头的行时（忽略冒号前的空格），会将其后的语句识别为“标记”而不是命令语句，因此类似
  # “:label”这样的东东在批处理中仅仅是一个标记。
  ```

  