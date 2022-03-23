// Fill out your copyright notice in the Description page of Project Settings.


#include "Plugins/GVoice/GVocieCallBack.h"
#include "Plugins/GVoice/GVoiceManager.h"
//#include "GVoiceTest/BluePrints/MessageRoom/MessageRoomBlueprint.h"
#include "Plugins/PluginsManager.h"

DEFINE_LOG_CATEGORY_STATIC(LogGVoice, Log, All);

using namespace TenmoveSDK;
GVocieCallBack::GVocieCallBack()
{
}

GVocieCallBack::~GVocieCallBack()
{
}

void GVocieCallBack::OnJoinRoom(CompleteCode code, const char* roomName, int memberID)
{
	UE_LOG(LogGVoice, Warning, TEXT("OnJoinRoom res %s roomName: %s memberID: %s"), *FString::FromInt(code), *FString(roomName), *FString::FromInt(memberID));
	if (code == CompleteCode::kCompleteCodeJoinRoomSucc)
	{
		SDKEventParam param;
		param.str_0 = roomName;
		param.int_0 = memberID;
		PluginsEventManager::Instance()->TriggerEvent(SDKEventType::GVoiceJoinRoomSucc, param);
	}
	else
	{
		UE_LOG(LogGVoice, Warning, TEXT(" OnJoinRoom Failed!!!"));
	}

}
void GVocieCallBack::OnMemberVoice(const unsigned int* members, int count)
{}
void GVocieCallBack::OnMemberVoice(const char* roomName, unsigned int member, int status)
{
	SDKEventParam param;
	param.str_0 = roomName;
	param.int_0 = member;
	param.int_1 = status;
	PluginsEventManager::Instance()->TriggerEvent(SDKEventType::GVoiceMemberSlateChanger, param);
}
void GVocieCallBack::OnRoleChanged(CompleteCode code, const char* roomName, int memberID, int role)
{}
void GVocieCallBack::OnStatusUpdate(CompleteCode status, const char* roomName, int memberID)
{}
void GVocieCallBack::OnQuitRoom(CompleteCode code, const char* roomName)
{
	SDKEventParam param;
	param.str_0 = roomName;
	PluginsEventManager::Instance()->TriggerEvent(SDKEventType::GVoiceQuitRoomSucc, param);
	UE_LOG(LogGVoice, Warning, TEXT(" OnQuitRoom!!!"));
}

//Gvoice
void GVocieCallBack::OnApplyMessageKey(CompleteCode code)
{
	UE_LOG(LogGVoice, Warning, TEXT(" OnApplyMessageKey Code %s"), *FString::FromInt(code));
}
void GVocieCallBack::OnRecording(const unsigned char* audioData, unsigned int dataLength)
{}

void GVocieCallBack::OnUploadFile(CompleteCode code, const char* filePath, const char* fileID)
{
	UE_LOG(LogGVoice, Warning, TEXT("OnUploadFile res %s filePath: %s fileID : %s "), *FString::FromInt(code), *FString(filePath), *FString(fileID));
	std::string path = filePath;
	std::string id = fileID;
	SDKEventParam param = {fileID,filePath };
	PluginsEventManager::Instance()->TriggerEvent(SDKEventType::GVoiceUploadSucc, param);
	//SDKEventParam param1 = { fileID };
	//PluginsEventManager::Instance()->TriggerEvent(SDKEventType::GVoiceNoTranslateSucc, param);
}

void GVocieCallBack::OnDownloadFile(CompleteCode code, const char* filePath, const char* fileID)
{
	UE_LOG(LogGVoice, Warning, TEXT("OnDownloadFile res %s filePath: %s fileID : %s "), *FString::FromInt(code), *FString(filePath), *FString(fileID));
	//GVoiceManager::GetInstance()->gVoiceInterface->CacheVoiceMap.insert(std::pair<std::string, std::string>(fileID, filePath));
	SDKEventParam param = { fileID,filePath };
	PluginsEventManager::Instance()->TriggerEvent(SDKEventType::GVoiceDownloadSucc, param);
}
void GVocieCallBack::OnPlayRecordedFile(CompleteCode code, const char* filePath)
{
	UE_LOG(LogGVoice, Warning, TEXT("OnPlayRecordedFile res %s filePath: %s"), *FString::FromInt(code), *FString(filePath));
}
void GVocieCallBack::OnSpeechToText(CompleteCode code, const char* fileID, const char* result)
{
	UE_LOG(LogGVoice, Warning, TEXT("OnSpeechToText res %s filePath: %s result : %s "), *FString::FromInt(code), *FString(fileID), *FString(result));
	SDKEventParam param = { fileID,result };
	PluginsEventManager::Instance()->TriggerEvent(SDKEventType::GVoiceTranslateSucc, param);
}
void GVocieCallBack::OnStreamSpeechToText(CompleteCode code, int error, const char* result, const char* voicePath)
{}
void GVocieCallBack::OnEvent(Event event, const char* info)
{}
void GVocieCallBack::OnMuteSwitchResult(int nState)
{}
void GVocieCallBack::OnReportPlayer(CompleteCode nCode, const char* cszInfo)
{}
void GVocieCallBack::OnSaveRecFileIndex(CompleteCode code, const char* fileid, int fileindex)
{}
void GVocieCallBack::OnRoomMemberInfo(CompleteCode code, const char* roomName, int memid, const char* openID)
{}
void GVocieCallBack::OnSpeechTranslate(CompleteCode nCode, const char* srcText, const char* targetText, const char* targetFileID, int srcFileDuration)
{}
void GVocieCallBack::OnRSTS(CompleteCode nCode, SpeechLanguageType srcLang, SpeechLanguageType targetLang, const char* srcText, const char* targetText, const char* targetFileID, int srcFileDuration)
{}
void GVocieCallBack::OnEnableTranslate(CompleteCode code, const char* roomName, RealTimeTranslateType transType)
{}
void GVocieCallBack::OnRealTimeTranslateText(const char* roomName, unsigned int memberID, const char* sessionID, unsigned int seq, const char* text)
{}
void GVocieCallBack::OnTextToSpeech(CompleteCode nCode, const char* text, SpeechLanguageType lang, const char* fileID)
{}
void GVocieCallBack::OnEnableMagicVoice(CompleteCode code, const char* roomName, const char* magicType, bool enable)
{}

void GVocieCallBack::OnEnableRecvMagicVoice(CompleteCode code, const char* roomName, bool enable)
{
}

void GVocieCallBack::OnTextToStreamSpeech(CompleteCode nCode, const char* text, int err)
{
}

void GVocieCallBack::OnTextTranslate(CompleteCode nCode, SpeechLanguageType srcLang, const char* srcText, SpeechLanguageType targetLang, const char* targetText)
{
}

void GVocieCallBack::OnSTTReport(CompleteCode nCode, const char* transText, const char* openID, const char* fileID)
{
}
