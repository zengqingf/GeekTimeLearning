using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Reflection;


public class BuildPlayerSettingUtility
{
    public static void BuildIL2CPP()
    {
        bool isIL2cpp  = UnityCommandArgumentParse.GetFuctionArgs(1, "true") == "true";
        if (isIL2cpp)
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

            //PlayerSettings.SetPropertyInt("ScriptingBackend", (int)ScriptingImplementation.IL2CPP, BuildTargetGroup.Android);
            UnityEngine.Debug.LogFormat("[BuildPlayer] Change Android 2 il2cpp {0}", Application.platform);
        }
        else 
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);

            UnityEngine.Debug.LogFormat("[BuildPlayer] Change Android 2 mono {0}", Application.platform);
        }

    }        
    public static void NotStripEngineCode()
    {
        PlayerSettings.stripEngineCode = false;
    }        


    public static void OpenProject()
    {
        EditorApplication.ExecuteMenuItem("Assets/Open C# Project");
    }
	
	public static void BuildAndroidSystem()
	{
        string buildSystemType = UnityCommandArgumentParse.GetFuctionArgs(1, "Internal");
#if UNITY_ANDROID
        if (buildSystemType == "Gradle" && EditorUserBuildSettings.androidBuildSystem != AndroidBuildSystem.Gradle)
        {
            UnityEngine.Debug.LogFormat("[BuildPlayer] Old Unity Build System is {0}", EditorUserBuildSettings.androidBuildSystem.ToString());
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
            UnityEngine.Debug.LogFormat("[BuildPlayer] New Unity Build System is {0}", EditorUserBuildSettings.androidBuildSystem.ToString());
        }
        else if (buildSystemType == "Internal" && EditorUserBuildSettings.androidBuildSystem != AndroidBuildSystem.Internal)
        {
            UnityEngine.Debug.LogFormat("[BuildPlayer] Old Unity Build System is {0}", EditorUserBuildSettings.androidBuildSystem.ToString());
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Internal;
            UnityEngine.Debug.LogFormat("[BuildPlayer] New Unity Build System is {0}", EditorUserBuildSettings.androidBuildSystem.ToString());
        }
#endif
	}

    public static void SetAndroidHeguKeystore()
    {
        string KeystorePath = UnityCommandArgumentParse.GetFuctionArgs(1, "None");
        UnityEngine.Debug.LogFormat("[BuildPlayer] Unity hegu KeyStore Name is {0}", KeystorePath);
#if UNITY_ANDROID
        if (KeystorePath != "None")
        {
            PlayerSettings.Android.keystoreName = KeystorePath;
            PlayerSettings.Android.keyaliasName = "hegu";
            PlayerSettings.Android.keyaliasPass = "123456";
            PlayerSettings.Android.keystorePass = "123456";           
        }
#endif
    }

    public static void SetBundleName()
    {
        string BundleName = UnityCommandArgumentParse.GetFuctionArgs(1, "None");
        UnityEngine.Debug.LogFormat("[BuildPlayer] Unity BundleName Name is {0}", BundleName);
#if UNITY_ANDROID
        if (BundleName != "None")
        {
           PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, BundleName);
           UnityEngine.Debug.LogFormat("[BuildPlayer] Unity identifiter Name is {0}", Application.identifier.ToString());
        }
#endif
    }
	
	public static void SetStackTypeAllLogTypeNone()
	{
                for (int i = 0; i <= (int)LogType.Exception; i++)
                {
                    PlayerSettings.SetStackTraceLogType((LogType)i, StackTraceLogType.None);
                }		
	}
}
