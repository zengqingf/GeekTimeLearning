// Fill out your copyright notice in the Description page of Project Settings.


#include "Plugins/GVoice/GVoiceManager.h"
#include "time.h"
#include "HAL/PlatformFilemanager.h"
#include "UtilityMarcoDefine.h"


GVoiceManager::GVoiceManager()
{
	mEventHandleList.push_back(PluginManager::Instance().EventManager().RegisterEvent(SDKEventType::GVoiceUploadSucc, SDK_EVENT_FUNC(HandleUploadVoiceSucc)));
	mEventHandleList.push_back(PluginManager::Instance().EventManager().RegisterEvent(SDKEventType::GVoiceDownloadSucc, SDK_EVENT_FUNC(HandleDownloadVoiceSucc)));
	mEventHandleList.push_back(PluginManager::Instance().EventManager().RegisterEvent(SDKEventType::GVoiceJoinRoomSucc, SDK_EVENT_FUNC(HandleJoinRoomSucc)));
	mEventHandleList.push_back(PluginManager::Instance().EventManager().RegisterEvent(SDKEventType::GVoiceQuitRoomSucc, SDK_EVENT_FUNC(HandleQuitRoomSucc)));
	mEventHandleList.push_back(PluginManager::Instance().EventManager().RegisterEvent(SDKEventType::GVoiceTranslateSucc, SDK_EVENT_FUNC(HandleTranslateSucc)));

	Init();
}

GVoiceManager::~GVoiceManager()
{
	for (auto iter = mEventHandleList.begin(); iter != mEventHandleList.end(); ++iter)
	{
		PluginManager::Instance().EventManager().RemoveEvent(*iter);
	}
	mEventHandleList.clear();

	Uninit();
	UE_LOG(LogTemp, Log, TEXT("### gvoice mgr dtor"));
}

void GVoiceManager::Init()
{
	mGVoiceInterface = new GVoiceInterface();
	InitAppIDAndKey();
	InitMessageVoicePathRoot();
}

void GVoiceManager::Uninit()
{
	ClaerMessageVoicePath();
	SAFE_DELETE_PTR(mGVoiceInterface);
	mCacheVoiceMap.clear();
	mRoomList.clear();
}

bool GVoiceManager::Tick(float DeltaTime)
{
	return false;
}

void GVoiceManager::InitGVoice(string openId)
{
	if (mGVoiceAppID.IsEmpty() || mGVoiceAppKey.IsEmpty())
	{
		return;
	}
	mGVoiceInterface->InitApp(TCHAR_TO_UTF8(*mGVoiceAppID), TCHAR_TO_UTF8(*mGVoiceAppKey), openId);
}

void GVoiceManager::SetVoicePoll()
{
	mGVoiceInterface->Poll();
}

void GVoiceManager::Pause()
{
	mGVoiceInterface->Pause();
}

void GVoiceManager::Resume()
{
	mGVoiceInterface->Resume();
}

//void GVoiceManager::InitMessageVoice(string openId)
//{
//
//	mGVoiceInterface->GRealTimeMessageInit(TCHAR_TO_UTF8(*mGVoiceAppID), TCHAR_TO_UTF8(*mGVoiceAppKey),openId);
//}

void GVoiceManager::StartRecord(bool onlyTranslate)
{
	GetDealMessageVoicePath();
	if (mSaveVoicePath.empty())
	{
		UE_LOG(LogTemp, Warning, TEXT("getMessageVoicePath is null"));
		return;
	}
	UE_LOG(LogTemp, Warning, TEXT("getMessageVoicePath is %s"), *FString(mSaveVoicePath.c_str()));
	if (onlyTranslate)
	{
		mGVoiceInterface->StartRecordOnlyForTrans(mSaveVoicePath);
	}
	else
	{
		mGVoiceInterface->StartRecord(mSaveVoicePath);
	}
}

void GVoiceManager::StopRecord(bool onlyTranslate)
{
	mGVoiceInterface->StopRecord();
	if (!onlyTranslate)
	{
		mGVoiceInterface->Upload(mSaveVoicePath);
	}
}
void GVoiceManager::CancleRecord()
{
	mGVoiceInterface->StopRecord();
}

void GVoiceManager::StartPlayVoice(string voiceFileId)
{
	if (voiceFileId.empty())
	{
		UE_LOG(LogTemp, Warning, TEXT("voiceFileId is null"));
		return;
	}
	if (mCacheVoiceMap.find(voiceFileId) != mCacheVoiceMap.end())
	{
		mGVoiceInterface->Play(mCacheVoiceMap[voiceFileId]);
	}
	else
	{
		mGVoiceInterface->Download(voiceFileId, GetDealMessageVoicePath());
	}
}

void GVoiceManager::StopPlayVoice()
{
	mGVoiceInterface->StopPlay();
}


//realtime voice func

//void GVoiceManager::InitRealTimeVoice(string openID)
//{
//	if (mGVoiceAppID.IsEmpty() || mGVoiceAppKey.IsEmpty())
//	{
//		return;
//	}
//	mGVoiceInterface->GRealTimeVoiceInit(TCHAR_TO_UTF8(*mGVoiceAppID), TCHAR_TO_UTF8(*mGVoiceAppKey), openID);
//	mGVoiceInterface->EnableMultiRoom(true);
//}

void GVoiceManager::EnableMultiRoom(bool enable)
{
	mGVoiceInterface->EnableMultiRoom(enable);
}

void GVoiceManager::JoinRoom(string roomName)
{
	mGVoiceInterface->JoinRoom(roomName.c_str());
}

void GVoiceManager::QuitRoom(string roomName)
{
	mGVoiceInterface->QuitRoom(roomName.c_str());
}

void GVoiceManager::QuitAllRoom()
{
	if (!mRoomList.empty())
	{
		for (list<string>::iterator iter = mRoomList.begin();iter!=mRoomList.end();++iter)
		{
			string room = *iter;
			QuitRoom(room.c_str());
		}
	}
}

void GVoiceManager::ControlMic(bool isOpen)
{
	mGVoiceInterface->ControlMic(isOpen);
}

void GVoiceManager::ControlSpeaker(bool isOpen)
{
	mGVoiceInterface->ControlSpeaker(isOpen);
}

void GVoiceManager::EnableRoomMicrophone(string roomName, bool enable)
{
	mGVoiceInterface->EnableRoomMicrophone(roomName.c_str(), enable);
}

void GVoiceManager::EnableRoomSpeaker(string roomName, bool enable)
{
	mGVoiceInterface->EnableRoomSpeaker(roomName.c_str(), enable);
}

void GVoiceManager::ForbidMemberVoice(int member, bool enable, string roomName)
{
	mGVoiceInterface->ForbidMemberVoice(member, enable, roomName.c_str());
}

bool GVoiceManager::IsSpeaking()
{
	return mGVoiceInterface->IsSpeaking();
}

void GVoiceManager::InitAppIDAndKey()
{
	GConfig->GetString(TEXT("/Script/GVoice.GVoiceSettings"),TEXT("GVoiceAppID"),mGVoiceAppID,GGameIni);
	GConfig->GetString(TEXT("/Script/GVoice.GVoiceSettings"),TEXT("GVoiceAppKey"),mGVoiceAppKey,GGameIni);
}

//private func
void GVoiceManager::InitMessageVoicePathRoot()
{
#ifdef __ANDROID__
	mSaveVoicePathRoot = TCHAR_TO_UTF8(*(FPaths::ProjectPersistentDownloadDir()));
#else
#ifdef __APPLE__

	FString _FilePath = FPaths::ProjectSavedDir() / TEXT("Gvoice");
	FString absPath = FPlatformFileManager::Get().GetPlatformFile().GetPlatformPhysical().ConvertToAbsolutePathForExternalAppForRead(*_FilePath);
	FString _FullPath = absPath;

	char* filepath = (char*)TCHAR_TO_ANSI(*_FullPath);

	mSaveVoicePathRoot.assign(filepath);


#else
	FString _FilePath = FPaths::ProjectSavedDir() / TEXT("Gvoice");
	FString _fullPath = FPaths::ConvertRelativePathToFull(_FilePath);
	char* filepath = (char*)TCHAR_TO_ANSI(*_fullPath);
	mSaveVoicePathRoot.assign(filepath);
	FString path = mSaveVoicePathRoot.c_str();
	IPlatformFile& PlatformFile = FPlatformFileManager::Get().GetPlatformFile();
	if (!PlatformFile.DirectoryExists(*path))
	{
		PlatformFile.CreateDirectory(*path);
		UE_LOG(LogTemp, Warning, TEXT(" CreateVoiceDirectory !"));
	}
#endif
#endif
}

string GVoiceManager::GetDealMessageVoicePath()
{
	string finalvoicepath;
	FDateTime  timeNow = FDateTime::Now();
	int64 timeNowstamp = timeNow.ToUnixTimestamp();
	string voicename = to_string(timeNowstamp) + "_v.dat";
	mSaveVoicePath = mSaveVoicePathRoot + "/" + voicename;
	return mSaveVoicePath;
}

void GVoiceManager::ClaerMessageVoicePath()
{
	IPlatformFile& PlatformFile = FPlatformFileManager::Get().GetPlatformFile();
	FString path = mSaveVoicePathRoot.c_str() + FString("/")+ FString("*_v.dat");
	TArray<FString> FindedFiles;
	IFileManager::Get().FindFiles(FindedFiles, *path, true, false);
	FString SearchFile = "";
	for (int i = 0; i < FindedFiles.Num(); i++)
	{
		SearchFile = mSaveVoicePathRoot.c_str() + FString("/") + FindedFiles[i];
		if (PlatformFile.FileExists(*SearchFile))
		{
			PlatformFile.DeleteFile(*SearchFile);
		}
	}
	UE_LOG(LogTemp, Warning, TEXT(" ClaerMessageVoicePath !"));
}

void GVoiceManager::HandleUploadVoiceSucc(const SDKEventParam& param)
{
	if (param.str_0.empty() || param.str_1.empty())
	{
		UE_LOG(LogTemp, Warning, TEXT(" HandleUpload param is null !!!!"));
		return;
	}
	UE_LOG(LogTemp, Warning, TEXT(" HandleUploadSucc !!!!"));
	mCacheVoiceMap.insert(std::pair<std::string, std::string>(param.str_0, param.str_1));
	mGVoiceInterface->TranslateMessage(param.str_0);
}

void GVoiceManager::HandleDownloadVoiceSucc(const SDKEventParam& param)
{
	if (param.str_0.empty() || param.str_1.empty())
	{
		UE_LOG(LogTemp, Warning, TEXT(" HandleDownloadVoice param is null !!!!"));
		return;
	}
	UE_LOG(LogTemp, Warning, TEXT(" HandleDownloadVoiceSucc !!!!"));
	mCacheVoiceMap.insert(std::pair<std::string, std::string>(param.str_0, param.str_1));
	StartPlayVoice(param.str_0);
}

void GVoiceManager::HandleJoinRoomSucc(const SDKEventParam& param)
{
	if (param.str_0.empty())
	{
		return;
	}
	for (list<string>::iterator iter = mRoomList.begin(); iter != mRoomList.end(); ++iter)
	{
		if (*iter == param.str_0)
		{
			return;
		}
	}
	mRoomList.push_back(param.str_0);
	EnableRoomMicrophone(param.str_0, true);
	EnableRoomSpeaker(param.str_0, true);
}

void GVoiceManager::HandleQuitRoomSucc(const SDKEventParam& param)
{
	if (param.str_0.empty())
	{
		return;
	}
	if (!mRoomList.empty())
	{
		mRoomList.remove(param.str_0);
	}
}

void GVoiceManager::HandleTranslateSucc(const SDKEventParam& param)
{
	if (param.str_0.empty())
	{
		UE_LOG(LogTemp, Warning, TEXT(" HandleTranslate file id is null !!!!"));
		return;
	}
	if (mCacheVoiceMap.find(param.str_0) != mCacheVoiceMap.end())
	{
		unsigned int bytes = 1024;
		float seconds = 0;
		mGVoiceInterface->GetVoiceByteAndTime(mCacheVoiceMap[param.str_0], &bytes, &seconds);
		SDKEventParam params;
		params.str_0 = param.str_0;
		params.str_1 = param.str_1;
		params.int_0 = bytes;
		params.float_0 = seconds;
		PluginManager::Instance().EventManager().TriggerEvent(SDKEventType::GVoiceDealInfoSucc, params);
	}
}
