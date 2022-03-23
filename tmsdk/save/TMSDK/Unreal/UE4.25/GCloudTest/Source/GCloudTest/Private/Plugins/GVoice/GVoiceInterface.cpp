// Fill out your copyright notice in the Description page of Project Settings.


#include "Plugins/GVoice/GVoiceInterface.h"
#include "UtilityMarcoDefine.h"

GVoiceInterface::GVoiceInterface()
{
	mGVoiceEngine = gcloud_voice::GetVoiceEngine();
	mCallback = new GVocieCallBack();
	mIsGVoiceInit = false;
	mIsNeedPoll = false;
}

GVoiceInterface::~GVoiceInterface()
{
	//SAFE_DELETE_PTR(mGVoiceEngine);
	SAFE_DELETE_PTR(mCallback);
	mIsNeedPoll = false;
	mIsGVoiceInit = false;
	mIsApplyMessageKey = false;
}


void GVoiceInterface::InitApp(std::string appID, std::string appKey, std::string openID)
{
	if (mIsGVoiceInit)
	{
		return;
	}
	GCloudVoiceErrno GVoiceResult = mGVoiceEngine->SetAppInfo(appID.c_str(), appKey.c_str(), openID.c_str());
	LogErrorNoResult(TEXT("GRealTimeVoiceInit"), GVoiceResult);
	if (GVoiceResult == GCloudVoiceErrno::GCLOUD_VOICE_SUCC)
	{
		GVoiceResult = mGVoiceEngine->Init();
		if (GVoiceResult == GCloudVoiceErrno::GCLOUD_VOICE_SUCC)
		{
			mIsGVoiceInit = true;
			mIsNeedPoll = true;
			GVoiceResult = mGVoiceEngine->SetNotify(mCallback);
			LogErrorNoResult(TEXT("SetNotify"), GVoiceResult);
			ApplyAuthKey();
			unsigned int i = 1;
			unsigned int* j = &i;
			mGVoiceEngine->invoke(9004, i, i, j);
		}
	}	
}

void GVoiceInterface::SetMode(GCloudVoiceMode mode)
{
	GCloudVoiceErrno GVoiceResult = mGVoiceEngine->SetMode(mode);
	mVoiceMode = mode;
	LogErrorNoResult(TEXT("SetMode"), GVoiceResult);
	if (mVoiceMode == GCloudVoiceMode::Messages && !mIsApplyMessageKey)
	{
		ApplyAuthKey();
	}
	else if (mVoiceMode == GCloudVoiceMode::RealTime)
	{
		EnableMultiRoom(true);
	}
}

void GVoiceInterface::Pause()
{
	if (!mIsGVoiceInit)
	{
		return;
	}
	mGVoiceEngine->Pause();
}

void GVoiceInterface::Resume()
{
	if (!mIsGVoiceInit)
	{
		return;
	}
	mGVoiceEngine->Resume();
}

//void GVoiceInterface::GRealTimeVoiceInit(std::string appID, std::string appKey, std::string openID)
//{
//	InitApp(appID,appKey,openID);
//	if (mIsGVoiceInit)
//	{
//		GCloudVoiceErrno GVoiceResult = mGVoiceEngine->SetNotify(mCallback);
//		LogErrorNoResult(TEXT("GRealTimeVoiceInit"), GVoiceResult);
//		GVoiceResult = mGVoiceEngine->SetMode(IGCloudVoiceEngine::kModeRealTime);
//		LogErrorNoResult(TEXT("GRealTimeVoiceInit"), GVoiceResult);
//		mIsNeedPoll = true;
//	}
//
//}

void GVoiceInterface::Poll()
{
	if (mIsGVoiceInit && mIsNeedPoll)
	{
		mGVoiceEngine->Poll();
	}
}

/// <summary>
/// ֻ֧��ͬʱ������������
/// </summary>
/// <param name="enable"></param>
void GVoiceInterface::EnableMultiRoom(bool enable)
{
	if (!mIsGVoiceInit)
	{
		return;
	}
	GCloudVoiceErrno GVoiceResult = mGVoiceEngine->EnableMultiRoom(enable);

	LogErrorNoResult(TEXT("EnableMultiRoom"), GVoiceResult);
}


void GVoiceInterface::JoinRoom(const char* roomName)
{
	if (!mIsGVoiceInit)
	{
		return;
	}
	if (mVoiceMode != GCloudVoiceMode::RealTime)
	{
		SetMode(GCloudVoiceMode::RealTime);
	}
	GCloudVoiceErrno GVoiceResult = mGVoiceEngine->JoinTeamRoom(roomName, 10000);
	LogErrorNoResult(TEXT("JoinRoom"), GVoiceResult);
}
void GVoiceInterface::QuitRoom(const char* roomName)
{
	if (!mIsGVoiceInit)
	{
		return;
	}
	GCloudVoiceErrno GVoiceResult = mGVoiceEngine->QuitRoom(roomName, 10000);
	LogErrorNoResult(TEXT("QuitRoom"), GVoiceResult);
}

void GVoiceInterface::ControlMic(bool isOpen)
{
	if (!mIsGVoiceInit)
	{
		return;
	}
	if (isOpen)
	{
		GCloudVoiceErrno GVoiceResult = mGVoiceEngine->OpenMic();
		LogErrorNoResult(TEXT("OpenMic"), GVoiceResult);
	}
	else
	{
		GCloudVoiceErrno GVoiceResult = mGVoiceEngine->CloseMic();
		LogErrorNoResult(TEXT("CloseMic"), GVoiceResult);
	}
}

void GVoiceInterface::ControlSpeaker(bool isOpen)
{
	if (!mIsGVoiceInit)
	{
		return;
	}
	if (isOpen)
	{
		GCloudVoiceErrno GVoiceResult = mGVoiceEngine->OpenSpeaker();
		LogErrorNoResult(TEXT("OpenSpeaker"), GVoiceResult);
	}
	else
	{
		GCloudVoiceErrno GVoiceResult = mGVoiceEngine->CloseSpeaker();
		LogErrorNoResult(TEXT("CloseSpeaker"), GVoiceResult);
	}
}

/// <summary>
/// ���ط�����˷�
/// </summary>
/// <param name="roomName"></param>
/// <param name="enable"></param>
void GVoiceInterface::EnableRoomMicrophone(const char* roomName, bool enable)
{
	if (!mIsGVoiceInit)
	{
		return;
	}
	GCloudVoiceErrno GVoiceResult = mGVoiceEngine->EnableRoomMicrophone(roomName, enable);
	LogErrorNoResult(TEXT("EnableRoomMicrophone"), GVoiceResult);
}
/// <summary>
/// ���ط���������
/// </summary>
/// <param name="roomName"></param>
/// <param name="enable"></param>
void GVoiceInterface::EnableRoomSpeaker(const char* roomName, bool enable)
{
	if (!mIsGVoiceInit)
	{
		return;
	}
	GCloudVoiceErrno GVoiceResult = mGVoiceEngine->EnableRoomSpeaker(roomName, enable);
	LogErrorNoResult(TEXT("EnableRoomSpeaker"), GVoiceResult);
}

/// <summary>
/// ��ֹ����ĳ��Ա����
/// </summary>
/// <param name="member">ָ�����������������ݵ���� ID</param>
/// <param name="enable">true:������,false����</param>
/// <param name="roomName">���������������ʱ�ķ�����һ��</param>
void GVoiceInterface::ForbidMemberVoice(int member, bool enable, const char* roomName)
{
	if (!mIsGVoiceInit)
	{
		return;
	}
	GCloudVoiceErrno GVoiceResult = mGVoiceEngine->ForbidMemberVoice(member, enable, roomName);
	LogErrorNoResult(TEXT("ForbidMemberVoice"), GVoiceResult);
}


bool GVoiceInterface::IsSpeaking()
{
	if (!mIsGVoiceInit)
	{
		return false;
	}
	return mGVoiceEngine->IsSpeaking();
}


//������Ϣ�ӿ�

//void GVoiceInterface::GRealTimeMessageInit(std::string appID, std::string appKey, std::string openID)
//{
//	InitApp(appID,appKey,openID);
//	if (mIsGVoiceInit)
//	{
//		GCloudVoiceErrno GVoiceResult = mGVoiceEngine->SetMode(IGCloudVoiceEngine::kModeMessages);
//		LogErrorNoResult(TEXT("SetMode"), GVoiceResult);
//		GVoiceResult = mGVoiceEngine->SetNotify(mCallback);
//		LogErrorNoResult(TEXT("SetNotify"), GVoiceResult);
//		GVoiceResult = mGVoiceEngine->ApplyMessageKey();
//		LogErrorNoResult(TEXT("ApplyMessageKey"), GVoiceResult);
//		mIsNeedPoll = true;
//	}
//}

void GVoiceInterface::ApplyAuthKey()
{
	if (!mIsGVoiceInit)
	{
		return;
	}
	GCloudVoiceErrno GVoiceResult = mGVoiceEngine->ApplyMessageKey();
	if (GVoiceResult == GCloudVoiceErrno::GCLOUD_VOICE_SUCC)
	{
		mIsApplyMessageKey = true;
	}
	LogErrorNoResult(TEXT("ApplyAuthKey"), GVoiceResult);

}

void GVoiceInterface::StartRecord(std::string filepath = "")
{
	if (!mIsGVoiceInit)
	{
		return;
	}
	if (mVoiceMode != GCloudVoiceMode::Messages)
	{
		SetMode(GCloudVoiceMode::Messages);
	}
	GCloudVoiceErrno GVoiceResult = mGVoiceEngine->StartRecording(filepath.c_str());
	LogErrorNoResult(TEXT("StartRecord"), GVoiceResult);
}

void GVoiceInterface::StartRecordOnlyForTrans(std::string filepath)
{
	if (!mIsGVoiceInit)
	{
		return;
	}
	if (mVoiceMode != GCloudVoiceMode::RSTT)
	{
		SetMode(GCloudVoiceMode::RSTT);
	}
	GCloudVoiceErrno GVoiceResult = mGVoiceEngine->StartRecording(filepath.c_str());
	LogErrorNoResult(TEXT("StartRecordOnlyForTrans"), GVoiceResult);
}

void GVoiceInterface::StopRecord()
{
	if (!mIsGVoiceInit)
	{
		return;
	}
	GCloudVoiceErrno GVoiceResult = mGVoiceEngine->StopRecording();
	LogErrorNoResult(TEXT("StopRecord"), GVoiceResult);
	//Upload(mSaveVoicePath);
}

void GVoiceInterface::Upload(std::string filepath)
{
	if (!mIsGVoiceInit)
	{
		return;
	}
	GCloudVoiceErrno GVoiceResult = mGVoiceEngine->UploadRecordedFile(filepath.c_str());
	LogErrorNoResult(TEXT("Upload"), GVoiceResult);
}

void GVoiceInterface::Download(std::string fileid, std::string filepath)
{
	if (!mIsGVoiceInit)
	{
		return;
	}
	if (fileid.empty() || filepath.empty())
	{
		return;
	}
	GCloudVoiceErrno GVoiceResult = mGVoiceEngine->DownloadRecordedFile(fileid.c_str(), filepath.c_str());
	LogErrorNoResult(TEXT("Download"), GVoiceResult);
}

void GVoiceInterface::Play(std::string filepath)
{
	if (!mIsGVoiceInit)
	{
		return;
	}
	GCloudVoiceErrno GVoiceResult = mGVoiceEngine->PlayRecordedFile(filepath.c_str());
	LogErrorNoResult(TEXT("Play"), GVoiceResult);
}

void GVoiceInterface::StopPlay()
{
	if (!mIsGVoiceInit)
	{
		return;
	}
	GCloudVoiceErrno GVoiceResult = mGVoiceEngine->StopPlayFile();
	LogErrorNoResult(TEXT("StopPlay"), GVoiceResult);
}

void GVoiceInterface::TranslateMessage(string fileid)
{
	if (!mIsGVoiceInit)
	{
		return;
	}
	if (mVoiceMode != GCloudVoiceMode::Translation)
	{
		unsigned int i = 1;
		unsigned int* j = &i;
		mGVoiceEngine->invoke(9004, i, i, j);
		SetMode(GCloudVoiceMode::Translation);
	}
	GCloudVoiceErrno GVoiceResult = mGVoiceEngine->SpeechToText(fileid.c_str(),60000, GCloudLanguage::Chinese);
}

void GVoiceInterface::GetVoiceByteAndTime(string filepath, unsigned int* bytes, float* seconds)
{
	if (!mIsGVoiceInit)
	{
		return;
	}
	GCloudVoiceErrno GVoiceResult = mGVoiceEngine->GetFileParam(filepath.c_str(), bytes, seconds);
	LogErrorNoResult(TEXT("GetVoiceByteAndTime"), GVoiceResult);
}

void GVoiceInterface::LogErrorNoResult(FString type, GCloudVoiceErrno  error)
{
	UE_LOG(LogTemp, Warning, TEXT("GVoice %s res is  %s "), *type, *FString::FromInt(error));
}

