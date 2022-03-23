// Copyright Epic Games, Inc. All Rights Reserved.
using UnrealBuildTool;

public class TMSDK : ModuleRules
{
    //这个模块用于主模块调用封装的SDK层接口，如TMSDKManager，BuglyManager...
	public TMSDK(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = ModuleRules.PCHUsageMode.UseExplicitOrSharedPCHs;
		
        //迁移到TMSDKLib.Build.cs中了
		//string PluginPath = Utils.MakePathRelativeTo(ModuleDirectory, "..\\");
		//if (Target.Platform == UnrealTargetPlatform.Android)
		//{
		//	string aplPath = Path.Combine(PluginPath, "TMSDK_APL.xml");
		//	AdditionalPropertiesForReceipt.Add(new ReceiptProperty("AndroidPlugin", aplPath));
		//}

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

				"TMSDKCommon",
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

				//"Bugly",   //need add the modify Bugly Plugin from TMSDK				
				"TMSDKBridge",
			}
			);
		
		
		DynamicallyLoadedModuleNames.AddRange(
			new string[]
			{
				// ... add any modules that your module loads dynamically here ...
			}
			);
	}
}
