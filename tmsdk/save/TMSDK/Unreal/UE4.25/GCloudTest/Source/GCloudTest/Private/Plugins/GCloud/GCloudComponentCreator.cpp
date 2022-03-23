// Fill out your copyright notice in the Description page of Project Settings.


#include "Plugins/GCloud/GCloudComponentCreator.h"

#include "Plugins/GameHotfix.h"

#include "Plugins/GCloud/SDK_Dolphin.h"
#include "Plugins/GCloud/SDK_Maple.h"

#include "UtilityMarcoDefine.h"
#include "JsonLibUtil.h"

/*---------------------------------------------       Dolphin Start    ---------------------------------------------*/

void GCloudDolphin::Init(const SDKInfo& sdkInfo, bool isDebug)
{
	_createGCloudDolphin(sdkInfo);
	if (m_dolphin != nullptr)
	{
		m_dolphin->SetDebugMode(isDebug);
	}

	//先初始化程序更新检查
	_initAppUpdate();

	m_updateFinishHandle = PluginManager::Instance().EventManager().RegisterEvent(SDKEventType::UpdateFinish, SDK_EVENT_FUNC(_onUpdateFinished));
}

void GCloudDolphin::Uninit()
{
	UE_LOG(LogTemp, Warning, TEXT("### dolphin uninit ..."));

	PluginManager::Instance().EventManager().RemoveEvent(m_updateFinishHandle);

	if (m_dolphin != nullptr)
	{
		m_dolphin->Cancel();
		m_dolphin = nullptr;
	}
	SDK_Dolphin::Destroy();

	m_bAppUpdateInited = false;
	m_bResUpdateInited = false;
	m_bUpdateStarted = false;
	m_bUpdateFinished = false;
}

void GCloudDolphin::Tick()
{
	//dolphin
	if (m_bAppUpdateInited || m_bResUpdateInited)
	{
		if (!m_bUpdateStarted)
		{
			m_bUpdateStarted = true;

			//开始更新
			if (m_dolphin != nullptr)
			{
				m_dolphin->UpdateStart();
			}
			UE_LOG(LogTemp, Warning, TEXT("### update start ..."));
		}

		//更新未完成
		if (m_bUpdateStarted && !m_bUpdateFinished) {
			if (m_dolphin != nullptr)
			{
				m_dolphin->UpdateLoop();
			}
		}
	}
}

void GCloudDolphin::_createGCloudDolphin(const SDKInfo& sdkInfo)
{
	m_dolphin =
		SDK_Dolphin::Create()
		.SetVersionInfo(sdkInfo.AppVersion.c_str(), sdkInfo.ResVersion.c_str())
		.SetChannelId(sdkInfo.ChannelID, sdkInfo.UpdateUrl.c_str())
		.SetExtraUserInfo(sdkInfo.ExtraData.c_str())
		.SetGrayInfo(sdkInfo.ServerID.c_str(), sdkInfo.OpenID.c_str()).GetDolphin();
}

void GCloudDolphin::_updateResVersionInConfig(const FString& newResVer)
{
	if (newResVer.IsEmpty())
	{
		UE_LOG(LogTemp, Error, TEXT("### update res new version is empty!"));
		return;
	}
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
			FString currResVer = fobjPtr->GetStringField(TEXT("resVersion"));
			UE_LOG(LogTemp, Log, TEXT("### read file json: resVersion: %s"), *currResVer);

			fobjPtr->SetStringField(TEXT("resVersion"), newResVer);
			FString jsonContent;
			JsonLibUtil::ToJson<TSharedPtr<FJsonObject>>(fobjPtr, jsonContent);
			succ = WriteFileInPluginsPath(jsonContent, relPathRoot, relFileName);
			if (succ)
			{
				UE_LOG(LogTemp, Log, TEXT("### write file json: %s"), *jsonContent);
			}
			else
			{
				UE_LOG(LogTemp, Log, TEXT("### write file failed, json: %s"), *jsonContent);
			}
		}
	}
	else
	{
		UE_LOG(LogTemp, Error, TEXT("load file:%s failed"), *fs);
	}
}

void GCloudDolphin::_initAppUpdate()
{
	if (m_bAppUpdateInited)
	{
		return;
	}
	if (m_dolphin != nullptr)
	{
		m_bAppUpdateInited = m_dolphin->Init_AppUpdate();
		UE_LOG(LogTemp, Warning, TEXT("### GCloud Dolphin <App Update> Init... %s"),
			m_bAppUpdateInited ? TEXT("success") : TEXT("failed"));
	}

	if (m_bAppUpdateInited)
	{
		m_bUpdateStarted = false;
		m_bUpdateFinished = false;
	}
	//更新初始化失败 默认为更新结束
	if (!m_bAppUpdateInited)
	{
		UE_LOG(LogTemp, Warning, TEXT("### app update not inited, continue init res update"));
		//再初始化资源更新检查
		_initResUpdate();
	}
}

void GCloudDolphin::_initResUpdate()
{
	if (m_bResUpdateInited)
	{
		return;
	}
	if (m_dolphin != nullptr)
	{
		m_bResUpdateInited = m_dolphin->Init_ResUpdate();
		UE_LOG(LogTemp, Warning, TEXT("### GCloud Dolphin <Res Update> Init... %s"),
			m_bResUpdateInited ? TEXT("success") : TEXT("failed"));
	}

	if (m_bResUpdateInited)
	{
		m_bUpdateStarted = false;
		m_bUpdateFinished = false;
	}
	//更新初始化失败 默认为更新结束
	if (!m_bResUpdateInited)
	{
		UE_LOG(LogTemp, Warning, TEXT("### res update not inited, send update error..."));
		//PluginManager::Instance().EventManager().TriggerEvent(SDKEventType::AllUpdateEnd);
		_initResCheck();
	}
}

void GCloudDolphin::_initResCheck()
{
	if (m_bResCheckInited)
	{
		return;
	}
	if (m_dolphin != nullptr)
	{
		m_bResCheckInited = m_dolphin->InitNoIFSResCheck();
		UE_LOG(LogTemp, Warning, TEXT("### GCloud Dolphin <Res Check> Init... %s"),
			m_bResCheckInited ? TEXT("success") : TEXT("failed"));
	}

	if (m_bResCheckInited)
	{
		m_bUpdateStarted = false;
		m_bUpdateFinished = false;
	}
	//更新初始化失败 默认为更新结束
	if (!m_bResCheckInited)
	{
		UE_LOG(LogTemp, Warning, TEXT("### res check not inited, send update error..."));
		PluginManager::Instance().EventManager().TriggerEvent(SDKEventType::AllUpdateEnd);
	}
}

void GCloudDolphin::_onUpdateFinished(const SDKEventParam& param)
{
	if (!m_bUpdateFinished)
	{
		m_bUpdateFinished = true;

		//倒推初始化顺序
		if (m_bResCheckInited)
		{
			PluginManager::Instance().EventManager().TriggerEvent(SDKEventType::AllUpdateEnd);
		}
		else if (m_bResUpdateInited)
		{
			UpdateEventParam updateEventParam = static_cast<const UpdateEventParam&>(param);
			_updateResVersionInConfig(updateEventParam.fstr_0);

			_initResCheck();
		}
		else if (m_bAppUpdateInited)
		{
			_initResUpdate();
		}
	}
}

/*---------------------------------------------       Dolphin End    ---------------------------------------------*/

/*---------------------------------------------       Maple Start    ---------------------------------------------*/

void GCloudMaple::Init(const SDKInfo& sdkInfo, bool isDebug)
{
	_createGCloudMaple();

	if (m_maple != nullptr)
	{
		m_bSDKMapleInited = m_maple->Initialize(sdkInfo.OpenID.c_str());
	}
	m_dirPullFinishHandle = PluginManager::Instance().EventManager().RegisterEvent(SDKEventType::DirPullFinish, SDK_EVENT_FUNC(_onDirPullFinished));
}

void GCloudMaple::Uninit()
{
	UE_LOG(LogTemp, Warning, TEXT("### maple uninit ..."));

	PluginManager::Instance().EventManager().RemoveEvent(m_dirPullFinishHandle);

	SAFE_DELETE_PTR(m_maple);
	m_bSDKMapleInited = false;
	m_bDirPullStarted = false;
	m_bDirPullFinished = false;
}

void GCloudMaple::Tick()
{
	//maple
	if (m_bSDKMapleInited)
	{
		if (!m_bDirPullStarted)
		{
			m_bDirPullStarted = true;
			m_maple->QueryDirServiceTree();
			UE_LOG(LogTemp, Warning, TEXT("### dir pull start ..."));
		}
		//目录拉取未完成
		if (m_bDirPullStarted && !m_bDirPullFinished)
		{
			if (m_maple != nullptr) {
				m_maple->UpdateDirService();
			}
		}
	}
}

void GCloudMaple::_createGCloudMaple()
{
	if (nullptr == m_maple)
	{
		m_maple = new SDK_Maple();
	}
}

void GCloudMaple::_onDirPullFinished(const SDKEventParam& param)
{
	if (!m_bDirPullFinished)
	{
		m_bDirPullFinished = true;
	}
}

/*---------------------------------------------       Maple End    ---------------------------------------------*/

GCloudComponentCreator::~GCloudComponentCreator()
{
	int total = SDKComponentType::TOTAL;
	total = (total - 1) * 2 - 1;
	Destroy(total);
	sdkComponentMap.clear();
}

SDKComponent* GCloudComponentCreator::Create(SDKComponentType type)
{
	SDKComponent* com = nullptr;
	if (sdkComponentMap.find(type) != sdkComponentMap.end())
	{
		com = sdkComponentMap[type];
	}
	else
	{
		switch (type)
		{
		case SDKComponentType::GAME_UPDATE:
			com = new GCloudDolphin(type);
			break;
		case SDKComponentType::GAME_DIR_SERVER:
			com = new GCloudMaple(type);
			break;
		}
		if (com != nullptr)
		{
			sdkComponentMap.emplace(type, com);
		}
	}
	return com;
}

void GCloudComponentCreator::Destroy(SDKComponent* com)
{
	if (com != nullptr)
	{
		if (sdkComponentMap.find(com->Type()) != sdkComponentMap.end())
		{
			sdkComponentMap.erase(com->Type());
		}
		com->Uninit();
		SAFE_DELETE_PTR(com);
	}
}

void GCloudComponentCreator::Destroy(SDKComponentType type)
{
	if (sdkComponentMap.find(type) != sdkComponentMap.end())
	{
		Destroy(sdkComponentMap[type]);
	}
}

void GCloudComponentCreator::Destroy(int sdkComponentType)
{
	auto itr = sdkComponentMap.begin();
	for (; itr != sdkComponentMap.end();)
	{
		SDKComponentMapPair com = *itr;
		if (com.first & sdkComponentType)
		{
			itr = sdkComponentMap.erase(itr);

			if (com.second != nullptr)
			{
				com.second->Uninit();
				SAFE_DELETE_PTR(com.second);
			}
		}
		else
		{
			++itr;
		}
	}
}
