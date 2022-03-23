// Fill out your copyright notice in the Description page of Project Settings.


#include "Plugins/GVoice/GVoiceInterface.h"

DEFINE_LOG_CATEGORY_STATIC(LogGVoice, Log, All);

GVoiceInterface::GVoiceInterface()
{
	mGVoiceEngine = GCloud::GVoice::GetVoiceEngine();
	mCallback = new GVocieCallBack();
}

GVoiceInterface::~GVoiceInterface()
{
	//delete mGVoiceEngine;
	mGVoiceEngine = NULL;
	//delete mCallback;
	mCallback = NULL;
}


void GVoiceInterface::SetMode(GCloud::GVoice::IGCloudVoiceEngine::Mode mode)
{
	ErrorNo GVoiceResult = mGVoiceEngine->SetMode(mode);
	mVoiceMode = mode;
	LogErrorNoResult(TEXT("SetMode"), GVoiceResult);
}

void GVoiceInterface::GRealTimeVoiceInit(std::string appID, std::string appKey, std::string openID)
{
	ErrorNo GVoiceResult  = mGVoiceEngine->SetAppInfo(appID.c_str(), appKey.c_str(), openID.c_str());
	GVoiceResult = mGVoiceEngine->Init();
	GVoiceResult = mGVoiceEngine->SetNotify(mCallback);
	GVoiceResult = mGVoiceEngine->SetMode(IGCloudVoiceEngine::kModeRealTime);
	LogErrorNoResult(TEXT("GRealTimeVoiceInit"), GVoiceResult);

}

void GVoiceInterface::Poll()
{
	mGVoiceEngine->Poll();
}

/// <summary>
/// 只支持同时加入两个房间
/// </summary>
/// <param name="enable"></param>
void GVoiceInterface::EnableMultiRoom(bool enable)
{
	ErrorNo GVoiceResult = mGVoiceEngine->EnableMultiRoom(enable);

	LogErrorNoResult(TEXT("EnableMultiRoom"), GVoiceResult);
}


void GVoiceInterface::JoinRoom(const char* roomName)
{
	if (mVoiceMode != IGCloudVoiceEngine::Mode::kModeRealTime)
	{
		SetMode(IGCloudVoiceEngine::kModeRealTime);
	}
	ErrorNo GVoiceResult = mGVoiceEngine->JoinTeamRoom(roomName, 10000);
	LogErrorNoResult(TEXT("JoinRoom"), GVoiceResult);
}
void GVoiceInterface::QuitRoom(const char* roomName)
{
	ErrorNo GVoiceResult = mGVoiceEngine->QuitRoom(roomName, 10000);
	LogErrorNoResult(TEXT("QuitRoom"), GVoiceResult);
}

void GVoiceInterface::OpenMic()
{
	ErrorNo GVoiceResult = mGVoiceEngine->OpenMic();
	LogErrorNoResult(TEXT("OpenMic"), GVoiceResult);
}
void GVoiceInterface::CloseMic()
{
	ErrorNo GVoiceResult = mGVoiceEngine->CloseMic();
	LogErrorNoResult(TEXT("CloseMic"), GVoiceResult);
}

void GVoiceInterface::OpenSpeaker()
{
	ErrorNo GVoiceResult = mGVoiceEngine->OpenSpeaker();
	LogErrorNoResult(TEXT("OpenSpeaker"), GVoiceResult);
}
void GVoiceInterface::CloseSpeaker()
{
	ErrorNo GVoiceResult = mGVoiceEngine->CloseSpeaker();
	LogErrorNoResult(TEXT("CloseSpeaker"), GVoiceResult);
}

/// <summary>
/// 开关房间麦克风
/// </summary>
/// <param name="roomName"></param>
/// <param name="enable"></param>
void GVoiceInterface::EnableRoomMicrophone(const char* roomName, bool enable)
{
	ErrorNo GVoiceResult = mGVoiceEngine->EnableRoomMicrophone(roomName, enable);
	LogErrorNoResult(TEXT("EnableRoomMicrophone"), GVoiceResult);
}
/// <summary>
/// 开关房间扬声器
/// </summary>
/// <param name="roomName"></param>
/// <param name="enable"></param>
void GVoiceInterface::EnableRoomSpeaker(const char* roomName, bool enable)
{
	ErrorNo GVoiceResult = mGVoiceEngine->EnableRoomSpeaker(roomName, enable);
	LogErrorNoResult(TEXT("EnableRoomSpeaker"), GVoiceResult);
}

/// <summary>
/// 禁止接收某成员语音
/// </summary>
/// <param name="member">指定不收听其语音数据的玩家 ID</param>
/// <param name="enable">true:不收听,false收听</param>
/// <param name="roomName">房间名，需与进房时的房间名一致</param>
void GVoiceInterface::ForbidMemberVoice(int member, bool enable, const char* roomName)
{
	ErrorNo GVoiceResult = mGVoiceEngine->ForbidMemberVoice(member, enable, roomName);
	LogErrorNoResult(TEXT("ForbidMemberVoice"), GVoiceResult);
}


bool GVoiceInterface::IsSpeaking()
{
	return mGVoiceEngine->IsSpeaking();
}


//语音消息接口

void GVoiceInterface::GRealTimeMessageInit(std::string appID, std::string appKey, std::string openID)
{
	ErrorNo GVoiceResult = mGVoiceEngine->SetAppInfo(appID.c_str(), appKey.c_str(), openID.c_str());
	GVoiceResult = mGVoiceEngine->Init();
	if (GVoiceResult == ErrorNo::kErrorNoSucc)
	{
		GVoiceResult = mGVoiceEngine->SetMode(IGCloudVoiceEngine::kModeMessages);
		GVoiceResult = mGVoiceEngine->SetNotify(mCallback);
		GVoiceResult = mGVoiceEngine->ApplyMessageKey();
		LogErrorNoResult(TEXT("GRealTimeMessageInit"), GVoiceResult);
	}
}

void GVoiceInterface::ApplyAuthKey()
{
	ErrorNo GVoiceResult = mGVoiceEngine->ApplyMessageKey();
	LogErrorNoResult(TEXT("ApplyAuthKey"), GVoiceResult);
}

void GVoiceInterface::StartRecord(std::string filepath = "")
{
	if (mVoiceMode != IGCloudVoiceEngine::Mode::kModeMessages)
	{
		SetMode(IGCloudVoiceEngine::Mode::kModeMessages);
	}
	ErrorNo GVoiceResult = mGVoiceEngine->StartRecording(filepath.c_str());
	LogErrorNoResult(TEXT("StartRecord"), GVoiceResult);
}

void GVoiceInterface::StopRecord()
{
	ErrorNo GVoiceResult = mGVoiceEngine->StopRecording();
	LogErrorNoResult(TEXT("StopRecord"), GVoiceResult);
	//Upload(mSaveVoicePath);
}

void GVoiceInterface::Upload(std::string filepath)
{
	ErrorNo GVoiceResult = mGVoiceEngine->UploadRecordedFile(filepath.c_str());
	LogErrorNoResult(TEXT("Upload"), GVoiceResult);
}

void GVoiceInterface::Download(std::string fileid, std::string filepath)
{
	if (fileid.empty() || filepath.empty())
	{
		return;
	}
	ErrorNo GVoiceResult = mGVoiceEngine->DownloadRecordedFile(fileid.c_str(), filepath.c_str());
	LogErrorNoResult(TEXT("Download"), GVoiceResult);
}

void GVoiceInterface::Play(std::string filepath)
{
	ErrorNo GVoiceResult = mGVoiceEngine->PlayRecordedFile(filepath.c_str());
	LogErrorNoResult(TEXT("Play"), GVoiceResult);
}

void GVoiceInterface::StopPlay()
{
	ErrorNo GVoiceResult = mGVoiceEngine->StopPlayFile();
	LogErrorNoResult(TEXT("StopPlay"), GVoiceResult);
}

void GVoiceInterface::TranslateMessage(string fileid)
{
	if (mVoiceMode != IGCloudVoiceEngine::Mode::kModeRSTT)
	{
		SetMode(IGCloudVoiceEngine::kModeRSTT);
	}
	ErrorNo GVoiceResult = mGVoiceEngine->SpeechToText(fileid.c_str(),60000,IGCloudVoiceEngine::Language::kLanguageChina);
}

void GVoiceInterface::GetVoiceByteAndTime(string filepath, unsigned int* bytes, float* seconds)
{
	ErrorNo GVoiceResult = mGVoiceEngine->GetFileParam(filepath.c_str(), bytes, seconds);
	LogErrorNoResult(TEXT("GetVoiceByteAndTime"), GVoiceResult);
}

void GVoiceInterface::LogErrorNoResult(FString type, ErrorNo  error)
{
	UE_LOG(LogGVoice, Warning, TEXT("GVoice %s res is  %s "), *type, *FString::FromInt(error));
}

