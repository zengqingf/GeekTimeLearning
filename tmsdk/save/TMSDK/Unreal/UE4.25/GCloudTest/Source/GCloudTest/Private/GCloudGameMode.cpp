// Fill out your copyright notice in the Description page of Project Settings.


#include "GCloudGameMode.h"

#include "UI/GameUpdate.h"
#include "UtilityMarcoDefine.h"

#include "UGCloudGameInstance.h"
#include "Plugins/TMSDKDefine.h"


void AGCloudGameMode::StartPlay()
{
	Super::StartPlay();

	TMSDKManager::GetInstance().RequestPermission(PermissionType::RecordVoice);
	
	UE_LOG(LogTemp, Warning, TEXT("### gcloud game mode start play 1"));

	UE_LOG(LogTemp, Warning, TEXT("### game update is %s"), PluginManager::IsPluginFunctionOpen(TEXT("game_update")) ? TEXT("open"): TEXT("close"));
	if (nullptr == mGameUpdate)
	{
		mGameUpdate = new AGameUpdate();
		mGameUpdate->Init(this, &AGCloudGameMode::_afterGameUpdate);
	}
	UE_LOG(LogTemp, Warning, TEXT("### gcloud game mode start play 2"));

	PluginManager::Instance().SetMultiPointerCount(5);
	SDKEventHandle* eventHandle = PluginManager::Instance().EventManager().RegisterEvent(SDKEventType::GetMultiTouchEvent,
		SDK_EVENT_FUNC(_onGetMultiTouchEvent));
	PluginManager::Instance().EventManager().RemoveEvent(eventHandle);
}

void AGCloudGameMode::EndPlay(const EEndPlayReason::Type EndPlayReason)
{
	UE_LOG(LogTemp, Warning, TEXT("### gcloud game mode end play 1"));
	if (mGameUpdate)
	{
		mGameUpdate->UnInit();
		SAFE_DELETE_PTR(mGameUpdate);
	}
	UE_LOG(LogTemp, Warning, TEXT("### gcloud game mode end play 2"));

	Super::EndPlay(EndPlayReason);
}

void AGCloudGameMode::_afterGameUpdate()
{
	UE_LOG(LogTemp, Log, TEXT("### after update callback..."));

	UUGCloudGameInstance::GetInstance()->OpenLevel("DefaultLevel");
}

void AGCloudGameMode::_onGetMultiTouchEvent(const SDKEventParam& param)
{
	UE_LOG(LogTemp, Log, TEXT("### on get multi touch callback..."));
}
