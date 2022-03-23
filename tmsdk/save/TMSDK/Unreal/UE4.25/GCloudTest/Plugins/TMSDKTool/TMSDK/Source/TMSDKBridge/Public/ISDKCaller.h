#pragma once

class TMSDKBRIDGE_API ISDKCaller
{
public:
	virtual ~ISDKCaller() {}

	//通用
	virtual FString GetVersionName() { return ""; }
	virtual void RequestRecordVoicePermissions() {}
	virtual void RequestSDCardPermissions() {}
	virtual void SetMultiPointerCount(int count) {}
};