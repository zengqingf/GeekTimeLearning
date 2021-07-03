# 工具

## Windows

* 包管理

  Chocolatey

  Scoop



* 通用

  wps office 完整卸载

  ``` text
  基于WPS Office （ W.P.S.10314.12012.2019 ）
  删除C:\ProgramData\kingsoft，需要赋予ProgramData完全控制 Everyone权限
  参考：https://blog.csdn.net/lanluyug/article/details/76559748
  
  删除注册表：计算机\HKEY_CURRENT_USER\SOFTWARE\Kingsoft
  		  计算机\HKEY_LOCAL_MACHINE\SOFTWARE\Kingsoft
  
  删除安装目录
  有些文件删不掉 如 qingsnse64.dll
  参考:https://blog.csdn.net/u011643463/article/details/86515271
  
  rm "文件地址"
  ```

  

* 配置Win DevOps开发环境

  ``` text
  java / android
  
  1.jdk
  2.sdk (adb)
  3.apktool
  4.svn 
  5.git  (github desktop)
  6.finalshell
  7.python2 / 3  (virtualenv)
  8.vmware 
  9.vs code
  10.typora
  11.picgo
  12.drawio
  13.android studio
  14.lua
  15.vs
  16.notepad++
  17.beyondcompare
  18.filezilla client
  ```




* win hosts配置

  * typora + picgo + github 上传图片无法显示问题

    ``` tex
    可能和hosts解析github地址有关
    C:\Windows\System32\drivers\etc\hosts
    
    # GitHub Start 
    185.199.109.133    raw.githubusercontent.com
    # GitHub End
    ```

    