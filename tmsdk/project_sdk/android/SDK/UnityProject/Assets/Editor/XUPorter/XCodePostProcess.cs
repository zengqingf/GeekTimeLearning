using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;
#endif
using System.IO;
using System.Collections;

public static class XCodePostProcess
{

#if UNITY_EDITOR
	[PostProcessBuild(999)]
	public static void OnPostProcessBuild( BuildTarget target, string pathToBuiltProject )
	{
#if UNITY_5
		if (target != BuildTarget.iOS) {
#else
        if (target != BuildTarget.iOS) {
#endif
			Debug.LogWarning("Target is not iPhone. XCodePostProcess will not run");
			return;
		}

		// Create a new project object from build target
		XCProject project = new XCProject( Path.GetFullPath(pathToBuiltProject) );

		// Find and run through all projmods files to patch the project.
		// Please pay attention that ALL projmods files in your project folder will be excuted!
		string[] files = Directory.GetFiles( Application.dataPath, "*.projmods", SearchOption.AllDirectories );
		foreach( string file in files ) {
			UnityEngine.Debug.Log("ProjMod File: "+file);
			project.ApplyMod( file );
		}

		//TODO disable the bitcode for iOS 9
		project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "Release");
		project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "Debug");
		project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "ReleaseForProfiling");
		project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "ReleaseForRunning");

        //TODO use try catch in Xcode
		project.overwriteBuildSetting("GCC_ENABLE_OBJC_EXCEPTIONS", "YES", "Release");
		project.overwriteBuildSetting("GCC_ENABLE_OBJC_EXCEPTIONS", "YES", "Debug");
		project.overwriteBuildSetting("GCC_ENABLE_OBJC_EXCEPTIONS", "YES", "ReleaseForProfiling");
		project.overwriteBuildSetting("GCC_ENABLE_OBJC_EXCEPTIONS", "YES", "ReleaseForRunning");

        //TODO add framework path
        project.overwriteBuildSetting("LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks", "ReleaseForProfiling");
        project.overwriteBuildSetting("LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks", "ReleaseForRunning");

        //TODO implement generic settings as a module option
        //		project.overwriteBuildSetting("CODE_SIGN_IDENTITY[sdk=iphoneos*]", "iPhone Distribution", "Release");
        string path = Path.GetFullPath (pathToBuiltProject);

//		EditorPlist(path);
		EditorCode(path);

		// Finally save the xcode project
		project.Save();

	}
	private static void EditorCode(string filePath)
	{
		
		//读取UnityAppController.mm文件
		XClass UnityAppController = new XClass(filePath + "/Classes/UnityAppController.mm");
		XClass UnityAppController_h = new XClass(filePath + "/Classes/UnityAppController.h");

		UnityAppController_h.WriteBelow ("@property (nonatomic, copy)                                 void(^quitHandler)();\n", "@property (nonatomic,strong)NSString *allowRotate;");

		//在指定代码后面增加一行代码
		//UnityAppController.WriteBelow("#include <mach/mach_time.h>\n","#import <AWSDK/DDSDKApplicationDelegate.H>\n");

		//在指定代码后面增加一行
		UnityAppController.WriteBelow("- (NSUInteger)application:(UIApplication*)application supportedInterfaceOrientationsForWindow:(UIWindow*)window\n{\n    ","if (window == self.window) {\n        return (UIInterfaceOrientationMaskLandscape);\n    }\n    if ([_allowRotate isEqualToString:@\"1\"]) {\n        return UIInterfaceOrientationMaskPortrait;\n    }else{\n        return (UIInterfaceOrientationMaskLandscape);\n    }\n    return (UIInterfaceOrientationMaskLandscape);");
		UnityAppController.WriteBelow("    [self createUI];\n    [self preStartUnity];\n","    _allowRotate = @\"0\";\n    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(orientChange) name:UIDeviceOrientationDidChangeNotification object:nil];\n");
		UnityAppController.WriteBelow("- (void)preStartUnity               {}\n\n","-(void)orientChange{\n    \n    UIDeviceOrientation orientation = [UIDevice currentDevice].orientation;\n    \n    if ((orientation == UIDeviceOrientationPortrait || orientation == UIDeviceOrientationUnknown) && [self.allowRotate isEqualToString:@\"0\"]) {\n        \n        NSNumber *orientationUnknown = [NSNumber numberWithInt:UIInterfaceOrientationUnknown];\n        [[UIDevice currentDevice] setValue:orientationUnknown forKey:@\"orientation\"];\n        NSNumber *orientationMG = [NSNumber numberWithInt:[XYPlatform defaultPlatform].MGInterfaceOrientation];\n        NSNumber *orientationTarget = [NSNumber numberWithInt:UIInterfaceOrientationLandscapeLeft];\n        if(orientationMG != nil){\n            [[UIDevice currentDevice] setValue:orientationMG forKey:@\"orientation\"];\n        }else {\n            [[UIDevice currentDevice] setValue:orientationTarget forKey:@\"orientation\"];\n        }\n    }\n    \n}");
		UnityAppController.WriteBelow ("#include \"PluginBase/AppDelegateListener.h\"\n", "#import <HZPlatform/XYPlatform.h>");

        //lebian
		UnityAppController.WriteBelow ("#include \"PluginBase/AppDelegateListener.h\"\n", "#import <LBSDK/LBInit.h>");
		UnityAppController.WriteBelow("- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions\n{\n", "if ([[LBInit sharedInstance] LBSDKShouldInitWithLaunchOptions:launchOptions]) {\n return YES;\n }");

        //breakpad
        UnityAppController.WriteBelow("#include \"CrashReporter.h\"\n", "#include \"client/ios/BreakpadController.h\"\n");
        UnityAppController.WriteBelow("- (BOOL)application:(UIApplication*)application willFinishLaunchingWithOptions:(NSDictionary*)launchOptions\n{\n",
               "\n[[BreakpadController sharedInstance] start:YES];\n[[BreakpadController sharedInstance] setUploadingEnabled: YES];\n");
        UnityAppController.WriteBelow("- (void)applicationWillTerminate:(UIApplication*)application\n{\n",
            "\n[[BreakpadController sharedInstance] stop];\n");

        //UnityAppController.WriteBelow("    [KeyboardDelegate Initialize];", "    \n    // for raw pack\n    [[XYPlatform defaultPlatform]MGChoose:YES Completion:^(aType type, NSString *appid) {\n        if (type == aTypeC) {\n        }else if (type == aTypeD) {\n            \n            [[XYPlatform defaultPlatform] initializeWithAppId:appid isContinueWhenCheckUpdateFailed:YES];\n        }else {\n            \n            [[XYPlatform defaultPlatform]loadAgain];\n        }\n    }];");

        //Unity2018上隐藏iPhone X下面的手势按键，防止进入游戏的误触，和unity5.6有区别
        XClass unityViewController = new XClass (filePath + "/Classes/UI/UnityViewControllerBase+iOS.mm");
		if (unityViewController != null) {
			unityViewController.Replace ("- (UIRectEdge)preferredScreenEdgesDeferringSystemGestures\n{\n    UIRectEdge res = UIRectEdgeNone;\n    if (UnityGetDeferSystemGesturesTopEdge())\n        res |= UIRectEdgeTop;\n    if (UnityGetDeferSystemGesturesBottomEdge())\n        res |= UIRectEdgeBottom;\n    if (UnityGetDeferSystemGesturesLeftEdge())\n        res |= UIRectEdgeLeft;\n    if (UnityGetDeferSystemGesturesRightEdge())\n        res |= UIRectEdgeRight;\n    return res;\n}\n\n- (BOOL)prefersHomeIndicatorAutoHidden\n{\n    return UnityGetHideHomeButton();\n}", 
				"- (BOOL)prefersHomeIndicatorAutoHidden\n{\n    return NO;\n}\n\n// add this\n- (UIRectEdge)preferredScreenEdgesDeferringSystemGestures\n{\n    return UIRectEdgeAll;\n}\n");
		}

	}
#endif

        public static void Log(string message)
	{
		UnityEngine.Debug.Log("PostProcess: "+message);
	}
}
