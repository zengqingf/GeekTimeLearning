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

