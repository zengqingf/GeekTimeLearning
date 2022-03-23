using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.Callbacks;
#endif
using System;
using System.IO;
using System.Reflection;
using UnityEditor.Callbacks;

public class UnityCommandArgumentParse
{
    private enum eState
    {
        onNone,
        onStart,
        onEnd,
    }

    public static string GetFuctionArgs(int idx, string def)
    {
        string[] args = GetFuctionArgs();
        if (args.Length > idx)
        {
            return args[idx];
        }

        return def;
    }

    public static string[] GetFuctionArgs()
    {
        eState state = eState.onNone;

        string[] args = System.Environment.GetCommandLineArgs();

        List<string> funcArgs = new List<string>();

        for (int i = 0; i < args.Length; ++i)
        {
            switch (state)
            {
                case eState.onNone:
                    if (args[i] == "-executeMethod")
                    {
                        state = eState.onStart;
                    }
                    break;
                case eState.onStart:
                    {
                        if (!args[i].StartsWith("-"))
                        {
                            funcArgs.Add(args[i]);
                        }
                        else 
                        {
                            state = eState.onEnd;
                            return funcArgs.ToArray();
                        }
                    }
                    break;
            }
        }
        return new string[0];
    }
}


public class BuildPlayer : MonoBehaviour 
{
#if UNITY_IOS
    private static object _getPBXObject(object project, string key)
    {
        Type type = project.GetType();
        bool isContain = (bool)type.InvokeMember("HasEntry",BindingFlags.GetField | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic, null, project, new object[] {key});

        if (isContain)
        {
            FieldInfo info = type.GetField("m_Entries", BindingFlags.NonPublic | BindingFlags.Public);
            return info.GetValue((object)key);
        }

        UnityEngine.Debug.LogErrorFormat("can't find the key {0}", key);

        return null;
    }


    private static FieldInfo _getFiled(object project, string key)
    {
        Type type = project.GetType ();
        return type.GetField(key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic);
    }

    private static object _getDict(object dic, string key)
    {
        PropertyInfo fkinfo = dic.GetType().GetProperty("Item", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
        return fkinfo.GetValue(dic, new object[] {key});
    }


	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
	{
		if (target == BuildTarget.iOS) 
		{
			UnityEngine.Debug.Log ("path " + pathToBuiltProject);

			var pbxpath = PBXProject.GetPBXProjectPath (pathToBuiltProject);
			var pbx = new PBXProject ();

			pbx.ReadFromFile (pbxpath);

            var unityTargetName = PBXProject.GetUnityTargetName ();
			var guid = pbx.TargetGuidByName (unityTargetName);
        #if UNITY_2018
            if (!pbx.ContainsFramework(guid, "libz.tbd"))
		#else
            if (!pbx.HasFramework ("libz.tbd")) 
        #endif
			{
				
				try {
					Type type = pbx.GetType ();
					PropertyInfo proInfo = type.GetProperty ("project", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic);
					object project = proInfo.GetValue (pbx, null);

                    PropertyInfo info = project.GetType().GetProperty("project", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
                    object pobj = info.GetValue(project, null);

                    FieldInfo filds = _getFiled(pobj, "m_Properties");
					object dict = filds.GetValue(pobj);

					FieldInfo realDic = _getFiled(dict, "m_PrivateValue");
					object fk = realDic.GetValue(dict);

					object attribute = _getDict(fk, "attributes");
					object tarattr = _getDict(attribute, "TargetAttributes");

					object reDict = tarattr.GetType().InvokeMember("CreateDict", BindingFlags.InvokeMethod | BindingFlags.Public, null, tarattr, new object[] {guid});

					reDict.GetType().InvokeMember("SetString", BindingFlags.InvokeMethod | BindingFlags.Public, null, reDict, new object[] {"ProvisioningStyle", "Manual"});
			

				} catch (Exception e) {

				}
				pbx.WriteToFile (pbxpath);
			}
		}

	}


    public static void Test()
    {
        OnPostprocessBuild(BuildTarget.iOS, "DNF");
    }
#endif

#if UNITY_ANDROID
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target == BuildTarget.Android)
        {
            PostProcessAndroidBuild(pathToBuiltProject);
        }
    }
#endif
    public static void PostProcessAndroidBuild(string pathToBuiltProject)
    {
		UnityEditor.ScriptingImplementation backend = (UnityEditor.ScriptingImplementation)UnityEditor.PlayerSettings.GetScriptingBackend (UnityEditor.BuildTargetGroup.Android);
        if (backend == UnityEditor.ScriptingImplementation.IL2CPP)
        {
            CopyAndroidIL2CPPSymbols(pathToBuiltProject, PlayerSettings.Android.targetArchitectures);
        }
    }

    public static void CopyAndroidIL2CPPSymbols(string pathToBuiltProject, AndroidArchitecture targetDevice)
    {
        string buildName = Path.GetFileNameWithoutExtension(pathToBuiltProject);
        FileInfo fileInfo = new FileInfo(pathToBuiltProject);
        string symbolsDir = fileInfo.Directory.Name;
        symbolsDir = symbolsDir + "/"+buildName+"_IL2CPPSymbols";
        string il2cpp_folder = Application.dataPath + "/../Temp/StagingArea/Il2Cpp/il2cppOutput";
        string savename = "source_il2cpp";

        CreateDir(symbolsDir);

        switch (PlayerSettings.Android.targetArchitectures)
        {
            case AndroidArchitecture.All:
                {
                    CopyARMSymbols(symbolsDir);
                    CopyX86Symbols(symbolsDir);
                    break;
                }
            case AndroidArchitecture.ARMv7:
                {
                    CopyARMSymbols(symbolsDir);
                    break;
                }
#if UNITY_2019_3_OR_NEWER                 
#else
            case AndroidArchitecture.X86:
                {
                    CopyX86Symbols(symbolsDir);
                    break;
                }
#endif                
            default:
                break;
        }
        if(Directory.Exists(il2cpp_folder))
        {
            string savepath = symbolsDir + "/" + savename;
            CreateDir(savepath);
            CopyTree(il2cpp_folder,savepath);
        }
    }


    const string libpath = "/../Temp/StagingArea/symbols/";
    const string libFilename = "libil2cpp.so.debug";
    private static void CopyARMSymbols(string symbolsDir)
    {
		string sourcefileARM = Application.dataPath + libpath + "armeabi-v7a/";// + libFilename;
        CreateDir(symbolsDir + "/armeabi-v7a/");
		CopyTree(sourcefileARM, symbolsDir + "/armeabi-v7a/");

        //File.Copy(sourcefileARM, symbolsDir + "/armeabi-v7a/libil2cpp.so.debug");
    }

    private static void CopyX86Symbols(string symbolsDir)
    {
		string sourcefileX86 = Application.dataPath + libpath + "x86/";//libil2cpp.so.debug";
		CopyTree(sourcefileX86, symbolsDir + "/x86/");
        //File.Copy(sourcefileX86, symbolsDir + "/x86/libil2cpp.so.debug");
    }

	private static void CopyTree(string src, string dst)
	{
		string[] allFiles = Directory.GetFiles(src, "*", SearchOption.AllDirectories);

		for (int i = 0; i < allFiles.Length; ++i) {
			string srcPath = allFiles [i];

			string destPath = srcPath.Replace (src, dst);

			if (File.Exists (srcPath)) 
			{
				CreateDir(Path.GetDirectoryName (destPath));

				if (File.Exists (destPath)) 
				{
					File.Delete (destPath);
				}

				File.Copy(srcPath, destPath);
			}
		}
	}


    public static void CreateDir(string path)
    {
        if (Directory.Exists(path))
            return;

        Directory.CreateDirectory(path);
    }


    public static void BuildAllAssets()
    {
        //string isIncremental = "false";//UnityCommandArgumentParse.GetFuctionArgs(1, "true");
        //BuildPackage.IncrementBuildBundle(isIncremental == "true");
        // AssetBundlePackerStrategyWindow.BuildPackageInterface(isIncremental == "true");

        /// 老的1.0打包
        //AssetPacker.PackAsset_CommandMode();
    }

    public static void BuildOnlyDataAsset()
    {
        //AssetPacker.PackDataAsset_CommandMode();
    }

    private static string[] _updateSceneList()
    {
        List<string> list = new List<string>();
        int count = EditorBuildSettings.scenes.Length;
        for (int i = 0; i < count; ++i)
        {
            var scene = EditorBuildSettings.scenes[i];

            if (scene.enabled && File.Exists(scene.path))
            {
                list.Add(scene.path);
                UnityEngine.Debug.LogFormat("add scene with path {0}", scene.path);
            }
            else 
            {
                UnityEngine.Debug.LogErrorFormat("scene not exit with name {0}", scene.path);
            }
        }

        return list.ToArray();
    }

    public static void OpenCSharpProject()
    {
        UnityEditor.EditorApplication.ExecuteMenuItem("Assets/Open C# Project");
    }
    public static void Refresh()
    {
        UnityEngine.Debug.LogFormat("Refresh");
    }
    //[MenuItem("[Build]/[OpenMainSceneAddUWAPrefab]")]
    public static void OpenMainSceneAddUWAPrefab()
    {
        string type = UnityCommandArgumentParse.GetFuctionArgs(1, "None");
        bool isMonoManual = UnityCommandArgumentParse.GetFuctionArgs(2, "false") == "true";
        
        //string[] SCENE_LIST = _updateSceneList();

        string prefabPath = "Assets/UWA/Prefabs/UWA_Launcher.prefab";
        if (!System.IO.File.Exists(prefabPath))
        {
            return;
        }

        // Assets/UWA/Prefabs/UWA_Launcher.prefab
        EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");


        var res = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (null == res)
        {
            return;
        }

        GameObject obj = GameObject.Instantiate(res);
        if (null == obj)
        {
            return;
        }

        obj = UnityEditor.PrefabUtility.ConnectGameObjectToPrefab(obj, res);

        if (type == "Mono" && isMonoManual)
        {
            Component com = obj.GetComponent("UWA_Launcher");
            if (null != com)
            {
                SerializedObject so = new SerializedObject(com);
                var propty = so.FindProperty("DirectManualMono");
                if (null != propty)
                {
                    propty.boolValue = true;
                    so.ApplyModifiedPropertiesWithoutUndo();
                }
                else 
                {
                    UnityEngine.Debug.LogErrorFormat("DirectManualMono not found!");
                }
            }
            else 
            {
                UnityEngine.Debug.LogErrorFormat("UWA_Launcher not found!");
            }
        }

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }


    //[MenuItem("[Build]/[BuildTarget]")]
	public static void BuildTargets()
	{
        string path = UnityCommandArgumentParse.GetFuctionArgs(1, "./DNF");
        string autoConnect = UnityCommandArgumentParse.GetFuctionArgs(2, "false");
        string platform = UnityCommandArgumentParse.GetFuctionArgs(3, "Win");

        path = Path.GetFullPath(path);

        UnityEngine.Debug.LogFormat("[BuildPlayer] Path {0} Target {1}, Platform: {2}", path, Application.platform, platform);

        string[] SCENE_LIST = _updateSceneList();

#if UNITY_IOS
		BuildPipeline.BuildPlayer(SCENE_LIST, path, BuildTarget.iOS, _getBuildOptions(autoConnect == "true"));
#elif UNITY_ANDROID
		PlayerSettings.Android.keyaliasPass = "123456";
		PlayerSettings.Android.keystorePass = "123456";
		BuildPipeline.BuildPlayer(SCENE_LIST, path+".apk", BuildTarget.Android, _getBuildOptions(autoConnect == "true"));
#else
        if (platform.ToLower().Equals("win64"))
        {
            BuildPipeline.BuildPlayer(SCENE_LIST, path+".exe", BuildTarget.StandaloneWindows64, _getBuildOptions(autoConnect == "true"));
        }
        else if ( platform.ToLower().Equals("osxuniversal"))
        {
            BuildPipeline.BuildPlayer(SCENE_LIST, path+".app", BuildTarget.StandaloneOSX, _getBuildOptions(autoConnect == "true"));
        }
#endif
	}

    public static void BuildTargetsWithOnlyScripts()
	{
        string path = UnityCommandArgumentParse.GetFuctionArgs(1, "./DNF");

        path = Path.GetFullPath(path);

        UnityEngine.Debug.LogFormat("[BuildPlayer] Path {0} Target {1}", path, Application.platform);

        string[] SCENE_LIST = _updateSceneList();

        BuildOptions buildOp = _getBuildOptions();

        buildOp |= BuildOptions.AcceptExternalModificationsToPlayer;
        buildOp |= BuildOptions.BuildScriptsOnly;

#if UNITY_IOS
		BuildPipeline.BuildPlayer(SCENE_LIST, path, BuildTarget.iOS, buildOp);
#elif UNITY_ANDROID
		PlayerSettings.Android.keyaliasPass = "123456";
		PlayerSettings.Android.keystorePass = "123456";
		BuildPipeline.BuildPlayer(SCENE_LIST, path, BuildTarget.Android, buildOp);
#elif UNITY_EDITOR_OSX
#elif UNITY_EDITOR_WIN
#else
#endif
	}


    private static BuildOptions _getBuildOptions(bool isAutoconnect = false)
    {
        BuildOptions op = BuildOptions.None;

        if (EditorUserBuildSettings.connectProfiler && isAutoconnect)
        {
            op |= BuildOptions.ConnectWithProfiler;

            UnityEngine.Debug.LogFormat("[BuildPlayer] connnect with profiler");
        }

        if ( EditorUserBuildSettings.development)
        {
            op |= BuildOptions.Development;

            UnityEngine.Debug.LogFormat("[BuildPlayer] development build");
        }

        return op;
    }

    public static void BuildMarcoConfig()
    {
        string marco = UnityCommandArgumentParse.GetFuctionArgs(1, "LOG_ERROR");

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, marco);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, marco);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, marco);
    }

    public static void SetBuildDevAndAutoConnectProfiler()
    {
        string isDev = UnityCommandArgumentParse.GetFuctionArgs(1, "true");

        EditorUserBuildSettings.development     = isDev == "true";
        EditorUserBuildSettings.allowDebugging  = isDev == "true";
    }

    public static void SetGlobalSetting()
    {
        string[] args = UnityCommandArgumentParse.GetFuctionArgs();
        int i = 1;
        while (i < args.Length && i < args.Length - 1)
        {
            string filed = args[i];//UnityCommandArgumentParse.GetFuctionArgs(1, "isGuide");
            string isOpen = args[i + 1];//UnityCommandArgumentParse.GetFuctionArgs(2, "true");

            _setglobalSetting(filed, isOpen == "true");

            i += 2;
        }
    }

    public static void SetGlobalChannel()
    {
        //string sdkChannel = UnityCommandArgumentParse.GetFuctionArgs(1, SDKChannel.NONE.ToString());
        //_setglobalSetting("sdkChannel", sdkChannel);

        //File.WriteAllText("Assets/StreamingAssets/sdk.conf", sdkChannel);
    }

    private static void _setglobalSetting(string filed, object v)
    {
//        UnityEngine.Debug.LogFormat("GlobalSetting KeyValue {0} {1}", filed, v);
//#if !LOGIC_SERVER
//        GlobalSetting setting = AssetDatabase.LoadAssetAtPath<GlobalSetting>("Assets/Resources/"+Global.PATH+".asset");

//        SerializedObject sobj = new SerializedObject(setting);

//        SerializedProperty prop = sobj.FindProperty(filed);

//        if (null != prop)
//        {
//            switch (prop.propertyType)
//            {
//                case SerializedPropertyType.String:
//                    prop.stringValue = (string)v;
//                    break;
//                case SerializedPropertyType.Boolean:
//                    prop.boolValue = (bool)v;
//                    break;
//                case SerializedPropertyType.Integer:
//                    prop.intValue = (int)v;
//                    break;
//                case SerializedPropertyType.Float:
//                    prop.floatValue = (float)v;
//                    break;
//                case SerializedPropertyType.Enum:
//                    for (int i = 0; i < prop.enumNames.Length; ++i)
//                    {
//                        if (prop.enumNames[i] == (string)v)
//                        {
//                            prop.enumValueIndex = i;
//                            break;
//                        }
//                    }
//                    break;
//            }
//        }

//        sobj.ApplyModifiedProperties();

//        EditorUtility.SetDirty(setting);
//        AssetDatabase.SaveAssets();
//#endif
    }





    public class BuildConf
    {
        public bool isDev;
        public bool isIL2CPP;
        public bool isConnectProfiler;

    }
    public static void NewBuild()
    {
        string config_json = File.ReadAllText("buildconf.json");
        var buildconf = JsonUtility.FromJson<BuildConf>(config_json);

        // 宏直接改ProjectSetting.assets，可以少一次编译脚本
        // yaml:
        // ProjectSettings.scriptingDefineSymbols[7]
        // Index：
        // 7 = Android
        // 1 = IOS
        // 4 = Standalone
        // ProjectSetting.assets UnityYaml格式
        SetScriptingBackend(buildconf.isIL2CPP);

        // EditorUserBuildSettings.assets AB包格式
        EditorUserBuildSettings.development = buildconf.isDev;
        EditorUserBuildSettings.connectProfiler = buildconf.isConnectProfiler;


        AssetDatabase.SaveAssets();

        //build
        BuildPlayerOptions opt = new BuildPlayerOptions();
        opt.options |= BuildOptions.AcceptExternalModificationsToPlayer;
        UnityEditor.BuildPipeline.BuildPlayer(opt);
        //拷贝符号表
    }
    private static void SetScriptingBackend(bool isIL2CPP)
    {
#if UNITY_IOS
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, isIL2CPP ? ScriptingImplementation.IL2CPP : ScriptingImplementation.Mono2x);
#elif UNITY_ANDROID
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, isIL2CPP ? ScriptingImplementation.IL2CPP : ScriptingImplementation.Mono2x);
#elif UNITY_STANDALONE
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, isIL2CPP ? ScriptingImplementation.IL2CPP : ScriptingImplementation.Mono2x);
#elif UNITY_WEBGL
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.WebGL, isIL2CPP ? ScriptingImplementation.IL2CPP : ScriptingImplementation.Mono2x);
#endif
    }








}
