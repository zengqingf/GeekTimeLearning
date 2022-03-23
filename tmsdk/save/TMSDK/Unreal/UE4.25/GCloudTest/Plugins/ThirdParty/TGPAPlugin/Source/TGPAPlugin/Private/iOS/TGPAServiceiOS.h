#pragma once

#if PLATFORM_IOS
#import <kgvmp/kgvmp.h>
#endif

#include "ITGPAService.h"

using namespace GCloud::TGPA;

class TGPAServiceiOS : public ITGPAService 
{
private:
    int fpsCount = 0;
    float fpsTotal = 0;

    FString ConvertTMap2JsonStr (const TMap<FString, FString>& mapData);

public:
    static ITGPACallback *sTGPACallback;

public: 
    TGPAServiceiOS();

    void EnableLog(bool enable);

    void EnableDebugMode();

    void Init();

    void RegisterCallback(ITGPACallback *callback);

    void UpdateGameFps (float value);

    void UpdateGameInfo(const int key, const int value);

    void UpdateGameInfo(const int key, const FString& value);

    void UpdateGameInfo(const FString& key, const FString& value);

    void UpdateGameInfo(const FString& key, const TMap<FString, FString>& mapData);

    FString GetOptCfgStr();
	
    FString CheckDeviceIsReal();

    FString GetDataFromTGPA(const FString& key, const FString& value);
};


