// Fill out your copyright notice in the Description page of Project Settings.

#pragma once


#include "CoreMinimal.h"
#include <string>
#include "Plugins/GVoice/GVocieCallBack.h"

#include "IGCloudVoiceEngine.h"
#include "IGCloudVoiceEngineNotify.h"
//#include "IGCloudVoiceExtension.h"

using namespace GCloud::GVoice;
using namespace std;



/**
 *
 */

enum class GVoiceMode
{
	None,
	MessageMode,
	RealTimeMode,
	TranslateMode
};

class UESDKTEST_API GVoiceInterface
{
public:
	GVoiceInterface();
	~GVoiceInterface();

public:
	void Poll();
	void SetMode(GCloud::GVoice::IGCloudVoiceEngine::Mode mode);

	//实时消息相关
public:
	void GRealTimeVoiceInit(std::string appID, std::string appKey, std::string openID);
	//是否允许多房间
	void EnableMultiRoom(bool enable);
	void JoinRoom(const char* roomName);
	void QuitRoom(const char* roomName);
	void OpenMic();
	void CloseMic();
	void OpenSpeaker();
	void CloseSpeaker();
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
	void GRealTimeMessageInit(std::string appID, std::string appKey, std::string openID);
	void ApplyAuthKey();
	void StartRecord(std::string filepath);
	void StopRecord();
	void Upload(std::string filepath);
	void Download(std::string fileid, std::string filepath);
	void Play(std::string filepath);
	void StopPlay();
	void TranslateMessage(string fileid );
	void GetVoiceByteAndTime(string filepath, unsigned int* bytes, float* seconds);
	GVocieCallBack* mCallback;

private:
	IGCloudVoiceEngine::Mode mVoiceMode;
	IGCloudVoiceEngine* mGVoiceEngine;
	void LogErrorNoResult(FString type, ErrorNo error);


};
