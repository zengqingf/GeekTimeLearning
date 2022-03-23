// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "TMSDKEventManager.h"
#include "GameFramework/GameMode.h"
#include "GCloudGameMode.generated.h"

/**
 * 
 */
UCLASS()
class AGCloudGameMode : public AGameMode
{
	GENERATED_BODY()
	
public:
	virtual void StartPlay() override;
	virtual void EndPlay(const EEndPlayReason::Type EndPlayReason) override;

private:
	class AGameUpdate* mGameUpdate = nullptr;
	void _afterGameUpdate();

	void _onGetMultiTouchEvent(const TenmoveSDK::SDKEventParam& param);
};
