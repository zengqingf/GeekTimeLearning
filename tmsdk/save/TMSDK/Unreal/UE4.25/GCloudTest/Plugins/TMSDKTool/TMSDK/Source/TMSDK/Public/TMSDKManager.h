#pragma once

#ifndef _TMSDKMANAGER_H_
#define _TMSDKMANAGER_H_

#include "CoreMinimal.h"
#include "PermissionType.h"

class TMSDK_API TMSDKManager
{
public:
	TMSDKManager();
	~TMSDKManager();

	static TMSDKManager& GetInstance();

public:
	void Initialize();
	void SetUserId(const FString& userId);
	void RequestPermission(TenmoveSDK::PermissionType pType);
	void SetMultiPointerCount(int count);
private:
	class TMBuglySDK* buglySdk;
	class ISDKCaller* commonCaller;
};

#endif //_TMSDKMANAGER_H_