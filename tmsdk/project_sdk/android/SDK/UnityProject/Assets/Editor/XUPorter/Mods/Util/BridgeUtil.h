

#define SDKCALLBACK_GAMEOBJECT_NAME   "Singleton of SDKClient.SDKCallback"
#define SDKCALLBACK_LOGIN  "OnLogin"
#define SDKCALLBACK_LOGOUT  "OnLogout"
#define SDKCALLBACK_ONPAY  "OnPayResult"//0-成功 1-失败 2-取消
#define SDKCALLBACK_ONBINDPHONESUCC  "OnBindPhoneSucc"//手机绑定成功
#define SDKCALLBACK_ONLOADSMALLGAME  "OnLoadSmallGame"//加载小游戏


@interface BridgeUtil : NSObject{
}
//@property(nonatomic,retain)  NSString*  SDKCALLBACK_LOGIN;


+(void) UnitySendMsg: (const char *) methodName : (const char *) methodParam;
+(NSDictionary *) JsonParse :(const char *) jsonString;

@end
