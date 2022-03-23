//
//  GCloudVoiceNotify.h
//  GCloudVoice
//
//  Created by Lilac on 2019/3/22.
//  Copyright © 2019 gcloud. All rights reserved.
//

#ifndef GCloudVoiceNotify_h
#define GCloudVoiceNotify_h

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

#include "GCloudVoiceErrno.h"
#include <stdlib.h>
#include <string.h>

namespace gcloud_voice
{
    struct WXMemberInfo {
        unsigned int MemberID;
        char OpenID[64];
        char Info[512] ;
        WXMemberInfo():
        MemberID(0xffffffff){
            memset(OpenID, 0, sizeof(OpenID));
            memset(Info, 0, sizeof(Info));
        }
    };
    
    /**
     * IGCloudVoiceNotify is a notify for voice engine. You should implement it to get the callback message.
     */
    class GCLOUD_VOICE_API IGCloudVoiceNotify
    {
    public:
        IGCloudVoiceNotify();
        virtual ~IGCloudVoiceNotify();
        
    public:
        /*************************************************************
         *                  Real-Time Voice Callbacks
         *************************************************************/
        /**
         * Callback after you called JoinXxxRoom, you can get the result of JoinXxxRoom from the parameters.
         *
         * @param code: A GCloudVoiceCompleteCode code. You should check this first the get the result of successful or not.
         * @param roomName: Name of the room which you joined, it is the one you set in JoinXxxRoom method.
         * @param memberID: If success, returns the player's ID in this room.
         * @see JoinTeamRoom, JoinNationalRoom, JoinRangeRoom
         * @see GCloudVoiceCompleteCode
         */
        virtual void OnJoinRoom(GCloudVoiceCompleteCode code, const char *roomName, int memberID) ;
        
        /**
         * Callback when someone in the same room changes saying status, such as begining saying from silence or stopping saying.
         *
         * @param members: An int array composed of [memberid_0, status, memberid_1, status ... memberid_2*count, status],
         * here, status could be 0, 1, 2. 0 means being silence from saying, 1 means begining saying from silence
         * and 2 means continue saying.
         * @param count: The count of members who's status has changed.
         */
        virtual void OnMemberVoice(const unsigned int *members, int count) ;
        
        /**
         * Callback when someone in the same room changes saying status, such as begining saying from silence or stopping saying.
         *
         * @param roomName: Name of the room which you joined.
         * @param member: The ID of the member who's status has changed.
         * @param status : Status could be 0, 1, 2. 0 means being silence from saying, 1 means begining saying from silence
         * and 2 means continue saying.
         */
        virtual void OnMemberVoice(const char *roomName, unsigned int member, int status);
        
        /**
         * Callback after you called ChangeRole, you can get the result of ChangeRole from the parameters.
         *
         * @param code: A GCloudVoiceCompleteCode code. You should check this first the get the result of successful or not.
         * @param roomName: Name of the room which the member joined.
         * @param memberID: The ID of the member who changed role.
         * @param role: Current role of the member, Anchor or Audience.
         * @see ChangeRole
         * @see GCloudVoiceCompleteCode
         */
        virtual void OnRoleChanged(GCloudVoiceCompleteCode code, const char *roomName, int memberID, int role);
        
        /**
         * Callback when dropped from the room. When a member be offline more than 1min, he will be dropped from the room.
         *
         * @param code: A GCloudVoiceCompleteCode code. You should check this first the get the result of successful or not.
         * @param roomName: Name of the room which the member joined.
         * @param memberID: If success, return the ID of the mermber who has been dropped from the room.
         * @see GCloudVoiceCompleteCode
         */
        virtual void OnStatusUpdate(GCloudVoiceCompleteCode status, const char *roomName, int memberID) ;
        
        /**
         * Callback after you called QuitRoom, you can get the result of QuitRoom from the parameters.
         *
         * @param code: A GCloudVoiceCompleteCode code. You should check this first the get the result of successful or not.
         * @param roomName: Name of the room which you quited.
         * @see QuitRoom
         * @see GCloudVoiceCompleteCode
         */
        virtual void OnQuitRoom(GCloudVoiceCompleteCode code, const char *roomName) ;
        
        
        /*************************************************************
         *                  Voice Messages Callbacks
         *************************************************************/
        /**
         * Callback after you called ApplyMessageKey, you can get the result of ApplyMessageKey from the parameters.
         *
         * @param code: A GCloudVoiceCompleteCode code. You should check this first the get the result of successful or not.
         * @see ApplyMessageKey
         * @see GCloudVoiceCompleteCode
         */
        virtual void OnApplyMessageKey(GCloudVoiceCompleteCode code) ;
        
        /**
         * Callback when client is using microphone recording audio.
         *
         * @param pAudioData: Audio data pointer.
         * @param nDataLength: Audio data length.
         */
        virtual void OnRecording(const unsigned char* pAudioData, unsigned int nDataLength);
        
        /**
         * Callback after you called UploadRecordedFile, you can get the result of UploadRecordedFile from the parameters.
         *
         * @param code: A GCloudVoiceCompleteCode code. You should check this first the get the result of successful or not.
         * @param filePath: The path of the voice file uploaded.
         * @param fileID: If success, return the ID of the file.
         * @see UploadRecordedFile
         * @see GCloudVoiceCompleteCode
         */
        virtual void OnUploadFile(GCloudVoiceCompleteCode code, const char *filePath, const char *fileID) ;
        
        /**
         * Callback after you called DownloadRecordedFile, you can get the result of DownloadRecordedFile from the parameters.
         *
         * @param code: A GCloudVoiceCompleteCode code. You should check this first the get the result of successful or not.
         * @param filePath: The path of the file which the voice download to.
         * @param fileID: If success,return the ID of the file.
         * @see DownloadRecordedFile
         * @see GCloudVoiceCompleteCode
         */
        virtual void OnDownloadFile(GCloudVoiceCompleteCode code, const char *filePath, const char *fileID) ;
        
        /**
         * Callback after you called PlayRecordedFile and the voice file has been played to the end, you can get the result of PlayRecordedFile from the parameters.
         *
         * @param code: A GCloudVoiceCompleteCode code. You should check this first the get the result of successful or not.
         * @param filePath: The path of the file which had been played.
         * @see PlayRecordedFile
         * @see GCloudVoiceCompleteCode
         */
        virtual void OnPlayRecordedFile(GCloudVoiceCompleteCode code, const char *filePath) ;
        
        
        /*************************************************************
         *                  Translation Callbacks
         *************************************************************/
        /**
         * Callback after you called SpeechToText, you can get the result of SpeechToText from the parameters.
         *
         * @param code: A GCloudVoiceCompleteCode code. You should check this first the get the result of successful or not.
         * @param fileID: The ID of the file which had been translated.
         * @param result: If success, return the translation result, which is a piece of text in a specific language.
         * @see SpeechToText
         * @see GCloudVoiceCompleteCode
         */
        virtual void OnSpeechToText(GCloudVoiceCompleteCode code, const char *fileID, const char *result) ;
        
        /**
         * Callback after you called StopRecording in RSTT mode, you can get the result of stream speech to text from the parameters.
         *
         * @param code: A GCloudVoiceCompleteCode code. You should check this first the get the result of successful or not.
         * @param error: An error code for internal use, you can ignore it.
         * @param result: If success, return the translation result, which is a piece of text in a specific language.
         * @param voicePath: The path of the voice file.
         * @see GCloudVoiceCompleteCode
         */
        virtual void OnStreamSpeechToText(GCloudVoiceCompleteCode code, int error, const char *result, const char *voicePath);
        
        
        /*************************************************************
         *                  Other Callbacks
         *************************************************************/
        /**
         * Event Callback. e.g. the device connect Event, the device disconcet Event
         * @see GCloudVoiceEvent
         */
        virtual void OnEvent(GCloudVoiceEvent event, const char * info);
        
        /**
         * Callback after you called CheckDeviceMuteState, you can get the result of CheckDeviceMuteState from the parameters.
         *
         * @param: nState Mute state flag. Non-zero means mute state.
         * @see: GCloudVoiceErrno IGCloudVoiceEngine::CheckDeviceMuteState()
         **/
        virtual void OnMuteSwitchResult(int nState);

        /**
         * Callback after you called ReportPlayer.
         *
         * @param nCode: the reported result, 0 means server receive your reporter succ, @see GCloudVoiceCompleteCode
         * @param cszInfo: the infomation got from server
         * @see: GCloudVoiceErrno IGCloudVoiceEngine::CheckDeviceMuteState()
         **/
        virtual void OnReportPlayer(GCloudVoiceCompleteCode nCode, const char* cszInfo);

        /**
         * Callback when rec and upload the fileindex for LGameVideo Voice
         **/
        virtual void OnSaveRecFileIndex(GCloudVoiceCompleteCode code, const char *fileid, int fileindex) ;
		/**
         * Callback when member join in room or quit room
		 *
		 * @param code : GV_ON_ROOM_MEMBER_INROOM/GV_ON_ROOM_MEMBER_OUTROOM
		 * @param roomNam : roomName
		 * @param memid : room memberid
		 * @param openID : openid
		 * @return : void
         */
		virtual void OnRoomMemberInfo(GCloudVoiceCompleteCode code, const char* roomName, int memid, const char* openID);
		
		/// @brief Callback function for speech translate
		///
		/// @param nCode, this operation's result @enum GCloudVoiceCompleteCode.
		/// @param srcText, text that the source speech file translate to.
		/// @param targetText, target text that translated from source text.
		/// @param targetFileID, ID of the target speech file.
		/// @param srcFileDuration, duration of the source speech file, the unit is milliseconds.
		virtual void OnSpeechTranslate(GCloudVoiceCompleteCode nCode, const char* srcText, const char* targetText, const char* targetFileID, int srcFileDuration);

		/// @brief Callback function for real-time speech translate
		///
		/// @param nCode, this operation's result @enum GCloudVoiceCompleteCode.
		/// @param srcLang, speech language associated with fileID.
		/// @param targetLang, target language that we want to translate to.
		/// @param srcText, text that the source speech file translate to.
		/// @param targetText, target text that translated from source text.
		/// @param targetFileID, ID of the target speech file.
		/// @param srcFileDuration, duration of the source speech file, the unit is milliseconds.
		virtual void OnRSTS(GCloudVoiceCompleteCode nCode, SpeechLanguageType srcLang, SpeechLanguageType targetLang, const char* srcText, const char* targetText, const char* targetFileID, int srcFileDuration);

		/// @brief Callback function for tts
		///
		/// @param nCode, this operation's result @enum GCloudVoiceCompleteCode.
		/// @param text, source text, from request.
		/// @param lang, source text language.
		/// @param fileID, ID of the target speech file.
		virtual void OnTextToSpeech(GCloudVoiceCompleteCode nCode, const char* text, SpeechLanguageType lang, const char* fileID);
		
		
		/// @brief Callback function for ttt
		///
		/// @param nCode, this operation's result @enum GCloudVoiceCompleteCode.
		/// @param srcLang, speech language associated with fileID.
		/// @param srcText, text that the source speech file translate to.
		/// @param targetLang, target language that we want to translate to.
		/// @param targetText, text that the source text translate to.
		virtual void OnTextTranslate(GCloudVoiceCompleteCode nCode, SpeechLanguageType srcLang, const char* srcText, SpeechLanguageType targetLang, const char* targetText);

        /// @brief Callback function for speech QueryUserInfo
        ///
        /// @param nCode, this operation's result @enum GCloudVoiceCompleteCode.
        /// @param roomName, name of the room .
        /// @param member, member to query.
        virtual void OnQueryUserInfo(GCloudVoiceCompleteCode code, const char* roomName, WXMemberInfo *member);
        
        /// @brief Callback function for  WXMemberVoiceHandler
        ///
        /// @param members,An int array composed of [memberid_0, status, memberid_1, status ... memberid_2*count, status],
        /// here, status could be 0, 1, 2. 0 means being silence from saying, 1 means begining saying from silence
        /// and 2 means continue saying.
        /// @param count, how many members.
        /// @param roomName, name of the room.
        virtual void OnWXMemberVoice(const char* roomName, unsigned int *members, int count);
        
        /// @brief Callback function for speech QueryUserInfo
        ///
        /// @param code, this operation's result @enum GCloudVoiceCompleteCode.
        /// @param members, all members' info.
        /// @param count, count of members.
        virtual void OnQueryWXMembers(GCloudVoiceCompleteCode code,const char* roomName, WXMemberInfo *members, int count);
        
        /// @brief Callback function for UploadUserInfo
        ///
        /// @param code, this operation's result @enum GCloudVoiceCompleteCode.
        /// @param roomName, name of the room .
        /// @param memberID, memberID to query.
        virtual void OnUpdateUserInfo(GCloudVoiceCompleteCode code, const char* roomName, unsigned int memberID);
        /// @brief Callback function for OnUpdateMicLevel
        ///
        /// @param level, mic vol value .

		virtual void OnUpdateMicLevel(int level);

		/// @brief Callback function for EnableTranslate
		///
		/// @param code, this operation's result @enum GCloudVoiceCompleteCode.
		/// @param roomName, name of the room .
		/// @param transType, refer to EnableTranslate.
		virtual void OnEnableTranslate(GCloudVoiceCompleteCode code, const char *roomName, RealTimeTranslateType transType);
  
		/// @brief After call EnableTranslate function to enable realtime voice STT, we can get realtime translated text by this callback
		///
		/// @param sessionID, ID of the Session for one sentence.
		/// @param seq, The sequence of each segment in a sentence
		/// @param roomID, ID of the room which a player joined in.
		/// @param memberID, the player's ID in this room.
		/// @param text, the result of realtime voice stt, utf-8.
		virtual void OnRealTimeTranslateText(const char *roomName, unsigned int memberID, const char* sessionID, unsigned int seq, const char* text);

        /**
         * Callback method of EnableMagicVoice.
         * @see EnableMagicVoice
         *
         * @param code: A GCloudVoiceCompleteCode code. You should check this first the get the result of successful or not.
         * @param magicType: MagicVoice type the player want to translate to
         * @param enable: true for enable MagicVoice function, and false for disable MagicVoice function
         * @see GCloudVoiceCompleteCode
         */
        virtual void OnEnableMagicVoice(GCloudVoiceCompleteCode code, const char* magicType, bool enable);
        
        /**
         * Callback method of EnableRecvMagicVoice.
         * @see EnableRecvMagicVoice
         *
         * @param code: A GCloudVoiceCompleteCode code. You should check this first the get the result of successful or not.
         * @param enable: true for enable RecvMagicVoice function, and false for disable RecvMagicVoice function
         * @see GCloudVoiceCompleteCode
         */
        virtual void OnEnableRecvMagicVoice(GCloudVoiceCompleteCode code, bool enable);

		/// @brief Callback function for stream tts
		///
		/// @param nCode, GV_ON_OK when TTS succeed, else GV_ON_FAIL.
		/// @param text, source text, from request.
		/// @param error code, defail of fail.
		virtual void OnTextToStreamSpeech(GCloudVoiceCompleteCode nCode, const char* text, int err);

	/// @brief Callback function for report speech translate
	///
	/// @param nCode, this operation's result @enum GCloudVoiceCompleteCode.
	/// @param transText, translation text that translated from report speech.
	/// @param openID, ID of the reported player's openid.
	virtual void OnSTTReport(GCloudVoiceCompleteCode nCode, const char* transText, const char* openID,const char *fileid);

	/// @brief Callback method of magic voice message function.
	///
	/// @param nCode, this operation's result @enum GCloudVoiceCompleteCode.
	/// @param fileID, if magic voice file upload succ ,or NULL 
	virtual void OnMagicVoiceMsg(GCloudVoiceCompleteCode nCode,const char *filePath);

	/// @brief Callback function for security info to gameclient forbide/punish user voice ablitiy
	///
	/// @param nRet, security return code; 0:not forbide user talk, >0 forbide/punish user voice
	/// @param roomname, roomname
	/// @param sectransinfo, the info from security department.
	virtual void OnRTSecInfo(int nRet, const char* roomname, const char* sectransinfo);

	};
}// endof namespace gcloud_voice

#endif /* GCloudVoiceNotify_h */
