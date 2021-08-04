# shell

### rsync

* ref 

  [rsync 用法教程](http://www.ruanyifeng.com/blog/2020/08/rsync.html)





---



* sshpass

  [sshpass-如何在脚本中执行scp时自动输入密码](https://blog.csdn.net/jiangguilong2000/article/details/12971649)

  ``` shell
  yum -y install sshpass
  #http://sourceforge.net/projects/sshpass/
  
  #install
  ./configure
  make
  make install
  cp sshpass /usr/bin/
  
  sshpass -p $PASSWD scp -o StrictHostKeyChecking=no $CMD
  
  #
  `cd $path && sshpass -p HS~u8ro8e scp -o StrictHostKeyChecking=no $package javadev\@121.199.60.78:/home/javadev/server`;
  
  ```

  

---



* mv

  ``` shell
  # 批量移动 重命名
  #例：移动stf-xxx.sh到 xxx/目录下
  mv stf-*.sh xxx/
  ```

  