# C++踩坑

### 野指针

* 野指针1

  ``` tex
  场景：
  UE4中，第一次运行正常，停止后再运行，发生奔溃，奔溃日志（由于之后知道是野指针，导致多次获取日志位置可能不统一）
  tips: UE4 Play后Stop时，不会触发创建C++原生类（不继承UObject）的析构，可以在MyGameInstance中的Shutdown中统一处理
  ```

  ``` c++
  //示例代码
  //GCloudManager.cpp
  if (m_sdkInstance != nullptr && m_gsdkInfo != nullptr)
  {
  	UE_LOG(LogTemp, Log, TEXT("### FGCloudManager init sdk 1"));
  	m_sdkInstance->InitSDK(sdkType, *m_gsdkInfo, TM_isDebug);
  	UE_LOG(LogTemp, Log, TEXT("### FGCloudManager init sdk 2"));
  }
  //上述代码都判空了，但还是闪退在这里，因为m_sdkInstance在第一次运行关闭后，被这个管理器析构了，析构用的是delete instance
  //解决如下：需要对应写CreateInstance的DestroyInstance，并且不要忘了 在对象池中清理那些指向已经析构了的对象的指针
  
  //SDKInstanceFactory.cpp
  SDKInstance* SDKInstanceFactory::CreateSDKInstance(const std::string& name)
  {
  	SDKInstance* ins = nullptr;
  	if (m_sdkInstanceMap.find(name) != m_sdkInstanceMap.end())
  	{
  		ins =  m_sdkInstanceMap[name];
  		UE_LOG(LogTemp, Log, TEXT("### sdk factory get sdk ins: %s"), UTF8_TO_TCHAR(name.c_str()));
  	}
  	else if(std::strcmp(name.c_str(), GCloudSDKInstance::SDK_NAME.c_str()) == 0)
  	{
  		ins = new GCloudSDKInstance();
  		m_sdkInstanceMap.emplace(name, ins);
  		UE_LOG(LogTemp, Log, TEXT("### sdk factory create new sdk ins: %s"), UTF8_TO_TCHAR(name.c_str()));
  	}
  	return ins;
  }
  
  SDKInstanceFactory& SDKInstanceFactory::GetInstance()
  {
  	static SDKInstanceFactory instance;
  	return instance;
  }
  
  SDKInstanceFactory::~SDKInstanceFactory()
  {
  	m_sdkInstanceMap.clear();
  	UE_LOG(LogTemp, Log, TEXT("### sdk ins factory dtor"));
  }
  
  //问题发生时没有对应写Init 和 Uninit
  void SDKInstanceFactory::DestroySDKInstance(SDKInstance* ins)
  {
      //清理池子，否则可能出现野指针，被池子引用，但是指针指向的对象已经被delete
  	if (ins != nullptr)
  	{
  		if (m_sdkInstanceMap.find(ins->GetName()) != m_sdkInstanceMap.end())
  		{
  			m_sdkInstanceMap.erase(ins->GetName());
  		}
  	}
      //delete
  	SAFE_DELETE_PTR(ins);
  }
  ```

* 





---



### 内存泄露

* 常见场景

  [内存泄漏2——C++中常见内存泄漏情形总结](https://blog.csdn.net/haimianjie2012/article/details/56496047)

  ``` tex
  1.类成员变量动态分配内存
  	类所有动态分配的成员变量，一定记得在析构函数中全部进行判断释放内存。当类中动态分配内存的成员一般是指针成员变量。
  	
  2.指针容器
  	使用std::vector<CType*>时，记得在clear或是删除一个元素之前，应该释放指针指向的内容。
  	若是简单结构、简单类，你直接用std::vector<CType>可以避免内存泄漏错误。
  	
  3.指针赋值
  	如果不是在定义指针作用范围内，使用其他地方的指针（如全局指针，类成员指针变量）赋值时，首先判断该指针是否为NULL,为NULL时new一块内存，
  	否则，考虑重用原来的内存或先删除后new内存。因为若指针原来有值的话，你一覆盖原来分配的内存就再也找不到了，也就产生了泄漏。
  	
  4.扫尾函数
  	有些类型对象如CDialog，CWindow，CFile，CImage等需要在Delete前做Close、Release、Destroy等操作的，
  	Delete时检查是否已经调用了相应的扫尾函数。 
  	
  5.公共模块/SDK
  	公共模块一般有init()、open()和release()、terminate()、close()两种类型的函数，不要忘记扫尾类型函数的调用。 
  	
  6.异常分支
  	若正常分支有内存需要释放，则不要忘了异常分支的内存释放如try语句的catch分支，函数中的多个return分支都要考虑到相应内存的释放。 
  	
  7.动态分配对象数组
  	动态分配的对象数组，记得使用delete[]来进行删除。基于两个考虑：
  		(1)可以释放整个数组的空间； 	  --->由于动态分配的内存是连续的，使用delete也能将这块内存释放掉
  		(2)调用数组中每个对象的析构函数。  --->关键点，需要调用delete[]调用每个数组元素的析构
  	
  8.非常规动态内存分配
  	不是采用常规内存分配(new、malloc、calloc、realloc)的内存也要记得释放，如strdup等。 
  	有一些C/C++ Api返回的指针是动态分配的需要使用者来负责释放，这个只要使用时看清楚Api的说明就不会有什么问题了。 
  	
  9.单例模式
  	最好在程序退出时释放内存，虽然OS会回收，但对于我们以后内存泄漏检测工作能带来极大方便。 
  	虽然单态模式的内存泄漏是一次性泄漏，不会导致内存的不断增加，
  	但因为很多内存泄漏检查工具都是程序正常结束后开始统计内存泄漏的，此时会将单态模式的内存泄漏也统计进去。
  	这样我们就得一个个区分那个是单态泄漏那个是非法泄漏，会带来很大的工作量，若能在程序退出时将单态模式的内存泄漏也释放掉，
  	检测结果就会集中在有问题的内存泄漏上了，大大减少我们的工作量。
  	
  	解决方法： 为单态模式对象定义DestroyInstance()方法用来释放单态模式的内存，在程序退出时调用该函数。  
  				或是采用static的 smart 指针来让编译器自动在程序退出时负责释放相应的内存。
  				
  10.虚析构函数
  	一个类的指针被向上引用，作为基类的指针来使用的时候，把析构函数写成虚函数。
  	这样做是为了当用一个基类的指针类型来删除一个派生类的对象时，派生类的析构函数会被调用。
  	(new子类的对象，删除时却采用delete父类类型的指针。
  	new CConcreteClass的对象ptr，但delete CClass类型 的指针ptr，无法调用正确的析构函数
  	对于编译器而言，如果基类析构不使用虚析构，认为只是删除基类类型指针，而不会去调用子类成员变量的析构函数) 
  	
  	当针对接口进行编程时，涉及到动态分配的对象指针在各函数间传递时特别要注意将基类的析构函数定义成虚函数。
  	
  11.线程安全退出
  	线程的安全退出，user-interface thread安全退出 和窗口关联的user-interface thread 必须处理WM_DESTROY消息，
  	建议定义一个OnDestroy()函数，该函数调用PostQuitMessage(0)的方法让user-interface thread安全退出，防止线程不安全退出导致内存泄漏。 
  	
  12.内存动态分配后，在各个分支路径均要考虑是否需要释放掉
  
  ```

  

  ``` c++
  //虚析构函数的使用
  //以下为反例
  struct ST_Info 
  { 
     int iWeight; 
     char strName[128] 
  } 
  class CFruit { }; 
  class CApple:public CFruit { 
  public:  std::vector< ST_Info> m_vecInfo; 
  {
      CFruit * GetApple() { 
      CApple *ptrApple = new CApple();
      ST_Info st_Info = {9, “Apple1”}; 
      ptrApple->m_vecInfo.push_back(st_Info); 
      return ptrApple;  
  } 
  void main(int argc, char**argv) 
  { 
   	CFruit *ptrFruit = GetApple();   
  	delete ptrFruit;                       //此时不会释放调用CApple中的m_vecInfo ！！！
  	ptrFruit = NULL; 
  }
  ```

  ``` c++
  //线程进行安全退出，防止非正常退出的内存泄漏问题。
  LRESULT CMsgReflect::OnDestroy(HWND hWindow, UINT uiMessage, WPARAM uiParam, LPARAM ulParam) 
  {  
      PostQuitMessage(0);  
      return 0; 
  }
  ```

  ``` c++
  //内存动态分配后，在各个分支路径均要考虑是否需要释放掉
  {
  	for (std::vector<TeamInfo>::iterator it = e.teamlist.begin(); it != e.teamlist.end(); it++)
  	{    
     		FriendGroupData *pGroup=new FriendGroupData;    
     		if(it->unTeamID==DEFAULT_FRIEND_GROUP_ID)   
       		continue;
  	   	delete pGroup; 
  	}
  }
  ```

  

  