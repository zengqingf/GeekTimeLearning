using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundle
{

    [MenuItem("Bulid/BUild AssetBundles")]//编辑器菜单路径 打包
    static void BulidAllAssetBundle()
    {
        var marco = "LOG_ERROR";

        //PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, marco);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, marco);
        //PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, marco);

        AssetDatabase.SaveAssets();

    }
}