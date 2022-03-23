/*******************************************************************************\
 ** gcloud_voice:GCloudVoice.h
 ** Created by CZ  on 16/8/1
 **
 **  Copyright 2016 apollo. All rights reserved.
 \*******************************************************************************/

#ifndef GcloudVoiceErrno_h
#define GcloudVoiceErrno_h

namespace gcloud_voice
{
    /**
     * Destination language to translate to.
     */
    enum GCloudLanguage {
        Chinese     = 0,
        Korean      = 1,
        English     = 2,
        Japanese    = 3,
    };
    
    /**
     * Mode of the voice engine. You should set to one first.
     */
    enum GCloudVoiceMode
    {
        Unknown       = -1,
        RealTime      = 0,      // realtime mode for TeamRoom, NationalRoom, RangeRoom
        Messages      = 1,      // voice message mode
        Translation   = 2,      // speach to text mode
        RSTT          = 3,      // real-time speach to text mode
        HIGHQUALITY   = 4,      // high quality realtime voice mode[deprecated], will cost more network traffic
		RSTS          = 5,      // real-time source speach to target text/speach mode
    };
    
    /**
     * Member's role in National Room.
     */
    enum GCloudVoiceMemberRole
    {
        Anchor   = 1,       // member who can open/close microphone and speaker
        Audience = 2,       // member who can only open/close speaker
    };
    
    /**
     * The effect mode of voice.
     * You can set to one of this by SetVoiceEffects method when you want to change the player's voice.
     */
    enum SoundEffects
    {
        GVSE_REVB_CHURCH    = 0,      // reveb church effect
        GVSE_REVB_THEATER   = 1,      // reveb theater effect
        GVSE_HELL           = 2,      // voice from hell effect
        GVSE_ROBOT_1        = 3,      // robot voice effect
        GVSE_MALE_TO_FEMALE = 4,      // voice from male to female effect
        GVSE_FEMALE_TO_MALE = 5,      // voice from female to male effect
        GVSE_DRUNK          = 6,      // drunk voice effect
        GVSE_PAPI_JIANG     = 7,      // Chinese papi-Jiang voice effect
        GVSE_SQUIRREL       = 8,      // squrrel voice effect
        GVSE_NO_EFFECT      = 9,      // no voice effect
    };
    
	 /**
     * Speech Translation languages
     */
	enum SpeechLanguageType
	{
		SPEECH_LANGUAGE_ZH = 0,		// Chinese
		SPEECH_LANGUAGE_EN = 1,		// English
		SPEECH_LANGUAGE_JA = 2,		// Japanese
		SPEECH_LANGUAGE_KO = 3,		// Korean
		SPEECH_LANGUAGE_DE = 4,		// German
		SPEECH_LANGUAGE_FR = 5,		// French
		SPEECH_LANGUAGE_ES = 6,		// Spanish
		SPEECH_LANGUAGE_IT = 7,		// Italian
		SPEECH_LANGUAGE_TR = 8,		// Turkish
		SPEECH_LANGUAGE_RU = 9,		// Russian
		SPEECH_LANGUAGE_PT = 10,	// Portuguese
		SPEECH_LANGUAGE_VI = 11,	// Vietnamese
		SPEECH_LANGUAGE_ID = 12,	// Indonesian
		SPEECH_LANGUAGE_MS = 13,	// Malaysian
		SPEECH_LANGUAGE_TH = 14,	// Thai
		SPEECH_LANGUAGE_ZH_TW = 15,	// Traditional Chinese, Text Language Of TTS or STT 
		SPEECH_LANGUAGE_AF = 16,	// Afrikaans
		SPEECH_LANGUAGE_SQ = 17,	// Albanian
		SPEECH_LANGUAGE_AM = 18,	// Amharic
		SPEECH_LANGUAGE_AR = 19,	// Arabic
		SPEECH_LANGUAGE_HY = 20,	// Armenian
		SPEECH_LANGUAGE_AZ = 21,	// Azerbaijani
		SPEECH_LANGUAGE_EU = 22,	// Basque
		SPEECH_LANGUAGE_BN = 23,	// Bengali
		SPEECH_LANGUAGE_BS = 24,	// Bosnian
		SPEECH_LANGUAGE_BG = 25,	// Bulgarian
		SPEECH_LANGUAGE_MY = 26,	// Burmese
		SPEECH_LANGUAGE_CA = 27,	// Catalan
		SPEECH_LANGUAGE_HR = 28,	// Croatian
		SPEECH_LANGUAGE_CS = 29,	// Czech
		SPEECH_LANGUAGE_DA = 30,	// Danish
		SPEECH_LANGUAGE_NL = 31,	// Dutch
		SPEECH_LANGUAGE_ET = 32,	// Estonian
		SPEECH_LANGUAGE_FIL = 33,	// Filipino
		SPEECH_LANGUAGE_FI = 34,	// Finnish
		SPEECH_LANGUAGE_GL = 35,	// Galician
		SPEECH_LANGUAGE_KA = 36,	// Georgian
		SPEECH_LANGUAGE_EL = 37,	// Greek
		SPEECH_LANGUAGE_GU = 38,	// Gujarati
		SPEECH_LANGUAGE_IW = 39,	// Hebrew
		SPEECH_LANGUAGE_HI = 40,	// Hindi
		SPEECH_LANGUAGE_HU = 41,	// Hungarian
		SPEECH_LANGUAGE_IS = 42,	// Icelandic
		SPEECH_LANGUAGE_JV = 43,	// Javanese
		SPEECH_LANGUAGE_KN = 44,	// Kannada
		SPEECH_LANGUAGE_KK = 45,	// Kazakh
		SPEECH_LANGUAGE_KM = 46,	// Cambodian
		SPEECH_LANGUAGE_LO = 47,	// Lao
		SPEECH_LANGUAGE_LV = 48,	// Latvian
		SPEECH_LANGUAGE_LT = 49,	// Lithuanian
		SPEECH_LANGUAGE_MK = 50,	// Macedonian
		SPEECH_LANGUAGE_ML = 51,	// Malayalam
		SPEECH_LANGUAGE_MR = 52,	// Marathi
		SPEECH_LANGUAGE_MN = 53,	// Mongolian
		SPEECH_LANGUAGE_NE = 54,	// Nepali
		SPEECH_LANGUAGE_NO = 55,	// Bokmal Norwegian
		SPEECH_LANGUAGE_FA = 56,	// Persian
		SPEECH_LANGUAGE_PL = 57,	// Polish
		SPEECH_LANGUAGE_PA = 58,	// Punjabi
		SPEECH_LANGUAGE_RO = 59,	// Romanian
		SPEECH_LANGUAGE_SR = 60,	// Serbian
		SPEECH_LANGUAGE_SI = 61,	// Sinhala
		SPEECH_LANGUAGE_SK = 62,	// Slovak
		SPEECH_LANGUAGE_SL = 63,	// Slovenian
		SPEECH_LANGUAGE_SU = 64,	// Sundanese
		SPEECH_LANGUAGE_SW = 65,	// Swahili
		SPEECH_LANGUAGE_SV = 66,	// Swedish
		SPEECH_LANGUAGE_TA = 67,	// Tamil
		SPEECH_LANGUAGE_TE = 68,	// Telugu
		SPEECH_LANGUAGE_UK = 69,	// Ukrainian
		SPEECH_LANGUAGE_UR = 70,	// Urdu
		SPEECH_LANGUAGE_UZ = 71,	// Uzbek
		SPEECH_LANGUAGE_ZU = 72,	// Zulu
		SPEECH_LANGUAGE_CNT,
	};

	/**
     * Speech Translation type, pip nodes: Source Speech -> Source Text -> Target Text -> Target Speech
     */
	enum SpeechTranslateType
	{
		SPEECH_TRANSLATE_STST = 0,	//Source Speech -> Source Text
		SPEECH_TRANSLATE_STTT = 1,  //Source Speech -> Source Text -> Target Text
		SPEECH_TRANSLATE_STTS = 2,	//Source Speech -> Source Text -> Target Text -> Target Speech
	};

	/**
	* realtime voice translate type
	*/
	enum RealTimeTranslateType
	{
		RT_TRANSLATE_TEXT = 1, //my speech to text
		RT_TRANSLATE_SPEECH = 2, //my speech to other language speech
		RT_TRANSLATE_SPEECH_AND_TEXT = 3,
	};

	enum STTVoiceType
	{
		VOICE_TYPE_YUNXIAONING = 0, //default,friendly woman voice
		VOICE_TYPE_YUNXIAOQI = 1, //friendly man voice
		VOICE_TYPE_YUNXIAOWAN = 2, //mature man voice
		VOICE_TYPE_YUNXIAOYE = 4, //warm woman voice
		VOICE_TYPE_YUNXIAOXIN = 5, //emotional woman voice
		VOICE_TYPE_YUNXIAOLONG = 6, //emotional man voice

		VOICE_TYPE_ZHIXIA = 1000, //emotional man voice,latest 
		VOICE_TYPE_ZHIYU = 1001, //emotional woman voice,latest 
		VOICE_TYPE_ZHILIN = 1002, //generic woman voice,latest 
		VOICE_TYPE_ZHIMEI = 1003, //customer-service woman voice,latest

		VOICE_TYPE_JACK = 1050, //english man voice,latest
		VOICE_TYPE_ROSE = 1051, //english woman voice,latest
	};

    enum GCloudVoiceErrno
    {
        GCLOUD_VOICE_SUCC               = 0,
		
		//common base err
		GCLOUD_VOICE_PARAM_NULL         = 0x1001,	//4097, some parameter is null
		GCLOUD_VOICE_NEED_SETAPPINFO    = 0x1002,	//4098, you should call SetAppInfo first before call other api
		GCLOUD_VOICE_INIT_ERR           = 0x1003,	//4099, Init error
		GCLOUD_VOICE_RECORDING_ERR      = 0x1004,	//4100, now is recording, can't do other operator
		GCLOUD_VOICE_POLL_BUFF_ERR      = 0x1005,	//4101, poll buffer is not enough or null 
		GCLOUD_VOICE_MODE_STATE_ERR     = 0x1006,	//4102, call some api in the wrong voice engine mode, maybe you shoud call SetMode to correct it
		GCLOUD_VOICE_PARAM_INVALID      = 0x1007,	//4103, some parameter is null or its value is invalid, please use right parameters
		GCLOUD_VOICE_OPENFILE_ERR       = 0x1008,   //4104, open a file error
		GCLOUD_VOICE_NEED_INIT          = 0x1009,   //4105, you should call Init before do this operator
		GCLOUD_VOICE_ENGINE_ERR         = 0x100A,   //4106, you have not get engine instance, this common in use c# api, but not get gcloudvoice instance first
		GCLOUD_VOICE_POLL_MSG_PARSE_ERR = 0x100B,   //4107, this common in c# api, parse poll msg err
		GCLOUD_VOICE_POLL_MSG_NO        = 0x100C,   //4108, poll, no msg to update
		GCLOUD_VOICE_MODE_ERR        	= 0x100D,   //4109, The mode is not support
        GCLOUD_VOICE_FUNCTION_NOT_SUPPORT = 0x100E, //4110, the function is not support

		//realtime err
		GCLOUD_VOICE_REALTIME_STATE_ERR = 0x2001,   //8193, call some realtime api in wrong realtime voice state, such as OpenMic but you have not joined room
		GCLOUD_VOICE_JOIN_ERR           = 0x2002,   //8194, join room failed
		GCLOUD_VOICE_ROOMNAME_ERR       = 0x2003,   //8195, the roomname is not the same as the one when join room
		GCLOUD_VOICE_OPENMIC_NOTANCHOR_ERR = 0x2004,//8196, open mic in bigroom, but not anchor role
        GCLOUD_VOICE_CREATE_ROOM_ERR    = 0x2005,   //8197, create room error
        GCLOUD_VOICE_NO_ROOM = 0x2006,              //8198, no such room
        GCLOUD_VOICE_QUIT_ROOM_ERR      = 0x2007,   //8199, quit room error
        GCLOUD_VOICE_ALREADY_IN_THE_ROOM = 0x2008,  //8200, already in the room which in JoinXxxxRoom
        
		//message err
		GCLOUD_VOICE_AUTHKEY_ERR        = 0x3001,   //12289, apply authkey api error
		GCLOUD_VOICE_PATH_ACCESS_ERR    = 0x3002,   //12290, the path can not access, maybe the file is not exist or deny to access
		GCLOUD_VOICE_PERMISSION_MIC_ERR = 0x3003,	//12291, you don't have the right to access microphone in android
		GCLOUD_VOICE_NEED_AUTHKEY       = 0x3004,	//12292, you have not gotten authkey, call ApplyMessageKey first
		GCLOUD_VOICE_UPLOAD_ERR         = 0x3005,	//12293, upload file error
		GCLOUD_VOICE_HTTP_BUSY          = 0x3006,	//12294, http is busy, maybe the last upload/download has not finished
		GCLOUD_VOICE_DOWNLOAD_ERR       = 0x3007,	//12295, download file error
		GCLOUD_VOICE_SPEAKER_ERR        = 0x3008,   //12296, tve error when open or close speaker
		GCLOUD_VOICE_TVE_PLAYSOUND_ERR  = 0x3009,   //12297, tve error when play voice file
        GCLOUD_VOICE_AUTHING            = 0x300a,   //12298, already in applying auth key process
        GCLOUD_VOICE_LIMIT              = 0x300b,   //12299, upload limit, you can not upload permanent file
		GCLOUD_VOICE_NOTHING_TO_REPORT  = 0x300c,   //12300, no sound to report

//        GCLOUD_VOICE_REPORT_UP_SUCC     = 0x4001,   //16385, interface function never receive this error code, used in Engine

        //internal err
		GCLOUD_VOICE_INTERNAL_TVE_ERR   = 0x5001,   //20481, internal TVE error, GVoice internal used
		GCLOUD_VOICE_INTERNAL_VISIT_ERR = 0x5002,	//20482, internal non-TVE err, GVoice internal used
		GCLOUD_VOICE_INTERNAL_USED      = 0x5003,   //20483, internal used, you should not get this error
        
        GCLOUD_VOICE_BADSERVER          = 0x06001,  //24577, bad server address, the server address should like: "udp://capi.xxx.xxx.com"
        
        GCLOUD_VOICE_STTING             = 0x07001,  //28673, already in speaching to text process
        
        
        //other functions in realtime err
        GCLOUD_VOICE_CHANGE_ROLE        = 0x08001,  //32769, change role error
        GCLOUD_VOICE_CHANGING_ROLE      = 0x08002,  //32770, already in changing role
        GCLOUD_VOICE_NOT_IN_ROOM        = 0x08003,  //32771, no in room
        GCLOUD_VOICE_COORDINATE         = 0x09001,  //36865, sync coordinate error
        GCLOUD_VOICE_SMALL_ROOMNAME     = 0x09002,  //36866, query with a small room name
        GCLOUD_VOICE_COORDINATE_ROOMNAME_ERROR = 0x09003, //36867, update coordinate in a non-exist room
        
        GCLOUD_VOICE_SAVEDATA_DOWNLOADING = 0x0A001, //40961, dowloading file for lgame save voice data, need no nothing, just let userinterface know.
        GCLOUD_VOICE_SAVEDATA_INDEXNOTFOUND = 0x0A002,  //40962, this file index not found in file map, may not set, have not in this video
        
        GCLOUD_VOICE_NOENABGLE_WXMINI = 0x0B002, // 45058, need enable MiniApp

		GCLOUD_VOICE_DEVICE_TVE_ERR = 0x0C001, 
    };
    
    enum GCloudVoiceCompleteCode
    {
        // common code
		GV_ON_OK						  = 0x1000,    // 4096, ok. 
        GV_ON_NET_ERR                     = 0x1001,    // 4097, network error, maybe can't connect to network
        GV_ON_UNKNOWN                     = 0x1002,    // 4098
        GV_ON_INTERNAL_ERR                = 0x1003,    // 4099, this error needs log for problem location
        GV_ON_BUSINESS_NOT_FOUND          = 0x1004,    // 4100, the business not found, maybe you do not open the service
		GV_ON_FAIL						  = 0x1005,    // 4101, fail.
		GV_ON_SHOULD_ONE_ROOM_ONE_SCENES  = 0x1006,    // 4102, a room can only be associated with one scene.

        // realtime code
        GV_ON_JOINROOM_SUCC               = 0x2001,    // 8193, join room success
        GV_ON_JOINROOM_TIMEOUT            = 0x2002,    // 8194, join room timeout
        GV_ON_JOINROOM_SVR_ERR            = 0x2003,    // 8195, communication with svr meets some error, such as wrong data received from svr
        GV_ON_JOINROOM_UNKNOWN            = 0x2004,    // 8196, reserved, GVoice internal unknown error
        GV_ON_JOINROOM_RETRY_FAIL         = 0x2005,    // 8197, join room try again fail
        GV_ON_QUITROOM_SUCC               = 0x2006,    // 8198, quitroom success, if you have joined room success first, quit room will alway return success
        GV_ON_ROOM_OFFLINE                = 0x2007,    // 8199, dropped from the room
        GV_ON_ROLE_SUCC                   = 0x2008,    // 8200, change role success
        GV_ON_ROLE_TIMEOUT                = 0x2009,    // 8201, change role timeout
        GV_ON_ROLE_MAX_AHCHOR             = 0x2010,    // 8202, too many anchors, no more than 5 anchors in the same time are allowed in a national room
        GV_ON_ROLE_NO_CHANGE              = 0x2011,    // 8203, the same role as before
        GV_ON_ROLE_SVR_ERROR              = 0x2012,    // 8204, server's error in change role
        
        
        // message mode
        GV_ON_MESSAGE_KEY_APPLIED_SUCC    = 0x3001,    // 12289，apply message authkey succ
        GV_ON_MESSAGE_KEY_APPLIED_TIMEOUT = 0x3002,    // 12290，apply message authkey timeout
        GV_ON_MESSAGE_KEY_APPLIED_SVR_ERR = 0x3003,    // 12291，communication with svr meets some error, such as wrong data received
        GV_ON_MESSAGE_KEY_APPLIED_UNKNOWN = 0x3004,    // 12292，reserved, GVoice internal unknown error
        
        GV_ON_UPLOAD_RECORD_DONE          = 0x3005,    // 12293，upload record file success
        GV_ON_UPLOAD_RECORD_ERROR         = 0x3006,    // 12294，upload record file meets some error
        GV_ON_DOWNLOAD_RECORD_DONE        = 0x3007,    // 12295，download record file success
        GV_ON_DOWNLOAD_RECORD_ERROR       = 0x3008,    // 12296，download record file meets some error
        GV_ON_PLAYFILE_DONE               = 0x3009,    // 12297，the record file have played to the end        
        
        // translate mode
        GV_ON_STT_SUCC                    = 0x4001,    // 16385，speech to text success
        GV_ON_STT_TIMEOUT                 = 0x4002,    // 16386，speech to text timeout
        GV_ON_STT_APIERR                  = 0x4003,    // 16387，server's error
        
        
        // rstt mode
        GV_ON_RSTT_SUCC                   = 0x5001,    // 20481，stream speech to text success
        GV_ON_RSTT_TIMEOUT                = 0x5002,    // 20482，stream speech to text timeout
        GV_ON_RSTT_APIERR                 = 0x5003,    // 20483，server's error in stream speech to text
        GV_ON_RSTT_RETRY                  = 0x5004,    // 20484，need retry stt
        
        
        // voice report
        GV_ON_REPORT_SUCC                 = 0x6001,    // 24577，report other player succ
        GV_ON_DATA_ERROR                  = 0x6002,    // 24578，receive illegal or invalid data from serve
        GV_ON_PUNISHED                    = 0x6003,    // 24579，the player is punished because of being reported
        GV_ON_NOT_PUNISHED                = 0x6004,    // 24580，the player
        GV_ON_KEY_DELECTED                = 0x6005,
        GV_ON_REPORT_SUCC_SELF            = 0x6006,
        // for LGame
        GV_ON_SAVEDATA_SUCC               = 0x7001,    // 28673, LGame save rec
        
        // member synchornize
        GV_ON_ROOM_MEMBER_INROOM          = 0x8001,    // 32769, member join or in room
        GV_ON_ROOM_MEMBER_OUTROOM         = 0x8002,    // 32770, member out of room
		GV_ON_ROOM_MEMBER_MICOPEN = 0x8003,    // 32771, member mic is open
		GV_ON_ROOM_MEMBER_MICCLOSE = 0x8004,    // 32772, member mic is close

		GV_ON_DEVICE_EVENT_ADD            = 0x8101, //device event notify
		GV_ON_DEVICE_EVENT_UNUSABLE, //device unusable event
		GV_ON_DEVICE_EVENT_DEFAULTCHANGE, //default event changed
        
        // for civilized voice
        GV_ON_UPLOAD_REPORT_INFO_ERROR    = 0x9001,    // 36865, civilized voice reporting error
        GV_ON_UPLOAD_REPORT_INFO_TIMEOUT  = 0x9002,    // 36866, civilized voice reporting timeout
        
        // for speech translation
        GV_ON_ST_SUCC                     = 0xA001,    // 40961, speech translate success
        GV_ON_ST_HTTP_ERROR               = 0xA002,    // 40962, http failed
        GV_ON_ST_SERVER_ERROR             = 0xA003,    // 40963, server error
        GV_ON_ST_INVALID_JSON             = 0xA004,    // 40964, parse rsp json faild.
		GV_ON_ST_ALREADY_EXIST			  = 0xA005,    // 40965, doing already.
		GV_ON_ST_RC_FAILED				  = 0xA006,    // 40966, resource alloc failed.

        // for mini app
        GV_ON_WX_UPLOAD_SUCC              = 0xB001,    // 45057, upload self info to success
        GV_ON_WX_UPLOAD_ERR               = 0xB002,    // 45058, upload self info to failed
        GV_ON_WX_ROOM_SUCC                = 0xB003,    // 45059, query room members success
        GV_ON_WX_ROOM_ERR                 = 0xB004,    // 45060, query room members failed
        GV_ON_WX_USER_SUCC                = 0xB005,    // 45061, query user info success
        GV_ON_WX_USER_ERR                 = 0xB006,    // 45062, query user info failed

		//for realtime translate
		GV_ON_TRANSLATE_SUCC			  = 0xC001,	   // 49153, realtime enable translate ok
		GV_ON_TRANSLATE_SERVER_ERR		  = 0xC002,	   // 49154, realtime enable translate server error

        // for magic voice
        GV_ON_MAGICVOICE_SUCC              = 0xD001,    // 53249, enable magic voice success in realtime mode
        GV_ON_MAGICVOICE_SERVER_ERR        = 0xD002,    // 53250, enable magic voice failed in realtime mode
        GV_ON_RECVMAGICVOICE_SUCC          = 0xD003,    // 53251, enable recv magic voice success in realtime mode
        GV_ON_RECVMAGICVOICE_SERVER_ERR    = 0xD004,    // 53252, enable recv magic voice failed in realtime mode
        GV_ON_MAGICVOICE_FILE_SUCC         = 0xD005,    //53253, magic voice offline file upload succ
		GV_ON_MAGICVOICE_FILE_FAIL         = 0XD006,    //53254, magic voice offline file record fail
	};
    
    /**
     * Event of GCloudVoice
     *
     */
    enum GCloudVoiceEvent
    {
        EVENT_NO_DEVICE_CONNECTED            = 0,  //no any device is connected
        EVENT_HEADSET_DISCONNECTED           = 10, //a headset device is connected
        EVENT_HEADSET_CONNECTED              = 11, //a headset device is disconnected
        EVENT_BLUETOOTH_HEADSET_DISCONNECTED = 20, //a bluetooth device is connected
        EVENT_BLUETOOTH_HEADSET_CONNECTED    = 21, //a bluetooth device is disconnected
        EVENT_MIC_STATE_OPEN_SUCC            = 30, //open microphone success
        EVENT_MIC_STATE_OPEN_ERR             = 31, //open microphone meets error
        EVENT_MIC_STATE_NO_OPEN              = 32, //microphone not open
		EVENT_MIC_STATE_OCCUPANCY            = 33, //indicates the microphone has been occupancyed by others
        EVENT_SPEAKER_STATE_OPEN_SUCC        = 40, //open speaker success
        EVENT_SPEAKER_STATE_OPEN_ERR         = 41, //open speaker meets error
        EVENT_SPEAKER_STATE_NO_OPEN          = 42, //speaker not open
		EVENT_AUDIO_INTERRUPT_BEGIN          = 50, //audio device begin to be interrupted
		EVENT_AUDIO_INTERRUPT_END            = 51, //audio device end to be interrupted
        EVENT_AUDIO_RECORDER_EXCEPTION       = 52, //indicates the recorder thread throws a exception, maybe you can resume the audio
        EVENT_AUDIO_RENDER_EXCEPTION         = 53, //indicates the render thread throws a exception, maybe you can resume the audio
		EVENT_PHONE_CALL_PICK_UP             = 54, //indicates that you picked up the phone
		EVENT_PHONE_CALL_HANG_UP             = 55, //indicates that you hanged up the phone
	};
    
    /**
     * Audio device event of GCloudVoice
     */
    enum GCloudVoiceDeviceState
    {
        AUDIO_DEVICE_UNCONNECTED            = 0,  //no any audio device is connected
        AUDIO_DEVICE_WIREDHEADSET_CONNECTED = 1,  //a wiredheadset device is connected
        AUDIO_DEVICE_BULETOOTH_CONNECTED    = 2,  //a bluetooth device is disconnected
        
    };

} //endof namespace gcloud_voice

#endif /* GcloudVoiceErrno_h */
