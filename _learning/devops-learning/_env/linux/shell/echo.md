# echo输出

* printf

  printf >= echo

  ``` shell
  echo "hello world"      #默认是换行输出
  printf "hello world\n"  #需要加\n 进行末尾换行
  
  echo -e "hello \nworld"  #使用”-e”参数，使其”\n”具有特殊意义，实现字符换行输出
  printf "hello \nworld\n"
  
  echo -n "hello world"	#默认是换行输出，使用”-n”选项可用实现不换行输出。
  printf "hello world"
  
  echo -e "hello \tworld" #通过”\t”参数，可用实现字符之间有一个tab建的距离。
  printf "hello \tworld\n"
  
  #判断上一条命令执行的结果是否正确
  #通过”$?”判断上一条命令执行后返回的结果，0：正确，1-255：错误
  echo $?
  printf "$? \n"
  
  #输出变量值
  hello=world
  echo $hello
  printf "$hello \n"
  
  
  #输出字符串带颜色
  #!/bin/bash
  echo -e "\033[31mplease input one number: \033[0m"  #-->红色
  echo -e "\033[32mplease input one number: \033[0m"  #-->绿色
  echo -e "\033[33mplease input one number: \033[0m"  #-->黄色
  echo -e "\033[34mplease input one number: \033[0m"  #-->蓝色
  echo -e "\033[35mplease input one number: \033[0m"  #-->粉色
  echo -e "\033[36mplease input one number: \033[0m"  #-->青色
  echo -e "\033[37mplease input one number: \037[0m"  #-->白色
  
  #!/bin/bash
  printf "\033[31mplease input one number: \033[0m\n"
  printf "\033[32mplease input one number: \033[0m\n"
  printf "\033[33mplease input one number: \033[0m\n"
  printf "\033[34mplease input one number: \033[0m\n"
  printf "\033[35mplease input one number: \033[0m\n"
  printf "\033[36mplease input one number: \033[0m\n"
  printf "\033[37mplease input one number: \033[0m\n"
  ```

  ``` shell
  # printf 特性
  
  #从命令行引用传值
  printf "%s %s %s %s\n" Are you happy today
  #Are you happy today分别被%s->Are %s->you %s->happy %s->today引用，终端输出的结果即为Are you happy today
  printf "%s %s %s\n" 1 2 3 4 5 6
  #output: 1 2 3
  #		 4 5 6
  
  
  #格式化输出
  printf "%s %10s %10s %10s\n" Are you happy today
  #%s字符之间分隔的空间，%10s表示分隔10个字符的间距。
  
  
  #使用前导零扩展%d
  for ip in `seq 00 20`;do printf "%d \n" $ip;done
  #output:
  '''
  0
  1
  2
  ...
  '''
  #上述命令使用for循环00-20的数字，因为%d默认精度为1，printf打印在终端的结果为0-20。
  
  for ip in `seq 00 99`;do printf "%02d \n" $ip;done
  #”%02d”使其默认的精度从1变为2，printf打印的结果便为我们看到的00-20，在数字1前置0.
  #output:
  '''
  00
  01
  02
  ...
  '''
  # %d：接受整数值并将它转换为有符号的十进制符号表示法。精度指定显示的最小数字位数。如果值转换后可以用更少的位数来表示，将使用前导零扩展。缺省精度是 1。
  ```
  
  