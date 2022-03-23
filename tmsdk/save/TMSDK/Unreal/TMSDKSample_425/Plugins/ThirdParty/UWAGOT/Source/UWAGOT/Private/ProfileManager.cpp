// Fill out your copyright notice in the Description page of Project Settings.

#include "ProfileManager.h"
#include "Runtime/Engine/Classes/Engine/Engine.h"
#include "Runtime/ImageWrapper/Public/IImageWrapper.h"
#include "Runtime/ImageWrapper/Public/IImageWrapperModule.h"
#include "Runtime/Core/Public/Misc/Paths.h"
#include "Runtime/Core/Public/HAL/FileManager.h"
#include "Runtime/Core/Public/HAL/IConsoleManager.h"
#include "Runtime/Engine/Public/Engine.h"
#include "UWAStats.h"
#include "PlatformManager.h"
#include "AssetStats.h"
#include "ThirdParty/UWALib/Public/uwa.h"
#include "LuaManager.h"

DEFINE_STAT(STAT_UWA_SDK);
DEFINE_STAT(STAT_UWA_ASSET);
DEFINE_STAT(STAT_UWA_SCREENSHOT);

UProfileManager::UProfileManager() :
	ProfileMode(EProfileMode::PM_Overview),
	ProfileState(EProfileState::PS_Ready),
	FrameIndex(0),
	PreLevel(nullptr)
{
	RegisterCmd();
}

UUserWidget* UProfileManager::Widget = nullptr;
void UProfileManager::CmdExec(const TArray<FString>& Cmd)
{
	if (Cmd.Num() == 1)
	{
		PRINT_SCREEN(10, TEXT("UWA Cmd:") + Cmd[0]);
		UProfileManager::Get()->ConsoleExec(*Cmd[0]);
	}
}
static bool flag = false;
void UProfileManager::Tick(float DeltaTime)
{
	if (flag)
	{
		TArray<APlayerController*> PlayerList;
		GEngine->GetAllLocalPlayerControllers(PlayerList);
		ULevel* pl = nullptr;
		if(PlayerList.Num()>0)
			pl = PlayerList[0]->GetWorld()->GetCurrentLevel();
		if (pl && pl != PreLevel)
		{
			PreLevel = pl;
			Levels.Add(FString::Printf(TEXT("%d,%s:%s"), FrameIndex,*pl->GetWorld()->GetName(), *pl->GetName()));
		}
	}

#if !WITH_EDITOR
	CreateMainWidget();
#endif

	if (ProfileState == EProfileState::PS_Start)
	{
		SCOPE_CYCLE_COUNTER(STAT_UWA_SDK);

		if (ProfileMode == EProfileMode::PM_Asset)
		{
			SCOPE_CYCLE_COUNTER(STAT_UWA_ASSET);
			FAssetStats::Get().TakeSample(FrameIndex);
		}
		else if (ProfileMode == EProfileMode::PM_Overview)
		{
			FPlatformManager::Get().NativeHardwareInfo(FrameIndex);
		}
		else if (ProfileMode == EProfileMode::PM_Lua)
		{
			LuaManager::UpdateLuaData(FrameIndex);
		}

		{
			SCOPE_CYCLE_COUNTER(STAT_UWA_SCREENSHOT);
			if (!FPlatformManager::Get().NativeScreenShot(FrameIndex))
			{
				ScreenCapture::Capture(FrameIndex);
			}
		}

		FrameIndex++;
	}
}

bool UProfileManager::IsTickable() const
{
	return !IsTemplate();
}

TStatId UProfileManager::GetStatId() const
{
	return TStatId();
}

bool UProfileManager::ConsoleExec(const FString CmdStr)
{
	if (CmdStr == "ui")
	{
#if WITH_EDITOR
		CreateMainWidget();
#else
		ReCreateMainWidget();
#endif
	}
	else if (CmdStr == "Overview")
	{
		SetMode(EProfileMode::PM_Overview);
	}
	else if (CmdStr == "Asset")
	{
		SetMode(EProfileMode::PM_Asset);
	}
	else if (CmdStr == "Lua")
	{
		SetMode(EProfileMode::PM_Lua);
	}
	else if (CmdStr == "Start")
	{
		Start();
	}
	else if (CmdStr == "Stop")
	{
		Stop();
	}
	return true;
}

void UProfileManager::Initialize()
{
	FPlatformManager::Get().Initialize();
}

void UProfileManager::CreateMainWidget()
{
#if !WITH_EDITOR
	// crash in editor, when switch "Play" frequently
	if (Widget != nullptr)
	{
		if (!Widget->IsInViewport())
		{
			Widget->AddToViewport(10);
		}
	}
	else
#endif
	{
		TSubclassOf<UUserWidget> MainUIWidgetClass = LoadClass<UUserWidget>(nullptr, TEXT("WidgetBlueprint'/UWAGOT/UWAGOTMainUI.UWAGOTMainUI_C'"));
		if (MainUIWidgetClass != nullptr && GEngine != nullptr)
		{
			TArray<APlayerController*> PlayerList;
			GEngine->GetAllLocalPlayerControllers(PlayerList);
			if (PlayerList.Num() > 0)
			{
				Widget = CreateWidget<UUserWidget>(PlayerList[0], MainUIWidgetClass);
#if !WITH_EDITOR
				Widget->AddToRoot();
#endif
				if (Widget != nullptr)
				{
					Widget->AddToViewport(10);
				}
			}
		}
	}
}

void UProfileManager::ReCreateMainWidget()
{
	//if (ProfileState == EProfileState::PS_Stop)
	//{
	//	ProfileState = EProfileState::PS_Ready;
	//	Widget->RemoveFromRoot();
	//	Widget->RemoveFromParent();
	//	Widget = nullptr;
	//}
}
void UProfileManager::SetMode(EProfileMode Mode)
{
	if (ProfileState == EProfileState::PS_Ready)
	{
		ProfileMode = Mode;
		PRINT_SCREEN(15, GetModeName());
	}
	else
	{
		PRINT_SCREEN(15, TEXT("Please Restart App"));
		return;
	}

	ProfileState = EProfileState::PS_SelectMode;
	FPlatformManager::Get().SetMode();
}

bool UProfileManager::SupportMode(EProfileMode Mode)
{
	if (Mode == EProfileMode::PM_Lua)
	{
#if UWA_SLUA_PROFILE
		return true;
#else
		return false;
#endif
	}
	return true;
}

FString UProfileManager::GetModeName() const
{
	FString ModeName = TEXT("Unknown");
	switch (ProfileMode)
	{
	case EProfileMode::PM_Overview:
		ModeName = TEXT("Overview");
		break;
	case EProfileMode::PM_Asset:
		ModeName = TEXT("Assets");
		break;
	case EProfileMode::PM_Lua:
		ModeName = TEXT("Lua");
		break;
	default:
		break;
	}

	UE_LOG(LogTemp, Log, TEXT("UProfileManager::GetModeName  ------- %s"), *ModeName);

	return ModeName;
}

void UProfileManager::Start()
{
	flag = true;
	if (ProfileState == EProfileState::PS_SelectMode)
	{
		PRINT_SCREEN(15, TEXT("UWA Start"));
	}
	else
	{
		PRINT_SCREEN(15, TEXT("Please Select Test Mode [Overview|Asset]"));
		return;
	}

	FPlatformManager::Get().Start();

	if (ProfileMode == EProfileMode::PM_Overview)
	{
#if !WITH_EDITOR
		UWAStatCmd("Start");
#endif
	}
	else if (ProfileMode == EProfileMode::PM_Asset)
	{
		FAssetStats::Get().Initialize();
	}
	else if (ProfileMode == EProfileMode::PM_Lua)
	{
		LuaManager::InitLuaEnvDump();
	}

	ProfileState = EProfileState::PS_Start;
}

void UProfileManager::Stop()
{
	if (ProfileState == EProfileState::PS_Start)
	{
		PRINT_SCREEN(15, TEXT("UWA Stop"));
	}
	else
	{
		PRINT_SCREEN(15, TEXT("Please Start Profile"));
		return;
	}

	ProfileState = EProfileState::PS_Stop;
	if (ProfileMode == EProfileMode::PM_Overview)
	{
#if !WITH_EDITOR
		UWAStatCmd("Stop");
#endif
	}
	else if (ProfileMode == EProfileMode::PM_Lua)
	{
		LuaManager::EndLuaTest();
	}

	FFileHelper::SaveStringArrayToFile(Levels, *(FPlatformManager::Get().GetCurrentDataDirectory()+TEXT("/scene")));
	flag = false;
	FPlatformManager::Get().Stop();
}

void UProfileManager::RegisterCmd()
{
	if (IConsoleManager::Get().FindConsoleObject(TEXT("uwa")) == nullptr)
	{
		IConsoleManager::Get().RegisterConsoleCommand(
			TEXT("uwa"),
			TEXT("uwa cmd"),
			FConsoleCommandWithArgsDelegate::CreateStatic(CmdExec),
			ECVF_Default
		);
	}
}

FDelegateHandle ScreenCapture::CaptureHandle;
int32 ScreenCapture::FrameIndex = 0;
void ScreenCapture::Capture(int32 InFrameIndex)
{
	if (InFrameIndex % SCREEN_SHOT_INVERVAL != 0)
		return;

	if (GEngine && GEngine->GameViewport)
	{
		FrameIndex = InFrameIndex;
		CaptureHandle = UGameViewportClient::OnScreenshotCaptured().AddStatic(&OnCaptureScreenshotComplete);
		GEngine->GameViewport->Exec(nullptr, TEXT("HighResShot 200x120"), *GLog);
	}
}

void ScreenCapture::OnCaptureScreenshotComplete(int32 InWidth, int32 InHeight, const TArray<FColor>& InColors)
{
	FString CurrentDataDirectory = FPlatformManager::Get().GetCurrentDataDirectory();
	FString ScreenShotName = CurrentDataDirectory + FString::Printf(TEXT("%d.jpg"), FrameIndex);
	IModuleInterface& Module = FModuleManager::Get().LoadModuleChecked(FName("ImageWrapper"));
	IImageWrapperModule& ImageWrapperModule = (IImageWrapperModule&)(Module);
	TSharedPtr<IImageWrapper> ImageWrapper = ImageWrapperModule.CreateImageWrapper(EImageFormat::JPEG);
	if (ImageWrapper->SetRaw(InColors.GetData(), InColors.Num() * sizeof(FColor), InWidth, InHeight, ERGBFormat::BGRA, 8))
	{
		int32 Quality = 100;//0~100
		FFileHelper::SaveArrayToFile(ImageWrapper->GetCompressed(Quality), *ScreenShotName);
	}
	UGameViewportClient::OnScreenshotCaptured().Remove(CaptureHandle);
}
