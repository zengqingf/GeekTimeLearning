using UnityEngine;
using System.Collections;
using System;

namespace SDKClient
{
#if UNITY_EDITOR
    public class SDKInterfaceAndroid : SDKInterface
    {
    }
#elif UNITY_ANDROID

    public class SDKInterfaceAndroid : SDKInterface
    {
		protected AndroidJavaObject currentActivity;
		protected  AndroidJavaObject GetActivity()
		{
			if (currentActivity == null)
			{
				AndroidJavaClass unity = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
				currentActivity = unity.GetStatic<AndroidJavaObject> ("currentActivity");
			}
					
			return currentActivity;
		}

        protected AndroidJavaObject lebianJavaObj;
		protected AndroidJavaObject GetLebianJavaObject()
		{
			if (lebianJavaObj == null)
			{
				lebianJavaObj = new AndroidJavaObject("com.excelliance.open.LeBianSDKImpl");
				lebianJavaObj.CallStatic("SetCommonContext", GetActivity());
			}
			return lebianJavaObj;
		}

        protected AndroidJavaObject commonJavaObj;
        protected AndroidJavaObject GetCommonJavaObject()
        {
            if (commonJavaObj == null)
            {
                commonJavaObj = new AndroidJavaObject("com.tm.commonlib.AndroidCommonImpl");
                commonJavaObj.CallStatic("SetCommonContext", GetActivity());
            }
            return commonJavaObj;
        }

    #region LeBian sdk

        public override void LBRequestUpdate()
		{
			GetLebianJavaObject().CallStatic("RequestUpdate");
		}

		public override void LBDownloadFullRes()
		{
			GetLebianJavaObject().CallStatic("DownloadFullResNotify");
		}

        public override bool LBIsAfterUpdate()
        {
            bool isAfterUpdate = false;
            try
            {
                isAfterUpdate = GetLebianJavaObject().CallStatic<bool>("IsAfterUpdate");
                //Debug.Log("LBIsAfterUpdate return value --->>> " + isAfterUpdate);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            return isAfterUpdate;
        }

        public override string LBGetFullResPath()
        {
            string fullResPath = "";
            try
            {
                fullResPath = GetLebianJavaObject().CallStatic<string>("GetFullResPath");
                //Debug.Log("LBGetFullResPath return value --->>> " + fullResPath);
            }
            catch(Exception e)
            {
                Debug.Log(e);
            }
            return fullResPath;
        }

    #endregion

    #region UniWebView
    public override  int GetKeyboardSize()
    {
        using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject View = null;

            try
            {
                if (UnityClass == null)
                    return 0;
                AndroidJavaObject activity = UnityClass.GetStatic<AndroidJavaObject>("currentActivity");
                if (activity == null)
                    return 0;
                AndroidJavaObject unityPlayer = activity.Get<AndroidJavaObject>("mUnityPlayer");
                if (unityPlayer == null)
                    return 0;
                View = unityPlayer.Call<AndroidJavaObject>("getView");
            }
            catch (Exception e)
            {
                Logger.LogError("try GetUnityPlayerView in Android failed : "+e.ToString());
                return 0;
            }

            if (View == null)
                return 0;

            using (AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect"))
            {
                if (Rct == null)
                    return 0;
                try
                {
                    View.Call("getWindowVisibleDisplayFrame", Rct);
                }
                catch (Exception e)
                {
                    Logger.LogError("Platform Android GetKeyboardSize failed :"+e.ToString());
                    return 0;
                }
                return Screen.height - Rct.Call<int>("height");
            }
        }
    }

    public override int TryGetCurrVersionAPI()
    {
        string op = SystemInfo.operatingSystem;
        Logger.LogProcessFormat("operating system api info : {0}", op);

        int res = 0;
        try
        {
            string headPlat = "Android OS";
            string headStr = "API-";
            int headStrLength = headStr.Length;
            int apiCount = 2;

            if (op.Contains(headPlat))
            {
                if (op.Contains(headStr) == false)
                    return res;
                int index = op.IndexOf(headStr);

                Logger.LogProcessFormat("try index = {0}", index);

                string api = op.Substring(index + headStrLength, apiCount);

                Logger.LogProcessFormat("try api = {0}", api);

                if (int.TryParse(api, out res))
                {
                    return res;
                }
            }

            return res;

        }catch(Exception e)
        {
            Logger.LogErrorFormat("Get Android Api error : {0}",e.ToString());
            return res;
        }
    }
    #endregion

    #region Common lib

        public override void InitCommonLib()
        {
            base.InitCommonLib();
            KeepScreenOn(true);
        }

        public override void KeepScreenOn(bool isOn)
        {
            try
            {
                GetActivity().Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject jo = GetActivity().Call<AndroidJavaObject>("getWindow");
                    if (jo == null)
                        return;
                    if (isOn)
                    {
                        jo.Call("addFlags", 128);
                    }
                    else
                    {
                        jo.Call("clearFlags", 128);
                    }
                }));
            }
            catch (Exception e)
            {
                SDKUtility.SDKDebugFormat(DebugType.ErrorNoMask, "error : {0}", e.ToString());
            }
        }

        public override void SetScreenBrightness(float value)
        {
            //不要完全关闭亮度
            if (value <= 0f)
            {
                return;
            }
            try
            {
                float brightness255 = value * 255f;
                GetCommonJavaObject().CallStatic("SetScreenBrightness", brightness255);
            }
            catch (Exception e)
            {
                SDKUtility.SDKDebugFormat(DebugType.ErrorNoMask, "error : {0}", e.ToString());
            }
        }

        public override float GetScreenBrightness()
        {
            try
            {
                int brightnessInt = GetCommonJavaObject().CallStatic<int>("GetScreenBrightness");
                return brightnessInt / 255f;
            }
            catch (Exception e)
            {
                SDKUtility.SDKDebugFormat(DebugType.ErrorNoMask, "error : {0}", e.ToString());
                return base.GetScreenBrightness();
            }
        }

        public override float GetBatteryLevel()
        {
            try
            {
                return GetCommonJavaObject().CallStatic<float>("GetBatteryLevel");
            }
            catch (Exception e)
            {
                SDKUtility.SDKDebugFormat(DebugType.ErrorNoMask, "error : {0}", e.ToString());
                return base.GetBatteryLevel();
            }
        }

        public override bool IsBatteryCharging()
        {
            try
            {
                return GetCommonJavaObject().CallStatic<bool>("IsBatteryCharging");
            }
            catch (Exception e)
            {
                SDKUtility.SDKDebugFormat(DebugType.ErrorNoMask, "error : {0}", e.ToString());
                return base.IsBatteryCharging();
            }
        }

        public override string GetClipboardText()
        {
            try
            {
                return GetCommonJavaObject().CallStatic<string>("GetClipboardText");
            }
            catch (Exception e)
            {
                SDKUtility.SDKDebugFormat(DebugType.ErrorNoMask, "error : {0}", e.ToString());
                return GUIUtility.systemCopyBuffer;
            }
        }
        public override bool HasNotch(bool debug = false)
        {
            if (debug)
                return base.HasNotch(debug);

            try
            {
                return GetCommonJavaObject().CallStatic<bool>("hasNotch");
            }
            catch (Exception e)
            {
                SDKUtility.SDKDebugFormat(DebugType.ErrorNoMask, "error : {0}", e.ToString());
                return base.HasNotch(debug);
            }
        }
        public override int[] GetNotchSize(NotchDebugType debug = NotchDebugType.None, ScreenOrientation screenOrientation = ScreenOrientation.LandscapeLeft)
        {
            if (debug != NotchDebugType.None)
                return base.GetNotchSize(debug, screenOrientation);
            try
            {
                var i = GetCommonJavaObject().CallStatic<int[]>("getNotchsize");
                return i;
            }
            catch (Exception e)
            {
                SDKUtility.SDKDebugFormat(DebugType.ErrorNoMask, "error : {0}", e.ToString());
                return base.GetNotchSize(debug);
            }

        }
        public override int GetSystemVersion()
        {
            try
            {
                var i = GetCommonJavaObject().CallStatic<int>("getSystemVersion");
                return i;
            }
            catch (Exception e)
            {
                SDKUtility.SDKDebugFormat(DebugType.ErrorNoMask, "error : {0}", e.ToString());
                return base.GetSystemVersion();
            }
        }
        //public override void SaveImage2Album(byte[] bytes, string suc= "截图成功", string fail="截图失败")
        //{  
        //    try
        //    {
        //           GetCommonJavaObject().CallStatic("saveImage2Album", bytes, suc, fail);
        //    }
        //    catch (Exception e)
        //    {
        //        SDKUtility.SDKDebugFormat(DebugType.ErrorNoMask, "error : {0}", e.ToString());
        //    }

        //}

    #endregion
    }   


#endif

}