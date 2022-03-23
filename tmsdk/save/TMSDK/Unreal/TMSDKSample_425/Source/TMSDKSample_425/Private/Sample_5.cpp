// Fill out your copyright notice in the Description page of Project Settings.


#include "Sample_5.h"
#include "HAL/FileManagerGeneric.h"
#include "Misc/FileHelper.h"
#include "Misc/Paths.h"

Sample_5::Sample_5()
{
	//Save Dir
#if PLATFORM_ANDROID
	extern FString GFilePathBase;
	extern FString GInternalFilePath;
	extern FString GAPKFilename;
	UE_LOG(LogTemp, Warning, TEXT("apk GFilePathBase %s"), *GFilePathBase);								/* Dev: /storage/emulated/0   Shipping:  */
	UE_LOG(LogTemp, Warning, TEXT("apk GInternalFilePath %s"), *GInternalFilePath);						/* /data/user/0/com.tm.sdk.mg/files */
	UE_LOG(LogTemp, Warning, TEXT("apk GAPKFilename %s"), *GAPKFilename);								/* /data/app/com.tm.sdk.mg-0qo3AcPkxer2VKGGR0EReA==/base.apk */

	FAndroidMisc::LocalPrint(UTF8_TO_TCHAR("### start log"));
	FAndroidMisc::LocalPrint(*GFilePathBase);
	FAndroidMisc::LocalPrint(*GInternalFilePath);
	FAndroidMisc::LocalPrint(*GAPKFilename);
	FAndroidMisc::LocalPrint(UTF8_TO_TCHAR("### end log"));

	FString saveRootDir = AndroidRelativeToAbsolutePath_TM(FPaths::ProjectSavedDir());
#elif PLATFORM_IOS
	FString saveRootDir = IFileManager::Get().ConvertToAbsolutePathForExternalAppForRead(*FPaths::ProjectSavedDir());
#else
	FString saveRootDir = FPaths::ProjectSavedDir();                   //windows也可以使用  IFileManager::Get().ConvertToAbsolutePathForExternalAppForRead
#endif
	//FString Path override /  ===>   
	FString infoSavePath_rel(TEXT("test_save_dir"));
	FString infoSavePath = saveRootDir / infoSavePath_rel;

	UE_LOG(LogTemp, Log, TEXT("### TEST Save Dir: %s, %s"), *saveRootDir, *infoSavePath);

	//确保目录存在
	FPlatformFileManager::Get().GetPlatformFile().CreateDirectoryTree(*infoSavePath);
	//拷贝文件到目录
	if (FFileManagerGeneric::Get().FileExists(TEXT("test.json")))
	{
		FFileManagerGeneric::Get().Copy(*(infoSavePath / FString(TEXT("test.json"))), TEXT("test_dir/test.json"));
	}

	//读取文件 各平台不同
	FString testFileRelPath = TEXT("test.json");
	FString testFileAbsPath = FPaths::ConvertRelativePathToFull(*testFileRelPath);  //windows下转为引擎目录了：F:/_Dev/programs/EpicGames/UE_4.25/Engine/Binaries/Win64/test.json
	FString readFilePath;
#if PLATFORM_ANDROID
	readFilePath = testFileRelPath;						//android下为 沙盒根目录（Engine 和 项目名文件夹 同级）
#elif PLATFORM_IOS
	readFilePath = testFileRelPath;						//ios下为 沙盒根目录（Engine 和 项目名文件夹 同级）
#else
	readFilePath = testFileAbsPath;
#endif

	UE_LOG(LogTemp, Log, TEXT("### Load File Path: %s， abs path: %s"),*readFilePath, *testFileAbsPath);

	FString fstream;
	//判断文件是否存在
	if (IFileManager::Get().FileExists(*readFilePath))
	{
		//获取文件大小
		IFileManager::Get().FileSize(*readFilePath);

		//读取文件
		TArray<uint8> data;
		if (FFileHelper::LoadFileToArray(data, *readFilePath, 0))
		{
			//UE_LOG(LogTemp, Log, TEXT("### Load File, data size: %d"), (int)(data.ArrayNum));
		}
		if (FFileHelper::LoadFileToString(fstream, *readFilePath))
		{
			UE_LOG(LogTemp, Log, TEXT("### Load File, data str: %s"),*fstream);
		}
	}

	//写入文件
	fstream += "+1";
	FFileHelper::SaveStringToFile(fstream, *readFilePath);
	//对文本逐字符处理 存入OutArray
	TCHAR* txtData = fstream.GetCharArray().GetData();
	for (int i = 0; i < fstream.Len(); i++) {}


}

Sample_5::~Sample_5()
{
}

void Sample_5::Test1()
{
	// 常用路径获取接口
	UE_LOG(LogTemp, Display, TEXT("### EngineDir: %s"), *FPaths::EngineDir());
	UE_LOG(LogTemp, Display, TEXT("### EngineSavedDir: %s"), *FPaths::EngineSavedDir());
	UE_LOG(LogTemp, Display, TEXT("### EngineIntermediateDir: %s"), *FPaths::EngineIntermediateDir());

	//Root Dir  ==> 引擎根目录
	UE_LOG(LogTemp, Log, TEXT("### RootDir: %s"), *FPaths::RootDir());
	UE_LOG(LogTemp, Log, TEXT("### RootDir: %s"), *(FPaths::ConvertRelativePathToFull(FPaths::RootDir())));

	//Save Dir  ==>  引擎Binaries/Win64
	UE_LOG(LogTemp, Log, TEXT("### BaseDir: %s"), FPlatformProcess::BaseDir());
	UE_LOG(LogTemp, Log, TEXT("### Binaries Sub Dir: %s"), FPlatformProcess::GetBinariesSubdirectory());


	UE_LOG(LogTemp, Display, TEXT("### ProjectDir: %s"), *FPaths::ProjectDir());
	UE_LOG(LogTemp, Display, TEXT("### ProjectContentDir: %s"), *FPaths::ProjectContentDir());
	UE_LOG(LogTemp, Display, TEXT("### ProjectConfigDir: %s"), *FPaths::ProjectConfigDir());

	UE_LOG(LogTemp, Log, TEXT("### ProjectPluginsDir Relative: %s"), *(FPaths::ProjectPluginsDir()));
	UE_LOG(LogTemp, Log, TEXT("### ProjectPluginsDir Full: %s"), *(FPaths::ConvertRelativePathToFull(FPaths::ProjectPluginsDir())));

	UE_LOG(LogTemp, Display, TEXT("### ProjectIntermediateDir: %s"), *FPaths::ProjectIntermediateDir());

	UE_LOG(LogTemp, Display, TEXT("### ProjectSavedDir: %s"), *FPaths::ProjectSavedDir());
	UE_LOG(LogTemp, Display, TEXT("### ProjectSavedDir Full: %s"), *(FPaths::ConvertRelativePathToFull(FPaths::ProjectSavedDir())));

	FString TestFilename(TEXT("ParentDirectory/Directory/FileName.extion"));
	FString Extension = FPaths::GetExtension(TestFilename);
	FString BaseFilename = FPaths::GetBaseFilename(TestFilename);
	FString CleanFilename = FPaths::GetCleanFilename(TestFilename);
	FString Directory = FPaths::GetPath(TestFilename);
	bool bFileExists = FPaths::FileExists(TestFilename);
	bool bDirectoryExists = FPaths::DirectoryExists(Directory);

	UE_LOG(LogTemp, Display, TEXT("### TestFilename: %s"), *TestFilename);
	// 获取文件扩展名
	UE_LOG(LogTemp, Display, TEXT("### Extension: %s"), *Extension);
	// 获取文件名，不带扩展名
	UE_LOG(LogTemp, Display, TEXT("### BaseFilename: %s"), *BaseFilename);
	// 获取文件名，带扩展名
	UE_LOG(LogTemp, Display, TEXT("### CleanFilename: %s"), *CleanFilename);
	// 获取路径文件夹，去除CleanFilename后的路径
	UE_LOG(LogTemp, Display, TEXT("### Directory: %s"), *Directory);
	// 检测文件是否存在
	UE_LOG(LogTemp, Display, TEXT("### FileExists: %s"), bFileExists ? TEXT("True") : TEXT("False"));
	// 检测文件夹是否存在
	UE_LOG(LogTemp, Display, TEXT("### DirectoryExists: %s"), bDirectoryExists ? TEXT("True") : TEXT("False"));


	//其他
	FString testLeafPath = FPaths::GetPathLeaf(TestFilename);
	UE_LOG(LogTemp, Display, TEXT("### Test Leaf Path: %s"), *testLeafPath);

	// 路径拼接
	FString NewFilePath = FPaths::Combine(Directory, TEXT("NewFilePath.txt"));
	// 简便写法
	FString NewFilePathEasy = Directory / TEXT("NewFilePath.txt");
	UE_LOG(LogTemp, Display, TEXT("### NewFilePath: %s"), *NewFilePath);
	UE_LOG(LogTemp, Display, TEXT("### NewFilePathEasy: %s"), *NewFilePathEasy);

	// 相对路径转换为绝对路径
	FString FullPath = FPaths::ConvertRelativePathToFull(FPaths::EngineDir());
	UE_LOG(LogTemp, Display, TEXT("### FullPath: %s"), *FullPath);

	// 绝对路径转相对路径
	FString saveRootWritePath = IFileManager::Get().ConvertToAbsolutePathForExternalAppForWrite(*FPaths::ProjectSavedDir());
	FString saveRootReadPath = IFileManager::Get().ConvertToAbsolutePathForExternalAppForRead(*FPaths::ProjectSavedDir());
	UE_LOG(LogTemp, Display, TEXT("### Save dir absolute for external app for write: %s"), *saveRootWritePath);
	UE_LOG(LogTemp, Display, TEXT("### Save dir absolute for external app for read: %s"), *saveRootReadPath);
	//@注意：
	/* android输出的 均为 ../../../工程名/Saved/  即Android端不支持 需要使用其他路径获取方式 */
	/* ios 通过ConvertToAbsolutePathForExternalAppForWrite输出为： /var/mobile/Containers/Data/Application/E5F4B8E6-6A55-4757-AD34-CDC0739A9A99/Documents/TMSDKSample_425/Saved/*/
}

#if PLATFORM_ANDROID
// 源码里拷出来的
// Constructs the base path for any files which are not in OBB/pak data
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
#endif

FString GetProjectSavePath_TM()
{
#if PLATFORM_ANDROID
	FString saveRootDir = AndroidRelativeToAbsolutePath_TM(FPaths::ProjectSavedDir());
#elif PLATFORM_IOS
	FString saveRootDir = IFileManager::Get().ConvertToAbsolutePathForExternalAppForRead(*FPaths::ProjectSavedDir());
#else
	FString saveRootDir = FPaths::ConvertRelativePathToFull(FPaths::ProjectSavedDir());                   //windows也可以使用  IFileManager::Get().ConvertToAbsolutePathForExternalAppForRead
#endif
	return saveRootDir;
}