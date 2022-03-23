// Fill out your copyright notice in the Description page of Project Settings.


#include "Plugins/GameHotfix.h"

#include "Plugins/TMSDKDefine.h"

#include "UGCloudGameInstance.h"

GameHotfix::GameHotfix():
	m_currProgress(0)
{
}

GameHotfix::~GameHotfix()
{
}

void GameHotfix::StartUpdate()
{
	UE_LOG(LogTemp, Log, TEXT("### trigger update start"));

	UWorld* world = UUGCloudGameInstance::GetInstance()->GetWorld();
	if (world)
	{
		m_durationSecond = world->GetRealTimeSeconds();
	}
	PluginManager::Instance().EventManager().TriggerEvent(SDKEventType::UpdateStart);
}

void GameHotfix::ProgressUpdate(TM_HotfixState state, uint64 cur, uint64 max)
{
	UpdateEventParam param;
	param.float_0 = (max != 0) ? ((float)cur / (float)max) : 0;
	//char str1[42];
	//snprintf(str1, sizeof(str1), "%llu/%llu", cur, max);
	//snprintf(str1, sizeof(str1), "%llu/%llu", cur, max);
	FString fstr = FString::Printf(TEXT("%s/%s"), UTF8_TO_TCHAR(_convertSizeToStr(cur).c_str()), UTF8_TO_TCHAR(_convertSizeToStr(max).c_str()));
	param.fstr_0 = fstr;

	UWorld* world = UUGCloudGameInstance::GetInstance()->GetWorld();
	if (world)
	{
		double curr = static_cast<double>(cur) / (1024 * 1024);
		float currSeconds = world->GetRealTimeSeconds();

		if (curr < m_currProgress)
		{
			m_currProgress = 0;
		}
		double deltaProgress = curr - m_currProgress;
		//UE_LOG(LogTemp, Log, TEXT("--- %llu --- %lf --- %lf"), cur, curr, m_currProgress);
		double deltaTime = currSeconds - m_durationSecond;
		if (m_currProgress > 0 && deltaProgress > 0 && m_durationSecond > 0 && deltaTime > 0)
		{
			//UE_LOG(LogTemp, Log, TEXT("%lf --- %lf"), deltaProgress, deltaTime);
			//这里60不确定是否需要加？？？
			m_updateSpeed = deltaProgress / (deltaTime * 60);
			//保留两位小数
			m_updateSpeedStr = FString::SanitizeFloat((int)(m_updateSpeed * 100 + 0.5) / 100.0);
		}
		m_currProgress = curr;
		m_durationSecond = currSeconds;
	}

	param.fstr_1 = FString::Printf(TEXT("%s...%s MB/s"), *HotfixState2Desc(state), *m_updateSpeedStr);
	PluginManager::Instance().EventManager().TriggerEvent(SDKEventType::UpdateProgress, param);
}

void GameHotfix::OptionalUpdate(std::function<void()> ContinueUpdateAction, std::function<void()> ExitUpdateAction)
{
	ContinueUpdateAction();
}

void GameHotfix::Finish(const char* newVersionName)
{
	UpdateEventParam param;
	param.fstr_0 = UTF8_TO_TCHAR(newVersionName);
	UE_LOG(LogTemp, Log, TEXT("### trigger update finish, new version name: %s"), *(param.fstr_0));
	PluginManager::Instance().EventManager().TriggerEvent(SDKEventType::UpdateFinish, param);
}

void GameHotfix::Error()
{
	UpdateEventParam param;
	param.fstr_1 = TEXT("更新出错，请检查网络，重启游戏");

	UE_LOG(LogTemp, Error, TEXT("### trigger update error: %s"), *param.fstr_1);

	PluginManager::Instance().EventManager().TriggerEvent(SDKEventType::UpdateError, param);
}