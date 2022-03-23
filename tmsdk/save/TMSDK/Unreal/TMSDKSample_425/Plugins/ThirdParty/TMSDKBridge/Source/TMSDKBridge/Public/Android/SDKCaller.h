#pragma once

#ifndef _SDKCALLER_H_
#define _SDKCALLER_H_

#include "SDKBaseCaller.h"
#include "JsonLibUtil.h"

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
			jstring res = (jstring)FJavaWrapper::CallObjectMethod(env, FJavaWrapper::GameActivityThis, CallAndroidMethod, cinfo);
			env->DeleteLocalRef(cinfo);

			resultStr = FStringFromLocalRef(env, res);
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
	static jmethodID InitCallAndroidMethod;
	static jmethodID CallAndroidMethod;	

	static FString FStringFromParam(JNIEnv* Env, jstring JavaString)
	{
		if (!Env || !JavaString || Env->IsSameObject(JavaString, NULL))
		{
			return {};
		}
		const auto chars = Env->GetStringUTFChars(JavaString, 0);
		FString ReturnString(UTF8_TO_TCHAR(chars));
		Env->ReleaseStringUTFChars(JavaString, chars);
		return ReturnString;
	}

	static FString FStringFromLocalRef(JNIEnv* Env, jstring JavaString)
	{
		FString ReturnString = FStringFromParam(Env, JavaString);
		if (Env && JavaString)
		{
			Env->DeleteLocalRef(JavaString);
		}
		return ReturnString;
	}
#endif
};

#endif //_SDKCALLER_H_