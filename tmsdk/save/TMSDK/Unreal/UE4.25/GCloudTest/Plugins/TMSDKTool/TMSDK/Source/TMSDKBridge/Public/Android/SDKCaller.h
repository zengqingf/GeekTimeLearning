#pragma once

#ifndef _SDKCALLER_H_
#define _SDKCALLER_H_

#include "SDKBaseCaller.h"
#include "JsonLibUtil.h"
#include "TMSDKPlatformUtils.h"

#if PLATFORM_ANDROID
#include "Android/AndroidJNI.h"
#include "Android/AndroidApplication.h"
#endif

class TMSDKBRIDGE_API SDKCaller : public SDKBaseCaller<SDKCaller>
{
	friend class SDKBaseCaller<SDKCaller>;

	template<typename T>
	SDKCallResult& _Call(SDKCallInfo& callInfo)
	{
#if PLATFORM_ANDROID
		FString resultStr;
		FString callInfoStr;
		callInfo.ConvertToJson(callInfoStr);
		UE_LOG(LogTemp, Log, TEXT("Call Info : %s"), *callInfoStr);

		if (JNIEnv * env = FAndroidApplication::GetJavaEnv())
		{
			jstring cinfo = env->NewStringUTF(TCHAR_TO_UTF8(*callInfoStr));
			jstring res = (jstring)FJavaWrapper::CallObjectMethod(env, FJavaWrapper::GameActivityThis, ACallAndroidMethod, cinfo);
			env->DeleteLocalRef(cinfo);

			resultStr = TMSDKPlatformUtils::FStringFromLocalRef(env, res);
		}

		UE_LOG(LogTemp, Log, TEXT("CallResult Info : %s"), *resultStr);

		//FJsonObject* jObj = new FJsonObject();
		TSharedPtr<FJsonObject> jObj = MakeShareable(new FJsonObject());
		JsonLibUtil::ToObject<TSharedPtr<FJsonObject>>(resultStr, jObj);
		callRes = new SDKCallResultTemp<T>(*jObj, "obj");
		callRes->SetCode(jObj->GetIntegerField("code"));
		callRes->SetMsg(jObj->GetStringField("message"));
		return *callRes;
#else
		return *callRes;
#endif
	}

public:
	SDKCaller();
	~SDKCaller();

#if PLATFORM_ANDROID
	static jmethodID AInitCallAndroidMethod;
	static jmethodID ACallAndroidMethod;


	//通用
	static jmethodID AGetVersionCodeMethod;
	static jmethodID ARequestRecordVoicePermissions;
	static jmethodID ARequestSDCardPermissions;
	static jmethodID ASetMultiPointerCount;
#endif

	FString GetVersionName() override;
	void RequestRecordVoicePermissions() override;
	void RequestSDCardPermissions() override;
	void SetMultiPointerCount(int count) override;
};

#endif //_SDKCALLER_H_