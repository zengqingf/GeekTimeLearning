# 常用Shell命令



### 通用



1. 获取mac地址

   ``` shell
   ifconfig en0 ether | grep ether  | awk '{print $2}'
   ```

2. 获取ip

   ``` shell
   ifconfig en0 inet | grep inet | awk '{print $2}'
   ```

3. 树形结构展示

   ``` shell
   tree --help
   ```

4. 查看文件（夹）大小

   ``` shell
   du -h --max-depth=1   #层级1
   du -d 1 -h			  #human可读	-d 1 层级1
   
   du -k xxx  			#以1024字节为单位展示
   du -m xxx			#以MB为单位展示
   du -g xxx			#以GB为单位展示
   
   du -ah xxx			#包括目录和文件 -a
   du -0h xxx			#-0 每列一个目录信息不换行
   du -h --exclude='*xyz*' #列出当前目录中的目录名不包括xyz字符串的目录的大小
   du -s xxx			#只输出当前目录大小信息
   ```

5. 删除指定日期前的文件

   ``` shell
   #删除系统中就的备份文件 #在一个目录中保留最近30天的文件，30天前的文件自动删除
   find /tmp -mtime +30 -type f -name *.sh[ab] -exec rm -f {} \;
   
   find . -mtime +30 -type f | xargs rm -rf
   
   #先ls -ltr 查看时间，没有太久的所以就用 -cmin n查找系统中最后N分钟被改变文件状态的文件
   find /home/oracle/test6 -cmin +20 -type f -name *.xml -exec rm -f { } \;
   find . -type f -cmin +10 -exec rm -rf *.xml {} \;
   find . -type f -name "debug*"  -atime +3 -exec rm -f {} \;
   ```

6. cp时排除一个或多个目录

   ``` shell
   #/home目录里面有data目录，data目录里面有a、b、c、d、e五个目录，现在要把data目录里面除过e目录之外的所有目录拷贝到/bak目录中
   
   cp -R `find /home/data -type d -path /home/data/e -prune -o -print | sed 1d ` /bak
   #-prune：使得find命令不进入到当前路径 (跳过)
   #-o：类似于逻辑或运算
   
   rsync -av --exclude data/a --exclude data/b --exclude data/c data /bak
   #--exclude后面的路径不能为绝对路径，必须为相对路径
   ```

7. EOF运用：多行文本输出或者追加到文件					

   ``` shell
   <<EOF        #开始  EOF可以换成任意字母
   ....
   EOF          #结束
   
   #应用场景：
   #1.考虑下面的需求，在主shell执行命令，进入其他的命令，后面的输入，想作为命令的输入，而不是主shell的输入
   #2.使用<<EOF，告诉主shell，后续的输入，是其他命令或者子shell的输入，直到遇到EOF为止，再回到主shell
   
   
   cat << EOF
   #在出现输入提示符">"，输入以下内容：
   > xxx
   > EOF
   #结束时输出 xxx
   
   #多行追加到文件
   cat << EOF > test.txt  #或 cat > test.txt << EOF
   > xxx
   > yyy
   > EOF
   ```

   ``` shell
   #自动新建分区并挂载
   #https://blog.csdn.net/zongshi1992/article/details/71693045
   #cat auto_add_disk.sh
   #!/bin/bash
   fdisk  /dev/sdb  <<EOF
   n
   p
   1
    
    
   wq
   EOF
    
   /sbin/mkfs .ext4  /dev/sdb1  &&   /bin/mkdir  -p  /data  &&  /bin/mount  /dev/sdb1  /data
   echo  'LABEL=data_disk /data ext4 defaults 0 2'  >>  /etc/fstab
   ```

8. 判断上一条命令是否执行成功

   ``` shell
   if [ $? -eq 0 ];then
   #成功TODO
   else
   #失败TODO
   fi
   ```

9. 创建一个选择菜单

   ``` shell
   #!/bin/bash
   # Bash Menu Script Example
   
   PS3='Please enter your choice: '
   options=("Option 1" "Option 2" "Option 3" "Quit")
   select opt in "${options[@]}"
   do
       case $opt in
           "Option 1")
               echo "you chose choice 1"
               ;;
           "Option 2")
               echo "you chose choice 2"
               ;;
           "Option 3")
               echo "you chose choice $REPLY which is $opt"
               ;;
           "Quit")
               break
               ;;
           *) echo "invalid option $REPLY";;
       esac
   done
   ```

10. 多行注释

    ```shell
    #方法1     HERE DOCUMENT 
    << "COMMENT"
    ...
    COMMENT
    
    #方法2   : + 空格 + 单引号   （临时使用）
    : '
    COMMENT1
    COMMENT2
    '
    
    #方法3		: + << 'COMMENT'
    : << 'COMMENT'
    ...
    COMMENT
    ```

    

11. 安装自动确认

    ``` shell
    #linux
    sudo apt-get -y install libmp3lame-dev  #添加y
    yes|sh install.sh
    yes|sudo sudo apt-get install libmp3lame-dev
    ```

    

12. 定时任务

    ``` tex
    ref:https://honglu.me/2014/09/20/OSX%E7%B3%BB%E7%BB%9F%E6%B7%BB%E5%8A%A0%E5%AE%9A%E6%97%B6%E4%BB%BB%E5%8A%A1/
    
    crontab [-u username] [-l|-e|-r]
    相关参数：
    -u ：只有 root 才能进行这个任务，也就是帮其他使用者建立/移除 crontab 工作排程；
    -e ：编辑 crontab 的工作內容
    -l ：查看 crontab 的工作內容
    -r ：移除所有的 crontab 的工作內容，若仅仅移除一项，请用 -e 去编辑。
    crontab file [-u user]：用指定的文件替代目前的crontab。
    
    下面几个数字位
    * * * * *
    代表意义	分钟	小时	日期	月份	周
    数字范围	0-59	0-23	1-31	1-12	0-7
    
    
    如果使用crontab -e编辑无法保存，说明你还没有相关文件，你可以新建一个txt文件，文件内协商你要执行的任务。然后通过sudo crontab file这个命令来新建相关文件，然后你就可以通过crontab -e来修改定时任务了
    通过上面的命令介绍可见crontab的最小时间间隔是一分钟
    ```

    ``` shell
    sudo crontab -e
    # 此时会进入 vi 编辑器！注意到，每项工作都是一行。
    # 基本格式：* * * * * command 
    0 12 * * * mail dmtsai -s "at 12:00" < /home/dmtsai/.bashrc
    #分 时 日 月 周 |<==============指令串========================>|
    sudo crontab -l
    # 查看已经添加的定时任务
    
    # 12：00执行这个Python脚本
    sudo crontab -e
    0 12 * * * /usr/bin/python /Users/aigo/Documents/demo.py
    ```

    ``` shell
    #macos
    #launchctl 定时任务  
    #通过plist配置的方式来实现定时任务，最小时间间隔为一秒
    
    #plist脚本存放路径为/Library/LaunchDaemons或/Library/LaunchAgents
    #后一个路径的脚本当用户登陆系统后才会被执行，前一个只要系统启动了，哪怕用户不登陆系统也会被执行
    
    #设置脚本时间
    # StartInterval：指定脚本每间隔多久（秒）执行一次
    # StartCalendarInterval：指定脚本在多少分钟、小时、天、星期几、月时间上执行
    ```

    ``` xml
    <!--~/Library/LaunchAgents  新建 com.aigo.launchctl.plist-->
    
    <!--每一天13点4分的时候执行一次-->
    <?xml version="1.0" encoding="UTF-8"?>
    <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
    <plist version="1.0">
    <dict>
      <key>Label</key>
      <string>com.aigo.launchctl.plist</string>
      <key>ProgramArguments</key>
      <array>
        <string>/Users/aigo/Documents/AutoMakeLog.sh</string>
      </array>
      <key>StartCalendarInterval</key>
      <dict>
            <key>Minute</key>
            <integer>4</integer>
            <key>Hour</key>
            <integer>13</integer>
      </dict>
      <key>StandardOutPath</key>
    <string>/Users/aigo/Documents/AutoMakeLog.log</string>
    <key>StandardErrorPath</key>
    <string>/Users/aigo/Documents/AutoMakeLog.err</string>
    </dict>
    </plist>
    
    <!--每隔30秒执行一次-->
    <?xml version="1.0" encoding="UTF-8"?>
    <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
    <plist version="1.0">
    <dict>
        <key>Label</key>
        <string>com.aigo.launchctl.plist</string>
        <key>ProgramArguments</key>
        <array>
            <string>/Users/aigo/Documents/AutoMakeLog.sh</string>
        </array>
        <key>KeepAlive</key>
        <false/>
        <key>RunAtLoad</key>
        <true/>
        <key>StartInterval</key>
        <integer>30</integer>
    </dict>
    </plist>
    
    <!--label为任务名，常设为plist文件名，不要重复-->
    <!--
    launchctl load   com.aigo.launchctl.plist   #加载
    launchctl unload com.aigo.launchctl.plist
    launchctl start  com.aigo.launchctl.plist	#立即执行
    launchctl stop   com.aigo.launchctl.plist
    launchctl list
    -->
    ```

    

    



---



### 时间

1. 获取前一天日期

   ``` shell
   #linux
   date +%Y%m%d --date='-1 day'	#前一天
   date +%Y%m%d --date='1 day'		#后一天
   
   #mac unix
   date -v -1d +%Y-%m-%d		#前一天
   date -v +1d +%Y-%m-%d		#后一天
   
   date  +"%Y%m%d" -d  "+n days"         #今天的后n天日期  
   date  +"%Y%m%d" -d  "-n days"         #今天的前n天日期
   ```

2. 时间戳转换

   ``` shell
   #转本地时间
   date -r 1354291200
   
   #转时间戳
   date -j -f "%Y-%m-%d %H:%M:%S" "2012-12-01 00:00:00" "+%s"
   ```

   





---



### 函数



1. 获取函数返回值

   ``` shell
   function f()
   {
   	echo "xxx"
   	return 0    #返回值可有可无
   }
   
   a=`f`  #调用函数 并将函数的标准输出传递给a   函数调用不需要加括号
   echo $?  #获取上一句返回状态，可以获取函数return值
   
   #示例
   #使用num = num+1 无效 
   num=10
   add(){
        echo "test"
        ((num++))
   }
   #调用
   add
   
   #函数传参
   #在函数体内使用 $1 $2...
   #调用时 f arg1 arg2
   ```

   ``` shell
   # 变量自增
   i=`expr $i + 1`;
   let i+=1;
   ((i++));
   i=$[$i+1];
   i=$(( $i + 1 ))
   
   #@注意：
   i=i+1 #不支持
   
   #test
   i=0;
   while [ $i -lt 4 ];
   do
      echo $i;
      i=`expr $i + 1`;
      # let i+=1;
      # ((i++));
      # i=$[$i+1];
      # i=$(( $i + 1 ))
   done
   
   #固定次数循环，使用seq替换，输出固定间隔的连续数字
   for j in $(seq 1 5)
   do
     echo $j
   done
   ```

   





2. 变量在shell脚本间传递

   ``` shell
   #test1.sh
   #!/bin/bash
   aaa=yuanfaxiang
   echo "test1:$aaa"
   
   
   #test2.sh
   #!/bin/bash
   source /root/test1.sh
   echo "test2:$aaa"
   
   #结果显示test2.sh继承了test1.sh中定义的变量aaa。
   #原因分析：在第一次执行test2.sh时，test1.sh被作为了test2.sh的子shell来执行，其中定义的变量只
   #在test1.sh中起效，不能逆向传递到test2.sh中；而在第二次执行中，采用source来执行test1.sh,意思
   #是直接把test1.sh在当前的test2.sh中执行，没有作为子shell去执行，test1.sh中定义的变量，就影响
   #到了test2.sh。
   
   
   #test3.sh
   #!/bin/bash
   echo "test3:$aaa"
   
   
   #-------------------------------------#
   
   
   #test2_2.sh
   #!/bin/bash
   source /root/test1.sh
   echo "test2:$aaa"
   /root/test3.sh
   
   #结果显示test3.sh没有继承test1.sh中申明的变量，因为source /root/test1.sh只是让test1.sh
   #中的变量在test2.sh中生效，aaa毕竟还是个普通局部变量，并不能被test3.sh这个子shell所继承，
   #所以我们可以想到环境变量，把aaa变成test2.sh这个脚本的环境变量，让test2.sh的子进程也能继承。
   
   
   #-------------------------------------#
   
   
   #test1_2.sh
   #!/bin/bash
   export aaa=yuanfaxiang
   echo "test1:$aaa"
   
   #test2.sh
   #!/bin/bash
   source /root/test1_2.sh
   echo "test2:$aaa"
   /root/test3.sh  
   
   #在test1.sh中声明了环境变量也就是全局变量，在test2.sh中用source执行test1.sh，将变量带到了
   #test2.sh中，并使之成为test2.sh执行过程中的环境变量，可以被test2.sh的子进程继承，起到了顺向
   #传递效果。
   ```

   









---



### 文件相关



1. 遍历文件内容

   ``` sh
   #!/bin/bash
   while read line
   do
       echo $line
   done < file(待读取的文件)
   
   
   #!/bin/bash
   cat file(待读取的文件) | while read line
   do
       echo $line
   done
   
   
   #!/bin/bash
   for line in `cat file(待读取的文件)`
   do
       echo $line
   done
   
   
   [【Linux】Shell脚本：while read line无法读取最后一行？？？](https://blog.csdn.net/qq_34018840/article/details/106717598)
   #注意，如果while read line无法读取最后一行
   #根本原因是 文件格式不是unix
   
   
   #使用awk
   cat data.dat | awk '{print $0}'   #输出data.dat里的每行数据
   cat data.dat | awk 'for(i=2;i<NF;i++) {printf $i} printf "\n"}' #输出每行中从第2列之后的数据
   
   
   ```

   ``` shell
   #去除换行符 for win doc
   #读取win下的文件会出现文本结尾有^M \r \n
   cat keywords.txt |while read line
   do
   line=${line//[$'trn']}
   cdomain=$line.$domain
   done
   ```

   

2. 读写json

   [jq](https://stedolan.github.io/jq/manual/)

   ``` shell
   # jq '.key'
   cat xxx.json | jq '.sign'
   cat yyy.json | jq '.live_node_config.node1.ip'
   curl -s "https://xxx" | jq '.mjx1010_config.node1.ip'   #-s表示静默模式 例如不输出文件下载进度等
   curl -s "https://xxx" | jq 'has("sign")'				#判断key是否存在
   
   #json 数组的键命名必须为下划线"_"，不能为"-"，否则解析不了
     "mjx1010-config": {   #改为  "mjx1010_config"
       "ip": "192.168.2.100",
       "user": "www",
       "pwd": "123456"
     }
     
   #格式化json
   cat xxx.json | jq .
   ```



3. 文件存在 和 可执行权限判断

   ``` shell
   if [ ! -x "$filePath"]; then
   #没有可执行权限
   fi
    
   if [ ! -d "$dirPath"]; then
   #目录不存在
   fi
   
   if [ ! -d "$filePath"]; then
   #文件不存在
   fi
   ```

   

4. 文件前后缀（获取文件名和后缀）

   ``` shell
   #获取去掉后缀名的前缀
   ls -al | cut -d “.” -f1
   #获取后缀名
   ls -al | cut -d “.” -f2
   
   ${basename $var}	   #获取文件名（包括后置）
   ${basename $var .xxx}  #文件名去除后缀.xxx
   ```

   

5. 文件内容排序

   ``` shell
   sort -t ":" -k 3,3 /etc/passwd       #没把uid当作字符串对待，而当数值型对待，需要加-n参数
   ```

   

6. 遍历目录

   ``` shell
   path=$1
   files=$(ls $path)
   for filename in $files
   do
      echo $filename >> filename.txt
   done
   ```

   

7. 提取目录名

   ``` shell
   dirname $var
   #提取父目录（即使var为目录）
   ```

   



---



### 字符串相关

* 读取字符串值

  ``` shell
  ${var}	变量var的值, 与$var相同
  ${var-DEFAULT}	如果var没有被声明, 那么就以$DEFAULT作为其值 *
  ${var:-DEFAULT}	如果var没有被声明, 或者其值为空, 那么就以$DEFAULT作为其值 *
  ${var=DEFAULT}	如果var没有被声明, 那么就以$DEFAULT作为其值 *
  ${var:=DEFAULT}	如果var没有被声明, 或者其值为空, 那么就以$DEFAULT作为其值 *
  ${var+OTHER}	如果var声明了, 那么其值就是$OTHER, 否则就为null字符串
  ${var:+OTHER}	如果var被设置了, 那么其值就是$OTHER, 否则就为null字符串
  ${var?ERR_MSG}	如果var没被声明, 那么就打印$ERR_MSG *
  ${var:?ERR_MSG}	如果var没被设置, 那么就打印$ERR_MSG *
  ${!varprefix*}	匹配之前所有以varprefix开头进行声明的变量
  ${!varprefix@}	匹配之前所有以varprefix开头进行声明的变量
  ```

* 字符串操作

  ``` shell
  ${#string}	$string的长度
  ${string:position}	在$string中, 从位置$position开始提取子串
  ${string:position:length}	在$string中, 从位置$position开始提取长度为$length的子串
   	 
  ${string#substring}	从变量$string的开头, 删除最短匹配$substring的子串
  ${string##substring}	从变量$string的开头, 删除最长匹配$substring的子串
  ${string%substring}	从变量$string的结尾, 删除最短匹配$substring的子串
  ${string%%substring}	从变量$string的结尾, 删除最长匹配$substring的子串
   	 
  ${string/substring/replacement}		使用$replacement, 来代替第一个匹配的$substring
  ${string//substring/replacement}	使用$replacement, 代替所有匹配的$substring
  ${string/#substring/replacement}	如果$string的前缀匹配$substring, 那么就用$replacement来代替匹配到的$substring
  ${string/%substring/replacement}	如果$string的后缀匹配$substring, 那么就用$replacement来代替匹配到的$substring
  ```

* 操作符

  ``` shell
  string=abc12342341
  
  #获取字符串长度
  expr length $string
  expr "$string" : ".*"  #:两边要有空格，类似match用法
  
  #获取字符串所在位置  字符串对应的下标是从1开始的
  expr index $string '123'	#output: 4
  expr index $string "a"		#output: 1
  expr index $string ""		#output: 0
  
  #字符串开头到子串的最大长度
  expr match $string 'abc.*3'   #output: 9    
  
  #字符串截取
  expr substr $string 3 3		#output: 123 从第三位开始截取后面3位
  
  #字符串匹配
  expr match $string '\([a-c]*[0-9]*\)'		#output:	abc12342341
  expr $string : '\([a-c]*[0-9]\)'			#output:	abc1
  expr $string : '.*\([0-9][0-9][0-9]\)'		#output:	341
  ```

* 正则表达式

  ![](https://raw.githubusercontent.com/MJX1010/PicGoRepo/main/img/202109162213644.jpg)

  



1. 字符串截取

   ``` sh
   #按指定字符串截取
   ${varible##*string}   #从左向右截取最后一个string后的字符串
   ${varible#*string}    #从左向右截取第一个string后的字符串
   ${varible%%string*}   #从右向左截取最后一个string后的字符串
   ${varible%string*}    #从右向左截取第一个string后的字符串
   
   #去除空行、空格
   cat 文件名 |tr -s ‘\n'
   cat 文件名 |sed ‘/^$/d'
   cat 文件名 |awk ‘{if($0!=”")print}'
   cat 文件名 |awk ‘{if(length !=0) print $0}'
   grep -v “^$” 文件名
   
   #删除行首空格
   sed 's/^[ \t]*//g'	 #^以...开头
   #删除行末空格
   sed 's/[ \t]*$//g'   #$以...结尾
   #删除所有空格
   sed 's/[[:space:]]//g'
   
   #两个字符间截取  
   str="abcdefg"  #截取c和f之间的字符串，得到de
   #用split函数，以c和f为分隔符，将字符串分割，取分割后的第二个字段
   echo "$str" | awk '{split($0,a,"[cf]");print a[2]}'  
   #可以分别计算出c和f在字符串中的位置，然后根据截取字符串的起始位置（c的位置+1）和截取长度（f的位置-c的位置-1），用substr函数来得到截取后的字符串
   echo "$str" | awk '{a=index($0,"c");b=index($0,"f");print substr($0,a+1,b-a-1)}'
   
   echo "$str" | sed -r 's/.*c(.*)f.*/\1/'
   ```

   ``` shell
   #here string可以看成是here document的一种定制形式. 除了COMMAND <<<$WORD, 就什么都没有了, de>$WORDde>将被扩展并且被送入de>COMMANDde>的stdin中.
   
   #ref:https://www.cnblogs.com/liuweijian/archive/2009/12/27/1633661.html
   #示例1
   String="This is a string of words."
   read -r -a Words <<< "$String"
   #  "read"命令的-a选项
   #+ 将会把结果值按顺序的分配给数组中的每一项. 
   echo "First word in String is:    ${Words[0]}"   # This
   echo "Second word in String is:   ${Words[1]}"   # is
   echo "Third word in String is:    ${Words[2]}"   # a
   echo "Fourth word in String is:   ${Words[3]}"   # string
   echo "Fifth word in String is:    ${Words[4]}"   # of
   echo "Sixth word in String is:    ${Words[5]}"   # words.
   echo "Seventh word in String is:  ${Words[6]}"   # (null)
                                                    # $String的结尾. 
                                                    
   #示例2 在文件的开头添加文本
   E_NOSUCHFILE=65
   10 read -p "File: " file   #  'read'命令的-p参数用来显示提示符. 
   if [ ! -e "$file" ]
   then   # 如果这个文件不存在, 那就进来. 
    echo "File $file not found."
    exit $E_NOSUCHFILE
   fi
   read -p "Title: " title
   cat - $file <<<$title > $file.new
   echo "Modified file is $file.new"
   exit 0
   
   # 下边是'man bash'中的一段: 
   # Here String
   #  here document的一种变形，形式如下: 
   # 
   #   <<<word
   # 
   #  word被扩展并且被提供到command的标准输入中. 
   ```

   ``` shell
   #ref: https://blog.51cto.com/wt7315/1860063
   
   cat /etc/passwd | grep /bin/bash | grep -v root | cut -d":" -f 1
   
   #使用FS=””,指定分割符，第一行已经读完，用冒号分割已经来不及了，默认的用空格分割，后面的用冒号分割
   awk '{FS=":"}{print $1"\t" $3}' /etc/passwd
   cat /etc/passwd | awk '{FS=":"} $3>=500 {printf $1"\n"}'
   
   df -h | sed -n "2p"
   sed -n "1p" /etc/passwd 	#查看etc/passwd中的第一行，如果不加-n参数，显示这条操作外，还会显示文件的全部内容，加-n 只会显示处理的行
   sed "2,36d" /etc/passwd		#删除第二行到第四行的数据，但不修改文件本身，只有加了-i参数才会修改文件本身
   ```

   

   ``` shell
   #正则字符串提取
   #-P参数表明要应用正则表达式
   #-o表示只输出匹配的字符串
   echo office365 | grep -P '\d+' -o
   find . -name "*.txt" | xargs grep -P 'regex' -o   #xargs会将find结果作为grep的输入，防止find结果过多无法处理
   
   echo here365test | sed 's/.*ere\([0-9]*\).*/\1/g'  #output: 365
   ```

   

2. 换行符替换为空格

   ``` shell
   '''
   China
   America
   France
   German
   '''
   
   #sed
   cat country.txt | sed ':label;N;s/\n/ /;b label'
   
   #tr
   cat country.txt | tr "\n" " "
   
   #output: China America France German
   #两个命令输出一致，但是sed命令的输出结尾有换行符，而tr命令的输出结尾没有换行符
   ```



3. 字符串替换

   ``` shell
   ${parameter/pattern/string}
   
   a=/data/wxnacy/data/log/log.txt
   echo ${a/data/User}           		# 将第一个 data 替换为 User
   #output: /User/wxnacy/data/log/log.txt
   
   echo ${a//data/User}           		# 将全部 data 替换为 User
   #output: /User/wxnacy/User/log/log.txt
   
   echo ${a/#\/data/\/User}            # 匹配开头 /data 替换为 /User（/ 需要转义）
   #output: /User/wxnacy/data/log/log.txt
   
   echo ${a/%log.txt/User}           	# 匹配结尾 log.txt 替换为 User
   #output: /data/wxnacy/data/log/User
   
   
   $ echo $a | sed -e "s/data/User/g"
   #output: /User/wxnacy/User/log/log.txt
   ```

   

4. 字符串判空

   ``` shell
   #!/bin/sh
   STRING=
   if [ -z "$STRING" ]; then       #@注意：""不能省
       echo "STRING is empty"
   fi
   
   if [ -n "$STRING" ]; then
       echo "STRING is not empty"
   fi
   ```



5. 字符串是否为数字

   ``` shell
   a=1234;echo "$a"|[ -n "`sed -n '/^[0-9][0-9]*$/p'`" ] && echo string a is numbers
   
   if grep '^[[:digit:]]*$' <<< "$1";then  
       echo "$1 is number."  
   else  
       echo 'no.'  
   fi  
   
   if [ "$1" -gt 0 ] 2>/dev/null ;then  
       echo "$1 is number."  
   else  
       echo 'no.'  
   fi  
   
   case "$1" in  
       [1-9][0-9]*)   
           echo "$1 is number."  
           ;;  
       *)    
           ;;  
   esac  
   
   
   echo $1| awk '{print($0~/^[-]?([0-9])+[.]?([0-9])+$/)?"number":"string"}'  
   
   
   if [ -n "$(echo $1| sed -n "/^[0-9]\+$/p")" ];then  
       echo "$1 is number."  
   else  
       echo 'no.'  
   fi  
   
   
   expr $1 "+" 10 &> /dev/null  
   if [ $? -eq 0 ];then  
       echo "$1 is number"  
   else  
       echo "$1 not number"  
   fi  
   ```

   

6. 字符串比较

   ``` shell
   [[ "a.txt" == a* ]]        # 逻辑真 (pattern matching)  
   [[ "a.txt" =~ .*\.txt ]]   # 逻辑真 (regex matching)  
   [[ "abc" == "abc" ]]       # 逻辑真 (string comparision)   
   [[ "11" < "2" ]]           # 逻辑真 (string comparision), 按ascii值比较 
   ```



7. 字符串包含

   ``` shell
   #grep
   result=$(echo $strA | grep "${strB}")
   if [[ "$result" != "" ]] ...
   
   #字符串运算符
   strA="helloworld"
   strB="low"
   if [[ $strA =~ $strB ]]
   
   #通配符
   A="helloworld"
   B="low"
   if [[ $A == *$B* ]]
   
   #case in
   thisString="1 2 3 4 5" # 源字符串
   searchString="1 2" # 搜索字符串
   case $thisString in 
       *"$searchString"*) echo Enemy Spot ;;
       *) echo nope ;;
   esac
   
   #替换
   STRING_A=$1
   STRING_B=$2
   if [[ ${STRING_A/${STRING_B}//} == $STRING_A ]]
   ```

   



---
