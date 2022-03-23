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

ISDKCaller& TMSDKCaller::GetCommonCaller()
{
	FTMSDKCallerPtr sdkCallerPtr = FTMSDKBridgeModule::Get().GetSDKCaller();
	return const_cast<ISDKCaller&>(static_cast<const ISDKCaller&>(*sdkCallerPtr));
}

//SDKCallResult& TMSDKCaller::Call(SDKCallInfo& callInfo)
//{
//	SDKCallResult &callRes = FTMSDKBridgeModule::Get().GetSDKCaller()->Call(callInfo);
//	return callRes;
//}