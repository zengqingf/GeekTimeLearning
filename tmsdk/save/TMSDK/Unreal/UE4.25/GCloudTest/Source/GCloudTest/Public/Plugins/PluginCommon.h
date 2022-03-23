#pragma once

#include "CoreMinimal.h"
#include "HAL/Platform.h"
#include "GenericPlatform/GenericPlatformFile.h"
#include "Containers/UnrealString.h"
#include "Plugins/IHotfix.h"

#include <string>
#include <vector>

#if PLATFORM_ANDROID
#include "Android/AndroidJavaEnv.h"
#include "Android/AndroidPlatformFile.h"
#endif


#ifndef WIN32
#define gc_snprintf snprintf
#else
#define gc_snprintf _snprintf_s
#endif


FString HotfixState2Desc(TM_HotfixState state);

#if PLATFORM_ANDROID
extern FString GFilePathBase;
extern FString GInternalFilePath;
extern FString GAPKFilename;

// 源码里拷出来的
// Constructs the base path for any files which are not in OBB/pak data
const FString& GetFileBasePath_TM();


FString AndroidRelativeToAbsolutePath_TM(FString RelPath);
#endif

const FString& GetProjectSavePath_TM();

FString LoadFileInPluginsPath(const FString& pluginRelativePath, const FString& infileRelativePath);

bool WriteFileInPluginsPath(const FString& jsonStr, const FString& fileRelativePathRoot, const FString& infileRelativeName);

//转换成B KB MB GB
//并保留2位小数
std::string _convertSizeToStr(uint64 byteSize);