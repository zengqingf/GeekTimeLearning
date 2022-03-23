// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#ifndef _TMSDK_H_
#define _TMSDK_H_

#include "CoreMinimal.h"
#include "Modules/ModuleManager.h"

DECLARE_LOG_CATEGORY_EXTERN(LogTMSDK, Log, All);

class FTMSDKModule : public IModuleInterface
{
public:

	/** IModuleInterface implementation */
	virtual void StartupModule() override;
	virtual void ShutdownModule() override;
};

#endif //_TMSDK_H_