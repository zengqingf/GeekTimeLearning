// Copyright Epic Games, Inc. All Rights Reserved.

#include "TMSDKCommon.h"

#define LOCTEXT_NAMESPACE "FTMSDKCommonModule"

FOnTMSDKSample FTMSDKCommonModule::OnSample;

void FTMSDKCommonModule::StartupModule()
{
	// This code will execute after your module is loaded into memory; the exact timing is specified in the .uplugin file per-module

	OnSample.AddRaw(this, &FTMSDKCommonModule::_testCallSelf);
}

void FTMSDKCommonModule::ShutdownModule()
{
	// This function may be called during shutdown to clean up your module.  For modules that support dynamic reloading,
	// we call this function before unloading the module.
	UE_LOG(LogTemp, Log, TEXT("### TMSDKCommon moudle shutdown"));
	OnSample.Clear();
}

void FTMSDKCommonModule::_testCallSelf()
{
	UE_LOG(LogTemp, Log, TEXT("### 本模块调用 -- TMSDKCommon module call self func"));
}

void FTMSDKCommonModule::TestCallMain()
{
	if(OnSample.IsBound())
	{
		OnSample.Broadcast();
	}
}

#undef LOCTEXT_NAMESPACE


//TMSDKCommon 为模块名
IMPLEMENT_MODULE(FTMSDKCommonModule, TMSDKCommon)