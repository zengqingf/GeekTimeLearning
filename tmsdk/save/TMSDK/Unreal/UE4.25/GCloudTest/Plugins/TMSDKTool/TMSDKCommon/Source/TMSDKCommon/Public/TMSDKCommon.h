// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "TMSDKEventManager.h"
#include "Modules/ModuleManager.h"


DECLARE_DELEGATE_RetVal(TenmoveSDK::TMSDKEventManager*, FOnMainModule)

class TMSDKCOMMON_API FTMSDKCommonModule : public IModuleInterface
{
public:

	/** IModuleInterface implementation */
	virtual void StartupModule() override;
	virtual void ShutdownModule() override;

	static FOnMainModule OnMainModule;
	static TenmoveSDK::TMSDKEventManager* GetEventManager();
};
