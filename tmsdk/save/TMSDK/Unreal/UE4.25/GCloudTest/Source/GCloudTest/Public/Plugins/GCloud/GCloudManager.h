// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

#include "Plugins/TMSDKDefine.h"

/**
 * 
 */
class GCLOUDTEST_API GCloudManager : public IPluginMgr
{
public:
	GCloudManager();
	~GCloudManager();

	void Init() override;
	void Uninit() override;
	bool Tick(float DeltaTime) override;

	SDKComponent* EnableComponent(SDKComponentType type);
	void DisableComponent(SDKComponentType type);

	void SetNetReachability(bool bReach);

	FString GetResVersionFromConfig();

private:
	const uint64_t GAME_ID = 948137280;
	const char* GAME_KEY = "8ab7d865a7a686ae4c94ff9b3d3fccd4";
	const char* updateBaseUrl = "gcloud.qq.com";
	const char* updateUrl = "pre-download.4.948137280.cs.gcloud.qq.com";

private:
	SDKInfo* mSdkInfo = nullptr;
	class GCloudComponentCreator* mCreator = nullptr;
	int mInitedComponentType = 0;
	bool mIsGCloudInited = false;
	bool mIsNetReachability = false;

	void _initSDKInfo();
	void _initGCloud(bool isDebug);
	void _initTDM();
	void _checkNetReachablity();
};
