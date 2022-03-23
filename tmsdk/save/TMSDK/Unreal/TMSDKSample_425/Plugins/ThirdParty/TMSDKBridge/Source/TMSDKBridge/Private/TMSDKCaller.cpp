#include "TMSDKCaller.h"
#include "TMSDKBridge.h"

TMSDKCaller::TMSDKCaller()
{
}

TMSDKCaller::~TMSDKCaller()
{
}

TMSDKCaller& TMSDKCaller::GetInstance()
{
	static TMSDKCaller instance;
	return instance;
}

//SDKCallResult& TMSDKCaller::Call(SDKCallInfo& callInfo)
//{
//	SDKCallResult &callRes = FTMSDKBridgeModule::Get().GetSDKCaller()->Call(callInfo);
//	return callRes;
//}