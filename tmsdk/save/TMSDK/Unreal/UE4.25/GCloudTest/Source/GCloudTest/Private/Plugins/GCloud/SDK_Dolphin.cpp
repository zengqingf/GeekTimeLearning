
#include "Plugins/GCloud/SDK_Dolphin.h"
#include "HAL/FileManagerGeneric.h"
#include "Plugins/GameHotfix.h"


#include "Plugins/GCloud/GCloudCommonDefine.h"


SDK_Dolphin::SDK_Dolphin()
{
	if (nullptr == this->hotfixCallback)
	{
		this->hotfixCallback = new GameHotfix();
	}
}
SDK_Dolphin::~SDK_Dolphin()
{
	UnInit();

	if (hotfixCallback != nullptr)
	{
		delete hotfixCallback;
		hotfixCallback = nullptr;
	}
}
void SDK_Dolphin::UnInit()
{
	_releaseDolphinMgr();

	isUpdating = false;
	isFinish = false;
	isSuc = false;

	isDebug = false;
	m_resNewVersion = "";

	delete initInfo;
	delete pathInfo;
	delete grayInfo;
	delete firstExtractInfo;

	initInfo = nullptr;
	pathInfo = nullptr;
	grayInfo = nullptr;
	firstExtractInfo = nullptr;
}
void SDK_Dolphin::UpdateStart()
{
	if(mgr)
		mgr->CheckAppUpdate();
}
void SDK_Dolphin::UpdateLoop()
{
	if (mgr)
		mgr->PollCallback();
}
void SDK_Dolphin::Finish(const char* resNewVerion)
{
	if (isFinish)
		return;
	isFinish = true;

	_releaseDolphinMgr();

	if (hotfixCallback)
	{
		hotfixCallback->Finish(resNewVerion);
	}
}
void SDK_Dolphin::ContinueUpdate()
{
	if (mgr)
		mgr->ContinueUpdate(true);
}
void SDK_Dolphin::PauseUpdate()
{
	if (mgr)
		mgr->ContinueUpdate(false);
}
void SDK_Dolphin::Cancel()
{
	if(mgr)
		mgr->CancelUpdate();
	UE_LOG(LogTemp, Warning, TEXT("### update cancel finish call"));
	Finish("");
}

void SDK_Dolphin::SetDebugMode(bool openDebug)
{
	this->isDebug = openDebug;
}

bool SDK_Dolphin::Init_AppUpdate()
{
	UnInit();
	SetInfo_Internal(dolphinUpdateInitType::UpdateInitType_OnlyProgram);
	return Init_Internal();
}

bool SDK_Dolphin::Init_ResUpdate()
{
	UnInit();
	SetInfo_Internal(dolphinUpdateInitType::UpdateInitType_OnlySource);
	return Init_Internal();
}

bool SDK_Dolphin::Init_FirstExtract_All()
{
	UnInit();
	SetInfo_Internal(dolphinUpdateInitType::UpdateInitType_FirstExtract_All);
	return Init_Internal();
}

bool SDK_Dolphin::InitNoIFSResCheck()
{
	UnInit();
	SetInfo_Internal(dolphinUpdateInitType::UpdateInitType_SourceCheckAndSync_Optimize_Full_Scatter);
	return Init_Internal();
}

void SDK_Dolphin::OnDolphinVersionInfo(dolphinVersionInfo& newVersinInfo)
{
	UE_LOG(LogTemp, Warning, TEXT("###获取到新版本用户字段:%s"), UTF8_TO_TCHAR(newVersinInfo.userDefineStr));
	FString newVersionStr = FString::Printf(TEXT("%d.%d.%d.%d"),
		newVersinInfo.versionNumberOne, newVersinInfo.versionNumberTwo, newVersinInfo.versionNumberThree, newVersinInfo.versionNumberFour);
	UE_LOG(LogTemp, Warning, TEXT("###获取到新版本, 版本号为:%s"), *newVersionStr);

	//isAppUpdating = true是APP应用更新
	//isAppUpdating = false是资源更新
	if (newVersinInfo.isAppUpdating)
	{
		UE_LOG(LogTemp, Warning, TEXT("###APP热更中"));
	}
	else
	{
		//设置新资源版本号
		m_resNewVersion = TCHAR_TO_UTF8(*newVersionStr);
		UE_LOG(LogTemp, Warning, TEXT("###资源热更中... new version:%s"), UTF8_TO_TCHAR(m_resNewVersion.c_str()));
	}

	if (newVersinInfo.isNeedUpdating && newVersinInfo.needDownloadSize > 0)
	{
		UE_LOG(LogTemp, Warning, TEXT("###获取到新版本描述:%s, 需要的下载大小是:%.4fM"), 
			UTF8_TO_TCHAR(newVersinInfo.versionDescrition), newVersinInfo.needDownloadSize / 1024.0f / 1024.0f);

		if (newVersinInfo.isForcedUpdating)
		{
			//程序更新 + 强制更新
			if (newVersinInfo.isAppUpdating)
			{
#if PLATFORM_IOS
				//TODO goto apple app store 
				return;
#endif
			}
			if (mgr != nullptr)
			{
				if (hotfixCallback)
					hotfixCallback->StartUpdate();
				ContinueUpdate();
			}
		}
		else
		{
			UE_LOG(LogTemp, Warning, TEXT("###可选更新"));
			auto yes = [=]() -> void { 
				if (newVersinInfo.isAppUpdating)
				{
#if PLATFORM_IOS
					//TODO goto apple app store 
					return;
#endif
				}
				if (hotfixCallback)
					hotfixCallback->StartUpdate();
				ContinueUpdate();
			};
			auto no = [=]() -> void { Cancel(); };

			//可选更新，弹窗提示
			if (hotfixCallback)
				hotfixCallback->OptionalUpdate(yes, no);
		}
	}
	else
	{
		UE_LOG(LogTemp, Warning, TEXT("###不需要下载"));
		UE_LOG(LogTemp, Warning, TEXT("### not need download finish call"));
		Finish("");
	}

}
void SDK_Dolphin::OnDolphinNoticeInstallApk(char* apkurl)
{
	UE_LOG(LogTemp, Warning, TEXT("### install apk url: %s"), UTF8_TO_TCHAR(apkurl));
	int code = -10;

#if PLATFORM_ANDROID
	jclass cls = AndroidJavaEnv::FindJavaClass("com/tencent/gcloud/dolphin/CuIIPSMobile");
	jobject obj = AndroidJavaEnv::GetGameActivityThis();
	JNIEnv* env = AndroidJavaEnv::GetJavaEnv();
	jmethodID method = env->GetStaticMethodID(cls, "installAPK", "(Ljava/lang/String;Ljava/lang/Object;)I");
	code = (env->CallStaticIntMethod(cls, method, env->NewStringUTF(apkurl), obj));
#endif
	if (code == -1)
	{
		UE_LOG(LogTemp, Warning, TEXT("###路径不存在或者各种参数为空"));
	}
	else if (code == -2)
	{
		UE_LOG(LogTemp, Warning, TEXT("###路径不支持"));
	}
	else if (code == -3)
	{
		UE_LOG(LogTemp, Warning, TEXT("###吊起安装界面失败"));
	}
	else if (code == -10)
	{
		UE_LOG(LogTemp, Warning, TEXT("###非安卓平台"));
	}
	else if (code == 0)
	{
		UE_LOG(LogTemp, Warning, TEXT("###安装成功"));
	}
	else
	{
		UE_LOG(LogTemp, Warning, TEXT("###未知错误"));
	}
	//通过系统安装界面安装，这里需要直接完成热更流程，不管是否安装成功
	Cancel();

}

void SDK_Dolphin::OnDolphinProgress(dolphinUpdateStage curVersionStage, cu_uint64 totalSize, cu_uint64 nowSize)
{
	TM_HotfixState state = TM_HotfixState::TM_Fail;
	if (curVersionStage >= dolphinUpdateStage::VS_Start && curVersionStage <= dolphinUpdateStage::VS_Filelist_Check)
		state = TM_HotfixState::TM_LocalFileCheck;
	else if (curVersionStage >= dolphinUpdateStage::VS_Dolphin_Version && curVersionStage <= dolphinUpdateStage::VS_Dolphin_Version)
		state = TM_HotfixState::TM_GetVersionInfo;
	else if (curVersionStage >= dolphinUpdateStage::VS_ApkUpdate && curVersionStage <= dolphinUpdateStage::VS_ApkUpdateCheckDiff)
		state = TM_HotfixState::TM_AppUpdate;
	else if (curVersionStage == dolphinUpdateStage::VS_ApkUpdateMergeDiff)
		state = TM_HotfixState::TM_AppMerge;
	else if (curVersionStage >= dolphinUpdateStage::VS_ApkUpdateCheckFull && curVersionStage <= dolphinUpdateStage::VS_ApkUpdateCheckPredownloadApk)
		state = TM_HotfixState::TM_AppCheck;
	else if (curVersionStage >= dolphinUpdateStage::VS_SourceUpdateCures && curVersionStage <= dolphinUpdateStage::VS_SourceDownload)
		state = TM_HotfixState::TM_ResUpdate;
	else if (curVersionStage >= dolphinUpdateStage::VS_SourceExtract && curVersionStage <= dolphinUpdateStage::VS_SourceExtractPredownloadPatch)
		state = TM_HotfixState::TM_ResExtract;
	else if (curVersionStage == dolphinUpdateStage::VS_Success)
		state = TM_HotfixState::TM_Success;
	else if (curVersionStage == dolphinUpdateStage::VS_Fail)
		state = TM_HotfixState::TM_Fail;

	if (hotfixCallback)
		hotfixCallback->ProgressUpdate(state, nowSize, totalSize);

	ContinueUpdate();
}
void SDK_Dolphin::OnDolphinError(dolphinUpdateStage curVersionStage, cu_uint32 errorCode)
{
	isSuc = false;
	UE_LOG(LogTemp, Warning, TEXT("###Dolphin热更失败,state = %d , code = %u"), curVersionStage, errorCode);

	IIPSMobileErrorCodeCheck* errorCodeCheck = new IIPSMobileErrorCodeCheck();
	int translateErrorCode = errorCodeCheck->CheckIIPSErrorCode(errorCode).m_nErrorCode;
	UE_LOG(LogTemp, Warning, TEXT("###Dolphin热更失败, translate code = %d"), translateErrorCode);

	if(hotfixCallback)
		hotfixCallback->Error();

	ContinueUpdate();
	UE_LOG(LogTemp, Warning, TEXT("### update error finish call"));
	Finish("");
}
void SDK_Dolphin::OnDolphinSuccess()
{
	isSuc = true;
	UE_LOG(LogTemp, Warning, TEXT("###Dolphin热更成功"));
	UE_LOG(LogTemp, Warning, TEXT("### update success finish call"));
	Finish(m_resNewVersion.c_str());
}

void SDK_Dolphin::OnDolphinServerCfgInfo(const char* config)
{
	UE_LOG(LogTemp, Warning, TEXT("###Dolphin 获取服务器渠道配置信息:%s"), UTF8_TO_TCHAR(config));
}

void SDK_Dolphin::OnDolphinIOSBGDownloadDone()
{
#if PLATFORM_IOS
	UE_LOG(LogTemp, Warning, TEXT("###Dolphin IOS更新后台下载完成"));
#endif
}

void SDK_Dolphin::OnActionMsgArrive(const char* msg)
{
	UE_LOG(LogTemp, Warning, TEXT("###Dolphin 获取操作信息:%s"), UTF8_TO_TCHAR(msg));
}

bool SDK_Dolphin::Init_Internal()
{
	if (nullptr == mgr)
	{
		mgr = CreateDolphin();
	}
	//不用首包解压的话第四个参数必须为nullptr
	if (initInfo != nullptr)
	{
		UE_LOG(LogTemp, Log, TEXT("### init info... appversion: %s, resversion: %s, channelId: %d"), 
			UTF8_TO_TCHAR(initInfo->appVersion), UTF8_TO_TCHAR(initInfo->srcVersion),
			initInfo->channelId);
	}
	else
	{
		UE_LOG(LogTemp, Log, TEXT("### init info... null"));
	}

//#if PLATFORM_IOS
//	mgr->SetIOSBGDownloadAttribute(true, true, 0, 0);
//#endif
//	mgr->DynamicAdjustDolphinSystemParameter(SystemParameterKey::HttpCurlSigleDownloadTimeout, new char('0'));
//
//	INetworkChecker::GetInstance().Ping(updateUrl, 1, [](PingResult& res)
//		{
//			UE_LOG(LogTemp, Log, TEXT("### ping updateurl, res code: %d"), res.ResultCode);
//		});

	return mgr->Init(initInfo, pathInfo, grayInfo, nullptr, callBack);
}

//设置初始化值
void SDK_Dolphin::SetInfo_Internal(dolphinUpdateInitType initType)
{
#if PLATFORM_ANDROID
	UE_LOG(LogTemp, Warning, TEXT("### apk GFilePathBase %s"), *GFilePathBase);
	UE_LOG(LogTemp, Warning, TEXT("### apk GInternalFilePath %s"), *GInternalFilePath);

	FString baseDir = AndroidRelativeToAbsolutePath_TM(FPaths::ProjectSavedDir());
#else
	FString baseDir = FPaths::ProjectSavedDir();
#endif

	//更新信息存储路径
	FString infoSavePath_rel(TEXT("dolphin_info"));
	FString infoSavePath = baseDir / infoSavePath_rel;
	//更新内容储存路径
	FString contextSavePath_rel(TEXT("dolphin_ctx"));
	FString contextSavePath = baseDir / infoSavePath_rel;
	//首包资源解压路径
	FString firstExtrctResPath_rel(TEXT("dolphin_fe"));
	FString firstExtrctResPath = baseDir / firstExtrctResPath_rel;
	//确保目录存在
	FPlatformFileManager::Get().GetPlatformFile().CreateDirectoryTree(*infoSavePath);
	FPlatformFileManager::Get().GetPlatformFile().CreateDirectoryTree(*contextSavePath);
	FPlatformFileManager::Get().GetPlatformFile().CreateDirectoryTree(*firstExtrctResPath);

	//资源更新 / 散资源修复
	if (initType == dolphinUpdateInitType::UpdateInitType_OnlySource ||
		initType == dolphinUpdateInitType::UpdateInitType_SourceCheckAndSync_Optimize_Full_Scatter)
	{
		//首包解压（含散资源，也即不需要解压） 需要使用到这两个配置文件
		//同时拷贝.ifs.res和filelist.json到更新目录
		if (FFileManagerGeneric::Get().FileExists(TEXT("dolphin/filelist.json")) &&
			FFileManagerGeneric::Get().FileExists(TEXT("dolphin/first_source.ifs.res")))
		{
			FFileManagerGeneric::Get().Copy(*(infoSavePath / FString(TEXT("filelist.json"))), TEXT("dolphin/filelist.json"));
			FFileManagerGeneric::Get().Copy(*(infoSavePath / FString(TEXT("first_source.ifs.res"))), TEXT("dolphin/first_source.ifs.res"));
		}

		//散资源修复用
		if (FFileManagerGeneric::Get().FileExists(TEXT("dolphin/filelistcheck.res")))
		{
			FFileManagerGeneric::Get().Copy(*(infoSavePath / FString(TEXT("filelistcheck.res"))), TEXT("dolphin/filelistcheck.res"));
		}
	}

	initInfo = new dolphinInitInfo();
	gc_snprintf(initInfo->appVersion, strlen(appVersion) + 1, "%s", appVersion);//
	gc_snprintf(initInfo->srcVersion, strlen(resVersion) + 1, "%s", resVersion);//
	gc_snprintf(initInfo->updateUrl, strlen(updateUrl) + 1, "%s", updateUrl);//
	gc_snprintf(initInfo->userData, strlen(userData) + 1, "%s", userData);


	initInfo->channelId = channelId;				//渠道id,与更新控制台上的渠道分类id对应
	initInfo->isGrayUpdate = false;					// 默认给他允许灰度更新，控制台可以进行灰度控制
	initInfo->isCreateDiffFlist = true;				//
	initInfo->updateType = initType;				//更新类型

	initInfo->openDebugLog = isDebug;
	initInfo->openErrorLog = isDebug;

	if (initInfo->updateType >= dolphinUpdateInitType::UpdateInitType_OnlySource
		&& initInfo->updateType < dolphinUpdateInitType::UpdateInitType_Normal)
	{
		initInfo->mergeFilelistType = MERGEFILELISTTYPE::kMergeAppResIntoSDCARDRes;   //跨程序版本的首包修正（修正sdcard目录下的资源，以sdcard下的资源为主）
	}
	else
	{
		initInfo->mergeFilelistType = MERGEFILELISTTYPE::kNormalNotMergeRes;
	}

#if PLATFORM_IOS
	initInfo->enableIOSBgDownload = true;
#endif

	pathInfo = new dolphinPathInfo();
	UE_LOG(LogTemp, Warning, TEXT("### apk 下载路径 %s"), *(FPaths::ProjectSavedDir()));
#if PLATFORM_ANDROID
	gc_snprintf(pathInfo->curApkPath, strlen(TCHAR_TO_ANSI(*GAPKFilename)) + 1, "%s", TCHAR_TO_ANSI(*GAPKFilename)); //当前apk路径,当前正在运行apk的绝对路径
#endif
	
	gc_snprintf(pathInfo->updatePath, strlen(TCHAR_TO_ANSI(*contextSavePath)) + 1, "%s", TCHAR_TO_ANSI(*contextSavePath)); //更新路径，资源存放路径，传入时存在并可写，资源从此目录读取
	gc_snprintf(pathInfo->dolphinPath, strlen(TCHAR_TO_ANSI(*infoSavePath)) + 1, "%s", TCHAR_TO_ANSI(*infoSavePath));		//更新信息存储路径，传入时存在并可写，不要删除修改此目录下的内容
	
	gc_snprintf(pathInfo->ifsPath, strlen(TCHAR_TO_ANSI(*firstExtrctResPath)) + 1, "%s", TCHAR_TO_ANSI(*firstExtrctResPath));//首包解压文件路径 资源修复优化方案这个必须设置，否则会失败
	//strcpy(pathInfo->ifsPath, R"(E:\tool\GCloudDolphinTools\GCloudDolphinTools\dolphin_tools\windows\aa.ifs)");

	grayInfo = new dolphinGrayInfo();																	//init初始化参数三grayInfo，可以为NULL,在initInfo.isGrayUpdate为true不能是NULL
	gc_snprintf(grayInfo->worldID, strlen(worldId) + 1, "%s", worldId);	//用户区服id, 用户登录到游戏区服对应的id
	gc_snprintf(grayInfo->userID, strlen(userId) + 1, "%s", userId);		//用户id，例如游戏微信登录的openid或QQ号等

	firstExtractInfo = new dolphinFirstExtractInfo(); //init初始化参数四feInfo，为NULL时不做首包解压，不为NULL时先解压再更新
	gc_snprintf(firstExtractInfo->ifsPath, strlen(TCHAR_TO_ANSI(*firstExtrctResPath)) + 1, "%s", TCHAR_TO_ANSI(*firstExtrctResPath));//首包解压文件路径 
	//strcpy(firstExtractInfo->ifsPath, R"(E:\tool\GCloudDolphinTools\GCloudDolphinTools\dolphin_tools\windows\aa.ifs)");

	callBack = this;
}

void SDK_Dolphin::_releaseDolphinMgr()
{
	if (mgr != nullptr)
	{
		mgr->Uninit();
		ReleaseDolphin(&mgr);
	}
	delete mgr;
	mgr = nullptr;
}

//[[unused]]
void SDK_Dolphin::_copyUpdateFilesToSaved(const char* savedRoot)
{
	TArray<FString> filePaths = {
		TEXT("apollo_reslist.flist"),					//资源目录文件 在资源修复功能 检查修复标记			
		TEXT("apollo_reslist.flist.diff"),			    //当初始化设置了生成差异文件列表时生成的文件
		TEXT("filelist.json"),							//服务器json文件 记录版本信息
		TEXT("filelistcheck.res"),						//在散资源修复功能 记录apk内散资源目录信息
		TEXT("first_source.ifs.res")					//在首包解压优化方案 资源更新的目录文件
	};

	for (FString fPath : filePaths)
	{
		FString dstPath = FString(UTF8_TO_TCHAR(savedRoot)) / fPath;
		FString srcPath = FString(TEXT("dolphin")) / fPath;
		if (FFileManagerGeneric::Get().FileExists(*srcPath))
		{
			FFileManagerGeneric::Get().Copy(*dstPath, *srcPath);
			UE_LOG(LogTemp, Log, TEXT("### copy update file: %s, to saved path: %s"), *srcPath, *dstPath);
		}
		else
		{
			UE_LOG(LogTemp, Warning, TEXT("### copy failed, update file: %s, to saved path: %s"), *srcPath, *dstPath);
		}
	}
}

SDKDolphinBuilder* SDK_Dolphin::builder = nullptr;

SDKDolphinBuilder& SDK_Dolphin::Create()
{
	if (nullptr == builder)
	{
		builder = new SDKDolphinBuilder();
	}
	return *builder;
}

void SDK_Dolphin::Destroy()
{
	if (builder != nullptr)
	{
		delete builder;
		builder = nullptr;
	}
}

SDKDolphinBuilder& SDKDolphinBuilder::SetVersionInfo(const char* appVer, const char* resVer)
{
	dolphin->appVersion = appVer;
	dolphin->resVersion = resVer;
	return *this;
}

SDKDolphinBuilder& SDKDolphinBuilder::SetChannelId(uint32 channelId, const char* updateUrl)
{
	dolphin->channelId = channelId;
	dolphin->updateUrl = updateUrl;
	return *this;
}

SDKDolphinBuilder& SDKDolphinBuilder::SetGrayInfo(const char* worldId, const char* userId)
{
	dolphin->worldId = worldId;
	dolphin->userId = userId;
	return *this;
}

SDKDolphinBuilder& SDKDolphinBuilder::SetExtraUserInfo(const char* userData)
{
	dolphin->userData = userData;
	return *this;
}
