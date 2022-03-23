using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using LitJson;

using System.Runtime.InteropServices;
using System;
using System.Reflection;

using System.ComponentModel;

[CustomEditor(typeof(Global))]
public class SDKChannelLocalInfoWindow : EditorWindow
{
    const string LocalInfoSavePath = "Resources/SDKChannelLocalInfo";
    static SDKChannelLocalInfo sdkChannelLocalInfo = null;

    //[MenuItem("GameBuilder/SDKChannelLocalInfoWindow")]
    public static void OpenSDKChannelLocalInfo()
    {
        Create();
        SDKChannelLocalInfoWindow window = (SDKChannelLocalInfoWindow)EditorWindow.GetWindow(typeof(SDKChannelLocalInfoWindow), true, "SDKChannaleLocalInfo", true);
        window.Show();
    }

    static void Create()
    {
        string resPath = PathUtil.EraseExtension(LocalInfoSavePath.Substring(10));

        sdkChannelLocalInfo = Resources.Load(resPath) as SDKChannelLocalInfo;

        Debug.Log(resPath);

        if (sdkChannelLocalInfo == null)
        {
            sdkChannelLocalInfo = FileTools.CreateAsset<SDKChannelLocalInfo>(resPath);
        }

        //自动选中对象
        EditorGUIUtility.PingObject(sdkChannelLocalInfo);
        Selection.activeObject = sdkChannelLocalInfo;
    }

}

namespace CustomGameBuild
{
	public enum DemoBuildPackType
    {
        [Description("阿拉德之怒安卓包内日志收集工具(update on 2019.03.27)")]
        [Remark("com.tm.dnl2.logtool.demo")]
        [RemarkFloat(13.0f)]
        [RemarkInt(17)]
        [RemarkStringArray(new string[]{"Scenes/CrashLogCollectTool.unity"})]
        [RemarkString("logtool")]
        [RemarkBoolean(true)] //标识是否按渠道分
        LogCollector = 0,

        [Description("阿拉德之怒安卓资源热更新地址工具(update on 2019.03.27)")]
        [Remark("com.test.steset.value")]
        [RemarkFloat(9.5f)]
        [RemarkInt(95)]
        [RemarkStringArray(new string[]{"Scenes/HotfixDebugTestTool.unity"})]
        [RemarkString("hotfix")]
        [RemarkBoolean(false)] //标识是否按渠道分
        HotfixTool,

        [Description("阿拉德之怒_Ping网络环境检测工具(update on 2019.03.27)")]
        [Remark("com.tm.dnl2.pingtool")]
        [RemarkFloat(1.0f)]
        [RemarkInt(1)]
        [RemarkStringArray(new string[]{"Scenes/PingIPLogCollectTool.unity"})]
        [RemarkString("pingtool")]
        [RemarkBoolean(false)] //标识是否按渠道分
        PingTool,

        [Description("乐变SDK_1892测试Demo(update on 2019.03.27)")]
        [Remark("com.tm.dnl2.lebiansdk.m1892.demo")]
        [RemarkFloat(1.0f)]
        [RemarkInt(1)]
        [RemarkStringArray(new string[]{"Scenes/Lebian/LebianSDKDemoTest_1.unity"})]
        [RemarkString("lebian")]
        [RemarkBoolean(false)] //标识是否按渠道分
        LebianSDKTest1892,

        [Description("本地推送Demo(update on 2019.04.11)")]
        [Remark("com.tm.dnl2.localpush.demo")]
        [RemarkFloat(1.0f)]
        [RemarkInt(1)]
        [RemarkStringArray(new string[]{"Scenes/SimpleLocalPush/SimpleLocalPush_1.unity"})]
        [RemarkString("localpush")]
        [RemarkBoolean(false)] //标识是否按渠道分
        LocalPushDemo,

        [Description("安卓动态权限测试(update on 2019.07.22)")]
        [Remark("com.tm.dnl2.reqpermission.demo")]
        [RemarkFloat(1.0f)]
        [RemarkInt(1)]
        [RemarkStringArray(new string[] { "Scenes/PermissionRequest/RequestPermissions_1.unity" })]
        [RemarkString("reqpermission")]
        [RemarkBoolean(false)] //标识是否按渠道分
        ReqPermissionDemo,


        [Description("SDK渠道测试(update on 2019.08.11)")]
        [Remark("com.tm.dnl2.channel.demo")]
        [RemarkFloat(1.0f)]
        [RemarkInt(1)]
        [RemarkStringArray(new string[] { "Scenes/SDKChannel/SDKChannelScene_1.unity" })]
        [RemarkString("sdkchannel")]
        [RemarkBoolean(true)] //标识是否按渠道分
        AndroidChannelTest,
    }

    public class Messagebox
    {
        [DllImport("User32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern int MessageBox(IntPtr handle, String message, String title, int type);
    }

    [CustomEditor(typeof(Global))]
    public class GloablSettingWindow : EditorWindow
    {
        //[MenuItem("GameBuilder/GloablSettingWindow")]
        public static void OpenGlobalSetting()
        {
            Create();
            GloablSettingWindow window = (GloablSettingWindow)EditorWindow.GetWindow(typeof(GloablSettingWindow), true, "GlobalSetting", true);
            window.Show();
        }

        static Global global = null;
        const string GLOBAL_PATH = "Resources/Global";
        static void Create()
        {
            string resPath = PathUtil.EraseExtension(GLOBAL_PATH.Substring(10));

            global = Resources.Load(resPath) as Global;

            Debug.Log(resPath);

            if (global == null)
            {
                global = FileTools.CreateAsset<Global>(resPath);
            }

            //自动选中对象
            EditorGUIUtility.PingObject(global);
            Selection.activeObject = global;
        }
    }

    
    [CustomEditor(typeof(BuilderParams))]
    public class GameBuilderWindow : EditorWindow 
    {
        [MenuItem("GameBuilder/GameBuilderWindow")]
        public static void OpenGameBuilder()
        {
            Create();

            GameBuilderWindow window = (GameBuilderWindow)EditorWindow.GetWindow(typeof(GameBuilderWindow), true, "GameBuilder", true);
            window.InitParams();
            window.Show();
        }

        #region Params

        const string GameBuildPath = "Resources/GameBuildParams";

        static BuilderParams builderParams;
        static GUIStyle builderGUIStyle = new GUIStyle();

        // string versionConfigPath = "";
        // string versionJsonPath = "";
        // string sdkConfigPath = "";

        string buildToFilePath = "";
        string buildToFileName = "";

        protected static SerializedObject _currSerializedObject;
        protected static SerializedProperty _buildOptionsProperty;
        protected static SerializedProperty _buildGameScenesProperty;
        protected static SerializedProperty _buildSDKChannelCommonLibsProperty;

        private static GUIContent
            duplicateBtnContent = new GUIContent("+", "duplicate"),
            deleteBtnContent = new GUIContent("-", "delete"),
            addBtnContent = new GUIContent("+", "add");

        private static GUILayoutOption miniBtnWidth = GUILayout.Width(50f);


        private bool isPlatformSupport = false;
        private bool beDirty1 = false;

        private BuildTargetGroup currSelectBuildTargetGroup = BuildTargetGroup.Standalone;

        private DemoBuildPackType currSelectBuildPackType = DemoBuildPackType.LogCollector;

        private List<string> sdkCopyDirSDKChannelTypes = new List<string>() { "NONE" };
        //增加渠道通用包
        public List<string> sdkCopyDirSDKChannelCommon = new List<string>();        

        //通用库包

        private int currSelectSDKChannelTypeIndex = 0;
        //private SDKChannel currSelectSDKChannel = SDKChannel.None;
        //private string currSelectSDKChannelType = "NONE";
        // private string[] scriptDefineTypes = new string[(int)DemoBuildPackType.Count];
        // private int scriptDefineTypeIndex = 0;
        // private int tempScriptDefineTypeIndex = 0;

        private string appAndroidPluginPath = "";
        private string sdkAndroidCopyPath = "";
        private string sdkChannelCommonCopyPath = "";

        private float currBundleVersion = 0.0f;
        private int currBundleVersionCode = 0;

        private static string SDK_COPY_DIR_RELATIVE_PATH = "./../BuildTools_SDKCopyFiles/";
        private static string SDK_KEYSTORE_DIR_RELATIVE_PATH = "./../BuildTools_keystore/";


        //public const string SDK_CHANNEL_CONFIG_FILENAME = "sdkchannel.conf";
        //private static SDKClient.SDKChannelInfo sdkChannelInfo = null;
        //private bool isLoadedSDKChannelConfig = false;

        #endregion

        static void Create()
        {
            string resPath = PathUtil.EraseExtension(GameBuildPath.Substring(10));

            Debug.Log(resPath);

            if(string.IsNullOrEmpty(resPath))
            {
                Debug.Log("builderParams res path is empty : "+GameBuildPath);
                builderParams = FileTools.CreateAsset<BuilderParams>(GameBuildPath);
            }

            builderParams = Resources.Load(resPath) as BuilderParams;

            if (builderParams == null)
            {
                Debug.Log("builderParams is null");
                builderParams = FileTools.CreateAsset<BuilderParams>(GameBuildPath);
            }

            //自动选中对象
            EditorGUIUtility.PingObject(builderParams);
            Selection.activeObject = builderParams;
        }

        //private static byte[] _LoadConfigData()
        //{
        //    byte[] data = null;
        //    try
        //    {
        //        if (!FileArchiveAccessor.LoadFileInLocalFileArchive(SDK_CHANNEL_CONFIG_FILENAME, out data))
        //        {
        //            Debug.LogErrorFormat("can not load file in streamingPath: sdkchannel.conf");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.LogErrorFormat("error : {0}", e.ToString());
        //    }

        //    return data;
        //}

        //private static void _LoadSDKChannelConfig()
        //{
        //    try
        //    {
        //        byte[] data = _LoadConfigData();
        //        if (null != data)
        //        {
        //            string content = System.Text.ASCIIEncoding.Default.GetString(data);
        //            if (string.IsNullOrEmpty(content))
        //            {
        //                Debug.LogError("load config content is null 111");
        //            }
        //            else
        //            {
        //                sdkChannelInfo = LitJson.JsonMapper.ToObject<SDKClient.SDKChannelInfo>(content);
        //                if (sdkChannelInfo == null)
        //                {
        //                    Debug.LogError("load config content is null 222");
        //                    sdkChannelInfo = new SDKClient.SDKChannelInfo();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.LogErrorFormat("error : {0}", e.ToString());
        //    }
        //}

        void Awake()
        {
            //versionConfigPath = Application.streamingAssetsPath + "/version.config";
            //versionJsonPath = Application.streamingAssetsPath + "/version.json";
            //sdkConfigPath = Application.streamingAssetsPath + "/sdk.conf";

            // if(scriptDefineTypes != null)
            // {
            //     for (int i = 0; i < scriptDefineTypes.Length; i++)
            //     {
            //         scriptDefineTypes[i] = EnumExtension.GetDescriptionByName((DemoBuildPackType)i);
            //     }
            // }

            //这里还能加其他支持的平台
#if UNITY_ANDROID
            isPlatformSupport = true;

            //appAndroidPluginPath = Path.Combine(Application.dataPath, "Plugins/Android/");
            appAndroidPluginPath = Application.dataPath;
#endif
            beDirty1 = false;
        }

        void OnEnable()
        {
            if(builderParams == null)
            {
                return;
            }
            if (_currSerializedObject == null)
            {
                _currSerializedObject = new SerializedObject(builderParams);
                if (_currSerializedObject != null)
                {
                    _buildOptionsProperty = _currSerializedObject.FindProperty("buildOptions");
                    _buildGameScenesProperty = _currSerializedObject.FindProperty("gameScenes");
                    _buildSDKChannelCommonLibsProperty = _currSerializedObject.FindProperty("sdkChannelCommonLibs");
                }
            }
        }

        void OnDisable()
        {

        }        

        void OnGUI()
        {
            if (builderParams == null)
                return;

            if (!isPlatformSupport)
            {
                if (!beDirty1)
                {
                    beDirty1 = true;
                    Messagebox.MessageBox(IntPtr.Zero, "当前平台不支持该功能，\n请转到相应平台", "提示", 0);
                }
                return;
            }

            //if (Application.platform != RuntimePlatform.Android)
            //{
            //    Logger.LogError("当前设备平台不是Android");
            //    return;
            //}

            EditorGUILayout.LabelField("说明 ###这是本地打Android的配置参数清单###");
            EditorGUILayout.Space();
            EditorGUILayout.TextField("Android SDK拷贝目录在这里(注：也可以用2.0sdk工程下的拷贝目录) http://192.168.2.61:8080/svn/DNF_Project_2_0/Program/BuildTools/SDKCopyFilesByChannel/Android");
            EditorGUILayout.TextField("Android 签名文件目录在这里 http://192.168.2.61:8080/svn/DNF_Project_2_0/Program/BuildTools/keystore");
            EditorGUILayout.LabelField("注意 执行下面操作时先备份本地 Plugin/Android目录 然后将对应SDK的 Android目录覆盖替换 Plugin/Android目录");
            EditorGUILayout.Space();

			Rect totalVertical_1 = EditorGUILayout.BeginVertical();
            //builderParams.debugModeEnabled = EditorGUILayout.Toggle("调试模式是否开启(日志的输出等)", builderParams.debugModeEnabled);
            //builderParams.isSDKDebug = EditorGUILayout.Toggle("SDK调试模式", builderParams.isSDKDebug);
            //builderParams.isSDKPayDebug = EditorGUILayout.Toggle("SDK支付调试模式", builderParams.isSDKPayDebug);

            // Rect horizontal_1 = EditorGUILayout.BeginHorizontal();
            // builderParams.sdkConfigPath = EditorGUILayout.TextField("SDK_Config文件路径（默认）", builderParams.sdkConfigPath);
            // if (GUILayout.Button("选择渠道配置文件"))
            // {
            //     string sdkConfigPath = EditorUtility.OpenFilePanel("Select a sdk.conf for building", "", "conf");
            //     if (sdkConfigPath.Length != 0)
            //     {
            //         builderParams.sdkConfigPath = sdkConfigPath;

            //     }
            // }
            // EditorGUILayout.EndHorizontal();

            builderParams.buildPackType = (DemoBuildPackType)EditorGUILayout.EnumPopup("Build Pack Type", builderParams.buildPackType);
            if(currSelectBuildPackType != builderParams.buildPackType)
            {
                InitParams();
                EditorUtility.SetDirty(builderParams);
                currSelectBuildPackType = builderParams.buildPackType;
            }

            //是否需要按渠道分开打包
            if(builderParams.buildPackType == DemoBuildPackType.LogCollector ||
                builderParams.buildPackType == DemoBuildPackType.AndroidChannelTest)
            {
                //Rect horizontal_33 = EditorGUILayout.BeginHorizontal();
                //builderParams.sdkChannel = (SDKChannel)EditorGUILayout.EnumPopup("SDK Channel", builderParams.sdkChannel);
                int tempSelectSDKChannelIndex = EditorGUILayout.Popup("SDK Channel Type", currSelectSDKChannelTypeIndex, sdkCopyDirSDKChannelTypes.ToArray());                
                //if (GUILayout.Button("刷新"))
                //if(currSelectSDKChannel != builderParams.sdkChannel)
                if (currSelectSDKChannelTypeIndex != tempSelectSDKChannelIndex)
                {
                    builderParams.sdkChannelType = sdkCopyDirSDKChannelTypes[tempSelectSDKChannelIndex];

                    Global gloabl = AssetDatabase.LoadAssetAtPath<Global>("Assets/" + GlobalSetting.GLOBAL_PATH + ".asset");
                    if (gloabl != null)
                    {
                        //gloabl.sdkChannel = builderParams.sdkChannel;
                        gloabl.sdkChannelType = builderParams.sdkChannelType;
                        EditorUtility.SetDirty(gloabl);
                        AssetDatabase.SaveAssets();
                    }                    
                    InitParams();
                    EditorUtility.SetDirty(builderParams);
                    //currSelectSDKChannel = builderParams.sdkChannel;
                    //currSelectSDKChannelType = builderParams.sdkChannelType;
                    currSelectSDKChannelTypeIndex = tempSelectSDKChannelIndex;
                }

                _ShowArrayElemnets(_buildSDKChannelCommonLibsProperty);

                //if (_currSerializedObject != null)
                //{
                //    if (_buildSDKChannelCommonLibsProperty != null)
                //    {
                //        EditorGUILayout.Space();
                //        EditorGUI.BeginChangeCheck();
                //        EditorGUILayout.PropertyField(_buildSDKChannelCommonLibsProperty, true);

                //        if (EditorGUI.EndChangeCheck())
                //        {
                //            _currSerializedObject.ApplyModifiedProperties();
                //        }
                //        EditorGUILayout.Space();
                //    }
                //}

                //EditorGUILayout.EndHorizontal();
            }
            else
            {
                //if(builderParams.sdkChannel != SDKChannel.None)
                if (builderParams.sdkChannelType != "NONE")
                {
                    InitParams();
                    EditorUtility.SetDirty(builderParams);
                    //currSelectSDKChannel = builderParams.sdkChannel = SDKChannel.None;
                    //currSelectSDKChannelType = builderParams.sdkChannelType = "NONE";
                    currSelectSDKChannelTypeIndex = 0;
                }
            }
            
            EditorGUILayout.Space();
            builderParams.buildToPath = EditorGUILayout.TextField("App build to path:", builderParams.buildToPath);
            builderParams.buildToFileName = EditorGUILayout.TextField("App build to name:", builderParams.buildToFileName);

            EditorGUILayout.Space();
            builderParams.buildPlatform = (BuildTargetGroup)EditorGUILayout.EnumPopup("App build platform", builderParams.buildPlatform);
            if (currSelectBuildTargetGroup != builderParams.buildPlatform)
            {
                _ReadSDKCopyDirChannelTypes();
                InitParams();
                currSelectBuildTargetGroup = builderParams.buildPlatform;
            }

            builderParams.buildType = (ScriptingImplementation)EditorGUILayout.EnumPopup("App build type", builderParams.buildType);

            EditorGUILayout.Space();
            if(builderParams.buildPackType == DemoBuildPackType.LogCollector ||
                builderParams.buildPackType == DemoBuildPackType.AndroidChannelTest)
            {
                //builderParams.appIdentifier = string.Format(EnumExtension.GetRemark<DemoBuildPackType>(builderParams.buildPackType), EnumExtension.GetRemark<SDKChannel>(builderParams.sdkChannel));
                string appIdentifierHead = EnumExtension.GetRemark<DemoBuildPackType>(builderParams.buildPackType);
                builderParams.appIdentifier = string.Format("{0}.{1}",appIdentifierHead, builderParams.sdkChannelType.ToString().ToLower());
            }
            else
            {
                builderParams.appIdentifier = EnumExtension.GetRemark<DemoBuildPackType>(builderParams.buildPackType);
            }            
            builderParams.appIdentifier = EditorGUILayout.TextField("Application identifier:", builderParams.appIdentifier);

            builderParams.companyName = EditorGUILayout.TextField("Application companyName:", builderParams.companyName);

            builderParams.productName = GetBuildPackName();
            builderParams.productName = EditorGUILayout.TextField("Application productName:", builderParams.productName);

            EditorGUI.BeginChangeCheck();
            builderParams.bundleVersion = EditorGUILayout.TextField("Application bundleVersion:", builderParams.bundleVersion);
            if(float.TryParse(builderParams.bundleVersion, out currBundleVersion))
            {
                if(currBundleVersion < EnumExtension.GetRemarkFloat<DemoBuildPackType>(builderParams.buildPackType))
                {
                    currBundleVersion = EnumExtension.GetRemarkFloat<DemoBuildPackType>(builderParams.buildPackType);
                    builderParams.bundleVersion = currBundleVersion.ToString("f1");                    
                }           
            }
            if(EditorGUI.EndChangeCheck())
            {
                InitParams();
            }


            EditorGUILayout.Space();            
            builderParams.bundleVersionCode = EditorGUILayout.IntField("AndroidApp VersionCode:", builderParams.bundleVersionCode);
            if(builderParams.bundleVersionCode < EnumExtension.GetRemarkInt<DemoBuildPackType>(builderParams.buildPackType))
            {
                currBundleVersionCode = builderParams.bundleVersionCode = EnumExtension.GetRemarkInt<DemoBuildPackType>(builderParams.buildPackType);
            }       

            Rect horizontal_2 = EditorGUILayout.BeginHorizontal();
            builderParams.keystoreName = EditorGUILayout.TextField("AndroidApp keystoreName:", builderParams.keystoreName);
            if (GUILayout.Button("选择签名文件"))
            {
                string keyStorePath = EditorUtility.OpenFilePanel("Select a keystore for android", "", "keystore");
                if (keyStorePath.Length != 0)
                {
                    builderParams.keystoreName = keyStorePath;
                }
            }
            EditorGUILayout.EndHorizontal();

#if UNITY_ANDROID
            builderParams.keystorePass = EditorGUILayout.TextField("AndroidApp keystorePass:", builderParams.keystorePass);
            builderParams.keyaliasName = EditorGUILayout.TextField("AndroidApp keyaliasName:", builderParams.keyaliasName);
            builderParams.keyaliasPass = EditorGUILayout.TextField("AndroidApp keyaliasPass:", builderParams.keyaliasPass);
            builderParams.forceSDCard = EditorGUILayout.Toggle("AndroidApp forceSDCard:", builderParams.forceSDCard);
            builderParams.preferInstall = (AndroidPreferredInstallLocation)EditorGUILayout.EnumPopup("AndroidApp installlocaltion:", builderParams.preferInstall);
            builderParams.minSdkVersion = (AndroidSdkVersions)EditorGUILayout.EnumPopup("AndroidApp minSdk:", builderParams.minSdkVersion);
            builderParams.targetSdkVersion = (AndroidSdkVersions)EditorGUILayout.EnumPopup("AndroidApp targetSdk:", builderParams.targetSdkVersion);
            builderParams.targetDevice = (AndroidArchitecture)EditorGUILayout.EnumPopup("AndroidApp targetDevice:", builderParams.targetDevice);
#endif

            _ShowArrayElemnets(_buildOptionsProperty);
            _ShowArrayElemnets(_buildGameScenesProperty);

            //if (_currSerializedObject != null)
            //{
            //    if (_buildOptionsProperty != null && _buildGameScenesProperty != null)
            //    {
            //        EditorGUILayout.Space();
            //        EditorGUI.BeginChangeCheck();

            //        EditorGUILayout.PropertyField(_buildOptionsProperty, true);
            //        EditorGUILayout.PropertyField(_buildGameScenesProperty, true);
            //        if (EditorGUI.EndChangeCheck())
            //        {
            //            _currSerializedObject.ApplyModifiedProperties();
            //        }
            //        EditorGUILayout.Space();
            //    }
            //}

            //Rect horizontal_3 = EditorGUILayout.BeginHorizontal();
            
            builderParams.scriptDefines = EditorGUILayout.TextField("Application scriptDefines:", builderParams.scriptDefines);

            // scriptDefineTypeIndex = EditorGUILayout.Popup(scriptDefineTypeIndex, scriptDefineTypes, new GUILayoutOption[] { GUILayout.MaxWidth(100f) });
            // if(tempScriptDefineTypeIndex != scriptDefineTypeIndex)
            // {                
            //     builderParams.scriptDefines = GetFilterScriptDefine((DemoBuildPackType)scriptDefineTypeIndex);
            //     tempScriptDefineTypeIndex = scriptDefineTypeIndex;
            // }            

            //EditorGUILayout.EndHorizontal();

            builderParams.showIcon = (Texture2D)EditorGUILayout.ObjectField("ICON", builderParams.showIcon, typeof(Texture2D), false);
            EditorGUILayout.Space();            

            if(GUILayout.Button("开始打包吧"))
            {
                EditorUtility.SetDirty(builderParams);
                AssetDatabase.SaveAssets();

                ApplySettings();
            }

            EditorGUILayout.EndVertical();
        }

        // string GetFilterScriptDefine(DemoBuildPackType scriptDefineType)
        // {            
        //     string orginalBuilderScriptDefines = builderParams.scriptDefines;
        //     for (int i = 0; i < (int)DemoBuildPackType.Count; i++)
        //     {                
        //         var type = (DemoBuildPackType)i;
        //         string typeDefine = EnumExtension.GetRemark(type);
        //         if(type == scriptDefineType && typeDefine != "")
        //         {
        //             if(orginalBuilderScriptDefines == "")
        //             {
        //                 orginalBuilderScriptDefines += string.Format("{0}", typeDefine);
        //             }
        //             else
        //             {
        //                 orginalBuilderScriptDefines = orginalBuilderScriptDefines.Replace(typeDefine, "");                        

        //                 orginalBuilderScriptDefines += string.Format(";{0}", typeDefine);
        //             }
        //             orginalBuilderScriptDefines = orginalBuilderScriptDefines.Replace(";;", ";");
        //             continue;
        //         }
        //         if(typeDefine != "")
        //         {
        //             orginalBuilderScriptDefines = orginalBuilderScriptDefines.Replace(typeDefine, "");
        //         }
        //         orginalBuilderScriptDefines = orginalBuilderScriptDefines.Replace(";;", ";");
        //     }   
        //     return orginalBuilderScriptDefines;         
        // }


        private static void _ShowArrayElemnets(SerializedProperty list)
        {
            if (list == null)
            {
                return;
            }
            if (_currSerializedObject != null)
            {                
                EditorGUILayout.Space();
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.LabelField(list.name);
                if (list.arraySize > 0)
                {
                    for (int i = 0; i < list.arraySize; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
                        ShowButtonsInArrayElement(list, i);
                        EditorGUILayout.EndHorizontal();
                    }
                }
                if (EditorGUI.EndChangeCheck())
                {
                    _currSerializedObject.ApplyModifiedProperties();
                }
                EditorGUILayout.Space();

                if (list.arraySize == 0 &&
                    GUILayout.Button(addBtnContent, EditorStyles.miniButton))
                {
                    list.arraySize += 1;
                }
            }
        }

        private static void ShowButtonsInArrayElement(SerializedProperty list, int index)
        {
            if (list == null)
            {
                return;
            }
            if (GUILayout.Button(duplicateBtnContent, EditorStyles.miniButtonLeft, miniBtnWidth))
            {
                list.InsertArrayElementAtIndex(index);
            }
            if (GUILayout.Button(deleteBtnContent, EditorStyles.miniButtonRight, miniBtnWidth))
            {
                int oldSize = list.arraySize;
                list.DeleteArrayElementAtIndex(index);
                if (list.arraySize == oldSize)
                {
                    list.DeleteArrayElementAtIndex(index);
                }
            }
        }

        public static void BuildTargets()
        {
            BuildOptions[] optionArray = builderParams.buildOptions;
            BuildOptions options = BuildOptions.None;
            if (optionArray != null)
            {
                for (int i = 0; i < optionArray.Length; i++)
                {
                    options |= optionArray[i];
                }
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(builderParams.buildPlatform, builderParams.scriptDefines);
            string[] scenes = builderParams.gameScenes;
            if (scenes == null || scenes.Length == 0)
            {
                Logger.LogError("打包时未选择场景！！！");
                return;
            }
            try
            {
                BuildPipeline.BuildPlayer(scenes,
                                        Path.Combine(builderParams.buildToPath, builderParams.buildToFileName),
                                        UnityEditor.BuildTarget.Android,
                                        options);
            }
            catch (Exception e)
            {

            }
            finally
            {
                DeleteCurrCopyFilesAndDirs();
            }
        }

        void Update()
        {

        }

        void Focus()
        {
            Debug.Log("当窗口获得焦点时调用一次");
            this.Repaint();
        }

        void OnLostFocus()
        {
            Debug.Log("当窗口失去焦点时调用一次");
        }

        void OnHierarchyChange()
        {
            Debug.Log("当hierarchy视图中的资源发生改变时调用一次");
            this.Repaint();
        }

        void OnProjectChange()
        {
            Debug.Log("当project视图中的资源发生改变时调用一次");
            this.Repaint();
        }

        void OnInspectorUpdate()
        {
            //Debug.Log("窗口面板更新了");
            this.Repaint();
        }

        void OnDestroy()
        {
            Debug.Log("当窗口关闭时调用");
        }

        void InitParams()
        {
            string exportPath_1 = GetBuildPackName();                        
            if(string.IsNullOrEmpty(exportPath_1))
            {
                 buildToFilePath = "./exportapk/";
            }else
            {
                bool isSplitChannel = EnumExtension.GetRemarkBoolean<DemoBuildPackType>(builderParams.buildPackType);
                if (isSplitChannel)
                {
                    //string exportPath_2 = EnumExtension.GetDescriptionByName<SDKChannel>(builderParams.sdkChannel);
                    string exportPath_2 = string.Format("{0}渠道", builderParams.sdkChannelType.ToString());
                    buildToFilePath = string.Format("./exportapk/{0}/{1}/", exportPath_1, exportPath_2);
                }
                else
                {
                    buildToFilePath = string.Format("./exportapk/{0}/", exportPath_1);
                }
            }
            
            if (!System.IO.Directory.Exists(buildToFilePath))
            {
                System.IO.Directory.CreateDirectory(buildToFilePath);
            }

            System.DateTime nowTime = System.DateTime.Now;
            buildToFileName = string.Format("{0}_{1}_{2}_v{3}_{4}.apk",
                   nowTime.Year.ToString("D2") + nowTime.Month.ToString("D2") + nowTime.Day.ToString("D2"),
                   nowTime.Hour.ToString("D2") + nowTime.Minute.ToString("D2") + nowTime.Second.ToString("D2"),
                   builderParams.buildPackType.ToString(),
                   builderParams.bundleVersion,
                   //builderParams.sdkChannel.ToString()
                   builderParams.sdkChannelType
                );
            builderParams.buildToFileName = buildToFileName;
            builderParams.buildToPath = buildToFilePath;
            // string currSdkChannel =  EnumExtension.GetRemark<SDKChannel>(builderParams.sdkChannel);
            
            // if(currSdkChannel.Contains("com.hegu.dxcmytest.mg"))
            // {
            //     builderParams.appIdentifier = "com.hegu.dxcmytest.mg.logtool";
            // }else
            // {
            //     builderParams.appIdentifier = currSdkChannel.Replace(".dnl.", ".dnl.logtool.");
            // }

            //builderParams.sdkConfigPath = sdkConfigPath;
            //builderParams.appIdentifier = "com.tm.dnl2.test";
            //builderParams.bundleVersion = "1.0.0.77777";
            //builderParams.companyName = "hegu";
            //builderParams.productName = "测试包";
            //builderParams.scriptDefines = "LOG_ERROR;BEHAVIAC_RELEASE;BEHAVIAC_NOT_USE_MONOBEHAVIOUR;DEBUG_SETTING;TEST_DEBUG_MG_OTHER;";

            //builderParams.bundleVersionCode = 0;
            //builderParams.keystoreName = "E:/WorkSpace/SDKChannel/keystore/hegu.keystore";
            //builderParams.keystorePass = "123456";
            //builderParams.keyaliasName = "hegu";
            //builderParams.keyaliasPass = "123456";
#if UNITY_ANDROID
            builderParams.forceSDCard = true;
            builderParams.preferInstall = AndroidPreferredInstallLocation.Auto;
            builderParams.minSdkVersion = AndroidSdkVersions.AndroidApiLevel16;
            //builderParams.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            builderParams.targetDevice = AndroidArchitecture.ARMv7;
            //builderParams.buildType = ScriptingImplementation.Mono2x;
            builderParams.buildPlatform = BuildTargetGroup.Android;

            if (builderParams.buildPackType == DemoBuildPackType.AndroidChannelTest)
            {
                sdkAndroidCopyPath = string.Format("{0}{1}/{2}", SDK_COPY_DIR_RELATIVE_PATH, builderParams.buildPlatform.ToString(), builderParams.sdkChannelType);
                sdkChannelCommonCopyPath = string.Format("{0}{1}", SDK_COPY_DIR_RELATIVE_PATH, builderParams.buildPlatform.ToString());
            }
            else
            {
                sdkAndroidCopyPath =  "./SDKCopyFiles/" + EnumExtension.GetRemarkString<DemoBuildPackType>(builderParams.buildPackType);
            }
#endif


            if(builderParams.showIcon == null)
            {
                var textures = PlayerSettings.GetIconsForTargetGroup(builderParams.buildPlatform, IconKind.Any);  //！！！ 注意 IconKind 类型注意设置 ！！！ 
                if(textures != null && textures.Length > 0)
                {
                    builderParams.showIcon = textures[0];
                }                
            }

            var editorBuildSettingsScenes = EnumExtension.GetRemarkStringArray<DemoBuildPackType>(builderParams.buildPackType);
            if(editorBuildSettingsScenes != null)
            {
                builderParams.gameScenes = new string[editorBuildSettingsScenes.Length];
                for (int i = 0; i < editorBuildSettingsScenes.Length; i++)
                {
                    var settingsScene = editorBuildSettingsScenes[i];
                    if(settingsScene == "")
                    {
                        continue;
                    }
                    builderParams.gameScenes[i] = "Assets/" + settingsScene;
                }
            }

            
            if(float.TryParse(builderParams.bundleVersion, out currBundleVersion))
            {
                if(currBundleVersion < EnumExtension.GetRemarkFloat<DemoBuildPackType>(builderParams.buildPackType))
                {
                    currBundleVersion = EnumExtension.GetRemarkFloat<DemoBuildPackType>(builderParams.buildPackType);
                    builderParams.bundleVersion = currBundleVersion.ToString("f1");
                }
            }
            if(builderParams.bundleVersionCode < EnumExtension.GetRemarkInt<DemoBuildPackType>(builderParams.buildPackType))
            {
                currBundleVersionCode = builderParams.bundleVersionCode = EnumExtension.GetRemarkInt<DemoBuildPackType>(builderParams.buildPackType);
            }

            if (string.IsNullOrEmpty(builderParams.keystoreName))
            {
                builderParams.keystoreName = Path.Combine(SDK_KEYSTORE_DIR_RELATIVE_PATH, "hegu.keystore");
            }

            if (_currSerializedObject != null)
            {
                _currSerializedObject.UpdateIfRequiredOrScript();
            }            
        }

        private void _ReadSDKCopyDirChannelTypes()
        {
            string sdk_copy_dir_path = SDK_COPY_DIR_RELATIVE_PATH + builderParams.buildPlatform.ToString();
            if (builderParams.buildPlatform.ToString() == "iPhone")
            {
                sdk_copy_dir_path = SDK_COPY_DIR_RELATIVE_PATH + "iOS";
            }
            sdkCopyDirSDKChannelTypes = new List<string>() { "NONE" };
            foreach (var d in Directory.GetDirectories(sdk_copy_dir_path))
            {
                var dirInfo = new DirectoryInfo(d);
                var dirName = dirInfo.Name;

                //添加渠道通用包
                if (dirName.StartsWith("CL_"))
                {
                    sdkCopyDirSDKChannelCommon.Add(dirName);
                    continue;
                }

                sdkCopyDirSDKChannelTypes.Add(dirName);
            }
            currSelectSDKChannelTypeIndex = sdkCopyDirSDKChannelTypes.IndexOf(builderParams.sdkChannelType);

            if (builderParams.sdkChannelCommonLibs == null || builderParams.sdkChannelCommonLibs.Length <= 0 || builderParams.sdkChannelCommonLibs[0] == "")
            {
                builderParams.sdkChannelCommonLibs = sdkCopyDirSDKChannelCommon.ToArray();
            }
        }

        void ApplySettings()
        {
            if (builderParams != null)
            {
                if (string.IsNullOrEmpty(builderParams.keystoreName)
                    || string.IsNullOrEmpty(builderParams.keyaliasName)
                    || string.IsNullOrEmpty(builderParams.keystorePass)
                    || string.IsNullOrEmpty(builderParams.keyaliasPass))
                {
                    EditorUtility.DisplayDialog("Notice!", "Please Select android keystore !","Let's Go");
                    return;
                }

                //GlobalSetting settings = AssetDatabase.LoadAssetAtPath<GlobalSetting>("Assets/Resources/" + Global.PATH + ".asset");
                //if (settings != null)
                //{
                //    settings.sdkChannel = builderParams.sdkChannel;
                //    settings.isPaySDKDebug = builderParams.isSDKPayDebug;
                //    settings.isDebug = builderParams.isSDKDebug;
                //}               
                //WriteInSDKConfig();

                if (!builderParams.buildPlatform.ToString().Equals(EditorUserBuildSettings.activeBuildTarget.ToString()))
                {
                    EditorUserBuildSettings.SwitchActiveBuildTarget(builderParams.buildPlatform, GetBuildTargetByGroup(builderParams.buildPlatform));
                }

                PlayerSettings.SetScriptingBackend(builderParams.buildPlatform, builderParams.buildType);
                if(builderParams.showIcon != null)
                {
                    PlayerSettings.SetIconsForTargetGroup(builderParams.buildPlatform, 
                        new Texture2D[] { builderParams.showIcon, builderParams.showIcon, builderParams.showIcon, builderParams.showIcon, builderParams.showIcon, builderParams.showIcon }, 
                        IconKind.Any);
                }

                PlayerSettings.applicationIdentifier = builderParams.appIdentifier;

                //为各平台设备
                //PlayerSettings.SetApplicationIdentifier();

                PlayerSettings.bundleVersion = builderParams.bundleVersion;
                if (!string.IsNullOrEmpty(builderParams.companyName))
                {
                    PlayerSettings.companyName = builderParams.companyName;
                }
                PlayerSettings.productName = builderParams.productName;

                BuildOptions[] optionArray = builderParams.buildOptions;
                BuildOptions options = BuildOptions.None;
                if (optionArray != null)
                {
                    for (int i = 0; i < optionArray.Length; i++)
                    {
                        options |= optionArray[i];
                    }
                }


#if UNITY_ANDROID

                PlayerSettings.Android.forceSDCardPermission = builderParams.forceSDCard;
                PlayerSettings.Android.preferredInstallLocation = builderParams.preferInstall;
                PlayerSettings.Android.keystoreName = builderParams.keystoreName;
                PlayerSettings.Android.keystorePass = builderParams.keystorePass;
                PlayerSettings.Android.keyaliasName = builderParams.keyaliasName;
                PlayerSettings.Android.keyaliasPass = builderParams.keyaliasPass;
                PlayerSettings.Android.minSdkVersion = builderParams.minSdkVersion;
                PlayerSettings.Android.targetSdkVersion = builderParams.targetSdkVersion;
                PlayerSettings.Android.targetArchitectures = builderParams.targetDevice;
                PlayerSettings.Android.bundleVersionCode = builderParams.bundleVersionCode;

#endif

                PlayerSettings.SetScriptingDefineSymbolsForGroup(builderParams.buildPlatform, builderParams.scriptDefines);
                string[] scenes = builderParams.gameScenes;
                if (scenes == null || scenes.Length == 0)
                {
                    Logger.LogError("打包时未选择场景！！！");
                    return;
                }

                //先拷贝功能sdk，再拷贝公共库！！！
                //sdk copy
                if (builderParams.sdkChannelCommonLibs != null)
                {
                    foreach (var item in builderParams.sdkChannelCommonLibs)
                    {
                        DirectoryCopy(Path.Combine(sdkChannelCommonCopyPath, item), appAndroidPluginPath, true, true, true);
                    }
                }

                DirectoryCopy(sdkAndroidCopyPath, appAndroidPluginPath, true, true, true);

                AssetDatabase.Refresh();
            
                //WriteInVersionConfig(builderParams.bundleVersion);
                //WriteInVersionJson(builderParams.bundleVersion, GetBuildTargetByGroup(builderParams.buildPlatform));

                try
                {
                    BuildPipeline.BuildPlayer(scenes, 
                                            Path.Combine(builderParams.buildToPath, builderParams.buildToFileName),
                                            GetBuildTargetByGroup(builderParams.buildPlatform), 
                                            options);
                }
                catch(Exception e)
                {
                    
                }
                finally
                {
                    DeleteCurrCopyFilesAndDirs();
                }

                //EditorUtility.OpenFolderPanel("生成安装包目录", builderParams.buildToPath, "");
            }
        }

        BuildTarget GetBuildTargetByGroup(BuildTargetGroup group)
        {
            BuildTarget target = BuildTarget.Android;
            target = (BuildTarget)System.Enum.Parse(typeof(BuildTarget), group.ToString());
            return target;
        }

        string GetBuildPackName()
        {
            string name = EnumExtension.GetDescriptionByName<DemoBuildPackType>(builderParams.buildPackType);
            if(name.Split('(') != null && name.Split('(').Length > 0)
            {
                name = name.Split('(')[0];
            }
            return name;
        }

        private static List<string> currCopyFilePaths = new List<string>();
        private static List<string> currCopyDirPaths = new List<string>();
        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="copySubDirs"></param>
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool bOverride = false, bool isLog = false)
        {
            if (isLog)
            {
                Debug.LogErrorFormat("### 开始拷贝：{0} ---> {1}", sourceDirName, destDirName);
            }
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                // throw new DirectoryNotFoundException(
                //     "Source directory does not exist or could not be found: "
                //     + sourceDirName);
                return;
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }
            
            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                
                string temppath = Path.Combine(destDirName, file.Name);
                var fileInfo = file.CopyTo(temppath, bOverride);

                //Debug.Log("拷贝到目标目录的文件全名：" + file.FullName);
                if (fileInfo != null)
                {
                    //Debug.LogError(fileInfo.FullName);
                    if(currCopyFilePaths != null)
                    {
                        currCopyFilePaths.Add(fileInfo.FullName);
                    }
                }
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    //if (currCopyDirPaths != null)
                    //{
                    //    currCopyDirPaths.Add(temppath);
                    //}
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs, bOverride);
                }
            }

            AssetDatabase.Refresh();
            if (isLog)
            {
                DirectoryInfo lib = new DirectoryInfo(destDirName + "/Plugins/Android/libs");
                    Debug.LogError("android下libs包含文件:");
                foreach (var item in lib.GetFiles())
                {
                    Debug.LogWarning(item.Name);
                }
                Debug.LogErrorFormat("### 拷贝结束：{0} ---> {1}", sourceDirName, destDirName);

            }
        }

        private static void DeleteCurrCopyFilesAndDirs()
        {
            if(currCopyFilePaths != null && currCopyFilePaths.Count > 0)
             {
                for (int i = 0; i < currCopyFilePaths.Count; i++)
                {
                    File.Delete(currCopyFilePaths[i]);
                }
                if(currCopyFilePaths != null)
                {
                    currCopyFilePaths.Clear();
                }         
                AssetDatabase.Refresh();      
            }  
            // if(currCopyDirPaths != null && currCopyDirPaths.Count > 0)
            // {
            //     currCopyDirPaths.Reverse();
            //     for (int i = 0; i < currCopyDirPaths.Count; i++)
            //     {
            //         Directory.Delete(currCopyDirPaths[i], true);
            //     } 
            //     if(currCopyDirPaths != null)
            //     {
            //         currCopyDirPaths.Clear();
            //     }
            //     AssetDatabase.Refresh();
            // }             
        }

        /*
        void WriteInSDKConfig()
        {
            Debug.LogError("BuildForAndroid sdkconfigPath :" + builderParams.sdkConfigPath);

            using (FileStream fs = new FileStream(builderParams.sdkConfigPath, FileMode.Create, FileAccess.ReadWrite))
            {
                byte[] text = System.Text.Encoding.Default.GetBytes(Global.Settings.sdkChannel.ToString());
                fs.Write(text, 0, text.Length);
                fs.Flush();
                fs.Close();
            }

            using (FileStream fs = new FileStream(builderParams.sdkConfigPath, FileMode.Open, FileAccess.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string content = sr.ReadToEnd();
                    UnityEngine.Debug.LogErrorFormat(" sdk.conf new content is {0}", content);
                }
            }
        }
         * */

        /* 
        void WriteInVersionConfig(string bundleVersion)
        {
            if (string.IsNullOrEmpty(bundleVersion))
                return;
            string[] bundleVersionArr = bundleVersion.Split('.');
            if (bundleVersionArr.Length != 4)
            {
                Debug.LogError("BuildForAndroid bundleVersion is error! " );
                return;
            }

            Debug.LogError("BuildForAndroid versionConfig :" + builderParams.bundleVersion);

            JsonData jsonData = null;
            using (FileStream fs = new FileStream(versionConfigPath, FileMode.Open, FileAccess.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string content = sr.ReadToEnd();
                    UnityEngine.Debug.LogErrorFormat("version.config old content is {0}", content);
                    jsonData = JsonMapper.ToObject(content);
                    if (jsonData != null)
                    {
                        jsonData["serverShortVersion"] = bundleVersionArr[1];
                        jsonData["serverVersion"] = bundleVersionArr[0];
                        jsonData["clientShortVersion"] = bundleVersionArr[3];
                        jsonData["clientVersion"] = bundleVersionArr[2];
                    }
                }
            }
            using (FileStream fs = new FileStream(versionConfigPath, FileMode.Create, FileAccess.ReadWrite))
            {
                if (jsonData != null)
                {
                    string json = JsonMapper.ToJson(jsonData);
                    UnityEngine.Debug.LogErrorFormat("version.config new content is {0}", json);
                    byte[] bytes = System.Text.Encoding.Default.GetBytes(json);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Flush();
                    fs.Close();
                }
            }
        }*/

        /* 
        void WriteInVersionJson(string bundleVersion, BuildTarget target)
        {
            if (string.IsNullOrEmpty(bundleVersion))
                return;

            Debug.LogError("BuildForAndroid versionJson :" + builderParams.bundleVersion);

            JsonData jsonData = null;
            using (FileStream fs = new FileStream(versionJsonPath, FileMode.Open, FileAccess.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string content = sr.ReadToEnd();
                    UnityEngine.Debug.LogErrorFormat("version.json old content is {0}", content);
                    jsonData = JsonMapper.ToObject(content);
                    if (jsonData != null)
                    {
                        if(target == BuildTarget.Android)
                        {
                            jsonData["android"] = bundleVersion;
                        }
                        else if (target == BuildTarget.iOS)
                        {
                            jsonData["ios"] = bundleVersion;
                        }
                        else
                        {
                            jsonData["pc"] = "1.0.1.0";
                        }
                    }
                }
            }
            using (FileStream fs = new FileStream(versionJsonPath, FileMode.Create, FileAccess.ReadWrite))
            {
                if (jsonData != null)
                {
                    string json = JsonMapper.ToJson(jsonData);
                    UnityEngine.Debug.LogErrorFormat("version.json new content is {0}", json);
                    byte[] bytes = System.Text.Encoding.Default.GetBytes(json);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Flush();
                    fs.Close();
                }
            }
        }
        */
    }
}