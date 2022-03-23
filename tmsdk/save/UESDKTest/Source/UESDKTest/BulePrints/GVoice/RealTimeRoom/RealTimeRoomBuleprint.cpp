// Fill out your copyright notice in the Description page of Project Settings.


#include "RealTimeRoomBuleprint.h"


URealTimeRoomBuleprint::URealTimeRoomBuleprint(const FObjectInitializer& ObjectInitializer) :Super(ObjectInitializer)
{
	//    this->mConnectorTestMgr = ConnectorTestManager::Instance();
	//    this->mConnectorTestMgr->SetBlueprintObj((UTestUserWidgetBase*)this);
	MicBtnLbl = "OpenMic";
	SpeakerBtnLbl = "OpenSpeaker";
	SaveFileBtnLbl = "StartSaveVoice";
	PlayVoiceBtnLbl = "StartPlayVoice";
	defaultURL_ = "udp://capi.voice.gcloud.qq.com:10001";
	logInfo = "Log: ";
	//TeamRoomTestManager::Instance()->SetBlueprintObj(this);
}

bool URealTimeRoomBuleprint::Initialize()
{
	if (!UUserWidget::Initialize()) {
		return false;
	}
	//TeamRoomTestManager::Instance()->SetBlueprintObj(this);
	return true;
}

void URealTimeRoomBuleprint::OnInit()
{
	PluginsEventManager::Instance()->RegisterEvent(SDKEventType::GVoiceJoinRoomSucc, EVENT_FUNC(_handleJoinRoomSucc));
	PluginsEventManager::Instance()->RegisterEvent(SDKEventType::GVoiceQuitRoomSucc, EVENT_FUNC(_handleQuitRoomSucc));
	PluginsEventManager::Instance()->RegisterEvent(SDKEventType::GVoiceMemberSlateChanger, EVENT_FUNC(_handleMemberSlateChanger));
	std::string openID(TCHAR_TO_UTF8(*openID_));
	//std::string URL(TCHAR_TO_UTF8(*defaultURL_));
	// PluginsManager::GetInstance()->Init("gcloud.test","test_key", openID, URL);
	PluginsManager::GetInstance()->InitRealTimeVoice(openID);
	//PluginsManager::GetInstance()->SetMode(GCloud::GVoice::IGCloudVoiceEngine::kModeRealTime);

	GetWorld()->GetTimerManager().SetTimer(timerHandler_, this, &URealTimeRoomBuleprint::TimerFunc, 0.5F, true);
	SetLog("Init!!!");
}

void URealTimeRoomBuleprint::TimerFunc()
{

	PluginsManager::GetInstance()->SetVoicePoll();
}

void URealTimeRoomBuleprint::SetLog(FString log)
{
	logInfo += log+"\n";
}

void URealTimeRoomBuleprint::_handleJoinRoomSucc(SDKEventParam& param)
{
	string msg = "JoinRoom Successful , roomeName : " + param.str_0 + " memberID: " + to_string(param.int_0);
	SetLog(msg.c_str());
}

void URealTimeRoomBuleprint::_handleQuitRoomSucc(SDKEventParam& param)
{
	string msg = "QuitRoom Successful , roomeName : " + param.str_0;
	SetLog(msg.c_str());
}

void URealTimeRoomBuleprint::_handleMemberSlateChanger(SDKEventParam& param)
{
	string msg = "Have MemberSlateChanger , roomeName : " + param.str_0 + " memberID: " + to_string(param.int_0) + " statue : " + to_string(param.int_1);
	SetLog(msg.c_str());
}

void URealTimeRoomBuleprint::OnJoinRoom()
{
	std::string roomName(TCHAR_TO_UTF8(*roomName_));
	PluginsManager::GetInstance()->JoinRoom(roomName);
}

void URealTimeRoomBuleprint::OnQuitRoom()
{
	std::string roomName(TCHAR_TO_UTF8(*roomName_));
	PluginsManager::GetInstance()->QuitRoom(roomName);
}


void URealTimeRoomBuleprint::OnQuitAllRoom()
{
	PluginsManager::GetInstance()->QuitAllRoom();
}

void URealTimeRoomBuleprint::OnMic()
{
	std::string roomName(TCHAR_TO_UTF8(*roomName_));
	static bool once = true;
	if (once) {
		MicBtnLbl = "CloseMic";
		PluginsManager::GetInstance()->OpenMic();
		once = false;
		SetLog("OpenMic!!!");
	}
	else {
		MicBtnLbl = "OpenMic";
		PluginsManager::GetInstance()->CloseMic();
		once = true;
		SetLog("CloseMic!!!");
	}
}

void URealTimeRoomBuleprint::OnSpeaker()
{
	static bool once = true;
	if (once) {
		SpeakerBtnLbl = "CloseSpeaker";
		PluginsManager::GetInstance()->OpenSpeaker();
		once = false;
		SetLog("OpenSpeaker!!!");
	}
	else {
		SpeakerBtnLbl = "OpenSpeaker";
		PluginsManager::GetInstance()->CloseSpeaker();
		once = true;
		SetLog("CloseSpeaker!!!");
	}
}

void URealTimeRoomBuleprint::OnInputRoomName(const FText& InText)
{
	FString msg = "RoomName:";
	msg += InText.ToString();
	roomName_ = InText.ToString();
}


void URealTimeRoomBuleprint::OnInputSetOpenIDs(const FText& InText)
{
	FString msg = "SetOpenIDs:";
	msg += InText.ToString();
	sOpenIDs_ = InText.ToString();
}


void URealTimeRoomBuleprint::OnInputSetMemberIDs(const FText& InText)
{
	FString msg = "SetMemberIDs:";
	msg += InText.ToString();
	sMemberIDs_ = InText.ToString();
}


void URealTimeRoomBuleprint::OnInputReportOpenIDs(const FText& InText)
{
	FString msg = "ReportOpenIDs:";
	msg += InText.ToString();
	rOpenIDs_ = InText.ToString();
}


void URealTimeRoomBuleprint::OnInputReportMemberIDs(const FText& InText)
{
	FString msg = "ReportMemberIDs:";
	msg += InText.ToString();
	sMemberIDs_ = InText.ToString();
}

void URealTimeRoomBuleprint::OnClearLog()
{
	logInfo = "Log: ";
}

void URealTimeRoomBuleprint::OnInputOpenID(const FText& InText)
{
	FString msg = "OpenID:";
	msg += InText.ToString();
	openID_ = InText.ToString();
}

void URealTimeRoomBuleprint::OnInputRangRoomName(const FText& InText)
{
	FString msg = "RangeRoom:";
	msg += InText.ToString();
	rangeName_ = InText.ToString();

}
