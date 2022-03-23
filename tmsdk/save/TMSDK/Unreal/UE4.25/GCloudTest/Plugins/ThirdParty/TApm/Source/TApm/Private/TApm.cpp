//------------------------------------------------------------------------------
//
// File: TApm.cpp
// Module: APM
// Version: 1.0
// Author: vincentwgao
//
//------------------------------------------------------------------------------


// Copyright 1998-2017 Epic Games, Inc. All Rights Reserved.

#include "TApm.h"
//#include "Modules/ModuleManager.h"


#define LOCTEXT_NAMESPACE "FTApmModule"

void FTApmModule::StartupModule()
{
    // This code will execute after your module is loaded into memory; the exact timing is specified in the .uplugin file per-module
}

void FTApmModule::ShutdownModule()
{
    // This function may be called during shutdown to clean up your module.  For modules that support dynamic reloading,
    // we call this function before unloading the module.
}

#undef LOCTEXT_NAMESPACE

IMPLEMENT_MODULE(FTApmModule, TApm)
