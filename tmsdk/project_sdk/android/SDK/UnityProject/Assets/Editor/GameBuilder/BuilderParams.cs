using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace CustomGameBuild
{
    public class BuilderParams : ScriptableObject
    {
        public DemoBuildPackType buildPackType;
        public string TestLoginVerifyUrl;
        public string TestChargeCallbackUrl;
        public string TestServerListUrl;

        public bool debugModeEnabled = true;
        public bool isSDKDebug;
        public bool isSDKPayDebug;

        public string sdkChannelAssetPath;
        //public SDKChannel sdkChannel;
        public string sdkChannelType;
        public string sdkConfigPath;

        public string[] sdkChannelCommonLibs;

        public BuildTargetGroup buildPlatform;
        public ScriptingImplementation buildType;

        [SerializeField]
        public BuildOptions[] buildOptions;

        public Texture2D showIcon;

        public string appIdentifier;

        /// <summary>
        /// 1.0.1.0
        /// </summary>
        public string bundleVersion;

        public string companyName;
        public string productName;

        public string scriptDefines;

        [SerializeField]
        public string[] gameScenes;

        public string buildToPath;
        public string buildToFileName;

        #region Android
        /// <summary>
        /// 1, 2, 3, 4,...       1.0,1.1,....
        /// </summary>
        public int bundleVersionCode = 0;

        /// <summary>
        /// 允许写入SD卡
        /// </summary>
        public bool forceSDCard = true;

        public string keystoreName;
        public string keystorePass;
        public string keyaliasName;
        public string keyaliasPass;

        public AndroidPreferredInstallLocation preferInstall = AndroidPreferredInstallLocation.Auto;

        public AndroidSdkVersions minSdkVersion;
        public AndroidSdkVersions targetSdkVersion;
        public AndroidArchitecture targetDevice;
        #endregion

        #region IOS

        #endregion
    }
}