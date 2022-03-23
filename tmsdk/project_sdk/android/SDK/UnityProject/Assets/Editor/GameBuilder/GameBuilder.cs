using UnityEngine;

using UnityEditor;
using System.Collections;
using System;
using System.IO;

/// <summary>
/// Build In Unity
/// 
/// be fast to build !
/// 
/// 
/// </summary>
/// 
namespace CustomGameBuild
{
    class GameBuilder
    {
        // 输出路径
        private const string BuildPath = "../export project/";
        private const string BuildApkPath = "../export apk/";

        /*
        [MenuItem("GameBuilder/BuildForAndroid")]
        public static void BuildForAndroid()
        {

            ////!!!!!!!!!!!!!!!!!!!!!!!!Test!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //Global.LOGIN_SERVER_ADDRESS = "120.132.26.173:59006";
            //Global.IOS_ZY_CHARGE = "120.132.26.173:59003";


            //需要动态修改的有

            //Global.LOGIN_SERVER_ADDRESS = "120.132.26.173:59006";//"192.168.2.26:56351";
            //Global.ANDROID_MG_CHARGE = "120.132.26.173:59003";//"192.168.2.26:57352";
            //Global.ROLE_SAVEDATA_SERVER_ADDRESS = "120.132.26.173:59765";

            //SDKInterface ServerList

            //version.json

            //version.config

            //icon


            GlobalSetting settings = AssetDatabase.LoadAssetAtPath<GlobalSetting>("Assets/Resources/" + Global.PATH + ".asset");

            SDKChannel sdkChannel = SDKChannel.MG;
            if (settings != null)
            {
                settings.sdkChannel = sdkChannel;
                settings.isPaySDKDebug = true;
                settings.isDebug = true;
            }

            string sdkconfigPath = Application.streamingAssetsPath + "/sdk.conf";
            Debug.LogError("BuildForAndroid sdkconfigPath :" + sdkconfigPath);

            using (FileStream fs = new FileStream(sdkconfigPath, FileMode.Create, FileAccess.ReadWrite))
            {
                //using (StreamReader sr = new StreamReader(fs))
                //{
                //    string content = sr.ReadToEnd();
                //    UnityEngine.Debug.LogFormat("{ sdk.conf original content is {0}}",content);
                //}

                byte[] text = System.Text.Encoding.Default.GetBytes(settings.sdkChannel.ToString());
                fs.Write(text, 0, text.Length);
                fs.Flush();
                fs.Close();
            }

            using (FileStream fs = new FileStream(sdkconfigPath, FileMode.Open, FileAccess.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string content = sr.ReadToEnd();
                    UnityEngine.Debug.LogErrorFormat(" sdk.conf new content is {0}", content);
                }
            }



            //开启打包!

            BuildTargetGroup buildTargetGroup = BuildTargetGroup.Android;
            string iconPathBySdkChannel;

            iconPathBySdkChannel = "file://" + Application.streamingAssetsPath + "/TestBuildGame_ICON_PATHS/" + "Title_DNL_Icon.png";

            Debug.LogError("iconPathBySdkChannel path is " + iconPathBySdkChannel);

            // 如果不是android平台,转为android平台
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);

                buildTargetGroup = BuildTargetGroup.Android;
            }

            ImageToTextureGenerator.instance.StartLoadIconImage(iconPathBySdkChannel);
            ImageToTextureGenerator.instance.loadIconCallback = (iconTex) =>
            {
                Texture2D[] textureIcons = new Texture2D[]{
                iconTex
             };


                PlayerSettings.SetIconsForTargetGroup(buildTargetGroup, textureIcons);


                //il2cpp
                //PlayerSettings.SetIncrementalIl2CppBuild(buildTargetGroup, true);
                PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.Mono2x);


                // 允许读写SD卡
                PlayerSettings.Android.forceSDCardPermission = true;

                PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;


                // 设置 keystore 信息
                //PlayerSettings.Android.keystoreName = "E:/WorkSpace/SDKChannel/keystore/MG/android.keystore";
                //PlayerSettings.Android.keystorePass = "123456";
                //PlayerSettings.Android.keyaliasName = "android.keystore";
                //PlayerSettings.Android.keyaliasPass = "123456";

                //
                PlayerSettings.Android.keystoreName = "E:/WorkSpace/SDKChannel/keystore/hegu.keystore";
                PlayerSettings.Android.keystorePass = "123456";
                PlayerSettings.Android.keyaliasName = "hegu";
                PlayerSettings.Android.keyaliasPass = "123456";

                //Android SDK 
                PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel16;
                PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel23;

                //Android target device
                PlayerSettings.Android.targetDevice = AndroidTargetDevice.ARMv7;

                // 设置标识符
                PlayerSettings.applicationIdentifier = "com.hegu.dnl.mg";//"com.hegu.dxcmy.joyland";//"com.hegu.dnl.huawei";//"com.hegu.dxcmytest.mg";//"com.hegu.dnl.huawei";//"com.hegu.dxcmy.joyland";//"com.hegu.dnl.huawei";//"com.hegu.dxcmy.joyland";//"com.mjsmz.dn.mnof";//"com.hegu.dxcmy.joyland";//"com.mjsmz.dn.mnof";//"com.hegu.dnl.mg";//"com.hegu.dxcmy4.m4399";//"com.hegu.dnl.meizu";//"com.hegu.dnl.oppo.nearme.gamecenter";//"com.hegu.dnl.sn79";//"com.hegu.dxcmytest.mg";//"com.hegu.dnf.test";//"com.hegu.dxcmytest.mg";//"com.hegu.dnl.mg";//"com.ali.dxcmy";//"com.hegu.dnl.oppo.nearme.gamecenter";//"com.hegu.dnl.huawei";"com.hegu.dnl.mg";


                PlayerSettings.bundleVersion = "1.0.0.77777";
                PlayerSettings.Android.bundleVersionCode = 100;

                PlayerSettings.companyName = "hegu";
                PlayerSettings.productName = "dnl_mg_other";//"dnl_joyland";//"dnl_new_all_huawei";//"dnl_Voice_testMG";


                // 充许调试 开发 外部修改（导出xcode或android工程）
                //BuildOptions options = BuildOptions.AllowDebugging | BuildOptions.Development | BuildOptions.AcceptExternalModificationsToPlayer;

                //编APK
                BuildOptions options = BuildOptions.AutoRunPlayer | BuildOptions.AllowDebugging | BuildOptions.Development;

                //
                
                //LOG_ERROR;BEHAVIAC_RELEASE;BEHAVIAC_NOT_USE_MONOBEHAVIOUR;DEBUG_SETTING
                // 添加宏
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "LOG_ERROR;BEHAVIAC_RELEASE;BEHAVIAC_NOT_USE_MONOBEHAVIOUR;DEBUG_SETTING;TEST_DEBUG_MG_OTHER;");//TEST_DEBUG;//LOG_PROCESS;//TEST_DEBUG_VERIFY_LOCAL

                // 添加场景
                string[] scenes ={
                                "Assets/Scenes/Main.unity",
                                "Assets/Scenes/Start.unity",
                                "Assets/Scenes/Town.unity",
                                "Assets/Scenes/DnH.unity",
                                "Assets/Scenes/GCEmpty.unity"
                            };

                // 检查输出路径
                //if (!System.IO.Directory.Exists(BuildPath))
                //{
                //    System.IO.Directory.CreateDirectory(BuildPath);
                //}

                if (!System.IO.Directory.Exists(BuildApkPath))
                {
                    System.IO.Directory.CreateDirectory(BuildApkPath);
                }


                System.DateTime nowTime = System.DateTime.Now;
                string buildApkName = nowTime.Year.ToString("D2") + "_" + nowTime.Month.ToString("D2") + nowTime.Day.ToString("D2") + "_" + nowTime.Hour.ToString("D2") + nowTime.Minute.ToString("D2") + "_" + Global.Settings.sdkChannel.ToString() + ".apk";
                string buildApkTotalPath = BuildApkPath + buildApkName;

                // 输出!
                //BuildPipeline.BuildPlayer(scenes, BuildPath, BuildTarget.Android, options);
                BuildPipeline.BuildPlayer(scenes, buildApkTotalPath, BuildTarget.Android, options);

            };
        }
        */
    }

    public class ImageToTextureGenerator : Singleton<ImageToTextureGenerator>
    {
        public Texture2D iconTex;
        public System.Action<Texture2D> loadIconCallback;

        private UnityEngine.Coroutine loadIconImageCor;
        public void StartLoadIconImage(string iconPath)
        {
            //if (loadIconImageCor != null)
            //{
            //    GameClient.GameFrameWork.instance.StopCoroutine(loadIconImageCor);
            //}
            //loadIconImageCor = GameClient.GameFrameWork.instance.StartCoroutine(LoadIconImage(iconPath));
        }

        IEnumerator LoadIconImage(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Logger.LogError("GameBuilder Load Icon failed , path is null");
                yield break;
            }
            WWW www = new WWW(path);
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                Logger.LogErrorFormat("GameBuilder Load Icon failed : {0}", www.error);
            }
            else
            {
                if (www.isDone)
                {
                    if (iconTex != null)
                    {
                        iconTex = null;
                    }
                    iconTex = new Texture2D(1024, 1024, TextureFormat.ARGB32, false, true);
                    www.LoadImageIntoTexture(iconTex);

                    if (loadIconCallback != null && iconTex != null)
                    {
                        Debug.LogError("www download texture name is " + iconTex.name);
                        loadIconCallback(iconTex);
                    }
                }
            }
        }

        public void DisposeTempIconTexture()
        {
            if (iconTex != null)
            {
                iconTex = null;
            }
        }
    }
}