# Unreal4 术语

* 常见视图模式

  | 模式     | 效果                                         |
  | -------- | -------------------------------------------- |
  | 带光照   | 显示应用了材质和光照的场景的最终结果         |
  | 不带光照 | 从场景中移除所有光照，显示来自材质的基础颜色 |
  | 线框     | 显示场景中Actor的所有多边形的边              |
  | 细节光照 | 在整个场景中显示中性材质，使用法线贴图       |
  | 仅光照   | 显示无法线信息且仅被光照影响的中性材质       |




---



* UE4 模块

  [UE4 模块,PrivateDependencyModuleNames?](https://zhuanlan.zhihu.com/p/107270501)

  [查LP_UE4 Build System：Target and Module](https://imzlp.com/posts/16643/)
  
  * Public / Private 文件夹
  * Build.cs 之 DependencyModuleNames / IncludePathModuleNames 依赖
  
  ``` text
  PublicDependencyModuleNames
  添加对执行 Module 的源文件依赖，自动添加所依赖 Module 的 Public 和 Private 源文件包含。
  
  PrivateDependencyModuleNames
  与 PublicDependencyModuleNames 不同的是，意味着所依赖的 Module 中的源文件只可以在 Private 中使用。
  
  假如现在有一个模块 A，还有一个模块 B，他们中都是 UE 的 Module/Public 和 Module/Private 的文件结构。
  
  如果 B 中依赖 A，如果使用的是 PrivateDependencyModuleNames 的方式添加的依赖，则 A 模块的源文件只可以在 B 的 Private 目录下的源文件中使用，在 Public 目录下的源文件使用时会报 No such file or directory 的错误。
  如果使用的是 PublicDependencyModuleNames 方式添加的依赖，则 A 的源文件在 B 的 Public 与 Private 中都可用。
  除了上述的区别之外，还影响依赖于 B 模块的模块 ，当一个模块 C 依赖模块 B 的时候，只能访问到 B 模块的 PublicDependencyModule 中的模块暴露出来的类。
  例如，C 依赖 B，B 依赖 A；那么，假如 C 想访问 A 中的类则有两种方式：
  
  1.在 C 的依赖中添加上 A 模块
  2.确保 B 在 PublicDependencyModuleNames 依赖中添加的 A 模块，这样 C 就可以间接的访问到 A。
  
  经过测试发现，其实对于游戏模块 (PROJECT_NAME/Source/PROJECT_NAME.target.cs) 使用而言，所依赖的模块是使用 PublicDependencyModuleNames 还是 PrivateDependencyModuleNames 包含，没什么区别。
  使用 Private 方式依赖的 Module 中的头文件依然可以在游戏模块的 Public 中用，这一点与插件等其他模块有所不同（但是这只有在所依赖的模块不是 bUsePreCompiled 的基础上的，如果所依赖的模块是 bUsePreCompiled 的，则与其他的模块一样，PrivateDependencyModuleNames 依赖的模块不可以在 Pulibc 目录下的源文件使用），这个行为比较奇怪：有时候出错有时又不出错。
  
  注意：在游戏项目中使用依赖其他 Module 时尽量确定性需求地使用 PrivateDependencyModuleNames 或者 PublicDependencyModuleNames，在组合其他的选项时可能会有一些奇怪的行为。
  
  注意：使用DependencyModuleNames时,会进行对两个模块进行链接，
  只有与其它模块链接之后,才能使用其它模块的定义在.cpp里函数(若使用了定义在.cpp内函数,但又没将引用到的模块添加到DependencyModuleNames,就会获得"无法解析的外部符号(unresolved external symbol)"错误)
  ```
  
  **考虑多模块链接问题**
  
  ![](https://i.loli.net/2021/03/26/cFfqtValOmno1yN.jpg)

