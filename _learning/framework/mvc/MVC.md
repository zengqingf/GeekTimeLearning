# MVC框架

* MVC

  ``` tex
  代码重用 （code reusability）
  关注分离点 （separation of concerns, SoC）
  ```

  











---



### PureMVC

* 核心

  ``` tex
  除了Model，View,Controller外
  
  还有Facade（提供了与核心层通信的唯一接口）
  Proxy: Model 保存对 Proxy 对象的引用，Proxy 负责操作数据模型，与远程服务通信存取数据。
  Mediator: View 保存对 Mediator 对象的引用 。由 Mediator 对象来操作具体的视图组件，包括：添加事件监听器，发送或接收 Notification ，直接改变视图组件的状态。这样做实现了把视图和控制它的逻辑分离开来。
  Command: Controller 保存所有 Command 的映射。Command 类是无状态的，只在需要时才被创建。Command 可以获取 Proxy 对象并与之交互，发送 Notification，执行其他的 Command。
  ```

  ![](https://raw.githubusercontent.com/MJX1010/PicGoRepo/main/img/202109291139556.png)

