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

  