// Copyright 1998-2016 Epic Games, Inc. All Rights Reserved.
// Created and edited by vincentwgao.com


using UnrealBuildTool;
using System.IO;
using System;
public class TApm : ModuleRules
{
#if WITH_FORWARDED_MODULE_RULES_CTOR
		public TApm(ReadOnlyTargetRules Target) : base(Target)
#else
		public TApm(TargetInfo Target)
#endif
    {
		PCHUsage = ModuleRules.PCHUsageMode.UseExplicitOrSharedPCHs;

#if UE_4_20_OR_LATER
        // ue 4.20 or later do not need PublicIncludePaths
#else
        PublicIncludePaths.AddRange(
            new string[] {
                "TApm/Public"
				// ... add public include paths required here ...
			}
            );
#endif

        PrivateIncludePaths.AddRange(
            new string[] {
                "TApm/Private"
				// ... add other private include paths required here ...
			}
            );


        PublicDependencyModuleNames.AddRange(
            new string[]
            {
                "Core",
                "CoreUObject",
                "Engine",
				"Slate",
				"SlateCore",
				// ... add other public dependencies that you statically link with here ...
                //"UMG",
            }
            );

        if (Target.Platform == UnrealTargetPlatform.IOS ||
            Target.Platform == UnrealTargetPlatform.Mac)
        {
            PrivateDependencyModuleNames.AddRange(
            new string[]
            {
                "CoreUObject",
                "Engine",
				// ... add private dependencies that you statically link with here ...	
			}
            );
        }
        else
        {
            PrivateDependencyModuleNames.AddRange(
            new string[]
            {
                "Core",
                "CoreUObject",
                "Engine",              
				// ... add private dependencies that you statically link with here ...	

            }
            );
        }
        if (Target.Type == TargetRules.TargetType.Editor)
        {

            DynamicallyLoadedModuleNames.AddRange(
                        new string[] {
                    "Settings",
                    }
                    );

            PrivateIncludePathModuleNames.AddRange(
                new string[] {
                    "Settings",
            }
            );
        }



        if (Target.Platform == UnrealTargetPlatform.Android)
        {
            PrivateDependencyModuleNames.AddRange(new string[] { "Launch" });

//            string PluginPath = Utils.MakePathRelativeTo(ModuleDirectory, BuildConfiguration.RelativeEnginePath);
//            //Console.Write(PluginPath);
//            AdditionalPropertiesForReceipt.Add(new ReceiptProperty("AndroidPlugin", System.IO.Path.Combine(PluginPath, "TApm_APL.xml")));
			string PluginPath = Utils.MakePathRelativeTo(ModuleDirectory, "..\\");
            string aplPath = Path.Combine(PluginPath, "TApm_APL.xml");
            //System.Console.WriteLine("-------------- AplPath = " + aplPath);

#if UE_4_19_OR_LATER
			AdditionalPropertiesForReceipt.Add("AndroidPlugin", aplPath);
#else 
            AdditionalPropertiesForReceipt.Add(new ReceiptProperty("AndroidPlugin", aplPath));
#endif
		
        }
        else if (Target.Platform == UnrealTargetPlatform.IOS)
        {
#if UE_4_24_OR_LATER
            PublicSystemLibraries.AddRange(
				new string[] {
				 "z","c++","z.1.1.3","sqlite3","xml2","resolv"
			});
#else
		    PublicAdditionalLibraries.AddRange(
                new string[] {
                "z","c++","z.1.1.3","sqlite3","xml2","resolv"
            });
#endif
                // To include OnlineSubsystemSteam, add it to the plugins section in your uproject file with the Enabled attribute set to true
                PublicFrameworks.AddRange(
                    new string[] {
                        "CoreTelephony",
                        "Foundation",
                        "SystemConfiguration",
                        "CFNetwork",
                    });

            PublicAdditionalLibraries.Add(Path.Combine(ModuleDirectory, "../APMSDKLib/iOS", "APM.a"));
          
        }


    }
}
