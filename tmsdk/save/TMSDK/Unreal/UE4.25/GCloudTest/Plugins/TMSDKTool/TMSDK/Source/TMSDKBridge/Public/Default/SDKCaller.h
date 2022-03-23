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
		//test
		FString cinfo;
		callInfo.ConvertToJson(cinfo);
		UE_LOG(LogTemp, Log, TEXT("Call Info : %s"), *cinfo);

		FString resultStr = TEXT("{\"code\":0,\"message\":\"\",\"obj\":{}}");
		TSharedPtr<FJsonObject> jObj = MakeShareable(new FJsonObject());
		JsonLibUtil::ToObject<TSharedPtr<FJsonObject>>(resultStr, jObj);
		callRes = new SDKCallResultTemp<T>(*jObj, "obj");
		callRes->SetCode(jObj->GetIntegerField("code"));
		callRes->SetMsg(jObj->GetStringField("message"));
		return *callRes;
	}

public:
	SDKCaller();
	~SDKCaller();
};

#endif //_SDKCALLER_H_