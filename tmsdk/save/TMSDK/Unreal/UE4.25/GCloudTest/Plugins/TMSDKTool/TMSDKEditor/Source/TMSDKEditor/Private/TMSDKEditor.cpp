// Copyright Epic Games, Inc. All Rights Reserved.

#include "TMSDKEditor.h"

#include "BlueprintEditorModule.h"
#include "TMSDKEditorStyle.h"
#include "TMSDKEditorCommands.h"
#include "LevelEditor.h"
#include "Widgets/Docking/SDockTab.h"
#include "Widgets/Layout/SBox.h"
#include "Widgets/Text/STextBlock.h"
#include "ToolMenus.h"
#include "SDKJenWindow.h"
#include "Blueprint/UserWidget.h"
#if WITH_EDITOR
#include "Windows/WindowsPlatformApplicationMisc.h"
#endif

#include "CreateLuaViewEditor.h"

class CreateLuaViewEditor;
static const FName TMSDKEditorTabName("TMSDKEditor");
static const FName TMSDKToolTabName("TMSDKTool");

#define LOCTEXT_NAMESPACE "FTMSDKEditorModule"

void FTMSDKEditorModule::OnPostEngineInit()
{
	FBlueprintEditorModule &BlueprintEditorModule = FModuleManager::LoadModuleChecked<FBlueprintEditorModule>("Kismet");
	BlueprintEditorModule.GetMenuExtensibilityManager()->GetExtenderDelegates().Add(FAssetEditorExtender::CreateRaw(this, &FTMSDKEditorModule::GetBlueprintToolbarExtender));
}


void FTMSDKEditorModule::StartupModule()
{
	// This code will execute after your module is loaded into memory; the exact timing is specified in the .uplugin file per-module
	OnPostEngineInitHandle = FCoreDelegates::OnPostEngineInit.AddRaw(this, &FTMSDKEditorModule::OnPostEngineInit);
	FTMSDKEditorStyle::Initialize();
	FTMSDKEditorStyle::ReloadTextures();

	FTMSDKEditorCommands::Register();
	
	PluginCommands = MakeShareable(new FUICommandList);

	PluginCommands->MapAction(
		FTMSDKEditorCommands::Get().OpenPluginWindow,
		FExecuteAction::CreateRaw(this, &FTMSDKEditorModule::PluginButtonClicked),
		FCanExecuteAction());

	PluginCommands->MapAction(FTMSDKEditorCommands::Get().TexturePackerToolCmd,
		FExecuteAction::CreateLambda([]()
		{
			FString path = FString::Printf(TEXT("%s/../../ExternalTool/TexturePacker/bin/TexturePackerGUI.exe"),*FPaths::ProjectDir());
			ExecuteProcess(path);
		}),
		FCanExecuteAction());

	UToolMenus::RegisterStartupCallback(FSimpleMulticastDelegate::FDelegate::CreateRaw(this, &FTMSDKEditorModule::RegisterMenus));
	
	FGlobalTabmanager::Get()->RegisterNomadTabSpawner(TMSDKEditorTabName, FOnSpawnTab::CreateRaw(this, &FTMSDKEditorModule::OnSpawnPluginTab))
		.SetDisplayName(LOCTEXT("FTMSDKEditorTabTitle", "Jenkins配置"))
		.SetMenuType(ETabSpawnerMenuType::Hidden);
	
}

void FTMSDKEditorModule::ShutdownModule()
{
	// This function may be called during shutdown to clean up your module.  For modules that support dynamic reloading,
	// we call this function before unloading the module.
	FCoreDelegates::OnPostEngineInit.Remove(OnPostEngineInitHandle);
	UToolMenus::UnRegisterStartupCallback(this);

	UToolMenus::UnregisterOwner(this);

	FTMSDKEditorStyle::Shutdown();

	FTMSDKEditorCommands::Unregister();

	FGlobalTabmanager::Get()->UnregisterNomadTabSpawner(TMSDKEditorTabName);
}

TSharedRef<SDockTab> FTMSDKEditorModule::OnSpawnPluginTab(const FSpawnTabArgs& SpawnTabArgs)
{
	FText WidgetText = FText::Format(
		LOCTEXT("WindowWidgetText", "Add code to {0} in {1} to override this window's contents"),
		FText::FromString(TEXT("FTMSDKEditorModule::OnSpawnPluginTab")),
		FText::FromString(TEXT("TMSDKEditor.cpp"))
		);

	return SNew(SDockTab)
		.TabRole(ETabRole::NomadTab)
		[
			// Put your tab content here!
			SNew(SDKJenWindow)
		];
}

void FTMSDKEditorModule::PluginButtonClicked()
{
	FGlobalTabmanager::Get()->InvokeTab(TMSDKEditorTabName);
}

void FTMSDKEditorModule::RegisterMenus()
{
	// Owner will be used for cleanup in call to UToolMenus::UnregisterOwner
	FToolMenuOwnerScoped OwnerScoped(this);
	FLevelEditorModule& LevelEditorMoudle = FModuleManager::LoadModuleChecked<FLevelEditorModule>("LevelEditor");
	UToolMenu* Menu = UToolMenus::Get()->FindMenu("LevelEditor.MainMenu.TMSDKTool");
	if (Menu == nullptr)
	{
		TSharedPtr<FExtender> MenuBarExtender = MakeShareable(new FExtender());
		MenuBarExtender->AddMenuBarExtension("Help", EExtensionHook::After, PluginCommands, FMenuBarExtensionDelegate::CreateRaw(this, &FTMSDKEditorModule::AddMenuBarExtension));
		LevelEditorMoudle.GetMenuExtensibilityManager()->AddExtender(MenuBarExtender);

		//TSharedPtr<FExtender> MenuExtender = MakeShareable(new FExtender());
		//MenuExtender->AddMenuExtension("TMSDKTool", EExtensionHook::After, PluginCommands, FMenuExtensionDelegate::CreateRaw(this, &FTMSDKEditorModule::AddMenuExtension));
		//LevelEditorMoudle.GetMenuExtensibilityManager()->AddExtender(MenuBarExtender);
	}
	else
	{
		FToolMenuSection& Section = Menu->FindOrAddSection("WindowLayout");
		Section.AddMenuEntryWithCommandList(FTMSDKEditorCommands::Get().OpenPluginWindow, PluginCommands);
	}

	//{
	//	UToolMenu* Menu = UToolMenus::Get()->ExtendMenu("LevelEditor.MainMenu.Window");
	//	{
	//		FToolMenuSection& Section = Menu->FindOrAddSection("WindowLayout");
	//		Section.AddMenuEntryWithCommandList(FTMSDKEditorCommands::Get().OpenPluginWindow, PluginCommands);
	//	}
	//}

	//{
	//	UToolMenu* ToolbarMenu = UToolMenus::Get()->ExtendMenu("LevelEditor.LevelEditorToolBar");
	//	{
	//		FToolMenuSection& Section = ToolbarMenu->FindOrAddSection("Settings");
	//		{
	//			FToolMenuEntry& Entry = Section.AddEntry(FToolMenuEntry::InitToolBarButton(FTMSDKEditorCommands::Get().OpenPluginWindow));
	//			Entry.SetCommandList(PluginCommands);
	//		}
	//	}
	//}
}

void FTMSDKEditorModule::AddMenuBarExtension(FMenuBarBuilder& Builder)
{
	Builder.AddPullDownMenu(
		LOCTEXT("TMSDK", "TMSDK"),
		LOCTEXT("TMSDK", "TMSDK"),
		FNewMenuDelegate::CreateRaw(this, &FTMSDKEditorModule::PullDwonBar),
		"TMSDKTool"
	);
}

void FTMSDKEditorModule::AddMenuExtension(FMenuBuilder& Builder)
{
	Builder.AddMenuEntry(FTMSDKEditorCommands::Get().OpenSDKToolWindow);
}

void FTMSDKEditorModule::PullDwonBar(FMenuBuilder& Builder)
{
	Builder.AddMenuEntry(FTMSDKEditorCommands::Get().OpenPluginWindow);
	Builder.AddMenuEntry(FTMSDKEditorCommands::Get().TexturePackerToolCmd);
	//Builder.AddMenuEntry(FTMSDKEditorCommands::Get().OpenSDKToolWindow);
}

TSharedRef<FExtender> FTMSDKEditorModule::GetBlueprintToolbarExtender(const TSharedRef<FUICommandList> CommandList, const TArray<UObject*> ContextSensitiveObjects)
{
	UBlueprint *Blueprint = ContextSensitiveObjects.Num() < 1 ? nullptr : Cast<UBlueprint>(ContextSensitiveObjects[0]);
	TSharedPtr<class FUICommandList> NewCommandList = MakeShareable(new FUICommandList);
	//NewCommandList->MapAction(FUnLuaEditorCommands::Get().CreateLuaTemplateCmd, FExecuteAction::CreateLambda([Blueprint]() { CreateUserWidgetBaseLuaTemplateFile(Blueprint); }), FCanExecuteAction());
	NewCommandList->MapAction(FTMSDKEditorCommands::Get().CreateLuaFileCmd, FExecuteAction::CreateLambda([Blueprint,this]() { this->OpenCreateLuaTab(Blueprint);}), FCanExecuteAction());
	TSharedRef<FExtender> ToolbarExtender(new FExtender());
	ToolbarExtender->AddToolBarExtension("Debugging", EExtensionHook::After, NewCommandList, FToolBarExtensionDelegate::CreateRaw(this, &FTMSDKEditorModule::AddToolbarExtension));
	return ToolbarExtender;
}

void FTMSDKEditorModule::AddToolbarExtension(FToolBarBuilder& Builder)
{
	Builder.BeginSection(NAME_None);
	Builder.AddToolBarButton(FTMSDKEditorCommands::Get().CreateLuaFileCmd); // 一个可以取到的TSharedPtr<FUICommandInfo>
	Builder.EndSection();
}
void FTMSDKEditorModule::OpenCreateLuaTab(UBlueprint* Blueprint)
{
	UClass* GeneratedClass = Blueprint->GeneratedClass;
	if (GeneratedClass->IsChildOf(UUserWidget::StaticClass()))
	{
		UWorld* World = GEditor->GetEditorWorldContext().World();
		UUserWidget* PreviewUMG = CreateWidget<UUserWidget>(World, GeneratedClass);

		TSharedRef<SWindow> Window =
			SNew(SWindow)
			.Title(LOCTEXT("CreateLuaFile", "创建lua文件"))
			//.ClientSize(FVector2D(1280, 720))
			//.SizingRule(ESizingRule::UserSized)
			.SizingRule(ESizingRule::Autosized)
			.AutoCenter(EAutoCenter::PreferredWorkArea);
		TSharedPtr<CreateLuaViewEditor> ParameterWindow;
		Window->SetContent
		(
		SAssignNew(ParameterWindow, CreateLuaViewEditor)
		.Parameters(Blueprint)
		.WidgetWindow(Window)
		);

		//PreviewWindow->SetContent(PreviewUMG->TakeWidget());
		FSlateApplication::Get().AddWindow(Window);
	}
}

void FTMSDKEditorModule::ExecuteProcess(const FString& cmd)
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
	
IMPLEMENT_MODULE(FTMSDKEditorModule, TMSDKEditor)