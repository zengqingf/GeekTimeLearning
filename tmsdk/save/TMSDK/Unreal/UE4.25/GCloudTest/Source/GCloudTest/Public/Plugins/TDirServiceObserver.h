// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Plugins/GCloud/GCloudCommonDefine.h"
#include "Plugins/TMSDKDefine.h"
/**
 * 
 */
class GCLOUDTEST_API TDirServiceObserver : public TDirObserver
{
public:
	TDirServiceObserver(int regionId, int subareaId)
		: m_regionId(regionId), m_subareaId(subareaId)
	{
		ABase::SetConsoleOutput(true);
	}
	virtual ~TDirServiceObserver();
	virtual void OnQueryTreeProc(const Result& result, const TreeInfo* tree) override;
	virtual void OnQueryLeafProc(const Result& result, const NodeWrapper* node) override;

	virtual void OnQueryAllProc(const Result& result, const TreeCollection* treeList) override;
	virtual void OnQueryFriendProc(const Result& result, const QueryFriendsResult& queryFriendsResult) override;

	static FString AStringToFString(const AString& astring)
	{
		return UTF8_TO_TCHAR(astring.c_str());
	}

private:
	const FString& _getDirServerJsonInfo();
	FString m_serverJsonStr;

	TArray<struct DirServerInfo> m_serverInfos;
	TArray<int> m_selectedLeafIds;
	int m_queryCount;

	//大区ID
	int m_regionId;
	//分区ID
	int m_subareaId;
};

struct DirInfoEventParam : public TenmoveSDK::SDKEventParam
{
public:
	~DirInfoEventParam() {}
	FString fstr_0;
};

//目录大区信息
struct GameRegionInfo
{
	int id;
	FString name;
};

//目录分区信息
struct DirSubareaInfo
{
	int id;
	FString name;
};

//目录服信息
struct DirServerInfo
{
	FString name;
	FString ip;
};