// Fill out your copyright notice in the Description page of Project Settings.


#include "BuglyAgent.h"
#include "TMSDKCaller.h"

#include "Misc/ConfigCacheIni.h"

FBuglyAgent::FBuglyAgent()
{
	if (GConfig)
	{
		GConfig->GetString(TEXT("/Script/Bugly.BuglySettings"), TEXT("AndroidAppID"), appId, GGameIni);
	}

	bDebug = !UE_BUILD_SHIPPING;

	UE_LOG(LogBugly, Display, TEXT("Bugly for android startup, AppID[%s]. DebugMode[%s]"), *appId, bDebug ? TEXT("On" : TEXT("Off")));
}

void FBuglyAgent::InitReport()
{
	if (appId.IsEmpty()) {
		appId = "8888dda3f9";
	}
	SDKCallInfo callInfo(2);
	callInfo.SetName("initCrashReport");
	callInfo.AddArgTemp<FString>(new SDKCallArgTemp<FString>("appId", appId));
	callInfo.AddArgTemp<bool>(new SDKCallArgTemp<bool>("isDebug", bDebug));
	UE_LOG(LogBugly, Log, TEXT("InitReport"));
	//返回object 可能为空
	SDKCallResult &callRes = TMSDKCaller::GetInstance().Call<FJsonObject>(callInfo);	


	//test
	SDKCallInfo callInfo2(1);
	callInfo2.SetName("init");
	callInfo2.SetIsCallback(false);
	callInfo2.AddArgTemp<bool>(new SDKCallArgTemp<bool>("isDebug", bDebug));
	UE_LOG(LogBugly, Log, TEXT("InitReport with Init Platform SDK"));
	SDKCallResult& callRes2 = TMSDKCaller::GetInstance().Call<FJsonObject>(callInfo2);
}

void FBuglyAgent::SetUserId(const FString& userId)
{
	SDKCallInfo callInfo(1);
	callInfo.SetName("setUserId");
	callInfo.AddArgTemp<FString>(new SDKCallArgTemp<FString>("userId", "tmsdk_test"));
	UE_LOG(LogBugly, Log, TEXT("SetUserId : %s"), *userId);
	//返回object 可能为空
	SDKCallResult &callRes = TMSDKCaller::GetInstance().Call<FJsonObject>(callInfo);
}