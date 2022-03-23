#include "SDKCaller.h"

#include "TMSDKPlatformUtils.h"


#if PLATFORM_ANDROID
jmethodID SDKCaller::ACallAndroidMethod;
jmethodID SDKCaller::AInitCallAndroidMethod;

jmethodID SDKCaller::AGetVersionCodeMethod;
jmethodID SDKCaller::ARequestRecordVoicePermissions;
jmethodID SDKCaller::ARequestSDCardPermissions;
jmethodID SDKCaller::ASetMultiPointerCount;
#endif

SDKCaller::SDKCaller()
{
#if PLATFORM_ANDROID
	if (JNIEnv *env = FAndroidApplication::GetJavaEnv())
	{
		AInitCallAndroidMethod = FJavaWrapper::FindMethod(env, FJavaWrapper::GameActivityClassID, "AndroidThunk_Java_InitCall", "()V", false);
		ACallAndroidMethod = FJavaWrapper::FindMethod(env, FJavaWrapper::GameActivityClassID, "AndroidThunk_Java_CallAndroid", "(Ljava/lang/String;)Ljava/lang/String;", false);


		FJavaWrapper::CallVoidMethod(env, FJavaWrapper::GameActivityThis, AInitCallAndroidMethod);
		UE_LOG(LogTemp, Log, TEXT("Init Android Call..."));
	}


	//通用
	if (JNIEnv* env = FAndroidApplication::GetJavaEnv())
	{
		AGetVersionCodeMethod = FJavaWrapper::FindMethod(env, FJavaWrapper::GameActivityClassID, "getVersionName", "()Ljava/lang/String;", false);

		ARequestRecordVoicePermissions = FJavaWrapper::FindMethod(env, FJavaWrapper::GameActivityClassID, "requestRecordVoicePermissions", "()V", false);
		ARequestSDCardPermissions = FJavaWrapper::FindMethod(env, FJavaWrapper::GameActivityClassID, "requestSDCardPermissions", "()V", false);
		ASetMultiPointerCount = FJavaWrapper::FindMethod(env, FJavaWrapper::GameActivityClassID, "setMultiPointerCount", "(I)V", false);
	}


#endif

}

SDKCaller::~SDKCaller()
{
}

//SDKCallResult& SDKCaller::Call(SDKCallInfo& callInfo)
//{
//
//}

FString SDKCaller::GetVersionName()
{
	FString versionName;
#if PLATFORM_ANDROID
	if (JNIEnv* env = FAndroidApplication::GetJavaEnv())
	{
		jstring res = (jstring)FJavaWrapper::CallObjectMethod(env, FJavaWrapper::GameActivityThis,AGetVersionCodeMethod);
		versionName = TMSDKPlatformUtils::FStringFromLocalRef(env, res);
		UE_LOG(LogTemp, Log, TEXT("### in version name: %s"), *versionName);
	}
#endif
	return versionName;
}

void SDKCaller::RequestRecordVoicePermissions()
{
#if PLATFORM_ANDROID
	if (JNIEnv* env = FAndroidApplication::GetJavaEnv())
	{
		FJavaWrapper::CallVoidMethod(env, FJavaWrapper::GameActivityThis, ARequestRecordVoicePermissions);
		UE_LOG(LogTemp, Log, TEXT("### request record voice permissions"));
	}
#endif
}

void SDKCaller::RequestSDCardPermissions()
{
#if PLATFORM_ANDROID
	if (JNIEnv* env = FAndroidApplication::GetJavaEnv())
	{
		FJavaWrapper::CallVoidMethod(env, FJavaWrapper::GameActivityThis, ARequestSDCardPermissions);
		UE_LOG(LogTemp, Log, TEXT("### request sdcard permissions"));
	}
#endif
}

void SDKCaller::SetMultiPointerCount(int count)
{
#if PLATFORM_ANDROID
	if (JNIEnv* env = FAndroidApplication::GetJavaEnv())
	{
		FJavaWrapper::CallVoidMethod(env, FJavaWrapper::GameActivityThis, ASetMultiPointerCount, count);
		UE_LOG(LogTemp, Log, TEXT("### set multi pointer count: %d"), count);
	}
#endif
}
