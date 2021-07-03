# UE4 UI开发



### UMG

* By C++

  * link

    [UE4 C++ —— UMG和C++交互](https://blog.csdn.net/niu2212035673/article/details/82792910)

    ``` tex
    交互方法
    一，强转子集
    GetRootWidget()        //获取根节点
    GetChildAt()            //获取子节点
    UMG控件呈树状结构，根据根节点可以获取到所有的子节点
    二，反射绑定
    UPROPERTY(Meta = (BindWidget))
    UButton *ButtonOne;
    绑定的类型和名称必须和蓝图内的一致
    三，根据控件名获取
    GetWidgetFromName()
    获取到UWidget*类型，强转成指定类型
    ```

    示例见：TMSDK/Unreal/UE4.25/GCloudTest







---



### Lua







---

