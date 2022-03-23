#include "SDKCaller.h"

#if PLATFORM_ANDROID
jmethodID SDKCaller::CallAndroidMethod;
jmethodID SDKCaller::InitCallAndroidMethod;
#endif

SDKCaller::SDKCaller()
{
#if PLATFORM_ANDROID
	if (JNIEnv *env = FAndroidApplication::GetJavaEnv())
	{
		InitCallAndroidMethod = FJavaWrapper::FindMethod(env, FJavaWrapper::GameActivityClassID, "AndroidThunk_Java_InitCall", "()V", false);
		CallAndroidMethod = FJavaWrapper::FindMethod(env, FJavaWrapper::GameActivityClassID, "AndroidThunk_Java_CallAndroid", "(Ljava/lang/String;)Ljava/lang/String;", false);


		FJavaWrapper::CallVoidMethod(env, FJavaWrapper::GameActivityThis, InitCallAndroidMethod);
		UE_LOG(LogTemp, Log, TEXT("Init Android Call..."));
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