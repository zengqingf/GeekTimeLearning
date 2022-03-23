using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Tenmove.Runtime.Client.Model.ChatData;

namespace VoiceSDK {
    public enum SDKVoiceLogLevel
    {
        None = 0,
        Error = 1,
        Warning = 2,
        Debug = 4,
    }

    public enum ChanelType
    {
        CHAT_CHANEL_COM = 0,    //综合频道
        CHAT_CHANNEL_SYSTEM = 1,    // 系统频道
        CHAT_CHANNEL_WORLD = 2,     //世界
        // CHAT_CHANNEL_OCCUP = 3,     //职业频道
        CHAT_CHANNEL_AROUND = 4,       // 附近频道
        CHAT_CHANNEL_UNION = 5,     //公会
        CHAT_CHANNEL_TEAM = 6,      //队伍频道
        CHAT_CHANEL_RECRUIT = 7,    // 招募频道
        CHAT_CHANNEL_HORN = 8,      //喇叭
        CHAT_CHANNEL_PRIVATE = 9,   //私聊
        CHAT_CHANNEL_MAX,
    };


    public class ChatDelegeInfo
    {
        public SDKVoiceInterface.OnVoiceChatLogin OnVoiceChatLogin;
        public SDKVoiceInterface.OnVoiceChatLogout OnVoiceChatLogout;
        public SDKVoiceInterface.OnVoiceChatRecordStart OnVoiceChatRecordStart;
        public SDKVoiceInterface.OnVoiceChatRecordEnd OnVoiceChatRecordEnd;
        public SDKVoiceInterface.OnVoiceChatRecordFailed OnVoiceChatRecordFailed;
        public SDKVoiceInterface.OnVoiceChatSendSucc OnVoiceChatSendSucc;
        public SDKVoiceInterface.OnVoiceChatSendFailed OnVoiceChatSendFailed;
        public SDKVoiceInterface.OnVoiceChatPlayStart OnVoiceChatPlayStart;
        public SDKVoiceInterface.OnVoiceChatPlayEnd OnVoiceChatPlayEnd;
    }

    public class SDKVoiceRoomInfo
    {
        public string serverId;
        public string roomtypeId;
        public string roomDec;
        public bool beSaveRoomMsg;
    }

    public class SDKVoiceRecordInfo
    {

        public SDKVoiceRoomInfo roomInfo = new SDKVoiceRoomInfo();    
        public bool isTranslate;
        public ulong iRequestId;
    }

    public class SDKVoiceToken
    {
        public string voicekey;
        public string serverId;
        public ulong targetId;
        public ulong roleId;
        public ulong serverTimeStamp;
        public string voiceType;
            
    }



    public class SDKVoiceInterface :ISDKVoiceInterface
    {

        private static SDKVoiceInterface _instance;
        public static SDKVoiceInterface Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new YouMiVoiceInterface();
                }
                return _instance;
            }
        }
        public static readonly string saveLocalPath =

#if UNITY_EDITOR
                 Application.dataPath + "/recordVoiceCache/"; //+ VoiceDataHelper.currDateTime + ".amr";
#elif UNITY_IOS && !UNITY_EDITOR
			     Application.persistentDataPath + "/recordVoiceCache/"; //+ VoiceDataHelper.currDateTime_less+chatTime + ".amr";
#elif UNITY_ANDROID && !UNITY_EDITOR
                 Application.persistentDataPath + "/recordVoiceCache/";  //+ VoiceDataHelper.currDateTime + ".amr";
#else 
                 Application.persistentDataPath + "/recordVoiceCache/";  //+ VoiceDataHelper.currDateTime + ".amr";
#endif

        //语音文件缓存路径
        public string SaveVoiceCachePath = "";

        protected Queue<string> voiceQueue = null;
        protected int maxVoiceQueueLength = 20;
        protected uint onTimeDeletePrivateChat = 10;
        protected int privateChatQueryMsgCount = 10;


        public delegate void OnVoiceChatLogin();
        public delegate void OnVoiceChatLogout();
        public delegate void OnVoiceChatNotJoinRoom();

        public OnVoiceChatLogin onVoiceChatLoginHandler; //登陆成功回调
        public OnVoiceChatLogout onVoiceChatLogoutHandler;
        public OnVoiceChatNotJoinRoom onVoiceChatNotJoinRoomHandler;

        public delegate void ShowDebugLogOnUI(string logMsg);  //测试日志输出
        public ShowDebugLogOnUI showDebugLogUIHandler;

        /* chatvoice delegate*/
        public delegate void OnVoiceChatRecordStart(string voicekey, string sText, int iDuration,string roomeTypeId);  //录音返回的一些参数
        public delegate void OnVoiceChatRecordFailed(int errorCode = 0);
        public delegate void OnVoiceChatRecordEnd();

        public delegate void OnVoiceChatSendSucc();
        public delegate void OnVoiceChatSendFailed(int errorCode = 0);

        public delegate void OnVoiceChatPlayStart();
        public delegate void OnVoiceChatPlayEnd();

        public OnVoiceChatRecordStart onVoiceChatRecordStart;
        public OnVoiceChatRecordFailed onVoiceChatRecordFailed;
        public OnVoiceChatRecordEnd onVoiceChatRecordEnd;
        public OnVoiceChatSendSucc onVoiceChatSendSucc;
        public OnVoiceChatSendFailed onVoiceChatSendFailed;
        public OnVoiceChatPlayStart onVoiceChatPlayStart;
        public OnVoiceChatPlayEnd onVoiceChatPlayEnd;



        /*游戏层调用*/
        public delegate void OnJoinChannelSuccess();
        public delegate void OnLeaveChannelSuccess();
        public delegate void OnVoiceMicOn(bool isOn);
        public delegate void OnVoicePlayerOn(bool isOn);

        public event OnJoinChannelSuccess onJoinChannelSucc;
        public event OnLeaveChannelSuccess onLeaveChannelSucc;
        public event OnVoiceMicOn onVoiceMicOn;
        public event OnVoicePlayerOn onVoicePlayerOn;


        /* voice chat  start  */

        public virtual void Init()
        {

        }

        public virtual void InitChatVoice() { }
        public virtual void UnInitChatVoice()
        {
  
            onVoiceChatLoginHandler = null;
            onVoiceChatLogoutHandler = null;
            onVoiceChatNotJoinRoomHandler = null;
        }

        public virtual void LoginVoice(YoumiVoiceGameAccInfo userInfo) { }
        public virtual void LogoutVoice() { }
        public virtual void StartRecordVoice() { }
        public virtual void StopRecordVoice() { }
        public virtual void CancelRecordVoice() { }

        public virtual void StartPlayVoice(string voicePath) { }
        public virtual void StopPlayVoice() { }
        public virtual void AddVoicePathInQueue(string voiceKey) { }
        public virtual void ClearVoicePathQueue() { }

        public virtual void SetVoiceVolume(float volume) { }
        public virtual float GetVoiceVolume()
        {
            return 0f;
        }

        public virtual void ClearLocalCache() { }

        public virtual void OnPause() { }

        public virtual void OnResume() { }

        public virtual bool IsVoiceRecording() { return false; }

        public virtual bool IsVoicePlaying() { return false; }

        //一般日志
        public virtual string ShowDebugLog()
        {
            return "";
        }

        public virtual bool BeInRealVoiceChannel() { return false; }
        public virtual void PlayVoiceCommon(string voicekey, SDKVoiceRoomInfo roomInfo = null) { }
        public virtual void JoinChatRoom(SDKVoiceRoomInfo roomInfo) { }
        public virtual void LeaveChatRoom() { }
        public virtual void SendVoiceMessage(SDKVoiceRecordInfo recordInfo, ref ulong iReqId) { }
        public virtual void StopAudioMessage(SDKVoiceToken voiceToken) { }
        public virtual void ClearVoiceChatMsgCache() { }
        public virtual bool CheckLoginChatVoice() { return true; }

        /* voice chat  end  */


        /*** voice talk  start  ***/
        public virtual void InitTalkVoice() { }

        public virtual void UnInitTalkVoice() { }

        public virtual void JoinChannel(string channelId, string roleId, string openId) { }

        public virtual void LeaveAllChannel() { }

        public virtual void LeaveChannel(string channelId) { }

        public virtual void OpenRealMic() { }

        public virtual void CloseRealMic() { }

        public virtual void OpenRealPlayer() { }

        public virtual void CloseReaPlayer() { }

        public virtual bool IsTalkRealMicOn()
        {
            return false;
        }
        public virtual bool IsTalkRealPlayerOn()
        {
            return false;
        }
        public virtual void SetPlayerVolume(float volume) { }

        public virtual float GetPlayerVolume()
        {
            return 0f;
        }

        public virtual void PauseChannel() { }

        public virtual void ResumeChannel() { }
        /*** voice talk  end  ***/

        #region LogLevel

        protected SDKVoiceLogLevel logLevel;
        public void SetLogLevel(SDKVoiceLogLevel logLevel)
        {
            this.logLevel = logLevel;
        }

        #endregion

        #region Talk Voice Event Method 
        protected virtual void JoinTalkChannelSucc()
        {
            if (onJoinChannelSucc != null)
            {
                onJoinChannelSucc();
            }
        }

        protected virtual void LeaveTalkChannelSucc()
        {
            if (onLeaveChannelSucc != null)
            {
                onLeaveChannelSucc();
            }
        }

        protected virtual void TalkVoiceMicOpenOn(bool isOn)
        {
            if (onVoiceMicOn != null)
            {
                onVoiceMicOn(isOn);
            }
        }

        protected virtual void TalkVoicePlayerOpenOn(bool isOn)
        {
            if (onVoicePlayerOn != null)
            {
                onVoicePlayerOn(isOn);
            }
        }
        #endregion
    }

}