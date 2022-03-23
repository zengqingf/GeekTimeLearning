// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/HUD.h"

#include "MapleView.h"

#include "MapleHUD.generated.h"

/**
 * 
 */
UCLASS()
class GCLOUDTEST_API AMapleHUD : public AHUD
{
	GENERATED_BODY()
	
protected:
	virtual void BeginPlay() override;

public:

	UPROPERTY(EditAnywhere, Category = "UserWidget")
	TSubclassOf<class UMapleView> mapleWidgetClass;
};
