# Windows Batch Command

* 目录切换 Cd

  ``` powershell
  # 同一分区
  cd 路径
  cd ..
  cd .
  cd \
  
  # 不同分区
  cd /d 路径
  ## 或者直接输入盘符  
  D:
  
  # 显示完整路径  
  @echo off
  echo %cd%  #有弊端 需要进入对应的目录  如果在其他目录访问 不可行
  echo %~dp0%  #bat所在目录
  pause
  ```

  

* 自启动

  [windows下.bat程序开机自启动的几种方法](https://blog.51cto.com/sxhxt/2312207)
  
  ``` text
  1. windows10 开机自启动目录：
  C:\Users\用户名\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup
  
  编写VBS脚本
  新建脚本script.vbs，脚本内容如下：
  set ws=WScript.CreateObject("WScript.Shell")
  ws.Run "C:\Users\Administrator\Desktop\aa.bat /start",0    #bat的存放路径
  
  2. 修改注册表
  开始 -> 运行 -> regedit打开注册表
  HKEY_LOCAL_MACHINE -> SOFTWARE-> Mirosoft -> Windows -> Run
  在右侧窗口点击右键创建字符串值 命名为Run ，双击填写bat脚本所在的路径即可实现开机运行bat程序
  
  3.直接把bat脚本放到开始 -> 所有程序 -> 启动 
  
  4.开始运行 -> 输入gqedit.msc
  双击计算机配置 -> windwos配置 -> 脚本启动 -> 启动
  点击添加 -> 弹出对应的对话框后 浏览你的bat脚本所在的路径 保存即可实现开机自启动
  
  5.控制面板 -> 小图标 -> 管理 -> 任务计划程序
  ```
  
  