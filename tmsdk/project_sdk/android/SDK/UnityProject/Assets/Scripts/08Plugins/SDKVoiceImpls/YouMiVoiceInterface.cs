using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YIMEngine;
using YouMe;

namespace VoiceSDK
{
    public class YoumiVoiceGameAccInfo
    {
        public string RoleId;
        public string OpenId;
        public string Token = "";

        public string YoumiId;

        public string TalkChannelId;
    }

    public class AudioInfo
    {
        public string token;                //自己构建的语音消息透传  角色id|服务器时间
        public ulong requestId;             //游密语音生成的消息Id
        public DownloadStatus status;       //语音下载状态
        public string path;                 //语音消息的存放路径
        public bool bPrivate;               //语音消息类型为私聊

        public AudioInfo()
        {
        }
    }

    public enum DownloadStatus
    {
        DS_SUCCESS = 0,
        DS_NOTDOWNLOAD = 1,
        DS_FAILED = 2,
        DS_DOWNLOADING = 3,
    }



    public class YouMiVoiceInterface : SDKVoiceInterface,
                                                          LoginListen,
                                                          ChatRoomListen,
                                                          MessageListen,
                                                          AudioPlayListen,
                                                          ContactListen,
                                                          LocationListen,
                                                          DownloadListen
    {
        const string AppKey = "YOUMEAA5EE5689436B39DC46E6195FF45F81CD46A1804";
        const string AppSecret = "3hzCOsAPitudP7DiQN7ANrkbnTpEVdm0KJ1fFNmXzwL6BZTfEGwfBU4W2efhnAxkx11idqN60lIJ26KkRhBrBQcgui8SahalzxtVv+hKHvDhg/KTMhmal8tuknnAcxlWkq7102ZG3EM6loBPCMp6t96078W7XCWNoszlzPxT6w0BAAE=";


        //调试日志
        string cacheLogMsg = "";

        YoumiVoiceGameAccInfo cacheChatVoiceAccInfo;

        public enum LoginVoiceState
        {
            Logined,
            Logining,
            Logouted,
            Logouting,
        }
        LoginVoiceState loginVoiceState = LoginVoiceState.Logouted;

        //即时聊天模式
        bool isChatVoiceInited = false;
        bool isVoiceTranslate = false;
        float voiceVolume = 0f;
        bool isRecording = false;
        bool isRecordPTTFailed = false;
        ErrorCode chatMethodResult;
        bool isQueryHistoryMsgClear = false;       

        List<string> voiceChatRoomIdList = new List<string>();

        /** 实时语音模式 **/

        //实时语音初始化 和 加入频道 的接口调用状态 根据是否能多次调用决定！！！
        public enum JoinTalkChannelState
        {
            Joined,
            Leaved,
            Joining,
            Leaving
        }
        JoinTalkChannelState joinChannelState = JoinTalkChannelState.Leaved;

        public enum VoiceTalkPauseState
        {
            Paused,
            Pausing,
            Resumed,
            Resuming,
        }
        VoiceTalkPauseState voiceTalkPauseState = VoiceTalkPauseState.Resumed;

        bool isTalkVoiceInited = false;
        float playerVolume = 0f;
        bool isTalkMicOn = false;
        bool isTalkPlayerOn = false;
        YouMeErrorCode talkMethodResult;
        ulong joinTalkRoomStartTime = 0;


        public override void Init()
        {
            //缓存的聊天模式语音的登录帐号信息
            cacheChatVoiceAccInfo = new YoumiVoiceGameAccInfo();
        }

        #region PUBLIC Method for Chat Voice
        public override void InitChatVoice()
        {
            if (isChatVoiceInited)
                return;
            isVoiceTranslate = false;
            chatMethodResult = IMAPI.Instance().Init(AppKey, AppSecret, ServerZone.China);
            if (chatMethodResult == ErrorCode.Success)
            {
                isChatVoiceInited = true;

                IMAPI.Instance().SetLoginListen(this);
                IMAPI.Instance().SetMessageListen(this);
                IMAPI.Instance().SetChatRoomListen(this);
                IMAPI.Instance().SetDownloadListen(this);
                IMAPI.Instance().SetAudioPlayListen(this);
                IMAPI.Instance().SetContactListen(this);
                IMAPI.Instance().SetLocationListen(this);

                if(string.IsNullOrEmpty(SaveVoiceCachePath))
                {
                    SaveVoiceCachePath = saveLocalPath;
                }
                    IMAPI.Instance().SetAudioCachePath(SaveVoiceCachePath);
                    LogForYouMiChat("InitVoice params set:", chatMethodResult, "SaveVoiceCachePath: " + SaveVoiceCachePath);
                //初始化缓存队列
                voiceQueue = new Queue<string>();
            }
            LogForYouMiChat("InitVoice", chatMethodResult);
        }

        public override void UnInitChatVoice()
        {
            base.UnInitChatVoice();

            //语音队列
            voiceQueue = null;
            cacheChatVoiceAccInfo = null;

            isChatVoiceInited = false;

            isVoiceTranslate = false;
            maxVoiceQueueLength = 20;
            cacheLogMsg = "";
            voiceVolume = 0f;
            isRecording = false;
            isRecordPTTFailed = false;
            _leastLocalAudioToken = "";
            _currentReqPlayAudioToken = "";
            _lastRoomTypeId = "";
            _AudioCache.Clear();

            loginVoiceState = LoginVoiceState.Logouted;

            isQueryHistoryMsgClear = false;

            voiceChatRoomIdList.Clear();
        }

        //自己构建的语音消息透传参数（结构为：角色|当前服务器时间）
        string _leastLocalAudioToken = "";
        //当前请求播放的语音携带的透传参数
        string _currentReqPlayAudioToken = "";
        //语音消息类型 私聊 及 其他 
        //ChanelType _leasetLocalAudioChatType = ChanelType.CHAT_CHANEL_COM;
        //上次录音的roomTypeId
        string _lastRoomTypeId = "";

    //语音消息本地缓存字典 消息透传 , 消息结构体
    Dictionary<string, AudioInfo> _AudioCache = new Dictionary<string, AudioInfo>();

        public string BuildRequestIdPath(ulong requestId)
        {
            return System.IO.Path.Combine(saveLocalPath, requestId.ToString() + this.GetFileNameExtension());
        }
        string GetFileNameExtension()
        {
            return ".wav";
        }

        //播放
        public override void PlayVoiceCommon(string voicekey, SDKVoiceRoomInfo roomInfo = null)
        {
            if (string.IsNullOrEmpty(voicekey))
            {
                return;
            }
            _currentReqPlayAudioToken = voicekey;

            if (_AudioCache == null)
            {
                return;
            }

            if (_AudioCache.ContainsKey(_currentReqPlayAudioToken))
            {
                AudioInfo audio = _AudioCache[_currentReqPlayAudioToken];

                if (audio.status == DownloadStatus.DS_SUCCESS)
                {
                    TryPlayVoiceByPath(audio.path);
                }
                else if (audio.status == DownloadStatus.DS_NOTDOWNLOAD)
                {
                    DownloadAudioFile(audio);
                }
            }
            else
            {
                if (roomInfo == null)
                {
                    return;
                }
                string targetUserId = SetRoomTypeId(roomInfo.serverId, roomInfo.roomtypeId, roomInfo.roomDec);
                QueryPrivateHistoryMsgByUserId(targetUserId, privateChatQueryMsgCount);
            }
        }

        public override void LoginVoice(YoumiVoiceGameAccInfo userInfo)
        {
            if (cacheChatVoiceAccInfo != null)
            {
                cacheChatVoiceAccInfo.RoleId = userInfo.RoleId;
                cacheChatVoiceAccInfo.OpenId = userInfo.OpenId;
                cacheChatVoiceAccInfo.Token = userInfo.Token;
            }
            LogForYouMiChat("LoginVoice !!! Just Set !!! YoumiChatVoiceAccInfo : ", chatMethodResult, "roleId " + userInfo.RoleId + "| pass " + userInfo.OpenId + "| token " + userInfo.Token);
            TryLoginYoumiChatVoice(cacheChatVoiceAccInfo);  //需要的时候再登陆
        }

        public override void LogoutVoice()
        {

            TryLogoutYoumiChatVoice();
        }

        public override void JoinChatRoom(SDKVoiceRoomInfo roomInfo)
        {
            if (!CheckYoumiChatVoiceLoginState(false))
            {
                return;
            }
            if (roomInfo == null)
            {
                LogForYouMiChat("JoinChatRoom", ErrorCode.Fail, "LeaveChatRoom roomInfo is null");
                return;
            }
            string roomId = SetRoomTypeId(roomInfo.serverId, roomInfo.roomtypeId, roomInfo.roomDec);
            TryJoinChatRoom(roomId, roomInfo.beSaveRoomMsg);
        }

        public override void LeaveChatRoom()
        {
            if (!CheckYoumiChatVoiceLoginState(false))
            {
                return;
            }
            if (voiceChatRoomIdList != null && voiceChatRoomIdList.Count >= 0)
            {
                LogForYouMiChat("TryLeaveChatRoom", ErrorCode.Success, string.Format("TryLeaveChatRoom roomCount is {0}",voiceChatRoomIdList.Count));
                for (int i = 0; i < voiceChatRoomIdList.Count; ++i)
                {
                    string roomId = voiceChatRoomIdList[i];
                    TryLeaveChatRoom(roomId);
                }             
            }
        }

        public override void AddVoicePathInQueue(string voiceKey)
        {
            if (voiceQueue != null && !string.IsNullOrEmpty(voiceKey))
            {
                if (_AudioCache != null && _AudioCache.ContainsKey(voiceKey))
                {
                    return;
                }

                if (isRecording)
                {
                    return;
                }

                if (voiceQueue.Count > maxVoiceQueueLength)
                {
                    voiceQueue.Dequeue();
                }
                voiceQueue.Enqueue(voiceKey);
                PlayVoiceQueue();
            }
        }


        public override void ClearVoicePathQueue()
        {
            if (voiceQueue != null && voiceQueue.Count > 0)
            {
                voiceQueue.Clear();
                LogForYouMiChat("ClearVoicePathQueue", ErrorCode.Success, "Chat Voice auto play queue is clear now");
            }
        }

        public override void SetVoiceVolume(float volume)
        {
            if (!CheckYoumiChatVoiceLoginState(false))
            {
                return;
            }
            try
            {
                IMAPI.Instance().SetVolume(volume);
                voiceVolume = volume;
                LogForYouMiChat("SetVoiceVolume", ErrorCode.Success, "set curr volumn is " + volume);
            }
            catch (System.Exception e)
            {
                Logger.LogErrorFormat("Chat Voice Set Volumn is Failed : {0}", e.ToString());
            }
        }

        public override float GetVoiceVolume()
        {
            return voiceVolume;
        }

        public override void ClearLocalCache()
        {
            if (!isChatVoiceInited)
            {
                //InitChatVoice();
                return;
            }
            bool isClearOk = IMAPI.Instance().ClearAudioCachePath();
            if (isClearOk)
            {
                LogForYouMiChat("ClearLocalCache", ErrorCode.Success, "clear local voice cache succ");
            }
            else
            {
                LogForYouMiChat("ClearLocalCache", ErrorCode.Fail, "clear local voice cache failed");
            }
        }
        /// <summary>
        /// 暂停播放
        /// </summary>
        public override void OnPause()
        {
            if (!isChatVoiceInited)
            {
                //InitChatVoice();
                return;
            }
            IMAPI.Instance().OnPause(false);
            LogForYouMiChat("OnPause Chat Voice", ErrorCode.Success, "[YouMe - Voice Chat] OnPause !!!");
        }

        /// <summary>
        /// 重新播放
        /// </summary>
        public override void OnResume()
        {
            if (!isChatVoiceInited)
            {
                return;
            }
            IMAPI.Instance().OnResume();
            LogForYouMiChat("OnResume Chat Voice", ErrorCode.Success, "[YouMe - Voice Chat] OnResume !!!");
        }

        //！！！ 注意 进副本时 关闭自动语音播放 ？？？
        public override void StopPlayVoice()
        {
            TryStopPlayVoice();

            //停止语音播放时，清空自动语音播放队列
            ClearVoicePathQueue();
        }

        /// <summary>
        ///按住开始录音
        /// </summary>
        /// <param name="receId"></param>
        /// <param name="chatType"></param>
        /// <param name="iReqId"></param>
        /// <param name="bTranslate"></param>

        public override void SendVoiceMessage(SDKVoiceRecordInfo recordInfo, ref ulong iReqId)
        {
            if (!CheckYoumiChatVoiceLoginState())
            {
                return;
            }
            if (isRecording)
            {
                return;
            }

            if (isRecordPTTFailed)
            {
                //StopAudioMessage("");
                chatMethodResult = IMAPI.Instance().StopAudioMessage("");
                LogForYouMiChat("SendVoiceMessage", ErrorCode.Success, "isRecordPTTFailed !!! stopaudioMessage");
                return;
            }
            if(recordInfo == null)
            { return; }
            _lastRoomTypeId = recordInfo.roomInfo.roomtypeId;
            string receId = SetRoomTypeId(recordInfo.roomInfo.serverId,recordInfo.roomInfo.roomtypeId, recordInfo.roomInfo.roomDec);
            if (string.IsNullOrEmpty(receId))
            {
                //TODO 如果录音时 目标房间或者角色ID为空 尝试加入 
                if (onVoiceChatNotJoinRoomHandler != null)
                {
                    LogForYouMiChat("SendVoiceMessage", ErrorCode.Success, "receId is null Not JoinRoom");
                    onVoiceChatNotJoinRoomHandler();
                }
                return;
            }

            //统一使用房间语音类型
            YIMEngine.ChatType yChatType = YIMEngine.ChatType.RoomChat;
            //设置本次录音 聊天类型
            //_leasetLocalAudioChatType = chatType;

            if (!recordInfo.isTranslate)
            {
                chatMethodResult = IMAPI.Instance().SendOnlyAudioMessage(receId, yChatType, ref iReqId);
            }
            else
            {
                chatMethodResult = IMAPI.Instance().SendAudioMessage(receId, yChatType, ref iReqId);
            }

            LogForYouMiChat("SendVoiceMessage", chatMethodResult, string.Format("Record Voice receId is {0}", receId.ToString()));

            if (chatMethodResult != ErrorCode.Success)
            {
                //不要调用 获取麦克风状态 的接口
                if (chatMethodResult == ErrorCode.PTT_Fail)
                {
                    isRecordPTTFailed = true;
                    isRecording = false;
                }
                else if (chatMethodResult == ErrorCode.NotLogin)
                {
                    loginVoiceState = LoginVoiceState.Logouted;
                    TryLoginYoumiChatVoice(cacheChatVoiceAccInfo);
                }
                onVoiceChatRecordFailed((int)chatMethodResult);
            }
            else
            {
                isRecording = true;
                isRecordPTTFailed = false;
            }
        }

        /// <summary>
        /// 拼接roomID
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="roomtypeId"></param>
        /// <param name="roomDec"></param>
        /// <returns></returns>
        string SetRoomTypeId(string serverId,string roomtypeId , string roomDec)
        {
            return string.Format("{0}_{1}_{2}", serverId, roomDec, roomtypeId);
        }

        string GetVoiceToken(ulong serverTime , string voicetype)
        {
            if (!string.IsNullOrEmpty(cacheChatVoiceAccInfo.RoleId))
            {
                return string.Format("{0}_{1}_{2}", cacheChatVoiceAccInfo.RoleId, serverTime, voicetype);
            }
            return "";
        }


        public override void StopAudioMessage(SDKVoiceToken voiceToken)
        {
            if (!CheckYoumiChatVoiceLoginState(false))
            {
                return;
            }

            if (voiceToken == null)
            {
                onVoiceChatSendFailed();
              return;
            }

            _leastLocalAudioToken = GetVoiceToken(voiceToken.serverTimeStamp, voiceToken.voiceType);

            chatMethodResult = IMAPI.Instance().StopAudioMessage(_leastLocalAudioToken);

            LogForYouMiChat("StopAudioMessage", chatMethodResult, "Stop record with type is " + voiceToken.voiceType);

            isRecording = false;
            if (chatMethodResult != ErrorCode.Success)
            {
                onVoiceChatRecordFailed((int)chatMethodResult);
            }
        }

        /// <summary>
        /// 上滑取消录音
        /// </summary>
        public override void CancelRecordVoice()
        {
            
            if (!CheckYoumiChatVoiceLoginState())
            {
                return;
            }
            chatMethodResult = IMAPI.Instance().CancleAudioMessage();
             
        }

       public override string ShowDebugLog()
        {
            return cacheLogMsg;
        }

        public override bool IsVoiceRecording()
        {
            if (!CheckYoumiChatVoiceLoginState(false))
            {
                return false;
            }
            return isRecording;
        }

        public override bool IsVoicePlaying()
        {
            if (!CheckYoumiChatVoiceLoginState(false))
            {
                return false;
            }
            return IMAPI.Instance().IsPlaying();
        }

        /// <summary>
        /// 清理记录
        /// </summary>
        public override void ClearVoiceChatMsgCache()
        {
            DeleteRoomChatMsgOnTime(onTimeDeletePrivateChat);

            if (voiceChatRoomIdList != null && voiceChatRoomIdList.Count > 0)
            {
                string rid = voiceChatRoomIdList[0];
                QueryHistoryMsgToClear(rid, privateChatQueryMsgCount, 0);
            }
            else
            {
                TryLogoutYoumiChatVoice();
            }
        }

        public override bool CheckLoginChatVoice()
        {
            return CheckYoumiChatVoiceLoginState();
        }

        #endregion

        #region PUBLIC Method for Talk Voice
        /** 实时语音 **/

        public override void InitTalkVoice()
        {
            YouMeVoiceAPI.GetInstance().SetCallback(SDKVoiceCallback.instance.gameObject.name);

            //绑定回调
            SDKVoiceCallback.instance.onRealVoiceInitHandler = OnTalkVoiceInit;
            SDKVoiceCallback.instance.onJoinChannelHandler = OnTalkVoiceJoinChannel;
            SDKVoiceCallback.instance.onLeaveChannelHandler = OnTalkVoiceLeaveChannel;
            SDKVoiceCallback.instance.onPauseChannelHandler = OnTalkVoiceChannelPause;
            SDKVoiceCallback.instance.onRealVoiceMicHandler = OnTalkVoiceMicOn;
            SDKVoiceCallback.instance.onRealVoicePlayerHandler = OnTalkVoicePlayerOn;

            TryInitTalkVoice();
        }

        public override void UnInitTalkVoice()
        {
            //解绑回调
            SDKVoiceCallback.instance.onRealVoiceInitHandler = null;
            SDKVoiceCallback.instance.onJoinChannelHandler = null;
            SDKVoiceCallback.instance.onLeaveChannelHandler = null;
            SDKVoiceCallback.instance.onPauseChannelHandler = null;
            SDKVoiceCallback.instance.onRealVoiceMicHandler = null;
            SDKVoiceCallback.instance.onRealVoicePlayerHandler = null;

            isTalkVoiceInited = false;

            joinChannelState = JoinTalkChannelState.Leaved;

            playerVolume = 0f;
            joinTalkRoomStartTime = 0;
            voiceTalkPauseState = VoiceTalkPauseState.Resumed;

            isTalkMicOn = false;
            isTalkPlayerOn = false;

            talkMethodResult = YouMeVoiceAPI.GetInstance().UnInit();
        }

        public override void JoinChannel(string channelId, string roleId, string openId)
        {
            if (cacheChatVoiceAccInfo != null)
            {
                cacheChatVoiceAccInfo.TalkChannelId = channelId;
                cacheChatVoiceAccInfo.RoleId = roleId;
                cacheChatVoiceAccInfo.OpenId = openId;
                cacheChatVoiceAccInfo.Token = "";
            }
            if (!isTalkVoiceInited)
            {
                //TryInitTalkVoice();
                return;
            }

            TryJoinYoumiTalkChannel(cacheChatVoiceAccInfo);
        }

        public override void LeaveAllChannel()
        {
            //if (isTalkVoiceJoinSucc)
            TryLeaveAllYoumiTalkChannel();
        }

        public override void LeaveChannel(string channelId)
        {
        }

        //打开麦克风
        public override void OpenRealMic()
        {
            if (!CheckYouMiTalkVoiceJoinState())
            {
                return;
            }
            YouMeVoiceAPI.GetInstance().SetMicrophoneMute(false);
            LogForYouMiTalk("OpenRealMic", YouMeErrorCode.YOUME_SUCCESS);
        }

        public override void CloseRealMic()
        {
            if (!CheckYouMiTalkVoiceJoinState())
            {
                return;
            }
            YouMeVoiceAPI.GetInstance().SetMicrophoneMute(true);
            LogForYouMiTalk("CloseRealMic", YouMeErrorCode.YOUME_SUCCESS);
        }

        //开启扬声器
        public override void OpenRealPlayer()
        {
            if (!CheckYouMiTalkVoiceJoinState())
            {
                return;
            }
            YouMeVoiceAPI.GetInstance().SetSpeakerMute(false);
            LogForYouMiTalk("OpenRealPlayer", YouMeErrorCode.YOUME_SUCCESS);
        }

        public override void CloseReaPlayer()
        {
            if (!CheckYouMiTalkVoiceJoinState())
            {
                return;
            }
            YouMeVoiceAPI.GetInstance().SetSpeakerMute(true);
            LogForYouMiTalk("CloseReaPlayer", YouMeErrorCode.YOUME_SUCCESS);
        }

        //麦克风是否开启
        public override bool IsTalkRealMicOn()
        {
            if (!CheckYouMiTalkVoiceJoinState())
            {
                return false;
            }
            isTalkMicOn = !YouMeVoiceAPI.GetInstance().GetMicrophoneMute();
            return isTalkMicOn;
        }

        //扬声器是否开启
        public override bool IsTalkRealPlayerOn()
        {
            if (!CheckYouMiTalkVoiceJoinState())
            {
                return false;
            }
            isTalkPlayerOn = !YouMeVoiceAPI.GetInstance().GetSpeakerMute();
            return isTalkPlayerOn;
        }

        public override void SetPlayerVolume(float volume)
        {
            try
            {
                if (volume <= 0)
                {
                    volume = 0;
                }
                else if (volume > 1f)
                {
                    volume = 1f;
                }
                YouMeVoiceAPI.GetInstance().SetVolume((uint)(volume * 100));
                playerVolume = volume;
                LogForYouMiTalk("SetPlayerVolume", YouMeErrorCode.YOUME_SUCCESS, "set volume is " + playerVolume);
            }
            catch (System.Exception e)
            {
                Logger.LogErrorFormat("Real Talk Voice Set Volumn is Failed : {0}", e.ToString());
            }
        }

        public override float GetPlayerVolume()
        {
            int v = YouMeVoiceAPI.GetInstance().GetVolume();
            return v / 100;
        }

        //暂停通话
        public override void PauseChannel()
        {
            if(voiceTalkPauseState == VoiceTalkPauseState.Resumed)
            {
                talkMethodResult = YouMeVoiceAPI.GetInstance().PauseChannel();
                voiceTalkPauseState = VoiceTalkPauseState.Pausing;
                LogForYouMiTalk("PauseChannel", YouMeErrorCode.YOUME_SUCCESS);
            }
        }

        public override void ResumeChannel()
        {
            if(voiceTalkPauseState == VoiceTalkPauseState.Paused)
            {
                talkMethodResult = YouMeVoiceAPI.GetInstance().ResumeChannel();
                voiceTalkPauseState = VoiceTalkPauseState.Resuming;
                LogForYouMiTalk("ResumeChannel", YouMeErrorCode.YOUME_SUCCESS);
            }
        }

        //是否在实时语音频道
        public override bool BeInRealVoiceChannel() 
        {
            LogForYouMiTalk("BeInRealVoiceChannel",YouMeErrorCode.YOUME_SUCCESS,"curr join channel state is "+joinChannelState.ToString());
            
            return joinChannelState == JoinTalkChannelState.Joined || joinChannelState == JoinTalkChannelState.Joining ? true : false;
        }

        #endregion

        #region PRIVATE METHOD  -  Chat voice
        /// <summary>
        /// 播放语音
        /// </summary>
        /// <param name="voicePath"></param>
        bool TryPlayVoiceByPath(string voicePath)
        {
            if (!CheckYoumiChatVoiceLoginState())
            {
                return false;
            }
            //反复播放 不需要调用停止播放！
            chatMethodResult = IMAPI.Instance().StartPlayAudio(voicePath);
            LogForYouMiChat("PlayVoiceSelected", chatMethodResult, "curr play voice path is " + voicePath);
            if (chatMethodResult == ErrorCode.Success)
            {
                onVoiceChatPlayStart();
                return true;
            }
            else if (chatMethodResult == ErrorCode.PTT_IsPlaying)
            {
                //先通知其他播放的暂停  再通知当前选中的开始
                onVoiceChatPlayStart();
                return true;
            }
            else if (chatMethodResult == ErrorCode.NotLogin)
            {
                TryLoginYoumiChatVoice(cacheChatVoiceAccInfo);
            }
            return false;
        }

        /// <summary>
        /// 暂停播放语音
        /// </summary>
        /// <returns></returns>
        bool TryStopPlayVoice()
        {
            if (!CheckYoumiChatVoiceLoginState(false))
            {
                return false;
            }
            chatMethodResult = IMAPI.Instance().StopPlayAudio();
            LogForYouMiChat("StopPlayVoice", chatMethodResult);
            if (chatMethodResult == ErrorCode.Success)
            {
                return true;
            }
            return false;
        }

        bool CheckYoumiChatVoiceLoginState(bool beTryLogin = true)
        {
            if (loginVoiceState == LoginVoiceState.Logined)
            {
                return true;
            }
            else if (loginVoiceState == LoginVoiceState.Logouted)
            {
                if (beTryLogin)
                {
                    LogForYouMiChat("CheckYoumiChatVoiceLoginState", ErrorCode.Success, "beTryLogin current loginState is " + loginVoiceState.ToString());
                    TryLoginYoumiChatVoice(cacheChatVoiceAccInfo);
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        void TryLoginYoumiChatVoice(YoumiVoiceGameAccInfo accInfo)
        {
            if (accInfo == null)
            {
                LogForYouMiChat("TryLoginYoumiChatVoice", ErrorCode.NotLogin, "TryLoginYoumiChatVoice accinfo is null");
                return;
            }

            if (string.IsNullOrEmpty(accInfo.RoleId) || string.IsNullOrEmpty(accInfo.OpenId))
            {
                LogForYouMiChat("TryLoginYoumiChatVoice", ErrorCode.NotLogin, "TryLoginYoumiChatVoice accinfo RoleId is " + accInfo.RoleId + ", OpenId is " + accInfo.OpenId);
                return;
            }

            LogForYouMiChat("TryLoginYoumiChatVoice isChatVoiceInited : ", ErrorCode.Success, isChatVoiceInited.ToString());

            if (!isChatVoiceInited)
            {
                return;
            }

            LogForYouMiChat("TryLoginYoumiChatVoice loginVoiceState : ", ErrorCode.Success, loginVoiceState.ToString());

            if (loginVoiceState != LoginVoiceState.Logouted)
            {
                return;
            }

            chatMethodResult = IMAPI.Instance().Login(accInfo.RoleId, accInfo.OpenId, accInfo.Token);
            loginVoiceState = LoginVoiceState.Logining;
            LogForYouMiChat("TryLoginYoumiChatVoice", chatMethodResult, "roleId " + accInfo.RoleId + "| pass " + accInfo.OpenId + "| token " + accInfo.Token);

            if (chatMethodResult == ErrorCode.Success)
            {
                LogForYouMiChat("TryLoginYoumiChatVoice", chatMethodResult, "is Logining !!!");
            }
            else if (chatMethodResult == ErrorCode.AlreadyLogin)
            {
                LogForYouMiChat("TryLoginYoumiChatVoice", chatMethodResult, "已经登录聊天语音了");
                TryLogoutYoumiChatVoice();
            }
            else if (chatMethodResult == ErrorCode.ParamInvalid)
            {
                LogForYouMiChat("TryLoginYoumiChatVoice", chatMethodResult, "ErrorCode.ParamInvalid !!!");
                loginVoiceState = LoginVoiceState.Logouted;
            }
            else
            {
                LogForYouMiChat("TryLoginYoumiChatVoice", chatMethodResult, "ErrorCode not success !!!");
                loginVoiceState = LoginVoiceState.Logouted;
            }
        }

        void TryLogoutYoumiChatVoice()
        {
            if (!isChatVoiceInited)
            {
                return;
            }

            LogForYouMiChat("TryLogoutYoumiChatVoice loginVoiceState : ", ErrorCode.Success, loginVoiceState.ToString());
            if (loginVoiceState == LoginVoiceState.Logouting)
            {
                return;
            }
            chatMethodResult = IMAPI.Instance().Logout();
            if (chatMethodResult != ErrorCode.Success)
            {
                loginVoiceState = LoginVoiceState.Logouted;
            }
            else
            {
                loginVoiceState = LoginVoiceState.Logouting;
            }
            LogForYouMiChat("TryLogoutYoumiChatVoice", chatMethodResult);
        }

        /// <summary>
        /// 语音聊天下载方法 
        /// 
        /// 不需要构建下载队列 内部已构建了的！！！
        /// </summary>
        /// <param name="audio"></param>
        void DownloadAudioFile(AudioInfo audio)
        {
            if (!CheckYoumiChatVoiceLoginState())
            {
                return;
            }

            if (null == audio)
            {
                LogForYouMiChat("DownloadAudioFile audio", ErrorCode.Fail, "AudioInfo audio  is null");
                return;
            }

            chatMethodResult = IMAPI.Instance().DownloadAudioFile(audio.requestId, audio.path);

            LogForYouMiChat("DownloadAudioFile audio", chatMethodResult, string.Format("AudioInfo audio requestId is {0}, path is {1}", audio.requestId, audio.path));

            if (chatMethodResult == ErrorCode.Success)
            {
                audio.status = DownloadStatus.DS_DOWNLOADING;
            }
            else if (chatMethodResult == ErrorCode.NotLogin)
            {
                TryLoginYoumiChatVoice(cacheChatVoiceAccInfo);
            }
        }

        void TryJoinChatRoom(string chatRoomId, bool beSaveRoomMsg)
        {

            //新增保存房间历史消息
            if (string.IsNullOrEmpty(chatRoomId))
            {
                return;
            }    
            //是否需要存储本地消息
            if (beSaveRoomMsg)
            {
                SetSaveHistoryChatRoomMsg(chatRoomId, beSaveRoomMsg);
                LogForYouMiChat("SetSaveHistoryChatRoomMsg", ErrorCode.Success, "chatRoomId is " + chatRoomId);
            }     
            chatMethodResult = IMAPI.Instance().JoinChatRoom(chatRoomId);
            LogForYouMiChat("JoinChatRoom", chatMethodResult, "JoinChatRoom id is " + chatRoomId);
        }

        void TryLeaveChatRoom(string chatRoomId)
        {
            chatMethodResult = IMAPI.Instance().LeaveChatRoom(chatRoomId);
            LogForYouMiChat("LeaveChatRoom", chatMethodResult, "LeaveChatRoom id is " + chatRoomId);
        }

        bool PlayVoiceQueue()
        {
            //如果当前正在播放语音，则再次调用此方法时不会重复进行播放
            if (IsVoicePlaying())
                return false;
            if (this.voiceQueue != null)
            {
                if (this.voiceQueue.Count <= 0)
                {
                    return false;
                }
                string voiceKey = this.voiceQueue.Dequeue();
                if (!string.IsNullOrEmpty(voiceKey))
                {
                    // return TryPlayVoiceByPath(path);
                    PlayVoiceCommon(voiceKey);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 新增保存房间历史消息
        /// </summary>
        /// <param name="chatRoomId"></param>
        /// <param name="bSave"></param>
        void SetSaveHistoryChatRoomMsg(string chatRoomId, bool bSave)
        {
            if (!isChatVoiceInited)
            {
                return;
            }
            List<string> chatRoomIds = new List<string>() { chatRoomId };
            IMAPI.Instance().SetRoomHistoryMessageSwitch(chatRoomIds, bSave);
        }

        /// <summary>
        /// 删除语音 一段时间前的语音消息 （包括了语音音频文件）
        /// </summary>
        /// <param name="day"></param>
        void DeleteRoomChatMsgOnTime(uint day)
        {
            if (!CheckYoumiChatVoiceLoginState(false))
            {
                return;
            }
            uint secondCount = day * 24 * 60 * 60;
            ulong currTimeStamp = SDKVoiceManager.GetInstance().GetServerTimeStamp();
            ulong timeStamp = currTimeStamp - secondCount;
            chatMethodResult = IMAPI.Instance().DeleteHistoryMessage(YIMEngine.ChatType.RoomChat, timeStamp);
            LogForYouMiChat("DeletePrivateChatMsgOnTime", chatMethodResult, "Delete history chat msg by curr time, type Private Chat");
        }

        void DeleteRoomChatMsgByMsgId(ulong msgId)
        {
            chatMethodResult = IMAPI.Instance().DeleteHistoryMessageByID(msgId);
            LogForYouMiChat("DeleteRoomChatMsgByMsgId", chatMethodResult, "Delete history chat msg by msg id : " + msgId);
        }


        /// <summary>
        ///  倒序查询!!!，每次查询一定数量消息，判断是否存在对应token的语音
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="onceQueryMsgCount"></param>
        /// 
        /// targetID：私聊用户的id
        /// chatType：表示查询私聊或者频道聊天的历史记录
        /// startMessageID：起始历史记录消息id（与requestid不同），为0表示首次查询，将倒序获取count条记录
        /// count：最多获取多少条
        /// direction：历史记录查询方向，startMessageID>0时，0表示查询比startMessageID小的消息，1表示查询比startMessageID大的消息
        /// 
        void QueryPrivateHistoryMsgByUserId(string targetId, int onceQueryMsgCount, ulong startId = 0)
        {
            if (!isChatVoiceInited)
            {
                return;
            }
            if (onceQueryMsgCount < 0)
            {
                return;
            }
            chatMethodResult = IMAPI.Instance().QueryHistoryMessage(targetId, YIMEngine.ChatType.RoomChat, startId, onceQueryMsgCount, 0);//最后一位为0为倒序查
            if (chatMethodResult == ErrorCode.Success)
            {
                isQueryHistoryMsgClear = false;
            }
            LogForYouMiChat("QueryPrivateHistoryMsgByUserId", chatMethodResult, string.Format("query History private msg , tarId {0} , once query count {1}", targetId, onceQueryMsgCount));
        }

        void QueryHistoryMsgToClear(string chatRoomId, int onceQueryMsgCount, ulong startId = 0)
        {
            if (!isChatVoiceInited)
            {
                return;
            }
            chatMethodResult = IMAPI.Instance().QueryHistoryMessage(chatRoomId, YIMEngine.ChatType.RoomChat, startId, onceQueryMsgCount, 0);
            if (chatMethodResult == ErrorCode.Success)
            {
                isQueryHistoryMsgClear = true;
            }
            LogForYouMiChat("QueryHistoryMsgToClear !", chatMethodResult, string.Format("query History private msg , tarId {0} , once query count {1}", chatRoomId, privateChatQueryMsgCount));
        }
        #endregion

        #region PRIVATE METHOD  -  Talk Voice

        void TryInitTalkVoice()
        {
#if UNITY_STANDALONE_OSX
            return;
#endif
            talkMethodResult = YouMeVoiceAPI.GetInstance().Init(AppKey, AppSecret, YOUME_RTC_SERVER_REGION.RTC_CN_SERVER, "cn");
            LogForYouMiTalk("TryInitTalkVoice", talkMethodResult);

            if (talkMethodResult == YouMeErrorCode.YOUME_ERROR_WRONG_STATE)
            {
                isTalkVoiceInited = true;
            }
        }

        void TryJoinYoumiTalkChannel(YoumiVoiceGameAccInfo accInfo)
        {
            if (accInfo == null)
            {
                LogForYouMiTalk("TryJoinYoumiTalkChannel", YouMeErrorCode.YOUME_ERROR_INVALID_PARAM, "TryLoginYoumiChatVoice accinfo is null");
                return;
            }
            if (accInfo.RoleId == "" || accInfo.TalkChannelId == "")
            {
                LogForYouMiTalk("TryJoinYoumiTalkChannel", YouMeErrorCode.YOUME_ERROR_INVALID_PARAM, "TryLoginYoumiChatVoice accinfo roleid is " + accInfo.RoleId + ", talkChannelId is " + accInfo.TalkChannelId);
                return;
            }
            if (!isTalkVoiceInited)
            {
                //TryInitTalkVoice();
                return;
            }
            if (joinChannelState != JoinTalkChannelState.Leaved)
            {
                LogForYouMiTalk("TryJoinYoumiTalkChannel", YouMeErrorCode.YOUME_ERROR_INVALID_PARAM, "TryLoginYoumiChatVoice but state is not leaved");
                //TryLeaveAllYoumiTalkChannel();
                return;
            }

            talkMethodResult = YouMeVoiceAPI.GetInstance().JoinChannelSingleMode(accInfo.RoleId, accInfo.TalkChannelId, YouMeUserRole.YOUME_USER_TALKER_FREE, false);
            //talkMethodResult = YouMeVoiceAPI.GetInstance().JoinChannelMultiMode(accInfo.RoleId, accInfo.TalkChannelId, false);
            joinChannelState = JoinTalkChannelState.Joining;
            LogForYouMiTalk("TryJoinYoumiTalkChannel JoinChannelSingleMode", talkMethodResult, "res :" + "RoleId : " + accInfo.RoleId + "TalkChannelId : " + accInfo.TalkChannelId +
                "!!! join state = " + joinChannelState.ToString());
        }

        /// <summary>
        /// 离开聊天房间接口可实时调用
        /// </summary>
        void TryLeaveAllYoumiTalkChannel()
        {
            if (joinChannelState != JoinTalkChannelState.Leaving)
            {
                talkMethodResult = YouMeVoiceAPI.GetInstance().LeaveChannelAll();
                joinChannelState = JoinTalkChannelState.Leaving;
                LogForYouMiTalk("LeaveChannel", talkMethodResult, "离开全部频道 : state = " + joinChannelState.ToString());
            }
            else
            {
                LogForYouMiTalk("LeaveChannel", talkMethodResult, "正在离开频道 ：state = " + joinChannelState.ToString());
            }
        }

        // 是否处于完全退出状态 ！！！
        bool CheckYouMiTalkVoiceJoinState()
        {
            if (joinChannelState == JoinTalkChannelState.Leaved)
            {
                //TryJoinYoumiTalkChannel(cacheChatVoiceAccInfo);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 注意 需要在加入频道前调用 !!!!!!!!!!!!
        /// </summary>
        /// <param name="enabled"></param>
        void SetMobileNetworkEnabled(bool enabled)
        {
            if (GetMobileNetworkEnabled() == enabled)
            {
                return;
            }
            YouMeVoiceAPI.GetInstance().SetUseMobileNetworkEnabled(enabled);

            LogForYouMiTalk("SetMobileNetworkEnabled", YouMeErrorCode.YOUME_SUCCESS, "是否允许在移动网络下可用 实时语音 isAllow : " + enabled);
        }

        public bool GetMobileNetworkEnabled()
        {
            return YouMeVoiceAPI.GetInstance().GetUseMobileNetworkEnabled();
        }

        //当麦克风静音时，释放麦克风设备，此时允许第三方模块使用麦克风设备录音。在Android上，语音通过媒体音轨，而不是通话音轨输出。
        void SetYoumiReleaseMicWhenMicOff(bool isRelease)
        {
            if (!CheckYouMiTalkVoiceJoinState())
            {
                return;
            }
            talkMethodResult = YouMeVoiceAPI.GetInstance().SetReleaseMicWhenMute(isRelease);
            LogForYouMiTalk("SetYoumiReleaseMicWhenMicOff", talkMethodResult);
        }

        //设置插耳机的情况下开启或关闭语音监听（即通过耳机听到自己说话的内容）。
        //这是一个同步调用接口。
        void SetYoumiHeadsetMontorOn(bool enabled)
        {
            if (!CheckYouMiTalkVoiceJoinState())
            {
                return;
            }
            talkMethodResult = YouMeVoiceAPI.GetInstance().SetHeadsetMonitorOn(enabled);
            LogForYouMiTalk("SetYoumiHeadsetMontorOn", talkMethodResult);
        }

        //设置是否通知其他人，自己开关麦克风扬声器的状态
        void SetYoumiAutoSendStatus(bool isAutoSend)
        {
            if (!CheckYouMiTalkVoiceJoinState())
            {
                return;
            }
            YouMeVoiceAPI.GetInstance().SetAutoSendStatus(isAutoSend);
            LogForYouMiTalk("SetYoumiAutoSendStatus", YouMeErrorCode.YOUME_SUCCESS);
        }

        //是否输出到扬声器
        void SetYoumiOutputToPlayer(bool outToPlayer)
        {
            if (!CheckYouMiTalkVoiceJoinState())
            {
                return;
            }
            talkMethodResult = YouMeVoiceAPI.GetInstance().SetOutputToSpeaker(outToPlayer);
            LogForYouMiTalk("SetOutputToPlayer", talkMethodResult);
        }

        #endregion

        #region Voice Chat Callback Listener
        public void OnLogin(ErrorCode errorcode, string strYouMeID)
        {
            LogForYouMiChat("OnLogin", errorcode, "login strYouMeID " + strYouMeID);
            loginVoiceState = LoginVoiceState.Logined;

            if (errorcode == ErrorCode.Success)
            {
                loginVoiceState = LoginVoiceState.Logined;
                if (cacheChatVoiceAccInfo != null)
                {
                    cacheChatVoiceAccInfo.YoumiId = strYouMeID;
                }
                if (onVoiceChatLoginHandler != null)
                {
                    onVoiceChatLoginHandler();
                }
            }
            else
            {
                loginVoiceState = LoginVoiceState.Logouted;
            }
        }

        public void OnLogout()
        {
            LogForYouMiChat("OnLogout !", ErrorCode.Success);
            loginVoiceState = LoginVoiceState.Logouted;

            if (onVoiceChatLogoutHandler != null)
            {
                onVoiceChatLogoutHandler();
            }
        }

        public void OnKickOff()
        { }

        public void OnDownload(ErrorCode errorcode, MessageInfoBase message, string strSavePath)
        {
            LogForYouMiChat("OnDownload", errorcode, "message RequestID : " + message.RequestID + " , savePath : " + strSavePath);
            if (errorcode == ErrorCode.Success)
            {
                if (string.IsNullOrEmpty(strSavePath))
                    return;

                VoiceMessage voiceMessage = message as VoiceMessage;
                if (null == voiceMessage)
                {
                    Logger.LogProcessFormat("YouMiVoice--message 异常.");
                    return;
                }

                string ymUserId = "";
                if (cacheChatVoiceAccInfo != null)
                {
                    ymUserId = cacheChatVoiceAccInfo.YoumiId;
                }
                //SDKVoiceManager.GetInstance().ReportUsingVoice(Protocol.CustomLogReportType.CLRT_LOAD_RECORD_VOICE, voiceMessage.RequestID + ""); //todo

                AudioInfo audio = null;
                if (_AudioCache == null)
                {
                    Logger.LogProcessFormat("YouMiVoice--message 语音字典未初始化 为空");
                    return;
                }
                if (!_AudioCache.ContainsKey(voiceMessage.Param))
                {
                    LogForYouMiChat("OnDownload", errorcode, "下载了未知的音频文件");

                    audio = new AudioInfo();
                    audio.token = voiceMessage.Param;
                    audio.requestId = voiceMessage.RequestID;
                    audio.path = strSavePath;
                    audio.status = DownloadStatus.DS_SUCCESS;
                    _AudioCache.Add(voiceMessage.Param, audio);
                    return;
                }
                else
                {
                    audio = _AudioCache[voiceMessage.Param];
                    audio.status = DownloadStatus.DS_SUCCESS;
                    audio.path = strSavePath;
                }

                if (audio.token.Equals(_currentReqPlayAudioToken))
                {
                    TryPlayVoiceByPath(audio.path);
                }
            }
        }

        public void OnDownloadByUrl(ErrorCode errorcode, string strFromUrl, string strSavePath)
        {
            LogForYouMiChat("OnDownloadByUrl", errorcode, "通过URL下载语音文件到指定路径");
        }

        public void OnJoinRoom(ErrorCode errorcode, string strChatRoomID)
        {
            if (errorcode != ErrorCode.Success)
            {
                LogForYouMiChat("OnJoinRoomFailedd", errorcode);
            }
            else
            {
                LogForYouMiChat("OnJoinRoom", errorcode, "strChatRoomID = " + strChatRoomID);
                //GameClient.UIEventSystem.GetInstance().SendUIEvent(GameClient.EUIEventID.OnVoiceChatPrivateJoin, strChatRoomID); //todo..
                if (voiceChatRoomIdList != null)
                {
                    if (!voiceChatRoomIdList.Contains(strChatRoomID))
                    {
                        voiceChatRoomIdList.Add(strChatRoomID);
                    }
                }
            }
        }

        public void OnLeaveRoom(ErrorCode errorcode, string strChatRoomID)
        {
            if (errorcode != ErrorCode.Success)
            {
                LogForYouMiChat("OnLeaveRoomFailed", errorcode);
            }
            else
            {
                LogForYouMiChat("OnLeaveRoom", errorcode, "strChatRoomID = " + strChatRoomID);
                //GameClient.UIEventSystem.GetInstance().SendUIEvent(GameClient.EUIEventID.OnVoiceChatPrivateLeave, strChatRoomID); //todo..
            }
        }

        public void OnLeaveAllRooms(YIMEngine.ErrorCode errorcode)
        {

        }

        public void OnUserJoinChatRoom(string strRoomID, string strUserID)
        {
            LogForYouMiChat("OnUserJoinChatRoom", ErrorCode.Success, "有玩家 : " + strUserID + ",加入了房间 ：strChatRoomID = " + strRoomID);
        }

        public void OnUserLeaveChatRoom(string strRoomID, string strUserID)
        {
            LogForYouMiChat("OnUserLeaveChatRoom", ErrorCode.Success, "有玩家 : " + strUserID + ",离开了房间 ：strChatRoomID = " + strRoomID);
        }

        public void OnGetRoomMemberCount(ErrorCode errorcode, string strRoomID, uint count)
        {
            LogForYouMiChat("OnGetRoomMemberCount", errorcode, strRoomID + " 号房间内当前一共有玩家人数 : " + count);
        }


        public void OnQueryHistoryMessage(ErrorCode errorcode, string targetID, int remain, List<HistoryMsg> messageList)
        {
            LogForYouMiChat("OnQueryHistoryMessage", errorcode, string.Format("OnQueryHistoryMessage targetId is {0}, remain msg num is {1}", targetID, remain));
            if (errorcode == ErrorCode.Success)
            {
                List<HistoryMsg> msgList = messageList;
                if (msgList == null)
                {
                    Logger.LogError("OnQueryHistoryMessage callback messageList is null !!!");
                    return;
                }

                LogForYouMiChat("OnQueryHistoryMessage", errorcode, string.Format("OnQueryHistoryMessage msgListCount is {0}", msgList.Count));

                bool isFindMsg = false;
                int currMsgListCount = msgList.Count;
                int msgTotalCount = currMsgListCount + remain;

                //是否需要进行缓存清理
                if (isQueryHistoryMsgClear)
                {
                    for (int i = 0; i < msgList.Count; i++)
                    {
                        var msg = msgList[i];
                        if (msg.MessageType != MessageBodyType.Voice)
                        {
                            return;
                        }
                        string voiceParam = msg.Param;
                        LogForYouMiChat("OnQueryHistoryMessage", errorcode, string.Format("OnQueryHistoryMessage msg.Param is {0}", voiceParam));
                        ChanelType chatType = SDKVoiceManager.GetInstance().GetChatTypeInVoiceKey(voiceParam);
                        if (chatType != ChanelType.CHAT_CHANNEL_PRIVATE)
                        {
                            DeleteRoomChatMsgByMsgId(msg.MessageID);
                        }
                    }

                    if (remain <= 0)
                    {
                        if (voiceChatRoomIdList == null)
                        {
                            TryLogoutYoumiChatVoice();
                            return;
                        }
                        else
                        {
                            if (voiceChatRoomIdList.Contains(targetID))
                            {
                                voiceChatRoomIdList.Remove(targetID);
                            }
                            if (voiceChatRoomIdList.Count <= 0)
                            {
                                TryLogoutYoumiChatVoice();
                                return;
                            }
                            QueryHistoryMsgToClear(voiceChatRoomIdList[0], privateChatQueryMsgCount);
                        }
                        return;
                    }

                    int nextStartMsgId = msgTotalCount - currMsgListCount;

                    if (privateChatQueryMsgCount > remain)
                    {
                        QueryHistoryMsgToClear(targetID, remain, (ulong)nextStartMsgId);
                    }
                    else
                    {
                        QueryHistoryMsgToClear(targetID, privateChatQueryMsgCount, (ulong)nextStartMsgId);
                    }

                    return;
                }

                for (int i = 0; i < currMsgListCount; i++)
                {
                    HistoryMsg hMsg = msgList[i];
                    if (hMsg.MessageType == MessageBodyType.Voice && hMsg.ChatType == ChatType.RoomChat)
                    {
                        if (hMsg.Param.Equals(_currentReqPlayAudioToken))
                        {
                            TryPlayVoiceByPath(hMsg.LocalPath);
                            isFindMsg = true;
                            LogForYouMiChat("OnQueryHistoryMessage", errorcode, string.Format("hMsg MessageType sendId is {0} , recId is {1} , MessageID is {2}", hMsg.SenderID, hMsg.ReceiveID, hMsg.MessageID));
                            break;
                        }
                    }
                }
                if (remain <= 0)
                {
                    return;
                }

                if (!isFindMsg)
                {
                    int nextStartMsgId = msgTotalCount - currMsgListCount;

                    if (privateChatQueryMsgCount > remain)
                    {
                        QueryPrivateHistoryMsgByUserId(targetID, remain, (ulong)nextStartMsgId);
                    }
                    else
                    {
                        QueryPrivateHistoryMsgByUserId(targetID, privateChatQueryMsgCount, (ulong)nextStartMsgId);
                    }
                }
            }
        }

        public void OnSendMessageStatus(ulong iRequestID, ErrorCode errorcode, uint sendTime, bool isForbidRoom, int reasonType, ulong forbidEndTime)
        {
        }

        public void OnRecvMessage(MessageInfoBase message)
        {
            if (message == null)
                return;
            if (message.MessageType == MessageBodyType.Voice)
            {
                LogForYouMiChat("OnRecvMessage", ErrorCode.Success, "message is voice");

                VoiceMessage voiceMessage = message as VoiceMessage;
                if (null == voiceMessage)
                {
                    Logger.LogProcessFormat("YouMiVoice--message 异常.");
                    return;
                }

                LogForYouMiChat("OnRecvMessage", ErrorCode.Success, "message is voice  req id is " + voiceMessage.RequestID + " | req token is " + voiceMessage.Param);

                if (string.IsNullOrEmpty(voiceMessage.Param))
                {
                    Logger.LogProcessFormat("YouMiVoice--message 透传参数为空.");
                    return;
                }

                AudioInfo audio = new AudioInfo();
                audio.token = voiceMessage.Param;
                audio.requestId = voiceMessage.RequestID;
                audio.status = DownloadStatus.DS_NOTDOWNLOAD;
                audio.path = BuildRequestIdPath(audio.requestId);
                ChanelType chatType = SDKVoiceManager.GetInstance().GetChatTypeInVoiceKey(voiceMessage.Param);
                audio.bPrivate = chatType == ChanelType.CHAT_CHANNEL_PRIVATE ? true : false; 

                LogForYouMiChat("OnRecvMessage", ErrorCode.Success, string.Format("AudioInfo token : {0}, requestId : {1}, path : {2} , is private ：{3}", audio.token, audio.requestId, audio.path, audio.bPrivate));
                onVoiceChatRecordStart(voiceMessage.Param, voiceMessage.Text, 1, "");   //Test的接受信息
                if (_AudioCache.ContainsKey(voiceMessage.Param))
                {
                    LogForYouMiChat("OnRecvMessage", ErrorCode.Success, "TOKEN冲突");
                    _AudioCache.Remove(voiceMessage.Param);
                }

                _AudioCache.Add(voiceMessage.Param, audio);

                if (audio.token.Equals(_currentReqPlayAudioToken))
                {
                    DownloadAudioFile(audio);
                }
            }
            else
            {
                LogForYouMiChat("OnRecvMessage", ErrorCode.Success, "message is not voice");
            }
        }

        //录音结束，开始发送录音的通知，这个时候已经可以拿到语音文件进行播放
        public void OnStartSendAudioMessage(ulong iRequestID, ErrorCode errorcode, string strText, string strAudioPath, int iDuration)
        {
            isRecording = false;

            LogForYouMiChat("OnStartSendAudioMessage", errorcode, string.Format("reqID {0} , text {1} , path {2} , duration {3}", iRequestID, strText, strAudioPath, iDuration));
            if (errorcode == ErrorCode.Success || errorcode == ErrorCode.PTT_ReachMaxDuration)
            {               
                //如果翻译失败
                if (string.IsNullOrEmpty(strText))
                {
                    strText = "语音消息";//VoiceInputBtnTextCN.VOICE_MESSAGE; 
                }

                if (string.IsNullOrEmpty(_leastLocalAudioToken))
                {
                    LogForYouMiChat("OnStartSendAudioMessage", errorcode, "这条消息没有透传参数");
                    //GameClient.UIEventSystem.GetInstance().SendUIEvent(GameClient.EUIEventID.OnVoiceChatSendFailed); //todo..
                    onVoiceChatSendFailed();
                    return;
                }

                if (_AudioCache.ContainsKey(_leastLocalAudioToken))
                {
                    LogForYouMiChat("OnStartSendAudioMessage", errorcode, "TOKEN冲突");
                    _AudioCache.Remove(_leastLocalAudioToken);
                }
                
                AudioInfo audio = new AudioInfo();
                audio.token = _leastLocalAudioToken;
                audio.requestId = iRequestID;
                audio.path = strAudioPath;
                audio.status = DownloadStatus.DS_SUCCESS;
                //audio.bPrivate = _leasetLocalAudioChatType == GameClient.ChatType.CT_PRIVATE ? true : false;

                _AudioCache.Add(_leastLocalAudioToken, audio);
                if(onVoiceChatRecordStart != null)
                {
                    onVoiceChatRecordStart(_leastLocalAudioToken, strText, iDuration,_lastRoomTypeId);                   
                }
                //重置
                _leastLocalAudioToken = "";
                _lastRoomTypeId = "";
            }
            else
            {
                onVoiceChatRecordFailed((int)errorcode);
            }
        }

        // 自己的语音消息发送成功或者失败的通知
        public void OnSendAudioMessageStatus(ulong iRequestID, ErrorCode errorcode, string strText, string strAudioPath, int iDuration, uint sendTime, bool isForbidRoom, int reasonType, ulong forbidEndTime)
        {
            isRecording = false;

            LogForYouMiChat("OnSendAudioMessageStatus", errorcode, string.Format("reqID {0} , text {1} , path {2} , duration {3} , sendTime {4}", iRequestID, strText, strAudioPath, iDuration, sendTime));
            if (errorcode == ErrorCode.Success || errorcode == ErrorCode.PTT_ReachMaxDuration)
            {
                onVoiceChatSendSucc();
                string ymUserId = "";
                if (cacheChatVoiceAccInfo != null)
                {
                    ymUserId = cacheChatVoiceAccInfo.YoumiId;
                }
            }
            else
            {
                onVoiceChatSendFailed((int)errorcode);
            }
        }



        public void OnStopAudioSpeechStatus(ErrorCode errorcode, ulong iRequestID, string strDownloadURL, int iDuration, int iFileSize, string strLocalPath, string strText)
        {
            LogForYouMiChat("OnStopAudioSpeechStatus", errorcode, string.Format("reqID {0} , text {1} , path {2} , duration {3}", iRequestID, strText, strLocalPath, iDuration));

            if (errorcode == ErrorCode.Success)
            {
                //onVoiceChatRecordEnd();
            }
            else
            {
                //onVoiceChatRecordFailed();
            }
        }

        public void OnPlayCompletion(ErrorCode errorcode, string path)
        {
            LogForYouMiChat("OnPlayCompletion", errorcode, "voice play end , voice path is " + path);
            if (errorcode == ErrorCode.Success ||
                errorcode == ErrorCode.PTT_IsPlaying)  //表示播放到中途被打断
            {
                onVoiceChatPlayEnd();

                //轮训播放语音队列！
                if (this.voiceQueue != null && this.voiceQueue.Count > 0)
                {
                    LogForYouMiChat("Start Play Voice Queue", errorcode, "curr voice queue length is " + this.voiceQueue.Count);
                    PlayVoiceQueue();
                }
                else
                {
                    LogForYouMiChat("Start Play Voice Queue is NULL, could do some things other", errorcode);
                }
            }
        }


        public void OnGetMicrophoneStatus(AudioDeviceStatus status)
        {
            LogForYouMiChat("OnGetMicrophoneStatus", ErrorCode.Success, "curr AudioDeviceStatus is " + status.ToString());
        }

        public void OnGetRecognizeSpeechText(ulong iRequestID, ErrorCode errorcode, string text)
        {
        }

        public void OnRecvNewMessage(YIMEngine.ChatType chatType, string targetID)
        {

        }
        /// <summary>
        /// 获取录音音量回调
        /// </summary>
        /// <param name="volume"></param>
        public void OnRecordVolume(int volume)
        {
            LogForYouMiChat("OnRecordVolume", ErrorCode.Success, string.Format("当前录音音量为 ： " + volume));

        }

        public void OnAccusationResultNotify(AccusationDealResult result, string userID, uint accusationTime)
        {

        }

        public void OnGetForbiddenSpeakInfo(ErrorCode errorcode, List<ForbiddenSpeakInfo> forbiddenSpeakList)
        {

        }

        public void OnBlockUser(ErrorCode errorcode, string userID, bool block)
        {

        }

        public void OnUnBlockAllUser(ErrorCode errorcode)
        {

        }

        public void OnGetBlockUsers(ErrorCode errorcode, List<string> userList)
        {

        }
        public void OnQueryRoomHistoryMessageFromServer(YIMEngine.ErrorCode errorcode, string roomID, int remain, List<YIMEngine.MessageInfoBase> messageList)
        {

        }

        public void OnRecordVolumeChange(float volume)
        {

        }

        public void OnGetContact(List<ContactsSessionInfo> contactLists)
        {

        }

        public void OnGetUserInfo(ErrorCode code, string userID, IMUserInfo userInfo)
        {

        }

        public void OnQueryUserStatus(ErrorCode code, string userID, UserStatus status)
        {

        }
        public void OnUpdateLocation(ErrorCode errorcode, GeographyLocation location)
        {

        }
        public void OnGetNearbyObjects(ErrorCode errorcode, List<RelativeLocation> neighbourList, uint startDistance, uint endDistance)
        {

        }
        public void OnGetDistance(ErrorCode errorcode, string userID, uint distance)
        {
        }
        #endregion

        #region YM Talk Voice Callback Listener

        public void OnTalkVoiceInit(bool isInited, YouMeErrorCode errorCode)
        {
            isTalkVoiceInited = isInited;
            LogForYouMiTalk("OnTalkVoiceInit", errorCode);

            if (isInited)
            {
                //实时语音一些参数配置
                SetMobileNetworkEnabled(true);

                SetYoumiReleaseMicWhenMicOff(true);
            }
        }

        public void OnTalkVoiceJoinChannel(bool isSuccess, YouMeErrorCode errorCode)
        {
            LogForYouMiTalk("OnTalkVoiceJoinChannel", errorCode);
            if (isSuccess)
            {
                if (joinChannelState != JoinTalkChannelState.Leaving)
                {
                    joinChannelState = JoinTalkChannelState.Joined;

                    JoinTalkChannelSucc();

                    //实时语音一些参数配置
                    SetYoumiAutoSendStatus(true);

                    if (cacheChatVoiceAccInfo != null && !string.IsNullOrEmpty(cacheChatVoiceAccInfo.TalkChannelId))
                    {
                        joinTalkRoomStartTime = SDKVoiceManager.GetInstance().GetServerTimeStamp();
                        //SDKVoiceManager.GetInstance().ReportUsingVoice(Protocol.CustomLogReportType.CLRT_JOIN_VOICE_ROOM, cacheChatVoiceAccInfo.TalkChannelId + "|" + joinTalkRoomStartTime); //todo
                    }
                }
            }
            else
            {
                //加这个判断的依据 如果已经触发了离开房间的接口 则不用处理加入房间的回调了
                if (joinChannelState != JoinTalkChannelState.Leaving)
                {
                    joinChannelState = JoinTalkChannelState.Leaved;
                }
            }
        }

        public void OnTalkVoiceLeaveChannel(bool isSuccess, YouMeErrorCode errorCode)
        {
            if (isSuccess)
            {
                joinChannelState = JoinTalkChannelState.Leaved;

                LeaveTalkChannelSucc();

                if (cacheChatVoiceAccInfo != null && !string.IsNullOrEmpty(cacheChatVoiceAccInfo.TalkChannelId))
                {
                    ulong leaveTalkRoomStartTime = SDKVoiceManager.GetInstance().GetServerTimeStamp();
                    ulong inTalkRoomTime = leaveTalkRoomStartTime - joinTalkRoomStartTime;
                    joinTalkRoomStartTime = 0;
                    //SDKVoiceManager.GetInstance().ReportUsingVoice(Protocol.CustomLogReportType.CLRT_QUIT_VOICE_ROOM, cacheChatVoiceAccInfo.TalkChannelId + "|" + inTalkRoomTime); //todo..
                    cacheChatVoiceAccInfo.TalkChannelId = "";
                }
            }
            else
            {
                if (joinChannelState == JoinTalkChannelState.Leaving)
                {
                    joinChannelState = JoinTalkChannelState.Joined;
                }
            }

            LogForYouMiTalk("OnTalkVoiceLeaveChannel", errorCode);
        }

        public void OnTalkVoiceChannelPause(bool isPaused, YouMeErrorCode errorCode)
        {
            LogForYouMiTalk("OnTalkVoiceChannelPause", errorCode, "[YouMe - Voice Talk] " + (isPaused ? "Paused !!! " : "Resumed !!!"));
            if (errorCode != YouMeErrorCode.YOUME_SUCCESS)
            {
                LogForYouMiTalk("OnTalkVoiceChannelPause", errorCode, "[YouMe - Voice Talk] " + (isPaused ? "Paused Failed!!! " : "Resumed Failed!!!"));
                return;
            }
            if (isPaused)
            {
                voiceTalkPauseState = VoiceTalkPauseState.Paused;
                ResumeChannel();
            }
            else
            {
                voiceTalkPauseState = VoiceTalkPauseState.Resumed;
            }
        }
        public void OnTalkVoiceMicOn(bool isOn, YouMeErrorCode errorCode)
        {
            if (errorCode == YouMeErrorCode.YOUME_SUCCESS)
            {
                isTalkMicOn = isOn;

                TalkVoiceMicOpenOn(isOn);
            }
            LogForYouMiTalk("OnTalkVoiceMicOn", errorCode, "Set Mic on : " + isOn);
        }
        public void OnTalkVoicePlayerOn(bool isOn, YouMeErrorCode errorCode)
        {
            if (errorCode == YouMeErrorCode.YOUME_SUCCESS)
            {
                isTalkPlayerOn = isOn;

                TalkVoicePlayerOpenOn(isOn);
            }
            LogForYouMiTalk("OnTalkVoicePlayerOn", errorCode, "Set Player on : " + isOn);
        }

        #endregion

        #region PROTECT METHOD FOR TALK VOICE

        protected override void JoinTalkChannelSucc()
        {
            base.JoinTalkChannelSucc();
        }

        protected override void LeaveTalkChannelSucc()
        {
            base.LeaveTalkChannelSucc();
        }

        protected override void TalkVoiceMicOpenOn(bool isOn)
        {
            base.TalkVoiceMicOpenOn(isOn);
        }

        protected override void TalkVoicePlayerOpenOn(bool isOn)
        {
            base.TalkVoicePlayerOpenOn(isOn);
        }

        #endregion
        #region Log For Youmi
        void LogForYouMiChat(string method, ErrorCode errorCode, string errorMsg = "", SDKVoiceLogLevel logLevel = SDKVoiceLogLevel.Error)
        {
            if (this.logLevel > 0)
            {
                cacheLogMsg = string.Format("[youmi voice] - Chat - method : {0} , errorCode : {1} , errorMsg : {2}", method, errorCode, errorMsg);
                showDebugLogUIHandler(cacheLogMsg);
                SetLogLevel(cacheLogMsg, logLevel);
            }
        }
        void LogForYouMiTalk(string method, YouMeErrorCode errorCode, string errorMsg = "", SDKVoiceLogLevel logLevel = SDKVoiceLogLevel.Error)
        {
            if (this.logLevel > 0)
            {
                cacheLogMsg = string.Format("[youmi voice] - Talk -method : {0} , errorCode : {1} , errorMsg : {2}", method, errorCode, errorMsg);
                showDebugLogUIHandler(cacheLogMsg);
                SetLogLevel(cacheLogMsg, logLevel);
            }
        }

        void SetLogLevel(string log, SDKVoiceLogLevel logLevel)
        {
            switch (logLevel)
            {
                case SDKVoiceLogLevel.Error:
                    Logger.LogError(log);
                    break;
                case SDKVoiceLogLevel.Warning:
                    Logger.LogProcessFormat(log);
                    break;
                case SDKVoiceLogLevel.Debug:
                    Logger.Log(log);
                    break;
            }
        }
        #endregion
    }
}