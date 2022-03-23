#pragma once

#include "Android/AndroidJava.h"
#include "Android/AndroidJavaEnv.h"
#include "Core.h"
#include "ITGPAService.h"

#ifndef PRIVATE_ANDROID_TGPASERVICEANDROID_H
#define PRIVATE_ANDROID_TGPASERVICEANDROID_H

using namespace GCloud::TGPA;

class TGPAServiceAndroid : public FJavaClassObject, public ITGPAService 
{
private: 
    float lastFps = 30;
    int fpsCount = 0;
    float fpsArr[5];
    static const int SIZE = 5;

private:
    bool CheckGameFps(float fps);
    FString ConvertTMap2JsonStr (const TMap<FString, FString>& mapData);

public:
    static ITGPACallback *sTGPACallback;
    
public:
    TGPAServiceAndroid();

    void EnableLog(bool enable);

    void EnableDebugMode();

    void Init();

    void RegisterCallback(ITGPACallback *callback);

    void UpdateGameFps (float value);

    void UpdateGameInfo(const int key, const FString& value);

    void UpdateGameInfo(const int key, const int value);

    void UpdateGameInfo(const FString& key, const FString& value);

    void UpdateGameInfo(const FString& key, const TMap<FString, FString>& mapData);

    FString GetOptCfgStr();

    FString CheckDeviceIsReal();

    FString GetDataFromTGPA(const FString& key, const FString& value);


private:
    void JNIEnableLog(bool enable);

    void JNIEnableDebugMode();

    void JNIInit();

    void JNIRegisterCallback();

    void JNIUpdateGameInfoIS(const int key, const FString& value);

    void JNIUpdateGameInfoII(const int key, const int value);
    
    void JNIUpdateGameInfoIF(const int key, float* value);

    void JNIUpdateGameInfoSS(const FString& key, const FString& value);

    FString JNIGetDataFromTGPA(const FString& key, const FString& value);

private:
    static FName GetPerformanceAjusterClassName();

    FJavaClassMethod EnableLogMethod;
    FJavaClassMethod EnableDebugModeMethod;
    FJavaClassMethod InitMethod;
    FJavaClassMethod RegisterCallbackMethod;
    FJavaClassMethod UpdateGameInfoISMethod;
    FJavaClassMethod UpdateGameInfoIIMethod;
    FJavaClassMethod UpdateGameInfoIFArrayMethod;
    FJavaClassMethod UpdateGameInfoSSMethod;
    FJavaClassMethod GetDataFromTGPAMethod;

};

#endif // PRIVATE_ANDROID_TGPASERVICEANDROID_H
