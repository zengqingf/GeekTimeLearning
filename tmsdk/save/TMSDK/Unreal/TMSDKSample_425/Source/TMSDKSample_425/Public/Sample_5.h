// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

/**
 * Test of path and file
 */
class TMSDKSAMPLE_425_API Sample_5
{
public:
	Sample_5();
	~Sample_5();

	void Test1();
};

#if PLATFORM_ANDROID
// 源码里拷出来的
// Constructs the base path for any files which are not in OBB/pak data
const FString& GetFileBasePath_TM();

FString AndroidRelativeToAbsolutePath_TM(FString RelPath);
#endif

FString GetProjectSavePath_TM();
