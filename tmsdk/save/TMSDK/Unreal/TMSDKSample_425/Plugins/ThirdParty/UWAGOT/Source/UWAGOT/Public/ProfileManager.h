// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "UObject/NoExportTypes.h"
#include "Runtime/Engine/Public/Tickable.h"
#include "Runtime/UMG/Public/Blueprint/UserWidget.h"
#include "UWAEnum.h"
#include "UWAMacros.h"
#include "ProfileManager.generated.h"


/**
 * This class is the main manager for profile.
 */
UCLASS()
class UWAGOT_API UProfileManager : public UObject, public FTickableGameObject
{
	GENERATED_BODY()

public:
	UProfileManager();

	static UProfileManager* Get()
	{
		static UProfileManager* Instance = nullptr;

		if (!Instance)
		{
			Instance = NewObject<UProfileManager>();
			Instance->AddToRoot();
		}

		return Instance;
	}
	
	void Tick(float DeltaTime) override;

	bool IsTickable() const override;
	
	TStatId GetStatId() const override;

	// Handle console command for uwa
	bool ConsoleExec(const FString Cmd);
	
	// initialize profile state, type, et al.
	void Initialize();
	
	// create(or flush) main widget (select mode, start, stop button, et al.)
	void CreateMainWidget();

	// call to Recreate main widget after stop(select mode, start, stop button, et al.)
	void ReCreateMainWidget();
	
	// set mode
	void SetMode(EProfileMode Mode);

	// support mode
	bool SupportMode(EProfileMode Mode);
	
	// get mode
	FString GetModeName() const;
	
	// start profile
	void Start();
	
	// stop profile
	void Stop();

	void RegisterCmd();

	//static void Test();
	static void CmdExec(const TArray<FString>& Cmd);

	UPROPERTY()
	EProfileMode ProfileMode;
	EProfileState ProfileState;
	static UUserWidget* Widget;
	int32 FrameIndex;
	TArray<FString> Levels;
	ULevel* PreLevel;
};


// Screen Capture ---------------------------
// This class is a screen shot implementation using engine method
class ScreenCapture
{
public:
	static void Capture(int32 FrameIndex);
	static void OnCaptureScreenshotComplete(int32 InWidth, int32 InHeight, const TArray<FColor>& InColors);
	static FDelegateHandle CaptureHandle;
	static int32 FrameIndex;
};