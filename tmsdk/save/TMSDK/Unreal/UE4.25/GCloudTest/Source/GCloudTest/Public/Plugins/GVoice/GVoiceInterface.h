// Fill out your copyright notice in the Description page of Project Settings.

#pragma once


#include "CoreMinimal.h"
#include <string>
#include "GVoice/GCloudVoice.h"
#include "Plugins/GVoice/GVocieCallBack.h"
//#include "IGCloudVoiceEngine.h"
//#include "IGCloudVoiceEngineNotify.h"
#include "GVoice/GCloudVoiceNotify.h"

using namespace gcloud_voice;
using namespace std;

enum class GVoiceMode
{
	None,
	MessageMode,
	RealTimeMode,
	TranslateMode
};
class GCLOUDTEST_API GVoiceInterface
{
public:
	GVoiceInterface();
	~GVoiceInterface();

public:
	void InitApp(std::string appID, std::string appKey, std::string openID);
	void Poll();
	void SetMode(GCloudVoiceMode mode);
	void Pause();
	void Resume();

	//实时消息相关
public:
	//void GRealTimeVoiceInit(std::string appID, std::string appKey, std::string openID);
	//是否允许多房间
	void EnableMultiRoom(bool enable);
	void JoinRoom(const char* roomName);
	void QuitRoom(const char* roomName);
	void ControlMic(bool isOpen);
	void ControlSpeaker(bool isOpen);
	//是否允许房间麦克
	void EnableRoomMicrophone(const char* roomName, bool enable);
	//是否允许房间扬声器
	void EnableRoomSpeaker(const char* roomName, bool enable);
	//屏蔽某人
	void ForbidMemberVoice(int member, bool enable, const char* roomName);
	//是否正在说话
	bool IsSpeaking();

	//语音消息相关
public:
	//void GRealTimeMessageInit(std::string appID, std::string appKey, std::string openID);
	void ApplyAuthKey();
	void StartRecord(std::string filepath);
	void StartRecordOnlyForTrans(std::string filepath);
	void StopRecord();
	void Upload(std::string filepath);
	void Download(std::string fileid, std::string filepath);
	void Play(std::string filepath);
	void StopPlay();
	void TranslateMessage(string fileid );
	void GetVoiceByteAndTime(string filepath, unsigned int* bytes, float* seconds);
private:
	GCloudVoiceMode mVoiceMode = GCloudVoiceMode::Unknown;
	IGCloudVoiceEngine* mGVoiceEngine = nullptr;
	GVocieCallBack* mCallback = nullptr;
	void LogErrorNoResult(FString type, GCloudVoiceErrno error);
	bool mIsGVoiceInit = false;
	bool mIsApplyMessageKey = false;
	bool mIsNeedPoll = false;


};
