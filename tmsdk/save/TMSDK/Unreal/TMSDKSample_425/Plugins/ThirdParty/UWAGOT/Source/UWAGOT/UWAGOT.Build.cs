// Copyright 1998-2018 Epic Games, Inc. All Rights Reserved.

//#define UWA_SLUA_PROFILE

using UnrealBuildTool;
using System.IO;
public class UWAGOT : ModuleRules
{
    public UWAGOT(ReadOnlyTargetRules Target) : base(Target)
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
                "UWALib",
				"Projects",
                "UMG",
#if UWA_SLUA_PROFILE
                "slua_unreal",
#endif
                "RenderCore",
                "Renderer",
#if !UE_4_22_OR_LATER
                "ShaderCore",
#endif
                "RHI",
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
			});
        if (Target.Platform == UnrealTargetPlatform.Android)
        {
            PrivateDependencyModuleNames.AddRange(new string[] { "Launch" });
            string PluginPath = Utils.MakePathRelativeTo(ModuleDirectory, Target.RelativeEnginePath);
            AdditionalPropertiesForReceipt.Add(new ReceiptProperty("AndroidPlugin", Path.Combine(PluginPath, "UWAGOT_UPL_Android.xml")));
        }
    }
}
