using UnityEngine;
using System.Collections;

namespace SDKClient
{
    public class SDKCallback : MonoSingleton<SDKCallback>
    {
        const int TOTAL_COUNTDOWN = 60 * 2;//二分钟
        const float LOW_SCREEN_BRIGHTNESS = 0.01f;
        float deviceScreenBrightness = 0.5f;
        SimpleTimer2 timer = null;
        bool screenSaverInited = false;
        float durationTimeTemp = 0f;
        float durationTimeTemp2 = 0f;

        protected void Start()
        {
            GameObject.DontDestroyOnLoad(gameObject);
        }

        protected void Update()
        {
            float timeElapsed = Time.deltaTime;

            if (screenSaverInited)
            {
                if (timer != null)
                    timer.UpdateTimer((int)(timeElapsed * 1000));

                //判定是否点屏幕
#if UNITY_EDITOR
                if (Input.GetMouseButtonUp(0))
                {
                    RestoreScreenBrightness();
                    if (timer != null)
                        timer.StartTimer();
                }
#else
				if (Input.touchCount == 1)
				{
					if(Input.GetTouch(0).phase == TouchPhase.Ended)
					{
						RestoreScreenBrightness();
						if (timer != null)
							timer.StartTimer();
					}
				}
#endif
            }
        }

        public void OnLogin(string param)
        {
            if (param == null)
            {
                Logger.LogErrorFormat("login param is null");
                return;
            }
            Logger.LogProcessFormat("Login param:{0}", param);            
            SDKUserInfo userInfo = SDKInterface.Instance.SDKUserInfo;
            if (userInfo == null)
            {
                return;
            }

            var paramArray = param.Split(',');
            if (paramArray != null && paramArray.Length == 2)
            {
                userInfo.openUid = paramArray[0];
                userInfo.token = paramArray[1];
                userInfo.ext = "";
                Logger.LogProcessFormat("[登陆成功] {0} {1}", paramArray[0], paramArray[1]);
                if (SDKInterface.Instance.loginCallbackGame != null)
                {
                    SDKInterface.Instance.loginCallbackGame(userInfo);
                }
            }
            else if (paramArray != null && paramArray.Length == 3)
            {
                userInfo.openUid = paramArray[0];
                userInfo.token = paramArray[1];
                userInfo.ext = paramArray[2];
                Logger.LogProcessFormat("[登陆成功] {0} {1} {2}", paramArray[0], paramArray[1], paramArray[2]);
                if (SDKInterface.Instance.loginCallbackGame != null)
                {
                    SDKInterface.Instance.loginCallbackGame(userInfo);
                }
            }
            else
            {
                Logger.LogErrorFormat("### [SDK] - login callback to unity , param is wrong: {0} !!!", param);
            }
        }


        public void OnPayResult(string param)
        {
            if (SDKInterface.Instance.payResultCallbackGame != null)
            {
                SDKInterface.Instance.payResultCallbackGame(param);
            }
        }

        public void OnLogout()
        {
			if(SDKInterface.Instance.logoutCallbackGame != null)
			{
            	SDKInterface.Instance.logoutCallbackGame();
			}
        }


        public void OnBindPhoneSucc(string phoneNum)
        {
            if (SDKInterface.Instance.bindPhoneCallbackGame != null)
            {
                SDKInterface.Instance.bindPhoneCallbackGame(phoneNum);
            }
        }

        public void OnKeyBoardShowOn(string param)
        {
            Debug.LogError(param);
            if (SDKInterface.Instance.keyboardShowCallbackGame != null)
            {
                SDKInterface.Instance.keyboardShowCallbackGame(param);
            }
        }

        protected void OnApplicationFocus(bool focusStatus)
        {
            if (!focusStatus)
            {
                RestoreScreenBrightness();
            }
        }

        protected void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                RestoreScreenBrightness();
            }
        }

        public override void Init()
        {
        }

        #region Common Lib

        public void StartScreenSave()
        {
            //TODO
            //if (GameClient.SwitchFunctionUtility.IsOpen(5))
            if (true)
            {
                InitScreenBrightnessProtect();
            }
            else
            {
                screenSaverInited = false;
                timer = null;
            }
        }

        public void InitScreenBrightnessProtect()
        {
            if (screenSaverInited)
                return;

            timer = new SimpleTimer2();
            SaveBrightness();

            timer.timeupCallBack = () =>
            {
                SetLowScreenBrightness();
            };

            int countdown = TOTAL_COUNTDOWN;

            //TODO
            //var data = TableManager.instance.GetTableItem<ProtoTable.SwitchClientFunctionTable>(5);
            //if (data != null)
            //    countdown = data.ValueA;

            timer.SetCountdown(countdown);
            timer.StartTimer();
            screenSaverInited = true;
        }

        public void SetLowScreenBrightness()
        {
            if (screenSaverInited)
            {
                if (SDKInterface.Instance.GetScreenBrightness() != deviceScreenBrightness)
                {
                    SaveBrightness();
                }
                SDKInterface.Instance.SetScreenBrightness(LOW_SCREEN_BRIGHTNESS);
            }
        }

        public void RestoreScreenBrightness()
        {
            if (screenSaverInited && SDKInterface.Instance.GetScreenBrightness() != deviceScreenBrightness)
            {
                SaveBrightness();

                SDKInterface.Instance.SetScreenBrightness(deviceScreenBrightness);
            }
        }

        public void SaveBrightness()
        {
            if (SDKInterface.Instance.GetScreenBrightness() > LOW_SCREEN_BRIGHTNESS)
            {
                deviceScreenBrightness = SDKInterface.Instance.GetScreenBrightness();
            }
        }

        #endregion
    }
}