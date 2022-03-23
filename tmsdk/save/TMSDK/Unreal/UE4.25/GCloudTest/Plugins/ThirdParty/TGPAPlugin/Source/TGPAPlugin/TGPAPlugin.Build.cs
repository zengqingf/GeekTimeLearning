// Copyright 1998-2018 Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;
using System.IO;
using System;

public class TGPAPlugin : ModuleRules
{
	public TGPAPlugin(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = ModuleRules.PCHUsageMode.UseExplicitOrSharedPCHs;
		
		PublicIncludePaths.AddRange(
			new string[] {
				// ... add public include paths required here ...
				"TGPAPlugin/Public"
			}
			);
				
		
		PrivateIncludePaths.AddRange(
			new string[] {
				// ... add other private include paths required here ...
				"TGPAPlugin/Private"
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

		string pluginPath = Utils.MakePathRelativeTo(ModuleDirectory, Target.RelativeEnginePath);
		if (Target.Platform == UnrealTargetPlatform.Android)
        {
            PrivateDependencyModuleNames.AddRange(new string[] { "Launch" });
            string aplPath = Path.Combine(pluginPath, "TGPA_APL.xml");
            System.Console.WriteLine("TGPA APL path: " + aplPath);
#if UE_4_20_OR_LATER
			AdditionalPropertiesForReceipt.Add("AndroidPlugin", aplPath);	
#else
			AdditionalPropertiesForReceipt.Add(new ReceiptProperty("AndroidPlugin", aplPath));
#endif
        } else if (Target.Platform == UnrealTargetPlatform.IOS) {
			PublicAdditionalFrameworks.Add(
#if UE_4_22_OR_LATER
				new Framework("kgvmp", "iOS/kgvmp.embeddedframework.zip", "")
#else
				new UEBuildFramework("kgvmp", "iOS/kgvmp.embeddedframework.zip", "")
#endif
				);
			// PublicAdditionalLibraries.Add(Path.Combine(ModuleDirectory, "iOS", "libTuringShield.a"));
			PublicFrameworks.AddRange(
                    new string[] {
						"UIKit",
						"Foundation",
						"SystemConfiguration"
                    }
                );
		}
	}
}
