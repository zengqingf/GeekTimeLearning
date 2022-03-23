
#import "BridgeUtil.h"
#import "UnityInterface.h"

@implementation BridgeUtil


+(void) UnitySendMsg:(const char *)methodName :(const char *)methodParam{
    @try {
        NSLog(@"UnitySendMsg methodName :%s methodParam %s" ,methodName,methodParam);
        UnitySendMessage(SDKCALLBACK_GAMEOBJECT_NAME,methodName ,methodParam);
    } @catch (NSException *exception) {
        NSLog(@"UnitySendMsgFailed :%@ ",exception);
    }
}

+(NSDictionary *) JsonParse :(const char *) jsonString
{
    @try {
        NSError * error = nil;
        // 将json字符串转换成字典
        NSString *jsonInfoString = [NSString stringWithUTF8String:jsonString];
        NSLog(@"jsonInfoString : %@",jsonInfoString);
        NSData * getJsonData = [jsonInfoString dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary * dir = [NSJSONSerialization JSONObjectWithData:getJsonData options:NSJSONReadingMutableContainers error:&error];
        return dir;
    } @catch (NSException *exception) {
        NSLog(@"JsonParse Faile %@",exception);
    }
}
@end
