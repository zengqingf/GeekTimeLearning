# **Python相关**



* ref

  [Python 入门指南](http://www.pythondoc.com/pythontutorial3/index.html)





---



### **问题**

* ModuleNotFoundError: No module named 'XXX' 

  ``` python
  # -*- encoding: utf-8 -*-
  import sys,os
  thisdir = os.path.abspath(os.path.dirname(__file__))
  workdir = os.path.abspath(os.getcwd())
  sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
  
  import sys
  import os
  curPath = os.path.abspath(os.path.dirname(__file__))
  rootPath = os.path.split(curPath)[0]
  sys.path.append(rootPath)
  #sys.path.append(os.path.split(rootPath)[0])
  
  
  ```

  





---



### API

* 基础

  * 平台判断

    ``` python
    sys = platform.system().lower()  #加lower()保证字符串判断一致性
    if sys == "windows" :
       elif sys == "linux":
        ...
    ```

    

* 文件系统

  * 删除非空文件夹

    ``` python
    import os
    import shutil
    
    os.remove(path)   #删除文件
    os.removedirs(path)   #删除空文件夹
    
    shutil.rmtree(path)    #递归删除文件夹
    ```

    

  * 拷贝文件

    ``` python
    #拷贝过滤
    from shutil import copytree, ignore_patterns
    copytree(source, destination, ignore=ignore_patterns('*.pyc', 'tmp*'))
    ```

  

  * 路径

    * os.path

      ``` python
      os.path.sep				路径分隔符 linux下就用这个了’/’
      os.path.altsep			根目录
      os.path.curdir			当前目录
      os.path.pardir			父目录
      os.path.abspath(path)	绝对路径
      os.path.join()			常用来链接路径
      os.path.split(path)		把path分为目录和文件两个部分，以列表返回
      ```

      

  * 查找

    * find

      * 按时间

        ``` python
        '''
        -mtime 
        -mtime +1，表示1天以外的，即从距当前时间的1天前算起，往更早的时间推移。因此2015-02-28 22:31前的文件属于该结果，2015-02-28 22:31后的文件不属于该结果
        -mtime 1， 距离当前时间第1天的文件，当前时间为2015-03-01 22：31，往前推1天为2015-02-28 22:31，因此以此为时间点，24小时之内的时间为2015-02-28 22:31～2011-03-01 22:31，因此这段时间内的文件会被选中出来
        -mtime -1 表示1天以内的，从距当前时间的1天为2015-02-28 22：31，往右推移
        '''
        find . -mtime -180 -name \*
        
        '''
        atime：访问时间（access time），指的是文件最后被读取的时间，可以使用touch命令更改为当前时间；
        ctime：变更时间（change time），指的是文件本身最后被变更的时间，变更动作可以使chmod、chgrp、mv等等；
        '''
        
        '''
        第一个参数，.，代表当前目录，如果是其他目录，可以输入绝对目录和相对目录位置；
        第二个参数分两部分，前面字母a、c、m分别代表访问、变更、修改，后面time为日期，min为分钟，注意只能以这两个作为单位；
        第三个参数为量，其中不带符号表示符合该数量的，带-表示符合该数量以后的，带+表示符合该数量以前的。
        '''
        find . {-atime/-ctime/-mtime/-amin/-cmin/-mmin} [-/+]num
        ```

  * zip

    * zipfile

      ``` python
      import zipfile
      with zipfile.ZipFile(path_to_zip_file, 'r') as zip_ref:
          zip_ref.extractall(directory_to_extract_to)
      ```

      

* 字符串

  * 格式化

    * 大小写

      ``` python
      #title()方法:返回标题化字符串，即所有的单词以大写开始，其余的为小写
      a = "My name is xiao ming"
      print a.title()
      #output: My Name Is Xiao Ming
      
      #upper()方法：将字符串全部改为大写
      print a.upper()
      #output: MY NAME IS XIAO MING
      
      #lower()方法：将字符串全部改为小写
      print a.lower()
      #output: my name is xiao ming
      ```

  * 删除

    ``` python
    s.strip(rm)       #删除s字符串中开头、结尾处，位于 rm删除序列的字符
    s.lstrip(rm)      #删除s字符串中开头处，位于 rm删除序列的字符
    s.rstrip(rm)      #删除s字符串中结尾处，位于 rm删除序列的字符
    
    a = '123abc'
    a.strip('21')  #output: 3abc
    a.strip('12')  #output: 3abc
    ```

  
  * 正则表达式
  
    



---



### 内置库



* argparse

  [Argparse 教程 python3.9.7](https://docs.python.org/zh-cn/3/howto/argparse.html)







---



### 第三方库

* bs4

  Beautiful Soup 是一个可以从HTML或XML文件中提取数据的Python库

  [Beautiful Soup 4.4.0 文档 — Beautiful Soup 4.2.0 documentation (crummy.com)](https://www.crummy.com/software/BeautifulSoup/bs4/doc.zh/)



* requests

  ``` python
  import requests
  import shutil
  url="https://Hostname/saveReport/file_name.pdf"    #Note: It's https
  r = requests.get(url, auth=('usrname', 'password'), verify=False,stream=True)
  r.raw.decode_content = True
  with open("file_name.pdf", 'wb') as f:
          shutil.copyfileobj(r.raw, f)
  ```

  





---



### 示例

* ftp

  * ftp批量下载

    [github-ftpdown](https://github.com/dog-2/ftpdown)