// Fill out your copyright notice in the Description page of Project Settings.


#include "Plugins/GVoice/GVocieCallBack.h"
#include "Plugins/GVoice/GVoiceManager.h"
#include "Plugins/TMSDKDefine.h"

using namespace TenmoveSDK;
GVocieCallBack::GVocieCallBack()
{
}

GVocieCallBack::~GVocieCallBack()
{
}

void GVocieCallBack::OnJoinRoom(GCloudVoiceCompleteCode code, const char* roomName, int memberID)
{
	UE_LOG(LogTemp, Warning, TEXT("OnJoinRoom res %s roomName: %s memberID: %s"), *FString::FromInt(code), UTF8_TO_TCHAR(roomName), *FString::FromInt(memberID));
	if (code == GCloudVoiceCompleteCode::GV_ON_JOINROOM_SUCC)
	{
		SDKEventParam param;
		param.str_0 = roomName;
		param.int_0 = memberID;
		PluginManager::Instance().EventManager().TriggerEvent(SDKEventType::GVoiceJoinRoomSucc, param);
	}
	else
	{
		UE_LOG(LogTemp, Warning, TEXT(" OnJoinRoom Failed!!!"));
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
	PluginManager::Instance().EventManager().TriggerEvent(SDKEventType::GVoiceMemberSlateChanger, param);
}
void GVocieCallBack::OnRoleChanged(GCloudVoiceCompleteCode code, const char* roomName, int memberID, int role)
{}
void GVocieCallBack::OnStatusUpdate(GCloudVoiceCompleteCode status, const char* roomName, int memberID)
{}
void GVocieCallBack::OnQuitRoom(GCloudVoiceCompleteCode code, const char* roomName)
{
	UE_LOG(LogTemp, Warning, TEXT(" OnQuitRoom res %s roomName: %s"), *FString::FromInt(code), UTF8_TO_TCHAR(roomName));
	SDKEventParam param;
	param.str_0 = roomName;
	PluginManager::Instance().EventManager().TriggerEvent(SDKEventType::GVoiceQuitRoomSucc, param);
}

//Gvoice
void GVocieCallBack::OnApplyMessageKey(GCloudVoiceCompleteCode code)
{
	UE_LOG(LogTemp, Warning, TEXT(" OnApplyMessageKey Code %s"), *FString::FromInt(code));
}
void GVocieCallBack::OnRecording(const unsigned char* audioData, unsigned int dataLength)
{}

void GVocieCallBack::OnUploadFile(GCloudVoiceCompleteCode code, const char* filePath, const char* fileID)
{
	UE_LOG(LogTemp, Warning, TEXT("OnUploadFile res %s filePath: %s fileID : %s "), *FString::FromInt(code), UTF8_TO_TCHAR(filePath), UTF8_TO_TCHAR(fileID));
	std::string path = filePath;
	std::string id = fileID;
	SDKEventParam param = {fileID,filePath };
	PluginManager::Instance().EventManager().TriggerEvent(SDKEventType::GVoiceUploadSucc, param);
	//SDKEventParam param1 = { fileID };
	//PluginsEventManager::Instance()->TriggerEvent(SDKEventType::GVoiceNoTranslateSucc, param);
}

void GVocieCallBack::OnDownloadFile(GCloudVoiceCompleteCode code, const char* filePath, const char* fileID)
{
	UE_LOG(LogTemp, Warning, TEXT("OnDownloadFile res %s filePath: %s fileID : %s "), *FString::FromInt(code), UTF8_TO_TCHAR(filePath), UTF8_TO_TCHAR(fileID));
	//GVoiceManager::GetInstance()->gVoiceInterface->CacheVoiceMap.insert(std::pair<std::string, std::string>(fileID, filePath));
	SDKEventParam param = { fileID,filePath };
	PluginManager::Instance().EventManager().TriggerEvent(SDKEventType::GVoiceDownloadSucc, param);
}
void GVocieCallBack::OnPlayRecordedFile(GCloudVoiceCompleteCode code, const char* filePath)
{
	UE_LOG(LogTemp, Warning, TEXT("OnPlayRecordedFile res %s filePath: %s"), *FString::FromInt(code), UTF8_TO_TCHAR(filePath));
}
void GVocieCallBack::OnSpeechToText(GCloudVoiceCompleteCode code, const char* fileID, const char* result)
{	

	UE_LOG(LogTemp, Warning, TEXT("OnSpeechToText res %s fileID: %s result : %s "), *FString::FromInt(code), UTF8_TO_TCHAR(fileID), UTF8_TO_TCHAR(result));
	if (code == GCloudVoiceCompleteCode::GV_ON_STT_SUCC)
	{
		SDKEventParam param = { fileID,result };
		PluginManager::Instance().EventManager().TriggerEvent(SDKEventType::GVoiceTranslateSucc, param);
	}
}
void GVocieCallBack::OnStreamSpeechToText(GCloudVoiceCompleteCode code, int error, const char* result, const char* voicePath)
{
	//MultiByteToWideChar();
	//FString res(UTF8_TO_TCHAR(result));
	UE_LOG(LogTemp, Warning, TEXT("OnStreamSpeechToText res %s issucc %d filePath: %s result : %s "), *FString::FromInt(code), error, UTF8_TO_TCHAR(voicePath), UTF8_TO_TCHAR(result));
	if (code == GCloudVoiceCompleteCode::GV_ON_RSTT_SUCC)
	{
		SDKEventParam param;		
		param.str_0 = result;
		param.int_0 = error;
		PluginManager::Instance().EventManager().TriggerEvent(SDKEventType::GVoiceRealTimeTranslateSucc, param);
	}
}
void GVocieCallBack::OnEvent(GCloudVoiceEvent event, const char* info)
{}
void GVocieCallBack::OnMuteSwitchResult(int nState)
{}
void GVocieCallBack::OnReportPlayer(GCloudVoiceCompleteCode nCode, const char* cszInfo)
{}
void GVocieCallBack::OnSaveRecFileIndex(GCloudVoiceCompleteCode code, const char* fileid, int fileindex)
{}
void GVocieCallBack::OnRoomMemberInfo(GCloudVoiceCompleteCode code, const char* roomName, int memid, const char* openID)
{}
void GVocieCallBack::OnSpeechTranslate(GCloudVoiceCompleteCode nCode, const char* srcText, const char* targetText, const char* targetFileID, int srcFileDuration)
{}
void GVocieCallBack::OnRSTS(GCloudVoiceCompleteCode nCode, SpeechLanguageType srcLang, SpeechLanguageType targetLang, const char* srcText, const char* targetText, const char* targetFileID, int srcFileDuration)
{}
void GVocieCallBack::OnEnableTranslate(GCloudVoiceCompleteCode code, const char* roomName, RealTimeTranslateType transType)
{}
void GVocieCallBack::OnRealTimeTranslateText(const char* roomName, unsigned int memberID, const char* sessionID, unsigned int seq, const char* text)
{}
void GVocieCallBack::OnTextToSpeech(GCloudVoiceCompleteCode nCode, const char* text, SpeechLanguageType lang, const char* fileID)
{}
void GVocieCallBack::OnEnableMagicVoice(GCloudVoiceCompleteCode code, const char* magicType, bool enable)
{}

void GVocieCallBack::OnEnableRecvMagicVoice(GCloudVoiceCompleteCode code, bool enable)
{
}

void GVocieCallBack::OnTextToStreamSpeech(GCloudVoiceCompleteCode nCode, const char* text, int err)
{
}

void GVocieCallBack::OnTextTranslate(GCloudVoiceCompleteCode nCode, SpeechLanguageType srcLang, const char* srcText, SpeechLanguageType targetLang, const char* targetText)
{
}

void GVocieCallBack::OnSTTReport(GCloudVoiceCompleteCode nCode, const char* transText, const char* openID, const char* fileID)
{
}
