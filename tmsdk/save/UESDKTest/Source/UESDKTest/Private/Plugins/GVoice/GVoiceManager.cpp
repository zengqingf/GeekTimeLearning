// Fill out your copyright notice in the Description page of Project Settings.


#include "Plugins/GVoice/GVoiceManager.h"

#include "time.h"


namespace TenmoveSDK

{

	GVoiceManager::GVoiceManager()
	{
		_InitAppIDAndKey();
		this->gVoiceInterface = new GVoiceInterface();
		PluginsEventManager::Instance()->RegisterEvent(SDKEventType::GVoiceUploadSucc, EVENT_FUNC(_handleUploadVoiceSucc));
		PluginsEventManager::Instance()->RegisterEvent(SDKEventType::GVoiceDownloadSucc, EVENT_FUNC(_handleDownloadVoiceSucc));
		PluginsEventManager::Instance()->RegisterEvent(SDKEventType::GVoiceJoinRoomSucc, EVENT_FUNC(_handleJoinRoomSucc));
		PluginsEventManager::Instance()->RegisterEvent(SDKEventType::GVoiceQuitRoomSucc, EVENT_FUNC(_handleQuitRoomSucc));
		PluginsEventManager::Instance()->RegisterEvent(SDKEventType::GVoiceTranslateSucc, EVENT_FUNC(_handleTranslateSucc));
	}

	GVoiceManager::~GVoiceManager()
	{
	}

	void GVoiceManager::SetVoicePoll()
	{
		gVoiceInterface->Poll();
	}

	void GVoiceManager::InitMessageVoice(string openId)
	{
		if (gVoiceAppID.IsEmpty() || gVoiceAppKey.IsEmpty())
		{
			return;
		}
		gVoiceInterface->GRealTimeMessageInit(TCHAR_TO_UTF8(*gVoiceAppID), TCHAR_TO_UTF8(*gVoiceAppKey),openId);
		_InitMessageVoicePathRoot();
	}

	void GVoiceManager::StartRecord()
	{
		dealMessageVoicePath();
		if (mSaveVoicePath.empty())
		{
			UE_LOG(LogTemp, Warning, TEXT("getMessageVoicePath is null"));
			return;
		}
		UE_LOG(LogTemp, Warning, TEXT("getMessageVoicePath is %s"), *FString(mSaveVoicePath.c_str()));
		gVoiceInterface->StartRecord(mSaveVoicePath);
	}

	void GVoiceManager::StopRecord()
	{
		gVoiceInterface->StopRecord();
		gVoiceInterface->Upload(mSaveVoicePath);
	}
	void GVoiceManager::CancleRecord()
	{
		gVoiceInterface->StopRecord();
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
			gVoiceInterface->Play(mCacheVoiceMap[voiceFileId]);
		}
		else
		{
			gVoiceInterface->Download(voiceFileId, dealMessageVoicePath());
		}
	}

	void GVoiceManager::StopPlayVoice()
	{
		gVoiceInterface->StopPlay();
	}


	//realtime voice func

	void GVoiceManager::InitRealTimeVoice(string openID)
	{
		if (gVoiceAppID.IsEmpty() || gVoiceAppKey.IsEmpty())
		{
			return;
		}
		gVoiceInterface->GRealTimeVoiceInit(TCHAR_TO_UTF8(*gVoiceAppID), TCHAR_TO_UTF8(*gVoiceAppKey), openID);
		gVoiceInterface->EnableMultiRoom(true);
	}

	void GVoiceManager::EnableMultiRoom(bool enable)
	{
		gVoiceInterface->EnableMultiRoom(enable);
	}

	void GVoiceManager::JoinRoom(string roomName)
	{
		gVoiceInterface->JoinRoom(roomName.c_str());
	}

	void GVoiceManager::QuitRoom(string roomName)
	{
		gVoiceInterface->QuitRoom(roomName.c_str());
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

	void GVoiceManager::OpenMic()
	{
		gVoiceInterface->OpenMic();
	}

	void GVoiceManager::CloseMic()
	{
		gVoiceInterface->CloseMic();
	}

	void GVoiceManager::OpenSpeaker()
	{
		gVoiceInterface->OpenSpeaker();
	}

	void GVoiceManager::CloseSpeaker()
	{
		gVoiceInterface->CloseSpeaker();
	}

	void GVoiceManager::EnableRoomMicrophone(string roomName, bool enable)
	{
		gVoiceInterface->EnableRoomMicrophone(roomName.c_str(), enable);
	}

	void GVoiceManager::EnableRoomSpeaker(string roomName, bool enable)
	{
		gVoiceInterface->EnableRoomSpeaker(roomName.c_str(), enable);
	}

	void GVoiceManager::ForbidMemberVoice(int member, bool enable, string roomName)
	{
		gVoiceInterface->ForbidMemberVoice(member, enable, roomName.c_str());
	}

	bool GVoiceManager::IsSpeaking()
	{
		return gVoiceInterface->IsSpeaking();
	}




	void GVoiceManager::_InitAppIDAndKey()
	{
		GConfig->GetString(TEXT("/Script/GVoice.GVoiceSettings"),TEXT("GVoiceAppID"),gVoiceAppID,GGameIni);
		GConfig->GetString(TEXT("/Script/GVoice.GVoiceSettings"),TEXT("GVoiceAppKey"),gVoiceAppKey,GGameIni);
	}

	//private func
	void GVoiceManager::_InitMessageVoicePathRoot()
	{
#ifdef __ANDROID__
		mSaveVoicePathRoot = TCHAR_TO_UTF8(*(FPaths::ProjectPersistentDownloadDir()));
#else
#ifdef __APPLE__

		FString _FilePath = FPaths::ProjectSavedDir();
		FString absPath = FPlatformFileManager::Get().GetPlatformFile().GetPlatformPhysical().ConvertToAbsolutePathForExternalAppForRead(*_FilePath);
		FString _FullPath = absPath;

		char* filepath = (char*)TCHAR_TO_ANSI(*_FullPath);

		mSaveVoicePathRoot.assign(filepath);


#else
		FString _FilePath = FPaths::ProjectSavedDir();
		FString _fullPath = FPaths::ConvertRelativePathToFull(_FilePath);
		char* filepath = (char*)TCHAR_TO_ANSI(*_fullPath);
		mSaveVoicePathRoot.assign(filepath);
#endif
#endif
	}

	string GVoiceManager::dealMessageVoicePath()
	{
		string finalvoicepath;
		FDateTime  timeNow = FDateTime::Now();
		int64 timeNowstamp = timeNow.ToUnixTimestamp();
	
		//time_t t = time(NULL);
		//string now_times = ctime(&t);
		//string voicename = t + ".dat";
		string voicename = to_string(timeNowstamp) + ".dat";


#ifdef __ANDROID__
		mSaveVoicePath = mSaveVoicePathRoot + "/" + voicename;
#else
#ifdef __APPLE__
		mSaveVoicePath.assign(mSaveVoicePathRoot + voicename);
#else
		mSaveVoicePath.assign(mSaveVoicePathRoot + voicename);
#endif
#endif
		return mSaveVoicePath;
	}

	void GVoiceManager::_handleUploadVoiceSucc(SDKEventParam& param)
	{
		if (param.str_0.empty() || param.str_1.empty())
		{
			UE_LOG(LogTemp, Warning, TEXT(" _handleUpload param is null !!!!"));
			return;
		}
		UE_LOG(LogTemp, Warning, TEXT(" _handleUploadSucc !!!!"));
		mCacheVoiceMap.insert(std::pair<std::string, std::string>(param.str_0, param.str_1));
		gVoiceInterface->TranslateMessage(param.str_0);
	}

	void GVoiceManager::_handleDownloadVoiceSucc(SDKEventParam& param)
	{
		if (param.str_0.empty() || param.str_1.empty())
		{
			UE_LOG(LogTemp, Warning, TEXT(" _handleDownloadVoice param is null !!!!"));
			return;
		}
		UE_LOG(LogTemp, Warning, TEXT(" _handleDownloadVoiceSucc !!!!"));
		mCacheVoiceMap.insert(std::pair<std::string, std::string>(param.str_0, param.str_1));
		StartPlayVoice(param.str_0);
	}

	void GVoiceManager::_handleJoinRoomSucc(SDKEventParam& param)
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

	void GVoiceManager::_handleQuitRoomSucc(SDKEventParam& param)
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

	void GVoiceManager::_handleTranslateSucc(SDKEventParam& param)
	{
		if (param.str_0.empty())
		{
			UE_LOG(LogTemp, Warning, TEXT(" _handleTranslate file id is null !!!!"));
			return;
		}
		if (mCacheVoiceMap.find(param.str_0) != mCacheVoiceMap.end())
		{
			unsigned int bytes = 1024;
			float seconds = 0;
			gVoiceInterface->GetVoiceByteAndTime(mCacheVoiceMap[param.str_0], &bytes, &seconds);
			SDKEventParam params;
			params.str_0 = param.str_0;
			params.str_1 = param.str_1;
			params.int_0 = bytes;
			params.float_0 = seconds;
			PluginsEventManager::Instance()->TriggerEvent(SDKEventType::GVoiceDealInfoSucc, params);
		}
	}

}
