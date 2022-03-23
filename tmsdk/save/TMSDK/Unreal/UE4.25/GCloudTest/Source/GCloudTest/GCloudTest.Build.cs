// Copyright Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;

public class GCloudTest : ModuleRules
{
	public GCloudTest(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;
	
		PublicDependencyModuleNames.AddRange(new string[] { 
		"Core", 
		"CoreUObject", 
		"Engine", 
		"InputCore",
		"Bugly",
		"TMSDK",
		"HTTP",
		"PakFile",
		 });

        PrivateDependencyModuleNames.AddRange(new string[] { 
		"TApm", "GCloud", "GCloudCore", "GVoiceSDK", "TDM", "TGPAPlugin", "TuringShield", // GCloudSDK
		"Json", "JsonUtilities", // UE4 JSON
		"TMSDKCommon",
		});

        // Uncomment if you are using Slate UI
        // PrivateDependencyModuleNames.AddRange(new string[] { "Slate", "SlateCore" });

        // Uncomment if you are using online features
        // PrivateDependencyModuleNames.Add("OnlineSubsystem");

        // To include OnlineSubsystemSteam, add it to the plugins section in your uproject file with the Enabled attribute set to true



    }
}
