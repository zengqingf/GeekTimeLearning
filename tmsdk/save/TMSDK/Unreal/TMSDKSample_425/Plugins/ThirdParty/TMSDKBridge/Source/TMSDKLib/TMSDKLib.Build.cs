// Copyright Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;
using System.IO;

public class TMSDKLib : ModuleRules
{
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
	}
}
