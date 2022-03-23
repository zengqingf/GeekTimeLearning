//------------------------------------------------------------------------------
//
// File: TApm.h
// Module: APM
// Version: 1.0
// Author: vincentwgao
//
//------------------------------------------------------------------------------
#pragma once

#include "CoreMinimal.h"
#include "Modules/ModuleManager.h"

class FTApmModule : public IModuleInterface
{
public:

    /** IModuleInterface implementation */
    virtual void StartupModule() override;
    virtual void ShutdownModule() override;
};

