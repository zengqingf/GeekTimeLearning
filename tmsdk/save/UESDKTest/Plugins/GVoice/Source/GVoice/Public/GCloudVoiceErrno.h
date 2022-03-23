/*******************************************************************************\
 ** File: GCloudVoiceErrno.h
 ** Module: GVoice
 ** Version: 1.0
 ** Author: CZ
 \*******************************************************************************/

#ifndef _GCloud_GVoice_GCloudVoiceErrno_h
#define _GCloud_GVoice_GCloudVoiceErrno_h

#if defined(WIN32) || defined(_WIN32)
    #ifdef GCLOUD_VOICE_EXPORTS
        #define GCLOUD_VOICE_API __declspec(dllexport)
    #else
        #define GCLOUD_VOICE_API __declspec(dllimport)
    #endif
#else
    #if defined __ANDROID__
        #define GCLOUD_VOICE_API __attribute__ ((visibility ("default")))
    #else
        #define GCLOUD_VOICE_API
    #endif
#endif

namespace GCloud
{
    namespace GVoice
    {
        /**
         * Speech Translation languages
         */
        enum SpeechLanguageType
        {
            SPEECH_LANGUAGE_ZH = 0,        // Chinese
            SPEECH_LANGUAGE_EN = 1,        // English
            SPEECH_LANGUAGE_JA = 2,        // Japanese
            SPEECH_LANGUAGE_KO = 3,        // Korean
            SPEECH_LANGUAGE_DE = 4,        // German
            SPEECH_LANGUAGE_FR = 5,        // French
            SPEECH_LANGUAGE_ES = 6,        // Spanish
            SPEECH_LANGUAGE_IT = 7,        // Italian
            SPEECH_LANGUAGE_TR = 8,        // Turkish
            SPEECH_LANGUAGE_RU = 9,        // Russian
            SPEECH_LANGUAGE_PT = 10,       // Portuguese
            SPEECH_LANGUAGE_VI = 11,       // Vietnamese
            SPEECH_LANGUAGE_ID = 12,       // Indonesian
            SPEECH_LANGUAGE_MS = 13,       // Malaysian
            SPEECH_LANGUAGE_TH = 14,       // Thai
            SPEECH_LANGUAGE_ZH_TW = 15,    // Traditional Chinese, Text Language Of TTS or STT
        };
        
        /**
         * Speech Translation type, pip nodes: Source Speech -> Source Text -> Target Text -> Target Speech
         */
        enum SpeechTranslateType
        {
            SPEECH_TRANSLATE_STST = 0,    //Source Speech -> Source Text
            SPEECH_TRANSLATE_STTT = 1,    //Source Speech -> Source Text -> Target Text
            SPEECH_TRANSLATE_STTS = 2,    //Source Speech -> Source Text -> Target Text -> Target Speech
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
    
        /**
        * realtime voice translate type
        */
        enum RealTimeTranslateType
        {
            RT_TRANSLATE_TEXT = 1, //my speech to text
            RT_TRANSLATE_SPEECH = 2, //my speech to other language speech
            RT_TRANSLATE_SPEECH_AND_TEXT = 3,
        };
        
        enum ErrorNo
        {
            kErrorNoSucc                    = 0,                       // 0, no error
            
            //common base err
            kErrorParseJSON                 = 0x1000,                  // 4096, parse JSON error
            kErrorNoParamNULL               = 0x1001,                  // 4097, some param is null
            kErrorNoNeedSetAppInfo          = 0x1002,                  // 4098, you should call SetAppInfo first before call other api
            kErrorNoInitErr                 = 0x1003,                  // 4099, Init Erro
            kErrorNoRecordingErr            = 0x1004,                  // 4100, now is recording, can't do other operator
            kErrorNoPollBuffErr             = 0x1005,                  // 4101, poll buffer is not enough or null
            kErrorNoModeStateErr            = 0x1006,                  // 4102, call some api, but the mode is not correct, maybe you shoud call SetMode first and correct
            kErrorNoParamInvalid            = 0x1007,                  // 4103, some param is null or value is invalid for our request, used right param and make sure is value range is correct by our comment
            kErrorNoOpenFileErr             = 0x1008,                  // 4104, open a file err
            kErrorNoNeedInit                = 0x1009,                  // 4105, you should call Init before do this operator
            kErrorNoEngineErr               = 0x100A,                  // 4106, you have not get engine instance, this common in use c# api, but not get gcloudvoice instance first
            kErrorNoPollMsgParseErr         = 0x100B,                  // 4107, this common in c# api, parse poll msg err
            kErrorNoPollMsgNo               = 0x100C,                  // 4108, poll, no msg to update
            kErrorNoModeErr                 = 0x100D,                  //4109, The mode is not support
            kErrorNoFunctionNotSupport      = 0x100E,                  //4110, the function is not support
            kErrorFail                      = 0x100F,                  //4111, fail
            
            
            //realtime err
            kErrorNoRealtimeStateErr        = 0x2001,                  // 8193, call some realtime api, but state err, such as OpenMic but you have not Join Room first
            kErrorNoJoinErr                 = 0x2002,                  // 8194, join room failed
            kErrorNoQuitRoomNameErr         = 0x2003,                  // 8195, quit room err, the quit roomname not equal join roomname
            kErrorNoOpenMicNotAnchorErr     = 0x2004,                  // 8196, open mic in bigroom,but not anchor role
            kErrorNoCreateRoomErr           = 0x2005,                  // 8197, create room error
            kErrorNoNoRoom                  = 0x2006,                  // 8198, no such room
            kErrorNoQuitRoomErr             = 0x2007,                  // 8199, quit room error
            kErrorNoAlreadyInTheRoom        = 0x2008,                  // 8200, already in the room which in JoinXxxxRoom
            
            //message err
            kErrorNoAuthKeyErr              = 0x3001,                  // 12289, apply authkey api error
            kErrorNoPathAccessErr           = 0x3002,                  // 12290, the path can not access ,may be path file not exists or deny to access
            kErrorNoPermissionMicErr        = 0x3003,                  // 12291, you have not right to access micphone in android
            kErrorNoNeedAuthKey             = 0x3004,                  // 12292,you have not get authkey, call ApplyMessageKey first
            kErrorNoUploadErr               = 0x3005,                  // 12293, upload file err
            kErrorNoHttpBusy                = 0x3006,                  // 12294, http is busy,maybe the last upload/download not finish.
            kErrorNoDownloadErr             = 0x3007,                  // 12295, download file err
            kErrorNoSpeakerErr              = 0x3008,                  // 12296, open or close speaker tve error
            kErrorNoTVEPlaySoundErr         = 0x3009,                  // 12297, tve play file error
            kErrorNoAuthing                 = 0x300a,                  // 12298, Already in applying auth key processing
            kErrorNoLimit                   = 0x300b,                  // 12299, upload limit
            kErrorNoNothingToReport         = 0x300c,                  // 12300, no sound to report
            
            kErrorNoInternalTVEErr          = 0x5001,                  // 20481, internal TVE err, our used
            kErrorNoInternalVisitErr        = 0x5002,                  // 20482, internal Not TVE err, out used
            kErrorNoInternalUsed            = 0x5003,                  // 20483, internal used, you should not get this err num
            
            kErrorNoBadServer               = 0x06001,                  // 24577, bad server address,should be "udp://capi.xxx.xxx.com"
            
            kErrorNoSTTing                  =  0x07001,                 // 28673, Already in speach to text processing
            
            kErrorNoChangeRole              = 0x08001,                  // 32769, change role error
            kErrorNoChangingRole            = 0x08002,                  // 32770, is in changing role
            kErrorNoNotInRoom               = 0x08003,                  // 32771, no in room
            kErrorNoCoordinate              = 0x09001,                  // 36865, sync coordinate error
            kErrorNoSmallRoomName           = 0x09002,                  // 36866, query with a small roomname
            kErrorNoCoordinateRoomNameError = 0x09003,                  // 36867, update coordinate in a non-exist room
            
            kErrorSaveDataDownloading       = 0x0A001,                  // 40961, dowloading file for lgame save voice data, need no nothing, just let userinterface know.
            kErrorSaveDataIndexNotFound     = 0x0A002,                  // 40962, this file index not found in file map ,may not set ,have not in this video
            kErrorDeviceTveErr              = 0x0C001,                  // 36866, query with a small roomname
        };
    
        enum CompleteCode
        {
            // common code
            kCompleteCodeOk                      = 0x1000,              // 4096, ok.
            kCompleteCodeNetErr                  = 0x1001,              // 4097, network error, maybe can't connect to ne
            kCompleteCodeUnknown                 = 0x1002,              // 4098
            kCompleteCodeInternalErr             = 0x1003,              // 4099, this error needs log for problem location
            kCompleteCodeBusinessNotFound        = 0x1004,              // 4100, the business not found, maybe you do not open the service
            kCompleteCodeFail                    = 0x1005,              // 4101, fail.
            kCompleteCodeShouldOneRoomOneScenes  = 0x1006,              // 4102, a room can only be associated with one scene.
            
            
            // realtime code
            kCompleteCodeJoinRoomSucc       = 0x2001,                    // 8193, join room success
            kCompleteCodeJoinRoomTimeout    = 0x2002,                    // 8194, join room timeout
            kCompleteCodeJoinRoomSVRErr     = 0x2003,                    // 8195, communication with svr meets some error, such as wrong data received from svr
            kCompleteCodeJoinRoomUnknown    = 0x2004,                    // 8196, reserved, GVoice internal unknown error
            kCompleteCodeJoinRoomRetryFail  = 0x2005,                    // 8197, join room try again fail
            kCompleteCodeQuitRoomSucc       = 0x2006,                    // 8198, quitroom success, if you have joined room success first, quit room will alway return success
            kCompleteCodeRoomOffline        = 0x2007,                    // 8199, dropped from the room
            kCompleteCodeRoleSucc           = 0x2008,                    // 8200, change role success
            kCompleteCodeRoleTimeout        = 0x2009,                    // 8201, change role timeout
            kCompleteCodeRoleMaxAnchor      = 0x2010,                    // 8202, too many anchors, no more than 5 anchors in the same time are allowed in a national room
            kCompleteCodeRoleNoChange       = 0x2011,                    // 8203, the same role as before
            kCompleteCodeRoleSvrErr         = 0x2012,                    // 8204, server's error in change role
            
            // message mode
            kCompleteCodeMessageKeyAppliedSucc    = 0x3001,              // 12289, apply message authkey succ
            kCompleteCodeMessageKeyAppliedTimeout = 0x3002,              // 12290, apply message authkey timeout
            kCompleteCodeMessageKeyAppliedSVRErr  = 0x3003,              // 12291, communication with svr meets some error, such as wrong data received
            kCompleteCodeMessageKeyAppliedUnknown = 0x3004,              // 12292, reserved, GVoice internal unknown error
            
            kCompleteCodeUploadRecordDone         = 0x3005,              // 12293, upload record file success
            kCompleteCodeUploadRecordError        = 0x3006,              // 12294, upload record file meets some error
            kCompleteCodeDownloadRecordDone       = 0x3007,              // 12295, download record file success
            kCompleteCodeDownloadRecordError      = 0x3008,              // 12296, download record file meets some error
            kCompleteCodePlayFileDone             = 0x3009,              // 12297, the record file have played to the end
            
            // translate mode
            kCompleteCodeSTTSucc                  = 0x4001,              // 16385, speech to text success
            kCompleteCodeSTTTimeout               = 0x4002,              // 16386, speech to text timeout
            kCompleteCodeSTTAPIErr                = 0x4003,              // 16387, server's error
            
            // rstt mode
            kCompleteCodeRSTTSucc                 = 0x5001,              // 20481, stream speech to text success
            kCompleteCodeRSTTTimeout              = 0x5002,              // 20482, stream speech to text timeout
            kCompleteCodeRSTTAPIErr               = 0x5003,              // 20483, server's error in stream speech to text
            kCompleteCodeRSTTRetry                = 0x5004,              // 20484, need retry stt
            
            // voice report
            kCompleteCodeReportSucc               = 0x6001,              // 24577, report other player succ
            kCompleteCodeDataError                = 0x6002,              // 24578, receive illegal or invalid data from serve
            kCompleteCodePunished                 = 0x6003,              // 24579, the player is punished because of being reported
            kCompleteCodeNotPunished              = 0x6004,              // 24580, the player
            kCompleteCodeKeyDeleted               = 0x6005,              // 24581
            
            // for LGame
            kCompleteCodeSaveDataSucc             = 0x7001,              // 28673, LGAME Save RecData
            
            // member synchornize
            kCompleteCodeRoomMemberInRoom         = 0x8001,              // 32769, member join or in room
            kCompleteCodeRoomMemberOutRoom        = 0x8002,              // 32770, member out of room
            kCompleteCodeDeviceEventAdd           = 0x8101,              // 33025, device event notify
            kCompleteCodeDeviceEventUnusable      = 0x8102,              // 33026, device unusable event
            kCompleteCodeDeviceEventDefaultChange = 0x8103,              // 33027, default event changed
            
            // for civilized voice
            kCompleteCodeUploadReportInfoError    = 0x9001,              // 36865, civilized voice reporting error
            kCompleteCodeUploadReportInfoTimeout  = 0x9002,              // 36866, civilized voice reporting timeout
            
            // for speech translate
            kCompleteCodeStSucc                   = 0xA001,              // 40961, speech translate success
            kCompleteCodeStHttpError              = 0xA002,              // 40962, http failed
            kCompleteCodeStServerError            = 0xA003,              // 40963, server error
            kCompleteCodeStInvalidJson            = 0xA004,              // 40964, parse rsp json faild
			kCompleteCodeStAlreadyExist			  = 0xA005,    			 // 40965, doing already.
			kCompleteCodeStRcFailed				  = 0xA006,    			 // 40966, resource alloc failed.
		
            //for realtime translate
            kCompleteCodeTranslateSucc			  = 0xC001,	             // 49153, realtime enable translate ok
            kCompleteCodeTranslateServerErr		  = 0xC002,	             // 49154, realtime enable translate server error
            
            // for magic voice
            kCompleteCodeMagicVoiceSucc           = 0xD001,              // 53249, enable magic voice success in realtime mode
            kCompleteCodeMagicVoiceServerErr      = 0xD002,              // 53250, enable magic voice failed in realtime mode
            kCompleteCodeRecvMagicVoiceSucc       = 0xD003,              // 53251, enable recv magic voice success in realtime mode
            kCompleteCodeRecvMagicVoiceServerErr  = 0xD004,              // 53252, enable recv magic voice failed in realtime mode
        };
    
        /**
         * Event of GCloudVoice.
         *
         */
        enum Event
        {
            kEventNoDeviceConnected               = 0,                   // not any device is connected
            kEventHeadsetDisconnected             = 10,                  // a headset device is connected
            kEventHeadsetConnected                = 11,                  // a headset device is disconnected
            kEventBluetoothHeadsetDisconnected    = 20,                  // a bluetooth device is connected
            kEventBluetoothHeadsetConnected       = 21,                  // a bluetooth device is disconnected
            kEventMicStateOpenSucc                = 30,                  // open microphone
            kEventMicStateOpenErr                 = 31,                  // open microphone
            kEventMicStateNoOpen                  = 32,                  // close micrphone
            kEventMicStateOccupancy               = 33,                  // indicates the microphone has been occupancyed by others
            kEventSpeakerStateOpenSucc            = 40,                  // open speaker
            kEventSpeakerStateOpenErr             = 41,                  // open speaker error
            kEventSpeakerStateNoOpen              = 42,                  // close speaker
            kEventAudioInterruptBegin             = 50,                  // audio device begin to be interrupted
            kEventAudioInterruptEnd               = 51,                  // audio device end to be interrupted
            kEventAudioRecoderException           = 52,                  // indicates the recorder thread throws a exception, maybe you can resume the audio
            kEventAudioRenderException            = 53,                  // indicates the render thread throws a exception, maybe you can resume the audio
            kEventPhoneCallPickUp                 = 54,                  // indicates that you picked up the phone
            kEventPhoneCallHangUp                 = 55,                  // indicates that you hanged up the phone
        };
    
        /**
         * Event of GCloudVoice.
         *
         */
        enum DeviceState
        {
            kDeviceStateUnconnected               = 0,                   // not any audio device is connected
            kDeviceStateWriteHeadsetConnected     = 1,                   // have a wiredheadset device is connected
            kDeviceStateBluetoothConnected        = 2,                   // have a bluetooth device is disconnected
        };
        
    } // endof namespace GVoice
} // endof namespace GCloud

#endif /* _GCloud_GVoice_GCloudVoiceErrno_h */
