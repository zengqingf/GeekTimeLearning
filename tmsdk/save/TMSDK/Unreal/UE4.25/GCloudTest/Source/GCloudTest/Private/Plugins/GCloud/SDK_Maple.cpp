// Fill out your copyright notice in the Description page of Project Settings.


#include "Plugins/GCloud/SDK_Maple.h"
#include "UtilityMarcoDefine.h"

#include "JsonLibUtil.h"
#include "Plugins/TMSDKDefine.h"

SDK_Maple::SDK_Maple()
{
	DIR_URL = "pre-dir.6.948137280.cs.gcloud.qq.com";
}

SDK_Maple::~SDK_Maple()
{
	if (m_dirObserver != nullptr)
	{
		ITDir::GetInstance().RemoveObserver(m_dirObserver);
	}
	SAFE_DELETE_PTR(m_dirObserver);
}

bool SDK_Maple::Initialize(const char* openid)
{
	m_regionId = 0;
	m_subareaId = 0;
	_loadDirRegionInfo();

	//test
	m_dirObserver = new TDirServiceObserver(m_regionId, m_subareaId);
	ITDir::GetInstance().AddObserver(m_dirObserver);
	ITDir::GetInstance().EnableManualUpdate(true);
	TDirInitInfo initInfo;
	initInfo.OpenID = openid;
	initInfo.Url = DIR_URL;
	initInfo.EnableManualUpdate = true;
	initInfo.MaxIdleTime = 3;

	bool bSucc = ITDir::GetInstance().Initialize(initInfo);
	UE_LOG(LogTemp, Log, TEXT("### init dir service, result: %d..."), bSucc ? 0 : -1);
	return bSucc;
}

void SDK_Maple::UpdateDirService()
{
	ITDir::GetInstance().UpdateByManual();
}

void SDK_Maple::QueryDirServiceTree()
{
	SeqId sId = ITDir::GetInstance().QueryTree(m_regionId);
	UE_LOG(LogTemp, Log, TEXT("### query dir tree, tree id:%d, return seq id: %d..."), m_regionId, sId);
}


void SDK_Maple::_loadDirRegionInfo()
{
	FString fs = LoadFileInPluginsPath(TEXT("TMSDKTool/TMSDK/PlatformRes/All"), TEXT("maple/dir_region_info.json"));
	if (!fs.IsEmpty())
	{
		TSharedRef<TJsonReader<>> freader = TJsonReaderFactory<>::Create(fs);
		TSharedPtr<FJsonObject> fobjPtr;
		bool succ = FJsonSerializer::Deserialize(freader, fobjPtr);
		if (succ)
		{
			m_regionId = fobjPtr->GetIntegerField(TEXT("regionId"));
			m_subareaId = fobjPtr->GetIntegerField(TEXT("subareaId"));

			UE_LOG(LogTemp, Log, TEXT("read file json: regionId: %d. subareaId: %d"),
				m_regionId, m_subareaId);
		}
	}
	else
	{
		//test log
		UE_LOG(LogTemp, Error, TEXT("load file is empty: %s"), FPlatformProcess::BaseDir());
		//only for ue4 windows and editor
		UE_LOG(LogTemp, Error, TEXT("load file is empty: %s"), *(FPaths::ProjectPluginsDir()));
		UE_LOG(LogTemp, Error, TEXT("load file is empty: %s"), *(FPaths::ConvertRelativePathToFull(FPaths::ProjectPluginsDir())));
	}
}
