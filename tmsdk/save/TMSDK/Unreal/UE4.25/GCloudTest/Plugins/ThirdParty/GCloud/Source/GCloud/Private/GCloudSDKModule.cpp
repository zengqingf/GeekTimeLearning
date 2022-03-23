// Copyright 1998-2017 Epic Games, Inc. All Rights Reserved.
#include "GCloudSDKModule.h"
#include "Misc/CoreDelegates.h"
#include "CoreGlobals.h"
#include "GCloudAppDelegate.h"

#if PLATFORM_MAC
#include "Misc/Paths.h"
extern "C" void gcloud_set_engine_plugin_path(const char* path , int len);
#endif

//using namespace ABase;
#define LOCTEXT_NAMESPACE "FGCloudSDKModule"

void FGCloudSDKModule::StartupModule()
{
#if PLATFORM_MAC
    FString relativePath = FPaths::ProjectDir();
    FString absPath = FPaths::ConvertRelativePathToFull(relativePath);
    const char* cpath = TCHAR_TO_UTF8(*absPath);
    gcloud_set_engine_plugin_path(cpath, strlen(cpath));
#endif
    
#if !PLATFORM_LINUX
	//UE_LOG(LogInit, Warning, TEXT("StartupModule()"));
	//UE_LOG(LogInit, Warning, TEXT("GCloudCachePath :%s"), *FString(ABase::CPath::GetCachePath()));
	//SetABaseLogCallback(MyLogCallback);
    
    GCloudAppDelegate::GetInstance().Initialize();
#endif
} 

void FGCloudSDKModule::ShutdownModule()
{
	 
}


#undef LOCTEXT_NAMESPACE
	
IMPLEMENT_MODULE(FGCloudSDKModule, GCloud)
