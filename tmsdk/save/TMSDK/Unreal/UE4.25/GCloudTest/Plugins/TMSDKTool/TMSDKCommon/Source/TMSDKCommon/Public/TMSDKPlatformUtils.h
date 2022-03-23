// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

#include <string>

#if PLATFORM_ANDROID
#include "Android/AndroidJNI.h"
#include "Android/AndroidApplication.h"
#endif

/**
 * 
 */
class TMSDKCOMMON_API TMSDKPlatformUtils
{
public:
#if PLATFORM_ANDROID
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
