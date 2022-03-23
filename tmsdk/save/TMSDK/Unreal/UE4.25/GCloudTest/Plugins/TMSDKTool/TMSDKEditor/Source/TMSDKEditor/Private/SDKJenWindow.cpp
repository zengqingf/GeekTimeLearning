// Fill out your copyright notice in the Description page of Project Settings.


#include "SDKJenWindow.h"
#include "Widgets/Layout/SScrollBox.h"
#include "Widgets/Layout/SExpandableArea.h"

#include  "Widgets/DeclarativeSyntaxSupport.h"
#include "Widgets/Layout/SScrollBox.h"
#include "Widgets/Input/STextComboBox.h"
#include "Kismet/GameplayStatics.h"
#include "Widgets/Notifications/SNotificationList.h"
#include "SDKInfoSaveConfig.h"
#include <Windows.h>

#include "Framework/Notifications/NotificationManager.h"

#define LOCTEXT_NAMESPACE "SDKJenWindow"

TSharedRef<SDKJenWindow> SDKJenWindow::New()
{
	return MakeShareable(new SDKJenWindow());
}

void SDKJenWindow::Construct(const FArguments& InArgs)
{
	curlpath = FString::Printf(TEXT("%sTMSDKTool/TMSDKEditor/Source/ThirdParty/curl/bin/curl.exe"), *FPaths::ProjectPluginsDir());
	jenkinscliPath = FString::Printf(TEXT("%sTMSDKTool/TMSDKEditor/Source/ThirdParty/tools/jenkins-cli.jar"), *FPaths::ProjectPluginsDir());
	javaPath = FString::Printf(TEXT("%sTMSDKTool/TMSDKEditor/Source/ThirdParty/jre/bin/java.exe"), *FPaths::ProjectPluginsDir());

	InitConfig();

	if (UGameplayStatics::DoesSaveGameExist("SDKInfoSaveConfig", 0))
	{
		sdkInfoSave = Cast<USDKInfoSaveConfig>(UGameplayStatics::LoadGameFromSlot("SDKInfoSaveConfig", 0));
		userPhoneNum = sdkInfoSave->UserPhoneNum;
		userBuildComment = sdkInfoSave->UserBuildComment;
	}
	else
	{
		sdkInfoSave = Cast<USDKInfoSaveConfig>(UGameplayStatics::CreateSaveGameObject(USDKInfoSaveConfig::StaticClass()));
	}
	mCreatePlatformOptions.Add(MakeShareable(new FString(TEXT("android"))));
	mCreatePlatformOptions.Add(MakeShareable(new FString(TEXT("ios"))));
	mCreatePlatformOptions.Add(MakeShareable(new FString(TEXT("windows"))));
	
	_createStartModeOptions.Add(MakeShareable(new FString(TEXT("登录"))));
	_createStartModeOptions.Add(MakeShareable(new FString(TEXT("城镇"))));
	_createStartModeOptions.Add(MakeShareable(new FString(TEXT("PVE"))));
	_createStartModeOptions.Add(MakeShareable(new FString(TEXT("PVP"))));
	_createStartModeOptions.Add(MakeShareable(new FString(TEXT("练习场"))));
	_createStartModeOptions.Add(MakeShareable(new FString(TEXT("NotSet"))));

	_createJobTypeOptions.Add(MakeShareable(new FString(TEXT("NotSet"))));
	_createJobTypeOptions.Add(MakeShareable(new FString(TEXT("狂战"))));
	_createJobTypeOptions.Add(MakeShareable(new FString(TEXT("浪人"))));
	_createJobTypeOptions.Add(MakeShareable(new FString(TEXT("魔剑"))));
	_createJobTypeOptions.Add(MakeShareable(new FString(TEXT("雷烬"))));
	_createJobTypeOptions.Add(MakeShareable(new FString(TEXT("格斗"))));
	_createJobTypeOptions.Add(MakeShareable(new FString(TEXT("气功"))));
	_createJobTypeOptions.Add(MakeShareable(new FString(TEXT("柔道"))));
	_createJobTypeOptions.Add(MakeShareable(new FString(TEXT("魔法师"))));
	_createJobTypeOptions.Add(MakeShareable(new FString(TEXT("召唤师"))));
	_createJobTypeOptions.Add(MakeShareable(new FString(TEXT("战斗法师"))));

	mSelectPlatform = *mCreatePlatformOptions[0];
	selectJobType = *_createJobTypeOptions[4];
	selectStartMode = *_createStartModeOptions[2];
	this->ChildSlot
		[
			SNew(SScrollBox)
			+ SScrollBox::Slot()
			[
				SNew(SVerticalBox)
				+ SVerticalBox::Slot().AutoHeight().Padding(15,15,0,0)
				[
					SNew(SHorizontalBox)
					+SHorizontalBox::Slot().AutoWidth().HAlign(HAlign_Right)
					[
						SNew(STextBlock).Text(FText::FromString(TEXT("钉钉手机号(用于打包结束的钉钉通知，可不填)")))//.ToolTipText(TEXT("用于打包结束后的钉钉通知,可以不填"))
					]
					+ SHorizontalBox::Slot().AutoWidth().Padding(10,0,0,0)
					[
						SNew(SEditableTextBox).MinDesiredWidth(50).
						BackgroundColor(FSlateColor(FLinearColor(0.5f, 0.5f, 0.5f))).Text(FText::FromString(userPhoneNum))//.ToolTipText(L"用于打包结束后的钉钉通知,可以不填")
						.OnTextChanged_Lambda([this](const FText& text)
						{
							userPhoneNum = text.ToString();
							if (text.ToString().Len() == 0 || text.ToString().Len() == 11)
							{
								if (sdkInfoSave)
								{
									sdkInfoSave->UserPhoneNum = text.ToString();
									UGameplayStatics::SaveGameToSlot(sdkInfoSave, "SDKInfoSaveConfig", 0);									
								}
							}
						})
					]
				]
				+ SVerticalBox::Slot().AutoHeight().Padding(15,15,0,0)
				[
					SNew(SHorizontalBox)
					+ SHorizontalBox::Slot().AutoWidth().HAlign(HAlign_Right)
					[
						SNew(STextBlock).Text(FText::FromString(TEXT("打包备注(便于在出包群里找到自己打的包，可不填)")))
					]
					+SHorizontalBox::Slot().AutoWidth().Padding(10,0,0,0)
					[
						SNew(SEditableTextBox).MinDesiredWidth(50)
						.BackgroundColor(FSlateColor(FLinearColor(0.5f, 0.5f, 0.5f))).Text(FText::FromString(userBuildComment))
						.OnTextChanged_Lambda([this](const FText& text)
						{
							userBuildComment = text.ToString().Left(24);
							if (sdkInfoSave)
							{
								sdkInfoSave->UserBuildComment = userBuildComment;
								bool succ = UGameplayStatics::SaveGameToSlot(sdkInfoSave, "SDKInfoSaveConfig", 0);
							}
						})
					]
				]
				+ SVerticalBox::Slot().AutoHeight().Padding(0,15,0,0)
				[					
					JenkinsBuildInfo(EJenkinsPlatform::NONE).ToSharedRef()
				]
			]
		];
}

TSharedPtr<SWidget> SDKJenWindow::JenkinsBuildInfo(EJenkinsPlatform platform)
{
	auto view = SNew(SExpandableArea)
		.AreaTitle(FText::FromString(TEXT("a8打包")))
		.InitiallyCollapsed(false).BodyContent()
		[
			SNew(SBorder).Padding(FMargin(10, 10, 10, 10))
			[
				SNew(SVerticalBox)
				+SVerticalBox::Slot().HAlign(HAlign_Left).AutoHeight()
				[
					SNew(SHorizontalBox)
					+SHorizontalBox::Slot().AutoWidth().FillWidth(1)
					[
						SNew(STextBlock).Text(FText::FromString(TEXT("打包平台")))
					]
					+SHorizontalBox::Slot().AutoWidth()
					[
						SNew(STextComboBox).OptionsSource(&mCreatePlatformOptions)
						.OnSelectionChanged_Lambda([=](TSharedPtr<FString> Selection, ESelectInfo::Type SelectInfo) 
						{
							mSelectPlatform = *Selection;
							EJenkinsPlatform suildPlatform = EJenkinsPlatform::ANDROID;
							if (mSelectPlatform == "windows")
							{
								mBuildPlatform = EJenkinsPlatform::WINDOWS;
							}
							else if (mSelectPlatform == "ios")
							{
								mBuildPlatform = EJenkinsPlatform::IOS;
							}
							else
							{
								mBuildPlatform = EJenkinsPlatform::ANDROID;
							}
							UE_LOG(LogTemp,Warning,TEXT(":::%s"),*mSelectPlatform);
						})
						.InitiallySelectedItem(mCreatePlatformOptions[0])
					]
				]
				+ SVerticalBox::Slot().HAlign(HAlign_Left).AutoHeight().Padding(0,15,0,0)
				[
					SNew(SHorizontalBox)
					+SHorizontalBox::Slot().AutoWidth().FillWidth(1)
					[
						SNew(STextBlock).Text(FText::FromString(TEXT("启动模式")))
					]
					+SHorizontalBox::Slot().AutoWidth().FillWidth(1)
					[
						SNew(STextComboBox).OptionsSource(&_createStartModeOptions)
						.OnSelectionChanged_Lambda([=](TSharedPtr<FString> Selection, ESelectInfo::Type SelectInfo) 
						{
							selectStartMode = *Selection;
							UE_LOG(LogTemp,Warning,TEXT(":::%s"),*selectStartMode);
						})
						.InitiallySelectedItem(_createStartModeOptions[2])
					]
					+ SHorizontalBox::Slot().AutoWidth().FillWidth(1).Padding(15,0,0,0)
					[
						SNew(STextBlock).Text(FText::FromString(TEXT("选择职业")))
					]
					+ SHorizontalBox::Slot().AutoWidth().FillWidth(1)
					[
						SNew(STextComboBox).OptionsSource(&_createJobTypeOptions)
						.OnSelectionChanged_Lambda([=](TSharedPtr<FString> Selection, ESelectInfo::Type SelectInfo) 
						{
							selectJobType = *Selection;
							UE_LOG(LogTemp, Warning, TEXT(":::%s"), *selectJobType);
						})
						.InitiallySelectedItem(_createJobTypeOptions[4])
					]
					//SAssignNew(box, SHorizontalBox)
					//+ SHorizontalBox::Slot().AutoWidth().FillWidth(1).Padding(15,0,0,0)[SNew(STextBlock).Text(FText::FromString(TEXT("是否单机模式"))).MinDesiredWidth(1)]
					//+ SHorizontalBox::Slot().FillWidth(1)[SNew(SCheckBox).IsChecked(0).OnCheckStateChanged_Lambda([](){})]
				]
				+SVerticalBox::Slot().Padding(0,15,0,0)
				[
					SNew(SHorizontalBox)
					+ SHorizontalBox::Slot().AutoWidth()
					[
						SNew(STextBlock).Text(FText::FromString(UTF8_TO_TCHAR("是否单机模式")))
					]+ SHorizontalBox::Slot().Padding(15,0,0,0)
					[
						SNew(SCheckBox).IsChecked(0).OnCheckStateChanged_Lambda([this](ECheckBoxState state)
						{
							mIsStandMode = state == ECheckBoxState::Checked;
						})
					]
				]
				+ SVerticalBox::Slot().HAlign(HAlign_Center).AutoHeight()
				[
					SNew(SBox).WidthOverride(120).HeightOverride(25)
					[
						//SNew(SButton).Text(FText::FromString(FString::Printf(TEXT("执行%s打包"), *buildPlatforms[(int)_platform])))
						SNew(SButton).Text(FText::FromString(TEXT("执行打包")))
						.OnClicked_Lambda([=]()
						{
								FString isStandMode = mIsStandMode?TEXT("true"):TEXT("false");
								FString cmd = FString::Printf(TEXT("%s -jar %s -s %s build %s -p platform=%s -p startMode=%s -p jobType=%s -p standaloneGame=%s"),
									*javaPath,
									*jenkinscliPath,
									*jenkinsip,
									*FString::Printf(TEXT("%s/%s"), *buildJobRoot, *buildJobNames[(int)mBuildPlatform]),
									*mSelectPlatform,
									*selectStartMode,
									*selectJobType,
									*isStandMode);
								if (userPhoneNum.Len() == 11)
								{
									cmd = FString::Printf(TEXT("%s -p userBulidPhoneNum=%s"), *cmd, *userPhoneNum);
								}
								if (userBuildComment.Len() > 0)
								{
									cmd = FString::Printf(TEXT("%s -p userBuildComment=%s"), *cmd, *userBuildComment);
								}
								UE_LOG(LogTemp, Log, TEXT("%s"), *cmd);
								ExecuteProcess(cmd);
								FPlatformProcess::Sleep(2.0f);
								SpawnJenkinsBuildSNotification();
								return FReply::Handled();

						}).VAlign(VAlign_Center).HAlign(HAlign_Center)	
					]
				]
			]
		];
	return view;
}

void SDKJenWindow::SpawnJenkinsBuildSNotification()
{
	FText NotificationMessage = LOCTEXT("JumpToWeb","构建成功,是否跳转到网页查看");
	FNotificationInfo Info(NotificationMessage);
	Info.ExpireDuration = 20.0f;
	Info.ButtonDetails.Add(FNotificationButtonInfo(
	LOCTEXT("Reopen_Confirm", "跳转"),
	FText(),
	FSimpleDelegate::CreateLambda([this]()
	{
		TSharedPtr<SNotificationItem> JenkinsBuildNotificationPtr = mJenkinsBuildNotificationPtr.Pin();
		if (JenkinsBuildNotificationPtr.IsValid())
		{
			JenkinsBuildNotificationPtr->SetExpireDuration(0.0f);
			JenkinsBuildNotificationPtr->SetFadeOutDuration(0.5f);
			JenkinsBuildNotificationPtr->ExpireAndFadeout();
		}
		FString job = FString::Printf(TEXT("%s/%s") , *buildJobRoot, *buildJobNames[(int)mBuildPlatform]);
		FString http = job.Replace(TEXT("/"),TEXT("/job/"));
		FString url = FString::Printf(TEXT("explorer %s/job/%s"),*jenkinsip, *http);
		UE_LOG(LogTemp,Warning,TEXT("url %s"),*url);
		ExecuteProcess(url);
	}),
	SNotificationItem::CS_None
	));
	Info.ButtonDetails.Add(FNotificationButtonInfo(
		LOCTEXT("Reopen_Cancel", "取消"),
		FText(),
		FSimpleDelegate::CreateLambda([this]()
		{
			TSharedPtr<SNotificationItem> JenkinsBuildNotificationPtr = mJenkinsBuildNotificationPtr.Pin();
			if (JenkinsBuildNotificationPtr.IsValid())
			{
				JenkinsBuildNotificationPtr->SetExpireDuration(0.0f);
				JenkinsBuildNotificationPtr->SetFadeOutDuration(0.5f);
				JenkinsBuildNotificationPtr->ExpireAndFadeout();
			}
		}),
		SNotificationItem::CS_None
	));
	mJenkinsBuildNotificationPtr = FSlateNotificationManager::Get().AddNotification(Info);
}

void SDKJenWindow::InitConfig()
{
	GConfig->GetString(TEXT("Jenkins.BuildConfig"), TEXT("JenkinsUrl"), jenkinsip,FPaths::ProjectConfigDir()/"DefaultJenkinsConfig.ini");
	GConfig->GetString(TEXT("Jenkins.BuildConfig"), TEXT("BuildJobRoot"), buildJobRoot, FPaths::ProjectConfigDir() / "DefaultJenkinsConfig.ini");
	GConfig->GetString(TEXT("Jenkins.BuildConfig"), TEXT("AndroidBuildJob"), androidBuildJobName, FPaths::ProjectConfigDir()/"DefaultJenkinsConfig.ini");
	GConfig->GetString(TEXT("Jenkins.BuildConfig"), TEXT("iOSBuildJob"), iOSBuildJobName, FPaths::ProjectConfigDir()/"DefaultJenkinsConfig.ini");
	GConfig->GetString(TEXT("Jenkins.BuildConfig"), TEXT("WindowsBuildJob"), windowsBuildJobName, FPaths::ProjectConfigDir()/"DefaultJenkinsConfig.ini");

	buildJobNames.Add(androidBuildJobName);
	buildJobNames.Add(iOSBuildJobName);
	buildJobNames.Add(windowsBuildJobName);

	buildPlatforms.Add(TEXT("android"));
	buildPlatforms.Add(TEXT("ios"));
	buildPlatforms.Add(TEXT("windows"));
}

void SDKJenWindow::ExecuteProcess(const FString& cmd)
{
#if PLATFORM_DESKTOP
	STARTUPINFO si; //参数设置  发
	memset(&si, 0, sizeof(STARTUPINFO));
	si.cb = sizeof(STARTUPINFO);
	si.dwFlags = STARTF_USESHOWWINDOW;
	si.wShowWindow = SW_SHOW;
	PROCESS_INFORMATION pi;
	CreateProcess(NULL, const_cast<LPWSTR>(*cmd), NULL, NULL, false, 0, NULL, NULL, &si, &pi);
#endif
}

#undef LOCTEXT_NAMESPACE

