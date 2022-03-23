// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "Modules/ModuleManager.h"

class FToolBarBuilder;
class FMenuBuilder;

class FTMSDKEditorModule : public IModuleInterface
{
public:

	/** IModuleInterface implementation */
	void OnPostEngineInit();
	virtual void StartupModule() override;
	virtual void ShutdownModule() override;
	
	/** This function will be bound to Command (by default it will bring up plugin window) */
	void PluginButtonClicked();
	void OpenCreateLuaTab(UBlueprint* Blueprint);
	static void  ExecuteProcess(const FString& cmd);

	
private:

	void RegisterMenus();
	void AddMenuBarExtension(FMenuBarBuilder& Builder);
	void AddMenuExtension(FMenuBuilder& Builder);
	void PullDwonBar(FMenuBuilder& Builder);

	TSharedRef<class SDockTab> OnSpawnPluginTab(const class FSpawnTabArgs& SpawnTabArgs);
	TSharedRef<FExtender> GetBlueprintToolbarExtender(const TSharedRef<FUICommandList> CommandList, const TArray<UObject*> ContextSensitiveObjects);
	void AddToolbarExtension(FToolBarBuilder& Builder);
	void CreateUMGPreview(UBlueprint* Blueprint);
	

private:
	TSharedPtr<class FUICommandList> PluginCommands;
	FDelegateHandle OnPostEngineInitHandle;
};
