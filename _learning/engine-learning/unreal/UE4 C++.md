# UE4 C++



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
    获取时间戳为默认值 FDateTime 0001.01.01-00.00.00
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

