// Copyright Epic Games, Inc. All Rights Reserved.


#include "GCloudTestGameModeBase.h"

//Test
#include "Plugins/GCloud/GCloudManager.h"
#include "Plugins/TDirServiceObserver.h"
#include "Plugins/PluginCommon.h"

#include "HAL/FileManagerGeneric.h"
#include "Misc/FileHelper.h"
#include "Misc/Paths.h"


AGCloudTestGameModeBase::AGCloudTestGameModeBase()
{
	UE_LOG(LogTemp, Log, TEXT("### Gloud TEST Game mode base ctor"));
}

void AGCloudTestGameModeBase::BeginPlay()
{
	Super::BeginPlay();

	TMSDKManager::GetInstance().RequestPermission(PermissionType::SDCard);

	UE_LOG(LogTemp, Log, TEXT("### Gloud TEST Game mode base begin play"));

	//Test
	UE_LOG(LogTemp, Log, TEXT("### start maple test..."));
	PluginManager::Instance().GCloudMgr().DisableComponent(SDKComponentType::GAME_DIR_SERVER);
	PluginManager::Instance().GCloudMgr().EnableComponent(SDKComponentType::GAME_DIR_SERVER);
	mDirPullFinishHandle = PluginManager::Instance().EventManager().RegisterEvent(SDKEventType::DirPullFinish, SDK_EVENT_FUNC(_onDirPullFinished));
	mDirPullErrorHandle = PluginManager::Instance().EventManager().RegisterEvent(SDKEventType::DirPullError, SDK_EVENT_FUNC(_onDirPullError));

	
	//Test2
#if PLATFORM_ANDROID
	FString testUpdateStr;
	FString infoSavePath_rel(TEXT("dolphin_info"));
	FString baseDir = AndroidRelativeToAbsolutePath_TM(FPaths::ProjectSavedDir());
	FString infoSavePath = baseDir / infoSavePath_rel;
	FString relFileName((infoSavePath / TEXT("update_info.json")));
	FString fs = LoadFileInPluginsPath(TEXT(""), relFileName);
	if (!fs.IsEmpty())
	{
		TSharedRef<TJsonReader<>> freader = TJsonReaderFactory<>::Create(fs);
		TSharedPtr<FJsonObject> fobjPtr;
		bool succ = FJsonSerializer::Deserialize(freader, fobjPtr);
		if (succ)
		{
			testUpdateStr = fobjPtr->GetStringField(TEXT("testResUpdate"));
			UE_LOG(LogTemp, Log, TEXT("### read file json: testResUpdate: %s"), *testUpdateStr);
		}
	}
	FString relPathRoot(TEXT("TMSDKTool/TMSDK/PlatformRes/All"));
	FString relFileName2(TEXT("dolphin/update_info.json"));
	FString fs2 = LoadFileInPluginsPath(relPathRoot, relFileName2);
	if (!fs2.IsEmpty())
	{
		TSharedRef<TJsonReader<>> freader2 = TJsonReaderFactory<>::Create(fs2);
		TSharedPtr<FJsonObject> fobjPtr2;
		bool succ2 = FJsonSerializer::Deserialize(freader2, fobjPtr2);
		if (succ2)
		{
			testUpdateStr = fobjPtr2->GetStringField(TEXT("testResUpdate"));
			UE_LOG(LogTemp, Log, TEXT("### read file json: testResUpdate: %s"), *testUpdateStr);
		}
	}

	if (FFileManagerGeneric::Get().FileExists(TEXT("dolphin/test.txt")))
	{
		UE_LOG(LogTemp, Error, TEXT("### test txt is exist !"));
	}
	else
	{
		UE_LOG(LogTemp, Error, TEXT("### test txt is not exist !"));
	}
	FString txtFileName((infoSavePath / TEXT("test.txt")));
	if (FFileManagerGeneric::Get().FileExists(*txtFileName))
	{
		UE_LOG(LogTemp, Error, TEXT("### test txt in update path is exist !"));
	}
	else
	{
		UE_LOG(LogTemp, Error, TEXT("### test txt in update path is not exist !"));
	}

#endif
}

void AGCloudTestGameModeBase::EndPlay(const EEndPlayReason::Type EndPlayReason)
{
	UE_LOG(LogTemp, Log, TEXT("### Gloud TEST Game mode base end play"));

	Super::EndPlay(EndPlayReason);
}

void AGCloudTestGameModeBase::_onDirPullFinished(const SDKEventParam& param)
{
	DirInfoEventParam dirInfoEventParam = static_cast<const DirInfoEventParam&>(param);
	UE_LOG(LogTemp, Log, TEXT("### Dir server info json: %s"), *(dirInfoEventParam.fstr_0));
	UE_LOG(LogTemp, Log, TEXT("### after maple callback... msg: %s"), *dirInfoEventParam.fstr_0);
	PluginManager::Instance().GCloudMgr().DisableComponent(SDKComponentType::GAME_DIR_SERVER);
}

void AGCloudTestGameModeBase::_onDirPullError(const SDKEventParam& param)
{
	UE_LOG(LogTemp, Log, TEXT("### after maple callback... error!"));
	PluginManager::Instance().GCloudMgr().DisableComponent(SDKComponentType::GAME_DIR_SERVER);
}