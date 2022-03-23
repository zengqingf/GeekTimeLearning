#include "TMSDKManager.h"
#include "TMBuglySDK.h"
#include "TMSDKCaller.h"

TMSDKManager::TMSDKManager()
{
}

TMSDKManager::~TMSDKManager()
{
	if (buglySdk != nullptr) {
		buglySdk->Uninit();
		delete buglySdk;
		buglySdk = nullptr;
	}
}

//static
TMSDKManager& TMSDKManager::GetInstance()
{
	static TMSDKManager instance;
	return instance;
}

void TMSDKManager::Initialize()
{
	if (nullptr == buglySdk) {
		buglySdk = new TMBuglySDK();
		buglySdk->Init();
	}
}

void TMSDKManager::SetUserId(const FString& userId)
{
	if (buglySdk != nullptr) {
		buglySdk->SetUserId(userId);
	}
}

void TMSDKManager::RequestPermission(TenmoveSDK::PermissionType pType)
{
	switch (pType)
	{
	case TenmoveSDK::PermissionType::RecordVoice:
		TMSDKCaller::GetInstance().GetCommonCaller().RequestRecordVoicePermissions();
		break;
	case TenmoveSDK::PermissionType::SDCard:
		TMSDKCaller::GetInstance().GetCommonCaller().RequestSDCardPermissions();
		break;
	}
}

void TMSDKManager::SetMultiPointerCount(int count)
{
	TMSDKCaller::GetInstance().GetCommonCaller().SetMultiPointerCount(count);
}
