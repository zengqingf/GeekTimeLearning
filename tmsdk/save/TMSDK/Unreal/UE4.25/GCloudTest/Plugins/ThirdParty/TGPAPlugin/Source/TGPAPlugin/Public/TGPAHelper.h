// ------------------------------------------------------------------------------
// File: TGPAHelper.h
// Module: TGPA
// Version: 1.0
// Author: zohnzliu
// ------------------------------------------------------------------------------
#pragma once

#include "Engine.h"
#include "ITGPAService.h"

#ifndef PUBLIC_TGPAHELPER_H
#define PUBLIC_TGPAHELPER_H

#if PLATFORM_WINDOWS
#define TGPA_API  __declspec(dllexport)
#else
#define TGPA_API
#endif

namespace GCloud {
namespace TGPA {

class TGPA_API TGPAHelper {
public:
    static void EnableLog(bool enable);

    static void EnableDebugMode();

    static void Init();

    static void RegisterCallback(GCloud::TGPA::ITGPACallback *callback);

    static void UpdateGameFps(float fps);

    static void UpdateGameInfo(const int key, const FString& value);

    static void UpdateGameInfo(const int key, const int value);

    static void UpdateGameInfo(const FString& key, const FString& value);
    
    static void UpdateGameInfo(const FString& key, const TMap<FString, FString>& mapData);

    static FString GetOptCfgStr();

    static FString CheckDeviceIsReal();

    static FString GetDataFromTGPA(const FString& key, const FString& value);
};

} // namespace TGPA
} // namespace GCloud

#endif // PUBLIC_TGPAHELPER_H
