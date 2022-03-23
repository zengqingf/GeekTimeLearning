// Fill out your copyright notice in the Description page of Project Settings.


#include "Plugins/GCloud/GCloudManager.h"

#include "Plugins/GCloud/GCloudComponentCreator.h"
#include "Plugins/GCloud/GCloudCommonDefine.h"

#include "UtilityMarcoDefine.h"


GCloudManager::GCloudManager():
	mSdkInfo(nullptr), mCreator(nullptr), mIsGCloudInited(false)
{
	if (nullptr == mCreator)
	{
		mCreator = new GCloudComponentCreator();
	}
	//gcloud需要先检查网络是否通
	//tencent feedback:
	//网络不通的场景下，有重试的逻辑，主要卡在了域名解析环节，目前采用的是local dns，所以阻塞在这里
	//这里是系统api，是一个同步接口，所以不能设置超时
	_checkNetReachablity();
}

GCloudManager::~GCloudManager()
{
	Uninit();
	UE_LOG(LogTemp, Log, TEXT("### gcloud mgr dtor"));
}

void GCloudManager::Init()
{
	if (!mIsGCloudInited)
	{
		//数据上报早点开
		_initTDM();
		_initGCloud(TM_isDebug);
		_initSDKInfo();
	}
}

void GCloudManager::Uninit()
{
	if (mCreator)
	{
		mCreator->Destroy(mInitedComponentType);
		mInitedComponentType = 0;							//反初始化后去除全部类型
	}

	SAFE_DELETE_PTR(mSdkInfo);
	SAFE_DELETE_PTR(mCreator);
	mIsGCloudInited = false;
	mIsNetReachability = false;
}

bool GCloudManager::Tick(float DeltaTime)
{
	if (!mIsNetReachability)
	{
		return false;
	}
	if (!mIsGCloudInited) 
	{
		return false;
	}
	if (mCreator)
	{
		auto comMap = mCreator->GetSDKComponents();
		if (comMap.size() == 0)
		{
			return false;
		}
		for (auto& comPair : comMap)
		{
			SDKComponent* com = comPair.second;
			if (nullptr == com) {
				continue;
			}
			if (!(mInitedComponentType & com->Type())			//判断对应类型是否初始化
				&& mSdkInfo != nullptr)		
			{
				com->Init(*mSdkInfo, TM_isDebug);
				mInitedComponentType |= com->Type();			//保存已经初始化的类型
			}
			com->Tick();
		}
	}
	return true;
}

SDKComponent* GCloudManager::EnableComponent(SDKComponentType type)
{
	SDKComponent* com = nullptr;
	if (mCreator != nullptr)
	{
		com = mCreator->Create(type);
	}
	return com;
}

void GCloudManager::DisableComponent(SDKComponentType type)
{
	if (mCreator)
	{
		mCreator->Destroy(type);
		if (mInitedComponentType > (int)SDKComponentType::NONE)
		{
			mInitedComponentType ^= type;						//反初始化后去除对应类型
		}
	}
}

void GCloudManager::SetNetReachability(bool bReach)
{
	if (bReach)
	{
		Init();
	}
	else
	{
		PluginManager::Instance().EventManager().TriggerEvent(SDKEventType::AllUpdateEnd);
	}
	mIsNetReachability = bReach;
}

FString GCloudManager::GetResVersionFromConfig()
{
	FString currResVer;
	FString relPathRoot(TEXT("TMSDKTool/TMSDK/PlatformRes/All"));
	FString relFileName(TEXT("dolphin/update_info.json"));
	FString fs = LoadFileInPluginsPath(relPathRoot, relFileName);
	if (!fs.IsEmpty())
	{
		TSharedRef<TJsonReader<>> freader = TJsonReaderFactory<>::Create(fs);
		TSharedPtr<FJsonObject> fobjPtr;
		bool succ = FJsonSerializer::Deserialize(freader, fobjPtr);
		if (succ)
		{
			currResVer = fobjPtr->GetStringField(TEXT("resVersion"));
			UE_LOG(LogTemp, Log, TEXT("### read file json: resVersion: %s"), *currResVer);
		}
	}
	//资源版本和程序版本不能一样！ 且两个版本号不能为空
	if (currResVer.IsEmpty())
	{
		//currResVer = PluginManager::Instance().GetVersionName();
		currResVer = TEXT("1.0.1.0");
	}
	return currResVer;
}

void GCloudManager::_initSDKInfo()
{
	if (nullptr == mSdkInfo)
	{
		FString versionNameStr = PluginManager::Instance().GetVersionName();
		FString resVersionNameStr = GetResVersionFromConfig();
		mSdkInfo = new SDKInfo();
		mSdkInfo->AppVersion = TCHAR_TO_UTF8(*versionNameStr);
		mSdkInfo->ResVersion = TCHAR_TO_UTF8(*resVersionNameStr);
		mSdkInfo->ChannelID = 3387;
		mSdkInfo->UpdateUrl = updateUrl;
		mSdkInfo->ServerID = TCHAR_TO_UTF8(TEXT("1"));
		mSdkInfo->OpenID = TCHAR_TO_UTF8(TEXT("10000"));

		UE_LOG(LogTemp, Log, TEXT("### init gcloud sdkinfo "));
	}
}

void GCloudManager::_initGCloud(bool isDebug)
{
	//GCloud Init
	IGCloud& ins = IGCloud::GetInstance();
	InitializeInfo initInfo;
	//gameid和gamekey参数只对PC游戏有效，对于移动端，sdk会通过配置文件获取gameid和gamekey
	initInfo.GameId = GAME_ID;
	initInfo.GameKey = GAME_KEY;
	EErrorCode errorCode = ins.Initialize(initInfo);
	UE_LOG(LogTemp, Log, TEXT("### try init gcloud... errorcode: %d"), errorCode);
	if (isDebug)
		ins.SetLogger(LogPriority::kDebug);
	else
		ins.SetLogger(LogPriority::kError);
	mIsGCloudInited = errorCode == 0;

	UE_LOG(LogTemp, Warning, TEXT("### gcloud sdk init %s"), mIsGCloudInited ? TEXT("success") : TEXT("failed"));
}

void GCloudManager::_initTDM()
{
	// TODO 隐私协议
	
	//腾讯埋点服务 目前设置为只支持android和ios
#if PLATFORM_ANDROID || PLATFORM_IOS
	TDM::ITDataMaster::GetInstance()->EnableReport(true);
	TDM::ITDataMaster::GetInstance()->EnableDeviceInfo(true);
#endif
}

void GCloudManager::_checkNetReachablity()
{
	PluginManager::Instance().CheckNetReachablity(UTF8_TO_TCHAR(updateBaseUrl), [](bool bEnable) ->void
		{
			UE_LOG(LogTemp, Log, TEXT("### Network type: %s, addressable: %s"), *(PluginManager::Instance().GetNetworkType()),
				bEnable ? TEXT("enable") : TEXT("disable"));
			PluginManager::Instance().GCloudMgr().SetNetReachability(bEnable);
		});
}
