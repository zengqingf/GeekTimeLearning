// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

#include "Plugins/IHotfix.h"
#include "Plugins/TMSDKDefine.h"
/**
 * 
 */
class GCLOUDTEST_API GameHotfix : public HotfixCallBack
{
public:
	GameHotfix();
	~GameHotfix();

	// Inherited via IHotfix;
	virtual void StartUpdate() override;
	virtual void ProgressUpdate(TM_HotfixState state, uint64 cur, uint64 max) override;
	virtual void OptionalUpdate(std::function<void()> ContinueUpdateAction, std::function<void()> ExitUpdateAction) override;
	virtual void Finish(const char* newVersionName) override;
	virtual void Error() override;

private:
	double m_currProgress;
	float m_durationSecond;
	double m_updateSpeed;
	FString m_updateSpeedStr;
};

struct UpdateEventParam : public TenmoveSDK::SDKEventParam
{
public:
	~UpdateEventParam() {}
	FString fstr_0;
	FString fstr_1;
};