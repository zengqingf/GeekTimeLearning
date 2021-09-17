# shell

### 教程参考

* ref

  [每天一个linux命令](https://www.cnblogs.com/peida/tag/%E6%AF%8F%E6%97%A5%E4%B8%80linux%E5%91%BD%E4%BB%A4/)

  [Shell 实例教程](https://geek-docs.com/shell/shell-examples/the-shell-display-output-in-the-terminal.html)



---

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






---



* find

  ``` shell
  #通过权限查找
  find -perm 777
  #通过类型查找
  find ./ -type f
  find ./ -type d
  #通过文件创建者查找
  find -user root
  #通过文件名
  find ./ -name "*.txt"
  #通过时间
  find ./ -mtime -1  #一天以内
  #通过大小
  find ./ size -10c  #小于10字节的文件
  ```

  ``` shell
  #递归查找
  find . -name "*.txt"
  #不递归查找
  find . -name "*.txt" -maxdepth 1   #查找深度为1
  ```



---



* [] vs. [[]]

  ``` shell
  #ref: https://www.zsythink.net/archives/2252
  #判断变量为空
  #“-z选项”可以判断指定的字符串是否为空，为空则返回真，非空则返回假，-z可以理解为zero
  #“-n选项”可以判断指定的字符串是否为空，非空则返回真，为空则返回假，-n可以理解为nozero
  a=abc
  [ $a ]		#output: 0
  [[ $a ]]	#output: 0
  [ $b ]		#output: 1
  [[ $b ]]	#output: 1
  [ !$c ]		#output: 0
  ![[ $c ]]	#output: 0
  
  test -n $b   #output: 0  错误！
  test -n "$b" #output: 1  需要加引号
  # [] 等价于 test
  # 而 [[]] 不需要担心是否加引号
  [[ -n $b ]]	 #output: 1	为空
  [[ -z $c ]]  #output: 0 为空
  
  [ "$a" == "$b" ]   ##output: 1   []中判断需要加引号 [[]]中不需要
  [ "$a" != "$b" ]   ##output: 0 
  
  #当使用”-n”或者”-z”这种方式判断变量是否为空时，”[ ]”与”[[  ]]”是有区别的
  #使用”[ ]”时需要在变量的外侧加上双引号，与test命令的用法完全相同，使用”[[  ]]”时则不用。
  
  
  #组合判断条件
  #使用”-a”或者”-o”对多个条件进行连接，然后进行”与运算”或者”或运算”，也可以使用”&&”或者”||”对多个条件进行连接
  [[ 3 -gt 1 && 5 -lt 8 ]] 	#output: 0
  [[ 5 -gt 2 || 9 -lt 3 ]]	#output: 0
  [[ 3 -gt 1]] && [[ 5 -lt 8 ]] #output: 0
  
  [[ 3 -gt 1 -a 5 -lt 0 ]]    #[[]]不支持 -a -o内或外连接
  
  [ 3 -gt 1 -a 5 -lt 8 ] 		#output: 0
  [ 3 -gt 1 ] -a [ 5 -lt 8 ]  #[]不支持 -a -o外连接
  [ 3 -gt 1 && 5 -lt 8 ]		#[]不支持&& || 内连接
  [ 3 -gt 1 ] || [ 5 -lt 8 ]  #output: 0
  
  #在使用”[[  ]]”时，不能使用”-a”或者”-o”对多个条件进行连接。
  #在使用”[  ]”时，如果使用”-a”或者”-o”对多个条件进行连接，”-a”或者”-o”必须被包含在”[ ]”之内。
  #在使用”[  ]”时，如果使用”&&”或者”||”对多个条件进行连接，”&&”或者”||”必须在”[ ]”之外。
  
  
  #部分运算符
  #判断变量值是否满足某个正则表达式
  tel=17858584210
  [[ $tel =~ [0-9]{11} ]]    #output: 0
  [ $tel =~ [0-9]{11} ]	   #=~不能用于[]
  
  [ "a" \< "b" ]
  #当使用”>”或者”<“判断字符串的ASCII值大小时，如果结合”[ ]”使用，则必须对”>”或者”<“进行转义
  
  #在shell中，”-gt”或者”-lt”只能用于比较两个数字的大小，当我们想要比较两个字符的ASCII值时，则必须使用”>”或者”<“，而且需要注意，当使用”双中括号”进行判断时，”>”或者”<“不用转义即可正常使用，当使用”单中括号”进行判断时，”>”或者”<“需要转义后才能正常使用。
  
  #比较日期
  #直接对比相同字符串格式的日期，日期较晚的字符串转换成ASCII以后，ASCII值应该更大
  [[ 20210917 > 20210915 ]]	 		#output: 0
  [[ "2021-09-30" < "2021-10-01" ]]   #output: 0
  ```

  