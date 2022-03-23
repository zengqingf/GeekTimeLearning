// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/SaveGame.h"
#include "SDKInfoSaveConfig.generated.h"

/**
 * 
 */
UCLASS()
class TMSDKEDITOR_API USDKInfoSaveConfig : public USaveGame
{
	GENERATED_BODY()

public:
	UPROPERTY()
	FString UserPhoneNum;
	UPROPERTY()
	FString UserBuildComment;  //打包备注
};
