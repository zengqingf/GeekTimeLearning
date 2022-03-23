using UnityEngine;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;

namespace SDKClient
{
#if UNITY_EDITOR
    public class SDKInterfaceIOS : SDKInterface
    {
    }
#elif UNITY_IPHONE || UNITY_IOS

    public class SDKInterfaceIOS : SDKInterface
    {
        //TODO SVN Merge from 1.0
    #region Common lib     

        public override void InitCommonLib()
        {
            base.InitCommonLib();
            _CommonInit();
        }

        public override void SetScreenBrightness(float value)
        {
            _SetBrightness(value);
        }

        public override float GetScreenBrightness()
        {
            return _GetBrightness();
        }

        public override float GetBatteryLevel()
        {
            return _GetBatteryLevel();
        }

        public override string GetClipboardText()
        {
            return _GetTextFromClipboard();
        }
            public override bool HasNotch(bool debug = false)
        {
            if (debug)
                return base.HasNotch(debug);
            return _HasNotch();
        }
        public override int[] GetNotchSize(NotchDebugType debug = NotchDebugType.None, ScreenOrientation screenOrientation = ScreenOrientation.LandscapeLeft)
        {
            if (debug != NotchDebugType.None)
                return base.GetNotchSize(debug, screenOrientation);
            return new int[2] { _GetNotchSize(), Screen.height };

        }
        public override int GetSystemVersion()
        {
            return _GetSystemVersion;
        }
        //public override void SaveImage2Album(byte[] bytes, string suc= "截图成功", string fail="截图失败")
        //{
        //    _SaveImage2Album(bytes, bytes.Length);
        //}
    #endregion

        public override int TryGetCurrVersionAPI()
        {
            if (_IsIOSSystemVersionMoreThanNine())
            {
                return 1000;
            }
            else
            {
                return 0;
            }
        }

        public override void Init(bool debug)
        {
            base.Init(debug);
            //_CommonInit();
        }

        public static void CommonInit()
        {
            _CommonInit();
        }
    #region 通知相关
        public override void SetNotification(int nid, string content, string title, int hour)
        {
            _SetNotification(nid, content, title, hour);
        }

        public override void SetNotificationWeekly(int nid, string content, string title, int weekday, int hour, int minute)
        {
            int weekdayId = ResetNidWeekly(nid, weekday);
            _SetNotificationWeekly(weekdayId, content, title, weekday, hour, minute);
        }

        public override void RemoveNotification(int nid)
        {
            for (int i = 1; i <= 7; i++)
            {
                _RemoveNotification(ResetNidWeekly(nid, i));
            }
        }

        public override void RemoveAllNotification()
        {
            _RemoveAllNotification();
        }

        private int ResetNidWeekly(int nid, int weekly)
        {
            return nid * 10 + 100000 + weekly;
        }

        public override void ResetBadge()
        {
            _ResetBadge();
        }

        public override string GetSystemTimeHHMM()
        {
            return base.GetSystemTimeHHMM();
        }
    #endregion

        public override void MobileVibrate()
        {
            Handheld.Vibrate();
        }

        // --- dllimport start ---

        [DllImport("__Internal")]
        private static extern void _CommonInit();

        [DllImport("__Internal")]
        private static extern void _SetBrightness(float value);

        [DllImport("__Internal")]
        private static extern float _GetBrightness();

        [DllImport("__Internal")]
        private static extern float _GetBatteryLevel();

        [DllImport("__Internal")]
        private static extern string _GetTextFromClipboard();

        [DllImport("__Internal")]
        private static extern bool _IsIOSSystemVersionMoreThanNine();

        [DllImport("__Internal")]
        private static extern void _SetNotification(int nid, string content, string title, int hour);

        [DllImport("__Internal")]
        private static extern void _SetNotificationWeekly(int nid, string content, string title, int weekday, int hour, int minute);

        [DllImport("__Internal")]
        private static extern void _RemoveNotification(int nid);

        [DllImport("__Internal")]
        private static extern void _RemoveAllNotification();

        [DllImport("__Internal")]
        private static extern void _ResetBadge();
    
        [DllImport("__Internal")]
        private static extern bool _HasNotch();

        [DllImport("__Internal")]
        private static extern int _GetNotchSize();
        [DllImport("__Internal")]
        private static extern int _GetSystemVersion();

        //[DllImport("__Internal")]
        //private static extern void _SaveImage2Album(byte[] bytes, int length, string suc, string fail);


    }

#endif
}