// Copyright 1998-2018 Epic Games, Inc. All Rights Reserved.

#include "UWAGOT.h"
#include "Core.h"
#include "ProfileManager.h"
#include "Modules/ModuleManager.h"
#include "Interfaces/IPluginManager.h"
#include "UWALib/Public/uwa.h"
#include "UWAFactory.h"

#define LOCTEXT_NAMESPACE "FUWAGOTModule"

void FUWAGOTModule::StartupModule()
{
	// This code will execute after your module is loaded into memory; the exact timing is specified in the .uplugin file per-module
	UProfileManager::Get()->Initialize();

	// Get the base directory of this plugin
	FString BaseDir = IPluginManager::Get().FindPlugin("UWAGOT")->GetBaseDir();
	// Add on the relative location of the third party dll and load it
	FString LibraryPath;
#if PLATFORM_WINDOWS
	LibraryPath = FPaths::Combine(*BaseDir, TEXT("Binaries/ThirdParty/UWALib/Win64/uwa.dll"));

	UwaLibHandle = !LibraryPath.IsEmpty() ? FPlatformProcess::GetDllHandle(*LibraryPath) : nullptr;

	if (UwaLibHandle)
	{
		UwaInit(&UWAFactory::Get());
	}
	else
	{
		FMessageDialog::Open(EAppMsgType::Ok, LOCTEXT("ThirdPartyLibraryError", "Failed to load example third party library"));
	}
 #endif // PLATFORM_WINDOWS
}

void FUWAGOTModule::ShutdownModule()
{
	// This function may be called during shutdown to clean up your module.  For modules that support dynamic reloading,
	// we call this function before unloading the module.

	// Free the dll handle
	FPlatformProcess::FreeDllHandle(UwaLibHandle);
	UwaLibHandle = nullptr;
}

bool FUWAGOTModule::ProcessConsoleExec(const TCHAR* Cmd, FOutputDevice& Ar, UObject* Executor)
{
	return UProfileManager::Get()->ProcessConsoleExec(Cmd, Ar, Executor);
}

#undef LOCTEXT_NAMESPACE
	
IMPLEMENT_MODULE(FUWAGOTModule, UWAGOT)