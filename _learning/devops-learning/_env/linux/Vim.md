# Vim

* link

  

* 设置文件

  ``` sh
  # 编码格式
  :set ff 
  # fileformat=dos
  :set ff=unix
  # fileformat=unix
  
  # 显示行号
  :set nu
  :set number
  
  # 删除重复行
  :sort u
  ```

  

* 删除空行

  ``` sh
  #删除空行
  :g/^$/d
  #删除空行以及只有空格的行
  :g/^\s*$/d
  #删除以 # 开头或 空格# 或 tab#开头的行
  :g/^\s*#/d
  #对于 php.ini 配置文件，注释为 ; 开头
  :g/^\s*;/d
  #使用正则表达式删除行
  #如果当前行包含 bbs ，则删除当前行
  :/bbs/d
  #删除从第二行到包含 bbs 的区间行
  :2,/bbs/d
  #删除从包含 bbs 的行到最后一行区间的行
  :/bbs/,$d
  #删除所有包含 bbs 的行
  :g/bbs/d
  #删除匹配 bbs 且前面只有一个字符的行
  :g/.bbs/d
  #删除匹配 bbs 且以它开头的行
  :g/^bbs/d
  #删除匹配 bbs 且以它结尾的行
  :g/bbs$/d
  .ini 的注释是以 ; 开始的，如果注释不在行开头，那么删除 ; 及以后的字符
  :%s/\;.\+//g
  #删除 # 之后所有字符
  %s/\#.*//g
  ```