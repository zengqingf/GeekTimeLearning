// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Widgets/SUserWidget.h"

enum  EJenkinsPlatform
{
	ANDROID = 0,
	IOS = 1,
	WINDOWS = 2,
	NONE = 3,
};

/**
 * 
 */
class TMSDKEDITOR_API SDKJenWindow : public SUserWidget
{
public:
	SLATE_USER_ARGS(SDKJenWindow) {}
	SLATE_END_ARGS()

	void Construct(const FArguments& InArgs);

	TSharedPtr<SWidget> JenkinsBuildInfo(EJenkinsPlatform platform);
private:
	void SpawnJenkinsBuildSNotification();

protected:
	class USDKInfoSaveConfig* sdkInfoSave;
	FString curlpath;
	FString jenkinscliPath;
	FString javaPath;
	FString jenkinsip;
	FString buildJobRoot;
	FString windowsBuildJobName;
	FString androidBuildJobName;
	FString iOSBuildJobName;
	bool mIsStandMode = false;
	EJenkinsPlatform mBuildPlatform = EJenkinsPlatform::ANDROID;

	TSharedPtr<SSearchBox> SearchBoxPtr = nullptr;
	TArray<TSharedPtr<FString>> _createOptions;

	FString mSelectPlatform;
	FString selectJobType;
	FString selectStartMode;
	TArray<TSharedPtr<FString>> mCreatePlatformOptions;
	TArray<TSharedPtr<FString>> _createStartModeOptions;
	TArray<TSharedPtr<FString>> _createJobTypeOptions;

	TArray<FString> buildJobNames;
	TArray<FString> buildPlatforms;

private:
	void InitConfig();
	void ExecuteProcess(const FString& cmd);
	FString userPhoneNum;
	FString userBuildComment;		//打包备注
	TWeakPtr<SNotificationItem> mJenkinsBuildNotificationPtr;
};
