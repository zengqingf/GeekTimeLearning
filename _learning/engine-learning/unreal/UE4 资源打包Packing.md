# UE4 资源打包Packing

* ref

  [UE4 资源热更打包工具 HotPatcher](https://imzlp.com/posts/17590/)

  [UE4资源加载（四）UnrealPak打包](https://www.dazhuanlan.com/2019/12/08/5dec71c68a74a/)

  [UE4 项目资源打包,Pak包生成](https://www.cnblogs.com/bodboy/p/6110528.html)

  [UE4打包后视频或者其他资源未打包进去](https://blog.csdn.net/u010925014/article/details/89638878)

  [UE4资源————Pak](https://www.233tw.com/unreal/8887)



* 资源

  * 目录结构

    ``` text
    You can add additional directories to stage via your project settings. Go to "Project Settings", then "Packaging", and you'll find "Additional Non-Asset Directories to Package" and "Additional Non-Asset Directories to Copy" under the advanced section.
    
    Which you use depends on whether your files are loaded via the Unreal File System (UFS), or whether they're loaded via a third-party IO API (including the STL).
    
    "Additional Non-Asset Directories to Package" will add the files to your .pak file, which is what you want when you're using the UFS API.
    
    "Additional Non-Asset Directories to Copy" will stage your files individually (outside the .pak file), which is what you want when you're using a third-party file IO API.
    ```

    

  * json存取

    ``` text
    Create new folder (like Data) inside your Content folder and place your .json file inside
    
    Ignore the editor converting the file (the original .json should still be where you put it).
    
    Inside project settings add your Data folder to the list of folders to copy (if you add it to the packaged it should only embed the .uasset file into game packages, which you don't want)
    
    Use the FPaths class and it's static methods to reference the folder, like so:
    
    FString path = FPaths::Combine(*FPaths::GameDir(), *FString("Data")); Note: Not at my dev machine at the moment so haven't tested that code - it might need adjusting
    ```

    ``` c++
    //使用绝对路径   content 外部访问 
    void UUnrealShooterDataSingleton::ParseJSON()
     {
         FString JsonString;
         const FString fileName = "D:/Projects/UnrealProjects/UnrealShooter/Shared/UnrealShooterData.json";
         FFileHelper::LoadFileToString(JsonString, *fileName);
         TSharedPtr<FJsonObject> JsonObject;
         TSharedRef<TJsonReader<>> Reader = TJsonReaderFactory<>::Create(JsonString);
     
         if (FJsonSerializer::Deserialize(Reader, JsonObject))
         {
             const TArray<TSharedPtr<FJsonValue>> SequencesJSON = JsonObject->GetArrayField(TEXT("sequences"));
             const TArray<TSharedPtr<FJsonValue>> WavesJSON = JsonObject->GetArrayField(TEXT("waves"));
             const TArray<TSharedPtr<FJsonValue>> TargetsJSON = JsonObject->GetArrayField(TEXT("targets"));
             const TArray<TSharedPtr<FJsonValue>> LocationsJSON = JsonObject->GetArrayField("locations");
     
             UUnrealShooterDataSingleton::ParseLocations(LocationsJSON);
             UUnrealShooterDataSingleton::ParseTargets(TargetsJSON);
             UUnrealShooterDataSingleton::ParseWaves(WavesJSON);
             UUnrealShooterDataSingleton::ParseSequences(SequencesJSON);
         }
     }
    ```

    




---



* 命令

  * 解pak

    ``` powershell
    Engine\Binaries\Win64\UnrealPak.exe somepak.pak -extract X:\extract\here 
    ```

    



---



### 资源加载

​	ref: [[UE4]C++实现动态加载的问题：LoadClass<T>()和LoadObject<T>() 及 静态加载问题：ConstructorHelpers::FClassFinder()和FObjectFinder()](https://www.cnblogs.com/sevenyuan/p/7728194.html)

​			[Aery的UE4 C++游戏开发之旅（4）加载资源&创建对象](https://www.cnblogs.com/KillerAery/p/12031057.html)

​			[UE4的资源管理](https://zhuanlan.zhihu.com/p/357904199)

* 路径

  ``` text
  路径获取
  如：取Content/Texture/jianpan_c时，路径应该写/Game/Texture/jianpan_c，以/Game开头（Content下），不管你的项目名字是什么，然后不要有/Content这个部分，直接接上后面的路径才能成功读取到图片
  或者右键 - Copy Reference
  
  如果使用LoadClass（）方法，路径名也必须带_C后缀（LoadObject不需要带_C后缀），
  例如，蓝图路径是：Blueprint'/Game/Blueprints/Test'， 加后缀以后，则是：Blueprint'/Game/Blueprints/Test_C'
  
  1. 类名'/路径/包名.对象名:子对象名'
  2. 类名其实会被忽略掉，可以不写
  3. 类：  /路径/包名.类名_C  （包名常和类名一致）
  4. 对象： /路径/包名.对象名  （包名常和对象名一致）
  5. 路径： /Root/Path1/Path2/...
  6. 路径Root: /Engine, /Game, /Module
  ```

  

* 加载蓝图

  * cook后的资源是没有蓝图对象的，即UBlueprint只用于Editor

    ``` c++
    //加载一个蓝图文件，然后获取到其GeneratedClass
    //路径：/Game/YOURDIR/BP_SomeBlueprint.BP_SomeBlueprint
    auto BP = Cast<UBlueprint>(StaticLoadObject(UBlueprint::StaticClass(), nullptr, *Path));
    UClass* Class =  BP ? BP->GeneratedClass : nullptr
    ```

  * cook后加载蓝图对象

    ``` c++
    //路径：/Game/YOURDIR/BP_SomeBlueprint.BP_SomeBlueprint_C。
    UClass* Class = Cast<UClass>(StaticLoadObject(UClass::StaticClass(), nullptr, *Path));
    ```

    ``` text
    另一种不推荐方法
    修改DefaultEditor.ini  将bDontLoadBlueprintOutsideEditor=true改成false
    使cook过的资源也保留蓝图部分，会影响包大小
    ```

  * 动态加载蓝图

    ``` c++
    //加载蓝图使用 LoadClass<T>
    //LoadClass<T>的模版名称，不能直接写UBlueprint，例如：LoadClass<UBlueprint>是错误的，创建蓝图时选择的是什么父类，则写对应的父类名，假如是Actor，那么要写成：LoadClass****<AActor****>，否则无法加载成功。
    UClass* Test = LoadClass<AActor>(NULL, TEXT("Blueprint'/Game/Blueprints/MapPathBrush_BP.MapPathBrush_BP_C'"));  
    ```

    

* 主模块加载Plugins中的蓝图

  ref: [Plugin Content can't be accessed by code.](https://answers.unrealengine.com/questions/315648/plugin-content-cant-be-accessed-by-code.html)

  ``` c++
  //UE4.25
  //如下例子中 TEXT中的路径通过右键->copy reference获取
  //不需要使用TEXT("Blueprint'/...'")  代替为 TEXT("/..._C")
  //路径的选择：
  //Plugins下的结构是：.../Plugins/RPGEngineToolKit/Content/RPGEngineTK/Blueprints/Huds/ModularHud.ModularHud'
  //使用时：static ConstructorHelpers::FClassFinder RPGHudClass(TEXT("/RPGEngineToolKit/RPGEngineTK/Blueprints/Huds/ModularHud.ModularHud_C"));
  
  
  //在编译期加载 如AGameMode或者派生类中的构造函数中  
  //e.g.
  	//加载UWA actor
  	static ConstructorHelpers::FClassFinder<AActor> UWAActor(TEXT("/UWAGOT/UWAActor.UWAActor_C"));
  	if (UWAActor.Succeeded())
  	{
  		UE_LOG(LogTemp, Log, TEXT("### 加载UWA Actor成功！！！"));
  	}
  	else
  	{
  		UE_LOG(LogTemp, Error, TEXT("### 加载UWA Actor失败！！！"));
  	}
  
  //在运行时加载
  //e.g.
  	//加载UWA actor
  	TSubclassOf<AActor> BP_UWA = LoadClass<AActor>(nullptr, TEXT("/UWAGOT/UWAActor.UWAActor_C"));
  	if (BP_UWA != nullptr)
  	{
  		AActor* uwaActor = GetWorld()->SpawnActor<AActor>(BP_UWA);
  		uwaActor->SetActorHiddenInGame(true);
  		UE_LOG(LogTemp, Log, TEXT("### 加载UWA Actor成功！！！"));
  	}
  	else
  	{
  		UE_LOG(LogTemp, Error, TEXT("### 加载UWA Actor失败！！！"));
  	}
  ```



* 动态加载

  ref: [UE4静态/动态加载资源方式](https://zhuanlan.zhihu.com/p/266859719)

  ``` text
  LoadClass()和LoadObject()
  LoadClass()和LoadObject()的区别如下：
  LoadObject()用来加载非蓝图资源，如贴图；
  LoadClass()用来加载蓝图并获取蓝图类；
  
  
  LoadClass<T>() 加载UClass，一般用来加载蓝图资源并获取蓝图Class。实际上源码里LoadClass的实现是调用LoadObject并获取类型。
  LoadClass的模版名称，和上面FClassFinder一样，不能直接写UBlueprint。
  LoadClass路径规范也和上面的FClassFinder一样，带_C后缀或去掉前缀。
  
  
  LoadObject（） vs StaticLoadObject（）
  有人会认为LoadObject要比StaticLoadObject的速度快一点，毕竟又多封装一次，但实际上我们发现，LoadObject使用了inline进行修饰，因此实际和StaticLoadObject没什么区别，只是参数不一样而已。
  StaticLoadObject()和StaticLoadClass()，是LoadObject()和LoadClass()的早期版本，前两者需要手动强转和填写冗杂参数，后两者则是前两者的封装，使用更方便，推荐使用后者。
  
  
  异步加载
  蓝图提供Async Load Asset异步加载资源的接口，可以直接使用
  
  
  编辑器加载
  编辑器中提供了Load Asset蓝图接口，对应的代码在EditorAssetLibrary下
  只能在Editor Utilities下使用 不能再继承Actor的蓝图中调用
  
  
  FindObject()
  全局函数FindObject()，用来查询资源是否载入进内存，若存在则返还资源对象指针，否则返还空。但是我们不用先查询再使用LoadXXX，因为LoadXXX里本身就有用到FindObject来检查存在性。
  ```

  

  * LoadObject

    ``` c++
    //LoadObject加载非蓝图对象，不需要加后缀_C
    UTexture2D* Tex = LoadObject<UTexture2D>(NULL, TEXT("Texture2D'/Game/Textures/UI/tex_test001.tex_test001'"));  
    
    //可以加载父类为UObject的资源对象
    //Texture、Material、SoundWave、SoundCue、ParticlesSystem、AnimMontage、BlendSpace(1D，2D，3D)、AnimSequence、AnimBlueprint、SkeletalMesh等等
    //可以先加载UObject* 然后强转为具体类型
    UObject* Obj = LoadObject<UObject>(NULL, TEXT("SkeletalMesh'/Game/MyMesh.MyMesh'"));   
    USkeletalMesh* MyMesh = Cast<USkeletalMesh*>(Obj);  
    ```

  * StaticLoadObject

    ``` c++
    UTexture2D* MyTextureLoader::LoadTextureFromPath(const FString& Path)   
    {   
        if (Path.IsEmpty()) return NULL;   
    
        return Cast<UTexture2D>(StaticLoadObject(UTexture2D::StaticClass(), NULL, *(Path)));   
    }
    
    //调用
    FString PathToLoad = "/Game/Textures/YourStructureHere";   
    UTexture2D* tmpTexture = LoadTextureFromPath(PathToLoad); 
    ```

  * FObjectFinderOptional<>

    ``` c++
    struct FConstructorStatics   
     {   
         ConstructorHelpers::FObjectFinderOptional<UTexture> TextureFinder;   
         ConstructorHelpers::FObjectFinderOptional<UMaterial> MaterialFinder;   
         FConstructorStatics()   
             : TextureFinder(TEXT("Texture2D'/Game/Textures/2DBackground.2DBackground'"))   
             , MaterialFinder(TEXT("Material'/Game/Materials/DynamicTextureMaterial.DynamicTextureMaterial'"))   
         {   
         }   
     };   
     static FConstructorStatics ConstructorStatics;   
    
     Texture = ConstructorStatics.TextureFinder.Get();   
     UMaterial* Material = ConstructorStatics.MaterialFinder.Get();   
     DynamicMaterial = UMaterialInstanceDynamic::Create(Material, this);  
    
    //设置加载好的Material Texture
    DynamicMaterial->SetTextureParameterValue(FName("DynamicTexture"), Texture);   
    Mesh->SetMaterial(0, DynamicMaterial);  
    ```

  

* 销毁资源

  ``` c++
  Texture2D* mytex; //这里假设mytex合法有效   
  
  mytex->ConditionalBeginDestroy();   
  mytex = NULL;   
  GetWorld()->ForceGarbageCollection(true);  
  ```

  

* 静态加载

  ``` text
  ConstructorHelpers::FClassFiner()  /   ConstructorHelpers::FObjectFiner()
  
  ConstructorHelpers::FClassFinder()和FObjectFinder()
  静态加载指的是在构造函数中完成的加载方式，这种方式的弊端明显，就是需要写死路径，一旦改变路径读取失败很容易造成程序崩溃
  
  需要在构造函数中调用
  
  在UE4源码里面，FObjectFinder构造函数里通过调用LoadObject()来加载资源，而FClassFinder构造函数里调用的也是LoadObject()。
  
  只能在类的构造函数中使用，如果在普通的逻辑代码中嵌套这份代码，会引起整个编译器的crash。（实际上里面代码就有检查是否在构造函数里，否则crash）
  
  FObjectFinder/FClassFinder变量必须是static的，从而保证只有一份资源实例。
  
  FClassFinder的模版名不能直接写UBlueprint，例如：FClassFinder<UBlueprint>是错误的。创建蓝图时选择的是什么父类，则写对应的父类名，假如是Actor，那么要写成：FClassFinder<AActor>，否则无法加载成功。
  FClassFinder的模版名必须和TSubclassOf变量的模版名一致，当然也可使用UClass*代替TSubclassOf<T>。实际上TSubclassOf<T>也是UClass*，只是更加强调这个Class是从T派生出来的。
  
  在启动游戏时若报错提示找不到文件而崩溃（例如：Default property warnings and errors:Error: COD Constructor (MyGameMode): Failed to find /Game/MyProject/MyBP.MyBP）
  这是因为UE4资源路径的一个规范问题，解决办法有两种：
  在copy reference出来的文件路径后面加_C，例如："Blueprint'/Game/Blueprints/MyBP.MyBP_C'"（_C可以理解为获取Class的意思）。
  去掉路径前缀，例如："/Game/Blueprints/MyBP"
  ```

  ``` c++
  //FObjectFinder<T>：一般用来加载非蓝图资源，比如StaticMesh、Material、SoundWave、ParticlesSystem、AnimSequence、SkeletalMesh等资源
  	static ConstructorHelpers::FObjectFinder<UTexture2D> ObjectFinder(TEXT("Texture2D'/Game/Textures/tex1.tex1'"));
  	UTexture2D* Texture2D = ObjectFinder.Object;
  //FClassFinder<T>：一般用来加载蓝图资源并获取蓝图Class。这是因为如果C++要用蓝图创建对象，必须先获取蓝图的Class，然后再通过Class生成蓝图对象
      static ConstructorHelpers::FClassFinder<AActor> BPClassFinder(TEXT("/Game/Blueprints/MyBP"));
      TSubclassOf<AActor> BPClass = BPClassFinder.Class;
      //...//利用Class生成蓝图对象
  ```

  * 示例

    ``` c++
    //错误:
    static ConstructorHelpers::FClassFinder<AActor> UnitSelector(TEXT("Blueprint'/Game/MyProject/MyBlueprint.MyBlueprint'"));   
    TSubclassOf<AActor> UnitSelectorClass = UnitSelector.Class;  
    
    //修正1：在copy reference出来的文件路径后面加_C
    static ConstructorHelpers::FClassFinder<AActor> UnitSelector(TEXT("Blueprint'/Game/Blueprints/MyBlueprint.MyBlueprint_C'"));   
    TSubclassOf<AActor> UnitSelectorClass = UnitSelector.Class;  
    
    //修正2：去掉路径前缀 和 后缀
    static ConstructorHelpers::FClassFinder<AActor> UnitSelector(TEXT("/Game/Blueprints/MyBlueprint"));   
    TSubclassOf<AActor> UnitSelectorClass = UnitSelector.Class;  
    
    //注意：
    /*
    另外注意：FClassFinder<T>的模版名称，不能直接写UBlueprint，例如：FClassFinder<UBlueprint>是错误的。
    创建蓝图时选择的是什么父类，则写对应的父类名，假如是Actor，那么要写成：FClassFinder<AActor>，否则无法加载成功。
    */
    
    //使用TSubclassOf<T>时模板名必须相同， FClassFinder<T>()函数中的模版名必须和TSubclassOf<T>变量的模版名一样
    static ConstructorHelpers::FClassFinder<UUserWidget> TestBP(TEXT("/Game/Blueprints/MyWidget_BP"));   
    TSubclassOf<UUserWidget> MyWidgetClass = TestBP.Class;  
    
    //可以用UClass* 替换 TSubclassOf<T>
    static ConstructorHelpers::FClassFinder<UUserWidget> TestBP(TEXT("/Game/Blueprints/MyWidget_BP"));   
    UClass* MyWidgetClass = TestBP.Class;  
    
    //用FObjectFinder()获取Class  区分：行1
    static ConstructorHelpers::FObjectFinder<UBlueprint> UnitSelector(TEXT("Blueprint'/Game/MyProject/MyBlueprint.MyBlueprint'"));   
    TSubclassOf<AActor> UnitSelectorClass = (UClass*)UnitSelector.Object->GeneratedClass;  
    ```

    

* UE4 Android apk size > 2GB

  ``` text
  解决1：
  Packaging - 高级 - Create compressed cooked packages
  ```

* UE4 iOS ipa : per pak size > 2GB

  ``` text
  iPhonePackager.cs / exe
  
  
  
  ```

  

  [在Linux和Windows平台上操作MemoryMappedFile(简称MMF)](https://www.cnblogs.com/shanyou/p/3353622.html)

  ``` text
  c#中 byte[] 不能大于2GB  int32.MaxSize
  会增加out of memory的风险
  ```

  

---



### 资源处理

* Cook

  ``` text
  UE4是跨平台的引擎，导入资源到UE4中时，是以uasset格式存储的
  这个格式无法被其他平台读取
  打包前需要把这个通用平台格式转换成对应平台格式（或者是性能更好的格式）
  
  Cook是一种转换的过程
  ```

  





---



### 热更新

* 热更新关键

  ``` text
  基于Pak的更新
  	资源存储在Pak中
  	可控的挂载Pak的时机(通过Mount方式挂载到游戏中)
  	Pak可以设定优先级
  
  UFS读取文件具有优先级(PakOrder)
  默认读取Order最大的Pak中的文件
  
  ==> 保证了热更新的基础
  ```

* 资源管理

  ``` text
  pak中包含
  
  Cooked uasset
  Slate资源				 窗体
  Internationalization  国际化（多语言）
  .uproject / .uplugin  包含插件 / 包含插件中的模块
  Config				  INI文件
  AssetRegistry.bin     资产注册表（游戏中包含的资源及其引用关系）
  ushaderbytecode		  （需要开启共享材质ShaderCode才会有   ProjectSetting -> Packaging -> Share Material Shader Code）
  shadercache			  （需要开启共享材质ShaderCode才会有） 
  添加的Non-Asset文件     lua / .db等数据类文件
  
  
  推荐热更新内容：
  uasset
  Internationalization
  AssetRegistry.bin
  Shaderbytecode
  Shadercache
  外部文件(lua等)
  ```
  
* Pak打包

  ``` text
  收集要打包的资源及外部文件
  cook uasset
  存储打包的Response文件（PakList*.txt）
  使用UnrealPak执行打包
  
  ResponseFile格式：
  文件绝对路径 + Mount之后的路径 + 打包参数
  如：
  "F:\_Dev\projects\Tenmove_Project_A8_trunk\Client\NextGenGame\Saved\Cooked\IOS\NextGenActionGame\Content\Resources\Scenes\Chapter\Chapter_A\Styles\Style01\Material\MI_a1sbcj_jianshiqi01_05.uexp" "../../../NextGenActionGame/Content/Resources/Scenes/Chapter/Chapter_A/Styles/Style01/Material/MI_a1sbcj_jianshiqi01_05.uexp"
  
  UnrealPak打包命令：
  UnrealPak.exe SAVE_PAK.pak -create=RESPONSE_FILE.txt -compress
  
  
  
  手动流程：
  1.需要分析uasset资源的依赖
  2.对uasset资源进行cook
  3.添加需要打包的Non-Asset文件
  4.编辑ResponseFile文件
  5.执行UnrealPak命令
  6.对每个平台都执行一遍上述流程
  
  
  
  pak的打包粒度：（根据不同项目使用）
  1.整个patch为单个pak
  2.根据地图为patch打包
  3.根据资源分类patch打包
  
  
  基础包更新后，先前热更的pak处理方式：
  1.基础包只包含基础的地图
  2.其他地图（及其依赖的资源）打成单独pak文件
  3.更新基础包1.0，并热更其他地图pak
  4.基础包2.0，如果需要覆盖之前热更的pak，则设置新的pak的order大于原先热更的pak
  （这样会优先查找基础包2.0里的pak，如果存在，则不会去查找其他pak了）
  5.比对最新基础包2.0里的资源和之前热更的资源，和当前游戏工程的资源做对比，把差异部分打包下载下来
  
  
  ```
  
  

---



