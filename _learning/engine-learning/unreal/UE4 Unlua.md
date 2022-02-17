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

  

* 蓝图 + UnLua 开发 UserWidget

  ``` tex
  为ListView创建Item(Entry)对象，并创建ItemData(Item)数据
  
  ItemData(Item)数据对象继承 蓝图的UObject，添加界面用变量和事件
  事件可以分发ItemData的变化给外部监听对象，
  	也可以ItemData关联的Item(Entry)的改变（Selected，InnerBtnClick等）分发给外部监听对象
  ```

  ``` lua
  --unlua中调用蓝图接口
  --蓝图接口 Tarray GetList()
  --lua中需要 遍历Tarray时，Tarray所包含的函数可以在蓝图编辑窗口将方法引线拖出可用函数
  --[[
  	Tarray的遍历需要使用 arrayList:Get(i)
  ]]
  
  --[[
  @注意， 序号从0开始还是从1开始
  ]]
  
  --GetItemAt(index) 从0开始
  ListView:GetItemAt(0)
          --例子
          submitItems = Vector.New() --自定义lua类
          for i = 1, submitItems:Size() do
             local itemGridObj = self.TileViewSubmitItem:GetItemAt(i - 1)
             itemGridObj:Update(submitItems[i])
             itemGridObj:AddSelectedHandler(self, self.OnSubmitItemSelected)
          end
  
  
  --Get(index) 从1开始
  TArray:Get(1)
          --例子
          local items = self.TileViewSubmitItem:GetListItems()
          for i = 1, items:Length() do
              local item = items:Get(i)
              if item ~= nil then
                 item:RemoveSelectedHandler(self, self.OnSubmitItemSelected)
              end
          end
  ```
  



* UnLua调用C++

  * 传给C++数字型字符串，C++可自动解析为数字

    ``` lua
    function TaskDataManager:ParseTaskTrackLinkText(type2, params)
        local iType2 = TaskDetailData.TASK_LINK_TYPE2[type2]
        if iType2 == 0 or iType2 == 1 then                  --寻路
            local targetId = params:Find("p1")						      --TODO test  unlua传给c++ 字符串和整型 是一样的效
            self:TaskAutoTrace(iType2, targetId)                          --TODO test  unlua传给c++ 字符串和整型 是一样的效果
    end
    ```

    ``` c++
    	/// <summary>
    	///	任务追踪
    	///	type: 0表示npc寻路， 1表示关卡寻路
    	///	targetId: 目标id
    	/// mapId: 地图城镇表ID
    	/// </summary>
    	UFUNCTION(BlueprintCallable)
    	static void StartAutoTaskTrace(int32 type, int32 targetId);
    ```






* UnLua延迟调用

  ``` lua
  --方法1
  --创建一个协程 延迟调用函数 测试代码
  function MailFrameView_C:DelayFunc(func, Induration)
     coroutine.resume(coroutine.create(MailFrameView_C.DoDelay), self, self, Induration)
  end
  
  function MailFrameView_C:DoDelay(worldContentObject, duration)
      UE4.UKismetSystemLibrary.Delay(worldContentObject, duration)
      --TODO 其他方法
  end
  ```

  ``` lua
  --方法2
  local anim = function()
       self:PlayGeActorQueueAction(index, animPaths)
       CommonUtility.ClearDelayCall(self,self.mHandle)
  end
  self.mHandle = CommonUtility.DelayCall({self,anim},0.01,false)
  
  --例子：
  --self.mMyHandle = CommonUtility.DelayCall({self, GameModeTown_C.OpenChapterSelectFrame}, 0.1, false)
  --function GameModeTown_C:OpenChapterSelectFrame()
  --    UIManager.OpenFrame(UIFrameClassPath.ChapterSelectFrame, 1, nil)
  --    CommonUtility.ClearDelayCall(self, self.mMyHandle)
  --end
  --
  --delegate:委托事件
  --time：延迟时间
  --isLoop：是否循环
  --在回调结束要清除自己的handle  UE4.UKismetSystemLibrary.K2_ClearTimerHandle(self, self.mHandle)
  ---重点提示：每一个使用DelayCall的地方都要考虑一下，调用对象是否有可能在DelayCall触发之前就已经不存在了。如果有可能，那么在调用者的析构函数里也要ClearDelayCall()。
  ---如果isLoop为true，考虑DelayCall是否会重复调用
  ---DelayCall的回调只能在UObject中调用，如果需要在Frame里调用，需要借用FrameView写回调
  function CommonUtility.DelayCall(delegate,time,isLoop)
      local handle = UE4.UKismetSystemLibrary.K2_SetTimerDelegate(delegate, time, isLoop)
      return handle
  end
  
  function CommonUtility.ClearDelayCall(target,handle)
      UE4.UKismetSystemLibrary.K2_ClearTimerHandle(target, handle)
  end
  ```

  



---



### UE4 & UnLua内存管理

* 



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

  

* require 时序问题

  ``` lua
  --Global.lua
  --DataManager模块
  DataManagerModuleMgr	= require "DataManager/DataManagerModule"
  --界面基类
  FrameBase 				= require "UI/Base/FrameBase"
  
  --DataManagerModule.lua
  TeamDataMgr = require "DataManager/TeamDataManager"
  
  --TeamDataManager.lua
  --TODO 移除，仅测试
  TeamMF = require("UI/Frame/TeamFrame/Main/TeamMainFrame")
  
  --TeamMainFrame.lua
  require('Base/extern')
  local TeamMainFrame = class("TeamMainFrame", FrameBase)
  function TeamMainFrame:Ctor()
      self.Super.Ctor(self)
  end
  
  --error: 导致 TeamMainFrame.lua中获取不到FrameBase作为Super
  --因为Global.lua中 DataMgr模块require先于FrameBase  继承DataMgr的TeamDataMgr中require了TeamMainFrame,导致引入的TeamMainFrame未继承FrameBase
  ```

  