// Copyright Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;

public class TMSDKCommon : ModuleRules
{
	public TMSDKCommon(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = ModuleRules.PCHUsageMode.UseExplicitOrSharedPCHs;
		
		PublicIncludePaths.AddRange(
			new string[] {
				// ... add public include paths required here ...
			}
			);
				
		
		PrivateIncludePaths.AddRange(
			new string[] {
				// ... add other private include paths required here ...
			}
			);
			
		
		PublicDependencyModuleNames.AddRange(
			new string[]
			{
				"Core",
				// ... add other public dependencies that you statically link with here ...

				"Json",
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
			}
			);
		
		
		DynamicallyLoadedModuleNames.AddRange(
			new string[]
			{
				// ... add any modules that your module loads dynamically here ...
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
