// Fill out your copyright notice in the Description page of Project Settings.


#include "BuglyInterface.h"
#include "Bugly.h"

BuglyInterface::BuglyInterface()
{
}

BuglyInterface::~BuglyInterface()
{
}

void BuglyInterface::SetUserId(const FString& InUserId)
{
	FBuglyModule::Get().GetAgent()->SetUserId(InUserId);
}

void BuglyInterface::SetUserSceneTag(int32 InTagId)
{
	FBuglyModule::Get().GetAgent()->SetUserSceneTag(InTagId);
}

void BuglyInterface::PutUserData(const FString& InKey, const FString& InValue)
{
	FBuglyModule::Get().GetAgent()->PutUserData(InKey, InValue);
}

void BuglyInterface::TestJavaCrash()
{
	FBuglyModule::Get().GetAgent()->TestJavaCrash();
}

void BuglyInterface::TestANRCrash()
{
	FBuglyModule::Get().GetAgent()->TestANRCrash(); //暂时收集不到
}

void BuglyInterface::TestNativeCrash()
{
	FBuglyModule::Get().GetAgent()->TestNativeCrash();
}

void BuglyInterface::VPostLogError( FString name, FString reason, FString stackTrace)
{
	FBuglyModule::Get().GetAgent()->PostLogError( name, reason, stackTrace);
}
