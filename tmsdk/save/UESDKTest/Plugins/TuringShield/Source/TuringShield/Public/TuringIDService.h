//
//  TuringIDService.h
//  TuringShield
//
//  Created by Sensheng Xu on 2020/7/15.
//  Copyright © 2020 Tecent Inc. All rights reserved.
//
//  $$api_level=TS_TURING_HIGHER_WRAPPER_LEVEL$$
//  $$sdk_file_filter=TS_TURING_ID_WRAPPER_APIS$$
//
//

#import <Foundation/Foundation.h>
#import "TuringServiceDefine.h"


@tsclass(TuringID);
@tsclass(TuringIDService);

TS_VISIBLE_LEVEL(TS_TURING_ID_API)
@interface TuringID : NSObject <NSCopying>

- (nonnull instancetype)init NS_UNAVAILABLE;

@property (nullable, nonatomic, readonly) NSError *error;

@property (nullable, nonatomic, readonly) NSString *TAIDTicket TS_AVAILABLE_IFS_OR(TS_ACT_AS_TAID_ADVERTISER, TS_ACT_AS_TAID_PROVIDER);

@property (nullable, nonatomic, readonly) NSString *TDIDTicket;

@end

TS_VISIBLE_LEVEL(TS_TURING_ID_API)
/// 图灵盾ID服务
@interface TuringIDService : NSObject

+ (nonnull instancetype)sharedService;
- (nonnull instancetype)init NS_UNAVAILABLE;


/// 开始设备ID服务
/// @param userID  用户ID
/// @discussion 具有登录态的业务，建议填入用户ID，例如uni、open ID或其MD5
- (void)startServiceWithUserID:(nullable NSString *)userID;

/// 开始设备ID服务（不带用户ID）
- (void)startService;

/// 停止设备ID服务
- (void)stopService;


/// 获取设备图灵ID
/// @param completionHandler 设备ID的异步返回的回调函数
/// @discussion
///     * 设备ID通过回调函数的参数返回给调用者
///     * 回调函数不在主线程，注意线程安全
- (void)fetchTuringIDWithCompletionHandler:(nonnull void(^)(TuringID *_Nonnull turingID))completionHandler;


/// 获取设备图灵ID（同步接口）
- (nullable TuringID *)fetchTuringIDSynchronically;

/// 获取缓存中的设备图灵ID
/// @discussion
///     * 函数会立即返回，不会执行联网请求，也不会阻塞当前线程。
///     * 如果当前没有缓存，也会返回空值。
@property (nullable, nonatomic, copy, readonly) TuringID *cachedTuringID;

@end

@interface TuringIDService (Debug)

/// 是否使用测试服务器，默认为否
/// @discussion
///     * 此开关主要用于方便后端联调，查找问题，切勿用在正式环境发布。
///     * 此开关仅在启用`- startService`之前设置有效。
@property (nonatomic, assign, class) BOOL usesDebugServer;

@end

