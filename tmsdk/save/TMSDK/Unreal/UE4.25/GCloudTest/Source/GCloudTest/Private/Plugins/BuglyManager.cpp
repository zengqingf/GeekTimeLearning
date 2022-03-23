// Fill out your copyright notice in the Description page of Project Settings.


#include "Plugins/Bugly/BuglyManager.h"

#include "UtilityMarcoDefine.h"

//#include "Common/LogInit.h"


BuglyManager::BuglyManager()
{
	Init();
}

BuglyManager::~BuglyManager()
{
	Uninit();
	UE_LOG(LogTemp, Log, TEXT("### bugly mgr dtor"));
}

void BuglyManager::Init()
{
	mBuglyInteface = new BuglyInterface();
}

void BuglyManager::Uninit()
{
	SAFE_DELETE_PTR(mBuglyInteface);
}

bool BuglyManager::Tick(float DeltaTime)
{
	return false;
}

/// <summary>
/// 初始化bugly，暂时用测试值
/// </summary>
void BuglyManager::InitBugly()
{
//#if TMSDK_LOG
//	UE_LOG(LogTemp,Log,TEXT("!!InitBugly!!"));
//#endif
//#if TMSDK_LOGERROR
//	UE_LOG(LogTemp, Error, TEXT("### TEST MACRO"));
//#endif
	mBuglyInteface->SetUserId(TEXT("TMTest"));
	mBuglyInteface->SetUserSceneTag(1);
}

void BuglyManager::PostLog2Bugly(FString stackTrace, bool isupload)
{
	if (isupload)
	{
		BuglyInterface::VPostLogError("UnrealLogError", "LogError", stackTrace);
	}
}
