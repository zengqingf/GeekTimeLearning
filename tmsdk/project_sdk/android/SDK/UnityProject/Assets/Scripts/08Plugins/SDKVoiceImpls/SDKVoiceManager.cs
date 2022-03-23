using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using YIMEngine;
using VoiceSDK;
using SDKClient;
//using Tenmove.Runtime.Client.Model.ChatData;

public class SDKVoiceManager : Singleton<SDKVoiceManager>
{
    //SDKVoiceInterface sdkInterface = new YouMiVoiceInterface();

    public static bool isInit = false;
    public bool IsChatVoiceEnabled { get; private set; }
    public bool IsTalkRealVoiceEnabled { get; private set; }
    public bool IsAutoPlayVoice { get; private set; }



    protected  ChanelType gameChatType;
    public ChanelType GameChatType
    {
        get { return gameChatType; }
        set { gameChatType = value; }
    }

    #region PUBLIC METHOD

    #region Menber Methods

    public override void Init()
    {
        isInit = true;
    }
    public override void UnInit()
    {
        Logger.LogProcessFormat("!!! SDKVoiceManager UnInit !!!");

            //清理语音自动播放队列
            ClearVoicePathQueue();


            //注意调用顺序 ， 先进行反初始化前的调用！！！
            UnInitChatVoice();
            UnInitTalkVoice();   

        isInit = false;
    }

    /// <summary>
    /// 初始化语音sdk模块  
    /// </summary>
    /// <param name="chatVoiceEnabled"></param>
    /// <param name="talkVoiceEnabled"></param>
    public void InitVoiceEnabled(bool chatVoiceEnabled, bool talkVoiceEnabled) 
    {
        IsChatVoiceEnabled = chatVoiceEnabled;
        IsTalkRealVoiceEnabled = talkVoiceEnabled;

        if (!IsChatVoiceEnabled && !IsTalkRealVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.Init();
    }

    public void LeaveVoiceSDK(bool beLogout = false)
    {
        LeaveChatRoom();
        if (beLogout)
        {
            LogoutVoice();
        }
    }

    /// <summary>
    /// 设置缓存路径
    /// </summary>
    /// <param name="path"></param>
    public void SetVoiceSavePath(string path)
    {
        SDKVoiceInterface.Instance.SaveVoiceCachePath = path;
    }
    /// <summary>
    /// 设置输出日志等级
    /// </summary>
    /// <param name="level"></param>
    public void SetVoiceDebugLevel(SDKVoiceLogLevel level)
    {
        SDKVoiceInterface.Instance.SetLogLevel(level);
    }

    public void ControlRealVoiceMic()
    {
        bool isMicOn = IsTalkRealMicOn();
        if (isMicOn)
        {
            CloseRealMic();
        }
        else
        {
            OpenRealMic();
        }
    }

    public void ControlRealVociePlayer()
    {
        bool isPlayerOn = IsTalkRealPlayerOn();
        if (isPlayerOn)
        {
            CloseReaPlayer();
        }
        else
        {
            OpenRealPlayer();
        }
    }
    #endregion

    #region VoiceImpl Methods


    /* Youmi Chat start */
    public void InitChatVoice(ChatDelegeInfo delegeInfo)
    {
        if (!IsChatVoiceEnabled)
            return;
            if(delegeInfo.OnVoiceChatLogin != null)
            {
            SDKVoiceInterface.Instance.onVoiceChatLoginHandler = delegeInfo.OnVoiceChatLogin;
            }
            if(delegeInfo.OnVoiceChatLogout != null)
            {
            SDKVoiceInterface.Instance.onVoiceChatLogoutHandler = delegeInfo.OnVoiceChatLogout;
            }
            if (delegeInfo.OnVoiceChatRecordStart != null)
            {
            SDKVoiceInterface.Instance.onVoiceChatRecordStart = delegeInfo.OnVoiceChatRecordStart;
            }
            if (delegeInfo.OnVoiceChatRecordEnd != null)
            {
            SDKVoiceInterface.Instance.onVoiceChatRecordEnd = delegeInfo.OnVoiceChatRecordEnd;
            }
            if (delegeInfo.OnVoiceChatRecordFailed != null)
            {
            SDKVoiceInterface.Instance.onVoiceChatRecordFailed = delegeInfo.OnVoiceChatRecordFailed;
            }
            if(delegeInfo.OnVoiceChatSendSucc != null)
            {
            SDKVoiceInterface.Instance.onVoiceChatSendSucc = delegeInfo.OnVoiceChatSendSucc;
            }
            if (delegeInfo.OnVoiceChatSendFailed != null)
            {
            SDKVoiceInterface.Instance.onVoiceChatSendFailed = delegeInfo.OnVoiceChatSendFailed;
            }
            if (delegeInfo.OnVoiceChatPlayStart != null)
            {
            SDKVoiceInterface.Instance.onVoiceChatPlayStart = delegeInfo.OnVoiceChatPlayStart;
            }
            if (delegeInfo.OnVoiceChatPlayEnd != null)
            {
            SDKVoiceInterface.Instance.onVoiceChatPlayEnd = delegeInfo.OnVoiceChatPlayEnd;
            }
        SDKVoiceInterface.Instance.InitChatVoice();
    }

    /// <summary>
    /// 反初始化 强烈建议只调用一次 根据SDK需求
    /// </summary>
    public void UnInitChatVoice()
    {
        if (!IsChatVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.onVoiceChatRecordStart = null;
        SDKVoiceInterface.Instance.onVoiceChatRecordEnd = null;
        SDKVoiceInterface.Instance.onVoiceChatRecordFailed = null;
        SDKVoiceInterface.Instance.onVoiceChatSendSucc = null;
        SDKVoiceInterface.Instance.onVoiceChatSendFailed = null;
        SDKVoiceInterface.Instance.onVoiceChatPlayStart = null;
        SDKVoiceInterface.Instance.onVoiceChatPlayEnd = null;
        SDKVoiceInterface.Instance.UnInitChatVoice();
    }

    public void LoginVoice(YoumiVoiceGameAccInfo userInfo)
    {
        if (!IsChatVoiceEnabled)
            return;

            if(string.IsNullOrEmpty(userInfo.OpenId))
            { userInfo.OpenId = "123456"; }
        SDKVoiceInterface.Instance.LoginVoice(userInfo);
    }

    public void LogoutVoice()
    {
        if (!IsChatVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.LogoutVoice();
    }

    /// <summary>
    /// 加入房间
    /// </summary>
    /// <param name="roomId"></param>
    /// <param name="beSaveRoomMsg"></param>
    public void JoinChatRoom(SDKVoiceRoomInfo roomInfo)
    {
        if (!IsChatVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.JoinChatRoom(roomInfo);
    }

    /// <summary>
    /// 离开房间
    /// </summary>
    /// <param name="roomInfo"></param>
    public void LeaveChatRoom()
    {
        if (!IsChatVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.LeaveChatRoom();
    }

    /// <summary>
    /// 点击按钮开始录音
    /// </summary>
    /// <param name="chattype"></param>
    /// <param name="onSendVoiceCommon"></param>
    public void StartRecordCommon(SDKVoiceRecordInfo recordInfo)
    {
        ulong voiceRequestId = 0;        //消息序列号，用于校验一条消息发送成功与否的标识
        if (!IsChatVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.SendVoiceMessage(recordInfo, ref voiceRequestId);
    }

    /// <summary>
    /// 松手停止录音
    /// </summary>
    public void StopRecordCommon(SDKVoiceToken voiceToken)
    {
        if (!IsChatVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.StopAudioMessage(voiceToken);
    }

    /// <summary>
    ///  播放语音
    /// </summary>
    /// <param name="voiceKey"></param>
    public void PlayVoiceCommon(string voicekey, SDKVoiceRoomInfo roomInfo = null)
    {
        if (!IsChatVoiceEnabled)
            return;
            //string privateVoiceId = voiceToken.targetId > voiceToken.roleId ? string.Format("{0}_{1}", voiceToken.roleId, voiceToken.targetId) : string.Format("{0}_{1}", voiceToken.targetId, voiceToken.roleId);
            //string privateUserId = string.Format("{0}_{1}_{2}", voiceToken.serverId, SDKInterface.SDKChannelInfo.channelParam, voiceToken.roleId);
        SDKVoiceInterface.Instance.PlayVoiceCommon(voicekey, roomInfo); 
    }

    

    /// <summary>
    /// 停止播放语音
    /// </summary>
    public void StopPlayVoice()
    {
        if (!IsChatVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.StopPlayVoice();
    }

    /// <summary>
    /// 上滑取消录音
    /// </summary>
    public void CancelRecordVoice()
    {
        if (!IsChatVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.CancelRecordVoice();
    }


    public void AddVoicePathInQueue(SDKVoiceToken voiceToken)
    {
        if (!IsChatVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.AddVoicePathInQueue(voiceToken.voicekey);
    }

    public void ClearVoicePathQueue()
    {
        if (!IsChatVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.ClearVoicePathQueue();
    }
    /// <summary>
    /// 设置音量
    /// </summary>
    /// <param name="volume"></param>
    public void SetVoiceVolume(float volume)
    {
        if (!IsChatVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.SetVoiceVolume(volume);
    }

    /// <summary>
    /// 获得音量
    /// </summary>
    /// <returns></returns>
    public float GetVoiceVolume()
    {
        if (!IsChatVoiceEnabled)
            return 0f;
            return SDKVoiceInterface.Instance.GetVoiceVolume();
    }

    public void ClearLocalCache()
    {
        if (!IsChatVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.ClearLocalCache();
    }

    public void ClearVoiceChatCache()
    {
        if (!IsChatVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.ClearVoiceChatMsgCache();
    }

    public bool IsVoiceRecording()
    {
        if (!IsChatVoiceEnabled)
            return false;
            return SDKVoiceInterface.Instance.IsVoiceRecording();
    }

    public bool IsVoicePlaying()
    {
        if (!IsChatVoiceEnabled)
            return false;
            return SDKVoiceInterface.Instance.IsVoicePlaying();
    }


    public bool CheckLoginChatVoice()
    {

            return SDKVoiceInterface.Instance.CheckLoginChatVoice();
    }
    /* Youmi Chat end */
    #endregion
    #endregion

    #region Talk Voice Methods
    public void InitTalkVoice()
    {
        if (!IsTalkRealVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.InitTalkVoice();
    }

    /// <summary>
    /// 反初始化 强烈建议只调用一次 根据SDK需求
    /// </summary>
    public void UnInitTalkVoice()
    {
        if (!IsTalkRealVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.UnInitTalkVoice();
    }

    //当前是否在实时语音频道
    public bool BeInRealVoiceChannel()
    {
        if (!IsTalkRealVoiceEnabled)
            return false;
            return SDKVoiceInterface.Instance.BeInRealVoiceChannel();
    }

    public void AddRealVoiceHandler(SDKVoiceInterface.OnJoinChannelSuccess joinSuccHandler,
        SDKVoiceInterface.OnVoiceMicOn voiceMicHandler,
        SDKVoiceInterface.OnVoicePlayerOn voicePlayerHandler)
    {

        //this.onJoinChannalSuccHandler = joinSuccHandler;
        if (joinSuccHandler != null)
        {
            SDKVoiceInterface.Instance.onJoinChannelSucc += joinSuccHandler;//this.onJoinChannalSuccHandler;
        }
        //this.onVoiceMicOnHandler = voiceMicHandler;
        if (voiceMicHandler != null)
        {
            SDKVoiceInterface.Instance.onVoiceMicOn += voiceMicHandler;//this.onVoiceMicOnHandler;
        }
        //this.onVoicePlayerOnHandler = voicePlayerHandler;
        if (voicePlayerHandler != null)
        {
            SDKVoiceInterface.Instance.onVoicePlayerOn += voicePlayerHandler;//this.onVoicePlayerOnHandler;
        }
    }

    public void RemoveRealVoiceHandler(SDKVoiceInterface.OnJoinChannelSuccess joinSuccHandler,
        SDKVoiceInterface.OnVoiceMicOn voiceMicHandler,
        SDKVoiceInterface.OnVoicePlayerOn voicePlayerHandler)
    {
        if (joinSuccHandler != null)
        {
            SDKVoiceInterface.Instance.onJoinChannelSucc -= joinSuccHandler;//this.onJoinChannalSuccHandler;
        }
        if (voiceMicHandler != null)
        {
            SDKVoiceInterface.Instance.onVoiceMicOn -= voiceMicHandler;//this.onVoiceMicOnHandler;
        }
        if (voicePlayerHandler != null)
        {
            SDKVoiceInterface.Instance.onVoicePlayerOn -= voicePlayerHandler;//this.onVoicePlayerOnHandler;
        }
    }

    public void JoinChannel(string channelId, string roleId, string openId, string token)
    {
        if (!IsTalkRealVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.JoinChannel(channelId , roleId, openId);
    }

    public void LeaveAllChannel()
    {
        if (!IsTalkRealVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.LeaveAllChannel();
    }

    public void LeaveChannel(string channelId)
    {
        if (!IsTalkRealVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.LeaveChannel(channelId);
    }

    public void OpenRealMic()
    {
        if (!IsTalkRealVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.OpenRealMic();
    }

    public void CloseRealMic()
    {
        if (!IsTalkRealVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.CloseRealMic();
    }

    public void OpenRealPlayer()
    {
        if (!IsTalkRealVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.OpenRealPlayer();
    }

    public void CloseReaPlayer()
    {
        if (!IsTalkRealVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.CloseReaPlayer();
    }

    public bool IsTalkRealMicOn()
    {
        if (!IsTalkRealVoiceEnabled)
            return false;
            return SDKVoiceInterface.Instance.IsTalkRealMicOn();
    }

    public bool IsTalkRealPlayerOn()
    {
        if (!IsTalkRealVoiceEnabled)
            return false;
            return SDKVoiceInterface.Instance.IsTalkRealPlayerOn();
    }

    // 设置当前程序输出音量大小。建议该状态值在加入房间成功后按需再重置一次!!!
    public void SetPlayerVolume(float volume)
    {
        if (!IsTalkRealVoiceEnabled)
            return;

        SDKVoiceInterface.Instance.SetPlayerVolume(volume);
    }
    public float GetPlayerVolume()
    {
        if (!IsTalkRealVoiceEnabled)
            return 0f;
            return SDKVoiceInterface.Instance.GetPlayerVolume();
    }

    public void PauseChannel()
    {
        if (!IsTalkRealVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.PauseChannel();
    }

    public void ResumeChannel()
    {
        if (!IsTalkRealVoiceEnabled)
            return;
        SDKVoiceInterface.Instance.ResumeChannel();
    }
    #endregion

    #region Game Params

    public ulong GetServerTimeStamp()
    {
        ulong serverTimeStamp = 0;
        try
        {
            serverTimeStamp = (ulong)System.DateTime.Now.Ticks;           
        }
        catch (System.Exception e)
        {
            Logger.LogError("get server timeStamp error" + e.ToString());
        }
        return serverTimeStamp;
    }

    /// <summary>
    /// 得到语音类型
    /// </summary>
    /// <param name="voiceKey"></param>
    /// <returns></returns>
    public ChanelType GetChatTypeInVoiceKey(string voiceKey)
    {
        ChanelType chatType = ChanelType.CHAT_CHANEL_COM;
        if (string.IsNullOrEmpty(voiceKey))
        {
            Logger.LogProcessFormat("GetAccidInVoiceKey voiceKey is null");
            return chatType;
        }

        string[] voiceKeyArr = voiceKey.Split('_');
        if (voiceKeyArr != null && voiceKeyArr.Length == 5)
        {
            if (string.IsNullOrEmpty(voiceKeyArr[4]))
            {
                return chatType;
            }
            int typeInt = -1;
            if (int.TryParse(voiceKeyArr[4], out typeInt))
            {
                if (typeInt < 0)
                {
                    return chatType;
                }
                return (ChanelType)typeInt;
            }
        }
        return chatType;
    }

    #endregion

    #region Report voice sdk using 
    //todo..
    #endregion

}
