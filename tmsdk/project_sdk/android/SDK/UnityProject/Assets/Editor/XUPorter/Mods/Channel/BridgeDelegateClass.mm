

#import "BridgeDelegateClass.h"
#import "UnityInterface.h"
#import "BridgeUtil.h"


//BridgeUtil *Util = [[BridgeUtil alloc] init];

@implementation BridgeDelete
NSString* bundleID = @"com.aebyh.ad";


- (void)MGplatformInitFinished:(NSNotification*)notification
{
    NSLog(@"InitFinished!!!!");
    self.isSDKInited = true;
    NSLog(@"555 MG aTypeD appid %@ !!!!!!!!!!!!!!", [[XYPlatform defaultPlatform]XY_APP_ID]);
    
    bundleID = [[NSBundle mainBundle]bundleIdentifier];
    NSLog(@"%@", [NSString stringWithFormat:@"bundle id is %@",bundleID]);
    //_InitIAP(self);
    for(NSString *s in self.IapArray)
    {
        NSLog(@" iosIap = %@",s);
    }
    [[XYPlatform defaultPlatform] XYIAPStartRequestProductsArray:self.IapArray WithDelegate:self];
}

- (void)loginCallBack:(NSNotification*)notification
{
    NSLog(@"666 MG aTypeD appid %@ !!!!!!!!!!!!!!", [[XYPlatform defaultPlatform]XY_APP_ID]);
    NSLog(@"loginCallBack!!!!");
    NSDictionary *userInfo = notification.userInfo;
    
    if ([userInfo[kXYPlatformErrorMsg] intValue]  == XY_PLATFORM_NO_ERROR)
    {
        //NSInteger isBang = [userInfo[@"isbang"] integerValue];
        
        
        NSString *token = [[XYPlatform defaultPlatform] XYToken];
        NSString *openuid = [[XYPlatform defaultPlatform] XYOpenUID];
        //NSLog(@"login!!!! token:%@, openuid:%@", token, openuid);
        
        /*
        if (loginCallback != NULL)
        {
            loginCallback([token UTF8String], [openuid UTF8String]);
        }
        */
        
        NSString *str = [NSString stringWithFormat:@"%@,%@", openuid, token];
        
        [BridgeUtil UnitySendMsg:SDKCALLBACK_LOGIN : [str UTF8String]];
    }
}

- (void)logoutCallBack:(NSNotification*)notification
{
    NSLog(@"777 MG aTypeD appid %@ !!!!!!!!!!!!!!", [[XYPlatform defaultPlatform]XY_APP_ID]);
    NSLog(@"logoutCallBack");
    NSDictionary *userInfo = notification.userInfo;
    if ([userInfo[kXYPlatformErrorKey] intValue]  == XY_PLATFORM_NO_ERROR)
    {
    	/*
        if (logoutCallback != NULL)
        {
            logoutCallback();
        }
        */
        [BridgeUtil UnitySendMsg:SDKCALLBACK_LOGOUT : ""];
    }
}

-(void)leavedPlatform:(NSNotification*)notification
{
    NSLog(@"888 MG aTypeD appid %@ !!!!!!!!!!!!!!", [[XYPlatform defaultPlatform]XY_APP_ID]);
    NSNumber* leavedType = (NSNumber*)notification.object;
    
    switch ([leavedType integerValue]) {
        case XYPlatformLeavedDefault: {
            //NSLog(@"用户离开平台－－> MGPlatformLeavedDefault");
            break;
        }
        case XYPlatformLeavedFromLogin: {
            //NSLog(@"用户离开平台－－> MGPlatformLeavedFromLogin");
            
            [[XYPlatform defaultPlatform] XYIsLogined:^(BOOL isLogined) {
                if (!isLogined) {
                    [[XYPlatform defaultPlatform] XYAutoLogin:0];
                }
            }];
            
            break;
        }
        case XYPlatformLeavedFromRegister: {
            //NSLog(@"用户离开平台－－> MGPlatformLeavedFromRegister");
            
            [[XYPlatform defaultPlatform] XYIsLogined:^(BOOL isLogined) {
                if (!isLogined) {
                    [[XYPlatform defaultPlatform] XYAutoLogin:0];
                }
            }];
            
            break;
        }
        case XYPlatformLeavedFromcoshow: {
            //NSLog(@"用户离开平台－－> MGPlatformLeavedFromcoshow");
            break;
        }
        case XYPlatformLeavedFromSNSCenter:{
            //NSLog(@"用户离开平台－－> MGPlatformLeavedFromSNSCenter");
            break;
        }
            
        default:
            //NSLog(@"用户离开平台－－> NULL");
            break;
    }

}

-(void)MGbindphoneNumSucc:(NSNotification*)notification
{
    if(notification == NULL)
    {
        return;
    }
    NSDictionary *userInfo = notification.userInfo;
    if(userInfo!=NULL)
    {
        NSString *bindphoneNum = userInfo[BINDPHONE_NOTIFICATION_KEY];
        if(bindphoneNum == nil || bindphoneNum == NULL)
        {
            NSLog(@"bind phone num is nil or NULL");
            return;
        }
        if([bindphoneNum isEqualToString:@"0"])
        {
            NSLog(@"bind phone num is 0");
            return;
        }
        const char* phoneNum = [bindphoneNum UTF8String];
        [BridgeUtil UnitySendMsg:SDKCALLBACK_LOGIN : phoneNum];
        //NSLog(@"result ： bind phone num is %s",phoneNum);
    }
}

- (void)MGsmallgameLoad:(NSNotification*)notification
{
    if(notification == NULL)
    {
        return;
    }
    NSLog(@"Callback - MGsmallgameLoad");
    [BridgeUtil UnitySendMsg:SDKCALLBACK_ONLOADSMALLGAME : ""];
}

/*void _InitIAP(BridgeDelete *bdClass)
{
    //询问哪些商品能够购买 //设置代理
    [[XYPlatform defaultPlatform] XYIAPStartRequestProductsArray:@[[NSString stringWithFormat:@"%@.6",bundleID],
                                                                   [NSString stringWithFormat:@"%@.30",bundleID],
                                                                   [NSString stringWithFormat:@"%@.60",bundleID],
                                                                   [NSString stringWithFormat:@"%@.98",bundleID],
                                                                   [NSString stringWithFormat:@"%@.198",bundleID],
                                                                   [NSString stringWithFormat:@"%@.328",bundleID],
                                                                   [NSString stringWithFormat:@"%@.488",bundleID],
                                                                   [NSString stringWithFormat:@"%@.648",bundleID],
                                                                   [NSString stringWithFormat:@"%@.M30",bundleID],
                                                                   [NSString stringWithFormat:@"%@.LB1",bundleID],
                                                                   [NSString stringWithFormat:@"%@.LB3",bundleID],
                                                                   [NSString stringWithFormat:@"%@.LB6",bundleID]] WithDelegate:bdClass]; 
} */

#pragma mark-- 充值回调 PayDelegate

//IAP工具已获得可购买的商品
-(void)IAPToolGotProducts:(NSMutableArray *)products {
//    NSLog(@"GotProducts:%@",products);
    
    //_productArray = products;
    
//        for (MGIAPModel *product in products){
//            NSLog(@"localizedDescription:%@\nlocalizedTitle:%@\nprice:%@\nproductID:%@",
//                  product.localizedDescription,
//                  product.localizedTitle,
//                  product.price,
//                  product.productIdentifier);
//            NSLog(@"--------------------------");
//        }

//    NSLog(@"成功获取到可购买的商品");
    
    self.productArray = products;
}
//支付失败/取消
-(void)IAPToolCanceldWithProductID:(NSString *)productID {
    //NSLog(@"canceld:%@",productID);
    [BridgeUtil UnitySendMsg:SDKCALLBACK_ONPAY : "2"];
    //[BridgeUtil UnitySendMsg:SDKCALLBACK_ONPAY :"2"];
}
//支付成功了，并开始向苹果服务器进行验证（若CheckAfterPay为NO，则不会经过此步骤）
-(void)IAPToolBeginCheckingdWithProductID:(NSString *)productID {
    //NSLog(@"BeginChecking:%@",productID);
}
//商品被重复验证了
-(void)IAPToolCheckRedundantWithProductID:(NSString *)productID {
    //NSLog(@"CheckRedundant:%@",productID);
    //NSLog(@"重复验证了");
    
}
//商品完全购买成功且验证成功了。（若CheckAfterPay为NO，则会在购买成功后直接触发此方法）
-(void)IAPToolBoughtProductSuccessedWithProductID:(NSString *)productID
                                          andInfo:(NSDictionary *)infoDic {
    //NSLog(@"BoughtSuccessed:%@",productID);
    //NSLog(@"successedInfo:%@",infoDic);
    [BridgeUtil UnitySendMsg:SDKCALLBACK_ONPAY : "0"];
}
//商品购买成功了，但向苹果服务器验证失败了
//2种可能：
//1，设备越狱了，使用了插件，在虚假购买。
//2，验证的时候网络突然中断了。（一般极少出现，因为购买的时候是需要网络的）
-(void)IAPToolCheckFailedWithProductID:(NSString *)productID
                               andInfo:(NSData *)infoData {
    //NSLog(@"CheckFailed:%@",productID);
    //NSLog(@"验证失败了");
    [BridgeUtil UnitySendMsg:SDKCALLBACK_ONPAY : "1"];
}
//恢复了已购买的商品（仅限永久有效商品）
-(void)IAPToolRestoredProductID:(NSString *)productID {
    //NSLog(@"Restored:%@",productID);
}
//内购系统错误了
-(void)IAPToolSysWrong {
    //NSLog(@"SysWrong");
    
    //NSLog(@"内购系统出错");
}

@end
