# **Python相关**



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



### 内置库



* argparse

  [Argparse 教程 python3.9.7](https://docs.python.org/zh-cn/3/howto/argparse.html)







---



### 第三方库

* bs4

  Beautiful Soup 是一个可以从HTML或XML文件中提取数据的Python库

  [Beautiful Soup 4.4.0 文档 — Beautiful Soup 4.2.0 documentation (crummy.com)](https://www.crummy.com/software/BeautifulSoup/bs4/doc.zh/)

