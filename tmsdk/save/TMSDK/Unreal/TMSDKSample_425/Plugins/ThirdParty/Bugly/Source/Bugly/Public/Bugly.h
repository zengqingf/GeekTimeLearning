// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "Modules/ModuleManager.h"

#include "IBuglyAgent.h"

DECLARE_LOG_CATEGORY_EXTERN(LogBugly, Log, All);

typedef TSharedPtr<IBuglyAgent, ESPMode::ThreadSafe> FBuglyAgentPtr;

class BUGLY_API FBuglyModule : public IModuleInterface
{
public:

	static inline FBuglyModule& Get()
	{
		return FModuleManager::LoadModuleChecked<FBuglyModule>("Bugly");
	}

	static inline bool IsAvailable()
	{
		return FModuleManager::Get().IsModuleLoaded("Bugly");
	}

	/** IModuleInterface implementation */
	virtual void StartupModule() override;
	virtual void ShutdownModule() override;

	FBuglyAgentPtr GetAgent();

private:
	FBuglyAgentPtr mBuglyAgent;
};
