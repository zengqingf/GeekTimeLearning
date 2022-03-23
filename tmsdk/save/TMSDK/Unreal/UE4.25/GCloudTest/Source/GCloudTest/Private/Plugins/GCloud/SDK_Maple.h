// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Plugins/TDirServiceObserver.h"

/**
 * 
 */
class GCLOUDTEST_API SDK_Maple
{
public:
	SDK_Maple();
	~SDK_Maple();

	const char* DIR_URL = "";

	bool Initialize(const char* openid);
	void UpdateDirService();
	void QueryDirServiceTree();

private:
	TDirServiceObserver* m_dirObserver;

	void _loadDirRegionInfo();
	int m_regionId;
	int m_subareaId;
};