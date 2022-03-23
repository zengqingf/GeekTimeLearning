// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "TMSDKManager.h"
#include "TMSDKEventManager.h"

#include <unordered_map>

extern bool TM_isDebug;

/**
 * 
 */
class IPluginMgr
{
public:
	enum Type
	{
		NONE = 0x00,
		BUGLY = 0x01,
		GCLOUD = 0x02,
		GVOICE = 0x04,
	};

public:
	virtual ~IPluginMgr() {};
	virtual void Init() = 0;
	virtual void Uninit() = 0;
	virtual bool Tick(float DeltaTime) = 0;
};

typedef void (*NetReachabilityCallback)(bool bReach);

class GCLOUDTEST_API PluginManager final
{
public:
	static PluginManager& Instance();

	//Bugly
	static void PostLog(FString stackTrace);
	void InitBugly();

	//Get Some DeviceInfo
	const FString& GetSysDateTime();
	const FString& GetSysTime();
	const FString& GetNetworkType();
	int32 GetBatteryLevel();

	//清理手机pso缓存目录
	void CleanPsoDir();
	//上传手机pso缓存文件到ftp
	void UploadPsoCachetoFtp();

	void CheckNetReachablity(const FString& url, NetReachabilityCallback callback);

	//获取版本号
	const FString& GetVersionName();

	//Utils
	static bool IsPluginFunctionOpen(FString funcName);

	//设置多点触控点触发需要数量
	void SetMultiPointerCount(int pointerCount);

public:
	class GVoiceManager& GVoiceMgr();
	class GCloudManager& GCloudMgr();
	class TenmoveSDK::TMSDKEventManager& EventManager();

private:
	void _allowScreensaver(bool enable);

//管理器相关
public:
	void InitPlugins();
	void UninitPlugins();
	void TickPlugins(float deltaSeconds);
private:
	PluginManager();
	~PluginManager();

private:
	class TenmoveSDK::TMSDKEventManager* mEventManager = nullptr;
	 
	std::unordered_map<IPluginMgr::Type, IPluginMgr*> mPluginMgrMap;
	  
	FString mSystemDateTime;
	FString mSystemTime;
	FString mNetworkType;
	FString mVersionName;				//整包版本号（应用程序版本号）
};

namespace TenmoveSDK
{
	//加class后不能默认转int
	enum SDKComponentType
	{
		NONE = 0x00,
		GAME_UPDATE = 0x01,
		GAME_DIR_SERVER = 0x02,
		//GAME_VOICE = 0x04,   //GVoice不包括在GCloud内了

		TOTAL
	};

	struct GCLOUDTEST_API SDKInfo
	{
		uint32 ChannelID;
		std::string UpdateUrl;

		std::string OpenID;
		std::string ServerID;

		std::string AppVersion;
		std::string ResVersion;

		std::string ExtraData;
	};

	class GCLOUDTEST_API SDKComponent
	{
	public:
		SDKComponent() : mType(SDKComponentType::NONE){}
		SDKComponent(SDKComponentType type) : mType(type) {}
		virtual ~SDKComponent() {}
		virtual void Init(const SDKInfo& sdkInfo, bool isDebug) = 0;
		virtual void Uninit() = 0;
		virtual void Tick() = 0;

		SDKComponentType Type() const { return mType; }

	private:
		SDKComponentType mType;
	};

	class GCLOUDTEST_API SDKComponentCreator
	{
	public:
		using SDKComponentMap = std::unordered_map<SDKComponentType, SDKComponent*>;
		using SDKComponentMapIter = SDKComponentMap::iterator;
		using SDKComponentMapPair = std::pair<SDKComponentType, SDKComponent*>;

		SDKComponentCreator() {}
		virtual ~SDKComponentCreator() {}
		virtual SDKComponent* Create(SDKComponentType type) = 0;
		virtual void Destroy(SDKComponent* com) = 0;
		virtual void Destroy(SDKComponentType type) = 0;
		virtual void Destroy(int sdkComponentType) = 0;

		SDKComponentMap GetSDKComponents() const { return sdkComponentMap; }
	protected:
		SDKComponentMap sdkComponentMap;
	};
}