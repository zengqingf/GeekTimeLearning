using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YouMe;

namespace VoiceSDK
{
    public class SDKVoiceCallback : MonoSingleton<SDKVoiceCallback>
    {
        //下面回调可放在 SDKVoiceInterface 中 统一两个语音模块回调管理

        /** Talk voice 语音层使用**/
        public delegate void OnRealVoiceInitHandler(bool isInited, YouMeErrorCode errorCode);
        public delegate void OnJoinChannelHandler(bool success, YouMeErrorCode errorCode);
        public delegate void OnLeaveChannelHandler(bool success, YouMeErrorCode errorCode);
        public delegate void OnPauseChannelHanlder(bool isPaused, YouMeErrorCode errorCode);
        public delegate void OnRealVoiceMicOnHandler(bool isOn, YouMeErrorCode errorCode);
        public delegate void OnRealVoicePlayerOnHandler(bool isOn, YouMeErrorCode errorCode);

        public OnRealVoiceInitHandler onRealVoiceInitHandler;
        public OnJoinChannelHandler onJoinChannelHandler;
        public OnLeaveChannelHandler onLeaveChannelHandler;
        public OnPauseChannelHanlder onPauseChannelHandler;
        public OnRealVoiceMicOnHandler onRealVoiceMicHandler;
        public OnRealVoicePlayerOnHandler onRealVoicePlayerHandler;

        public void Start()
        { 
            GameObject.DontDestroyOnLoad(gameObject);
        }

        public override void Init()
        {
            //Logger.LogErrorFormat("SDKVoiceCallback init");
        }

        public void OnApplicationPause(bool isPaused)
        {
            if (isPaused)
            {
               SDKVoiceInterface.Instance.OnPause();
               SDKVoiceInterface.Instance.PauseChannel();
            }
            else
            {
               SDKVoiceInterface.Instance.OnResume();
               SDKVoiceInterface.Instance.ResumeChannel();
            }
        }

        protected override void OnApplicationQuit()
        {
            OnYoumiVoiceDestroy();


            base.OnApplicationQuit();
        }

        protected override void OnDestroy()
        {
            OnYoumiVoiceDestroy();
            base.OnDestroy();
        }

        private void OnYoumiVoiceDestroy()
        {
            if (!SDKVoiceManager.isInit)
            {
                return;
            }

            //聊天语音登出
            SDKVoiceManager.GetInstance().LeaveVoiceSDK(true);

            SDKVoiceManager.GetInstance().UnInit();
        }


        #region YM Talk Voice Callback Listener

        public void OnEvent(string strParam)
        {
            string[] strSections = strParam.Split(new char[] { ',' }, 4);
            if (strSections == null)
            {
                Logger.LogError("SDKVoiceCallback onEvent strParams split is error");
                return;
            }

            int strSectionsOne;
            YouMe.YouMeEvent eventType = YouMeEvent.YOUME_EVENT_INIT_FAILED;
            if (int.TryParse(strSections[0], out strSectionsOne))
            {
                eventType = (YouMeEvent)strSectionsOne;
                Logger.LogProcessFormat("Talk Real YouMe OnEvent Callback | EventType : {0}", eventType.ToString());
            }
            else
            {
                Logger.LogProcessFormat(" SDKVoiceCallback onEvent YouMeEvent res is error !!!!!!");
            }
            int strSectionsTwo;
            YouMe.YouMeErrorCode errorCode = YouMeErrorCode.YOUME_ERROR_NOT_INIT;
            if (int.TryParse(strSections[1], out strSectionsTwo))
            {
                errorCode = (YouMeErrorCode)strSectionsTwo;
            }
            else
            {
                Logger.LogProcessFormat(" SDKVoiceCallback onEvent YouMeErrorCode res is error !!!!!!");
            }
            string channelID = strSections[2];
            string param = strSections[3];

            switch (eventType)
            {
                //对eventType的case列举请查询枚举类型YouMeEvent的定义，以下只是部分列举
                //使用者请按需自行添加或删除
                case YouMe.YouMeEvent.YOUME_EVENT_INIT_OK:
                    //"初始化成功";

                    if (onRealVoiceInitHandler != null)
                    {
                        onRealVoiceInitHandler(true, errorCode);
                    }

                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_INIT_FAILED:
                    //"初始化失败，错误码：" + errorCode;

                    if (onRealVoiceInitHandler != null)
                    {
                        onRealVoiceInitHandler(false, errorCode);
                    }

                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_JOIN_OK:
                    //加入频道成功

                    if (onJoinChannelHandler != null)
                    {
                        onJoinChannelHandler(true,errorCode);
                    }

                    //if (onJoinChannelSucc != null)
                    //{
                    //    onJoinChannelSucc();
                    //}
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_LEAVED_ALL:
                    //"离开所有频道成功";

                    if (onLeaveChannelHandler != null)
                    {
                        onLeaveChannelHandler(true,errorCode);
                    }

                    //Logger.LogProcessFormat("SDK Voice - lastRealTalkVoiceScene is {0}",SDKVoiceManager.GetInstance().lastRealTalkVoiceScene.ToString());

                    //设置退出场景为完全离开状态  上个状态是
                    //if (SDKVoiceManager.GetInstance().realTalkVocieScene == RealTalkVoiceScene.TeamBattle ||
                    //    (SDKVoiceManager.GetInstance().realTalkVocieScene == RealTalkVoiceScene.Pvp3v3Room && 
                    //    SDKVoiceManager.GetInstance().lastRealTalkVoiceScene == RealTalkVoiceScene.Pvp3v3Battle))
                    //{
                    //    SDKVoiceManager.GetInstance().SetCurrRealVoiceScene(RealTalkVoiceScene.None);
                    //}
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_LEAVED_ONE:
                    //退出单个语音频道完成

                    //if (onLeaveChannelSucc != null)
                    //{
                    //    onLeaveChannelSucc();
                    //}

                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_JOIN_FAILED:
                    //进入语音频道失败

                    if (onJoinChannelHandler != null)
                    {
                        onJoinChannelHandler(false,errorCode);
                    }

                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_REC_PERMISSION_STATUS:
                    //"录音启动失败（此时不管麦克风mute状态如何，都没有声音输出";
                    //通知录音权限状态，成功获取权限时错误码为YOUME_SUCCESS，获取失败为YOUME_ERROR_REC_NO_PERMISSION（此时不管麦克风mute状态如何，都没有声音输出）
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_RECONNECTING:
                    //"断网了，正在重连";
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_RECONNECTED:
                    //"断网重连成功";
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_OTHERS_MIC_OFF:
                    //其他用户的麦克风关闭：param是关闭用户的userid
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_OTHERS_MIC_ON:
                    //其他用户的麦克风打开
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_OTHERS_SPEAKER_ON:
                    //其他用户的扬声器打开
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_OTHERS_SPEAKER_OFF:
                    //其他用户的扬声器关闭
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_OTHERS_VOICE_ON:
                    //其他用户开始讲话
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_OTHERS_VOICE_OFF:
                    //其他用户结束讲话
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_MY_MIC_LEVEL:
                    //麦克风的语音级别，把errorCode转为整形即是音量值
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_MIC_CTR_ON:
                    //麦克风被其他用户打开
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_MIC_CTR_OFF:
                    //麦克风被其他用户关闭
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_SPEAKER_CTR_ON:
                    //扬声器被其他用户打开
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_SPEAKER_CTR_OFF:
                    //扬声器被其他用户关闭
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_LISTEN_OTHER_ON:
                    //取消屏蔽某人语音
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_LISTEN_OTHER_OFF:
                    //屏蔽某人语音
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_PAUSED:
                    //暂停语音频道完成
                    if (onPauseChannelHandler != null)
                    {
                        onPauseChannelHandler(true, errorCode);
                    }

                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_RESUMED:
                    //恢复语音频道完成
                    if (onPauseChannelHandler != null)
                    {
                        onPauseChannelHandler(false, errorCode);
                    }

                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_LOCAL_MIC_OFF:
                    if (onRealVoiceMicHandler != null)
                    {
                        onRealVoiceMicHandler(false,errorCode);
                    }
                    //if (onVoiceMicOn != null)
                    //{
                    //    onVoiceMicOn(false);
                    //}
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_LOCAL_MIC_ON:
                    if (onRealVoiceMicHandler != null)
                    {
                        onRealVoiceMicHandler(true, errorCode);
                    }
                    //if (onVoiceMicOn != null)
                    //{
                    //    onVoiceMicOn(true);
                    //}
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_LOCAL_SPEAKER_OFF:
                    if (onRealVoicePlayerHandler != null)
                    {
                        onRealVoicePlayerHandler(false, errorCode);
                    }
                    //if (onVoicePlayerOn != null)
                    //{
                    //    onVoicePlayerOn(false);
                    //}
                    break;
                case YouMe.YouMeEvent.YOUME_EVENT_LOCAL_SPEAKER_ON:
                    if (onRealVoicePlayerHandler != null)
                    {
                        onRealVoicePlayerHandler(true, errorCode);
                    }
                    //if (onVoicePlayerOn != null)
                    //{
                    //    onVoicePlayerOn(true);
                    //}
                    break;
                default:
                    // "事件类型" + eventType + ",错误码" + errorCode;
                    break;
            }
        }

        void OnRequestRestApi(string strParam)
        {
            Logger.LogProcessFormat("YOUMI VOICE - OnRequestRestApi json is {0}", strParam);
        }


        void OnMemberChange(string strParam)
        {
            Logger.LogProcessFormat("YOUMI VOICE - OnMemberChange json is {0}", strParam);
        }



        #endregion

    }
}
