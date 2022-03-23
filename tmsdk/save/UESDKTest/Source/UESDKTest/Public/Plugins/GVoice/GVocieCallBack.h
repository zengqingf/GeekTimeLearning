// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "IGCloudVoiceEngineNotify.h"
#include <string>

using namespace GCloud::GVoice;
/**
 *
 */
 //DECLARE_DELEGATE(FMessageUploadDelegateSucc)

class UESDKTEST_API GVocieCallBack : public IGCloudVoiceNotify
{
public:
	GVocieCallBack();
	~GVocieCallBack();
public:
	void OnJoinRoom(CompleteCode code, const char* roomName, int memberID) override;
	void OnMemberVoice(const unsigned int* members, int count)override;
	void OnMemberVoice(const char* roomName, unsigned int member, int status)override;
	void OnRoleChanged(CompleteCode code, const char* roomName, int memberID, int role)override;
	void OnStatusUpdate(CompleteCode status, const char* roomName, int memberID)override;
	void OnQuitRoom(CompleteCode code, const char* roomName)override;
	void OnApplyMessageKey(CompleteCode code)override;
	void OnRecording(const unsigned char* audioData, unsigned int dataLength)override;
	void OnUploadFile(CompleteCode code, const char* filePath, const char* fileID)override;
	void OnDownloadFile(CompleteCode code, const char* filePath, const char* fileID)override;
	void OnPlayRecordedFile(CompleteCode code, const char* filePath)override;
	void OnSpeechToText(CompleteCode code, const char* fileID, const char* result)override;
	void OnStreamSpeechToText(CompleteCode code, int error, const char* result, const char* voicePath)override;
	void OnEvent(Event event, const char* info)override;
	void OnMuteSwitchResult(int nState)override;
	void OnReportPlayer(CompleteCode nCode, const char* cszInfo)override;
	void OnSaveRecFileIndex(CompleteCode code, const char* fileid, int fileindex)override;
	void OnRoomMemberInfo(CompleteCode code, const char* roomName, int memid, const char* openID)override;
	void OnSpeechTranslate(CompleteCode nCode, const char* srcText, const char* targetText, const char* targetFileID, int srcFileDuration)override;
	void OnRSTS(CompleteCode nCode, SpeechLanguageType srcLang, SpeechLanguageType targetLang, const char* srcText, const char* targetText, const char* targetFileID, int srcFileDuration)override;
	void OnEnableTranslate(CompleteCode code, const char* roomName, RealTimeTranslateType transType)override;
	void OnRealTimeTranslateText(const char* roomName, unsigned int memberID, const char* sessionID, unsigned int seq, const char* text)override;
	void OnTextToSpeech(CompleteCode nCode, const char* text, SpeechLanguageType lang, const char* fileID)override;
	void OnEnableMagicVoice(CompleteCode code, const char* roomName, const char* magicType, bool enable)override;
	void OnEnableRecvMagicVoice(CompleteCode code, const char* roomName, bool enable) override;
	void OnTextToStreamSpeech(CompleteCode nCode, const char* text, int err) override;
	void OnTextTranslate(CompleteCode nCode, SpeechLanguageType srcLang, const char* srcText, SpeechLanguageType targetLang, const char* targetText)override;
	void OnSTTReport(CompleteCode nCode, const char* transText, const char* openID, const char* fileID) override;
};
