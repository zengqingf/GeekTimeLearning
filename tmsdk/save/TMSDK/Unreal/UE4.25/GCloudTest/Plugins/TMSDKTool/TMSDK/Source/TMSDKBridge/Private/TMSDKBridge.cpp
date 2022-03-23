// Copyright Epic Games, Inc. All Rights Reserved.

#include "TMSDKBridge.h"

DEFINE_LOG_CATEGORY(LogTMSDKBridge);

#define LOCTEXT_NAMESPACE "FTMSDKBridgeModule"

void FTMSDKBridgeModule::StartupModule()
{
	// This code will execute after your module is loaded into memory; the exact timing is specified in the .uplugin file per-module

	mSDKCaller = MakeShareable(new SDKCaller());

	if (!mSDKCaller.IsValid())
	{
		UE_LOG(LogTMSDKBridge, Error, TEXT("### Create SDK Caller failed !"));
	}
}

void FTMSDKBridgeModule::ShutdownModule()
{
	// This function may be called during shutdown to clean up your module.  For modules that support dynamic reloading,
	// we call this function before unloading the module.
}


FTMSDKCallerPtr FTMSDKBridgeModule::GetSDKCaller()
{
	return mSDKCaller;
}

#undef LOCTEXT_NAMESPACE
	
IMPLEMENT_MODULE(FTMSDKBridgeModule, TMSDKBridge)