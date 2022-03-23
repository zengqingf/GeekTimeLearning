//
//  BridgeDelegateClass.m
//  PlatformDemo
//
//  Created by Yu Lekai on 03/11/2016.
//  Copyright © 2016 Eason. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <HZPlatform/XYPlatform.h>
#import <HZPlatform/DDSDKApplicationDelegate.h>

#define BINDPHONE_NOTIFICATION_KEY @"kMGbindphoneSucc"

//const char * Iapchars = nullptr;

@interface BridgeDelete : NSObject <XYIAPDelegate>

@property (nonatomic,strong) XYIAPModel *productModel;
@property (nonatomic,strong) NSMutableArray *productArray;
@property (nonatomic,assign) BOOL isSDKInited;
@property  (nonatomic)NSArray *IapArray;

- (void)MGplatformInitFinished:(NSNotification*)notification;
- (void)loginCallBack:(NSNotification*)notification;
- (void)logoutCallBack:(NSNotification*)notification;
- (void)leavedPlatform:(NSNotification*)notification;
- (void)MGbindphoneNumSucc:(NSNotification*)notification;
- (void)MGsmallgameLoad:(NSNotification*)notification;
@end
