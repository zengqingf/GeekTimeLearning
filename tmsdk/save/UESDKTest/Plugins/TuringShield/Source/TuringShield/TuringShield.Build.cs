// Copyright 1998-2017 Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;
using System;
using System.IO;

public class TuringShield : ModuleRules
{
	public TuringShield(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = ModuleRules.PCHUsageMode.UseExplicitOrSharedPCHs;

#if UE_4_20_OR_LATER
		// ue 4.20 or later do not need PublicIncludePaths
#else
		PublicIncludePaths.AddRange(
			new string[] {
				"TuringShield/Public"
				// ... add public include paths required here ...
			}
			);
#endif
				
		PrivateIncludePaths.AddRange(
			new string[] {
				"TuringShield/Private",
				// ... add other private include paths required here ...
			}
			);
			
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
			}
			);
		
		
		DynamicallyLoadedModuleNames.AddRange(
			new string[]
			{
				// ... add any modules that your module loads dynamically here ...
			}
			);

        if (Target.Platform == UnrealTargetPlatform.IOS)
        {
            System.Console.WriteLine("--------------Add iOS TuringShield");

			var libPath = Path.GetFullPath(ModuleDirectory);
			PublicAdditionalLibraries.Add(Path.Combine(libPath, "../TuringShield/lib/iOS/libTuringShield.a"));
            // //IOSEnd
            PublicFrameworks.AddRange(new string[] { "CoreTelephony", "AdSupport", "Security", "UIKit", "SystemConfiguration", "AudioToolbox", "Foundation", "WebKit"});
            // PublicWeakFrameworks.AddRange(new string[]{"AppTrackingTransparency"});
            PublicAdditionalLibraries.AddRange(new string[] { "z", "c++", "resolv" });
        }
    }
}
