// Copyright Epic Games, Inc. All Rights Reserved.

#include "TMSDKEditorCommands.h"

#define LOCTEXT_NAMESPACE "FTMSDKEditorModule"

void FTMSDKEditorCommands::RegisterCommands()
{
	UI_COMMAND(OpenPluginWindow, "A8打包", "执行Jenkins打包", EUserInterfaceActionType::Button, FInputGesture());
	UI_COMMAND(TexturePackerToolCmd, "打开图集处理工具", "打开TexturePacker图集处理工具", EUserInterfaceActionType::Button, FInputChord());
	UI_COMMAND(OpenSDKToolWindow, "SDKAutoTestTool", "open SDK Tools", EUserInterfaceActionType::Button, FInputGesture());
	UI_COMMAND(CreateLuaFileCmd, "创建Lua脚本", "创建对应的lua脚本", EUserInterfaceActionType::Button, FInputGesture());
	//UI_COMMAND(OpenSDKToolWindow, "SDKAutoTestTool", "open SDK Tools", EUserInterfaceActionType::Button, FInputGesture());

}

#undef LOCTEXT_NAMESPACE
