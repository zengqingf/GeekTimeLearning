//
//  LBInit.h
//  LBSDK
//
//  Created by xunjiangtao on 2018/6/6.
//  Copyright © 2018年 lebian. All rights reserved.
//

#import <Foundation/Foundation.h>

typedef void(^QueryUpdateCallback)(int tag);

@interface LBInit : NSObject

@property (nonatomic, copy) QueryUpdateCallback cb;

/**
 实例
 
 @return 返回实例
 */
+ (instancetype)sharedInstance;

/**
 SDK启动入口
 
 @param launchOptions 传入APP启动参数
 @return 返回为YES或者NO。
 */
- (BOOL)LBSDKShouldInitWithLaunchOptions:(NSDictionary *)launchOptions;

#pragma mark - 一些可能需要用到的接口
/**
 热更检查更新接口，带回调；如果不需要回调，方法参数传nil
 
 回调参数tag:  -1  网络错误，请求失败
              1  有强更版本
              2  有非强更版本
              3  商店更新版本
              4  没有更新
 */
+ (void)queryUpdate:(QueryUpdateCallback)cb;
// 热更获取乐变当前版本号
+ (int)getCurrentLBVercode;
// 老用户下载完整资源接口
+ (void)downloadFullResource;
// bwbx是否是小包
+ (bool)isSplitPackage;
// bwbx资源是否下载完成
+ (bool)isDownloadFinished;
// bwbx后台下载进度：(0-100)
+ (int)backgroundDownloadProgress;
// bwbx资源总大小    单位：字节
+ (long long)getTotalSize;
// bwbx已下载资源大小    单位：字节
+ (long long)getCurrentDlSize;

@end
