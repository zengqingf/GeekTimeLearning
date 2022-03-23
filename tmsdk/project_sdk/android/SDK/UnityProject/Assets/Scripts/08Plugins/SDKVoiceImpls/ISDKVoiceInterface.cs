using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoiceSDK;

namespace VoiceSDK
{
    public interface ISDKVoiceInterface
    {
        /* chat voice */
        void InitChatVoice();//初始化聊天模式的语音
        void UnInitChatVoice();//反初始化聊天模式语音
        void LoginVoice(YoumiVoiceGameAccInfo userInfo);
        void LogoutVoice();
        void CancelRecordVoice();
        void AddVoicePathInQueue(string voiceKey);
        void ClearVoicePathQueue();
        void StopPlayVoice();

        void SetVoiceVolume(float volume);
        float GetVoiceVolume();

        //local cache 
        void ClearLocalCache();
        //lift cycle
        void OnPause();
        void OnResume();

        bool IsVoiceRecording();
        bool IsVoicePlaying();

        /* real talk */
        void InitTalkVoice();
        void UnInitTalkVoice();
        void JoinChannel(string channelId, string roleId, string openId);
        void LeaveAllChannel();
        void LeaveChannel(string channelId);
        void OpenRealMic();
        void CloseRealMic();
        void OpenRealPlayer();
        void CloseReaPlayer();
        bool IsTalkRealMicOn();
        bool IsTalkRealPlayerOn();
        void SetPlayerVolume(float volume);
        float GetPlayerVolume();


        //resoureces manager
        void PauseChannel();
        void ResumeChannel();

        //Log show 一般可屏蔽日志
        string ShowDebugLog();

    }
}