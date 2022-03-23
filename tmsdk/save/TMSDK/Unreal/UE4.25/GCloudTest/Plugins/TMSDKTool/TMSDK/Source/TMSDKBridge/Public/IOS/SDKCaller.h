#pragma once

#ifndef _SDKCALLER_H_
#define _SDKCALLER_H_

#include "SDKBaseCaller.h"
#include "JsonLibUtil.h"

class TMSDKBRIDGE_API SDKCaller : public SDKBaseCaller<SDKCaller>
{
	friend class SDKBaseCaller<SDKCaller>;

	template<typename T>
	SDKCallResult& _Call(SDKCallInfo& callInfo)
	{
		callRes = new SDKCallResult();
		return *callRes;
	}
public:
	SDKCaller();
	~SDKCaller();
};

#endif //_SDKCALLER_H_