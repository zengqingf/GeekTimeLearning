# UE4 C++



### UE4 C++基础

* 文件名.generated.h

  ``` tex
  #include "文件名.generated.h" 文件会在UE4编译时生成；注意文件名不是类名
  目录：<UE4Project>/Intermediate/Build/Win64/UnrealEditor/Inc/<UE4Project>
  ```

* class 项目模块宏 类名

  ``` tex
  class 项目模块宏 类名: public AActor
  UE4中项目模块宏格式   项目名全大写_API
  ```

* 类相关的宏

  ``` c++
  UCLASS()									//表明属于UE的类
  class AMyActor : public AActor
  {
  	GENERATED_BODY()						//在编译时会生成多行UE代码模板
  }
  ```

  ``` c++
  /*
  宏：UPROPERTY() 
  用于修饰成员变量，只能用于UE的Class中，即UCLASS()宏修饰的类
  和普通C++变量的区别：生命周期和对蓝图的访问权限不同，蓝图可以访问UPROPERTY()的属性
  
  宏参数：
  BlueprintReadWrite：蓝图可读可写，类似于GetXXX() / SetXXX()
  EditAnywhere：在任意地方可以编辑该属性，如Level中的Actor，继承自该类的子类蓝图等；但是只添加这个参数，蓝图等还是无法访问
  Category = "XXX" ：修改蓝图中和Details（即property windows）中的当前属性分类组
  
  	参数定义位置：EpicGames/UEXXX/Engine/Source/Runtime/CoreUObject/Public/UObject/ObjectMacros.h
  	参数添加方式：UPROPERTY(EditAnywhere, BlueprintReadWrite) 用逗号分割
  	
  访问修饰：
  UPROPERTY()可以由public protected private修饰
  当private修饰时，不能同时使用BlueprintReadWrite（会编译错误）
  */
  ```

  ``` c++
  /*
  UFUNCTION()
  用于修饰函数（成员或静态），只能用于UE的Class中，即UCLASS()宏修饰的类
  只有UFUNCTION修饰的函数才能被蓝图访问（调用，重写，实现）
  
  宏参数：
  BlueprintCallable：蓝图可以调用该函数
  					《《同时注意不受代码中public, protected，private限制，继承C++蓝图中以及关卡蓝图中都可以访问成员函数》》
  BlueprintImplementableEvent:表示蓝图重写该函数但C++不能提供实现
  					由蓝图继承类实现具体逻辑（蓝图中会添加 Event 前缀）
  					《《不能使用private访问权限； C++中不能提供实现，会编译报Link错》》
  BlueprintNativeEvent:表示蓝图可以重写该函数并且C++可以提供默认实现
  					蓝图也可以调用父类方法，调用默认实现
  					《《C++默认实现，即在C++中定义被修饰方法名加上 _Implementation 的方法, 调用该方法时同时也会调用该默认实现，
  					蓝图中也可以重写该默认实现（蓝图中会添加 Event 前缀），当子类蓝图重写后，C++中的默认实现不会生效，
  					需要子类蓝图和默认实现都能调用，子类蓝图中需要调用Parent:被修饰方法名》》
  */
  ```

  ``` c++
  /*
  UPROPERTY
      VisibleAnywhere:	Can be seen in details panel anywhere 	
      					实例和默认下均可显示; 如果修饰的是数值、布尔值，效果是：只显示并且变量是灰色，无法进行编辑
      EditInstanceOnly:	Can only be edited in Editor details panel
      					变量在实例下可编辑，默认下不可编辑
      VisibleInstanceOnly	变量在物体实例下只读，不可编辑
      					
      EditDefaultsOnly:	Can only be edited in Blueprint details panel
      					默认下可编辑，实例下不显示
      VisibleDefaultsOnly 在默认下显示并且可读不可编辑
      					
      BlueprintReadOnly:	Blueprint can only “get”
      					蓝图中只能获取变量，不能设置
      BlueprintReadWrite:	Blueprint can “get” and “set”
      					在蓝图的事件图表中，能过获取、设置此变量信息，是因为BlueprintReadWrit关键词与蓝图的访问相关。
      EditAnywhere:		Can be edited in details panel anywhere
      					在实例、默认中可编辑
  */
  
  //e.g. c++创建静态网格组件
  UPROPERTY(VisibleAnyWhere, Category = "ActorMeshComponents")
  UStaticMeshComponent* StaticMesh;
  
  AFlobter::AFloater()
  {
      PrimaryActorTick.bCanEverTick = true;
      
      /*
      1、此函数只能在无参构造器中使用，不能在BeginPlay等函数中使用。
  	2、参数中的TEXT或者FName参数在同一个Actor中不能重复。
      */
      StaticMesh = CreateDefaultSuboject<UStaticMeshComponent>(TEXT("CustomStaticMesh"));
  }
  
  
  //e.g. 注意：在蓝图的事件图表中，能过获取、设置此变量信息，是因为BlueprintReadWrit关键词与蓝图的访问相关。
  UPROPERTY(EditInstanceOnly, BlueprintReadWrite, Category = "FloaterVectors")
  FVector InitialLocation = FVector(0.0f);
  ```

  







* 基础include文件

  ``` c++
  #include "CoreMinimal.h" //包含了UE C++中需要的头文件，如FString等
  ```





---



* sprintf_s vs snprintf

  sprintf 用于将输出存到字符缓冲中

  ``` c++
  //函数原型：sprintf(char *buffer, const char *format, [argument]);
  
  int a=1,b=2;
  char s[10];
  sprintf(s,"a=%d,b=%d",1,2);
  puts(s);
  ```

  [sprintf_s的使用](https://www.cnblogs.com/dirt2/p/6104198.html)

  ``` text
  windows平台下线程安全的格式化字符串函数sprint_s并非标准C函数，因此linux下无法使用，但可以使用snprintf函数代替。
  
  *函数原型：*/
  int snprintf(char *dest, size_t n, const char *fmt, ...);
  
  /*函数说明: 最多从源串中拷贝n－1个字符到目标串中，然后再在后面加一个0。所以如果目标串的大小为n的话，将不会溢出。
  函数返回值: 若成功则返回存入数组的字符数，若编码出错则返回负值。
  推荐的用法：*/
  void f(const char *p)
  {
      char buf[11]={0};
      snprintf(buf, sizeof(buf), "%10s", p); // 注意：这里第2个参数应当用sizeof(str)，而不要使用硬编码11，也不应当使用sizeof(str)-1或10
      printf("%sn",buf);
  }
  ```

* vsprintf vs sprintf

  [C语言printf()、sprintf()、vsprintf() 的区别与联系](https://blog.csdn.net/Raito__/article/details/48860119)

  ``` c++
  //需要引入相关头文件 #include <stdarg.h>
  //函数原型： vsprintf(char *buffer, char *format, va_list param);
  
  //可以用 vsprintf() 来实现 sprintf()
  
  void Myprintf(const char* fmt,...);
   
  int a=1,b=2;
  char s[10];
  Myprintf("a=%d,b=%d",a,b);
   
  void Myprintf(const char* fmt,...)
  {
    char s[10];
    va_start(ap, fmt);	
    vsprintf(s,fmt,ap);
    va_end(ap);	
    puts(s);
  }
  
  
  //新需求：功能是将格式化字符串输出两遍
  void Myprintf(const char* fmt,...)
  {
    char s[10];
    sprintf(s,fmt);
    puts(s);
    puts(s);
  }
  //传入的其实是 sprintf(s,"a=%d,b=%d") 而不是 sprintf(s,"a=%d,b=%d",a,b)
  //类似这种封装用 sprintf() 是无法实现的，使用 sprintf() 只能原始的为它输入所有的参数而不能以传参的方式给它
  
  //修改为vsprintf
  void Myprintf(const char* fmt,...)
  {
    char s[10];
    va_list ap;
    va_start(ap,fmt);
    vsprintf(s,fmt,ap);
    va_end(ap);
    puts(s);
    puts(s);
  }
  //调用：Myprintf("a=%d,b=%d",a,b);
  //输出: 
  	//a=1,b=2
  	//a=1,b=2
  
  
  //vsprintf原理：
  /*
  执行函数时，函数参数是倒序压入栈中（栈，先进后出）
  
  vsprintf() 为了能够解析你传给它的多个参数，你必须告诉它参数从哪里开始。
  vadefs.h 头文件中这么定义 ：typedef char * va_list，于是我们定义了一个 va_list ap 来保存参数起始地址。
  va_start(ap,fmt) 就找出这个函数在栈中排列的一堆参数的起始地址，然后直接浏览栈中参数，并用 vsprintf() 实现格式化字符串的读取，最后 vs_end(ap) 释放ap，就像释放指针一样。通俗地说就是因为 vsprintf() 比 sprintf() 更加接近底层(栈)，因此能实现这个目的，也是因此能用 vsprintf() 来实现 sprintf()。
  */
  ```




* %02x vs. %2x  （可结合上面的sprintf使用）

  ``` tex
  输出最小宽度
  用十进制整数来表示输出的最少位数。若实际位数多于定义的宽度，则按实际位数输出，若实际位数少于定义的宽度则补以空格或0(当最小宽度数值以0开头时)。
  
  X 表示以十六进制形式输出
  02 表示不足两位，前面补0输出；如果超过两位，则实际输出
  举例：
  printf("%02X", 0x345);  //打印出：345
  printf("%02X", 0x6); //打印出：06
  
  而如果直接写为 %2x，数据不足两位时，实际输出，即不额外补0输出； 如果超过两位，则实际输出。
  printf("%2X", 0x345);  //打印出：345
  printf("%2X", 0x6); //打印出：6
  ```




* FString to const char* with android ndk 29

  ``` c++
  	FString test = TEXT("1.0.1.1002");
  	FString test1("1.0.1.1002");
  	FString test2("1.0.1.1002");
  
  	std::string vStr = TCHAR_TO_UTF8(*versionNameStr);
  	const char* versionName1 = vStr.c_str();
  
  	std::string vStr1 = TCHAR_TO_ANSI(*versionNameStr);
  	const char* versionName2 = vStr1.c_str();
  
  	const char* testStr = TCHAR_TO_UTF8(*test);
  	const char* testStr1 = TCHAR_TO_UTF8(*test1);
  	const char* testStr2 = TCHAR_TO_ANSI(*test2);
  
  	UE_LOG(LogTemp, Log, TEXT("### version name: %s --- %s"), UTF8_TO_TCHAR(versionName1), *versionNameStr);
  	UE_LOG(LogTemp, Log, TEXT("### version name: %s --- %s"), ANSI_TO_TCHAR(versionName2), *versionNameStr);
  
  	UE_LOG(LogTemp, Log, TEXT("### version name: %s --- %s"), UTF8_TO_TCHAR(testStr), *test);
  	UE_LOG(LogTemp, Log, TEXT("### version name: %s --- %s"), UTF8_TO_TCHAR(testStr1), *test1);
  	UE_LOG(LogTemp, Log, TEXT("### version name: %s --- %s"), ANSI_TO_TCHAR(testStr2), *test2);
  
  /*android上输出：
  LogTemp: ### version name: 1.0.1.1002 --- 1.0.1.1002
  LogTemp: ### version name: 1.0.1.1002 --- 1.0.1.1002
  LogTemp: ### version name: 1 --- 1.0.1.1002
  LogTemp: ### version name: 1 --- 1.0.1.1002
  LogTemp: ### version name: 1 --- 1.0.1.1002
  
  windows上输出：
  LogTemp: ### version name: 1.0.1.1002 --- 1.0.1.1002
  LogTemp: ### version name: 1.0.1.1002 --- 1.0.1.1002
  LogTemp: ### version name: 1.0.1.1002 --- 1.0.1.1002
  LogTemp: ### version name: 1.0.1.1002 --- 1.0.1.1002
  LogTemp: ### version name: 1.0.1.1002 --- 1.0.1.1002
  */
  ```



* FString to const char* with android ndk 29  **@注意：问题修复**

  ``` c++
  //问题环境：
  //PluginManager.cpp
  FString versionName;
  FString PluginsManager::GetVersionName()
  {
      versionName = "111";
      return versionName;
  }
  
  //FGCloudManager.cpp
  {
  	FString versionNameStr = PluginsManager::GetInstance()->GetVersionName();   //返回的是 （FString）versionName的拷贝
      																			//versionNameStr为临时变量了！！！
      const char* versionName = TCHAR_TO_UTF8(*versionNameStr);
      m_gsdkInfo.AppVersion = versionName;										
  }//离开作用域时 versionNameStr为空，  char* 指向空
  {
      UE_LOG(LogTemp, Log, TEXT("%s"). UTF8_TO_TCHAR(m_gsdkInfo.AppVersion));   //输出为空！！！
  }
  
  
  //修复： 返回单例中的引用，而不是拷贝！！！
  //PluginManager.cpp
  FString versionName;
  const FString& PluginsManager::GetVersionName()
  {
      versionName = "111";
      return versionName;
  }
  ```

  ``` c++
  //注意：像转换函数（函数内返回的内容根据传入参数会改变）返回带指针的对象时，需要考虑返回拷贝而不是引用
  //因为可能多个地方使用，尤其注意，不要在这样的函数中使用static，会出现多个地方使用时，数据被改动
  
  FString HotfixState2Desc(TM_HotfixState state)
  {
      FString desc;
      if(state==1)
      {
          desc = "1";
      }else if(state == 2)
      {
          desc = "2";
      }
      return desc;
  }
  ```

  




* float to FString 四舍五入（保留小数）

  ``` c++
  float num = 0.85469;
  //保留两位小数，注意除以 100.0 为了隐式转换成 float
  FString numStr = FString::SanitizeFloat((int)(num * 100 + 0.5) / 100.0);
  
  num = 0.85549; //numStr 0.855
  num = 0.85559; //numStr 0.856
  //保留三位
  FString numStr = FString::SanitizeFloat((int)(num * 1000 + 0.5) / 1000.0);
  ```

  

---



* UE4 设置宏

  ``` c#
  //错误做法：UE4的VS工程，Games/项目 - 属性 - 预处理器定义
  //正确做法：
  
  //1. 项目模块编译文件 xxx.build.cs
  public class MyTest : ModuleRules
  {
      public MyTest(TargetInfo Target)
      {
          PublicDependencyModuleNames.AddRange(new string[] { "Core", "CoreUObject", "Engine", "InputCore" });
  
          Definitions.Add("HELLO_WORLD"); //添加 自定义的宏 或者 引擎的宏
      }
  }
  //2. 重新生成vs工程
  //3. 观察宏里面的代码是否高亮
  //4. build vs or compile ue4
  ```

  

* UE4 #if vs #ifdef

  ``` text
  #if 	判断这个宏 是否是true
  #ifdef  判断这个宏 是否定义（不一定为true）
  ```

  

* UE4 WITH_EDITOR vs WITH_EDITORONLY_DATA

  ``` text
  WITH_EDITOR used for methods(except cases when that methods needs some strictly related fields)
  WITH_EDITORONLY_DATA used for fields(except cases when that fields needs some strictly related methods, for example Getter/Setter)
  
  Whether to compile the editor or not. Only desktop platforms (Windows or Mac) will use this, other platforms force this to false.
    Whether to compile WITH_EDITORONLY_DATA disabled. Only Windows will use this, other platforms force this to false.
  
    WITH_EDITORONLY_DATA in headers for wrapping reflected members.
    WITH_EDITOR in CPP files for code.. Has nothing to do with reflection.
  
    # UPROPERTY和UFUNCTION包裹的成员变量，在头文件中应该使用WITH_EDITORONLY_DATA
    # WITH_EDITOR一般只是在CPP中使用的
  ```



* UE4 C++ with Java

  ``` java
  public class Base<T>
  {
      public void doSomething()
      {
          nativeDoSomething();
      }
  
      private native void nativeDoSomething();
  }
  ```

  ``` c++
  //泛型类的jni方法签名
  JNIEXPORT void JNICALL Java_Base_nativeDoSomething
     (JNIEnv *, jobject);
  ```





* UE4 隐藏Actor

  ``` c++
  MyActor->bHidden = true;
  MyActor->GetMesh()->SetVisibility(false);
  MyActor->SetActorHiddenInGame(true);
  ```




* UE4 json

  使用参考见UE4 JsonTests.cpp （UE 4.25.4）

  [虚幻4之JSON学习](https://sanctorum003.github.io/2019/08/07/CG/UE4/JSON/)

  ``` c++
  //例子：
  
  //build.cs 添加依赖  Json  (自定义构建Json)         JsonUtilities (封装的一些接口)
  //Json.h  JsonUtilities.h
  
  /** Data.json **/
  [
      {
          "1-1":"1-1",
          "1-2":"1-2"
      },
      {
          "2-1":"2-1"
      },
      {
          "3-1":
          [
              {
                  "1":"3-1-1"
              },
              {
                  "2":"3-1-2"
              }
          ]
      }
  ]
  
  //FPaths::GameContentDir() 能获取 */Content/ 的位置
  //FFileHelper::LoadFileToString() Load a text file to an FString.
  bool MyJsonHandle::LoadStringFromFile(FString & RelativePathName, FString & FileName, FString & ResultString)
  {
      if (!FileName.IsEmpty())
      {
          FString AbsloutePathName = FPaths::GameContentDir() + RelativePathName + FileName;
          if (FPaths::FileExists(AbsloutePathName))
          {
              if (FFileHelper::LoadFileToString(ResultString, *AbsloutePathName))
              {
                  return true;
              }
          }
      }
      return false;
  }
  
  /** MyJsonHandle.cpp **/
  bool MyJsonHandle::RecordDataJsonRead()
  {
      FString result;
      //这是我们上面自定义的函数
      LoadStringFromFile(RelativePathName, DataFileName, result);
  
      TArray<TSharedPtr<FJsonValue>> JsonParse;
      TSharedRef<TJsonReader<TCHAR>> JsonReader = TJsonReaderFactory<TCHAR>::Create(result);
  
      if (FJsonSerializer::Deserialize(JsonReader, JsonParse))
      { //这样解析后的节点全部存在JsonParse中
          UE_LOG(LogTemp, Warning, TEXT("%s"), *JsonParse[0]->AsObject()->GetStringField(FString("1-1")));
          UE_LOG(LogTemp, Warning, TEXT("%s"), *JsonParse[0]->AsObject()->GetStringField(FString("1-2")));
          UE_LOG(LogTemp, Warning, TEXT("%s"), *JsonParse[1]->AsObject()->GetStringField(FString("2-1")));
          //如果对象是嵌套的，则需要再声明一次TArray<TSharedPtr<FJsonValue>>来获取
          TArray<TSharedPtr<FJsonValue>> JsonParse3 = JsonParse[2]->AsObject()->GetArrayField(FString("3-1"));
          if (JsonParse3.IsValidIndex(0))
          {
              for (int cnt = 1; cnt <= JsonParse3.Num(); ++cnt)
              {
             UE_LOG(LogTemp, Warning, TEXT("%s"), *JsonParse3[cnt-1]->AsObject()->GetStringField(FString::FromInt(cnt)));
              }
          }
          else
          {
              UE_LOG(LogTemp, Warning, TEXT("error JsonParse3"));
          }
          return true;
      }
      return false;
  }
  
  //修改json
  TSharedPtr<FJsonObject> Object1 = MakeShareable(new FJsonObject);
  Object1->SetStringField("1-1", "1--1");
  Object1->SetStringField("1-2", "1--2");
  TSharedPtr<FJsonValueObject> CultureValue = MakeShareable(new FJsonValueObject(CultureObject));
  ```

  [UE4 序列化,反序列化,读写 Json](https://blog.csdn.net/qq_35760525/article/details/77531286)

  ``` c++
  //注意：
  /*
  TSharedPtr<FJsonObject> t_ptr;    需要先判断共享指针是否为空，再判断里面是否有值
  
  if (_ptrJsonObj.IsValid() && _ptrJsonObj->Values.Num()>0)
  */
  
  //创建Json Object
  TSharedPtr<FJsonObject> t_jsonObject = MakeShareable(new FJsonObject);
  
  //设置键值对 三种基本类型：bool  string   number
  t_jsonObject->SetBoolField("userBool",true)
  t_jsonObject->SetStringField("userName",_data.m_userName);
  t_jsonObject->SetNumberField("userId", _data.m_userIndex);
  //嵌套单个json object
  TSharedPtr<FJsonObject> t_insideObj = MakeShareable(new FJsonObject);
  t_jsonObject->SetObjectField("insideObj",t_insideObj );
  //嵌套多个object 数组
  TArray<TSharedPtr<FJsonValue>> t_objects;
  TSharedPtr<FJsonValueObject>t_objValue=MakeShareable(newFJsonValueObject(t_insideObj));
  /*
  注意：
  		TSharedPtr<FJsonObject> aObjPtr = arg->GetJsonObj();
  
  		//错误！
  		FJsonValueObject aObjValue(aObjPtr);
  		TSharedPtr<FJsonValue> aObjValuePtr = MakeShareable(&aObjValue);
  		//正确做法
  		TSharedPtr<FJsonValueObject> aObjValuePtr = MakeShareable(new FJsonValueObject(aObjPtr));
  */
  t_objects.Add(t_objValue);
  // json value 类型可以不同
  TArray<TSharedPtr<FJsonValue>> _res;
  for (int i = 0; i < t_datasNum; ++i)
  {
      TSharedPtr<FJsonValueNumber> t_value = MakeShareable(new FJsonValueNumber(_datas[i]));
      _res.Add(t_value);
  }
  
  //FJsonObject to FString
  bool GetFStringInJsonFormat(const TSharedPtr<FJsonObject> &_ptrJsonObj, FString &_strGet)
  {
  	if (_ptrJsonObj.IsValid()&&_ptrJsonObj->Values.Num()>0)
  	{		
  		TSharedRef<TJsonWriter<TCHAR>> t_writer = TJsonWriterFactory<>::Create(&_strGet);
  		FJsonSerializer::Serialize(_ptrJsonObj.ToSharedRef(), t_writer);
  		return true;
  	}
  	return false;
  }
  
  //FString to FJsonObject
  bool GetJsonObjectFromJsonFString(const FString &_jsonFString, TSharedPtr<FJsonObject> &_jsonObject)
  {
  	if (!_jsonFString.IsEmpty())
  	{
  		TSharedRef<TJsonReader<>> t_reader = TJsonReaderFactory<>::Create(_jsonFString);
  		if (FJsonSerializer::Deserialize(t_reader,_jsonObject))
  		{
  UE_LOG(LogTemp,Warning,TEXT("GetJsonObjectFromJsonFString---Read JsonObject from Json FString "));
  			return true;
  		}
  	}
  	return false;
  }
  
  //Object to JsonObject
  TSharedPtr<FJsonObject> :SerializeDataToJson(TMap<int, FData> &_data)
  {
  	TSharedPtr<FJsonObject> t_jsonObject = MakeShareable(new FJsonObject);
  	if (_data.Num()>0)
  	{
  		TArray<TSharedPtr<FJsonValue>> t_objects;
  		for (auto it:_data)
  		{
  			FWallPointStoredData t_data = it.Value;
  			if (t_data.IsValid())
  			{
  				TSharedPtr<FJsonObject> t_obj = MakeShareable(new FJsonObject);
  				t_obj->SetNumberField("id", t_data.m_id);								
  				TSharedPtr<FJsonValue> t_value = t_data.m_tv;
  				t_obj->SetField("tv", t_value);
                  TArray<TSharedPtr<FJsonValue>> t_value_arry = t_data.m_arr;
  				t_obj->SetArrayField("arr", t_value_arry);
  				TSharedPtr<FJsonValueObject> t_objValue = MakeShareable(new FJsonValueObject(t_obj));				
  				t_objects.Add(t_objValue);
  			}
  			else
  			{
  				UE_LOG(LogTemp, Error, TEXT("SerializeDataToJson %d"),it.Key);
  			}
  		}
   
  		t_jsonObject->SetArrayField("datas", t_objects);
  	}
   
  	return t_jsonObject;
  }
  
  //JsonObject to Object
  bool DeserializeFromJson(const TSharedPtr<FJsonObject> &_jsonObject, TMap<int, Data> &_data)
  {
  	if _jsonObject.IsValid() && _jsonObject->Values.Num()>0)
  	{
  		TArray<TSharedPtr<FJsonValue>> t_objects;
  		t_objects = _jsonObject->GetArrayField("datas");
  		int t_objectsNum = t_objects.Num();
  		if (t_objectsNum>0)
  		{
  			for (int i=0;i<t_objectsNum;i++)
  			{
  				TSharedPtr<FJsonObject> t_obj = t_objects[i]->AsObject();
  				if (t_obj->Values.Num()>0)
  				{
  					Data t_data;
  					int t_id = t_obj->GetNumberField("id");
  					t_data.m_id = t_id;
                      t_data.m_tv = t_obj->GetField<EJson::Object>("tv")
  					t_data.m_arr = t_obj>GetArrayField("arr")
  					_data.Add(t_id,t_data);
  				}
  				else
  				{
  					UE_LOG(LogTemp, Error, TEXT("DeserializeFromJson %d"),i);
  				}
  			}
  			return _data.Num() > 0;
  		}
  	}
  	return false;
  }
  
  //文件读写
  bool WriteFileWithJsonData(const FString &_jsonStr, const FString &_fileName)
  {
  	if (!_jsonStr.IsEmpty())
  	{
  		if (!_fileName.IsEmpty())
  		{
  			FString t_path = FPaths::GameContentDir() + "/"+_fileName+".json";
  			if (!FPaths::FileExists(t_path))
  			{
  				if (FFileHelper::SaveStringToFile(_jsonStr, *t_path))
  				{
      UE_LOG(LogTemp, Warning, TEXT("WriteTextFileWithJsonData---Save file : %s , path:  %s"), *_fileName, *t_path);
  					return true;
  				}		
  				else
  				{
  	UE_LOG(LogTemp,Error,TEXT("WriteTextFileWithJsonData---Save file : %s , path:  %s"), *_fileName,*t_path);
  				}
  			}			
  		}
  	}
  	return false;
  }
  bool LoadFStringFromFile(const FString &_fileName,const FString &_fromatStr,FString &_resultStr)
  {
  	if (!_fileName.IsEmpty())
  	{
  		FString t_path = FPaths::GamePluginsDir() + "DrawHouse/SaveData/" + _fileName + _fromatStr;
  		if (FPaths::FileExists(t_path))
  		{
  			if (FFileHelper::LoadFileToString(_resultStr, *t_path))
  			{
  	UE_LOG(LogTemp, Warning, TEXT("LoadFStringFromFile---load file : %s , path:  %s"), *_fileName, *t_path);
  				return true;
  			}
  			else
  			{
  	UE_LOG(LogTemp, Error, TEXT("LoadFStringFromFile---load file : %s , path:  %s"), *_fileName, *t_path);				
  			}
  		}
  		else
  		{
  	UE_LOG(LogTemp, Error, TEXT("LoadFStringFromFile---load file : %s , path:  %s"), *_fileName, *t_path);			
  		}
  	}
  	return false;
  }
  ```




* UE4智能指针




* UE4读写配置 ini

  ``` c++
  //使用自带配置文件
  #if PLATFORM_ANDROID
  	GConfig->GetString(
  		TEXT("/Script/AndroidRuntimeSettings.AndroidRuntimeSettings"),
  		TEXT("VersionDisplayName"),
  		versionName,
  		GEngineIni   //FPaths::ProjectConfigDir() / "DefaultEngine.ini" also able
  	);
  	//also able
  	//if (JNIEnv* env = FAndroidApplication::GetJavaEnv())
  	//{
  	//	static jmethodID Method_GetVersionName = FJavaWrapper::FindMethod(env, FJavaWrapper::GameActivityClassID, "getVersionName", "()Ljava/lang/String;", false);
  	//	jstring res = (jstring)FJavaWrapper::CallObjectMethod(env, FJavaWrapper::GameActivityThis, Method_GetVersionName);
  	//	versionName = FStringFromLocalRef(env, res);
  	//}
  	//UE_LOG(LogTemp, Log, TEXT("### in version name: %s"), *versionName);
  
  #elif PLATFORM_IOS
  	GConfig->GetString(
  		TEXT("/Script/IOSRuntimeSettings.IOSRuntimeSettings"),
  		TEXT("VersionInfo"),
  		versionName,
  		GEngineIni
  	);
  #endif
  ```

  ``` c++
  //自定义新建配置文件
  //MyActor.h
  
  //1.UClass的config配置需要保存的ini文件名
  UCLASS(config = XXX)
  class MYPROJECT_API AMyActor : public AActor
  {
  	GENERATED_BODY()
  	
  public:	
  	// Sets default values for this actor's properties
      //2.需要读取的变量 读取的变量的UPROPERTY中添加config
  	UPROPERTY(config, BlueprintReadWrite)
  		int32 TempA;
      
  //3.在工程目录下创建ini  (如上：DefaultXXX.ini)
  /*
  /Script/你的工程名.你的使用类去掉A或者U之类的前缀
  
  [/Script/Myproject.MyActor]
  TempA=123
  */
  //运行程序，可以读取到这里面的数值，如果修改的话，需要重启editor
  ```

  ``` c++
  //使用配置文件
  //注意路径：程序在打shipping包后变量读取的路径就变成了 C:\Users\用户名\AppData\Local\MyProject\Saved\Config\WindowsNoEditor
  	FString platFormName = FPlatformProperties::PlatformName();
  	FString gameConfigPath = FPaths::GameSavedDir() + TEXT("Config/") + platFormName + TEXT("/TTT.ini");
  	
  	if (GConfig)
  	{
  		GConfig->SetInt(TEXT("/Script/Myproject.MyActor"),
  			TEXT("TempA"),
  			ttt,
  			gameConfigPath);
   
  		GConfig->Flush(true, gameConfigPath);
  	}
  ————————————————
  版权声明：本文为CSDN博主「鸿蒙老道」的原创文章，遵循CC 4.0 BY-SA版权协议，转载请附上原文出处链接及本声明。
  原文链接：https://blog.csdn.net/maxiaosheng521/article/details/96425767
  ```



* UE4 路径读写文件

  [UE4文件系统](https://dhbloo.github.io/2020/09/07/UE4-FileSystem/#iplatformfile)

  ``` c++
  {
      FString fs = LoadFileInPluginsPath(TEXT("ThirdParty/TMSDKBridge/PlatformRes/All"), TEXT("maple/dir_region_info.json"));
  	if (!fs.IsEmpty())
  	{
  		TSharedRef<TJsonReader<>> freader = TJsonReaderFactory<>::Create(fs);
  		TSharedPtr<FJsonObject> fobjPtr;
  		bool succ = FJsonSerializer::Deserialize(freader, fobjPtr);
  		if (succ)
  		{
  			m_regionId = fobjPtr->GetIntegerField(TEXT("regionId"));
  			m_subareaId = fobjPtr->GetIntegerField(TEXT("subareaId"));
  			UE_LOG(LogTemp, Log, TEXT("read file json: regionId: %d. subareaId: %d"),
  				m_regionId, m_subareaId);
  		}
  	}
  	else
  	{
  		//test log
  		UE_LOG(LogTemp, Error, TEXT("load file is empty: %s"), FPlatformProcess::BaseDir());
  		//only for ue4 windows and editor
  		UE_LOG(LogTemp, Error, TEXT("load file is empty: %s"), *(FPaths::ProjectPluginsDir()));
  		UE_LOG(LogTemp, Error, TEXT("load file is empty: %s"), *(FPaths::ConvertRelativePathToFull(FPaths::ProjectPluginsDir())));
  	}
  }
  
  
  FString LoadFileInPluginsPath(const FString& fileRelativePathRoot, const FString& infileRelativeName)
  {
  	FString pluginRoot = FPaths::ConvertRelativePathToFull(FPaths::ProjectPluginsDir());
  	FString readFilePath;
  	FString fstream;
      /*相对路径：是相对于沙盒（Dev: /storage/emulated/0/UE4Game）
      或者应用内部路径（/data/user/0/包名/files）
      */
  #if PLATFORM_WINDOWS
  	readFilePath = FPaths::Combine(pluginRoot, fileRelativePathRoot, infileRelativeName);
  #elif PLATFORM_ANDROID
  	readFilePath = infileRelativeName;
  #elif PLATFORM_IOS
  	readFilePath = infileRelativeName;
  #endif
  	UE_LOG(LogTemp, Log, TEXT("### read file: %s..."), *readFilePath);
  	if (FFileManagerGeneric::Get().FileExists(*readFilePath))
  	{
  		bool succ = FFileHelper::LoadFileToString(fstream, *readFilePath);
  		if (!succ)
  		{
  			UE_LOG(LogTemp, Error, TEXT("### read file: %s is failed"), *readFilePath);
  		}
  	}
  	else
  	{
  		UE_LOG(LogTemp, Error, TEXT("### not found file: %s"), *readFilePath);
  	}
  	return fstream;
  }
  
  
  FString TxtStream;//文本流
  //写入文本
  FFileHelper::SaveStringToFile(TxtStream, *TxtPath);
  //对文本逐字符处理 存入OutArray
  TCHAR *TxtData = TxtStream.GetCharArray().GetData();
  for (int i = 0; i < TxtStream.Len(); i++){}
  ```

  ``` c++
  //[Android]FPaths::ConvertRelativePathToFull didn't work
  /*
  You shouldn't try to read or write to GetFileBasePath() directly since anything here is possibly inside another file wrapper. You should read and write native files to GExternalFilePath instead. If you really need to get this, though, you can do it like so:
  */
  const FString& GetFileBasePath_TM()
  {
  	extern FString GFilePathBase;
  	static FString BasePath = GFilePathBase + FString(TEXT("/UE4Game/")) + FApp::GetProjectName() + FString("/");
  	return BasePath;
  }
  
  FString AndroidRelativeToAbsolutePath_TM(FString RelPath)
  {
  	if (RelPath.StartsWith(TEXT("../"), ESearchCase::CaseSensitive))
  	{
  		do {
  			RelPath.RightChopInline(3, false);
  		} while (RelPath.StartsWith(TEXT("../"), ESearchCase::CaseSensitive));
  
  		return GetFileBasePath_TM() / RelPath;
  	}
  	return RelPath;
  }
  
  
  //@注意：  例如获取多平台的Save目录，方法并不一致
  /*
  FPaths::ProjectSavedDir();
  FPaths::ConvertRelativePathToFull(FPaths::ProjectSavedDir());
  IFileManager::Get().ConvertToAbsolutePathForExternalAppForRead(*FPaths::ProjectSavedDir());
  */
  /* android输出的 均为 ../../../工程名/Saved/  即Android端不支持 需要使用其他路径获取方式
  	extern FString GFilePathBase;
  	static FString BasePath = GFilePathBase + FString(TEXT("/UE4Game/")) + FApp::GetProjectName() + FString("/");
  	FString saveRootDir = AndroidRelativeToAbsolutePath_TM(FPaths::ProjectSavedDir());
  */
  /* ios 通过ConvertToAbsolutePathForExternalAppForWrite输出为： /var/mobile/Containers/Data/Application/E5F4B8E6-6A55-4757-AD34-CDC0739A9A99/Documents/TMSDKSample_425/Saved/*/
  ```

  

  * Android FileManager.GetTimeStamp(Filename) 

    ``` tex
    测试环境：UE4.25
    获取时间戳为默认值 
    FDateTime 0001.01.01-00.00.00
    ```
    
    
    

* UE4 FDateTime

  ``` c++
  //1.时分秒
  ToString(TEXT("%h:%m:%s.%t"))
  //2.年月日
  ToString(TEXT("%Y%m%d"))， 格式：20190101
  FDateTime::Now().ToString(TEXT("%Y-%m-%d-%H-%M-%S"))
     
  //时间戳
  FDateTime::Now().ToUnixTimestamp();
  //标准时区是：
  FDateTime::UtcNow().ToUnixTimestamp();
  ```

  



* UE4 Http

  * 上传文件
  * 请求验证





* UE4 Android Permission

  ``` tex
  4.25源码：
  模块：UE_4.25\Engine\Plugins\Runtime\AndroidPermission
  ```
  
  ``` c++
  //AndroidPermissionFunctionLibrary.h
  //用到的Android端库：
  #if PLATFORM_ANDROID && USE_ANDROID_JNI
  	JNIEnv* env = FAndroidApplication::GetJavaEnv();
  	_PermissionHelperClass = FAndroidApplication::FindJavaClassGlobalRef("com/google/vr/sdk/samples/permission/PermissionHelper");
  	_CheckPermissionMethodId = env->GetStaticMethodID(_PermissionHelperClass, "checkPermission", "(Ljava/lang/String;)Z");
  	_AcquirePermissionMethodId = env->GetStaticMethodID(_PermissionHelperClass, "acquirePermissions", "([Ljava/lang/String;)V");
  #endif
  
  //Permission Callback
  //AndroidPermissionCallbackProxy.cpp
  ```
  
  ```c++
  //使用示例：
  //UARCoreAndroidPermissionHandler.cpp
  bool UARCoreAndroidPermissionHandler::CheckRuntimePermission(const FString& RuntimePermission)
  {
  	return UAndroidPermissionFunctionLibrary::CheckPermission(RuntimePermission);
  }
  
  void UARCoreAndroidPermissionHandler::RequestRuntimePermissions(const TArray<FString>& RuntimePermissions)
  {
  	UAndroidPermissionCallbackProxy::GetInstance()->OnPermissionsGrantedDynamicDelegate.AddDynamic(this, &UARCoreAndroidPermissionHandler::OnPermissionsGranted);
  	UAndroidPermissionFunctionLibrary::AcquirePermissions(RuntimePermissions);
  }
  
  void UARCoreAndroidPermissionHandler::OnPermissionsGranted(const TArray<FString> &Permissions, const TArray<bool>& Granted)
  {
  	UAndroidPermissionCallbackProxy::GetInstance()->OnPermissionsGrantedDynamicDelegate.RemoveDynamic(this, &UARCoreAndroidPermissionHandler::OnPermissionsGranted);
      //TODO
  	FGoogleARCoreDevice::GetInstance()->HandleRuntimePermissionsGranted(Permissions, Granted);
  }



* UE4 Log

  * Shipping模式下开启Log

    [Debug Shipping Builds In Unreal Engine 4.23](https://blog.jamie.holdings/2019/09/15/debug-shipping-builds-in-unreal-engine-4-23/)

    [UE4 Quick Tip: Logging in Shipping Builds](https://stefanperales.com/blog/ue4-quick-tip-logging-in-shipping-builds/)

    ``` tex
    You must first have a source build of the engine on your PC. It will not work at all without it. Get it, compile it in Development Editor mode (with all the utility programs) before continuing.
    
    Once you have done that, you put that line of code, plus some other options, into the yourproject.Target.cs file in the source folder of your project.
    ```

    ``` c#
    //在4.25.4 EngineInstalled环境下不支持 ？？？
    public class GameTarget : TargetRules
    {
    	public GameTarget(TargetInfo Target) : base(Target)
    	{
    		Type = TargetType.Game;
    
    		// enable logs and debugging for Shipping builds
    		if (Configuration == UnrealTargetConfiguration.Shipping)
    		{
    			BuildEnvironment = TargetBuildEnvironment.Unique;
    			bUseChecksInShipping = true;
    			bUseLoggingInShipping = true;
    		}
    
    		ExtraModuleNames.AddRange( new string[] { "Game" } );
    	}
    }
    ```

    ``` tex
    By default, packaged games using the Shipping configuration will not log anything at all. This is great for security. However, when your play testers and live player's run into issues, it can be very difficult to troubleshoot without logs.
    
    With a small change, it is possible to enable logging in your shipping builds. Surprisingly, this one required a little but of research and trial and error.
    
    In your {projectname}.Target.cs file, in the contrsuctor, add the following line: bUseLoggingInShipping = true;
    
    By itself this will cause your builds to fail. You also need to set one of two other flags depending on whether or not you're using a source build of UE4 or one installed from the Epic Games Launcher.
    
    If using a source build add: BuildEnvironment = TargetBuildEnvironment.Unique
    If using an installed version, add: bOverrideBuildEnvironment = true;
    
    You should also consider keeping logging disabled in shipping and handling uncaught errors different. For example, gracefully falling back to your main menu and displaying an error message to the user.
    ```



* UE4 获取屏幕长宽，视口缩放，设置Widget位置，世界坐标转屏幕坐标

  ``` c++
  //viewport size
  FString screenXY = GEngine->GameViewport->Viewport->GetSizeXY().ToString();
  FVector2D Result = FVector2D(0, 0);
  if ( GEngine && GEngine->GameViewport )
  {
      GEngine->GameViewport->GetViewportSize( /*out*/Result );
  }
  
  //viewport center
  FVector2D centerPos = FVector2D(screenXY.X/2, screenXY.Y/2);
  
  //viewport scale
  float scale = UWidgetLayoutLibrary::GetViewportScale(this);
  FVector2D vec2D;
  vec2D = screenXY.X / 2 / scale;
  vec2D = screenXY.Y / 2 / scale;
  
  //widget translation
  //设置Widget位置1 （绝对坐标）
  UWidget::SetRenderTranslation(vec2D);
  ```

  ``` c++
  //判断一个点是否在屏幕范围内：https://blog.csdn.net/l346242498/article/details/100575120
  ////世界坐标转屏幕坐标1
  bool AMyActor::IsInScrrenViewport(const FVector& WorldPosition)
  {
  	APlayerController *Player = UGameplayStatics::GetPlayerController(this, 0);
  	ULocalPlayer* const LP = Player ? Player->GetLocalPlayer() : nullptr;
  	if (LP && LP->ViewportClient)
  	{
  		// get the projection data
  		FSceneViewProjectionData ProjectionData;
  		if (LP->GetProjectionData(LP->ViewportClient->Viewport, eSSP_FULL, /*out*/ ProjectionData))
  		{
  			FMatrix const ViewProjectionMatrix = ProjectionData.ComputeViewProjectionMatrix();
  			FVector2D ScreenPosition;
  			bool bResult = FSceneView::ProjectWorldToScreen(WorldPosition, ProjectionData.GetConstrainedViewRect(), ViewProjectionMatrix, ScreenPosition);
  			if (bResult && ScreenPosition.X > ProjectionData.GetViewRect().Min.X && ScreenPosition.X < ProjectionData.GetViewRect().Max.X
  				&& ScreenPosition.Y > ProjectionData.GetViewRect().Min.Y && ProjectionData.GetViewRect().Y < LensPadding.Max.Y)
  			{
  				return true;
  			}
  		}
  	}
  	return false;
  }
  
  //获取视口长宽：viewport size
  screenPos.X /= projectionData.GetConstrainedViewRect().Width();
  screenPos.Y /= projectionData.GetConstrainedViewRect().Height();
  UE_LOG(LogTemp, Log, TEXT("### transform position: screen width: %d, height: %d"),
  		projectionData.GetConstrainedViewRect().Width(),
  		projectionData.GetConstrainedViewRect().Height());
  
  
  //世界坐标转屏幕坐标2
  FVector2D screenPos2;
  UGameplayStatics::ProjectWorldToScreen(playerCtrl,worldPos, /*out*/screenPos2, false);
  FVector2D screenPos3;
  UGameplayStatics::ProjectWorldToScreen(playerCtrl,worldPos, /*out*/screenPos3, true);
  UE_LOG(LogTemp, Log, TEXT("### 2 transform position: screen pos, x: %f, y: %f"),screenPos2.X,screenPos2.Y);
  UE_LOG(LogTemp, Log, TEXT("### 3 transform position: screen pos, x: %f, y: %f"),screenPos3.X,screenPos3.Y);
  //同一场景下结果相同
  ```

  ``` c++
  //设置Widget位置2
  UPROPERTY(meta = (BindWidget))
  class UImage* ImageHpGuang;
  UPROPERTY(meta = (BindWidget))
  class UProgressBar* ProgressBarMainActorHpBar;
  UCanvasPanelSlot* canvasSlotGuang = UWidgetLayoutLibrary::SlotAsCanvasSlot(Cast<UWidget>(ImageHpGuang));
  UCanvasPanelSlot* canvasSlotHpBar = UWidgetLayoutLibrary::SlotAsCanvasSlot(ProgressBarMainActorHpBar);
  canvasSlotGuang->SetPosition(FVector2D((canvasSlotHpBar->GetSize().X) * hpPercent - 11, 0.9f));
  
  //设置、获取相对坐标
  Cast<UCanvasPanelSlot>( MyBtn->Slot)->SetPosition(FVector2D(X, Y));
  //ImgIcon is a UImage widget.
  if (UCanvasPanelSlot* Slot = Cast<UCanvasPanelSlot>(ImgIcon->Slot))
  {
      FVector2D Pos = Slot->GetPosition();
  }
  
  //设置、获取绝对坐标
  #include <Engine/UserInterfaceSettings.h>
  ImgIcon->GetCachedGeometry().GetAbsolutePosition();
  void UWidget::SetRenderTranslation(FVector2D Translation)   //设置widget绝对坐标
  void UUserWidget::SetPositionInViewport(FVector2D Position, bool bRemoveDPIScale )  //设置user widget（内嵌umg）绝对坐标
  ```

  ``` c++
  //获取HUD
  AMyHUD * hud = Cast<AMyHUD>(UGameplayStatics::GetPlayerController(this, 0)->GetHUD());
  ```

  

* UE4获取屏幕点击

  ``` c++
  //ref:https://blog.csdn.net/J_Wayne/article/details/107786787
  
  APlayerController* playerCtrl = Cast<APlayerController>(controller);
  if(playerCtrl) {
      playerCtrl->bShowMouseCursor = true;
  }
  FHitResult hit;
  if(playerCtrl) {
  	playerCtrl->GetHitResultUnderCursor(ECC_Visibility, false, hit);
  }
  if(hit.bBlockingHit) {
      //TODO
  }
  /*
  只要是继承于ACharacter或APawn类的，都可以把Controller转换成APlayerController来用GetHitResultUnderCursor这个函数，本质上其实是射线检测GetWorld()->LineTraceSingleByChannel，只是做了很多安全判断，比如保证获得玩家屏幕，保证点击的不是UI。
  通过Hit传引用进去，传结果出来。Hit.bBlockingHit代表检测到了物体
  */
  
  
  //射线检测
  if(GetHUD() != NULL && GetHUD()->GetHitBoxAtCoordinates(ScreenPosition, true))  //判断是否点击到UI
  {
      return false;
  }
  FVector WorldOrigin;
  FVector WorldDirection;
  //屏幕坐标转世界坐标
  if(UGameplayStatics::DeprojectScreenToWorld(this, ScreenPosition, WorldOrigin, WorldDirection) == true)
  {
      return GetWorld()->LineTraceSingleByChannel(HitResult, WorldOrigin, WorldOrigin + HitResultTraceDistance, TraceChannel, CollisionQueryParams);
  }
  /*
  官方：GetHitResultUnderCursor
  */
  ```

  



* UE4 TMap, TArray 不支持多层嵌套

  ``` c++
  //不支持
  UPROPERTY（）
  TMap<int64, TArray<int64>>
   
  //这种可以用结构体转换
  USTRUCT(BlueprintType)			//@注意：必须加BlueprintType ！！！
  struct FMyArray
  {
  	GENERATED_BODY()
   
      UPROPERTY(EditAnywhere, BlueprintReadOnly) //或者 BlueprintReadWrite
  	TArray<int64> myArray;
  };
  
  //使用
  UPROPERTY（）
  TMap<int64, FMyArray>
  ```



* UE4 Time

  ``` c++
  FDateTime Time = FDateTime::Now();
  //获取时间戳
  int64 Timestamp = Time.ToUnixTimestamp();
  UE_LOG(LogTemp, Warning, TEXT("%d"), Timestamp);
  
  //unix timestamp <=> datetime
  FDateTime Time = FDateTime::FromUnixTimestamp(int64 UnixTime);
  int64 Timestamp = Time.ToUnixTimestamp();
  
  int year = Time.GetYear();
  int month = Time.GetMonth();
  int day = Time.GetDay();
  int hour = Time.GetHour();
  int minute = Time.GetMinute();
  int second = Time.GetSecond();
  
  //获取CPU时钟
  //微妙格式
  uint64 cycle = FPlatformTime::Cycles64();
  uint32 cycle = FPlatformTime::Cycles();
  //秒格式
  double now = FPlatformTime::Seconds();
  
  //frame deltatime
  float DeltaTime = FApp::GetDeltaTime();
  ```

  



* UE4 常用宏

  [UE4入门-常见的宏-UFUNCTION](https://blog.csdn.net/u012793104/article/details/78487893)

  * UFUNCTION 函数说明符

    ``` tex
    BlueprintAuthorityOnly
    如果在具有网络权限的计算机（服务器，专用服务器或单人游戏）上运行，此功能只能从Blueprint代码执行,如无网络权限，则该函数将不会从蓝图代码中执行
    
    BlueprintCosmetic
    此函数为修饰函数而且无法运行在专属服务器上
    
    BlueprintGetter 修饰自定义的Getter函数专用
    该函数将用作Blueprint暴露属性的访问器。这个说明符意味着BlueprintPure和BlueprintCallable
    
    BlueprintSetter 修饰自定义的Setter函数专用
    此函数将用作Blueprint暴露属性的增变器。这个说明符意味着BlueprintCallable
    
    BlueprintInternalUseOnly
    表示该函数不应该暴露给最终用户
    
    
    
    ######  常用  ######
    
    UFUNCTION(BlueprintCallable, Category="这是分组标签1|这是分组页签2")
    void BlueprintCallableFunc();
    该函数可以在蓝图或关卡蓝图图表或UnLua中执行（即蓝图中可以搜索到并创建和调用）
    
    UFUNCTION(BlueprintImplementableEvent, Meta=(DisplayName="蓝图中Functions页签下Override Function的展示名字"))
    float BlueprintImplementableEventFunc(float infloat);
    此函数可以在蓝图或关卡蓝图图表或UnLua内进行重载
    (不能修饰private级别的函数，函数在C++代码中不需要实现定义)
    
    
    UFUNCTION(BlueprintNativeEvent, Meta=(DisplayName="Blueprint Native Event Func"))
    FString BlueprintNativeEventFunc(AActor* inActor);
    FString AActorTest::BlueprintNativeEventFunc_Implementation(AActor* inActor)
    {
    	return inActor->GetName();
    }
    此函数将由蓝图进行重载，但同时也包含native类的执行。提供一个名称为[FunctionName]_Implementation的函数本体而非[FunctionName];自动生成的代码将包含转换程序,此程序在需要时会调用实施方式
    
    
    UFUNCTION(BlueprintPure)
    AActor* BlueprintPureFunc();
    AActor* AActorTest::BlueprintPureFunc()
    {
    	return this;
    }
    该函数不会以任何方式影响拥有对象，并且可以在蓝图或级别蓝图图表中执行
    
    
    
    CallInEditor
    该函数可以在编辑器中通过详细信息面板中的按钮在选定实例中调用
    
    Category = TopCategory|SubCategory|Etc
    指定函数在编辑器中的显示分类层级，| 是分层级的符号 (蓝图窗口是树状层级)
    ```

  * UFUNCTION 元数据说明符

    ``` tex
    
    UFUNCTION(BlueprintCallable, meta = (DeprecatedFunction, DeprecationMessage = "This is Deprecation Message"), Category = "Snowing|BlueprintFunc")
        void DeprecatedFunctionFunction();
    任何对此函数的蓝图引用都会导致编译警告, 告诉用户该函数已被弃用。可以使用 DeprecationMessage 元数据说明符来添加到弃警告消息 (例如, 提供有关替换已弃用的函数的说明)
    添加这个标记后，在4.18.0引擎中(可能以及后续版本)，蓝图无法查找到被标记的函数
    
    DeprecationMessage=”MessageText”
    如果该函数已被弃用，则在尝试编译使用该函数的Blueprint时，此消息将被添加到标准弃用警告中
    
    
    UFUNCTION(BlueprintPure, meta = (DisplayName=”Blueprint Node Name”))
    FString OtherBlueprintPureFuncName();
    蓝图中此节点的名称将替换为此处提供的值，而不是代码生成的名称（在蓝图中搜索被修饰函数也用这里提供的值）
    ```

  

  [UE4入门-常见的宏-USTRUCT](https://blog.csdn.net/u012793104/article/details/78594119)

  * USTRUCT

    **USTRUCT宏收缩与指定某些设置和属性**； @注意，只有USTRUCT的UPROPERTY变量才被考虑到复制；只有PROPERTY宏标记的USTRUCT才能被计入垃圾回收

    （当在虚幻引擎中声明一个USTRUCT时，可以添加一个属于虚幻引擎的struct trait系统的NetSerialize方法，如果定义了这个方法，在属性复制和RPC之间，引擎会在struct序列化和反序列化的时候调用它）

    ``` tex
    USTRUCT(Atomic)		表示这个结构应该总是作为一个单元序列化
    struct Person {}
    
    USTRUCT(BlueprintType)  将此结构公开为蓝图中变量的类型
    struct Person {}
    
    USTRUCT(Immutable)	  在Object.h中只是合法的，正在被弃用
    struct Person {}
    
    USTRUCT(NoExport)    没有自动生成的代码将被创建为这个类，头只是用来提供解析元数据的
    struct Person {}
    ```

  [UE4入门-常见的宏-UCLASS](https://blog.csdn.net/u012793104/article/details/78547655)

  [UE4入门-常见的宏-UPROPERTY](https://blog.csdn.net/u012793104/article/details/78480085)






---



### UE4 C++注意点

* **Editor中Play和Stop，无法触发类的析构**

  ``` c++
  //自定义一个继承自GameInstance的类
  UCLASS()
  class HITBOXMAKERBLUEPRINT_API UTMGameInstance : public UGameInstance
  {
  	GENERATED_BODY()
          
     	virtual void Init() override;
      //在生命周期中处理类的初始化和反初始化
  	virtual void Shutdown() override;
  }
  ```

  



* NewObject单例不自动销毁，避免UObject被销毁导致的野指针清空

  ``` c++
  static UAndroidPermissionCallbackProxy *pProxy = NULL;
  
  UAndroidPermissionCallbackProxy *UAndroidPermissionCallbackProxy::GetInstance()
  {
      //挂载到Root上
  	if (!pProxy) {
  		pProxy = NewObject<UAndroidPermissionCallbackProxy>();
  		pProxy->AddToRoot();
  
  	}
  	UE_LOG(LogAndroidPermission, Log, TEXT("UAndroidPermissionCallbackProxy::GetInstance"));
  	return pProxy;
  }
  //取消挂载
  pProxy->RemoveFromRoot();
  ```






---



### UE4 C++ with blueprint

* 代码修改一个蓝图的Class Defaults

  ![](https://raw.githubusercontent.com/MJX1010/PicGoRepo/main/img/20210902082514.jpg)

  ``` c++
  void ModifyFieldValue(Blueprint* Blueprint, FName FiledName)
  {
      FBoolProperty* Op = FindFProperty<FBoolProperty>(Blueprint->SkeletonGeneratedClass, FiledName);
      if (nullptr == Op)
      {
          UE_LOG(LogTemp, Error, TEXT("Can't found bool property %s"), *FiledName.ToString());
          return;
      }
  
      Op->SetPropertyValue_InContainer(Blueprint->GeneratedClass->ClassDefaultObject, true);
  }
  ```






* 代码修改导致蓝图编辑器环境改变

  ``` c++
  // Copyright Epic Games, Inc. All Rights Reserved.
  #include "TableIDInput.h"
  
  #include "IntProperty_TableIDCustomization.h"
  #include "TableIDCustomization.h"
  
  #define LOCTEXT_NAMESPACE "FTableIDInputModule"
  
  void FTableIDInputModule::StartupModule()
  {
  	FPropertyEditorModule& PropertyModule = FModuleManager::LoadModuleChecked<FPropertyEditorModule>("PropertyEditor");
  	PropertyModule.RegisterCustomPropertyTypeLayout("TableID", FOnGetPropertyTypeCustomizationInstance::CreateStatic(&FTableIDCustomization::MakeInstance));
  	//PropertyModule.RegisterCustomPropertyTypeLayout("IntProperty", FOnGetPropertyTypeCustomizationInstance::CreateStatic(&FIntPropertyTableIDCustomization::MakeInstance));
      //@注意：会导致蓝图中如 TextBlock的size不可修改
  }
  
  void FTableIDInputModule::ShutdownModule()
  {
  	FPropertyEditorModule& PropertyModule = FModuleManager::LoadModuleChecked<FPropertyEditorModule>("PropertyEditor");
  	PropertyModule.UnregisterCustomPropertyTypeLayout("TableID");
  	//PropertyModule.UnregisterCustomPropertyTypeLayout("IntProperty");
  }
  
  #undef LOCTEXT_NAMESPACE
  	
  IMPLEMENT_MODULE(FTableIDInputModule, TableIDInput)
  ```

  

* C++ 和 蓝图相互调用

  ``` c++
  //创建 C++函数 蓝图调用
  // 头文件中声明函数
  UFUNCTION(BlueprintCallable, Category = "BPFunc_Lib")
      void CppPrint();
  /*
  和公开属性类似，使用宏 UFUNCTION 即可。UFUNCTION() 负责将C++函数公开给反射系统。**BlueprintCallable ** 选项将其公开给蓝图虚拟机。每一个公开给蓝图的函数都需要一个与之关联的类别(Category)，这样右键点击快捷菜单的功能才能正确生效。
  */
  
  //创建蓝图（实现）事件C++调用
  //虚幻引擎会在其内部自动生成C++函数的基本实现，该实现了解如何调用蓝图VM。这通常称为形实替换。
  //如果对应的蓝图没有实现该函数，则函数行为就像C++空函数体一样，不执行任何操作。
  UFUNCTION(BlueprintImplementableEvent, Category = "BP_Funclib")
  	void BPPrint();
  
  //创建蓝图（实现）事件 C++调用
  //提供C++函数的默认实现，同时仍允许蓝图覆盖此函数，
  UFUNCTION(BlueprintNativeEvent, Category = "BPFunc_Lib")
  	void BPPrint1();
  ```

  



---



### UE4 C++编码规范

* link

  [UE4编码规范](https://wangjie.rocks/2017/03/23/ue4-coding-standard/)

  [UE4官方 - 代码规范](https://docs.unrealengine.com/zh-CN/ProductionPipelines/DevelopmentSetup/CodingStandard/index.html)

  [UE4 项目的设计规范和代码标准](https://imzlp.com/posts/25915/)


* 规范表-1

  ```c++
  //使用nullptr == xxx 判空
  //避免写成 nullptr != xxx 以及 xxx = nullptr
  if(nullptr == XXX) {}
  ```

  



---



### UE4 GC

link: [UE4 TSharedPtr和UObject的垃圾回收](http://www.v5xy.com/?p=808)

``` text
TSharedPtr 不能用于 UObject
UObject基于UE4自身宏机制实现的GC, 即必须要添加UProperty()
例如：
    UProperty()
    UObject* Obj;

Unreal Build Tool(UBT)和Unreal Header Tool (UHT)
两个协同工作来生成运行时反射需要的数据。UBT属性通过扫描头文件，记录任何至少有一个反射类型的头文件的模块。如果其中任意一个头文件从上一次编译起发生了变化，那么 UHT就会被调用来利用和更新反射数据。UHT分析头文件，创建一系列反射数据，并且生成包含反射数据的C++代码（放到每一个模块的moulde.generated.inl中。注：最新版会生成到moudle.generated.cpp中），还有各种帮助函数以及thunk函数（每一个 头文件 .generated.h）

反射系统在对整个系统的宏进行扫描都是在系统编译之前
UE4本身利用UBT和UHT进行宏扫描和展开，然后生成特定记录元数据的代码提供给最后VS进行编译

如果是本身在函数内定义的对象，并不能加入UProperty，可选择使用虚幻自身的TArray和TMap进行存储也可参与GC。

如果不使用宏标记，也不适用TArray和TMap存储，尝试用自定义别名方式对UE4的UProperty宏进行预定义
例如：
    typedef UPROPERTY() AEntity* EntityPtr;
    #define EntityDefinePtr UPROPERTY() AEntity*

    测试：
    //1
    EntityPtr testP;
    //2
    EntityDefinePtr defP;
    //3
    UPROPERTY()
    AEntity* entityPtr;
    
测试结果：编译之后，虚幻自身GC对我们自定义的宏和别名并没进行替换和展开，generated.h里面并没有记录，也就不会参与GC。
原因：关键在于虚幻自身实现的UBT对他自身宏的展开，其实早于VS编译时的展开。
	UBT首先完成makefile的创建和编译规则，然后就会请求扫描头文件中的宏（UNFUNCTIN,UProperty...）生成generated.h文件，
	随后发出编译请求，链接到当前的编译器（VS），这时候才启动VS自身的编译，
	那么无论是#define在VS编译之前或者typedef别名替换在编译中，都已经晚于虚幻4自身的CG宏展开，那么自然也就没法实现GC了，
	其实从VS中提示intell语法检测中提示没有generated.h文件时的错误时可以看出generated.h文件并不被VS编辑器生成的，编译结果也能正常通过。

结论：
	只有常规定义的宏才被正确展开了，typedef和define都未正确被UBT展开。
	

```

