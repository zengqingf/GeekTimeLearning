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

  

  



---



### UE4 C++编码规范

* link

  [UE4编码规范](https://wangjie.rocks/2017/03/23/ue4-coding-standard/)

  [UE4官方 - 代码规范](https://docs.unrealengine.com/zh-CN/ProductionPipelines/DevelopmentSetup/CodingStandard/index.html)

  [UE4 项目的设计规范和代码标准](https://imzlp.com/posts/25915/)

  