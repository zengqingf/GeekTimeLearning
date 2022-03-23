// Fill out your copyright notice in the Description page of Project Settings.

#pragma once
#include "Engine.h"

#define PRINT_SCREEN(TimeSecond, Text) if(GEngine){GEngine->AddOnScreenDebugMessage(-1, TimeSecond, FColor::Red, Text);}

DECLARE_STATS_GROUP(TEXT("UWA"), STATGROUP_UWA, STATCAT_Advanced);
DECLARE_CYCLE_STAT_EXTERN(TEXT("UWA SDK Total"), STAT_UWA_SDK, STATGROUP_UWA, );
DECLARE_CYCLE_STAT_EXTERN(TEXT("UWA Asset"), STAT_UWA_ASSET, STATGROUP_UWA, );
DECLARE_CYCLE_STAT_EXTERN(TEXT("UWA ScreenShot"), STAT_UWA_SCREENSHOT, STATGROUP_UWA, );

#define SCREEN_SHOT_INVERVAL 120
#define ASSET_TAKE_SAMPLE_INTERVAL 60
#define SAVE_IDMAP_INTERVAL 60