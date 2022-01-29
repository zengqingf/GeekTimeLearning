# UE4 蓝图



---

### UE4 基础

* 概念

  ``` tex
  蓝图也是一种代码
  以节点形式存在，节点可以是：
  	属性（Get,Set函数）
  	函数（C++/蓝图）			UE库函数：KismetSystemLibrary.h   GameplayStatics.h
  	宏定义（蓝图中的宏）
  	条件判断节点（Branch, Not）
  	循环结构（ForEach）
  	...
  	
  大部分节点都是C++定义好的函数（静态、成员函数，如：PrintString），函数有调用对象（蓝图内调用成员函数 Target为Self）
  必须遵循一定的规则，C++才能定义蓝图可用的节点（如：UPROPERTY() ）
  蓝图也可以定义蓝图使用的节点（自定义事件，函数）
  蓝图不需要编译器编译（也有编译按钮，实际是解释执行？）
  蓝图运行效率没有C++高
  
  UE中大部分UE的类或者函数都需要和蓝图进行交互，同时蓝图也会用到所有C++类和函数
  如，角色蓝图：即继承自C++类的ACharacter；动画蓝图：继承自C++类UAnimInstance，等等
  
  
  区别于C++
  蓝图不用太关心对象，指针，引用等，不用太关注内存，蓝图函数变量命名规则没有太多限制（变量可以带中文或特殊符号）
  ```

  





---



### UE4 蓝图技巧

* 快捷键

  ``` tex
  删除连线： Alt + 鼠标左键
  ```

  





---
