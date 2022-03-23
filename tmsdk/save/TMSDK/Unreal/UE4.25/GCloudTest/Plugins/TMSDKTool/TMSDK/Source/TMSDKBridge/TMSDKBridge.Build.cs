// Copyright Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;
using System.IO;

public class TMSDKBridge : ModuleRules
{
	public TMSDKBridge(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = ModuleRules.PCHUsageMode.UseExplicitOrSharedPCHs;

        PublicIncludePaths.Add(Path.Combine(ModuleDirectory, "Public"));
        PrivateIncludePaths.Add(Path.Combine(ModuleDirectory, "Private"));

        if (Target.Platform == UnrealTargetPlatform.Android)
        {
            PublicIncludePaths.Add(Path.Combine(ModuleDirectory, "Public/Android"));
        }
        else if (Target.Platform == UnrealTargetPlatform.IOS)
        {
            PublicIncludePaths.Add(Path.Combine(ModuleDirectory, "Public/IOS"));
        }
        else
        {
            PublicIncludePaths.Add(Path.Combine(ModuleDirectory, "Public/Default"));
        }

        if (Target.Platform == UnrealTargetPlatform.Android)
		{
			PrivateIncludePaths.Add(Path.Combine(ModuleDirectory, "Private/Android"));
		}
		else if (Target.Platform == UnrealTargetPlatform.IOS)
		{
			PrivateIncludePaths.Add(Path.Combine(ModuleDirectory, "Private/IOS"));
		}
		else
		{
			PrivateIncludePaths.Add(Path.Combine(ModuleDirectory, "Private/Default"));
		}

		PublicDependencyModuleNames.AddRange(
			new string[]
			{
				"Core",
				// ... add other public dependencies that you statically link with here ...			
			}
			);
			
		
		PrivateDependencyModuleNames.AddRange(
			new string[]
			{
				"CoreUObject",
				"Engine",
				"Slate",
				"SlateCore",
				// ... add private dependencies that you statically link with here ...	
				
				"TMSDKLib",
                "Json",
                "TMSDKCommon",
            }
			);
		
		
		if (Target.Platform == UnrealTargetPlatform.Android)
		{
			//e.g. JNI path : XXX\EpicGames\UE_4.25\Engine\Source\Runtime\Launch\Public\Android\AndroidJNI.h
			//and used in Public / xxx.h
			//PrivateDependencyModuleNames.AddRange(new string[] { "Launch" });
			PublicDependencyModuleNames.AddRange(new string[] { "Launch" });
		}
	}
}
