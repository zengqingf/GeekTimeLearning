// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#ifndef _TMSDKBRIDGE_H_
#define _TMSDKBRIDGE_H_

#include "CoreMinimal.h"
#include "Modules/ModuleManager.h"
//#include "ISDKCaller.h"
#include "SDKCaller.h"

DECLARE_LOG_CATEGORY_EXTERN(LogTMSDKBridge, Log, All);

typedef TSharedPtr<SDKBaseCaller<SDKCaller>, ESPMode::ThreadSafe> FTMSDKCallerPtr;

class TMSDKBRIDGE_API FTMSDKBridgeModule : public IModuleInterface
{
public:

	static inline FTMSDKBridgeModule& Get()
	{
		return FModuleManager::LoadModuleChecked<FTMSDKBridgeModule>("TMSDKBridge");
	}

	static inline bool IsAvailable()
	{
		return FModuleManager::Get().IsModuleLoaded("TMSDKBridge");
	}

	/** IModuleInterface implementation */
	virtual void StartupModule() override;
	virtual void ShutdownModule() override;

	FTMSDKCallerPtr GetSDKCaller();

private:
	FTMSDKCallerPtr mSDKCaller;
};

#endif //_TMSDKBRIDGE_H_