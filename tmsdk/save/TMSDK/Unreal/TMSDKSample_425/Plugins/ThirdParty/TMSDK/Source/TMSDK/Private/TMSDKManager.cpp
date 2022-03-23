#include "TMSDKManager.h"

TMSDKManager::TMSDKManager()
{
}

TMSDKManager::~TMSDKManager()
{
	if (buglySdk != nullptr) {
		buglySdk->UnInit();
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
	if (buglySdk == nullptr) {
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
