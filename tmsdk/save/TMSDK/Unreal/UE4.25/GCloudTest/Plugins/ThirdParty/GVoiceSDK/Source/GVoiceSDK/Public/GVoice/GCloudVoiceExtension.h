/*******************************************************************************************\
 **
 ** GVoice(Game Voice) is a voice service that covers diverse game scenes.
 ** GVoice supports multiple voice modes, such as RealTime mode, Messages Mode,
 ** Translation mode and RSTT mode.
 **
 ** This file includes the extension APIs in GVoice SDK, which supplies the additional functions
 ** in GVoice, such as playing background music, setting the voice effect mode and so on.
 **
 \*******************************************************************************************/

#ifndef GCloudVoiceExtension_h
#define GCloudVoiceExtension_h

#include "GCloudVoiceErrno.h"
#include "GCloudVoice.h"

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

typedef void (*GVoiceLogFunc) (const char *str);

namespace gcloud_voice
{
#define OPENID_LEN_MAX 128
#define DEVICE_DESCRIBLE_CHAR_NUM 256

#pragma pack(push,1)
	struct RoomMembers
	{
		int memberid ;
		char openid[OPENID_LEN_MAX];
		int micstatus;
	};

	enum DeviceType {
		RENDER_DEVICE = 0,
		CAPTURE_DEVICE = 1,
	};

	struct Device_Info
	{
		char deviceid[DEVICE_DESCRIBLE_CHAR_NUM * 2];
		char devicename[DEVICE_DESCRIBLE_CHAR_NUM * 2];
	};


	typedef struct GVoice3DVector
	{
		float x;
		float y;
		float z;
	}GVoiceVector;

	enum EVoiceChatAttenuationModel
	{
		/** No attenuation is applied. The audio will drop to 0 at MaxDistance */
		None,
		/** The attenuation increases in inverse proportion to the distance. The Rolloff is the inverse of the slope of the attenuation curve. */
		InverseByDistance,
		/** The attenuation increases in inverse proportion to the distance raised to the power of the Rolloff. */
		ExponentialByDistance
	};

	typedef struct GVoice3DDistProperties
	{
		/** The model used to determine how loud audio is at different distances */
		EVoiceChatAttenuationModel AttenuationModel;
		/** The distance at which the sound will start to attenuate */
		//setmin distance d_r，0.2 <= d_r <= 100
		// if fRefDist < 0.2，d_r = 0.2
		// if fRefDist > 100，d_r = 100
		float MinDistance;
		/** The distance at which sound will no longer be audible */
		// Setmaxdistance d_m，1 <= d_m <= 500
		// fMaxDist < 1，d_m = 1
		// fMaxDist > 500，d_m = 500
		float MaxDistance;
		/** How fast the sound attenuates with distance */
		/** fRollFactor r_f，0.1 <= r_f <= 100
		// if fRollFactor < 0.1, r_f = 0.1
		// if fRollFactor > 100, r_f = 100/
		**/
		float Rolloff;
	}GVoice3DDistProperties;

#pragma pack(pop)

    class IGCloudVoiceEngineExtension{
        
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
		* @param scenesName: The scene of entering the room, Used to classify rooms, 
		*	one scene can only enter one room, will quit the previous room in the same scene automatically, 
		*	one room can only be associated with one scene, will fail to join room with anothor scenes,
		*	limit 127 bytes.
		* @param roomName: The name of The room to join, it should be a string composed by 0-9A-Za-Z._- and less than 127 bytes.
		* @param msTimeout: The length of the timeout for joining, it is a micro second, value range[5000, 60000].
		* @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
		* @see GCloudVoiceErrno
		*/
		virtual GCloudVoiceErrno JoinTeamRoom_Scenes(const char *scenesName, const char *roomName, int msTimeout = 10000) = 0;

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
		* @param scenesName: The scene of entering the room, Used to classify rooms,
		*	one scene can only enter one room, will quit the previous room in the same scene automatically,
		*	one room can only be associated with one scene, will fail to join room with anothor scenes,
		*	limit 127 bytes.
		* @param roomName: The name of The room to join, it should be composed by 0-9A-Za-Z._- and less than 127 bytes.
		* @param msTimeout: The length of the timeout for joining, it is a micro second, value range[5000, 60000].
		* @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
		* @see GCloudVoiceErrno
		*/
		virtual GCloudVoiceErrno JoinRangeRoom_Scenes(const char *scenesName, const char *roomName, int msTimeout = 10000) = 0;

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
		* @param scenesName: The scene of entering the room, Used to classify rooms,
		*	one scene can only enter one room, will quit the previous room in the same scene automatically,
		*	one room can only be associated with one scene, will fail to join room with anothor scenes,
		*	limit 127 bytes.
		* @param roomName: The name of The room to join, it should be composed by 0-9A-Za-Z._- and less than 127 bytes.
		* @param role: A GCloudVoiceMemberRole value illustrates wheather the player can send voice data or not.
		* @param msTimeout: The length of the timeout for joining, it is a micro second, value range[5000, 60000].
		* @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
		* @see GCloudVoiceErrno
		*/
		virtual GCloudVoiceErrno JoinNationalRoom_Scenes(const char *scenesName, const char *roomName, GCloudVoiceMemberRole role, int msTimeout = 10000) = 0;


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
		* @param scenesName: The scene of entering the room
		* @param msTimeout: The length of the timeout for quiting, it is a micro second, value range[5000, 60000].
		* @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
		* @see GCloudVoiceErrno
		*/
		virtual GCloudVoiceErrno QuitRoom_Scenes(const char *scenesName, int msTimeout = 10000) = 0;


		/*************************************************************************
		*                  Multiroom related APIs
		*
		* Multiroom is a function in GVoice real-time mode, it allows a member to join
		* 1~16 room(s) at the same time.
		*
		* The workflow of the Multiroom function:
		* GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RealTime)-->
		* EnableMultiRoom-->JoinTeamRoom/JoinRangeRoom-->EnableRoomMicrophone/
		* EnableRoomSpeaker-->...-->QuitRoom
		*************************************************************************/
        /**
         * Enable a member to join in multi rooms. Notice that this may cause higher bitrate.
         *
         * EnableMultiRoom method should be called after you have set the mode to RealTime
         * and before you call the JoinXxxRoom method.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RealTime)-->EnableMultiRoom
         * -->JoinXxxRoom-->.....-->QuitRoom
         *
         * @param enable: Enable joining in multi rooms if it is ture, if set false, will quit the previous room automatically before entering anothor room to make sure only one room.
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         */
        virtual GCloudVoiceErrno EnableMultiRoom(bool enable)=0;
        
        /**
         * Open or close the microphone in a specific room in MultiRoom mode.
         *
         * EnableRoomMicrophone method should be called after the member has joined a voice room in MultiRoom mode successfully.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RealTime)-->EnableMultiRoom(true)
         * -->JoinXxxRoom-->EnableRoomMicrophone-->.....-->QuitRoom
         *
         * @param roomName: The name of The room to enable microphone, it should be an exist room name.
         * @param enable: Open the microphone in The room if it is true, and close the microphone if it is false.
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         */
        virtual GCloudVoiceErrno EnableRoomMicrophone(const char *roomName, bool enable)=0;
        
        /**
         * Open or Close the speaker in a specific room in MultiRoom mode.
         *
         * EnableRoomSpeaker method should be called after the member has joined a voice room in MultiRoom mode successfully.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RealTime)-->EnableMultiRoom(true)
         * -->JoinXxxRoom-->EnableRoomMicrophone-->.....-->QuitRoom
         *
         * @param roomName: The name of The room to enable speaker, it should be an exist room name.
         * @param enable: Open the speaker in The room if it is true, and close the speaker if it is false.
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         */
        virtual GCloudVoiceErrno EnableRoomSpeaker(const char *roomName, bool enable)=0;
        

        /*************************************************************************
         *                  BGM related APIs
         *
         * GVoice supports mp3 format background music.
         * The workflow of the BGM function:
         * GetVoiceEngine-->SetAppInfo-->Init-->EnableNativeBGMPlay-->SetBGMPath
         * -->StartBGMPlay-->PauseBGMPlay-->ResumeBGMPlay-->StopBGMPlay
         *************************************************************************/
    public:
        /**
         * Set The path to a BGM file.
         * SetBGMPath method should be called after you have initialized the voice engine.
         *
         * @param pPath: The path to the BGM file.
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno SetBGMPath(const char * pPath)=0;
        
        /**
         * Enable or disable the native play mode.
         * EnableNativeBGMPlay method should be called after you have initialized the voice engine.
         *
         * @param bEnable: Enable the native play mode if it is true, and disable the native play mode if it is false.
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno EnableNativeBGMPlay(bool bEnable)=0;
        
        /**
         * Start playing the BGM.
         * StartBGMPlay method should be called after you have set The path of the BGM file by SetBGMPath method.
         *
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno StartBGMPlay()=0;
        
        /**
         * Set the play volume of the BGM.
         * SetBGMVol method should be called after you have initialized the voice engine.
         *
         * @param nvol: The play volume of the BGM, which should be an integer between 0~800.
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno SetBGMVol(int nvol)=0;
        
        /**
         * Pause the BGM.
         * When you want to pause the playing of BGM or when the application paused, you can call PauseBGMPlay
         * method to pause the BGM.
         *
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno PauseBGMPlay()=0;
        
        /**
         * Resume the BGM.
         * When you want to resume the playing of BGM after paused it or when the application resumed, you can call
         * ResumeBGMPlay method to resume the BGM.
         *
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno ResumeBGMPlay()=0;
        
        /**
         * Get the state of the BGM.
         * If you want to get the playing state of the BGM, you can call GetBGMPlayState method.
         * GetBGMPlayState method should be called after you have initialized the voice engine.
         *
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        // TODO Complete the return
        virtual int GetBGMPlayState()=0;
        
        /**
         * Stop the BGM.
         * StopBGMPlay method should be called after you have initialized the voice engine.
         *
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno StopBGMPlay()=0;
        
        
        /*************************************************************
         *                  Token related APIs
         *************************************************************/
    public:
        /**
         * Join in a team room with token.
         * Team room function allows no more than 20 members join in the same room to communicate freely.
         *
         * JoinTeamRoom_Token method should be called after you have set the engine mode to RealTime.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RealTime)-->JoinTeamRoom_Token
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
        virtual GCloudVoiceErrno JoinTeamRoom_Token(const char *roomName, const char *token, int timestamp, int msTimeout = 10000)=0;
        
        /**
         * Join in a national room with token.
         * National room function allows more than 20 members to join in the same room, and they can choose two different roles to be.
         * The Anchor role can open microphone to speak and open speaker to listen.
         * The Audience role can only open the speaker to listen.
         *
         * JoinNationalRoom_Token method should be called after you have set the engine mode to RealTime.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RealTime)-->JoinNationalRoom_Token
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
        virtual GCloudVoiceErrno JoinNationalRoom_Token(const char *roomName, GCloudVoiceMemberRole role, const char *token, int timestamp, int msTimeout = 10000)=0;
        
        /**
         * Apply the key for voice message with token.
         * In Messages, Translation and RSTT mode, you should first apply the message key before you use the functions.
         *
         * ApplyMessageKey_Token method should be called after you have set the voice mode to Messages, Translation or RSTT.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(Messages)-->ApplyMessageKey_Token-->...
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
        virtual GCloudVoiceErrno ApplyMessageKey_Token(const char *token, int timestamp, int msTimeout = 10000)=0;
        
        /**
         * Translate the voice data to a piece of text in a specific language with token, the default language is Chinese.
         *
         * SpeechToText_Token method should be called in Translation mode, and after you have
         * uploaded a voice message successfully.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(Translation)-->ApplyMessageKey
         * -->StartRecording-->StopRecording-->SpeechToText_Token-->...
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
        virtual GCloudVoiceErrno SpeechToText_Token(const char *fileID, const char *token, int timestamp, int msTimeout = 60000,  GCloudLanguage language = Chinese)=0;
        
        
        /*************************************************************
         *                  Microphone or speaker related APIs
         *************************************************************/
    public:
        /**
         * Open or close the speaker.
         * EnableSpeakerOn method should be called after you have initialized the voice engine.
         *
         * @param bEnable: Open the speaker if it is true and close the speaker if it is false.
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno EnableSpeakerOn(bool bEnable)=0;
        
        /**
         * Set the volume of microphone.
         * SetMicVolume method should be called after you have initialized the voice engine.
         *
         * @param vol: The volume to set, for windows platform, the vol should in -1000～1000,
         * and in other platforms, the vol should in -150～150.
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno SetMicVolume(int vol)=0;
        
        /**
         * Set the sepaker's volume.
         * SetSpeakerVolume method should be called after you have initialized the voice engine.
         *
         * @param vol: The volume to set, for windows platform, the vol should in 0～100,
         * and in other platforms, the vol should in 0～150, the real volume is equals to (the vol / 100 * the original voice volume).
         * If you set the vol to 120, then the real vol is (1.2*the original voice volume).
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno SetSpeakerVolume(int vol)=0;
        
        /**
         * Get the microphone's volume.
         * GetMicLevel method should be called after you have initialized the voice engine.
         *
         * @return the microphone's volume, if return value>0, means you have said something captured by microphone.
         */
        virtual int GetMicLevel(bool bFadeOut=true)=0;
        
        /**
         * Get the speaker's volume.
         * GetSpeakerLevel method should be called after you have initialized the voice engine.
         *
         * @return the speaker's volume, the value is equal to the param when you call SetSpeakerVolume method.
         */
        virtual int GetSpeakerLevel()=0;
        
        /**
         * Get the microphone's state, open microphone success, failed or be occupied.
         *
         * @return: The microphone's state. -1: microphone is closed; 0: open microphone failed;
         *          1: open microphone success; 2: microphone has been occupied.
         */
        virtual int GetMicState() = 0;
        
        /**
         * Get the speaker's state, open speaker success, failed or be occupied.
         *
         * @return: The speaker's state. -1: speaker is closed; 0: open speaker failed;
         *          1: open speaker success; 2: speaker has been occupied.
         */
        virtual int GetSpeakerState() = 0;
        
        /**
         * Test wheather the microphone is available or not.
         * Before you want to open microphone, call TestMic method to check whether the microphone is available or not.
         * TestMic method should be called after you have initialized the voice engine.
         *
         * @return If microphone device is available, returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno TestMic()=0;
        
        /**
         * Detect whether the member is speaking or just keep microphone opened.
         * IsSpeaking method should be called after you have initialized the voice engine.
         *
         * @return If the member is speaking, returns true, otherwise returns false.
         */
        virtual bool IsSpeaking()const=0;
        
        /**
         * Set the state of the headset, if there is a headset connected or not.
         *
         * @param bState: there is a headset connected if it is true, and there is a headset disconnected if it is false.
         */
        virtual void SetHeadSetState(bool bState)=0;
        
        // TODO if it is needed
        /**
         * Set the state of the bluetooth, if there is a bluetooth connected or not.
         *
         * @param bState: there is a bluetooth connected if it is true, or there is a bluetooth disconnected if it is false.
         */
        virtual void SetBluetoothState(bool bState)=0;
        
        /**
         * Enable or disable the bluetooth SCO mode. When you want to capture the voice via bluetooth, you can call EnableBluetoothSCO(true).
         * EnableBluetoothSCO method should be called after you have initialized the voice engine.
         *
         * @param enable: Enable the bluetooth SCO mode if it is true, and disable the bluetooth SCO mode if it is false.
         */
        virtual void EnableBluetoothSCO(bool enable)=0;
        
        // TODO
        virtual void DeviceEventNotify(int eventId, const char *info)=0;
        
        /**
         * Identify that whether there is any device connected or not.
         *
         * @return: 0: no audio device connected; 1: a wiredheadset device connected; 2: a bluetooth device connected.
         * @see GCloudVoiceDeviceState
         */
        virtual GCloudVoiceDeviceState getAudioDeviceConnectionState()=0;
        
        /**
         * Check mute switch state; iPhone is valiable; iOS simulator and android will return non-mute.
         *
         * @return: If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         * @see void IGCloudVoiceNotify::OnMuteSwitchResult(int nState);
         **/
        virtual GCloudVoiceErrno CheckDeviceMuteState()=0;
        
        /**
         * Get the mute state of the device.
         *
         * @return: The device is muted or not. non-zero:mute state; 0: not in mute state; -1:error.
         */
        virtual int GetMuteResult()const = 0;
        
        
        /*************************************************************
         *                  Voice algorithm related APIs
         *************************************************************/
    public:
        /**
         * This method supports setting sound effect mode.
         * SetVoiceEffects method should be called after you have initialized the voice engine.
         *
         * @param mode: The sound effect to set, @see SoundEffects
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno SetVoiceEffects(SoundEffects mode)=0;
        
        /**
         * This method supports enabling sound reverb function.
         * EnableReverb method should be called after you have initialized the voice engine.
         *
         * @param bEnable: Enable the sound reverb if it is true, and disable the sound reverb if it is false.
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        // TODO return
        virtual int EnableReverb(bool bEnable)=0;
        
        /**
         * This method supports setting sound reverb mode.
         * SetReverbMode method should be called after you have initialized the voice engine.
         *
         * @param mode: The reverb mode which you want to set, the value should in 0~5, and default is 0.
         *        0: strong vocal; 1: vocal; 2: small room; 3: large room; 4: church; 5: theater
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual int SetReverbMode(int mode)=0;
        
        /**
         * Identify the type of the voice.
         * GetVoiceIdentify method should be called after you have initialized the voice engine.
         *
         * @return 0: boy's sound; 1: girl's sound; 2: non human sound; -1: error.
         */
        virtual int GetVoiceIdentify()=0;
        
        
        /*************************************************************
         *                  Other APIs
         *************************************************************/
        /**
         * Set the server's address, only needed for games which published in foreign contries, such as Korea, Europe...
         *
         * SetServerInfo method should be called before JoinXxxRoom in RealTime mode
         * or ApplyMessageKey in Messages, Translation and RSTT mode.
         *
         * @param URL: Url of server, you can get the url from gcloud console after you have registered.
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
    public:
		virtual GCloudVoiceErrno SetServerInfo(const char * URL, const char *defaultipsvr = "") = 0;
        
        /**
         * Set the bit rate of the voice code.
         * SetBitRate method should be called after you have initialized the voice engine.
         *
         * @param bitrate: The bit rate you want to set, it should be an integer between 8~256K.
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno SetBitRate(int bitrate)=0;
        
        /**
         * Set if it is datafree.
         *
         * @param enable: Enable datafree if it is true, and disable datafree if it is false.
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno SetDataFree(bool enable)=0;
        
        // TODO
        virtual GCloudVoiceErrno SetLogCallBack(GVoiceLogFunc logFunc)=0;

        /**
         * Open Voice Engine's logcat.
         * EnableLog method should be called after you have initialized the voice engine.
         *
         * @param enable: Open logcat if it is true, and disable logcate if it is false.
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual void EnableLog(bool enable)=0;
        
        /**
         * Set the audience list who can hear, that is, members not in this list can not hear the voice from the members in the same room.
         *
         * @param members: The IDs of the members who can hear the voice.
         * @param count: Number of members to set.
         * @param roomName: The room to set the audience list.
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno SetAudience(int *members, int count, const char *roomName ="" ) = 0;
        
        /**
         * Don't play the member's voice.
         *
         * ForbidMemberVoice method should be called after the member has joined a voice room successfully.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RealTime)-->JoinXxxRoom
         * -->ForbidMemberVoice-->.....-->QuitRoom
         *
         * @param member: The ID of the member who you want to forbid his voice.
         * @param bEnable: Forbid the member's voice if it is true, and listen the member's voice if it is false.
         * @param roomName: The name of The room to forbid member's voice, it should be an exist room name.
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno ForbidMemberVoice(int member, bool bEnable, const char *roomName="")=0;
        
        /**
         * Get the voice message's file size and last time.
         *
         * @param filepath: The path of the voice file to get infomation, the filePath should like:"your_dir/your_file_name".
         * @param bytes: For returning the file's size.
         * @param seconds: For returning the voice's length.
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno GetFileParam(const char *filepath, unsigned int* bytes, float *seconds)=0;
        
        
        /*************************************************************
         *                  Voice report related APIs
         *************************************************************/
        /**
         * Set the buffered voice size in GVoice when use voice report function, the default value is 20s.
         *
         * @param nTimeSec: How many seconds you want GVoice to buffer
         */
        virtual void SetReportBufferTime(unsigned int nTimeSec) = 0;
        
        /**
         * Set the players information that maybe reported in your game.
         *
         * @param cszOpenID: All players openid you may report
         * @param nMemberID: All players memberid you may report
         * @param nCount: The count of members you may report, it should be equals to the length of cszOpenID and nMemberID
         *
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno SetReportedPlayerInfo(const char* cszOpenID[], unsigned int nMemberID[], unsigned int nCount) = 0;
        
        /**
         * Report uncivilized players in your game.
         *
         * @param cszOpenID: All players openid you may report, null will report all the players you set @see SetReportedPlayerInfo
         * @param nCount: Element count in array, 0 will report all the players you set @see SetReportedPlayerInfo
         * @param cszInfo: Information will be send to server
         *
         * @return : an integer result, @enum GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno ReportPlayer(const char* cszOpenID[], unsigned int nCount, const char* cszInfo) = 0;
        
        
        //For LGame Rec Interface
        //for rec
        virtual GCloudVoiceErrno StartSaveVoice() = 0;
        virtual GCloudVoiceErrno StopSaveVoice() = 0;
        virtual GCloudVoiceErrno SetRecSaveTs(int ts) = 0;
        //for play
        virtual GCloudVoiceErrno SetPlayFileIndex(const char* fileid, int fileindex) = 0;
        virtual GCloudVoiceErrno StartPlaySaveVoiceTs(int ts) = 0;
        virtual GCloudVoiceErrno StopPlaySaveVoice() = 0;
        virtual GCloudVoiceErrno DelAllSaveVoiceFile(const char* fileid, int fileindex) = 0; 

		/// @brief Get room members openid-memberid info
		///
		/// @param[in] roomName of which rooms members info you want to get
		/// @param[out] RoomMembers members info saved in here,if with value null, the function return value is room members count num
		/// @param[in] len, the members[] array lens, if len<=0, the function return value is the room members count num
		///
		/// @return an integer result, the room members count num, otherwist -1 means some error
		virtual int GetRoomMembers(const char* roomName, RoomMembers members[], int len) = 0;

		/**
	 	* Enable civilization voice detect.
	 	* 
	 	*
	 	* @param enable: enable detect if it is true, and disable  if it is false.
	 	* @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
	 	* @see GCloudVoiceErrno
	 	*/
		virtual int EnableCivilVoice(bool bEnable)=0;
		
		/// @brief Translate speech from one language to another.
		///
		/// @param[in] fileID, ID of speech file which to be translated.
		/// @param[in] srcLang, speech language associated with fileID.
		/// @param[in] targetLang, target language that we want to translate to.
		/// @param[in] transType, if set SPEECH_TRANSLATE_STST, targetLang will be ignored
		/// @param[in] nTimeoutMS, length of speech translate perform timeout, the unit is milliseconds, recommended >= 10000
		///
		/// @return an integer result, @enum GCloudVoiceErr
		virtual GCloudVoiceErrno SpeechTranslate(const char *fileID, SpeechLanguageType srcLang, SpeechLanguageType targetLang, SpeechTranslateType transType, int nTimeoutMS = 10000) = 0;

		/// @brief Text to speech.
		///
		/// @param[in] text, utf-8, MAX 255 length
		/// @param[in] lang, text's language.
		/// @param[in] voice type, refer to enum STTVoiceType.
		/// @param[in] nTimeoutMS, length of tts perform timeout, the unit is milliseconds, recommended >= 10000
		///
		/// @return an integer result, @enum GCloudVoiceErr
		virtual GCloudVoiceErrno TextToSpeech(const char *text, SpeechLanguageType lang, STTVoiceType voiceType, int nTimeoutMS = 10000) = 0;

		/// @brief Text translate
		///
		/// @param[in] text, utf-8, MAX 2000 length
		/// @param[in] srclang, text's language.
		/// @param[in] targetLang, target language that we want to translate to.
		/// @param[in] nTimeoutMS, length of text-translating perform timeout, the unit is milliseconds, recommended >= 10000
		///
		/// @return an integer result, @enum GCloudVoiceErr
		virtual GCloudVoiceErrno TextTranslate(const char *text, SpeechLanguageType srcLang, SpeechLanguageType targetLang, int nTimeoutMS = 10000) = 0;

		/// @brief Start Text to stream speech,auto play,only support chinese speech,voice type support customization.
		///
		/// @param[in] text, utf-8, MAX 255 length
		/// @param[in] voice type, as below:
		///				db1, General Sweet Girl Voice
		///				female0, General female voice
		///				femalen, emotional female voice
		///				kefu, Customer Service Voice
		///				male0, General male voice
		///				xdgz, emotional male voice
		///				txnews, news female voice
		///				wepay, payment dedicated female voice
		///				xiaowei, assistant female voice
		/// @param[in] nTimeoutMS, length of tts perform timeout, the unit is milliseconds, recommended >= 10000
		///
		/// @return an integer result, @enum GCloudVoiceErr
		virtual GCloudVoiceErrno TextToStreamSpeechStart(const char *text,  const char* voiceType, int nTimeoutMS = 10000) = 0;

		/// @brief Stop Text to stream speech.
		///
		/// @return an integer result, @enum GCloudVoiceErr
		virtual GCloudVoiceErrno TextToStreamSpeechStop() = 0;

		 /**
         * start real-time speach to speach.
         *
         * should be called in RSTS mode
		 * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RSTS)-->ApplyMessageKey
		 * -->RSTSStartRecording
         *
         * @param[in] srcLang, the speech language of the recorder.
		 * @param[in] pTargetLangs, target languages that we want to translate to.
		 * @param[in] nTargetLangCnt, number of target languages that we want to translate to.
		 * @param[in] transType, if set SPEECH_TRANSLATE_STST, pTargetLangs and nTargetLangCnt will be ignored
		 * @param[in] bNotVoip, use or donot use ios's voip mode
		 * @param[in] nTimeoutMS, length of RSTS perform timeout, the unit is milliseconds, recommended >= 3000
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno RSTSStartRecording(SpeechLanguageType srcLang, SpeechLanguageType pTargetLangs[], int nTargetLangCnt, SpeechTranslateType transType, int nTimeoutMS = 5000)=0;
        
		 /**
         * stop real-time speach to speach.
         *
         * should be called in RSTS mode
		 * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RSTS)-->ApplyMessageKey
		 * -->RSTSStartRecording-->RSTSStopRecording
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno RSTSStopRecording()=0;
    public:
        /**
         * It is not recommended to call this method.
         * If you want to use this, please contact with the GVoice team.
         */
        virtual int invoke (unsigned int nCmd, unsigned int nParam1, unsigned int   nParam2, unsigned int *pOutput )=0;
        
    public:
        /// @brief Enable MiniApp plugin for room.
        ///
        /// @param[in] roomName, room to set.
        /// @param[in] enable, true for enable and false reverse.
        ///
        /// @return an integer result, @enum GCloudVoiceErr
        virtual GCloudVoiceErrno EnableWXMiniApp(const char *roomName, bool enable) = 0;
        
        /// @brief Query members from  MiniApp.
        ///
        /// @param[in] roomName, name of room .
        ///
        /// @return an integer result, @enum GCloudVoiceErr
        virtual GCloudVoiceErrno QueryWXMembers(const char *roomName) = 0;
        
        /// @brief Query member's info from MiniApp.
        ///
        /// @param[in] roomName, name of room .
        /// @param[in] memberID: member ID to query.
        ///
        /// @return an integer result, @enum GCloudVoiceErr
        virtual GCloudVoiceErrno QueryUserInfo(const char *roomName, int memberID, const char *openID) = 0;
        
        /// @brief Send info to MiniApp.
        ///
        /// @param[in] roomName, name of room .
        /// @param[in] info, info data for MiniApp.
        ///
        /// @return an integer result, @enum GCloudVoiceErr
        virtual GCloudVoiceErrno UpdateSelfInfo(const char *roomName, const char *info) = 0;

		/*
		* set civil voice source file path
		*param: [in]pPath : bin  absolutive path
		*return: GCloudVoiceErrno
		*/
		virtual GCloudVoiceErrno SetCivilBinPath(const char * pPath) = 0;
		
		/**
		* set room member's volume, used for multi room
		*
		* @param[in] playerid: the room member's playerid who you want to set his volume
		* @param[in] pVol : [in] the member's volume
		* @return : see GCloudVoiceErrno
		*/
		virtual GCloudVoiceErrno SetPlayerVolume(const char* playerid, unsigned int nVol) = 0;
		/**
		* get room member's volume, used for multi room
		*
		* @param[in] playerid: the room member's playerid who you want to get his volume
		* @param pVol : [out] the member's volume, default is 100
		* @return[out] : see GCloudVoiceErrno
		*/
		virtual GCloudVoiceErrno GetPlayerVolume(const char* playerid, unsigned int *pVol) = 0;
		


		/*
		*enable key words detect
		*param:[in]bEnable:true  enable keywords detect,false:disable
		*return:GCloudVoiceErrno
		*/
		
		virtual GCloudVoiceErrno EnableKeyWordsDetect(bool bEnable) = 0;

		/**
		 * Enable or Disable real-time voice translate. for example, an American, a Chinese, and a German enter the same room and set their own language,
		 *the background service will translate Chinese speech into English speech and German speech, and send to corresponding person , and so do others .
		 *
		 * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RealTime)-->JoinTeamRoom
		 * -->EnableTranslate-->.....-->QuitRoom
		 *
		 * The result of EnableTranslate successful or not can be obtained by the callback method OnEnableTranslate.
		 * @see OnEnableTranslate
		 *
		 * @param roomName: The name of The room to join, it should be a string composed by 0-9A-Za-Z._- and less than 127 bytes.
		 * @param bEnable: true for enable translate, false for disable translate.
		 * @param myLang: speaker language, refer to SpeechLanguageType.
		 * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
		 * @see GCloudVoiceErrno
		 */
		virtual GCloudVoiceErrno EnableTranslate(const char *roomName, bool bEnable, SpeechLanguageType myLang, RealTimeTranslateType transType) = 0;
        
        /**
         * Enable or disable MagicVoice function in realtime voice room.
         * If a player enabled the MagicVoice function in a game room, then the play's voice will be translated to
         * another voice, which is specified by the magicType parameter, and send to other players in the same room.
         *
         * EnableMagicVoice method should be called at anytime after JoinXxxRoom success.
         * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RealTime)-->JoinTeamRoom(RangeRoom/NationalRoom)
         * -->EnableMagicVoice-->.....-->QuitRoom
         *
         * EnableMagicVoice is an Asynchronous interface, the result of EnableMagicVoice should be obtained by the callback method OnEnableMagicVoice.
         * @see OnEnableMagicVoice
         *
         * @param magicType: MagicVoice type the player want to translate to
         * @param enable: true for enable MagicVoice function, and false for disable MagicVoice function
         * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
         * @see GCloudVoiceErrno
         */
        virtual GCloudVoiceErrno EnableMagicVoice(const char* magicType, bool enable) = 0;
        
        /**
        * Enable or disable receive MagicVoice from other players in the same realtime voice room.
        * If a player A enabled the RecvMagicVoice function in a game room, and anyone B in this room enabled MagicVoice function, then the play A will receive magic voice of B.
        * In default, a player A will receive magic voice from B, if B enabled MagicVoice function.
        *
        * EnableRecvMagicVoice method should be called at anytime after JoinXxxRoom success.
        * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->SetMode(RealTime)-->JoinTeamRoom(RangeRoom/NationalRoom)
        * -->EnableRecvMagicVoice-->.....-->QuitRoom
        *
        * EnableRecvMagicVoice is an Asynchronous interface, the result of EnableRecvMagicVoice should be obtained by the callback method OnEnableRecvMagicVoice.
        * @see OnEnableRecvMagicVoice
        *
        * @param enable: true for enable RecvMagicVoice function, and false for disable RecvMagicVoice function
        * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
        * @see GCloudVoiceErrno
        */
        virtual GCloudVoiceErrno EnableRecvMagicVoice(bool enable) = 0;

		virtual bool IsPause() = 0;

		////////begin just for pc use to compatible old pc branch api/////////

		/**
		* get is the room member's is be mute
		*
		* @param memid: the room member's memberid
		* @return : if the membervoice be forbid,return true(means can't hear this memberid voice), else return false
		*/
		virtual bool IsMemberBeForbidVoice(unsigned int memid) = 0;

		/**
		* get user micvolume
		*
		* @param pVol : [out] the suer micvolume value
		* @return : see GCloudVoiceErrno
		*/
		virtual GCloudVoiceErrno GetMicVolume(unsigned int *pVol) = 0;
		/**
		* get user speaker volume
		*
		* @param pVol : [out] the suer speaker volume value
		* @return : see GCloudVoiceErrno
		*/
		virtual GCloudVoiceErrno GetSpeakerVolume(unsigned int *pVol) = 0;

		/**
		* is mic open or close now
		* @return : true:mic have open, false: mic is closed
		*/
		virtual bool IsMicEnabled() = 0;
		/**
		* is speaker open or close now
		* @return : true:speaker have open, false: speaker is closed
		*/
		virtual bool IsSpeakerEnabled() = 0;

		virtual void DestroyVoiceEngine() = 0;

		////////end just for pc use to compatible old pc branch api/////////
		
		/**
		* Room data channel, business can transfer private data.
		*
		* @param roomName, the room which you want to transfer private data.
		* @param content: the content of the room transmission, JSON format, utf-8, limit 1024 bytes
		* @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
		* @see GCloudVoiceErrno
		*/
		virtual GCloudVoiceErrno RoomGeneralDataChannel(const char* roomName, const char* content) = 0;

		/**
		* Report API call information for problem analysis.
		*
		* @param api, API that you want to trace.
		* @param callInfo: descriptive information of the API call, for example, in which scenario the API is called
		* @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
		* @see GCloudVoiceErrno
		*/
		virtual GCloudVoiceErrno APITrace(const char* api, const char* callInfo) = 0;
		
		virtual int SetPlayerInfoAbroad(const char *szopenID[],int nMemberID[],SpeechLanguageType pLang[],unsigned int nCount) = 0;
		
        /**
        * Set a specific magic voice type of the magic voice message function.
        *
        * @param magicType: the specific magic voice you want to recored
        *
        * @return If success returns 0, otherwise returns -1.
        */
		virtual int SetMagicVoiceMsgType(const char *magicType) = 0;

	/*funciton:audition file for specify magic type
	*@param :[in] file  :which file need play used specify magic type
	*		[in] magictype:specify magic type for play
	*
	*/
		virtual int AuditionFileForMagicType(const char *file,const char *magicType) = 0;
	
		/**
		* function:Set Transparent info for report judge. 
		*
		* @return.
		*/
		virtual void SetTransSecInfo(const char* secInfo) = 0;
		
		/*
			reprot all player
		*/
		virtual int EnableReportALL(bool bEnable) = 0;

		
		/*
			reprot all player for abroad
		*/
		virtual int EnableReportALLAbroad(bool bEnable) = 0;


			/**
	 	* Enable civilization voice detect for file.
	 	* 
	 	*
	 	* @param enable: enable detect if it is true, and disable  if it is false.
	 	* @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
	 	* @see GCloudVoiceErrno
	 	*/
		virtual int EnableCivilFile(bool bEnable)=0;


		/////////////////////////3D Audio API///////////////////////////
		/**
		* Enable 3d voice
		*
		* @param enable: enable detect if it is true, and disable  if it is false.
		* @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
		* @see GCloudVoiceErrno
		*/
		virtual GCloudVoiceErrno Enable3DVoice(bool bEnable) = 0;
		/**
		* Set Listener(self) 3d position vector
		*
		* @param GVoice3DVector: x/y/z.
		* @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
		* @see GCloudVoiceErrno
		*/
		virtual GCloudVoiceErrno Set3DPosition(GVoice3DVector pos) = 0;
		/**
		* Set Listener(self) face 3d position vector,default(0,1,0)
		*
		* @param GVoice3DVector: x/y/z.
		* @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
		* @see GCloudVoiceErrno
		*/
		virtual GCloudVoiceErrno Set3DForward(GVoice3DVector forward) = 0;
		/**
		* Set Listener(self) head 3d position vector,default(0,0,1)
		*
		* @param GVoice3DVector: x/y/z.
		* @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
		* @see GCloudVoiceErrno
		*/
		virtual GCloudVoiceErrno Set3DUpward(GVoice3DVector upward) = 0;
		/**
		* Set Distance-vol properties
		*
		* @param GVoice3DDistProperties: check the GVoice3DDistProperties define for more param detail
		* @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
		* @see GCloudVoiceErrno
		*/
		virtual GCloudVoiceErrno Set3DDistProperties(GVoice3DDistProperties g3dProperties) = 0;

		/////////////////////////End 3D Audio API///////////////////////////

		virtual int IsSaveMagicFile(bool bSave) = 0;

};
} // end of gcloud_voice
#endif /* GCloudVoiceExtension_h */
