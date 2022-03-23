using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMSDKClient
{
    public enum SDKEventType
    {
        None,

        /*********************************** Voice Start ************************************/
        ChatLogin,
        ChatLogout,
        ChatNotJoinRoom,
        ChatRecordStart,
        ChatRecordEnd,
        ChatRecordFailed,
        ChatRecordVolumeChanged,        //录音音量              
        ChatSendStart,
        ChatSendEnd,
        ChatSendFailed,
        ChatSendEndReport,             //埋点
        ChatPlayStart,
        ChatPlayEnd,
        ChatJoinRoom,
        ChatLeaveRoom,
        ChatDownloadRecordVoiceReport, //埋点
        TalkInitSucc,
        TalkInitFailed,
        TalkJoinChannel,
        TalkJoinChannelSucc,
        TalkJoinChannelSuccReport,     //埋点
        TalkLeaveChannel,
        TalkLeaveChannelSucc,
        TalkLeaveChannelSuccReport, //埋点
        TalkPauseChannel,
        TalkResumeChannel,
        TalkMicSwitch,
        TalkMicOn,
        TalkMicOff,
        TalkPlayerSwitch,
        TalkPlayerOn,
        TalkPlayerOff,
        TalkMicAuthNoPermission,           //麦权限关闭
        TalkSpeakChannelChange,           //切换说话频道
        TalkChangeSpeakChannelSucc,
        TalkChangeSpeakChannelFailed,
        TalkMicChangeByOther,             //被他人禁言
        TalkChannelMemberChanged,         //频道内玩家状态改变
        TalkChannelOtherSpeak,            //频道内有人说话
        TalkChannelOtherMicChanged,       //频道内他人麦状态改变
        TalkOtherChannelChanged,          //他人频道改变
        TalkChannelBroadcastMsg,          //频道内广播消息
        TalkCtrlGlobalSilence,            //控制禁言
        TalkGlobalSilenceChanged,         //禁言状态改变
        TalkOtherLeaveChannel,
        /*********************************** Voice End ************************************/
    }
}