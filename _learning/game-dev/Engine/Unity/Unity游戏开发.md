# Unity游戏开发

* Assetbundle
* Editor
* Shader
* Unity Plugins
* 功能实现
* 玩法实现
* 优化



---



* behaviac
* graphic / texture compress 见wiz
* runtime permissions
* spine animation





---



### 游戏更新

* Unity il2cpp热更

  [Unity Android APP il2cpp热更新解决方案](https://www.gameres.com/830868.html)



* ILRuntime

  [github - ILRuntime](https://github.com/Ourpalm/ILRuntime)

  [ILRuntime入门笔记](https://www.cnblogs.com/zhaoqingqing/p/10274176.html)

  [对C#热更新方案ILRuntime的探究](https://www.cnblogs.com/zblade/p/9041400.html)

  ``` tex
  C#代码在编写后，是需要执行编译的，才能起效，这样如果在手机端，没有对应的编译环境，那么对应的c#代码就无法实现热更。ILRuntime实现的基础，也是基于AssetBundle的资源热更新方式，将需要热更新的c#代码打包成DLL，放在工程的StreamingAssets下，在每次完成资源打包后，对应的DLL会被作为资源热更新出去。这样就规避了编译相关的环节，实现了热更。
  ```

  



---



### 游戏案例

* 海战

  [从零开始用Unity做一个海战游戏](https://zhuanlan.zhihu.com/p/46569993)	[demo](https://github.com/tank1018702/unity-004)





---



### 读《Unity3D 高级编程 - 主程手记》

#### 一、架构

* 游戏架构

  * 前端渲染引擎
  * UI系统
  * AI行为算法，行为树？状态机？事件型决策树
  * 数据获取和存储
  * 场景拆分方式
  * 资源分离方式
  * 长连接or短连接
  * TCP or UDP
  * 服务端语言
  * 数据库，关系型？加入Cache机制？
  * 网络协议，ProtocolBuff? Json? XML? 自定义格式
  * ...

* 架构图

  * 承载力

    能够承载多少个逻辑系统，代码行数限制，彼此工作的耦合度，人员共同开发数量和效率

    服务器承载量，日均访问量；客户端显示UI数量和渲染模型数量（同屏渲染和非同屏渲染）

  * 

* 架构好坏

  

  

  

  

  



