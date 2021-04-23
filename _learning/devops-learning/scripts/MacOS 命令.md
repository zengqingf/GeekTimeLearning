# MacOS 命令

* 查看进程占用

  ``` shell
  # 该命令会显示占用8080端口的进程，信息中包括进程 pid ,可以通过pid关掉该进程
  lsof -i tcp:8080 
  ```

* 杀进程

  ``` shell
  kill pid
  ```

  

