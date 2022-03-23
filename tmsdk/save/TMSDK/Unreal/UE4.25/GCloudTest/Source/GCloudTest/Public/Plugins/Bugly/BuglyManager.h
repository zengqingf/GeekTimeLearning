// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "BuglyInterface.h"
#include "Plugins/TMSDKDefine.h"

/**
 * 
 */
class GCLOUDTEST_API BuglyManager : public IPluginMgr
{
public:
	BuglyManager();
	~BuglyManager();

	void Init() override;
	void Uninit() override;
	bool Tick(float DeltaTime) override;

public:
	void InitBugly();
	static void PostLog2Bugly(FString stackTrace,bool isupload);
private:
	BuglyInterface* mBuglyInteface = nullptr;
};
