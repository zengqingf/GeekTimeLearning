# Jenkins

### with python

* jenkins执行python，实时打印输出

  ``` python
  print("", sys.stdout.flush())
  ```

  



---



### with svn

* svn触发jenkins自动构建

  ``` tex
  ref: https://blog.51cto.com/techsnail/2142599
  1.Jenkins主动轮询SVN仓库；
  2.在SVN客户端(如TortoiseSVN)创建客户端hooks来触发构建；
  3.在SVN服务器端，创建仓库hooks来触发构建。
  ```

  





---



### Windows Slave搭建

* 配置从节点

  ![](https://raw.githubusercontent.com/MJX1010/PicGoRepo/main/img/202110221620254.jpg)

  ![](https://raw.githubusercontent.com/MJX1010/PicGoRepo/main/img/202110221623646.jpg)

