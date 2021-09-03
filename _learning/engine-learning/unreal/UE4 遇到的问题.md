

# UE4 遇到的问题

**UE4.25.4**

* ref

  [Unreal Engine 4：编译打包Android应用问题汇总](https://blog.csdn.net/netyeaxi/article/details/86510678)

  [Unreal Engine 4：编译打包Windows应用遇到的问题汇总](https://blog.csdn.net/netyeaxi/article/details/82286048)

  [UE4错误笔记](https://blog.ch-wind.com/ue4-compile-errors-log/)

  [UE4 - 奇奇怪怪的报错](https://www.jianshu.com/p/edd848e424bf)

* error C4273: xxx inconsistent dll linkage （不一致的dll链接）

  ``` text
  在VS中新建.h和.cpp，并且在.h中自定义了类的导入导出宏定义（如下），这与UE4自身的宏定义冲突了
  ```

  ``` c++
  XX为Plugin/Module的名字
  
  #ifdef XX_DLL
  #define XX_API __declspec(dllexport)
  #else
  #define XX_API __declspec(dllimport)
  #endif
  class XX_API XX {...};
  
  解决方法：
  	#include "CoreMinimal.h"
      class XX_API XX {...};
  ```

* error LNK2019: unresolved external symbol  ( module A和B，B调用不到A的Public下的.h  )   

  __LNK2019错误一般都是 compile时找到了相应的（.h）文件，但链接时找不到相应的 lib 库文件（也含dll文件）__

  ``` 
  解决方法： A中.h class中添加 A_API
  //A.h
  class A_API A {...};
  
  //B.h
  #include 'A.h'
  
  
  注意：需要在A.h中依赖的其他类及其子类中都添加  A_API !!!
  ```

* error LNK2019: unresolved external symbol ( UE4.26 编辑器创建C++继承AIController，并且调用了UNavigationSystemV1的GetCurrent等接口 )

  ``` text
  需要在模块的*.Build.cs中的 PublicDependencyModuleNames 引入 NavigationSystem
  

（如何确定名称是：NavigationSystem，可以在Unreal Engine API Reference中的 搜索到 UNavigationSystemV1，看文档中 Module: NavigationSystem 即可确定）
  https://docs.unrealengine.com/en-US/API/Runtime/NavigationSystem/UNavigationSystemV1/index.ht19ml![](https://i.loli.net/2021/03/09/WMlnuCSye8ozJpc.jpg)
  ```
  
* UE4输出日志乱码

  ``` text
  VS2019只安装English语言包，卸载中文语言包
  ```

* Fastbuild disabled unable to find any executable to use  （VS2019 调试 UE4时报错）

  ``` text
  解决方法：
  C:\Users\用户名\AppData\Roaming\Unreal Engine\BuildConfiguration.xml
  
  修改内容
  <?xml version="1.0" encoding="utf-8" ?>
  <Configuration xmlns="https://www.unrealengine.com/BuildConfiguration">
  <!--新增内容 start-->
      <BuildConfiguration>
          <bAllowXGE>false</bAllowXGE>
          <bAllowFASTBuild>false</bAllowFASTBuild>
      </BuildConfiguration>
  <!--新增内容 end-->
  </Configuration>
  
  ```

* error: member access into incomplete type  （**这个引用问题如果涉及子类的引用，还可能出现其他的编译错误，需要理清引用关系**）

  [出错member access into incomplete type](https://blog.csdn.net/wcjwdq/article/details/43604679)

  ``` text
  在c++中，有四个文件demoA.h, demoA.cpp, demoB.h, demoB.cpp。
  在demoA.h中有一个类为class demoA, 在demoB.h中有一个类为class demoB。
  如果demoA类要用到demoB类，并且demoB类也要用到demoA类，那么必须要如下：
  
  1、在demoA类的cpp文件中包含demoB类的头文件路径；
  2、在demoB类的h文件中加Class demoA，同时也要在demoB类的cpp文件中添加demoA类的头文件路径；
  
  // demoA.h
  class demoA
  {
  	demoB * b;
  };
   
  // demoA.cpp
  #include "demoB.h"
   
   
  // demoB.h
  class demoA;
  class demoB
  {
  	demoA * parent;
  };
   
  // demoB.cpp
  #include "demoA.h"
  
  如果在demoB.cpp中没有加入demoA.h，那么会报错：member access into incomplete type
  
  如果demoA和demoB的.h和.cpp需要互相引用，则在各自.h中添加对方的class A/B; ，在各自.cpp中引用对方的头文件 A/B.h
  ```

  

* UE4 游戏模块无法被加载 可能存在操作系统错误 或模块未被正确设置

  ``` text
  回滚 项目目录下 Binaries 中的 Win64   UE4Editor.modules等修改
  ```

* hides overloaded virtual function

  [c++ overloaded virtual function warning by clang?](https://stackoverflow.com/questions/18515183/c-overloaded-virtual-function-warning-by-clang)

  ``` c++
  /*
  在重载父类的同名虚函数时会出现hides overloaded virtual function编译警告。
  
  重载的虚函数被隐藏了
  
  首先是发生了  重载，子类重载了父类的函数，其次被重载的是  虚函数，这时这个被重载的父类的虚函数将会被   隐藏。
  */
  
  struct Base
  {
  	virtual void* get(const char* e);
  };
  
  struct Derived: public Base
  {
      //fix
  	using Base::get;
      
  	virtual void* get(const char* e, int index);
  
  //也可以放到私有位置 避免调用Base的get
  //private:
  //	using Base::get;
  };
  ```

* error: 'LOG_TAG' macro redefined

  ``` text
  frameworks/native/services/inputflinger/EventHub.cpp:34:9: error: 'LOG_TAG' macro redefined [-Werror,-Wmacro-redefined]
  #define LOG_TAG "EventHub"
          ^
  system/core/liblog/include/log/log.h:51:9: note: previous definition is here
  #define LOG_TAG NULL
  
  fix...
  
  //使用#undef
  #ifdef LOG_TAG
  #undef LOG_TAG
  #define LOG_TAG "EventHub"
  #endif
  
  
  //first : #define and after that : #include
  #define LOG_TAG "EventHub"
   ... 
  #include<utils/CallStack.h> 
  ```

* error: Pasting formed with invalid preprocessing token

  [Error: Pasting formed with invalid preprocessing token](https://stackoverflow.com/questions/50114062/error-pasting-formed-with-invalid-preprocessing-token)

  ``` c++
  #define GOTO_RC(row,col) "\033["#row";"#col"H"
  #define DISPLAY_WRITE(row, col, fmt, args) printf(GOTO_RC(row,col)  ## fmt, ## args)
  
  //try
  //DISPLAY_WRITE(24,1,"Command:",12);
  // error:...
  
  /*因为您只想连接两个字符串，所以请删除DISPLAY_WRITE中的##。相邻的两个字符串字面值将自动连接。##用于将两个标记连接在一起，然后再次将它们解释为标记，而不是字符串字面量。
  另外，不完全相关，但如果你想要能够采取多个“args”，你将需要支持可变参数宏(大多数相关的东西支持它们)，并做一些像这样:*/
  #define DISPLAY_WRITE(row, col, fmt, ...) printf(GOTO_RC(row,col) fmt,    __VA_ARGS__)	//DISPLAY_WRITE(1、2、“错误”, 111)
  //不打印参数
  #define DISPLAY_WRITE(row, col, fmt, ...) printf(GOTO_RC(row,col) fmt,  ##__VA_ARGS__).  //DISPLAY_WRITE(1、2、“错误”)
  ```

* delete-non-virtual-dtor

  [delete-non-virtual-dtor 问题解决办法](https://blog.csdn.net/qcxyliyong/article/details/84949466)

  ``` c++
  //基类的析构函数一般需要定义为虚函数
  
  class CAN_driver
  {
  public:
  CAN_driver();
  	//~CAN_driver();  //error:...
      //fix...
      virtual ~CAN_driver();
  }
   
  class CAN_Type1::pubic CAN_driver
  {
  public:
  CAN_Type1()
  ~CAN_Type1();
  }
   
  CAN_driver *veh_CAN = new CAN_Type1();
   
  int main()
  {
  //application codes.
   
  delete veh_CAN;
  return 0;
  }
  //删除基类指针时，并不会调用继承类CAN_Type1的析构函数，而基类添加virtual后，则可以
  ```

* error: c2757 A symbol used in the current compilation as a namespace identifier is already being used in a referenced assembly.

  ``` c++
  //HeaderA.h
  class foo
  {}
  
  //HeaderB.h
  namespace foo
  {
  class ABC{}
  }
  
  //HeaderC.h
  #include <HeaderA.h>
  #include <HeaderB.h>
  using namespace foo;
  
  class Toimplement{
  ABC a; //Throws Error C2757
  }
  
  //fix
  namespace bar {
      #include "HeaderA.h"
  }
  #include "HeaderB.h"
  
  ...
  bar::foo fooObject;
  foo::ABC abcObject;
  ```

* error: declaration shadows a field of 'TenmoveBattle::BeActorAIManager' [-Werror,-Wshadow]

  ``` c++
  //AI/BeActorAIManager.cpp(274,75): error: declaration shadows a field of 'TenmoveBattle::BeActorAIManager' [-Werror,-Wshadow]
  
  string test1(string &str1){
      string str1="hello1";
      return str1;
  }
  
  //fix...
  //&str1与str1命名冲突
  ```

* error: expression result unused

  ``` c++
  int AccountSearch(BankArray bank, char name[100])
  {
      int i = 0;
      for(i ; i < maxAccounts ; i++)
      {
          if (strcmp(bank->list[i]->accountName, name) == 0)
          { 
              return i;
          }
      }
      return -1;
  }
  
  //fix...
  for (; i < maxAccounts ; ++i)
      
  //better fix...
  for (int i = 0 ; i < maxAccounts ; ++i)
  ```

* error C4530: C++ exception handler used, but unwind semantics are not enabled. Specify /EHsc

  [UE4 try catch 打包报错](https://www.cnblogs.com/PiaoLingJiLu/p/9373452.html)

  ``` text
  UE4默认的情况下不允许使用Exception
  //AnswerHub有回答说要在build.cs中设置：
  //过期：UEBuildConfiguration.bForceEnableExceptions = true;
  fix
  bEnableExceptions = true;
  ```

* error: duplicate symbol c++ singleton

  ``` text
  头文件里全局静态变量初始化导致的
  
  例：单例
  .h中 声明了全局的局部变量mInstance并初始化了
  InputEventManager* InputEventManager::mInstance = nullptr;
  
  应该放到.cpp中
  ```

  

* error: struct 'xxx' was previously declared as a class: this is valid, but way result in linker errors under the Microsoft C++ ABI []

  ``` text
  因为加了以下 windows平台宏 导致的
  #if defined(WIN32) || defined(_WIN32) || defined(__WIN32__) || defined(__NT__)
  #else
  	//这个是测试用。。#include "define.h"
  #endif
  
  可能应该用 #ifdef
  ```

   
  
* error : C2665 'TCastImpl<From,To,ECastType::UObjectToUObject>::DoCast': none of the 2 overloads could convert all the argument types	

  [UE4的Cast方法与类型转化](https://blog.csdn.net/zzk1995/article/details/49877469)

  ``` text
  UE4中 Cast<>()  只能用于基于UObject的强制类型转换 这时候需要用到static_cast等转换了
  ```

* error: Shared PCHs are only supported for engine modules

  ``` text
  可以使用 PrivatePCH...
  例：
  PrivatePCHHeaderFile = "Public/Table/TableManager.h";
  //SharedPCHHeaderFile = "Public/Table/TableManager.h";
  ```

* error: unreferenced local variable

  ``` text
  #define UNUSED_VARIANT(x) ((void)(x)) //定义宏通知编译器，知道该局部变量没用
  
  int _tmain(int argc, _TCHAR* argv[])
  {
       int *p;
  
       UNUSED_VARIANT(p);
  
       return 0;
  }
  ```

* error: invalid operands to binary expression ('__bind<int &, const sockaddr *, unsigned int>' and 'int')

  [mac下 jrtplib使用c++11报错](https://blog.csdn.net/wastedsoul/article/details/81670748)

  ``` text
  问题：android ndk编译时 defaultsocketwrapper_gcc.cpp
  
  解决：此时在bind前加 ::
  if (::bind(winSocket, reinterpret_cast<const sockaddr*>(&addr), sizeof(addr)) < 0) {
                  Close(h);
                  BEHAVIAC_LOGERROR("Listen failed at bind\n");
                  return false;
  }
  ```

* 模板类 模板函数 error LNK2019: unresolved external symbol

  error: member access into incomplete type

  ``` text
         出现这个错误error LNK2019: unresolved external symbol的原因在于模板函数的申明与定义在不同文件中，这就导致了错误的出现。当编译器遇到一个模板定义时，它并不生成代码。这就是为什么没有调用这个函数模板时，代码可以运行成功，而调用这个函数模板时，编译出现错误。只有当实例化出模板的特定版本时，编译器才会生成代码。由于这个特性，会影响错误被检测出来。 对于调用普通的函数，编译器只要掌握函数的申明。为了生成一个实例化的版本，编译器要掌握函数模板和模板成员函数的定义。因此，与非模板代码不同，模板的头文件通常要包括申明和定义。
  
         函数模板和类模板成员函数的定义通常放在头文件中。
  ```

  ``` c++
  2	template<typename RangeType>
  	class FllowRangeFilter : public BeRangeFilter
  	{
  		RangeType m_range;
  	public:
  		FllowRangeFilter(const RangeType& range) :m_range(range) {}
  		bool Filter(BeActor* self, const VInt3& pos) const override;
  	};
  
  //将实现方法放到class外部，并且都在header下
  	template<typename RangeType>
  	bool FllowRangeFilter<RangeType>::Filter(BeActor* self, const VInt3& pos) const
  	{
  		return m_range.InRange(pos - GetActorPosition(self));
  	}
  //同时注意 ：GetActorPosition(self) 不能直接调用 self->GetPosition()  !!! 
  //error: member access into incomplete type
  //应该在基类BeFilter.h中声明
      class BeFilter
      {
      public:
          ...
              VInt3 GetActorPosition(BeActor* self) const;
      }
  //并在BeFilter.cpp中定义实现
  	VInt3 BeFilter::GetActorPosition(BeActor* self) const
  	{
  		return self->GetPosition();
  	}
  ```

  

* error: cast from sockaddr * to sockaddr_in * increases required alignment

  [cast from sockaddr * to sockaddr_in * increases required alignment](https://stackoverflow.com/questions/35551879/cast-from-sockaddr-to-sockaddr-in-increases-required-alignment)

  ``` text
  使用reinterpret_cast 代替 const_cast
  
  if (::bind(winSocket, reinterpret_cast<const sockaddr*>(&addr), sizeof(addr)) < 0) {
                  Close(h);
                  BEHAVIAC_LOGERROR("Listen failed at bind\n");
                  return false;
              }
  ```

*  fatal error LNK1181: cannot open input file 'Plugins目录下的插件名.lib'

  ``` text
  在编辑器 编译时
  问题目录：Projects\Plugins\插件名\Intermediate\Build\Win64\UE4Editor\Development\插件名\
  发现插件没有用到迭代生成的 UEEditor-插件名-0001.lib 
  而是报错指向没有找到/无法打开 UEEditor-插件名.lib
  同时也发现没有生成 UEEditor-插件名.lib 只有UEEditor-插件名.lib.response
  
  临时解决方法：
  去对应的VS安装目录下 
  C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Tools\MSVC\14.28.29333\bin\Hostx64\x64
  执行 
  .lib.exe  @项目根目录\Projects\Plugins\插件名\Intermediate\Build\Win64\UE4Editor\Development\插件名\UEEditor-插件名.lib.response
  ```

  

* missing referenced directories

  ``` 
  Build.cs中添加依赖需要注意：
  
  using System.IO;
  
  PublicIncludePaths.Add(Path.Combine(ModuleDirectory, "Public"));
  相当于添加了   [PLUGINNAME]/Public
  ```



* get modified time of Directory

  ``` c++
  //just enable in windows
  //android上获取到为0
  
  //IPlatformFile& platformFile = FPlatformFileManager::Get().GetPlatformFile();
  //uint64 dirTimestamp = platformFile.GetTimeStamp(FilenameOrDirectory).GetTicks();
  //uint64 dirTimestamp = platformFile.GetTimeStamp(FilenameOrDirectory).ToUnixTimestamp();
  
  /*
   FFileManagerGeneric fm;
   FDateTime mod = fm.GetTimeStamp(tchardirectory);
  */
  ```

  

* ue4 缺少“”的预编译清单 PrecompileForTargets = PrecompileTargetsType.Any

  ``` tex
  在主模块的Build.cs中添加依赖：
  PrivateDependencyModuleNames.AddRange(new string[]
  {
       "FileUtilities",
  });
  
  Windows可以编译通过，安卓编译不通过
  
  可能分析1：
  FileUtilities中没有Public以及没有实现 IModuleInterface的模块类
  ```

  



* error C2440: 'return': cannot convert from 'T *' to 'UObject *

  ``` tex
  问题1：
  继承自UObject的类中的委托绑定了普通C++类的方法，运行时绑定奔溃，日志如上
  ```

  ``` c++
  //问题1示例代码（运行时会奔溃）
  //在FOnlineServerListCommon.cpp中添加
  //此时GCloudManager继承自UObject
  auto gcloudMgr = UTMGameInstance::GetInstance()->GetGCloudManager();
  if (gcloudMgr != nullptr)
  {
  	gcloudMgr->InitializeSDK(SDKType::GAME_DIR_SERVER);
  	gcloudMgr->OnGCloudDirPullComplete().AddRaw(this, &FOnlineServerListCommon::_GCloudDirPullComplete, inCompletionDelegate);
  }
  ```

  

* error C2338 You cannot use UObject method delegates with raw 

  ``` tex
  问题1：
  DelegateInstanceImpl.h
  
  static_assert(UE4Delegates_Private::IsUObjectPtr((UserClass*)nullptr), "You cannot use UObject method delegates with raw pointers.");
  ```

  ``` c++
  //示例代码
  //GameVersion.cpp  raw c++
  void AGameVersion::Init(AGCloudTestGameModeBase* parentObj, void(AGCloudTestGameModeBase::*callback)(void))
  {
  	auto gcloudMgr = PluginsManager::GetInstance()->GetGCloudManager();
  	if (gcloudMgr)
  	{
  		m_updateFinishHandle = gcloudMgr->OnGCloudUpdateFinish().AddUObject(parentObj, callback);
  	}
  }
  
  //用工程默认生成的继承自AGameModeBase的类，不能将它的指针绑定给非UObject类/对象
  //GCloudTestGameModeBase.cpp  :  public AGameModeBase  	 ue4 c++
  void AGCloudTestGameModeBase::BeginPlay()
  {
  	m_gameVersion->SetCurrWorld(GetWorld());
      //编译不通过，无法将一个持久的UObject对象绑定到
  	m_gameVersion->Init(this, &AGCloudTestGameModeBase::_afterGameUpdate);
      
      //只能用这种方式绑定，但是会影响逻辑
  	auto gcloudMgr = PluginsManager::GetInstance()->GetGCloudManager();
  	if (gcloudMgr)
  	{
  		gcloudMgr->OnGCloudUpdateFinish().AddUObject(this, &AGCloudTestGameModeBase::_afterGameUpdate);
  	}
  }
  
  //解决：
  //创建继承自AGameMode的类，可以编译通过 ！
  //GCloudGameMode.cpp  : public AGameMode    ue4 c++
  void AGCloudGameMode::StartPlay()
  {
  	m_gameVersion->SetCurrWorld(GetWorld());
  	m_gameVersion->Init(this, &AGCloudGameMode::_afterGameUpdate);
  }
  ```

  

* Error: Unhandled Exception: EXCEPTION_ACCESS_VIOLATION 0xf95f1160 （每次奔溃地址不同...）

  ``` tex
  问题：运行并停止时，由于在GameMode / GameModeBase / GameInstance 的 生命周期函数中
  如 BeginPlay()  EndPlay(...)  StartPlay()  Init() Shutdown()  中delete 自定义对象，
  但是没有添加生命周期函数对应的Super::EndPlay() Super::Shutdown()等，导致执行自定义对象析构时奔溃
  ```

  ``` c++
  //示例代码一： 异常情况
  
  //UGCloudGameInstance.cpp  ： public UGameInstance
  void UUGCloudGameInstance::Shutdown()
  {
  	UE_LOG(LogTemp, Warning, TEXT("### gcloud game ins shutdown 1"));
  	PluginsManager::GetInstance()->UninitPlugins();
  	UE_LOG(LogTemp, Warning, TEXT("### gcloud game ins shutdown 2"));
  }
  
  //PluginsManager.cpp
  void PluginsManager::UninitPlugins()
  {
  	UE_LOG(LogTemp, Log, TEXT("### plugin mgr uninit start"));
  	SAFE_DELETE_PTR(mGCloudManager);
  	UE_LOG(LogTemp, Log, TEXT("### plugin mgr uninit end"));
  }
  
  //FGCloudManager.cpp 
  FGCloudManager::~FGCloudManager()
  {
  	UE_LOG(LogTemp, Log, TEXT("### gcloud mgr dtor start"));
  	Uninit();
  	UE_LOG(LogTemp, Log, TEXT("### gcloud mgr dtor end"));   //-----> crash
  }
  ```

  ``` c++
  //示例代码二： 正常情况1
  
  void UUGCloudGameInstance::Init()
  {
  	UE_LOG(LogTemp, Warning, TEXT("### gcloud game ins init 1"));
  	testGCloudMgr = new FGCloudManager();
  	UE_LOG(LogTemp, Warning, TEXT("### gcloud game ins init 2"));
  }
  
  //UGCloudGameInstance.cpp  ： public UGameInstance
  void UUGCloudGameInstance::Shutdown()
  {
  	UE_LOG(LogTemp, Warning, TEXT("### gcloud game ins shutdown 1"));
  	SAFE_DELETE_PTR(testGCloudMgr);
  	UE_LOG(LogTemp, Warning, TEXT("### gcloud game ins shutdown 2"));
  }
  ```

  ``` c++
  //示例代码三： 正常情况2   解决方法
  
  void UUGCloudGameInstance::Init()
  {
      //添加
  	Super::Init();
  
  	UE_LOG(LogTemp, Warning, TEXT("### gcloud game ins init 1"));
  	PluginsManager::GetInstance()->InitPlugins();
  	UE_LOG(LogTemp, Warning, TEXT("### gcloud game ins init 2"));
  }
  
  void UUGCloudGameInstance::Shutdown()
  {
  	UE_LOG(LogTemp, Warning, TEXT("### gcloud game ins shutdown 1"));
  	PluginsManager::GetInstance()->UninitPlugins();
  	UE_LOG(LogTemp, Warning, TEXT("### gcloud game ins shutdown 2"));
  
      //添加
  	Super::Shutdown();
  }
  ```



* 网上野指针问题：UObject被自动GC

  ``` tex
  问题1：
  Plugin—1 中
  static UMyObject* m_obj = nullptr;
  Plugin-1 蓝图中
  m_obj = NewObject<UMyObject>();
  
  运行中，该对象被析构，导致野指针，奔溃
  
  建议：不要将UObject对象绑定为static, 不要去限制UObject的生命周期
  ```

  ``` c++
  //一个不推荐方法：将UObject对象设置为不自动析构
  //C++类
  //ctor
  UObject* m_obj = NewObject<UMyObject>();
  m_obj->AddToRoot();
  
  //dtor
  m_obj->RemoveFromRoot();
  
  //如果继承UObject类中有一个UObject* A变量，可以用宏UPROPERTY标记变量，可以让GC来追踪UObject的引用数
  ```

  

  

* pure virtual function being called while application was running (gisrunning == 1) error

  ``` tex
  @注意：Windows系统级弹窗，暂未定位
  ```






* error: 缺少类型说明符 - 假定为 int。注意: C++ 不支持默认 int

  ``` tex
  头文件彼此包含了！！！
  或者头文件多次嵌套，出现不直观的自引用了！！！
  ```

  
