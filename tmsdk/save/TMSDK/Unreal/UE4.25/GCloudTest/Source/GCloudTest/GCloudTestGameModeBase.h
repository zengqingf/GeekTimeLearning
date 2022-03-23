// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/GameModeBase.h"

#include "Plugins//TMSDKDefine.h"

#include "GCloudTestGameModeBase.generated.h"

/**
 * 
 */
UCLASS()
class GCLOUDTEST_API AGCloudTestGameModeBase : public AGameModeBase
{
	GENERATED_BODY()

public:
	AGCloudTestGameModeBase();

protected:
	virtual void BeginPlay() override;
	virtual void EndPlay(const EEndPlayReason::Type EndPlayReason) override;

	//Test
private:
	class TenmoveSDK::SDKEventHandle* mDirPullFinishHandle = nullptr;
	class TenmoveSDK::SDKEventHandle* mDirPullErrorHandle = nullptr;
	void _onDirPullFinished(const TenmoveSDK::SDKEventParam& param);
	void _onDirPullError(const TenmoveSDK::SDKEventParam& param);
};
