# UE4 性能优化

#### 1. 内存优化

* 内存泄漏

  * ref

    [UE4内存分析方法](https://zhuanlan.zhihu.com/p/431759166)

  - 命令：Obj list -alphasort 打印出所有现存的Object

    可在打开界面后回登陆，通过命令打印Object List。查看界面的Object数量

    ![image-20220711083557098](UE4 优化.assets/image-20220711083557098-16574997582381.png)

  - 命令：Obj trygc 手动GC

  - 命令：Obj Refs Name=TalentFrameView_C 打印TalentFrameView_C这个Object的被引用信息。

    ![image-20220711083625094](UE4 优化.assets/image-20220711083625094-16574997862172-16574997883063.png)

    可以分析出TalentFrameView_C被LuaPayloadData持有。

  