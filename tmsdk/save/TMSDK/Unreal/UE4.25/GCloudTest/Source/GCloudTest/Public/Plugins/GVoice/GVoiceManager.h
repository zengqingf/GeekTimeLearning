// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Plugins/GVoice/GVoiceInterface.h"
#include "Plugins/TMSDKDefine.h"

#include <string>
#include <list>
#include <map>
#include <vector>
using std::string;
using std::list;
using std::map;
using std::vector;

class GCLOUDTEST_API GVoiceManager : public IPluginMgr
{
public:
	GVoiceManager();
	~GVoiceManager();
public:
	void Init() override;
	void Uninit() override;
	bool Tick(float DeltaTime) override;

	void InitGVoice(string openId);
	void SetVoicePoll();
	void Pause();
	void Resume();

	//void InitMessageVoice(string openId);
	void StartRecord(bool onlyTranslate = false);
	void StopRecord(bool onlyTranslate = false);
	void CancleRecord();
	void StartPlayVoice(string voiceFileId);
	void StopPlayVoice();

	//realtime voice
	//void InitRealTimeVoice(string openId);
	void EnableMultiRoom(bool enable);
	void JoinRoom(string roomName);
	void QuitRoom(string roomName);
	void QuitAllRoom();
	void ControlMic(bool isOpen);
	void ControlSpeaker(bool isOpen);
	//是否允许房间麦克
	void EnableRoomMicrophone(string roomName, bool enable);
	//是否允许房间扬声器
	void EnableRoomSpeaker(string roomName, bool enable);
	//屏蔽某人
	void ForbidMemberVoice(int member, bool enable, string roomName);
	//是否正在说话
	bool IsSpeaking();

private:
	void InitAppIDAndKey();
	void InitMessageVoicePathRoot();
	void ClaerMessageVoicePath();
	string GetDealMessageVoicePath();

	void HandleUploadVoiceSucc(const SDKEventParam& param);
	void HandleDownloadVoiceSucc(const SDKEventParam& param);
	void HandleJoinRoomSucc(const SDKEventParam& param);
	void HandleQuitRoomSucc(const SDKEventParam& param);
	void HandleTranslateSucc(const SDKEventParam& param);
private:
	GVoiceInterface* mGVoiceInterface = nullptr;
	FString mGVoiceAppID = "";
	FString mGVoiceAppKey = "";
	string mSaveVoicePathRoot = "";
	string mSaveVoicePath = "";
	map<string, string> mCacheVoiceMap;
	list<string> mRoomList;
	vector<SDKEventHandle*> mEventHandleList;
};