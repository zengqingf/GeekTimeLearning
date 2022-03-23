using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VoiceSDK;

public class YouMiTest : MonoBehaviour
{
    public Button initBtn;
    public Button loginVoiceBtn;
    public Button loginOutVoiceBtn;
    public Button joinChatRoomBtn;
    public Button leaveChatRoomBtn;
    public Button RecordBtn;
    public Button PlayVoiceBtn;


    public Text showInfo;

    public InputField roleId;
    public Button ClearBtn;

    string _lastvoicekey = "";


    //talk voice 
    public Button talkInitBtn;
    public Button joinChannelBtn;
    public Button leaveChannelBtn;
    public Button openMicBtn;
    public Button closeMicBtn;
    public Button openPlayerBtn;
    public Button closePlayerBtn;
    public Button PauseBtn;
    public Button ResumeBtn;

    public GameObject cube;

    void Start()
    {
        SDKVoiceManager.GetInstance().InitVoiceEnabled(true, true);
        SDKVoiceInterface.Instance.showDebugLogUIHandler += SetTextLog;
        SDKVoiceManager.GetInstance().SetVoiceDebugLevel(VoiceSDK.SDKVoiceLogLevel.Error);

        initBtn.onClick.AddListener(OnInitVoice);
        loginVoiceBtn.onClick.AddListener(OnLoginVoice);
        loginOutVoiceBtn.onClick.AddListener(OnLoginOutVoice);
        joinChatRoomBtn.onClick.AddListener(OnJoinChatRoom);
        leaveChatRoomBtn.onClick.AddListener(OnLeaveChatRoom);
        //RecordBtn.onClick.AddListener(OnRecord);
        PlayVoiceBtn.onClick.AddListener(OnPlayVoice);
        ClearBtn.onClick.AddListener(OnClearLog);


        talkInitBtn.onClick.AddListener(TalkInit);
        joinChannelBtn.onClick.AddListener(JoinChannel);
        leaveChannelBtn.onClick.AddListener(LeaveChannel);
        openMicBtn.onClick.AddListener(OpenMic);
        closeMicBtn.onClick.AddListener(CloseMic);
        openPlayerBtn.onClick.AddListener(OpenPlayer);
        closePlayerBtn.onClick.AddListener(ClosePlayer);
        PauseBtn.onClick.AddListener(PauseChannel);
        ResumeBtn.onClick.AddListener(ResumeChannel);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetTextLog(string log)
    {
        if (showInfo)
        {
            showInfo.text += "voice debug : " + log + "\n";
        }
    }
    void OnClearLog()
    {
        showInfo.text = "日志输出:";
    }

    public void OnInitVoice()
    {
            //SDKVoiceManager.GetInstance().SetVoiceSavePath(VoicePath);
            VoiceSDK.ChatDelegeInfo delegates = new VoiceSDK.ChatDelegeInfo();
            delegates.OnVoiceChatRecordStart = _OnVoiceChatRecordStart;
            //delegates.OnVoiceChatRecordEnd = _OnVoiceChatRecordEnd;
            delegates.OnVoiceChatLogin = _OnVoiceLoginOut;
            //delegates.OnVoiceChatRecordFailed = _OnVoiceChatRecordFailed;
            //delegates.OnVoiceChatSendSucc = _OnVoiceChatSendSucc;
            //delegates.OnVoiceChatSendFailed = _OnVoiceChatSendFailed;
            //delegates.OnVoiceChatPlayStart = _OnVoiceChatPlayStart;
            //delegates.OnVoiceChatPlayEnd = _OnVoiceChatPlayEnd;
            SDKVoiceManager.GetInstance().InitChatVoice(delegates);
            //SDKVoiceManager.GetInstance().InitTalkVoice();


    }

    public void OnLoginVoice()
    {
        VoiceSDK.YoumiVoiceGameAccInfo info = new VoiceSDK.YoumiVoiceGameAccInfo();
        info.RoleId = roleId.text.ToString();
        //info.RoleId = "1001";
        info.OpenId = "123456";
        info.Token = "";
        SDKVoiceManager.GetInstance().LoginVoice(info);
    }

    void OnLoginOutVoice()
    {
        SDKVoiceManager.GetInstance().LogoutVoice();
    }

    public void OnJoinChatRoom()
    {
        VoiceSDK.SDKVoiceRoomInfo roomInfo = new VoiceSDK.SDKVoiceRoomInfo();
        roomInfo.serverId = "1001";
        roomInfo.roomDec = "t";
        roomInfo.beSaveRoomMsg = true;
        roomInfo.roomtypeId = "world";
        SDKVoiceManager.GetInstance().JoinChatRoom(roomInfo);
    }

    void OnLeaveChatRoom()
    {
        SDKVoiceManager.GetInstance().LeaveChatRoom();
    }

    public void OnSendVoiceMessage()
    {
        //Debug.Log("touchdown")
        SDKVoiceRecordInfo recordInfo = new SDKVoiceRecordInfo();
        recordInfo.roomInfo.serverId = "1001";
        recordInfo.roomInfo.roomDec = "t";
        recordInfo.roomInfo.beSaveRoomMsg = true;
        recordInfo.roomInfo.roomtypeId = "world";
        recordInfo.isTranslate = true;
        SDKVoiceManager.GetInstance().StartRecordCommon(recordInfo);


    }

    public void OnStopAudio()
    {
        SDKVoiceToken token = new SDKVoiceToken();
        token.serverTimeStamp = (ulong)Time.time;
        token.voiceType = "world";

        SDKVoiceManager.GetInstance().StopRecordCommon(token);

    }

    public void OnPlayVoice()
    {
        SDKVoiceManager.GetInstance().PlayVoiceCommon(_lastvoicekey);
    }




    void _OnVoiceChatRecordStart(string voicekey, string sText, int iDuration, string roomID)
    {
        _lastvoicekey = voicekey;
        Debug.LogFormat("voicekey is {0} | sText is {1} | iDuration is {2}", voicekey, sText, iDuration);
    }

    private void _OnVoiceLoginOut()
    {
        OnJoinChatRoom();
    }


    void TalkInit()
    {
        SDKVoiceManager.GetInstance().InitTalkVoice();
    }

    void JoinChannel()
    {
        SDKVoiceManager.GetInstance().JoinChannel("tm666", UnityEngine.Random.Range(0,999)+"", "123456", "");
    }

    void LeaveChannel()
    {
        SDKVoiceManager.GetInstance().LeaveAllChannel();
    }

    void OpenMic()
    {
        SDKVoiceManager.GetInstance().OpenRealMic(); //打开麦克风
    }

    void CloseMic()
    {
        SDKVoiceManager.GetInstance().CloseRealMic();
    }

    void OpenPlayer()
    {
        SDKVoiceManager.GetInstance().OpenRealPlayer();
    }

    void ClosePlayer()
    {
        SDKVoiceManager.GetInstance().CloseReaPlayer();
    }
    void PauseChannel()
    {
        SDKVoiceManager.GetInstance().PauseChannel();
    }
    void ResumeChannel()
    {
        SDKVoiceManager.GetInstance().ResumeChannel();
    }

}
