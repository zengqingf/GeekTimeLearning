// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Blueprint/UserWidget.h"
#include "Plugins/GVoice/GVoiceManager.h"
#include "RealTimeRoomBuleprint.generated.h"

/**
 * 
 */
using namespace TenmoveSDK;

UCLASS()
class UESDKTEST_API URealTimeRoomBuleprint : public UUserWidget
{
	GENERATED_BODY()
public:
	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Transient)
		FString MicBtnLbl;

	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Transient)
		FString SpeakerBtnLbl;

	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Transient)
		FString SaveFileBtnLbl;

	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Transient)
		FString PlayVoiceBtnLbl;
	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Transient)
		FString logInfo;

public:
	URealTimeRoomBuleprint(const FObjectInitializer& ObjectInitializer);

	virtual bool Initialize() override;

public:

	UFUNCTION(BlueprintCallable)
		void OnInit();

	UFUNCTION(BlueprintCallable)
		void OnJoinRoom();

	UFUNCTION(BlueprintCallable)
		void OnQuitRoom();
	UFUNCTION(BlueprintCallable)
		void OnQuitAllRoom();

	UFUNCTION(BlueprintCallable)
		void OnMic();

	UFUNCTION(BlueprintCallable)
		void OnSpeaker();

	UFUNCTION(BlueprintCallable)
		void OnInputRoomName(const FText& InText);

	UFUNCTION(BlueprintCallable)
		void OnInputOpenID(const FText& InText);

	UFUNCTION(BlueprintCallable)
		void OnInputRangRoomName(const FText& InText);
	UFUNCTION(BlueprintCallable)
		void OnInputSetOpenIDs(const FText& InText);

	UFUNCTION(BlueprintCallable)
		void OnInputSetMemberIDs(const FText& InText);

	UFUNCTION(BlueprintCallable)
		void OnInputReportOpenIDs(const FText& InText);

	UFUNCTION(BlueprintCallable)
		void OnInputReportMemberIDs(const FText& InText);
	UFUNCTION(BlueprintCallable)
		void OnClearLog();

private:
	void TimerFunc();
	void SetLog(FString log);
	void _handleJoinRoomSucc(SDKEventParam& param);
	void _handleQuitRoomSucc(SDKEventParam& param);
	void _handleMemberSlateChanger(SDKEventParam& param);

private:
	FTimerHandle timerHandler_;
	FString roomName_;
	FString openID_;
	FString rangeName_;
	FString defaultURL_;
	FString sOpenIDs_;
	FString sMemberIDs_;
	FString rOpenIDs_;
	FString rMemberIDs_;
};
