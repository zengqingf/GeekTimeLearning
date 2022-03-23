// Copyright Epic Games, Inc. All Rights Reserved.

#include "TMSDKCommon.h"

#define LOCTEXT_NAMESPACE "FTMSDKCommonModule"

FOnMainModule FTMSDKCommonModule::OnMainModule;

void FTMSDKCommonModule::StartupModule()
{
	// This code will execute after your module is loaded into memory; the exact timing is specified in the .uplugin file per-module
}

void FTMSDKCommonModule::ShutdownModule()
{
	// This function may be called during shutdown to clean up your module.  For modules that support dynamic reloading,
	// we call this function before unloading the module.
}

TenmoveSDK::TMSDKEventManager* FTMSDKCommonModule::GetEventManager()
{
	if(OnMainModule.IsBound())
	{
		TenmoveSDK::TMSDKEventManager* mgr = OnMainModule.Execute();
		if(nullptr != mgr)
		{
			return mgr;
		}
	}
	return nullptr;
}

#undef LOCTEXT_NAMESPACE
	
IMPLEMENT_MODULE(FTMSDKCommonModule, TMSDKCommon)