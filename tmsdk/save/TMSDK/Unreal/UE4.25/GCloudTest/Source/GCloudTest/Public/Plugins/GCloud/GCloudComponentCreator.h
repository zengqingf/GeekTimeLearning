// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

#include "Plugins/TMSDKDefine.h"

/**
 * 
 */

class GCLOUDTEST_API GCloudComponentCreator : public SDKComponentCreator
{
public:
	GCloudComponentCreator() {}
	~GCloudComponentCreator();
	SDKComponent* Create(SDKComponentType type) override;
	void Destroy(SDKComponent* com) override;
	void Destroy(SDKComponentType type) override;
	void Destroy(int sdkComponentType) override;
};

class GCLOUDTEST_API GCloudDolphin : public SDKComponent
{
public:
	GCloudDolphin(SDKComponentType type): SDKComponent(type) {}
	~GCloudDolphin() {}
	void Init(const SDKInfo& sdkInfo, bool isDebug) override;
	void Uninit() override;
	void Tick() override;

private:
	void _createGCloudDolphin(const SDKInfo& sdkInfo);
	void _updateResVersionInConfig(const FString& newResVer);

	//程序更新和资源更新需要串行调用（创建两个对象，先创建一个对象执行应用程序更新，更新完成后再创建一个对象执行资源更新）
	void _initAppUpdate();
	void _initResUpdate();
	//资源修复
	void _initResCheck();

	void _onUpdateFinished(const SDKEventParam& param);
	class SDKEventHandle* m_updateStartHandle = nullptr;
	class SDKEventHandle* m_updateFinishHandle = nullptr;

	class SDK_Dolphin* m_dolphin = nullptr;
	bool m_bAppUpdateInited = false;
	bool m_bResUpdateInited = false;
	bool m_bResCheckInited = false;
	bool m_bUpdateStarted = false;				//一种更新开始
	bool m_bUpdateFinished = false;				//一种更新完成
};

class GCLOUDTEST_API GCloudMaple : public SDKComponent
{
public:
	GCloudMaple(SDKComponentType type): SDKComponent(type) {}
	~GCloudMaple() {}
	void Init(const SDKInfo& sdkInfo, bool isDebug) override;
	void Uninit() override;
	void Tick() override;

private:
	void _createGCloudMaple();
	class SDKEventHandle* m_dirPullFinishHandle = nullptr;
	void _onDirPullFinished(const SDKEventParam& param);

	class SDK_Maple* m_maple = nullptr;
	bool m_bSDKMapleInited = false;
	bool m_bDirPullStarted = false;
	bool m_bDirPullFinished = false;
};