// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Runtime/CoreUObject/Public/UObject/ObjectMacros.h"
#include "UWAEnum.generated.h"

/**
 * Profile Mode
 */
UENUM(BlueprintType)
enum class EProfileMode : uint8
{
	PM_Overview UMETA(DisplayName = "Overview"),
	PM_Asset UMETA(DisplayName = "Asset"),
	PM_Lua UMETA(DisplayName = "Lua")
};

/**
 * Profile State
 */
UENUM(BlueprintType)
enum class EProfileState : uint8
{
	PS_Ready UMETA(DisplayName = "Ready"),
	PS_SelectMode UMETA(DisplayName = "SelectMode"),
	PS_Start UMETA(DisplayName = "Start"),
	PS_Stop UMETA(DisplayName = "Stop")
};
