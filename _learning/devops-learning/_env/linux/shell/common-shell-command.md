# 常用Shell命令

## macOS环境

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

4. 遍历文件内容

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
   
   ```

5. 字符串截取

   ``` sh
   #按指定字符串截取
   ${varible##*string}   #从左向右截取最后一个string后的字符串
   ${varible#*string}    #从左向右截取第一个string后的字符串
   ${varible%%string*}   #从右向左截取最后一个string后的字符串
   ${varible%string*}    #从右向左截取第一个string后的字符串
   
   
   ```

   

6. 换行符替换为空格

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



7. 字符串替换

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

   
