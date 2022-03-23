// Copyright Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;
using System.Collections.Generic;

public class TMSDKSample_425Target : TargetRules
{
	public TMSDKSample_425Target( TargetInfo Target) : base(Target)
	{
		Type = TargetType.Game;

        //show logs when building with shipping
        bUseLoggingInShipping = true;
        bOverrideBuildEnvironment = true;

        DefaultBuildSettings = BuildSettingsVersion.V2;
		ExtraModuleNames.AddRange( new string[] { "TMSDKSample_425" } );

		bOverrideBuildEnvironment = true;
		if (Target.Platform != UnrealTargetPlatform.Win64)
		{
			ShadowVariableWarningLevel = WarningLevel.Off;
			AdditionalCompilerArguments += " -Wno-shadow";
			AdditionalCompilerArguments += " -Wno-unused-value";
		}

		GlobalDefinitions.AddRange(new string[] { 
			/*打包脚本宏占位符*/
		});


        // enable logs and debugging for Shipping builds
        //just for windows exe
        //if (Configuration == UnrealTargetConfiguration.Shipping)
        //{
        //    BuildEnvironment = TargetBuildEnvironment.Unique;
        //    bUseChecksInShipping = true;
        //    bUseLoggingInShipping = true;
        //}
    }
}
