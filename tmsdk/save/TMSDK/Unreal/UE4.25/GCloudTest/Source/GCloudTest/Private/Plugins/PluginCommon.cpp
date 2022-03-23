#include "Plugins/PluginCommon.h"

#include "HAL/FileManagerGeneric.h"
#include "Misc/FileHelper.h"
#include "Misc/Paths.h"

#include <list>
#include <math.h>

FString HotfixState2Desc(TM_HotfixState state)
{
	FString desc;

	switch (state)
	{
	case TM_LocalFileCheck:
		desc = TEXT("本地文件检查中");
		break;
	case TM_GetVersionInfo:
		desc = TEXT("获取版本文件中");
		break;
	case TM_AppUpdate:
		desc = TEXT("下载应用程序中");
		break;
	case TM_AppMerge:
		desc = TEXT("合并应用程序中");
		break;
	case TM_AppCheck:
		desc = TEXT("检查应用程序中");
		break;
	case TM_ResUpdate:
		desc = TEXT("下载应用资源中");
		break;
	case TM_ResCheck:
		desc = TEXT("检查应用资源中");
		break;
	case TM_ResExtract:
		desc = TEXT("解压应用资源中");
		break;
	case TM_Success:
		desc = TEXT("更新成功");
		break;
	case TM_Fail:
		desc = TEXT("更新失败");
		break;
	default:
		desc = TEXT("");
		break;
	}
	return desc;
}


#if PLATFORM_ANDROID
// 源码里拷出来的
// Constructs the base path for any files which are not in OBB/pak data
const FString& GetFileBasePath_TM()
{
	static FString BasePath =  FString(TEXT("/storage/emulated/0/UE4Game/")) + FApp::GetProjectName() + FString("/");
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

FString LoadFileInPluginsPath(const FString& fileRelativePathRoot, const FString& infileRelativeName)
{
	FString pluginRoot = FPaths::ConvertRelativePathToFull(FPaths::ProjectPluginsDir());
	FString readFilePath;
	FString fstream;
#if PLATFORM_WINDOWS
	readFilePath = FPaths::Combine(pluginRoot, fileRelativePathRoot, infileRelativeName);
#elif PLATFORM_ANDROID
	readFilePath = infileRelativeName;
#elif PLATFORM_IOS

	//TODO

#endif
	if (readFilePath.IsEmpty())
	{
		return fstream;
	}
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

bool WriteFileInPluginsPath(const FString& content, const FString& fileRelativePathRoot, const FString& infileRelativeName)
{
	FString pluginRoot = FPaths::ConvertRelativePathToFull(FPaths::ProjectPluginsDir());
	FString writeFilePath;
	FString fstream;
#if PLATFORM_WINDOWS
	writeFilePath = FPaths::Combine(pluginRoot, fileRelativePathRoot, infileRelativeName);
#elif PLATFORM_ANDROID
	writeFilePath = infileRelativeName;
#elif PLATFORM_IOS

	//TODO

#endif
	if (writeFilePath.IsEmpty())
	{
		return false;
	}
	if (FFileHelper::SaveStringToFile(content, *writeFilePath))
	{
		UE_LOG(LogTemp, Warning, TEXT("### write file: %s, content: %s"), *writeFilePath, *content);
		return true;
	}
	else
	{
		UE_LOG(LogTemp, Error, TEXT("### write file:%s failed, content: %s"), *writeFilePath, *content);
	}
	return false;
}


std::string _convertSizeToStr(uint64 byteSize)
{
	static std::vector<std::string> m_sizeStrVec = { "B", "KB", "MB", "GB" };

	uint64 temp = byteSize;
	uint64 remain = 0;
	double pair = 1024.0;
	int i = 0;

	while (temp >= pair && i < m_sizeStrVec.size())
	{
		remain = (uint64)temp % (uint64)pair;
		temp = temp / pair;   //temp /= pair;
		i++;
	}

	temp = std::floor(temp);
	std::string res = std::to_string((uint64)temp);
	if (remain > 0)
	{
		res += "." + std::to_string(remain).substr(0, 2);
	}
	res += m_sizeStrVec.at(i);
	return res;
}

const FString& GetProjectSavePath_TM()
{
	static FString saveRootDir;
#if PLATFORM_ANDROID
	saveRootDir = AndroidRelativeToAbsolutePath_TM(FPaths::ProjectSavedDir());
#elif PLATFORM_IOS
	saveRootDir = IFileManager::Get().ConvertToAbsolutePathForExternalAppForRead(*FPaths::ProjectSavedDir());
#else
	saveRootDir = FPaths::ConvertRelativePathToFull(FPaths::ProjectSavedDir());                   //windows也可以使用  IFileManager::Get().ConvertToAbsolutePathForExternalAppForRead
#endif
	return saveRootDir;
}