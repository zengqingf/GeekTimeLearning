// Copyright Epic Games, Inc. All Rights Reserved.

#include "Bugly.h"

#include "BuglyAgent.h"

DEFINE_LOG_CATEGORY(LogBugly);

#define LOCTEXT_NAMESPACE "FBuglyModule"

void FBuglyModule::StartupModule()
{
	// This code will execute after your module is loaded into memory; the exact timing is specified in the .uplugin file per-module

	mBuglyAgent = MakeShareable(new FBuglyAgent());
}

void FBuglyModule::ShutdownModule()
{
	// This function may be called during shutdown to clean up your module.  For modules that support dynamic reloading,
	// we call this function before unloading the module.
}

FBuglyAgentPtr FBuglyModule::GetAgent()
{
	return mBuglyAgent;
}

#undef LOCTEXT_NAMESPACE
	
IMPLEMENT_MODULE(FBuglyModule, Bugly)