// Fill out your copyright notice in the Description page of Project Settings.


#include "MessageRoomBlueprint.h"

#include "Plugins/PluginsManager.h"
#include "Plugins/GVoice/GVoiceInterface.h"

UMessageRoomBlueprint::UMessageRoomBlueprint(const FObjectInitializer& ObjectInitializer) :Super(ObjectInitializer)
{
	RecordBtnLbl = "StartRecord";
	PlayBtnLbl = "StartPlay";
	defaultURL_ = "udp://capi.voice.gcloud.qq.com:10001";
	LogInfo = "Log :";
	//PluginsManager::GetInstance()->SetBlueprintObj(this);
}
UMessageRoomBlueprint::~UMessageRoomBlueprint()
{
}

bool UMessageRoomBlueprint::Initialize()
{
	if (!UUserWidget::Initialize()) {
		return false;
	}
	//PluginsManager::GetInstance()->SetBlueprintObj(this);
	return true;
}

void UMessageRoomBlueprint::TimerFunc()
{
	PluginsManager::GetInstance()->SetVoicePoll();
}

void UMessageRoomBlueprint::OnInit()
{
	PluginsEventManager::Instance()->RegisterEvent(SDKEventType::GVoiceUploadSucc, EVENT_FUNC(_handleUploadSucc));
	PluginsEventManager::Instance()->RegisterEvent(SDKEventType::GVoiceDealInfoSucc, EVENT_FUNC(_handleDealInfoSucc));
	std::string openID(TCHAR_TO_UTF8(*openID_));
	//std::string URL(TCHAR_TO_UTF8(*defaultURL_));
	PluginsManager::GetInstance()->InitMessageVoice(openID);

	GetWorld()->GetTimerManager().SetTimer(timerHandler_, this, &UMessageRoomBlueprint::TimerFunc, 0.5F, true);
	//PluginsManager::GetInstance()->SetMode(GCloud::GVoice::IGCloudVoiceEngine::kModeMessages);
	//mCallback->messageUploadSuccEvent.AddUObject(this, &UMessageRoomBlueprint::SetFileID);

	//PluginsManager::GetInstance()->mCallback->messageUploadSuccEvent.AddUObject(this, &UMessageRoomBlueprint::SetFileID);
	SetLog("Init!!!");
}


void UMessageRoomBlueprint::OnRecord()
{
	static bool once = true;
	if (once) {
		PluginsManager::GetInstance()->StartRecord();
		RecordBtnLbl = "StopRecord";
		once = false;
		SetLog("StartRecord!!!");
	}
	else {
		PluginsManager::GetInstance()->StopRecord();
		RecordBtnLbl = "StartRecord";
		once = true;
		SetLog("StopRecord!!!");
	}
}

void UMessageRoomBlueprint::OnPlay()
{
	static bool once = true;
	if (once) {
		PluginsManager::GetInstance()->StartPlayVoice("");
		PlayBtnLbl = "StopPlay";
		once = false;
	}
	else {
		PluginsManager::GetInstance()->StopPlayVoice();
		PlayBtnLbl = "StartPlay";
		once = true;
	}
}



void UMessageRoomBlueprint::OnInputOpenID(const FText& InText)
{
	FString msg = "OpenID:";
	msg += InText.ToString();
	openID_ = InText.ToString();
}

void UMessageRoomBlueprint::OnClearLog()
{
	LogInfo = "Log :";
}

void UMessageRoomBlueprint::SetFileID(std::string fileID)
{
	_VoicefileID = fileID;
}

void UMessageRoomBlueprint::SetLog(FString log)
{
	LogInfo += log + "\n";
}


void UMessageRoomBlueprint::_handleUploadSucc(SDKEventParam& param)
{
	//UE_LOG(LogTemp, Warning, TEXT(" _handleUploadSucc !!!"));
	string msg = "Translate Successful , fileid : " + param.str_0 + " filepath: " + param.str_1;
	SetLog(msg.c_str());
}

void UMessageRoomBlueprint::_handleDealInfoSucc(SDKEventParam& param)
{
	if (param.str_1.empty())
	{
		UE_LOG(LogTemp, Warning, TEXT(" _handleTranslate file id is null !!!!"));
		return;
	}
	if (param.str_1.empty())
	{
		SetLog("translate info is null ");
	}
	string msg = "Translate Successful , fileid : " + param.str_0 + " translate : " + param.str_1 + " bytes : " + to_string(param.int_0) + " time :  " + to_string(param.float_0);
	SetLog(msg.c_str());
}

// ShowLogInNewLine(msg);



