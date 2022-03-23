// Fill out your copyright notice in the Description page of Project Settings.


#include "GameUpdate.h"

#include "GCloudGameMode.h"
#include "UGCloudGameInstance.h"

#include "Blueprint/UserWidget.h"

#include "UtilityMarcoDefine.h"
#include "Plugins/UI/GameUpdateView.h"
#include "Plugins/GCloud/GCloudManager.h"

#include "Plugins/GameHotfix.h"

void AGameUpdate::Init(AGCloudGameMode* parentObj, void(AGCloudGameMode::*callback)(void))
{
	mUpdateEndHandle = OnGameUpdateEnd.AddUObject(parentObj, callback);

	mEventHandles.Add(PluginManager::Instance().EventManager().RegisterEvent(SDKEventType::UpdateStart, SDK_EVENT_FUNC(_onUpdateStart)));
	mEventHandles.Add(PluginManager::Instance().EventManager().RegisterEvent(SDKEventType::UpdateProgress, SDK_EVENT_FUNC(_onUpdateProgress)));
	mEventHandles.Add(PluginManager::Instance().EventManager().RegisterEvent(SDKEventType::AllUpdateEnd, SDK_EVENT_FUNC(_onUpdateEnd)));

	_startGameUpdate();
}

void AGameUpdate::UnInit()
{
	for (auto handle : mEventHandles)
	{
		PluginManager::Instance().EventManager().RemoveEvent(handle);
	}
	mEventHandles.Empty();

	_stopGameUpdate();

	OnGameUpdateEnd.Clear();
	bUpdateEnd = false;
}

void AGameUpdate::_startGameUpdate()
{
	PluginManager::Instance().GCloudMgr().EnableComponent(SDKComponentType::GAME_UPDATE);
}

void AGameUpdate::_stopGameUpdate()
{
	PluginManager::Instance().GCloudMgr().DisableComponent(SDKComponentType::GAME_UPDATE);
}

void AGameUpdate::_broadcastUpdateEnd()
{
	if (!bUpdateEnd)
	{
		if (OnGameUpdateEnd.IsBound())
		{
			OnGameUpdateEnd.Broadcast();
		}
		bUpdateEnd = true;
	}
}

void AGameUpdate::_onUpdateStart(const SDKEventParam& param)
{
	_openGameUpdateView();
}

void AGameUpdate::_onUpdateProgress(const SDKEventParam& param)
{
	UpdateEventParam updateEventParam = static_cast<const UpdateEventParam&>(param);
	_updateGameUpdateView(updateEventParam.float_0, updateEventParam.fstr_0, updateEventParam.fstr_1);
}

void AGameUpdate::_onUpdateEnd(const SDKEventParam& param)
{
	_closeGameUpdateView();
	_broadcastUpdateEnd();
}

void AGameUpdate::_openGameUpdateView()
{
	//safe open
	if (mUpdateViewInstance)
	{
		mUpdateViewInstance->RemoveFromViewport();
		mUpdateViewInstance->BeginDestroy();
	}
	if (UClass* widgetClass = LoadClass<UUserWidget>(nullptr, TEXT("WidgetBlueprint'/Game/Blueprint/BP_GameUpdateWidget.BP_GameUpdateWidget_C'")))
	{
		UWorld* world = UUGCloudGameInstance::GetInstance()->GetWorld();
		if (world)
		{
			if (APlayerController* pc = world->GetFirstPlayerController())
			{
				mUpdateViewInstance = CreateWidget<UUserWidget>(pc, widgetClass);
				if (mUpdateViewInstance)
				{
					mUpdateViewInstance->AddToViewport();
					mUpdateView = Cast<UGameUpdateView>(mUpdateViewInstance);
				}
			}
		}
	}
	UE_LOG(LogTemp, Log, TEXT("### update start... open update view"));
}

void AGameUpdate::_closeGameUpdateView()
{
	if (mUpdateViewInstance)
	{
		mUpdateViewInstance->RemoveFromViewport();
	}
	UE_LOG(LogTemp, Log, TEXT("### update finish... close update view"));
}

void AGameUpdate::_updateGameUpdateView(float percent, const FString& perStr, const FString& desc)
{
	if (mUpdateView != nullptr) {
		if (perStr.IsEmpty())
		{
			mUpdateView->SetUpdateProgressDesc(desc);
		}
		else
		{
			mUpdateView->SetUpdateProgressBar(percent);
			mUpdateView->SetUpdateProgressPercent(perStr);
			mUpdateView->SetUpdateProgressDesc(desc);
		}
	}
}