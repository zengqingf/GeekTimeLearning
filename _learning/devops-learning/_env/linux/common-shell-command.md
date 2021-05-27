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

   

