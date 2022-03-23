#pragma once

#include "HAL/Platform.h"

#include "Plugins/PluginCommon.h"
#include "Plugins/IHotfix.h"
//Dolphin
#include "GCloudDolphinInterface.h"


using namespace GCloud;

class SDKDolphinBuilder;

/// <summary>
/// app更新流程：Init_AppUpdate->UpdateStart->UpdateLoop
/// 资源更新流程：Init_ResUpdate->UpdateStart->UpdateLoop
/// </summary>
class SDK_Dolphin : public GCloudDolphinCallBack
{
private:
	SDK_Dolphin();
public:
	~SDK_Dolphin();
	void UnInit();
	/// <summary>
	/// 开始更新
	/// </summary>
	void UpdateStart();
	/// <summary>
	/// 每帧调用的更新主循环
	/// </summary>
	void UpdateLoop();
	void Finish(const char* resNewVerion);
	void ContinueUpdate();
	void PauseUpdate();
	void Cancel();

	void SetDebugMode(bool openDebug);

	//app更新的初始化
	bool Init_AppUpdate();
	//资源更新的初始化
	bool Init_ResUpdate();

	//首包解压
	bool Init_FirstExtract_All();

	//资源修复-散资源修复(不使用IFS)
	bool InitNoIFSResCheck();

public:
	//GCloudDolphinCallBack
	virtual void OnDolphinVersionInfo(dolphinVersionInfo& newVersinInfo) override;
	virtual void OnDolphinNoticeInstallApk(char* apkurl) override;
	virtual void OnDolphinProgress(dolphinUpdateStage curVersionStage, cu_uint64 totalSize, cu_uint64 nowSize) override;
	virtual void OnDolphinError(dolphinUpdateStage curVersionStage, cu_uint32 errorCode) override;
	virtual void OnDolphinSuccess() override;
	virtual void OnDolphinServerCfgInfo(const char* config) override;
	virtual void OnDolphinIOSBGDownloadDone() override;
	virtual void OnActionMsgArrive(const char* msg) override;

private:
	bool isUpdating = false;
	bool isFinish = false;
	bool isSuc = false;

private:
	const char* appVersion;
	const char* resVersion;
	uint32 channelId;
	const char* userData;
	const char* userId;
	const char* worldId;
	const char* updateUrl;

	bool isDebug = false;
	std::string m_resNewVersion;

	GCloudDolphinInterface* mgr = nullptr;

	dolphinInitInfo* initInfo = nullptr;
	dolphinPathInfo* pathInfo = nullptr;
	dolphinGrayInfo* grayInfo = nullptr;
	dolphinFirstExtractInfo* firstExtractInfo = nullptr;
	GCloudDolphinCallBack* callBack = nullptr;
	HotfixCallBack* hotfixCallback = nullptr;

	bool Init_Internal();
	//设置初始化值
	void SetInfo_Internal(dolphinUpdateInitType initType);

	void _releaseDolphinMgr();

	//同步更新时的目录文件到自定义更新目录
	void _copyUpdateFilesToSaved(const char* savedRoot);


public:
	friend class SDKDolphinBuilder;
	static SDKDolphinBuilder& Create();
	static void Destroy();

private:
	static SDKDolphinBuilder* builder;
};


class SDKDolphinBuilder
{
private:
	SDK_Dolphin* dolphin = nullptr;

public:
	SDKDolphinBuilder() 
	{ 
		dolphin = new SDK_Dolphin();
	}

	~SDKDolphinBuilder()
	{
		delete dolphin;
		dolphin = nullptr;
	}

	operator SDK_Dolphin() const 
	{ 
		return *dolphin;
	}

	SDK_Dolphin* GetDolphin()
	{
		return dolphin;
	}

	SDKDolphinBuilder& SetVersionInfo(const char* appVer, const char* resVer);
	SDKDolphinBuilder& SetChannelId(uint32 channelId, const char* updateUrl);
	SDKDolphinBuilder& SetGrayInfo(const char* worldId, const char* userId);
	SDKDolphinBuilder& SetExtraUserInfo(const char* userData);
};