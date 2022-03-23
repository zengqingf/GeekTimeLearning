// Fill out your copyright notice in the Description page of Project Settings.

using System.IO;
using UnrealBuildTool;

public class UWALib : ModuleRules
{
	public UWALib(ReadOnlyTargetRules Target) : base(Target)
	{
		Type = ModuleType.External;

		if (Target.Platform == UnrealTargetPlatform.Win64)
		{
			// Add the import library
			PublicAdditionalLibraries.Add(Path.Combine(ModuleDirectory, "x64", "Release", "uwa.lib"));

			// Delay-load the DLL, so we can load it from the right place first
			PublicDelayLoadDLLs.Add("uwa.dll");

			// Ensure that the DLL is staged along with the executable
			RuntimeDependencies.Add("$(PluginDir)/Binaries/ThirdParty/UWALib/Win64/uwa.dll");
        }

        if (Target.Platform == UnrealTargetPlatform.Android)
        {
			string PluginPath = Utils.MakePathRelativeTo(ModuleDirectory, Target.RelativeEnginePath);
			AdditionalPropertiesForReceipt.Add(new ReceiptProperty("AndroidPlugin", Path.Combine(PluginPath, "UWALib_UPL_Android.xml")));

            PublicAdditionalLibraries.Add(ModuleDirectory + "/libs/arm64-v8a/libuwa.so");
            PublicAdditionalLibraries.Add(ModuleDirectory + "/libs/armeabi-v7a/libuwa.so");
            //PublicAdditionalLibraries.Add(ModuleDirectory + "/libs/x86/libuwa.so");
        }
    }
}
