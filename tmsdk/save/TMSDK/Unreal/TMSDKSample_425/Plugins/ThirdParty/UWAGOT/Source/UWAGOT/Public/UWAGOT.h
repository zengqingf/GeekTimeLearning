// Copyright 1998-2018 Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "Modules/ModuleManager.h"

class FUWAGOTModule : public IModuleInterface
{
public:

	/** IModuleInterface implementation */
	virtual void StartupModule() override;
	virtual void ShutdownModule() override;

	// // Handle console command for uwa
	bool ProcessConsoleExec(const TCHAR* Cmd, FOutputDevice& Ar, UObject* Executor);

private:
	void*	UwaLibHandle;
};
