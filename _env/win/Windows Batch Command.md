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

  