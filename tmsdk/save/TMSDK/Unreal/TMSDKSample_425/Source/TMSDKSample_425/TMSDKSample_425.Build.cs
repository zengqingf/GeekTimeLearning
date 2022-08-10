// Copyright Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;

public class TMSDKSample_425 : ModuleRules
{
	public TMSDKSample_425(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;
	
		PublicDependencyModuleNames.AddRange(new string[]
        { 
            "Core", 
            "CoreUObject", 
            "Engine", 
            "InputCore",  /*跨模块调用：必须包含引擎相关模块*/
            "TMSDKCommon", /*跨模块调用：需要调用当前主模块的子模块（需要Public）*/
            
            "CryptoKeys",
        });

		PrivateDependencyModuleNames.AddRange(new string[] 
        { 
            "Json", "JsonUtilities", 
            "TMSDK", 
            "Http",
            "TestTools",

            "UMG", "Slate", "SlateCore"
        });

        //if (Target.Configuration != UnrealTargetConfiguration.Shipping)
        //{
        //    PrecompileForTargets = PrecompileTargetsType.Any;
        //}

        //if (Target.Type == TargetRules.TargetType.Editor)
        //{
        //    DynamicallyLoadedModuleNames.AddRange(new string[]
        //    {
        //          "FileUtilities",
        //    });
        //    PrivateDependencyModuleNames.AddRange(new string[]
        //    {
        //          "FileUtilities",
        //    });
        //}

        // Uncomment if you are using Slate UI
        // PrivateDependencyModuleNames.AddRange(new string[] { "Slate", "SlateCore" });

        // Uncomment if you are using online features
        // PrivateDependencyModuleNames.Add("OnlineSubsystem");

        // To include OnlineSubsystemSteam, add it to the plugins section in your uproject file with the Enabled attribute set to true


        //regen vs project
        //PublicDefinitions.Add("TMLOG_DEBUG=1");
        //PublicDefinitions.Add("TMLOG_INFO=0");
        //PublicDefinitions.Add("TMLOG_WARN=0");
        //PublicDefinitions.Add("TMLOG_ERROR=1");
        //PublicDefinitions.Add("TMLOG_FATAL=1");

        PublicDefinitions.AddRange(new string[] { "TMLOG_DEBUG", "TMLOG_INFO", "TMLOG_WARN", "TMLOG_ERROR", "TMLOG_FATAL" });
    }
}
