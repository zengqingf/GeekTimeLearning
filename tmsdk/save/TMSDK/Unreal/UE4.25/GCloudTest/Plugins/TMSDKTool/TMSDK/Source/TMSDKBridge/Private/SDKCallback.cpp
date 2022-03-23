#include "SDKCallback.h"
#include "TMSDKCommon.h"
#include "TMSDKPlatformUtils.h"
using namespace TenmoveSDK;

#if PLATFORM_ANDROID

//#ifndef _SDK_CALLBACK
//#define _SDK_CALLBACK

//#ifdef __cplusplus
//extern "C" {
//#endif //__cplusplus

	JNI_METHOD void Java_com_tm_sdk_bridge_app_BridgeUnrealApp_Callback
	(JNIEnv* jenv, jobject thiz, jstring param)
	{
		UE_LOG(LogTemp, Log, TEXT("### SDK Callback : Java_com_tm_sdk_bridge_app_BridgeUnrealApp_Callback"));
		FString resultStr = TMSDKPlatformUtils::FStringFromLocalRef(jenv, param);
		//FJsonObject* jsonObj = new FJsonObject();
		TSharedPtr<FJsonObject> jsonObj = MakeShareable(new FJsonObject());
		JsonLibUtil::ToObject<TSharedPtr<FJsonObject>>(resultStr, jsonObj);
		if (nullptr == jsonObj) {
			UE_LOG(LogTemp, Error, TEXT("### SDK Callback to Json failed, result str : %s"), *resultStr);
			return;
		}
		UE_LOG(LogTemp, Log, TEXT("### SDK Callback to Json success, result str : %s"), *resultStr);
		//TODO
	}


	JNI_METHOD void Java_com_epicgames_ue4_GameActivity_nativeTouchScreenPos(JNIEnv* jenv, jobject thiz, jfloat posx, jfloat posy)
	{
		UE_LOG(LogTemp, Log, TEXT("### SDK Callback nativeTouchScreenPos x= %f and y= %f"), posx, posy);
		//SDKEventParam param;
		//param.float_0 = posx;
		//param.float_1 = posy;
		//TODO
		//TMSDKEventManager::GetInstance()->TriggerEvent(SDKEventType::GetTouchScreenPos, param);
	}

	JNI_METHOD void Java_com_epicgames_ue4_GameActivity_nativeMultiTouchEvent(JNIEnv* jenv, jobject thiz)
	{
		UE_LOG(LogTemp, Log, TEXT("### SDK Callback nativeMultiTouchEvent !!!"));
		if(nullptr != FTMSDKCommonModule::GetEventManager())
		{
			FTMSDKCommonModule::GetEventManager()->TriggerEvent(SDKEventType::GetMultiTouchEvent);
		}
	}

	/*
	//Test
	JNI_METHOD void Java_com_tm_sdk_ue4bridge_AndroidCaller_SetCallback
	(JNIEnv* jenv, jobject thiz, jobject classref)
	{
		jclass jc = jenv->GetObjectClass(classref);
		jmethodID mid = jenv->GetMethodID(jc, "GameCallback", "(Ljava/lang/String;)Ljava/lang/String;");
		if (mid == 0) {
			UE_LOG(TMSDK, Error, TEXT("could not get method id ! , get method name is %s"), "GameCallback");
			return;
		}
		jstring result = (jstring)jnienv->CallObjectMethod(classref, mid, jnienv->NewStringUTF("hello jni"));
		const char* nativeresult = jnienv->GetStringUTFChars(result, 0);
		printf("Echo from GameCallback: %s", nativeresult);
		jnienv->ReleaseStringUTFChars(result, nativeresult);
	}
	*/

	/*
	jobject g_callbackInterface;
	jmethodID g_method;
	JNIEnv* g_jenv;

	JNI_METHOD void Java_com_tm_sdk_ue4bridge_AndroidCaller_CallUE4
	(JNIEnv* jenv, jobject thiz, jstring param, jobject callbackInterface)
	{
		jclass objclass = jenv->GetObjectClass(callbackInterface);
		jmthodID method = jenv->GetMethodID(objclass, "Callback". "(Ljava/lang/String;)V");
		if (method == 0) {
			return;
		}
		g_method = method;
		g_callbackInterface = callbackInterface;
		g_jenv = jenv;
	}
	*/

	//MyCPPFunction(const FString &param, void (*callback)(const FString &json));
	/*
	void WrapperFunc(const FString &param)
	{
		jint retval;
		//marshalling an int* to a m_SizeClass boogy-woogy.

		g_env->ExceptionClear();
		retval = g_env->CallIntMethod(g_getSizeIface, g_method, );
		if (g_env->ExceptionOccurred()) {
			//panic! Light fires! The British are coming!!!

			g_env->ExceptionClear();
		}
		return rvalue;
	}
	*/

//#ifdef __cplusplus
//}
//#endif //__cplusplus

//#endif //_SDK_CALLBACK

#endif //PLATFORM_ANDROID