//
//  TuringAgeService.h
//  TuringShield
//
//  Created by Sensheng Xu on 2020/9/11.
//  Copyright © 2020 Tecent Inc. All rights reserved.
//
//  $$api_level=TS_TURING_HIGHER_WRAPPER_LEVEL$$
//  $$sdk_file_filter=TS_TURING_AGE_WRAPPER_APIS$$
//
//

#import <Foundation/Foundation.h>
#import "TuringServiceDefine.h"


@tsclass(TuringAgeService);
TS_VISIBLE_LEVEL(TS_TURING_AGE_API)
@interface TuringAgeService : NSObject

+ (nonnull instancetype)sharedService;
- (nonnull instancetype)init NS_UNAVAILABLE;

/// 设置采集参数
/// @param appID  应用ID
/// @param userID  用户ID
/// @param appContext  透传数据，用于关联业务，例如IEG各部门间打通业务数据
/// @param maximumNumberOfRecordsPerTask 每次开始采集时，最多采集多少份数据
/// @param usesDebugServer 是否使用测试服务器
/// @discussion
///  * 此方法必须在 `- startCollectingForDuration:`及`- reportWithCompletionHandler:`被调用前先行调用，否
///  则无效
///  * usesDebugServer设置为YES的情况仅用于后端联调，切勿用于正式上报，否则数据无法被正确处理。如
///  不理解该用途，请始终设为NO，或与作者@@samsonxu联系
///  * appContext目前建议填入的值包括：\c qq_app_id,  \c wx_qpp_id
+ (void)setupAppID:(NSUInteger)appID andUserID:(nonnull NSString *)userID appContext:(nullable NSDictionary<NSString *, NSString *> *)appContext collectingLimit:(NSUInteger)maximumNumberOfRecordsPerTask usesDebugServer:(BOOL)usesDebugServer;

/// 设置采集参数，`+ setupAppID: andUserID: collectingLimit: usesDebugServer:`的精减版本
/// @param appID  应用ID
/// @param userID  用户ID
/// @discussion
///  此方法必须在 `- startCollectingForDuration`及`- reportWithCompletionHandler`被调用前先行调用，否则
///  无效
+ (void)setupAppID:(NSUInteger)appID andUserID:(nonnull NSString *)userID;


/// 开始采集数据
/// @param duration 采集数据的最大时长，单位为秒
/// @discussion 当参数duration大于0时，超过该时间后自动停止采集，当小于等于0时，不限制采集时长
- (void)startCollectingForDuration:(NSTimeInterval)duration;

/// 停止采集数据
- (void)stopCollecting;

/// 重置采集次数限制，每局游戏开始时应调用一次
- (void)resetCollectingLimit;

/// 上报数据
/// @param completion  执行完成时的回调
- (void)reportWithCompletionHandler:(nonnull void(^)(NSUInteger numberOfRecords, NSError *_Nullable error))completion;

@end

