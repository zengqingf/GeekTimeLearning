// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Plugins/SDKEvent/SDKEvent.h"

using namespace std;
/**
 * 
 */
namespace TenmoveSDK
{
	class GVoiceManager;

	class PluginsEventManager
	{
	public:
		PluginsEventManager() {}
		~PluginsEventManager();
	private:
		static PluginsEventManager* mInstance;
	public:
		SDKEventProcessor mEventProcessor;
		static PluginsEventManager* Instance();
		SDKEventHandle* RegisterEvent(SDKEventType type, SDKEventHandle::SDKDel sdkDel);
		void RemoveEvent(SDKEventHandle* handler);
		void TriggerEvent(SDKEventType type);
		void TriggerEvent(SDKEventType type, SDKEventParam& param);
		void ClearHandleList();
	};

	class UESDKTEST_API PluginsManager
	{
	public:
		PluginsManager();
		~PluginsManager();
	public:
		static PluginsManager* GetInstance();
		void SetVoicePoll();


		void InitMessageVoice(string openId);
		void StartRecord();
		void StopRecord();
		void StartPlayVoice(string voiceFileId);
		void StopPlayVoice();

		//realtime voice		
		void InitRealTimeVoice(string openId);
		void JoinRoom(string roomName);
		void QuitRoom(string roomName);
		void QuitAllRoom();
		void OpenMic();
		void CloseMic();
		void OpenSpeaker();
		void CloseSpeaker();
		void EnableRoomMicrophone(string roomName, bool enable);
		void EnableRoomSpeaker(string roomName, bool enable);
		void ForbidMemberVoice(int member, bool enable, string roomName);
		bool IsSpeaking();

	private:
		void _InitPluginsManager();
		void _UnInitPluginsManager();
	private:
		static PluginsManager* mInstance;
		GVoiceManager* gVoiceManager;
	};

#define SAFE_DELETE_PTR(p)\
if(p != nullptr)\
{\
    delete p;\
    p = nullptr;\
}

#define EVENT_FUNC(func) [&](SDKEventParam& __param) { func(__param); }

}
