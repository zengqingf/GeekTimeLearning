// Copyright 1998-2016 Epic Games, Inc. All Rights Reserved.

#include "CrasheyeRuntimeSettings.h"
#include "CrasheyePrivatePCH.h"

//////////////////////////////////////////////////////////////////////////
// UPaperRuntimeSettings

UCrasheyeRuntimeSettings::UCrasheyeRuntimeSettings(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer)
	, Andoird_bEnableCrasheye(false)
	, Andoird_AppKey(TEXT(""))
	, IOS_bEnableCrasheye(false)
	, IOS_AppKey(TEXT(""))
{
}
