// Fill out your copyright notice in the Description page of Project Settings.


#include "Plugins/PluginsManager.h"
#include "Plugins/GVoice/GVoiceManager.h"
namespace TenmoveSDK
{
	PluginsEventManager* PluginsEventManager::mInstance = nullptr;

	PluginsEventManager::~PluginsEventManager()
	{
		SAFE_DELETE_PTR(mInstance);
		ClearHandleList();
	}
	PluginsEventManager* PluginsEventManager::PluginsEventManager::Instance()
	{
		if (nullptr == mInstance)
		{
			mInstance = new PluginsEventManager();
		}
		return mInstance;
	}

	SDKEventHandle* PluginsEventManager::RegisterEvent(SDKEventType type, SDKEventHandle::SDKDel sdkDel)
	{
		SDKEventHandle* handle = mEventProcessor.AddSDKEventHandle((int)type, sdkDel);
		return handle;
	}

	void PluginsEventManager::RemoveEvent(SDKEventHandle* handler)
	{
		mEventProcessor.RemoveHandler(handler);
	}

	void PluginsEventManager::TriggerEvent(SDKEventType type)
	{
		SDKEventParam param;
		mEventProcessor.HandleEvent((int)type, param);
	}

	void PluginsEventManager::TriggerEvent(SDKEventType type, SDKEventParam& param)
	{
		mEventProcessor.HandleEvent((int)type, param);
	}

	void PluginsEventManager::ClearHandleList()
	{
		mEventProcessor.ClearAll();
	}



	PluginsManager::PluginsManager()
	{
		_InitPluginsManager();

		GVoiceManager* A = new GVoiceManager();

	}

	PluginsManager::~PluginsManager()
	{
		_UnInitPluginsManager();
	}
	PluginsManager* PluginsManager::mInstance = nullptr;
	PluginsManager* PluginsManager::GetInstance()
	{
		if (nullptr == mInstance)
		{
			mInstance = new PluginsManager();
		}
		return mInstance;
	}

	void PluginsManager::SetVoicePoll()
	{
		gVoiceManager->SetVoicePoll();
	}

	void PluginsManager::InitMessageVoice(string openId)
	{
		gVoiceManager->InitMessageVoice(openId);
	}

	void PluginsManager::StartRecord()
	{
		gVoiceManager->StartRecord();
	}

	void PluginsManager::StopRecord()
	{
		gVoiceManager->StopRecord();
	}

	void PluginsManager::StartPlayVoice(string voiceFileId)
	{
		gVoiceManager->StartPlayVoice(voiceFileId);
	}

	void PluginsManager::StopPlayVoice()
	{
		gVoiceManager->StopPlayVoice();
	}

	void PluginsManager::InitRealTimeVoice(string openId)
	{
		gVoiceManager->InitRealTimeVoice(openId);
	}

	void PluginsManager::JoinRoom(string roomName)
	{
		gVoiceManager->JoinRoom(roomName);
	}

	void PluginsManager::QuitRoom(string roomName)
	{
		gVoiceManager->QuitRoom(roomName);
	}

	void PluginsManager::QuitAllRoom()
	{
		gVoiceManager->QuitAllRoom();
	}

	void PluginsManager::OpenMic()
	{
		gVoiceManager->OpenMic();
	}

	void PluginsManager::CloseMic()
	{
		gVoiceManager->CloseMic();
	}

	void PluginsManager::OpenSpeaker()
	{
		gVoiceManager->OpenSpeaker();
	}

	void PluginsManager::CloseSpeaker()
	{
		gVoiceManager->CloseSpeaker();
	}

	void PluginsManager::EnableRoomMicrophone(string roomName, bool enable)
	{
		gVoiceManager->EnableRoomMicrophone(roomName, enable);
	}

	void PluginsManager::EnableRoomSpeaker(string roomName, bool enable)
	{
		gVoiceManager->EnableRoomSpeaker(roomName, enable);
	}

	void PluginsManager::ForbidMemberVoice(int member, bool enable, string roomName)
	{
		gVoiceManager->ForbidMemberVoice(member, enable, roomName);
	}

	bool PluginsManager::IsSpeaking()
	{
		return gVoiceManager->IsSpeaking();
	}

	void PluginsManager::_InitPluginsManager()
	{
		gVoiceManager = new GVoiceManager();
	}

	void PluginsManager::_UnInitPluginsManager()
	{
		SAFE_DELETE_PTR(gVoiceManager);
	}

}