// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Blueprint/UserWidget.h"
#include "Plugins/GVoice/GVocieCallBack.h"

#include "Plugins/PluginsManager.h"
#include <list>



#include "MessageRoomBlueprint.generated.h"

/**
 * 
 */
using namespace TenmoveSDK;
UCLASS()
class UESDKTEST_API UMessageRoomBlueprint : public UUserWidget
{
	GENERATED_BODY()
public:
	UMessageRoomBlueprint(const FObjectInitializer& ObjectInitializer);
	~UMessageRoomBlueprint();

	virtual bool Initialize() override;

public:
	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Transient)
		FString RecordBtnLbl;

	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Transient)
		FString PlayBtnLbl;
	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Transient)
		FString LogInfo;

public:
	UFUNCTION(BlueprintCallable)
		void OnInit();

	UFUNCTION(BlueprintCallable)
		void OnRecord();

	UFUNCTION(BlueprintCallable)
		void OnPlay();
	UFUNCTION(BlueprintCallable)
		void OnInputOpenID(const FText& InText);
	UFUNCTION(BlueprintCallable)
		void OnClearLog();


private:
	void TimerFunc();

	void SetFileID(std::string fileID);
	void SetLog(FString log);
	GVocieCallBack* mCallback;


	void _handleUploadSucc(SDKEventParam& param);
	void _handleDealInfoSucc(SDKEventParam& param);
	void _handleNoTranslateSucc(SDKEventParam& param);

private:
	FString openID_;
	std::string filePath_;
	FString defaultURL_;
	FTimerHandle timerHandler_;
	std::string _VoicefileID;

	
};
