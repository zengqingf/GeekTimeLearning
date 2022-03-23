// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include <string>
//#include <GVoice/GCloudVoice.h>
//#include "CoreMinimal.h"
#include "GVoice/GCloudVoiceNotify.h"
using namespace gcloud_voice;

class GCLOUDTEST_API GVocieCallBack : public IGCloudVoiceNotify
{
public:
	GVocieCallBack();
	~GVocieCallBack();
public:
	void OnJoinRoom(GCloudVoiceCompleteCode code, const char* roomName, int memberID) override;
	void OnMemberVoice(const unsigned int* members, int count)override;
	void OnMemberVoice(const char* roomName, unsigned int member, int status)override;
	void OnRoleChanged(GCloudVoiceCompleteCode code, const char* roomName, int memberID, int role)override;
	void OnStatusUpdate(GCloudVoiceCompleteCode status, const char* roomName, int memberID)override;
	void OnQuitRoom(GCloudVoiceCompleteCode code, const char* roomName)override;
	void OnApplyMessageKey(GCloudVoiceCompleteCode code)override;
	void OnRecording(const unsigned char* audioData, unsigned int dataLength)override;
	void OnUploadFile(GCloudVoiceCompleteCode code, const char* filePath, const char* fileID)override;
	void OnDownloadFile(GCloudVoiceCompleteCode code, const char* filePath, const char* fileID)override;
	void OnPlayRecordedFile(GCloudVoiceCompleteCode code, const char* filePath)override;
	void OnSpeechToText(GCloudVoiceCompleteCode code, const char* fileID, const char* result)override;
	void OnStreamSpeechToText(GCloudVoiceCompleteCode code, int error, const char* result, const char* voicePath)override;
	void OnEvent(GCloudVoiceEvent event, const char* info)override;
	void OnMuteSwitchResult(int nState)override;
	void OnReportPlayer(GCloudVoiceCompleteCode nCode, const char* cszInfo)override;
	void OnSaveRecFileIndex(GCloudVoiceCompleteCode code, const char* fileid, int fileindex)override;
	void OnRoomMemberInfo(GCloudVoiceCompleteCode code, const char* roomName, int memid, const char* openID)override;
	void OnSpeechTranslate(GCloudVoiceCompleteCode nCode, const char* srcText, const char* targetText, const char* targetFileID, int srcFileDuration)override;
	void OnRSTS(GCloudVoiceCompleteCode nCode, SpeechLanguageType srcLang, SpeechLanguageType targetLang, const char* srcText, const char* targetText, const char* targetFileID, int srcFileDuration)override;
	void OnEnableTranslate(GCloudVoiceCompleteCode code, const char* roomName, RealTimeTranslateType transType)override;
	void OnRealTimeTranslateText(const char* roomName, unsigned int memberID, const char* sessionID, unsigned int seq, const char* text)override;
	void OnTextToSpeech(GCloudVoiceCompleteCode nCode, const char* text, SpeechLanguageType lang, const char* fileID)override;
	void OnEnableMagicVoice(GCloudVoiceCompleteCode code, const char* magicType, bool enable)override;
	void OnEnableRecvMagicVoice(GCloudVoiceCompleteCode code, bool enable) override;
	void OnTextToStreamSpeech(GCloudVoiceCompleteCode nCode, const char* text, int err) override;
	void OnTextTranslate(GCloudVoiceCompleteCode nCode, SpeechLanguageType srcLang, const char* srcText, SpeechLanguageType targetLang, const char* targetText)override;
	void OnSTTReport(GCloudVoiceCompleteCode nCode, const char* transText, const char* openID, const char* fileID) override;
};
