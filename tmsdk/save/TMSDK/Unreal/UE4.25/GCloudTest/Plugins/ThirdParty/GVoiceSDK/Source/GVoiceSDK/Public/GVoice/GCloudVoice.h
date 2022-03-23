/*******************************************************************************\
 ** C++ API for GCloudVoice
 **
 ** GVoice(Game Voice) is a voice service that covers diverse game scenes.
 ** GVoice supports multiple voice modes, such as RealTime mode, Messages mode,
 ** Translation mode and RSTT mode.
 **
 ** In RealTime mode, multiple members can join in the same room to communicate with each other.
 ** There are four different scenes in RealTime mode, they are TeamRoom, NationalRoom,
 ** RangeRoom and FMRoom.
 ** The workflow of RealTime mode is like below:
 ** GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RealTime)-->
 ** JoinXxxRoom-->...-->QuitRoom
 **
 ** In Messages mode, a member can quickly record and send a voice message to other members.
 ** The workflow of Messages mode is like below:
 ** For record side:
 ** GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(Messages)-->
 ** ApplyMessageKey-->StartRecording-->StopRecording-->UploadRecordedFile
 ** Or for play side:
 ** GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(Messages)-->
 ** ApplyMessageKey-->DownloadRecordedFile-->PlayRecordedFile-->StopPlayFile
 **
 ** In Translation mode, members can translate a recorded voice message to a piece of
 ** text in a specific language.
 ** The workflow of Translation mode is like below:
 ** GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(Translation)-->
 ** ApplyMessageKey-->StartRecording-->StopRecording-->SpeechToText
 ** Then you can get the translation result from the callback method "OnSpeechToText".
 **
 ** In RSTT mode, members can translate a recorded voice message to a piece of
 ** text in a specific language in realtime.
 ** The workflow of RSTT mode is like below:
 ** GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RSTT)-->
 ** ApplyMessageKey-->StartRecording-->StopRecording
 ** Then you can get the translation result from the callback method "OnStreamSpeechToText".
 **
 ** Notice: GVoice SDK uses asynchronous callback mechanism to notify you the result of a
 ** function, so please remember to do the following three things:
 ** 1. Implement the interface of IGCloudVoiceNotify;
 ** 2. Call the SetNotify method to set an appropriate implementation object of IGCloudVoiceNotify;
 ** 3. Periodically call the Poll method to drive the engine return the callback results.
 **
 \*******************************************************************************/

#ifndef gcloud_voice_GCloudVoice_h_
#define gcloud_voice_GCloudVoice_h_

#include "GCloudVoiceErrno.h"
#include "GCloudVoiceExtension.h"
#include "GCloudVoiceNotify.h"
#include <stdlib.h>
#include <string.h>
#include <stdint.h>

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

namespace gcloud_voice
{
    class IGCloudVoiceEngine: public IGCloudVoiceEngineExtension
    {
    
        /*************************************************************
         *                  Basic common APIs
         *************************************************************/
    public:
        /**
         * Set your app's info such as appID/appKey.
         *
         * SetAppInfo method should be called after you have gotten the voice engine by GetVoiceEngine.
         * e.g. GetVoiceEngine-->SetAppInfo
         *
         * @param appID: Your game ID after you have registered.
         * @param appKey: Your game key after you have registered.
         * @param openID: A unique user ID, any string which can uniquely identify a user.
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno SetAppInfo(const char *appID,const char *appKey, const char *openID)=0;

        /**
         * Initialize the GCloudVoice engine.
         *
         * Init method should be called after you have set the app information by SetAppInfo.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init
         *
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno Init()=0;
        
        /**
         * Set the notify to engine.
         *
         * SetNotify method should be called after you have initialized the voice engine by Init.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify
         *
         * @param notify: The notify object.
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see IGCloudVoiceNotify
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno SetNotify(IGCloudVoiceNotify *notify)=0;
        
        /**
         * Set the mode for the voice engine.
         *
         * SetMode method should be called after you have initialized the voice engine by Init.
         * You should choose an appropriate mode for your application according to the function you need.
         *
         * @param mode: Mode to set @see GCloudVoiceMode
         *              RealTime:    realtime mode for TeamRoom, NationalRoom or RangeRoom
         *              Messages:    voice message mode
         *              Translation: speach to text mode
         *              RSTT:        real-time speach to text mode
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno SetMode(GCloudVoiceMode mode)=0;
        
        /**
         * Trigger engine's callback. You should invoke poll on your loop periodically.
         *
         * Poll method should be called after you have initialized the voice engine by Init.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll
         *
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno Poll()=0;
        
        /**
         * The Application's Pause.
         * When your app pause such as goto backend, you should invoke this.
         *
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno Pause()=0;
        
        /**
         * The Application's Resume.
         * When your app resume such as come back from backend, you should invoke this.
         *
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno Resume()=0;
        

        /*************************************************************
         *                  Real-Time Voice APIs
         *************************************************************/
    public:
        /**
         * Join in a team room.
         * Team room function allows no more than 20 members join in the same room to communicate freely.
         *
         * JoinTeamRoom method should be called after you have set the engine mode to RealTime.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RealTime)-->JoinTeamRoom
         * -->.....-->QuitRoom
         *
         * The result of joining room successful or not can be obtained by the callback method OnJoinRoom.
         * @see OnJoinRoom
         *
         * @param roomName: The name of The room to join, it should be a string composed by 0-9A-Za-Z._- and less than 127 bytes.
         * @param msTimeout: The length of the timeout for joining, it is a micro second, value range[5000, 60000].
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno JoinTeamRoom(const char *roomName, int msTimeout = 10000)=0;

        /**
         * Join in a LBS room.
         * RangeRoom function allows user to join a LBS room.
         * After joined a RangeRoom, the member can hear the members' voice within a specific range.
         *
         * JoinRangeRoom method should be called after you have set the engine mode to RealTime.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RealTime)-->JoinRangeRoom
         * -->.....-->QuitRoom
         *
         * The result of joining room successful or not can be obtained by the callback method OnJoinRoom.
         * @see OnJoinRoom
         *
         * @param roomName: The name of The room to join, it should be composed by 0-9A-Za-Z._- and less than 127 bytes.
         * @param msTimeout: The length of the timeout for joining, it is a micro second, value range[5000, 60000].
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno JoinRangeRoom(const char *roomName, int msTimeout = 10000)=0;
        
        /**
         * Join in a national room.
         * National room function allows more 20 members to join in the same room, and they can choose two different roles to be.
         * The Anchor role can open microphone to speak and open speaker to listen.
         * The Audience role can only open the speaker to listen.
         *
         * JoinNationalRoom method should be called after you have set the engine mode to RealTime.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RealTime)-->JoinNationalRoom
         * -->.....-->QuitRoom
         *
         * The result of joining room successful or not can be obtained by the callback method OnJoinRoom.
         * @see OnJoinRoom
         *
         * @param roomName: The name of The room to join, it should be composed by 0-9A-Za-Z._- and less than 127 bytes.
         * @param role: A GCloudVoiceMemberRole value illustrates wheather the player can send voice data or not.
         * @param msTimeout: The length of the timeout for joining, it is a micro second, value range[5000, 60000].
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno JoinNationalRoom(const char *roomName, GCloudVoiceMemberRole role, int msTimeout = 10000)=0;
        
        /**
         * Update your coordinate.
         *
         * UpdateCoordinate method should be called after the member has joined a RangeRoom successfully.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RealTime)-->JoinRangeRoom
         * -->UpdateCoordinate-->.....-->QuitRoom
         *
         * @param roomName: The name of The room to update coordinate, it should be an exist room name.
         * @param x: The x coordinate.
         * @param y: The y coordinate.
         * @param z: The z coordinate.
         * @param r: The audience's radius.
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno UpdateCoordinate(const char *roomName, int64_t x, int64_t y, int64_t z, int64_t r)=0;
        
        /**
         * Change the member's role in a national room.
         *
         * ChangeRole is a function in NationalRoom, so this method should be called after the member has
         * joined a NationalRoom successfully.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RealTime)-->JoinNationalRoom
         * -->.....-->ChangeRole-->.....-->QuitRoom
         *
         * The result of changing role successful or not can be obtained by the callback method OnRoleChanged.
         * @see OnRoleChanged
         *
         * @param role: The member's role want to change to.
         * @param roomName: The name of The room to change role, it should be an exist national room name.
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceMemberRole
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno ChangeRole(GCloudVoiceMemberRole role, const char *roomName="")=0;
        
        /**
         * Open the player's microphone and begin to send the player's voice data.
         *
         * OpenMic method should be called after the member has joined a voice room successfully.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RealTime)-->JoinXxxRoom
         * -->OpenMic-->.....-->QuitRoom
         *
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno OpenMic()=0;
        
        /**
         * Close the players's microphone and stop sending the player's voice data.
         *
         * CloseMic method should be called after the member has joined a voice room successfully.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RealTime)-->JoinXxxRoom
         * -->.....-->OpenMic-->CloseMic-->.....-->QuitRoom
         *
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno CloseMic()=0;
        
        /**
         * Open the player's speaker and begin to recvie voice data from the network.
         *
         * OpenSpeaker method should be called after the member has joined a voice room successfully.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RealTime)-->JoinXxxRoom
         * -->.....-->OpenSpeaker-->.....-->QuitRoom
         *
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno OpenSpeaker()=0;
        
        /**
         * Close the player's speaker and stop reciving voice data from the network.
         *
         * CloseSpeaker method should be called after the member has joined a voice room successfully.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RealTime)-->JoinXxxRoom
         * -->.....-->OpenSpeaker-->CloseSpeaker-->.....-->QuitRoom
         *
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno CloseSpeaker()=0;
        
        /**
         * Quit the voice room.
         *
         * QuitRoom method should be called after the member has joined a voice room successfully.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RealTime)-->JoinXxxRoom
         * -->.....-->QuitRoom
         *
         * The result of quiting room successful or not can be obtained by the callback method OnQuitRoom
         * @see OnQuitRoom
         *
         * @param roomName: The name of The room to quit, it should be composed by 0-9A-Za-Z._- and less than 127 bytes
         * and should be an exist room names.
         * @param msTimeout: The length of the timeout for quiting, it is a micro second, value range[5000, 60000].
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno QuitRoom(const char *roomName, int msTimeout = 10000)=0;
        
        
        /*************************************************************
         *                  Messages Voice APIs
         *************************************************************/
    public:
        /**
         * Apply the key for voice message.
         * In Messages, Translation and RSTT mode, you should first apply the message key before you use the functions.
         *
         * ApplyMessageKey method should be called after you have set the voice mode to Messages, Translation or RSTT.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(Messages)-->ApplyMessageKey-->...
         *
         * The result of applying message key successful or not can be obtained by the callback method OnApplyMessageKey.
         * @see OnApplyMessageKey
         *
         * @param msTimeout: The length of the timeout for applying, it is a micro second, value range[5000, 60000].
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno ApplyMessageKey(int msTimeout = 10000)=0;
        
        /**
         * Open the player's microphone and record the player's voice.
         *
         * StartRecording method should be called in Messages, Translation or RSTT mode, and after you have
         * applied the message key successfully.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(Messages)-->ApplyMessageKey
         * -->StartRecording-->...
         *
         * @param filePath: The path of the file to store the voice data, the filePath should like:"your_dir/your_file_name".
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno StartRecording(const char * filePath, bool bNotVoip = false)=0;
        
        /**
         * Stop the player's microphone and stop record the player's voice.
         *
         * StopRecording method should be called in Messages, Translation or RSTT mode, and after you have
         * applied the message key successfully.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(Messages)-->ApplyMessageKey
         * -->StartRecording-->StopRecording-->...
         *
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno StopRecording()=0;
        
        /**
         * Upload the player's voice message file to the network.
         *
         * UploadRecordedFile method should be called in Messages or Translation mode, and after you have
         * recorded a voice message successfully.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(Messages)-->ApplyMessageKey
         * -->StartRecording-->StopRecording-->UploadRecordedFile-->...
         *
         * The result of uploading recorded file successful or not can be obtained by the callback method OnUploadFile.
         * @see OnUploadFile
         *
         * @param filePath: The path of the voice file to upload, the filePath should like:"your_dir/your_file_name"
         * @param msTimeout: The length of the timeout for uploading, it is a micro second, value range[5000, 60000].
		 * @param bPermanent: if set true, server will never delete upload-file but limit the NO. of uploads, if set false, upload-file will keep 7 days and not limited.
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno UploadRecordedFile(const char * filePath, int msTimeout = 60000, bool bPermanent = false)=0;
        
        /**
         * Download other players' voice message from the network.
         *
         * DownloadRecordedFile method should be called in Messages mode, and after the other member has
         * uploaded a voice message successfully.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(Messages)-->ApplyMessageKey
         * -->...-->DownloadRecordedFile-->...
         *
         * The result of downloading recorded file successful or not can be obtained by the callback method OnDownloadFile.
         * @see OnDownloadFile
         *
         * @param fileID: The ID of the file to be downloaded. FileID can be obtained from the callback method OnUploadFile.
         * @param downloadFilePath: The path of the voice file to download, the filePath should like:"your_dir/your_file_name".
         * @param msTimeout: The length of the timeout for downloading, it is a micro second, value range[5000, 60000].
		 * @param bPermanent: if the file is permanently saved on the server, set true, if not, set false
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno DownloadRecordedFile(const char *fileID, const char * downloadFilePath, int msTimeout = 60000, bool bPermanent = false)=0;
        
        /**
         * Play local voice message file.
         *
         * PlayRecordedFile method should be called in Messages mode, and after you have
         * recorded a voice message successfully or downloaded a voice message successfully.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(Messages)-->ApplyMessageKey
         * -->StartRecording-->StopRecording-->PlayRecordedFile-->...
         * or GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(Messages)-->ApplyMessageKey
         * -->DownloadRecordedFile-->PlayRecordedFile-->...
         *
         * If the voice file has been played to the end normally, the callback method OnPlayRecordedFile will be called.
         * And if you called StopPlayFile method before the end of the voice file, OnPlayRecordedFile will not be called.
         * @see OnPlayRecordedFile
         *
         * @param filePath: The path of the voice file to play, the filePath should like:"your_dir/your_file_name".
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno PlayRecordedFile(const char * downloadFilePath)=0;
        
        /**
         * Stop playing the voice file.
         *
         * StopPlayFile method should be called in Messages mode, and before the voice message has been played to the end.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(Messages)-->ApplyMessageKey
         * -->...-->PlayRecordedFile-->StopPlayFile
         *
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno StopPlayFile()=0;
        
        
        /*************************************************************
         *                  Translation APIs
         *************************************************************/
    public:
        /**
         * Translate the voice data to text in a specific language, the default language is Chinese.
         *
         * SpeechToText method should be called in Translation mode, and after you have
         * uploaded a voice message successfully.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(Translation)-->ApplyMessageKey
         * -->StartRecording-->StopRecording-->SpeechToText-->...
         *
         * The result of translating successful or not can be obtained by the callback method OnSpeechToText.
         * @see OnSpeechToText
         *
         * @param fileID: The ID of the file to be translated. FileID can be obtained from the callback method OnUploadFile.
         * @param msTimeout: The length of the timeout for translating, it is a micro second, value range[5000, 60000].
         * @param language: The specific language to be translated to.
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudLanguage
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno SpeechToText(const char *fileID, int msTimeout = 60000,  GCloudLanguage language = Chinese)=0;
        
        
        /*************************************************************
         *                  Token related APIs
         * Deprecated APIs, please move to the APIs ends with token
         * in IGCloudVoiceExtension class
         *************************************************************/
    public:
        /**
         * Join in a team room with token.
         * Team room function allows no more than 20 members join in the same room to communicate freely.
         *
         * JoinTeamRoom method should be called after you have set the engine mode to RealTime.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RealTime)-->JoinTeamRoom
         * -->.....-->QuitRoom
         *
         * The result of joining room successful or not can be obtained by the callback method OnJoinRoom.
         * @see OnJoinRoom
         *
         * @param roomName: The name of The room to join, it should be composed by 0-9A-Za-Z._- and less than 127 bytes.
         * @param msTimeout: The length of the timeout for joining, it is micro second, value range[5000, 60000].
         * @param token:
         * @param timestamp:
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno JoinTeamRoom(const char *roomName, const char *token, int timestamp, int msTimeout = 10000)=0;
        
        /**
         * Join in a national room with token.
         * National room function allows more than 20 members to join in the same room, and they can choose two different roles to be.
         * The Anchor role can open microphone to speak and open speaker to listen.
         * The Audience role can only open the speaker to listen.
         *
         * JoinNationalRoom method should be called after you have set the engine mode to RealTime.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RealTime)-->JoinNationalRoom
         * -->.....-->QuitRoom
         *
         * The result of joining room successful or not can be obtained by the callback method OnJoinRoom.
         * @see OnJoinRoom
         *
         * @param roomName: The name of The room to join, it should be composed by 0-9A-Za-Z._- and less than 127 bytes.
         * @param role: A GCloudVoiceMemberRole value illustrates wheather the player can send voice data or not.
         * @param token:
         * @param timestamp:
         * @param msTimeout: The length of the timeout for joining, it is micro second, value range[5000, 60000].
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno JoinNationalRoom(const char *roomName, GCloudVoiceMemberRole role, const char *token, int timestamp, int msTimeout = 10000)=0;
        
        /**
         * Apply the key for voice message with token.
         * In Messages, Translation and RSTT mode, you should first apply the message key before you use the functions.
         *
         * ApplyMessageKey method should be called after you have set the voice mode to Messages, Translation or RSTT.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(Messages)-->ApplyMessageKey-->...
         *
         * The result of applying message key successful or not can be obtained by the callback method OnApplyMessageKey.
         * @see OnApplyMessageKey
         *
         * @param token:
         * @param timestamp:
         * @param msTimeout: The length of the timeout for joining, it is micro second, value range[5000, 60000].
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno ApplyMessageKey(const char *token, int timestamp, int msTimeout = 10000)=0;
        
        /**
         * Translate the voice data to a piece of text in a specific language with token, the default language is Chinese.
         *
         * SpeechToText method should be called in Translation mode, and after you have
         * uploaded a voice message successfully.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(Translation)-->ApplyMessageKey
         * -->StartRecording-->StopRecording-->SpeechToText-->...
         *
         * The result of translating successful or not can be obtained by the callback method OnSpeechToText.
         * @see OnSpeechToText
         *
         * @param fileID: The ID of the file to be translated. FileID can be obtained from the callback method OnUploadFile.
         * @param token:
         * @param timestamp:
         * @param msTimeout: The length of the timeout for join, it is micro second, value range[5000, 60000].
         * @param language: The specific language to be translated to.
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudLanguage
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno SpeechToText(const char *fileID, const char *token, int timestamp, int msTimeout = 60000,  GCloudLanguage language = Chinese)=0;
	};
    
    /**
     * Get the voice engine instance.
     *
     * @return the voice instance on success, or NULL on failed.
     */
    extern "C" GCLOUD_VOICE_API IGCloudVoiceEngine *GetVoiceEngine();

    /**
     * Get the voice engine instance, different from GetVoiceEngine,
     * if the voice engine has not created, this method will return null
     *
     * @return the voice instance or NULL.
     */
	extern "C" GCLOUD_VOICE_API IGCloudVoiceEngine *GetVoiceEngine_OrNull();
    
} // end of namespace gcloud_voice

#endif /* gcloud_voice_GCloudVoice_h_ */
