// Copyright Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;
using System.IO;

public class TMSDKLib : ModuleRules
{
    //这个模块用于平台打包时预拷贝和添加SDK相关的资源、配置、代码
	public TMSDKLib(ReadOnlyTargetRules Target) : base(Target)
	{
		Type = ModuleType.External;

		if (Target.Platform == UnrealTargetPlatform.Android)
		{
			string pluginPath = Utils.MakePathRelativeTo(ModuleDirectory, Target.RelativeEnginePath);
			AdditionalPropertiesForReceipt.Add(new ReceiptProperty("AndroidPlugin", Path.Combine(pluginPath, "TMSDKLib_UPL.xml")));
		}
		else if (Target.Platform == UnrealTargetPlatform.IOS)
		{
			//TODO 
			
			/*
			PublicAdditionalFrameworks.Add(new UEBuildFramework("TMSDKBridge", "iOS/TMSDKBridge.embeddedframework.zip"));

			PublicFrameworks.Add("SystemConfiguration");
			PublicFrameworks.Add("Security");

			PublicAdditionalLibraries.Add("z");
			PublicAdditionalLibraries.Add("c++");
			*/
		}
        else if(Target.Platform == UnrealTargetPlatform.Win64 ||
                Target.Platform == UnrealTargetPlatform.Win32)
        {        
            RuntimeDependencies.Add(PluginDirectory + "/PlatformRes/All/maple/dir_region_info.json");
            RuntimeDependencies.Add(PluginDirectory + "/PlatformRes/All/plugin_functions.json");
        }
	}
}
