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

  