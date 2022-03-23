#include "TGPAServiceiOS.h"

void vmpCallback(const char* json)
{
    if (TGPAServiceiOS::sTGPACallback != nullptr)
    {
        TGPAServiceiOS::sTGPACallback->notifySystemInfo(ANSI_TO_TCHAR(json));
    }
}

ITGPACallback* TGPAServiceiOS::sTGPACallback = nullptr;

TGPAServiceiOS::TGPAServiceiOS()
{

}

void TGPAServiceiOS::EnableLog(bool enable)
{
    _SetLogAble(enable);
}

void TGPAServiceiOS::EnableDebugMode()
{
    _EnableDebugMode();
}

void TGPAServiceiOS::Init()
{
    _InitTGPA();
}

void TGPAServiceiOS::RegisterCallback(ITGPACallback *callback)
{
    TGPAServiceiOS::sTGPACallback = callback;
    _RegisterVmpCallback(vmpCallback);
}

void TGPAServiceiOS::UpdateGameFps (float value)
{
    fpsTotal += value;
    fpsCount += 1;
    if (fpsCount == 5)
    {
        _UpdateGameInfoIF (FPS, fpsTotal / 5);
        fpsCount = 0;
        fpsTotal = 0;
    }
}

void TGPAServiceiOS::UpdateGameInfo(const int key, const int value)
{
    _UpdateGameInfoII(key, value);
}

void TGPAServiceiOS::UpdateGameInfo(const int key, const FString& value)
{
    _UpdateGameInfoIS(key, TCHAR_TO_UTF8(*value));
}

void TGPAServiceiOS::UpdateGameInfo(const FString& key, const FString& value)
{
    _UpdateGameInfoSS(TCHAR_TO_UTF8(*key), TCHAR_TO_UTF8(*value));
}

void TGPAServiceiOS::UpdateGameInfo(const FString& key, const TMap<FString, FString>& mapData) 
{
    FString value = ConvertTMap2JsonStr(mapData);
    _UpdateGameInfoSS(TCHAR_TO_UTF8(*key), TCHAR_TO_UTF8(*value));
}

FString TGPAServiceiOS::ConvertTMap2JsonStr(const TMap<FString, FString>& mapData)
{
    FString result = FString(TEXT("{"));
    for (const TPair<FString, FString>& element : mapData)
    {
        result += FString::Printf(TEXT("\"%s\":\"%s\","), *element.Key, *element.Value);
    }
    result.RemoveFromEnd(FString(TEXT(",")), ESearchCase::IgnoreCase);
    return FString::Printf(TEXT("%s}"), *result);
}

FString TGPAServiceiOS::GetOptCfgStr() 
{
    return FString(TEXT("-1"));
}

FString TGPAServiceiOS::CheckDeviceIsReal() {
    return FString(TEXT("{\"result\":0}"));
}

FString TGPAServiceiOS::GetDataFromTGPA(const FString& key, const FString& value) {
    char* retStr = _GetDataFromTGPA(TCHAR_TO_UTF8(*key), TCHAR_TO_UTF8(*value));
    if (retStr == NULL) {
        return FString(TEXT("-1"));
    }
    FString ret = FString(ANSI_TO_TCHAR(retStr));
    free(retStr);
    return ret;
}