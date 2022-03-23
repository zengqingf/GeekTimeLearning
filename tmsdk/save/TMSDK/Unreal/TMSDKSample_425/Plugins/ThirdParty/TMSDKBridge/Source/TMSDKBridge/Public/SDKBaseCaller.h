#pragma once

#ifndef _SDKBASECALLER_H_
#define _SDKBASECALLER_H_

#include "ISDKCaller.h"
#include "SDKCallInfo.h"
#include "SDKCallResult.h"

template<typename D>
class TMSDKBRIDGE_API SDKBaseCaller
{
	template<typename T>
	SDKCallResult& _Call(SDKCallInfo& callInfo)
	{
		return *callRes;
	}

public:
	SDKBaseCaller() 
	{
		callRes = nullptr;
	}
	virtual ~SDKBaseCaller()
	{
		delete callRes;
		callRes = nullptr;
	}

	template<typename T>
	SDKCallResult& Call(SDKCallInfo& callInfo) 
	{
		return static_cast<D&>(*this).template _Call<T>(callInfo);
	}

protected:
	SDKCallResult *callRes;
};

#endif //_SDKBASECALLER_H_