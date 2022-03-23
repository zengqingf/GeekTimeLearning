// Fill out your copyright notice in the Description page of Project Settings.

using System.IO;
using UnrealBuildTool;

public class GVoiceSDKLibrary : ModuleRules
{
	public string ProjectDirectory
    {
        get
        {
            return Path.GetFullPath(
                  Path.Combine(ModuleDirectory, "../../../../../../")
            );
        }
    }

    private void CopyDllAndLibToProjectBinaries(string Filepath, ReadOnlyTargetRules Target)
    {
        string BinariesDirectory = Path.Combine(ProjectDirectory, "Binaries", Target.Platform.ToString());

        string Filename = Path.GetFileName(Filepath);

        if (!Directory.Exists(BinariesDirectory))
        {
            Directory.CreateDirectory(BinariesDirectory);
        }

        File.Copy(Filepath, Path.Combine(BinariesDirectory, Filename), true);

        RuntimeDependencies.Add(Path.Combine(BinariesDirectory, Filename));
    }

	public GVoiceSDKLibrary(ReadOnlyTargetRules Target) : base(Target)
	{
		Type = ModuleType.External;

		if (Target.Platform == UnrealTargetPlatform.Win64)
		{
			// Add the import library
			//PublicLibraryPaths.Add(Path.Combine(ModuleDirectory, "x64", "Release"));
			//PublicLibraryPaths.Add(Path.Combine(ModuleDirectory, "x64"));
			//PublicAdditionalLibraries.Add("ExampleLibrary.lib");

			// Delay-load the DLL, so we can load it from the right place first
			//PublicDelayLoadDLLs.Add("ExampleLibrary.dll");
			string Win64ThirdPartyDir = Path.GetFullPath(Path.Combine(ModuleDirectory, "x64"));
#if UE_4_24_OR_LATER
            PublicSystemLibraryPaths.Add(Win64ThirdPartyDir);
			//CopyDllAndLibToProjectBinaries(Path.Combine(ModuleDirectory,"x64", "GCloudVoice.dll"), Target);
       
#else
            PublicLibraryPaths.Add(Win64ThirdPartyDir);
#endif
		}
        else if (Target.Platform == UnrealTargetPlatform.Mac)
        {
            //PublicDelayLoadDLLs.Add(Path.Combine(ModuleDirectory, "Mac", "Release", "libExampleLibrary.dylib"));
        }
	}
}
