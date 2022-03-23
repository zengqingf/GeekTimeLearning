// Fill out your copyright notice in the Description page of Project Settings.

#include "Plugins/PluginManager.h"
#include "Kismet/KismetSystemLibrary.h"
#include "HAL/PlatformFilemanager.h"
#include "Engine.h"
#include "HttpModule.h"
#include "Interfaces/IHttpResponse.h"

#if PLATFORM_ANDROID
# include "Android/AndroidPlatformMisc.h"
#include "Runtime/Launch/Public/Android/AndroidJNI.h"
#include "Runtime/ApplicationCore/Public/Android/AndroidApplication.h"
#endif
#if PLATFORM_IOS
# include "IOS/IOSPlatformMisc.h"
#endif

//#include "Common.h"
#include "UtilityMarcoDefine.h"

#include "TMSDKPlatformUtils.h"
#include "TMSDKCommon.h"

//Bugly
#include "Plugins/Bugly/BuglyManager.h"
//GVoice
#include "Plugins/GVoice/GVoiceManager.h"
//GCloud
#include "Plugins/GCloud/GCloudManager.h"

using namespace TenmoveSDK;

#if UE_BUILD_SHIPPING
bool TM_isDebug = false;
#else
bool TM_isDebug = true;
#endif


PluginManager::PluginManager()
{
	mEventManager = new TMSDKEventManager();
	FTMSDKCommonModule::OnMainModule.BindLambda([&]()
	{
		return mEventManager;	
	});
}

PluginManager::~PluginManager()
{
	SAFE_DELETE_PTR(mEventManager);
}

PluginManager& PluginManager::Instance()
{
	static PluginManager mInstance;
	return mInstance;
}

void PluginManager::PostLog(FString stackTrace)
{
	BuglyManager::PostLog2Bugly(stackTrace,true);
}

GVoiceManager& PluginManager::GVoiceMgr()
{
	GVoiceManager* gvoiceMgr = nullptr;
	IPluginMgr::Type type = IPluginMgr::GVOICE;
	if (mPluginMgrMap.find(type) != mPluginMgrMap.end())
	{
		gvoiceMgr = static_cast<GVoiceManager*>(mPluginMgrMap[type]);
	}
	else
	{
		gvoiceMgr = new GVoiceManager();
		mPluginMgrMap.emplace(type, gvoiceMgr);
	}
	return *gvoiceMgr;
}

GCloudManager& PluginManager::GCloudMgr()
{
	GCloudManager* gcloudMgr = nullptr;
	IPluginMgr::Type type = IPluginMgr::GCLOUD;
	if (mPluginMgrMap.find(type) != mPluginMgrMap.end())
	{
		gcloudMgr = static_cast<GCloudManager*>(mPluginMgrMap[type]);
	}
	else
	{
		gcloudMgr = new GCloudManager();
		mPluginMgrMap.emplace(type, gcloudMgr);
	}
	return *gcloudMgr;
}

TMSDKEventManager& PluginManager::EventManager()
{
	if (nullptr == mEventManager)
	{
		mEventManager = new TMSDKEventManager();
	}
	return *mEventManager;
}

void PluginManager::InitPlugins()
{
	InitBugly();
	CleanPsoDir();
	_allowScreensaver(false);
}

void PluginManager::UninitPlugins()
{
	if (mEventManager != nullptr)
	{
		mEventManager->ClearEvent();
	}

	if (mPluginMgrMap.size())
	{
		for (auto& pluginPair : mPluginMgrMap)
		{
			if (pluginPair.second != nullptr)
			{
				pluginPair.second->Uninit();
				SAFE_DELETE_PTR(pluginPair.second);
			}
		}
	}
	mPluginMgrMap.clear();

	//置空
	mVersionName = TEXT("");
}

void PluginManager::TickPlugins(float deltaSeconds)
{
	if (mPluginMgrMap.size())
	{
		for (auto& pluginPair : mPluginMgrMap)
		{
			if (nullptr == (pluginPair.second))
			{
				continue;
			}
			if (!pluginPair.second->Tick(deltaSeconds))
			{
				continue;
			}
		}
	}
}

void PluginManager::InitBugly()
{
	BuglyManager* buglyMgr = nullptr;
	IPluginMgr::Type type = IPluginMgr::BUGLY;
	if (mPluginMgrMap.find(type) != mPluginMgrMap.end())
	{
		buglyMgr = static_cast<BuglyManager*>(mPluginMgrMap[type]);
	}
	else
	{
		buglyMgr = new BuglyManager();
		mPluginMgrMap.emplace(type, buglyMgr);
	}
	if (buglyMgr != nullptr)
	{
		buglyMgr->InitBugly();
	}
}

const FString& PluginManager::GetSysDateTime()
{
	mSystemDateTime = FDateTime::Now().ToString(TEXT("%Y-%m-%d_%H%M%S"));
	return mSystemDateTime;
}

const FString& PluginManager::GetSysTime()
{
	mSystemTime = FDateTime::Now().ToString(TEXT("%H:%M:%S"));
	return  mSystemTime;
}

const FString& PluginManager::GetNetworkType()
{
	mNetworkType = "Unkown";
	ENetworkConnectionType T = ENetworkConnectionType::None;
#if PLATFORM_ANDROID
	T = FAndroidMisc::GetNetworkConnectionType();
#endif
#if PLATFORM_IOS
	T = FIOSPlatformMisc::GetNetworkConnectionType();
#endif
	switch (T)
	{
	case ENetworkConnectionType::None:				mNetworkType = "None";
		break;
	case ENetworkConnectionType::AirplaneMode:		mNetworkType = "AirplaneMode";
		break;
	case ENetworkConnectionType::Ethernet:			mNetworkType = "Ethernet";
		break;
	case ENetworkConnectionType::Cell:				mNetworkType = "4G";
		break;
	case ENetworkConnectionType::WiFi:				mNetworkType = "WiFi";
		break;
	case ENetworkConnectionType::WiMAX:				mNetworkType = "WiMAX";
		break;
	case  ENetworkConnectionType::Bluetooth:		mNetworkType = "Bluetooth";
		break;
	}
	return mNetworkType;
}

int32 PluginManager::GetBatteryLevel()
{
	int32 BatteryLevel = 0;
#if PLATFORM_ANDROID
	BatteryLevel = FAndroidMisc::GetBatteryLevel();
#endif
#if PLATFORM_IOS
	BatteryLevel = FIOSPlatformMisc::GetBatteryLevel();
#endif
	return BatteryLevel;
}

void PluginManager::CleanPsoDir()
{
	IPlatformFile& PlatformFile = FPlatformFileManager::Get().GetPlatformFile();
	FString PsoFiles = FPaths::ProjectSavedDir() / TEXT("CollectedPSOs") + FString("/") + "*.rec.upipelinecache";
	TArray<FString> FindedFiles;
	IFileManager::Get().FindFiles(FindedFiles, *PsoFiles, true, false);
	FString SearchFile = "";
	for (int i = 0; i < FindedFiles.Num(); i++)
	{
		SearchFile = FPaths::ProjectSavedDir() / TEXT("CollectedPSOs") + FString("/") + FindedFiles[i];
		if (PlatformFile.FileExists(*SearchFile))
		{
			if (PlatformFile.DeleteFile(*SearchFile))
			{
				UE_LOG(LogTemp, Warning, TEXT("[pso]clean path : %s success"), *SearchFile);
			}
			else
			{
				UE_LOG(LogTemp, Warning, TEXT("[pso]clean path : %s failed"), *SearchFile);
			}
		}
		else
		{
			UE_LOG(LogTemp, Warning, TEXT("[pso]not exist clean path : %s "), *SearchFile);
		}
	}
}

void PluginManager::UploadPsoCachetoFtp()
{
	bool ref = false;
	FString FilePath = "";
	FString PsoFiles = FPaths::ProjectSavedDir() / TEXT("CollectedPSOs") + FString("/") + "*.rec.upipelinecache";
	TArray<FString> FindedFiles;
	IFileManager::Get().FindFiles(FindedFiles, *PsoFiles, true, false);
	FString SearchFile = "";
	for (int i = 0; i < FindedFiles.Num(); i++)
	{
		SearchFile = FPaths::ProjectSavedDir() / TEXT("CollectedPSOs") + FString("/") + FindedFiles[i];
		FString leftS;
		FString rightS;
		SearchFile.Split("../", &leftS, &rightS, ESearchCase::Type::IgnoreCase, ESearchDir::Type::FromEnd);
		UE_LOG(LogTemp, Warning, TEXT("FindedFiles Path : %s"), *FPaths::ConvertRelativePathToFull(SearchFile));
		GEngine->AddOnScreenDebugMessage(-1, 5, FColor::Red, SearchFile);
#if PLATFORM_ANDROID
		extern FString GFilePathBase;
		FilePath = GFilePathBase + FString("/UE4Game/") + FApp::GetProjectName() + "/" + rightS;
#endif
	}
	IPlatformFile& PlatformFile = FPlatformFileManager::Get().GetPlatformFile();
	UE_LOG(LogTemp, Warning, TEXT("Upload Path : %s"), *FilePath);
#if PLATFORM_ANDROID

	if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
	{
		static jmethodID MethonId_Test2 = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "UploadPSO", "(Ljava/lang/String;)Z", false);
		jstring s1 = Env->NewStringUTF(TCHAR_TO_UTF8(*FilePath));
		ref = FJavaWrapper::CallBooleanMethod(Env, FJavaWrapper::GameActivityThis, MethonId_Test2, s1);
		Env->DeleteLocalRef(s1);
	}
#endif
	if (ref)
	{
		GEngine->AddOnScreenDebugMessage(-1, 6, FColor::Red, "upload Successful !!!", true, FVector2D(2.0f, 2.0f));
	}
	else
	{
		GEngine->AddOnScreenDebugMessage(-1, 6, FColor::Red, "upload Failed !!!", true, FVector2D(2.0f, 2.0f));
	}
}

void PluginManager::CheckNetReachablity(const FString& url, NetReachabilityCallback callback)
{
	float tempTimeout = FHttpModule::Get().GetHttpTimeout();
	FHttpModule::Get().SetHttpTimeout(3);
	TSharedRef<IHttpRequest> req = FHttpModule::Get().CreateRequest();
	req->SetHeader(TEXT("Content-Type"), TEXT("application/json; charset=utf-8"));
	req->SetURL(*(FString::Printf(TEXT("https://%s"), *url)));
	req->SetVerb(TEXT("Post"));
	req->OnProcessRequestComplete().BindLambda([=](FHttpRequestPtr request, FHttpResponsePtr response, bool bSucceeded)->void
		{
			UE_LOG(LogTemp, Warning, TEXT("### CheckNetAddressable OnProcessRequestComplete, req url: %s, http req result: %s, http response error code: %d, elapsed time: %f"),
				*url, bSucceeded ? TEXT("success"): TEXT("failed"), (response != nullptr) ? response->GetResponseCode() : -1, request->GetElapsedTime());

			request->CancelRequest();
			FHttpModule::Get().SetHttpTimeout(tempTimeout);
			if (nullptr == callback)
			{
				return;
			}
			if (!bSucceeded || !response.IsValid())
			{
				callback(false);
				return;
			}
			if (EHttpResponseCodes::IsOk(response->GetResponseCode()))
			{		
				callback(true);
			}
			else
			{
				callback(false);
			}
		});
	req->OnRequestWillRetry().BindLambda([=](FHttpRequestPtr request, FHttpResponsePtr response, float secondsToRetry)->void
		{
			UE_LOG(LogTemp, Warning, TEXT("### CheckNetAddressable OnRequestWillRetry, req url: %s, seconds: %f to retry, http response error code: %d, elapsed time: %f"),
				*url, secondsToRetry, (response != nullptr) ? response->GetResponseCode() : -1, request->GetElapsedTime());
		}
	);
	req->ProcessRequest();
}

/// <summary>
/// 是否允许息屏
/// </summary>
/// <param name="enable"></param>
void PluginManager::_allowScreensaver(bool enable)
{
	UKismetSystemLibrary::ControlScreensaver(enable);
}


const FString& PluginManager::GetVersionName()
{
	if (!mVersionName.IsEmpty())
	{
		return mVersionName;
	}
#if PLATFORM_WINDOWS
	#if WITH_EDITOR
	const bool bLaunchDetached = false;
	const bool bLaunchHidden = true;
	void* ReadPipe = nullptr, * WritePipe = nullptr;

	char buffer[296] = {};

	_snprintf_s(buffer, 296, "info --show-item last-changed-revision %s", TCHAR_TO_ANSI(*FPaths::ProjectDir()));

	UE_LOG(LogTemp, Log, TEXT("### get version name on windows or editor by svn info in dir: %s"), *(FPaths::ProjectDir()));

	FPlatformProcess::CreatePipe(ReadPipe, WritePipe);
#if PLATFORM_WINDOWS
	const FString DefaultPath = FPaths::EngineDir() / TEXT("Binaries/ThirdParty/svn") / FPlatformProcess::GetBinariesSubdirectory() / TEXT("svn.exe");
#elif PLATFORM_MAC
	const FString DefaultPath = FPaths::EngineDir() / TEXT("Binaries/ThirdParty/svn") / FPlatformProcess::GetBinariesSubdirectory() / TEXT("bin/svn");
#else
	const FString DefaultPath = TEXT("/usr/bin/svn");
#endif
	FProcHandle ProcHandle = FPlatformProcess::CreateProc(*DefaultPath, *FString(buffer), bLaunchDetached, bLaunchHidden, bLaunchHidden, NULL, 0, NULL, WritePipe);
	if (ProcHandle.IsValid())
	{
		FPlatformProcess::WaitForProc(ProcHandle);
		mVersionName = FPlatformProcess::ReadPipe(ReadPipe);

		int32 NewLine = INDEX_NONE;
		// windows: 123456\r\n
		if (mVersionName.FindChar('\r', NewLine))
		{
			mVersionName.LeftInline(NewLine);
		}
		if (mVersionName.FindChar('\n', NewLine))
		{
			mVersionName.LeftInline(NewLine);
		}

		//临时格式
		mVersionName = FString::Printf(TEXT("1.0.1.%s"), *mVersionName);
	}
	FPlatformProcess::ClosePipe(ReadPipe, WritePipe);
	FPlatformProcess::CloseProc(ProcHandle);
	#else
	mVersionName = TEXT("1.0.1.0");
	#endif
#elif PLATFORM_ANDROID
	GConfig->GetString(
		TEXT("/Script/AndroidRuntimeSettings.AndroidRuntimeSettings"),
		TEXT("VersionDisplayName"),
		mVersionName,
		GEngineIni   //FPaths::ProjectConfigDir() / "DefaultEngine.ini" also able
	);
	//also able
	//if (JNIEnv* env = FAndroidApplication::GetJavaEnv())
	//{
	//	static jmethodID Method_GetVersionName = FJavaWrapper::FindMethod(env, FJavaWrapper::GameActivityClassID, "getVersionName", "()Ljava/lang/String;", false);
	//	jstring res = (jstring)FJavaWrapper::CallObjectMethod(env, FJavaWrapper::GameActivityThis, Method_GetVersionName);
	//	versionName = TMSDKPlatformUtils::FStringFromLocalRef(env, res);
	//}
	//UE_LOG(LogTemp, Log, TEXT("### in version name: %s"), *versionName);

#elif PLATFORM_IOS
	GConfig->GetString(
		TEXT("/Script/IOSRuntimeSettings.IOSRuntimeSettings"),
		TEXT("VersionInfo"),
		mVersionName,
		GEngineIni
	);
#endif
	return mVersionName;
}

bool PluginManager::IsPluginFunctionOpen(FString funcName)
{
	bool isOpen = false;
	FString relPathRoot(TEXT("TMSDKTool/TMSDK/PlatformRes/All"));
	FString relFileName(TEXT("plugin_functions.json"));
	FString fs = LoadFileInPluginsPath(relPathRoot, relFileName);
	if (!fs.IsEmpty())
	{
		TSharedRef<TJsonReader<>> freader = TJsonReaderFactory<>::Create(fs);
		TSharedPtr<FJsonObject> fobjPtr;
		bool succ = FJsonSerializer::Deserialize(freader, fobjPtr);
		if (succ)
		{
			isOpen = fobjPtr->GetBoolField(TEXT("game_update"));
			UE_LOG(LogTemp, Log, TEXT("### read file json: %s is %s"), *funcName, isOpen ? TEXT("open"): TEXT("close"));
		}
	}
	return isOpen;
}

void PluginManager::SetMultiPointerCount(int pointerCount)
{
	TMSDKManager::GetInstance().SetMultiPointerCount(pointerCount);
}

/*
void PluginManager::SetVersionName(const FString& versionName)
{
#if PLATFORM_ANDROID
GConfig->SetString(
	TEXT("/Script/AndroidRuntimeSettings.AndroidRuntimeSettings"),
	TEXT("VersionDisplayName"),
	*versionName,
	GEngineIni   //FPaths::ProjectConfigDir() / "DefaultEngine.ini" also able
);
GConfig->Flush(true, GEngineIni);
#elif PLATFORM_IOS
GConfig->SetString(
	TEXT("/Script/IOSRuntimeSettings.IOSRuntimeSettings"),
	TEXT("VersionInfo"),
	*versionName,
	GEngineIni
);
GConfig->Flush(true, GEngineIni);
#endif
}
*/