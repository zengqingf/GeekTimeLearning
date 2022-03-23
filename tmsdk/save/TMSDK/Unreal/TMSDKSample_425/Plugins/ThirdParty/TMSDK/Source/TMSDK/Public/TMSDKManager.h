#pragma once

#ifndef _TMSDKMANAGER_H_
#define _TMSDKMANAGER_H_

#include "CoreMinimal.h"
#include "TMBuglySDK.h"

class TMSDK_API TMSDKManager
{
public:
	TMSDKManager();
	~TMSDKManager();

	static TMSDKManager& GetInstance();

public:
	void Initialize();
	void SetUserId(const FString& userId);

private:
	TMBuglySDK* buglySdk;
};

#endif //_TMSDKMANAGER_H_