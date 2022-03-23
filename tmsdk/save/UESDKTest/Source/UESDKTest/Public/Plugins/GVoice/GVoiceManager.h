// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Plugins/GVoice/GVoiceInterface.h"
#include "Plugins/PluginsManager.h"
#include <string>
#include <list>
#include <map>
using namespace std;

/**
 *
 */
namespace TenmoveSDK
{
	class UESDKTEST_API GVoiceManager
	{
	public:
		GVoiceManager();
		~GVoiceManager();
	public:
		void SetVoicePoll();

		void InitMessageVoice(string openId);
		void StartRecord();
		void StopRecord();
		void CancleRecord();
		void StartPlayVoice(string voiceFileId);
		void StopPlayVoice();

		//realtime voice
		void InitRealTimeVoice(string openId);
		void EnableMultiRoom(bool enable);
		void JoinRoom(string roomName);
		void QuitRoom(string roomName);
		void QuitAllRoom();
		void OpenMic();
		void CloseMic();
		void OpenSpeaker();
		void CloseSpeaker();
		//是否允许房间麦克
		void EnableRoomMicrophone(string roomName, bool enable);
		//是否允许房间扬声器
		void EnableRoomSpeaker(string roomName, bool enable);
		//屏蔽某人
		void ForbidMemberVoice(int member, bool enable, string roomName);
		//是否正在说话
		bool IsSpeaking();

	private:
		void _InitAppIDAndKey();
		void _InitMessageVoicePathRoot();
		string dealMessageVoicePath();

		void _handleUploadVoiceSucc(SDKEventParam& param);
		void _handleDownloadVoiceSucc(SDKEventParam& param);
		void _handleJoinRoomSucc(SDKEventParam& param);
		void _handleQuitRoomSucc(SDKEventParam& param);
		void _handleTranslateSucc(SDKEventParam& param);
	private:
		GVoiceInterface* gVoiceInterface;
		FString gVoiceAppID;
		FString gVoiceAppKey;
		std::string mSaveVoicePathRoot;
		std::string mSaveVoicePath;
		map<string, string> mCacheVoiceMap;

		list<string> mRoomList;
	};

}
