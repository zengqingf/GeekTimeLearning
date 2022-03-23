// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Plugins/TMSDKDefine.h"

DECLARE_EVENT(AGameUpdate, GameUpdateEnd);

/**
 * 
 */
class AGCloudGameMode;
class AGameUpdate
{
public:
	void Init(AGCloudGameMode*, void(AGCloudGameMode::*)(void));
	void UnInit();

	GameUpdateEnd OnGameUpdateEnd;
private:
	class FDelegateHandle mUpdateEndHandle;
	TArray<SDKEventHandle*> mEventHandles;
	bool bUpdateEnd = false;

private:
	void _startGameUpdate();
	void _stopGameUpdate();

	void _broadcastUpdateEnd();

	void _onUpdateStart(const SDKEventParam& param);
	void _onUpdateProgress(const SDKEventParam& param);
	void _onUpdateEnd(const SDKEventParam& param);

private:
	class UUserWidget* mUpdateViewInstance;
	class UGameUpdateView* mUpdateView;
	void _openGameUpdateView();
	void _closeGameUpdateView();
	void _updateGameUpdateView(float percent, const FString& perStr, const FString& desc);
};
