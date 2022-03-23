// Fill out your copyright notice in the Description page of Project Settings.


#include "Sample_1.h"

#if !PLATFORM_IOS
#include "MessageLog\Public\MessageLogModule.h"
#endif
//#include <Runtime/Networking/Public/Common/UdpSocketReceiver.h>

//#include "Interfaces/IPluginManager.h"   只能在编辑器/引擎 使用

#include "Common/LogInit.h"


#include <cstdlib>

DEFINE_LOG_CATEGORY(LogTMSDKSample);
#define LOCTEXT_NAMESPACE "TM SDK"

Sample_1::Sample_1()
{
	/*----------------------------------日志 start-----------------------------------*/
	//普通日志，输出到Output窗口
	// 	
	// 
	// 	   注意：错误日志
	//UE_LOG(LogTemp, Error, TEXT("Sample_1"));
	UE_LOG(LogTemp, Warning, TEXT("Sample_1"));
	UE_LOG(LogTemp, Log, TEXT("Sample_1"));

	UE_LOG(LogTemp, Log, TEXT("Sample_%d"),1);

	UE_LOG(LogTMSDKSample, Warning, TEXT("Sample_1"));
	//UE_LOG(TMSDKSample, Error, TEXT("Sample_1 -- 中文输出"));


	//显示到屏幕  不能写在构造函数里 重启后打不开editor
	//GEngine->AddOnScreenDebugMessage(-1, 5.f, FColor::Green, FString::Printf(TEXT("DLL_Init")));


#if !PLATFORM_IOS && !UE_BUILD_SHIPPING
	//消息日志 创建分组
	FMessageLogModule& messageLogModule = FModuleManager::LoadModuleChecked<FMessageLogModule>("MessageLog");
	FMessageLogInitializationOptions initOptions;
	initOptions.bShowFilters = true;                    //开启过滤器
	initOptions.bShowPages = true;						//开启分页  注：设置为true时，ClearLog按钮会消失，不管bAllowClear是否为true
	messageLogModule.RegisterLogListing("TM_SDK", LOCTEXT("TM_SDK","TM SDK"), initOptions);
#endif

	//自定义日志结构
	//LogError("TM LOG_ERROR : test error");
	LogDebug("TM LOG_DEBUG : test debug");
	LogInfo("TM LOG_INFO : test info");
	LogWarn("TM LOG_WARN : test warn");
	//LogFatal("TM LOG_FATAL : test fatal");

	//消息日志
	//FMessageLog("TM_SDK").Warning(FText::FromString(TEXT("这是TM SDK的消息日志")));
	//FMessageLog("TM_SDK").Notify();//右下角消息框
	//FMessageLog("TM_SDK").Open();  //打开消息日志窗口 并定位到日志组

	//分页  可用 打包注释了...
	//FMessageLog("TM_SDK").NewPage(FText::FromString(TEXT("Page1")));
	//FMessageLog("TM_SDK").Message(EMessageSeverity::Type::Error, FText::FromString(TEXT("这是TM SDK的消息日志1")));

	//FMessageLog("TM_SDK").NewPage(FText::FromString(TEXT("Page2")));
	//FMessageLog("TM_SDK").Info(FText::FromString(TEXT("这是TM SDK的消息日志2")));
	/*----------------------------------日志 end-----------------------------------*/


	/*----------------------------------字符 start---------------------------------*/
	/*
	UE4 关于字符串的处理 https://al3ix.com/UE4-%E5%85%B3%E4%BA%8E%E5%AD%97%E7%AC%A6%E4%B8%B2%E7%9A%84%E5%A4%84%E7%90%86.html
	UE4 FString 常用API https://dawnarc.com/2016/03/ue4fstring%E5%B8%B8%E7%94%A8api/

	windows:
	char 单字符 占用一个字节 对应字符集是扩展的 ANSI
	wchar_t 宽字符 占用两个字节  UTF-16每一个字符都需要两个字节，用wchar_t表示
	TCHAR  是通过宏对 char 和 wchar_t 的封装  可以根据当前平台 选择对应类型
	同理：_T修饰字符串常量 根据是否定义的UNICODE宏 表示" "或者L" "


	UE4中的所有字符串都作为FStrings或TCHAR数组以UTF-16 格式存储在内存中
	大多数代码假设2个字节等于一个代码点，因此只支持基本多文种平面（BMP），这样虚幻内部编码可以更准确地描述为UCS-2。
	字符串以适合于当前平台的字节次序存储。
	当向/从磁盘序列化到程序包，或在联网期间序列化时，TCHAR字符小于0xff的字符串均存储为一串8位字节，否则存储为双字节UTF-16字符串。
	序列化代码可以根据需要处理任何字节次序转换。


	设置字符串变量文字时应使用 TEXT() 宏。
	如未指定 TEXT() 宏，将使用 ANSI 对文字进行编码，会导致支持字符高度受限。
	传入 FString 的 ANSI 文字需要完成到 TCHAR 的转换（本地万国码编码），以便更高效地使用 TEXT()。


	UE4的宏转换：
	这些宏可以将字符串转换为各种编码或从各种编码转换字符串。
	这些宏使用局部范围内声明的类实例，在堆栈上分配空间，因此保留指向这些宏的指针非常重要！
	它们仅用于将字符串传递给函数调用。
	TCHAR_TO_ANSI(str)
	TCHAR_TO_OEM(str)
	ANSI_TO_TCHAR(str)
	TCHAR_TO_UTF8(str)
	UTF8_TO_TCHAR(str)

	这里如果使用带中文的字符串转换成ANCI字符串，输出会出现乱码的情况。
	*/


	/*
	* not support android clang 
	* ISO C++11 does not allow conversion from string literal to 'wchar_t *' [-Werror,-Wwritable-strings]
	* 
	wchar_t* temp = L"新疆棉花好";
	UE_LOG(LogTemp, Log, TEXT("wchar_t content is : %s"), temp);
	TCHAR* tch = temp;
	UE_LOG(LogTemp, Log, TEXT("TCHAR content is : %s"), tch);
	FString str = temp;
	UE_LOG(LogTemp, Log, TEXT("FString content is : %s"), *str);
	*/


	//FString to FName
	FString fstr1 = "Hello";
	FName fname1 = FName(*fstr1);

	//FName to FString
	fstr1 = fname1.ToString();

	//FString to TCHAR*
	/*
	* not support android clang
	* ISO C++11 does not allow conversion from string literal to 'wchar_t *' [-Werror,-Wwritable-strings]
	*
	const TCHAR* tchar1 = *fstr1;
	const TCHAR* tchar2 = fstr1.GetCharArray().GetData();
	*/

	//FString to char*
	char* char1 = TCHAR_TO_ANSI(*fstr1);

	//FString to wchar_t*
	/*
	* not support android clang
	* ISO C++11 does not allow conversion from string literal to 'wchar_t *' [-Werror,-Wwritable-strings]
	*
	const wchar_t* wchar_t1 = *fstr1;
	int len_wchar_t1 = wcslen(wchar_t1);
	int len_wchar_t1_double = len_wchar_t1 * 2;
	//先转换为 char*, 比如 protobuf 不支持 wchar_t，那么可以将中文转换为char，然后储存在protobuf内部。
	char* buf_1 = new char[len_wchar_t1_double + 1]();
	memcpy(buf_1, wchar_t1, len_wchar_t1_double);
	//验证char* 能否还原为 wchar_t数组
	wchar_t* buf_2 = new wchar_t[len_wchar_t1_double/2 + 1]();
	memcpy(buf_2, buf_1, len_wchar_t1_double);
	*/

	//const char* to FString
	const char* char2 = "HW";
	FString fstr111(char2);

	//FString to FText
	FString fstr2 = TEXT("World");
	FText ftxt1 = FText::FromString(fstr2);

	//FText to FString
	FString fstr5 = ftxt1.ToString();

	//std::string to FString
	std::string stdStr1 = "Happy";
	//FString fstr3(stdStr1.c_str());
    FString fstr3(UTF8_TO_TCHAR(stdStr1.c_str()));  //这样能支持中文

	//FString to std::string
	FString fstr4 = "Time";
	std::string stdStr2(TCHAR_TO_UTF8(*fstr4));
	std::string stdStr3 = TCHAR_TO_UTF8(*fstr4);

	//FText to FName
	//先转成FString，再转成FName
	FName fname2 = FName(*(ftxt1.ToString()));

	//FName to FText
	FText ftxt2 = FText::FromName(fname2);

	//FString to Integer
	FString fstr6 = "1110.1101";
	int32 int1 = FCString::Atoi(*fstr6);
	//FString to Float
	float f1 = FCString::Atof(*fstr6);
	//Float/Int to FString
	FString fstr7 = FString::FromInt(int1);
	FString fstr8 = FString::SanitizeFloat(f1);


	//FArrayReaderPtr to FString
	//uint8 data1[512];
	//FMemory::Memzero(data1, 512);
	//FArrayReaderPtr arrayReaderPtr;
	//FMemory::Memcpy(data1, arrayReaderPtr->GetData(), arrayReaderPtr->Num());
	//FString fstr_1 = ((const char*)data1);

	//Array<uint8> to FString
	TArray<uint8> array1;
	const std::string stdStr(reinterpret_cast<const char*>(array1.GetData()), array1.Num());
	FString fstr_2 = stdStr.c_str();

	//FString to Array<uint8>
	FString fstr_3;
	TArray<uint8> array2;
	array2.SetNum(fstr_3.Len());
	memcpy(array2.GetData(), TCHAR_TO_ANSI(*fstr_3), fstr_3.Len());


	FString fstr_4;
	const TCHAR* tchar_1 = *fstr_4;
	FTCHARToUTF8 utf8Str_1(tchar_1);
	int32 size_1 = utf8Str_1.Length();
	TArray<uint8> array3;
	array3.SetNum(size_1);
	memcpy(array3.GetData(), utf8Str_1.Get(), size_1);


	//char array to FString
	char carr_1[] = "Hello World";
	FString fstr_5 = FString(ANSI_TO_TCHAR(carr_1));
	FString fstr_55 = FString(UTF8_TO_TCHAR(carr_1));

	
	// +
	FString Text1 = TEXT("Text") + FString::FormatAsNumber(1);

	//字符串分割
	FString fstr_6("1, 2, 3");
	TArray<FString> fstr_array1;
	fstr_6.ParseIntoArray(fstr_array1, TEXT(","), false);

	//字符串截取
	FString fstr_7("Hello World");
	fstr_7.Left(1);
	fstr_7.Right(2);

	//字符串格式化
	FString fstr_8 = FString::Printf(TEXT("StaticMesh'/Game/Shapes/ground_shape_%d.ground_shape_%d'"), 1, 1);
	UE_LOG(LogTemp, Warning, TEXT("FString format content is : %s"), *fstr_8);

	//FString format
	int32 i = 1;
	FString ffstr1 = FString::Printf(TEXT("text%d"), i);

	//FString TArray format
	//https ://blog.csdn.net/qq_20309931/article/details/52910467
	TArray<FStringFormatArg> fformatArray1;
	fformatArray1.Add(FStringFormatArg(i));
	FString ffstr2 = FString::Format(TEXT("Text{0}"), fformatArray1);

	TArray<FStringFormatArg> fformatArray2;
	fformatArray2.Add(FStringFormatArg(1));
	fformatArray2.Add(FStringFormatArg(2));
	FString ffstr3 = FString::Format(TEXT("Text{0}{1}{0}"), fformatArray2);

	//FString TMap format
	TMap<FString, FStringFormatArg> fformatMap1;
	fformatMap1.Add(TEXT("key1"), FStringFormatArg(1));
	FString ffstr4 = FString::Format(TEXT("Text{key1}"), fformatMap1);

	TMap<FString, FStringFormatArg> fformatMap2;
	fformatMap2.Add(TEXT("key1"), FStringFormatArg(1));
	fformatMap2.Add(TEXT("key2"), FStringFormatArg(2));
	FString ffstr5 = FString::Format(TEXT("Text{key1}{key2}{key1}"), fformatMap2);

	//FString to char 后的长度
	/*
	* 这些宏声明的对象的生存期很短。它们的预期用例是作为函数参数，它们适合于这种情况。
	不要将变量赋值给转换后的字符串的内容，因为对象将超出作用域，字符串将被释放。
	如果你的代码继续访问指向已释放内存的指针，这可能会导致崩溃。
	*/
	FString fstr_9("TO CHAR");
	//FTCHARToANSI convert_1(*String)
	//Ar->Serialize((ANSICHAR*)Convert, Convert.Length());  // FTCHARToANSI::Length() returns the number of bytes for the encoded string, excluding the null terminator.


	/*** TCHAR*、wchar_t*、char* 转换相关 ***/
	//char 转换为 TCHAR
	TCHAR* tchar_s1 = ANSI_TO_TCHAR("dddd");
	TCHAR* tchar_s2 = UTF8_TO_TCHAR("dddd");
	//TCHAR 转换为 char
	//char* char_s1 = TCHAR_TO_UTF8(*tchar_s1);
	//char* char_s1 = TCHAR_TO_ANSI(*tchar_s1);
	//char* char_s1 = TCHAR_TO_UTF8(*tchar_s2);

	FString fstr_s1;
	char* char_s2 = TCHAR_TO_ANSI(*fstr_s1);

	/*
	* not support android clang
	* ISO C++11 does not allow conversion from string literal to 'wchar_t *' [-Werror,-Wwritable-strings]
	*
	//TCHAR 转换为 wchar_t
	FString fstr_s2;
	wchar_t* wchar_t_s1 = TCHAR_TO_WCHAR(*fstr_s2);
	//wchar_t 转换为 TCHAR 
	wchar_t wchar_t_s2[] = L"sss";
	TCHAR* tchar_s3 = (TCHAR*)wchar_t_s2;
	*/

	/*
	如果char* 字符串有中文，转换为FString时，必须使用ANSI_TO_TCHAR 或者 UTF8_TO_TCHAR()
	如果中文在代码中，则代码文件编码格式需要改成UTF8
	*/
	FString Text = UTF8_TO_TCHAR("中国");



	/*中文乱码：
	
	代码文件中写了显示的中文字符串常量，可能切换引擎版本时会出现显示乱码
	
	引擎使用不同的编码格式读取

	一般解决方法：将代码文件的编码格式GB2312修改为UTF8
	*/

	//测试 TCHAR_TO_UTF8 vs. UTF8_TO_TCHAR
	FString versionName = "1.0.1.0";
	GConfig->GetString(
		TEXT("/Script/AndroidRuntimeSettings.AndroidRuntimeSettings"),
		TEXT("VersionDisplayName"),
		versionName,
		GEngineIni   //FPaths::ProjectConfigDir() / "DefaultEngine.ini" also
	);
	const char* ip = TCHAR_TO_ANSI(*versionName);
	UE_LOG(LogTemp, Log, TEXT("version name is %s"),ip);


	FString versionNameStr = versionName;
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

	/*
	android  build:
	NDK toolchain: r21b, NDK version: 29, GccVersion: 4.9, ClangVersion: 9.0.8
	Compiling Native 32-bit code with NDK API 'android-29'

2021-07-07 11:39:04.605 25343-25393/com.tenmove.a8.dev D/UE4: [2021.07.07-03.39.04:605][  0]LogTemp: ### version name: 1.0.1.1002 --- 1.0.1.1002
2021-07-07 11:39:04.605 25343-25393/com.tenmove.a8.dev D/UE4: [2021.07.07-03.39.04:605][  0]LogTemp: ### version name: 1.0.1.1002 --- 1.0.1.1002
2021-07-07 11:39:04.605 25343-25393/com.tenmove.a8.dev D/UE4: [2021.07.07-03.39.04:605][  0]LogTemp: ### version name: 1 --- 1.0.1.1002
2021-07-07 11:39:04.605 25343-25393/com.tenmove.a8.dev D/UE4: [2021.07.07-03.39.04:605][  0]LogTemp: ### version name: 1 --- 1.0.1.1002
2021-07-07 11:39:04.606 25343-25393/com.tenmove.a8.dev D/UE4: [2021.07.07-03.39.04:606][  0]LogTemp: ### version name: 1 --- 1.0.1.1002
	
	*/

	//const wchar_t* to const char*
	//const TCHAR* test3 = L"1.0.1.1002"; not able in clang android ndk
	//std::string vStr2 = "1.0.1.1002"; not able in clang android ndk

	//test1
	//#include <comdef.h>   ==> just for windows
	//_bstr_t b1(test3);
	//const char* c1 = b1;

	char c1_output[256];
	std::wcstombs(c1_output, (const wchar_t*)(*test2), sizeof(c1_output));
	UE_LOG(LogTemp, Log, TEXT("### %s"), UTF8_TO_TCHAR(c1_output));


	//test2
	char c2_output[256];
	//sprintf_s(c2_output, sizeof(c2_output), "%ws", test3);  
	snprintf(c2_output, sizeof(c2_output), "%ls", (const wchar_t*)(*test2));

	//sprintf_s(c2_output, sizeof(c2_output), "%ls", test3);  also able
	UE_LOG(LogTemp, Log, TEXT("### %s"), UTF8_TO_TCHAR(c2_output));



	//中文字符串打印乱码
	
	//FString fstr_11 = L"中文"; not able in clang android ndk
	FString fstr_12 = "中文";
	FString fstr_13= TEXT("中文");
	//UE_LOG(LogTemp, Log, TEXT("%s"), *fstr_11);
	UE_LOG(LogTemp, Log, TEXT("%s"), *fstr_12);  //???
	UE_LOG(LogTemp, Log, TEXT("%s"), *fstr_13);  //中文

	std::string cstr_11 = "中文";
	FString fstr_14 = FString(cstr_11.c_str());
	UE_LOG(LogTemp, Log, TEXT("%s"), *fstr_14);  //???

	/*
	std::string 可以认为是 char[]
	char占一个字节
	中文字符根据平台不同，一般占用两个字节  UTF-8占三到四个字节
	超出存储范围 导致乱码
	*/

	std::wstring cwstr_11 = L"中文";
	//wchar_t* wchar_11 = L"中文";  not able in clang android ndk

	//const wchar_t* <==>  std::string
	const wchar_t* cwchar_11 = cwstr_11.c_str();
	
	//TCHAR 是 wchar_t 的别名
	//TCHAR* tchar_11 = wchar_11; not able in clang android ndk

	//const wchar_t* ==> <const_cast> ==> wchar_t*
	//TCHAR* tchar_12 = const_cast<TCHAR*>(cwstr_11.c_str());  not able in clang android ndk

	//TCHAR* ==> FString	
	//FString fstr_15 = tchar_12; not able in clang android ndk

	//const wchar_t* / const TCHAR* ==> FString
	//FString fstr_16 = cwchar_11; not able in clang android ndk

	//需要处理其他平台传来的std::string 带中文的字符串
	////windows平台std::string使用utf-8编码格式,可以使用ue提供的UTF8_TO_TCHAR宏将std::string转换成ue支持的TCHAR
	//作者：埃罗芒阿Sensal https ://www.bilibili.com/read/cv9487608 出处：bilibili
	FString fstr_17 = FString(UTF8_TO_TCHAR(cwchar_11));
	UE_LOG(LogTemp, Log, TEXT("%s"), *fstr_17);				//-N?e


	//--------------推荐1
	//TCHAR* tchar_2 = const_cast<wchar_t*>(cwchar_11);   //android compile error:  cannot initialize a variable of type 'TCHAR *' (aka 'char16_t *') with an rvalue of type 
	wchar_t* tchar_2 = const_cast<wchar_t*>(cwchar_11);
	fstr_17 = FString(UTF8_TO_TCHAR(tchar_2));
	UE_LOG(LogTemp, Log, TEXT("%s"), *fstr_17);				//中文

	//FString to std::string
	//std::string cwstr_12 = TCHAR_TO_UTF8(*fstr_16);

	//std::string 不能存中文 但是可以存中文的数据  打印或者断点时 数据为乱码
	//可以转成std::wstring


	//-------------推荐2
	//其他系统传递来的std::string 字符串 带有中文
	std::string cstr_12 = "中文";
	FString fstr_18 = FString(UTF8_TO_TCHAR(cstr_12.c_str()));
	UE_LOG(LogTemp, Log, TEXT("### %s"), *fstr_18);			//中文


	/*----------------------------------字符 end---------------------------------*/

}

/*
#include <Windows.h>
//作者：埃罗芒阿Sensal https ://www.bilibili.com/read/cv9487608 出处：bilibili
std::wstring StringToWString(const std::string& str)
{
	//定义一个空的std::wstring
	std::wstring wstr = L"";

	// CodePage:该参数决定执行转换时使用的编码格式,本机使用编码格式为UTF8,所以使用CP_UTF8.
	// dwFlags:使用0即可
	// lpMultiByteStr:要转换的字符串指针,使用c_str()即可获得
	// cbMultiByte:要转换的字符串的长度,字节为单位,可以使用size()获得
	// lpWideCharStr:指向接收转换后字符串的缓冲区的指针
	// cchWideChar:缓冲区大小,如果为0,则返回所需要的缓冲区大小
	//详见https://docs.microsoft.com/zh-cn/windows/win32/api/stringapiset/nf-stringapiset-multibytetowidechar?redirectedfrom=MSDN

	 //lpWideCharStr设为NULL,cchWideChar设为0,该步骤用于获取缓冲区大小
	int len = MultiByteToWideChar(CP_UTF8, 0, str.c_str(), str.size(), NULL, 0);

	//创建wchar_t数组作为缓冲区,用于接收转换出来的内容,数组长度为len+1的原因是字符串需要以'\0'结尾,所以+1用来存储'\0'
	wchar_t* wchar = new wchar_t[len + 1];

	//缓冲区和所需缓冲区大小都已确定,执行转换
	MultiByteToWideChar(CP_UTF8, 0, str.c_str(), str.size(), wchar, len);

	//使用'\0'结尾
	wchar[len] = '\0';

	//缓冲区拼接到std::wstring
	wstr.append(wchar);

	//切记要清空缓冲区,否则内存泄漏
	delete[]wchar;
	return wstr;
} 

std::string WStringToString(const std::wstring wstr)
{
	std::string str;

	int len = WideCharToMultiByte(CP_UTF8, 0, wstr.c_str(), wstr.size(), NULL, 0, NULL, NULL);

	char* buffer = new char[len + 1];

	WideCharToMultiByte(CP_UTF8, 0, wstr.c_str(), wstr.size(), buffer, len, NULL, NULL);

	buffer[len] = '\0';

	str.append(buffer);

	delete[]buffer;

	return str;
}
*/

/*
4.20之前，wchar_t* 和 TCHAR* 之前直接强转没什么问题，
但是4.20版本开始，直接强转在android版本下只会截取字符串的第一个字母，之后的内容都丢失。
Windows版本没这个问题。

解决办法：
使用TCHAR_TO_WCHAR和WCHAR_TO_TCHAR转换，而不要直接强制转换。
强制转换在Windwos版本下没有问题，android就会出现数据错误。

使用WCHAR_TO_TCHAR在android下会编译错误
UATHelper: Packaging (Android (ASTC)):
UE_4.21/Engine/Source/Runtime/Core/Public/Misc/CString.h(658,9) :  error: no matching function for call to 'Strlen'
*/
/*
临时解决方法：
[[UE4]FString常用API](https://dawnarc.com/2016/03/ue4fstring%E5%B8%B8%E7%94%A8api/)
bool UMyStringUtil::WCharToTChar(const wchar_t* Src, TCHAR* Dest, int DestLen)
{
	bool Ret = true;
	if (!Src)
	{
		Ret = false;
	}
	else
	{
		int SrcLen = wcslen(Src) + 1;

		if (SrcLen == 1)
		{
			Ret = false;
		}
		else
		{
			int MinLen = SrcLen < DestLen ? SrcLen : DestLen;
			for (int i = 0; i < MinLen; i++)
			{
				Dest[i] = Src[i];
			}
		}
	}
	return Ret;
}

size_t StrLen = wcslen(MyWcharArray) + 1;
TCHAR *pData = new TCHAR[StrLen];
FMemory::Memset(pData, 0, StrLen);
WCharToTChar(MyWcharArray, pData, StrLen);
delete[] pData;
*/

Sample_1::~Sample_1()
{
}

void Sample_1::Init()
{
	//不能调用
	//TSharedPtr<IPlugin> buglyP = IPluginManager::Get().FindPlugin(TEXT("Bugly"));
	//if (buglyP.IsValid()) {
	//	FString buglyEnableStr = buglyP->IsEnabled() ? "enabled" : "not enabled";
	//	UE_LOG(LogTemp, Warning, TEXT("bugly plugin is %s"), &buglyEnableStr);
	//}
}