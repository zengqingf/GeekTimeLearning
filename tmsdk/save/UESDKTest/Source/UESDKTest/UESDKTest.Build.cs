// Copyright Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;

public class UESDKTest : ModuleRules
{
	public UESDKTest(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;
	
		PublicDependencyModuleNames.AddRange(new string[] { "Core", "CoreUObject", "Engine", "InputCore", "ApplicationCore" });

		//PublicDependencyModuleNames.AddRange(new string[] { "GCloudCore" , "GVoice" , "TDM" });
		PrivateDependencyModuleNames.AddRange(new string[] { "GCloudCore" , "GVoice" , "TDM" , "TuringShield", "ApplicationCore", "GCloud","Http", "Json", "JsonUtilities" });

        // Uncomment if you are using Slate UI
        // PrivateDependencyModuleNames.AddRange(new string[] { "Slate", "SlateCore" });

        // Uncomment if you are using online features
        // PrivateDependencyModuleNames.Add("OnlineSubsystem");

        // To include OnlineSubsystemSteam, add it to the plugins section in your uproject file with the Enabled attribute set to true
        if (Target.Platform == UnrealTargetPlatform.Android)
        {
            PrivateDependencyModuleNames.AddRange(new string[] { "Launch" });
        }
    }
}
