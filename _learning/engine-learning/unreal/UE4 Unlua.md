# UE4 UnLua

### 基础

* UnLua资源

  https://github.com/Tencent/UnLua

* UnLua应用场景

  ``` tex
  访问蓝图或C++变量
  	访问蓝图变量，播放UMG动画
  	访问C++变量，Lua创建MyActor
  扩展C++函数实现
  ...
  ```

* UnLua IDE

  VSCode + EmmyLua实现调试



* Demo学习

  ``` tex
  UnLua TPS demo
  1. UI按钮事件
  2. 操作事件绑定并演示蓝图如何绑定
  3. 动作蓝图
  ...
  ```



* 扩展学习

  ``` tex
  Lua栈函数
  基于原生Lua的C++接口实现
  ```

  





---



### 遇到的问题

* Android NDK 21b / 21e 打包时，编译报错

  ``` tex
  C:/build_env/a8/android_ndk/android-ndk-r21b/toolchains/llvm/prebuilt/windows-x86_64/sysroot/usr/include\bits/pthread_types.h(87,3): error: unknown type name 'int32_t'
    int32_t __private[14];
    ^
  C:/build_env/a8/android_ndk/android-ndk-r21b/toolchains/llvm/prebuilt/windows-x86_64/sysroot/usr/include\bits/pthread_types.h(98,3): error: unknown type name 'int64_t'
    int64_t __private;
    ^
  In file included from C:/jks111/workspace/A8/Observer/Compile/build/Program/Client/NextGenGame/Plugins/ThirdParty/UnLua/Source/ThirdParty/Lua/src/lfunc.c:17:
  In file included from C:/jks111/workspace/A8/Observer/Compile/build/Program/Client/NextGenGame/Plugins/ThirdParty/UnLua/Source/ThirdParty/Lua/src/ldebug.h:11:
  In file included from C:/jks111/workspace/A8/Observer/Compile/build/Program/Client/NextGenGame/Plugins/ThirdParty/UnLua/Source/ThirdParty/Lua/src/lstate.h:125:
  In file included from C:/build_env/a8/android_ndk/android-ndk-r21b/toolchains/llvm/prebuilt/windows-x86_64/sysroot/usr/include\signal.h:33:
  C:/build_env/a8/android_ndk/android-ndk-r21b/toolchains/llvm/prebuilt/windows-x86_64/sysroot/usr/include\sys/types.h(48,9): error: unknown type name 'uint32_t'
  typedef uint32_t __id_t;
          ^
  C:/build_env/a8/android_ndk/android-ndk-r21b/toolchains/llvm/prebuilt/windows-x86_64/sysroot/usr/include\sys/types.h(72,9): error: unknown type name 'uint64_t'
  typedef uint64_t ino64_t;
  ```

  