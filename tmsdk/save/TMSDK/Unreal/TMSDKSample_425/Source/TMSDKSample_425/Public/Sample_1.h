// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"


#undef USE_LOGGING_IN_SHIPPING
#define USE_LOGGING_IN_SHIPPING 1

//显示所有日志语句
DECLARE_LOG_CATEGORY_EXTERN(LogTMSDKSample, Log, All);

//不会显示VeryVerbose语句。
//DECLARE_LOG_CATEGORY_EXTERN(TMSDK, Verbose, All);

//不会显示VeryVerbose和Verbose语句。
//DECLARE_LOG_CATEGORY_EXTERN(TMSDK, VeryVerbose, All);

/**
 * 
 */
class TMSDKSAMPLE_425_API Sample_1
{
public:
	Sample_1();
	~Sample_1();

	void Init();


	void FStringSplit_1();
	void FStringSplit_2();
};
