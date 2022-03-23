// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "Framework/Commands/Commands.h"
#include "TMSDKEditorStyle.h"

class FTMSDKEditorCommands : public TCommands<FTMSDKEditorCommands>
{
public:

	FTMSDKEditorCommands()
		: TCommands<FTMSDKEditorCommands>(TEXT("TMSDKEditor"), NSLOCTEXT("Contexts", "TMSDKEditor", "TMSDKEditor Plugin"), NAME_None, FTMSDKEditorStyle::GetStyleSetName())
	{
	}

	// TCommands<> interface
	virtual void RegisterCommands() override;

public:
	TSharedPtr< FUICommandInfo > OpenPluginWindow;
	TSharedPtr< FUICommandInfo > OpenSDKToolWindow;
	TSharedPtr< FUICommandInfo > TexturePackerToolCmd;
	TSharedPtr< FUICommandInfo > CreateLuaFileCmd;
};