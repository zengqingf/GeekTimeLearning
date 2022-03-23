// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

/**
 * 
 */
class BUGLY_API BuglyInterface
{
public:
	BuglyInterface();
	~BuglyInterface();
public:
	void SetUserId(const FString& InUserId);
	void SetUserSceneTag(int32 InTagId);
	void PutUserData(const FString& InKey, const FString& InValue);




	void TestJavaCrash();
	void TestANRCrash(); //ANR收集不到
	void TestNativeCrash();

	static void VPostLogError(FString name, FString reason, FString stackTrace);

};
