# awk使用

* ref

  [AWK 简明教程](https://coolshell.cn/articles/9070.html)

  [awk从放弃到入门](https://www.zsythink.net/archives/tag/awk)

* 

* printf + awk

  [printf使用](./echo.md)

  [printf+awk完美结合 ](https://blog.51cto.com/cfwlxf/1309242)

  ``` shell
  # cat test.txt
  '''
  root		x	0		0		/root			/bin/bash
  bin			x	1		1		/bin			/sbin/nologin
  ...
  '''
  
  #通过逻辑运算符“&&”与“||”，过滤文本内容
  awk '$1=="root" && $3==0' test.txt
  #output: root x 0 0 /root /bin/bash
  #过滤的内容中包含字符串，需要用双引号“”
  #$1...$n：表示第文本内容的第几列，其中$1==root，$2==x，$3==0.....$n==$n。
  #&&：表示逻辑与，即满足条件第一列为root，第三列为0的行才会被显示。
  
  awk '$1=="sync" || $5=="/root"' test.txt
  # 满足$1==“sync”或者$5==“/root”其中的一个过滤条件，文本内容标准输出至终端。
  #||：表示逻辑或，即满足过滤条件任意一个,文本内容标准输出。
  
  
  #通过比较运算符!=，==，>=,<=，>，<过滤文本内容
  awk '$3>500 {printf "%-15s %-10s %-10s\n", $1,$3,$4}' test.txt
  #$3为用户的UID，满足过滤条件‘$3>500’的行，标准输出到终端。
  
  awk '$3 != 502 && $4 == 501 {printf "%-15s %-10s %-10s\n", $1,$3,$4}' test.txt
  
  awk '$4 <= 4' test.txt
  awk '$4 <= 4 && $4 != 0' test.txt
  
  
  #通过终端显示的结果可以看出，满足过滤条件”$1 ~ /root”，$0输出文件整行的内容，而$1输出整行中第一列。
  awk '$1 ~ /root/{print $0}' test.txt
  #output: root x 0 0 /root /bin/bash
  
  awk '$1 ~ /root/{print $1}' test.txt
  #output: root
  
  
  # NR记录当前文件中的行号，其中bin用户的行号为2，满足过滤条件”NR==2”标准输出，ntp用户的UID大于10，
  # 同时满足过滤条件”Nr==20”，cat -n test.txt | grep 20证实了这一结论。
  awk 'NR==2 {print $0}' test.txt
  #output: bin x 1 1 /bin /sbin/nologin
  
  awk '$3 >=10 && NR==20 {print $0}' test.txt
  #output: ntp x 38 38 /etc/ntp /sbin/nologin
  cat -n test.txt | grep 20
  #output: 20 ntp x 38 38 /etc/ntp /sbin/nologin
  
  
  awk '$3 < 5 {print $1,$2,$3,$4,$5}' test.txt
  #output： 紧凑输出 uid < 5的行
  awk '$3 < 5 {print $1,$2,$3,$4,$5}' OFS="\t" test.txt
  #output： 分隔输出 uid < 5的行
  #通过”OFS=”\t””指定了文件中列与列之间的距离，\t表示4个字符的距离，可以看出两条命令执行的结果有差异。
  ```

  

* awk常用内置变量

  | 内置变量 | 作用                                                         |
  | -------- | ------------------------------------------------------------ |
  | $0       | 当前记录，变量存放文件整行的内容。                           |
  | \$1~$n   | 记录文件当前行的第n列，字段间由FS分隔。                      |
  | NF       | 当前记录中的字段个数，就是有多少列。                         |
  | NR       | 已经读出的记录数，就是行号，从1开始，如果有多个文件话，这个值也是不断累加中。 |
  | FS       | 输入字段分隔符，默认是空格或Tab。                            |
  | OFS      | 输出字段分隔符， 默认也是空格。                              |

  [linux awk 内置函数详细介绍（实例）](https://www.cnblogs.com/chengmo/archive/2010/10/08/1845913.html)

  | **函数名**      | **说明**                                                     |
  | --------------- | ------------------------------------------------------------ |
  | atan2( y, x )   | 返回 y/x 的反正切。                                          |
  | cos( x )        | 返回 x 的余弦；x 是弧度。                                    |
  | sin( x )        | 返回 x 的正弦；x 是弧度。                                    |
  | exp( x )        | 返回 x 幂函数。                                              |
  | log( x )        | 返回 x 的自然对数。                                          |
  | sqrt( x )       | 返回 x 平方根。                                              |
  | int( x )        | 返回 x 的截断至整数的值。                                    |
  | rand( )         | 返回任意数字 n，其中 0 <= n < 1。                            |
  | srand( [Expr] ) | 将 rand 函数的种子值设置为 Expr 参数的值，或如果省略 Expr 参数则使用某天的时间。返回先前的种子值。 |

  ``` shell
  awk 'BEGIN{OFMT="%.3f";fs=sin(1);fe=exp(10);fl=log(10);fi=int(3.1415);print fs,fe,fl,fi;}'
  #output:  0.841 22026.466 2.303 3
  #OFMT 设置保留小数位数
  
  #随机数
  awk 'BEGIN{srand();fr=int(100*rand());print fr;}'
  ```

  | **函数**                            | **说明**                                                     |
  | ----------------------------------- | ------------------------------------------------------------ |
  | gsub( Ere, Repl, [ In ] )           | 除了正则表达式所有具体值被替代这点，它和 sub 函数完全一样地执行，。 |
  | sub( Ere, Repl, [ In ] )            | 用 Repl 参数指定的字符串替换 In 参数指定的字符串中的由 Ere 参数指定的扩展正则表达式的第一个具体值。sub 函数返回替换的数量。出现在 Repl 参数指定的字符串中的 &（和符号）由 In 参数指定的与 Ere 参数的指定的扩展正则表达式匹配的字符串替换。如果未指定 In 参数，缺省值是整个记录（$0 记录变量）。 |
  | index( String1, String2 )           | 在由 String1 参数指定的字符串（其中有出现 String2 指定的参数）中，返回位置，从 1 开始编号。如果 String2 参数不在 String1 参数中出现，则返回 0（零）。 |
  | length [(String)]                   | 返回 String 参数指定的字符串的长度（字符形式）。如果未给出 String 参数，则返回整个记录的长度（$0 记录变量）。 |
  | blength [(String)]                  | 返回 String 参数指定的字符串的长度（以字节为单位）。如果未给出 String 参数，则返回整个记录的长度（$0 记录变量）。 |
  | substr( String, M, [ N ] )          | 返回具有 N 参数指定的字符数量子串。子串从 String 参数指定的字符串取得，其字符以 M 参数指定的位置开始。M 参数指定为将 String 参数中的第一个字符作为编号 1。如果未指定 N 参数，则子串的长度将是 M 参数指定的位置到 String 参数的末尾 的长度。 |
  | match( String, Ere )                | 在 String 参数指定的字符串（Ere 参数指定的扩展正则表达式出现在其中）中返回位置（字符形式），从 1 开始编号，或如果 Ere 参数不出现，则返回 0（零）。RSTART 特殊变量设置为返回值。RLENGTH 特殊变量设置为匹配的字符串的长度，或如果未找到任何匹配，则设置为 -1（负一）。 |
  | split( String, A, [Ere] )           | 将 String 参数指定的参数分割为数组元素 A[1], A[2], . . ., A[n]，并返回 n 变量的值。此分隔可以通过 Ere 参数指定的扩展正则表达式进行，或用当前字段分隔符（FS 特殊变量）来进行（如果没有给出 Ere 参数）。除非上下文指明特定的元素还应具有一个数字值，否则 A 数组中的元素用字符串值来创建。 |
  | tolower( String )                   | 返回 String 参数指定的字符串，字符串中每个大写字符将更改为小写。大写和小写的映射由当前语言环境的 LC_CTYPE 范畴定义。 |
  | toupper( String )                   | 返回 String 参数指定的字符串，字符串中每个小写字符将更改为大写。大写和小写的映射由当前语言环境的 LC_CTYPE 范畴定义。 |
  | sprintf(Format, Expr, Expr, . . . ) | 根据 Format 参数指定的 [printf](http://www.cnblogs.com/chengmo/admin/zh_CN/libs/basetrf1/printf.htm#a8zed0gaco) 子例程格式字符串来格式化 Expr 参数指定的表达式并返回最后生成的字符串。 |

  其中 Ere可以为正则表达式

  ``` shell
  #gsub sub使用
  awk 'BEGIN{info="this is a test2010test!";gsub(/[0-9]+/,"!",info);print info}' 
  #output: this is a test!test!
  #在 info中查找满足正则表达式，/[0-9]+/ 用””替换，并且替换后的值，赋值给info 未给info值，默认是$0
  
  #查找字符串
  awk 'BEGIN{info="this is a test2010test!";print index(info,"test")?"ok":"no found";}'
  #output: ok
  
  #正则匹配
  awk 'BEGIN{info="this is a test2010test!";print match(info,/[0-9]+/)?"ok":"no found";}'
  #output: ok
  
  #截取字符串（substr）
  awk 'BEGIN{info="this is a test2010test!";print substr(info,4,10);}'
  #output: s is a tes
  #从第 4个 字符开始，截取10个长度字符串
  
  #分割字符串（split）
  awk 'BEGIN{info="this is a test";split(info,tA," ");print length(tA);for(k in tA){print k,tA[k];}}'
  #output:
  '''
  4
  1 this
  2 is
  3 a
  4 test
  '''
  #分割info,动态创建数组tA, awk for …in 循环，是一个无序的循环。 并不是从数组下标1…n ，因此使用时候需要注意。
  ```

  | **格式符** | **说明**                      |
  | ---------- | ----------------------------- |
  | %d         | 十进制有符号整数              |
  | %u         | 十进制无符号整数              |
  | %f         | 浮点数                        |
  | %s         | 字符串                        |
  | %c         | 单个字符                      |
  | %p         | 指针的值                      |
  | %e         | 指数形式的浮点数              |
  | %x         | %X 无符号以十六进制表示的整数 |
  | %o         | 无符号以八进制表示的整数      |
  | %g         | 自动选择合适的表示法          |

  ``` shell
  awk 'BEGIN{n1=124.113;n2=-1.224;n3=1.2345; printf("%.2f,%.2u,%.2g,%X,%o\n",n1,n2,n3,n1,n1);}'
  #output: 124.11,18446744073709551615,1.2,7C,174
  ```

  | **函数**                           | **说明**                                                     |
  | ---------------------------------- | ------------------------------------------------------------ |
  | close( Expression )                | 用同一个带字符串值的 Expression 参数来关闭由 print 或 printf 语句打开的或调用 getline 函数打开的文件或管道。如果文件或管道成功关闭，则返回 0；其它情况下返回非零值。如果打算写一个文件，并稍后在同一个程序中读取文件，则 close 语句是必需的。 |
  | system(Command )                   | 执行 Command 参数指定的命令，并返回退出状态。等同于 [system](http://www.cnblogs.com/chengmo/admin/zh_CN/libs/basetrf2/system.htm#a181929c) 子例程。 |
  | Expression \| getline [ Variable ] | 从来自 Expression 参数指定的命令的输出中通过管道传送的流中读取一个输入记录，并将该记录的值指定给 Variable 参数指定的变量。如果当前未打开将 Expression 参数的值作为其命令名称的流，则创建流。创建的流等同于调用 [popen](http://www.cnblogs.com/chengmo/admin/zh_CN/libs/basetrf1/popen.htm#sk62b0shad) 子例程，此时 Command 参数取 Expression 参数的值且 Mode 参数设置为一个是 r 的值。只要流保留打开且 Expression 参数求得同一个字符串，则对 getline 函数的每次后续调用读取另一个记录。如果未指定 Variable 参数，则 $0 记录变量和 NF 特殊变量设置为从流读取的记录。 |
  | getline [ Variable ] < Expression  | 从 Expression 参数指定的文件读取输入的下一个记录，并将 Variable 参数指定的变量设置为该记录的值。只要流保留打开且 Expression 参数对同一个字符串求值，则对 getline 函数的每次后续调用读取另一个记录。如果未指定 Variable 参数，则 $0 记录变量和 NF 特殊变量设置为从流读取的记录。 |
  | getline [ Variable ]               | 将 Variable 参数指定的变量设置为从当前输入文件读取的下一个输入记录。如果未指定 Variable 参数，则 $0 记录变量设置为该记录的值，还将设置 NF、NR 和 FNR 特殊变量。 |

  ```shell
  #打开外部文件（close）
  awk 'BEGIN{while("cat /etc/passwd"|getline){print $0;};close("/etc/passwd");}'
  
  #逐行读取外部文件（getline）
  awk 'BEGIN{while(getline < "/etc/passwd"){print $0;};close("/etc/passwd");}'
  #读取输入
  awk 'BEGIN{print "Enter your name:";getline name;print name;}'   
  
  #调用外部应用程序（system）
  awk 'BEGIN{b=system("ls -al");print b;}'
  # b返回值，是执行结果。
  ```

  | **函数名**                         | **说明**                                                     |
  | ---------------------------------- | ------------------------------------------------------------ |
  | mktime( YYYY MM DD HH MM SS[ DST]) | 生成时间格式                                                 |
  | strftime([format [, timestamp]])   | 格式化时间输出，将时间戳转为时间字符串 具体格式，见下表.     |
  | systime()                          | 得到时间戳,返回从1970年1月1日开始到当前时间(不计闰年)的整秒数 |

  ``` shell
  awk 'BEGIN{tstamp=mktime("2021 09 02 12 12 12");print strftime("%c",tstamp);}'
  #output: 2021年09月 2日 12:12:12
  
  awk 'BEGIN{tstamp1=mktime("2021 09 02 12 12 12");tstamp2=mktime("2021 10 01 0 0 0");print tstamp2-tstamp1;}'
  #output: 2461668
  #求2个时间段中间时间差,介绍了strftime使用方法
  
  awk 'BEGIN{tstamp1=mktime("2021 09 02 12 12 12");tstamp2=systime();print tstamp2-tstamp1;}'
  #output in 2021/9/2 10:07: -7496
  
  #strftime日期和时间格式说明符，见下表
  ```

  | 格式 | 描述                                                     |
  | ---- | -------------------------------------------------------- |
  | %a   | 星期几的缩写(Sun)                                        |
  | %A   | 星期几的完整写法(Sunday)                                 |
  | %b   | 月名的缩写(Oct)                                          |
  | %B   | 月名的完整写法(October)                                  |
  | %c   | 本地日期和时间                                           |
  | %d   | 十进制日期                                               |
  | %D   | 日期 08/20/99                                            |
  | %e   | 日期，如果只有一位会补上一个空格                         |
  | %H   | 用十进制表示24小时格式的小时                             |
  | %I   | 用十进制表示12小时格式的小时                             |
  | %j   | 从1月1日起一年中的第几天                                 |
  | %m   | 十进制表示的月份                                         |
  | %M   | 十进制表示的分钟                                         |
  | %p   | 12小时表示法(AM/PM)                                      |
  | %S   | 十进制表示的秒                                           |
  | %U   | 十进制表示的一年中的第几个星期(星期天作为一个星期的开始) |
  | %w   | 十进制表示的星期几(星期天是0)                            |
  | %W   | 十进制表示的一年中的第几个星期(星期一作为一个星期的开始) |
  | %x   | 重新设置本地日期(08/20/99)                               |
  | %X   | 重新设置本地时间(12：00：00)                             |
  | %y   | 两位数字表示的年(99)                                     |
  | %Y   | 当前月份                                                 |
  | %Z   | 时区(PDT)                                                |
  | %%   | 百分号(%)                                                |

​		







* awk实例

  * 查看服务里活动链接（查看Linux服务器活动链接状态，根据命令的返回结果，可以推论出当前服务器负载情况）

    ``` shell
    #windows + git bash
    netstat -n | awk '/TCP/{++S[$NF]}END{for(key in S) print key,"\t",S[key]}'
    
    #linux ?
    netstat -n | awk '/^tcp/{++S[$NF]}END{for(key in S) print key,"\t",S[key]}'
    
    #首先通过/^tcp/过滤出TCP的连接状态，然后定义S[]数组；$NF表示最后一列，++S[$NF]表示数组中$NF的值+1，END表示在最后阶段要执行的命令，通过for循环遍历整个数组；最后print打印数组的键和值
    
    netstat -n | grep TCP
    #output:   TCP    127.0.0.1:1647         127.0.0.1:1648         ESTABLISHED
    netstat -n | awk '/TCP/{print NF}'      #NF表示当前行的最后一行，用数字表示
    #output: 4
    netstat -n | awk '/TCP/{print $NF}'   	#$NF表示最后一列的字符串
    #output: ESTABLISHED
    netstat -n | awk '/TCP/{print $(NF-1)}' #$(NF-1)表示倒数第二行
    ```

  * web站点访问日志

    ``` shell
    #!/bin/bash
    # create author of cfwl
    # create date of 2013-10-12
    # scripts function analyze web site access log
    # print web site log and sort
    cat /application/nginx-1.2.9/logs/access.log | awk '{print $1,$4,$9,$19,$22}' |sort | uniq -c | sort -nr
    ```

    ``` shell
    #!/bin/bash
    # create author of cfwl
    # create date of 2013-10-12
    # scripts function analyze web site access log
    sh analyze_web_log.sh|
    awk '
    BEGIN{
      print "----------------------------------------------------------------------------";
      print "| Total | Access_IP |  Access_Date  | State | Access_browser |";
      print "----------------------------------------------------------------------------";
    }
    {
      printf "| %-5s | %-14s | %-11s | %-5s | %-15s |\n",$1,$2,$3,$4,$5,$6;
    }
    END{
      print "----------------------------------------------------------------------------";
    }'
    ```

    

* awk基本使用

  * trim

    [trim.awk](https://gist.github.com/andrewrcollins/1592991)

    trim.awk

    ``` shell
    function ltrim(s) { sub(/^[ \t\r\n]+/, "", s); return s }
    function rtrim(s) { sub(/[ \t\r\n]+$/, "", s); return s }
    function trim(s)  { return rtrim(ltrim(s)); }
    BEGIN {
    # whatever
    }
    {
    # whatever
    }
    END {
    # whatever
    }
    ```

  * basename or endswith

    ``` shell
    awk '$4 ~ /\/foo$/ { print $1 }'
    # /a/b/c/foo   匹配/foo

  * substr()

    ``` shell
    '''
    test.txt
    F115!16201!1174113017250745 10.86.96.41 211.140.16.1 200703180718
    F125!16202!1174113327151715 10.86.96.42 211.140.16.2 200703180728
    F235!16203!1174113737250745 10.86.96.43 211.140.16.3 200703180738
    F245!16204!1174113847250745 10.86.96.44 211.140.16.4 200703180748
    F355!16205!1174115827252725 10.86.96.45 211.140.16.5 200703180758
    '''
    awk -F '[ !]' '{print substr($3,6)}' test.txt
    #output: 
    '''
    13017250745
    13327151715
    13737250745
    13847250745
    15827252725
    '''
    
    '''
    test2.txt
    2007-08-04 04:45:03.084 - SuccessfulTradeResult(status: 1, currencyPair: 'USDJPY', tradeId: '17389681', clientReference: '20070803COVR00013176', tradeDateTime: '2007-08-03T19:45:02', dealerUserId: 'PANTARHEI.API1', clientName: 'PANTA RHEI SECURITIES CO LTD ', clientId: 'EU0271383', counterpartyName: 'DB', buySell: 'S', nearLeftAmount: 1810000.0, nearRightAmount: 2.138696E8, nearRate: 118.16, nearValueDate: '2007-08-07')
    '''
     grep -v 'errorMessage' ./test2.txt | awk -F',' '{printsubstr($4,20)","substr($3,12,8)","substr($2,17,6)","substr($5,18,19)","substr($9,21,2)","substr($10,12,1)","substr($11,18)","substr($12,19)","substr($13,12)","substr($14,18,10)}' | tr -d "'"
    
    #output:
    '''
    20070803COVR00013176,17389681,USDJPY,2007-08-3T19:45:02,DB,S,1810000.0,2.138696E8,118.16,2007-08-07
    '''
    
    #substr($4,20)    --->  表示是从第4个字段里的第20个字符开始，一直到设定的分隔符","结束.
    #substr($3,12,8)  --->  表示是从第3个字段里的第12个字符开始，截取8个字符结束.
    #substr($3,6)     --->  表示是从第3个字段里的第6个字符开始，一直到设定的分隔.
    ```

    