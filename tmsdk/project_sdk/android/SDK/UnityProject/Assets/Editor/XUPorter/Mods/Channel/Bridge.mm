//
//
//  Created by Yu Lekai on 03/11/2016.
//  Copyright © 2016 Eason. All rights reserved.
//

#include "Bridge.h"

#import <Foundation/Foundation.h>
#import <HZPlatform/XYPlatform.h>
#import "BridgeDelegateClass.h"
#import <HZPlatform/DDSDKApplicationDelegate.h>
#import "BridgeUtil.h"

#define APPLE_ID @"1437263159"              //！！！！！！！！！！！！！！！！！！！！！！！！！注意注意！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！

#define STR_FORMAT_RATING_LINK  @"itms-apps://itunes.apple.com/app/id%@?mt=8"

BridgeDelete *classBridge = NULL;
static NSString* const kMGbindphoneSuccNotification = @"com.aebyh.ad.kMGbindphoneSuccNotification";
static NSString* const kMGsmallgameloadNotification = @"com.aebyh.ad.kMGsmallgameloadNotification";
bool ifLoginSmallGame = false;

void _Init(const char *apkInfoString , const char *iOSInfoExtra, bool debug)
{
@try {
    NSDictionary *iosDir = [BridgeUtil JsonParse:apkInfoString];
    NSDictionary *iosExDir = [BridgeUtil JsonParse:iOSInfoExtra];
    NSString *APP_ID = [iosDir objectForKey:@"appId"];
    NSString *APP_KEY = [iosDir objectForKey:@"appKey"];
    NSLog(@"appId %@",APP_ID);
    NSArray *iapArray = [iosExDir objectForKey:@"iosIap"];
    BOOL checkUpdate = [[iosExDir objectForKey:@"checkUpdate"] boolValue];
    BOOL isLoadU3dSmallGame = [[iosExDir objectForKey:@"isLoadU3dSmallGame"] boolValue];
    
    NSLog(@"checkUpdata %@ ",checkUpdate?@"YES":@"NO");
    NSLog(@"debug %@ ",debug?@"YES":@"NO");
    NSLog(@"isLoadU3dSmallGame %@ ",isLoadU3dSmallGame?@"YES":@"NO");
    NSLog(@"appName is : %@",[[NSBundle mainBundle] objectForInfoDictionaryKey:@"CFBundleDisplayName"]);//  Get  Game AppName
    
    
    classBridge = [[BridgeDelete alloc] init];
    classBridge.IapArray = iapArray;
    if (classBridge != nullptr)
    {
        
        [[NSNotificationCenter defaultCenter] addObserver:classBridge
                                                 selector:@selector(MGplatformInitFinished:)
                                                     name:kXYPlatformInitDidFinishedNotification
                                                   object:nil];
        
        [[NSNotificationCenter defaultCenter] addObserver:classBridge
                                                 selector:@selector(loginCallBack:)
                                                     name:kXYPlatformLoginNotification
                                                   object:nil];
        
        [[NSNotificationCenter defaultCenter] addObserver:classBridge
                                                 selector:@selector(logoutCallBack:)
                                                     name:kXYPlatformLogoutNotification
                                                   object:nil];
        
        /*离开平台通知*/
        [[NSNotificationCenter defaultCenter] addObserver:classBridge
                                                 selector:@selector(leavedPlatform:)
                                                     name:kXYPlatformLeavedNotification
                                                   object:nil];
        [[NSNotificationCenter defaultCenter] addObserver:classBridge
                                                 selector:@selector(MGbindphoneNumSucc:)
                                                     name:kMGbindphoneSuccNotification
                                                   object:nil];
        [[NSNotificationCenter defaultCenter] addObserver:classBridge
                                                 selector:@selector(MGsmallgameLoad:)
                                                     name:kMGsmallgameloadNotification
                                                   object:nil];
    }
    

    [[XYPlatform defaultPlatform]MGChoose:YES Completion:^(aType type, NSString *appid) {
        if (type == aTypeC) {

            ifLoginSmallGame = true;
            
            if(isLoadU3dSmallGame)
            {
                [[NSNotificationCenter defaultCenter] postNotificationName:kMGsmallgameloadNotification
                                                                    object:nil];
            }
            
            if(debug)
            {
                NSLog(@"MG aTypeC !!!!!!!!!!!!!!");
            }
            
        }else if (type == aTypeD) {

            NSLog(@"111 MG aTypeD appid %@ !!!!!!!!!!!!!!", [[XYPlatform defaultPlatform]XY_APP_ID]);
            [[XYPlatform defaultPlatform] initializeWithAppId:appid isContinueWhenCheckUpdateFailed:checkUpdate];
            if(debug)
            {
                NSLog(@"222 MG aTypeD appid %@ !!!!!!!!!!!!!!", [[XYPlatform defaultPlatform]XY_APP_ID]);
                NSLog(@"MG aTypeD !!!!!!!!!!!!!!");
            }
            
        }else {
            
            [[XYPlatform defaultPlatform]loadAgain];
            if(debug)
            {
                NSLog(@"MG Type Other, LoadAgain !!!!!!!!!!!!!!");
            }
        }
    }];
    
    [[XYPlatform defaultPlatform] XYSetScreenOrientation:UIInterfaceOrientationLandscapeLeft];
    
    [[XYPlatform defaultPlatform] XYSetDebugModel:debug];
    [[XYPlatform defaultPlatform] XYSetShowSDKLog:debug];
    
    [[UIApplication sharedApplication] setStatusBarHidden:YES withAnimation:UIStatusBarAnimationNone];
    //NSLog(@"!![[UIApplication sharedApplication] setStatusBarHidden:YES withAnimation:UIStatusBarAnimationNone]!!");
} @catch (NSException *exception) {
    NSLog(@"initzy failed %@" , exception);
}
}

void _OpenLogin()
{
    NSLog(@"333 MG aTypeD appid %@ !!!!!!!!!!!!!!", [[XYPlatform defaultPlatform]XY_APP_ID]);
    [[XYPlatform defaultPlatform] XYAutoLogin:0];
    NSLog(@"444 MG aTypeD appid %@ !!!!!!!!!!!!!!", [[XYPlatform defaultPlatform]XY_APP_ID]);
}

void _Pay(const char *payInfoString , const char* userIofoString){
    
    
    @try {

        NSDictionary *payDir = [BridgeUtil JsonParse:payInfoString];
    //用户登录状态验证
    [[XYPlatform defaultPlatform] XYIsLogined:^(BOOL isLogined)
     {
         if(!isLogined)
         {
             [[XYPlatform defaultPlatform] XYAutoLogin:0];
             return;
         }
     }];
    
    
    NSString *ext = [payDir objectForKey:@"extra"];
    //NSString *productId = [payDir objectForKey:@"productIdandBundleId"];
    NSString *productId = @"com.ttt.jinxiao.6";
    NSString *price = [payDir objectForKey:@"price"];
    NSNumber *serverIdnum = [payDir objectForKey:@"serverId"];
    NSString *serverId = [NSString stringWithFormat:@"%@",serverIdnum];
    NSString *productName = [payDir objectForKey:@"productName"];
    NSString *productDesc = [payDir objectForKey:@"productDesc"];
    NSString *appName = [payDir objectForKey:@"appName"];
    NSString *userName = [payDir objectForKey:@"roleName"];
    NSString *payCallbackUrl = [payDir objectForKey:@"payCallbackUrl"];
        
        
        

    
    NSLog(@"extra = %@",[payDir objectForKey:@"extra"]);
        
        
    NSArray *nsArray = [ext componentsSeparatedByString:@","];
    if([nsArray count] != 3)
    {
        NSLog(@"extra extra extra !!! %lu",(unsigned long)[nsArray count]);
        return;
    }
    
    //MGIAPModel *prodModel = classBridge.productModel;
    
    //if (prodModel == NULL)
    //{
    //    prodModel = [[MGIAPModel alloc]init];
    //    classBridge.productModel = prodModel;
    //}
    
    //if(prodModel == NULL)
    //{
    //    return;
    //}
    
    XYIAPModel *prodModel = NULL;
    if(classBridge == NULL)
    {
        return;
    }
    if(classBridge.productArray != NULL)
    {
        for (XYIAPModel *product in classBridge.productArray)
        {
            if ([product.productIdentifier isEqualToString:productId])
            {
                prodModel = product;
                //NSLog(@"!!! 111 productModel : price is %@ , real price is %@ , id is %@ , title is %@ , desc is %@",prodModel.price, prodModel.real_price,prodModel.productIdentifier,prodModel.localizedTitle,prodModel.localizedDescription);
                break;
            }
        }
    }
    if(prodModel == NULL)
    {
        prodModel = [[XYIAPModel alloc]init];
    }
    
    if(prodModel == NULL)
    {
        return;
    }
    
    prodModel.price = price;
    prodModel.productIdentifier = productId;
    prodModel.localizedTitle = productName;
    prodModel.localizedDescription =productDesc;
    if(prodModel.real_price == NULL || prodModel.real_price == nil ||
       prodModel.real_price.length == 0 || [prodModel.real_price isKindOfClass:[NSNull class]])
    {
        //NSLog(@"product model real price is null !!!");
        prodModel.real_price = price;
    }
    
    XYIAPBuyModel *buyModel = [[XYIAPBuyModel alloc]init];
    buyModel.amount = prodModel.price;
    buyModel.real_price = prodModel.real_price;
    buyModel.appName = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"CFBundleDisplayName"];
    buyModel.appOrderID =  ext;
    buyModel.appUserID = [[XYPlatform defaultPlatform]XYOpenUID];//nsArray[2];
    buyModel.appUserName = userName;
    buyModel.SID = serverId;
    buyModel.openUID = [[XYPlatform defaultPlatform]XYOpenUID];
    buyModel.productId = prodModel.productIdentifier;
    buyModel.productName =  prodModel.localizedTitle;
    buyModel.packageName = [[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleIdentifier"];
    buyModel.deviceType = @"iOS";
    buyModel.appkey = [[XYPlatform defaultPlatform]XY_APP_ID];
    buyModel.callback_url = payCallbackUrl;//@"http://101.37.173.236:59268/zy_charge?";
    buyModel.app_extra1 = @"extra1";
    buyModel.app_extra2 = @"extra2";
    
    //NSLog(@"[Platform defaultPlatform] BuyProduct !!! price:%@ appName:%@ appOrderID:%@ appUserId:%@ appUserName:%@ SID:%@ openUID:%@ productID:%@ productName:%@ packageName:%@ deviceType:%@ appKey:%@ cbURl:%@",buyModel.amount,buyModel.appName,buyModel.appOrderID,buyModel.appUserID,buyModel.appUserName,buyModel.SID,buyModel.openUID,buyModel.productId,buyModel.productName,buyModel.packageName,buyModel.deviceType,buyModel.appkey,buyModel.callback_url);
    
    //[[MGManager defaultManager] MGBuyProduct:prodModel withBuyModel:buyModel];
    [[XYPlatform defaultPlatform] XYBuyProductWithModel:buyModel iap:false];
    } @catch (NSException *exception) {
        NSLog(@"pay  failed %@",exception);
    }
}

void _OpenMobileBind()
{
@try {
    [[XYPlatform defaultPlatform]XYEnterBindPhoneCompletion:^(NSString *phoneNum, BOOL status) {
        if(status == YES)
        {
            [[NSNotificationCenter defaultCenter] postNotificationName:kMGbindphoneSuccNotification
                                                            object:nil
                                                          userInfo:@{BINDPHONE_NOTIFICATION_KEY:phoneNum}];
        }
        //NSLog(@"_OpenMobileBind status : %d , phone num : %@",status,phoneNum);
    }];
    } @catch (NSException *exception) {
       NSLog(@"_OpenMobileBind Failed : %@",exception);
    }
}

void _CheckIsBindPhoneNum()
{
    [[XYPlatform defaultPlatform]XYPhoneStatus:^(NSString *phoneNum, BOOL status) {
        if(status == YES)
        {
            [[NSNotificationCenter defaultCenter] postNotificationName:kMGbindphoneSuccNotification
                                                                object:nil
                                                              userInfo:@{BINDPHONE_NOTIFICATION_KEY:phoneNum}];
        }
        //NSLog(@"_CheckIsBindPhoneNum status : %d , phone num : %@",status,phoneNum);
    }];
}

//角色信息上报
void _ReportRoleInfo(const char *roleInfo , int param) //1:login;2:createrole;3:levelup;4:logout
   {
       try {
           NSDictionary *RoleInfoDir = [BridgeUtil JsonParse:roleInfo];
           NSString *mRoleId = [RoleInfoDir objectForKey:@ " roleId"];
           NSNumber *serverId = [RoleInfoDir objectForKey:@"serverId"];
           NSString *mServerId = [NSString stringWithFormat:@"%@",serverId];
           NSString *name = [RoleInfoDir objectForKey:@ " roleName"];
           NSString *mLevel = [RoleInfoDir objectForKey:@ " roleLevel"];
           NSString *mJobName = [RoleInfoDir objectForKey:@ " jobName"];
           if(param == 2)
           {
               [[XYPlatform defaultPlatform] XYCreateRole: mRoleId roleName:name gameServer:mServerId ];
           }
           else if(param == 1)
           {
               MGGameEventDataModel *model = [[MGGameEventDataModel alloc]init];
               model.data_type = @"3"; //统计事件类型
               model.role_id = mRoleId;
               model.role_name = name;
               model.server_id = mServerId;
               model.level = mLevel;
               model.job = mJobName;
               [[XYPlatform defaultPlatform]gameEventData:model];
           }
       } catch (NSException * exception) {
           NSLog(@"_UpdateRolrInfo Failed : %@",exception);
       }
   }

void _SetRoleLoginGame()
{
    //[[XYPlatform defaultPlatform] XYInGame];
    //[[XYPlatform defaultPlatform]XYSwitchAccount];
}
void _SetRoleLogoutGame()
{
    //[[XYPlatform defaultPlatform] XYOutGame];
    //[[XYPlatform defaultPlatform]XYGameOut];
}

void _GetNewVersionInAppstore()
{
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString:[NSString stringWithFormat:STR_FORMAT_RATING_LINK, APPLE_ID]]];
}

bool _IsLoginSmallGame()
{
    return ifLoginSmallGame;
}

bool _IsSDKInited()
{
    if(classBridge == nil)
    {
        return false;
    }
    return classBridge.isSDKInited;
}


