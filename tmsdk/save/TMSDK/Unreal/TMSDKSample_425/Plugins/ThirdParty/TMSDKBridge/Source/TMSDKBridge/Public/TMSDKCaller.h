#pragma once

#ifndef _TMSDKCALLER_H_
#define _TMSDKCALLER_H_

#include "CoreMinimal.h"
#include "TMSDKBridge.h"

class TMSDKBRIDGE_API TMSDKCaller
{
public:
	TMSDKCaller();
	~TMSDKCaller();

	static TMSDKCaller& GetInstance();

	template<typename T>
	SDKCallResult& Call(SDKCallInfo& callInfo) 
	{
		return FTMSDKBridgeModule::Get().GetSDKCaller()->Call<T>(callInfo);
	}
};

#endif //_TMSDKCALLER_H_