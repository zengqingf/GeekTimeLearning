#ifndef GCloudInnerDefine_h
#define GCloudInnerDefine_h

#define JNI_CLASS_GSDK_GCLOUD_GCLOUD "com/tencent/gcloud/GCloud"
#define JNI_CLASS_GSDK_GCLOUD_QR_QRCODEAPI "com/tencent/gcloud/qr/QRCodeAPI"
#define JNI_CLASS_GSDK_GCLOUD_DOLPHIN_CUIIPSMOBILE "com/tencent/gcloud/dolphin/CuIIPSMobile"
#define JNI_CLASS_GSDK_GCLOUD_APKCHANNEL_APKCHANNELUTIL "com/tencent/gcloud/apkchannel/ApkChannelUtil"
#define MAC_GSDK_GCLOUD_TDM_BUNDLE_ID "com.tencent.TDataMaster"
namespace GCloud {
    namespace Conn
    {
        enum
        {
            kChannelNone = 0,
            kChannelTWChat = 1,
            kChannelTQChat = 2,
            kChannelWechat = kChannelTWChat,
            kChannelQQ = kChannelTQChat,
            kChannelGuest = 3,
            kChannelFacebook,
            kChannelGameCenter,
            kChannelGooglePlay,
        };
        typedef int ChannelType;

        enum EncryptMethod {
            kEncryptNone = 0,
            kEncryptTea  = 1,
            kEncryptTQCHAT   = 2,
            kEncryptQQ   = kEncryptTQCHAT,
            kEncryptAes  = 3,
            kEncryptAes2 = 4,
        };
    }
    enum
    {
        kChannelNone = 0,
        kChannelTWChat = 1,
        kChannelTQChat = 2,
        kChannelWechat = kChannelTWChat,
        kChannelQQ = kChannelTQChat,
        kChannelGuest = 3,
        kChannelFacebook,
        kChannelGameCenter,
        kChannelGooglePlay,
    };
    typedef int ChannelType;
}
#endif
