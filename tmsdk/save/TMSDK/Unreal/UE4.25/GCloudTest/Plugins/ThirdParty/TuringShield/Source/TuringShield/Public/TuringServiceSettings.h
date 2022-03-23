//
//  TuringServiceSettings.h
//  TuringShield
//
//  Created by 徐森圣 on 2018/3/19.
//  Copyright © 2018年 Tecent Inc. All rights reserved.
//


#ifndef TuringServiceSettings_h
#define TuringServiceSettings_h


/**
 编译目标，用于检查变量是否正确
 格式为TS_BUILD_TARGET_xxx
 
 @discussion
 受Build Settings中环境变量TS_BUILD_TARGET影响并自动更新
 */
#define TS_BUILD_TARGET_IEG

/**
 定义为1，使用主人识别模型
 
 @discussion
 受Build Settings中同名环境变量影响并自动更新
 */
#define TS_OWNER_PROJECT                            0


/**
 定义为1，使用青少年识别模型
 
 @discussion
 受Build Settings中同名环境变量影响并自动更新
 */
#define TS_AGE_PROJECT                              1

/**
 定义为1，使用人机识别模型
 
 @discussion
 受Build Settings中同名环境变量影响并自动更新
 */
#define TS_HUMAN_PROJECT                            0

/**
 渠道号，用于决定用哪个模型
 @discussion
 受Build Settings中同名环境变量影响并自动更新
 */
#define TS_SDK_CHANNEL_ID                           302010
#define TS_SDK_CHANNEL_STRING                       __TS_TO_NSSTRING(TS_SDK_CHANNEL_ID)

/**
 SDK是否自带数据请求和回复逻辑
 
 @discussion
 受Build Settings中同名环境变量影响并自动更新
 */
#define TS_ENABLES_DATA_SENDING                     1

/**
 SDK是否自带数据处理逻辑
 
 @discussion
 受Build Settings中同名环境变量影响并自动更新
 */
#define TS_ENABLES_PREDICTION_PROCEEDING            1


/**
 SDK是否带请求签名逻辑，云端请求需要WAF服务配合，SDK需要打开
 TS_ENABLES_DATA_SENDING和TS_HUMAN_PROJECT
 
 @discussion
 受Build Settings中同名环境变量影响并自动更新
 */
#define TS_ENABLES_HTTP_REQUEST_SIGN                0

/**
 如果设置为非0，则同一个场景（scene）和事件（action）最多保留指定数量的数据
 
 @discussion
 受Build Settings中同名环境变量影响并自动更新
 */
#define TS_LIMITED_RECORDS_PER_SCENE_ACTION         12

/**
 实验室模式，某些逻辑在工程化前后可能有变化，服务器接口的调用方式也可能不一致
 目前人机部分默认值为0，主人识别部分默认值为1
 
 @discussion
 受Build Settings中同名环境变量影响并自动更新
 */
#define TS_ENABLES_LAB_LOGIC                        0

/**
 打开日志打印
 
 @discussion
 受Build Settings中同名环境变量影响并自动更新
 */
#define TS_ENALBLES_LOG_PRINT                       0

/**
 使用模型预测之前不检查数据非空
 
 @discussion
 受Build Settings中同名环境变量影响并自动更新
 */
#define TS_DONT_CHECK_DATA_SIZE                     0

/**
 不记录（当然也不上报）触摸事件中的位置信息
 
 @discussion
 受Build Settings中同名环境变量影响并自动更新
 */
#define TS_DONT_RECORD_TOUCH_POSITION               0


/**
 不加密打包的数据
 
 @discussion
 受Build Settings中同名环境变量影响并自动更新
 */
#define TS_DONT_ENCRYPT_PACKING_DATA                0

/**
 不压缩打包的数据
 
 @discussion
 受Build Settings中同名环境变量影响并自动更新
 */
#define TS_DONT_COMPRESS_PACKING_DATA               0

/**
 使用设备指纹特性

 @discussion
 受Build Settings中同名环境变量影响并自动更新
 */
#define TS_ENABLES_FINGERPRINT_FEATURE              1

/**
支持的打包方式：WUP协议

@discussion
受Build Settings中同名环境变量影响并自动更新
*/
#define TS_NETWORKING_WUP_SUPPORTS                  0

/**
支持的打包方式：Shark协议

@discussion
受Build Settings中同名环境变量影响并自动更新
*/
#define TS_NETWORKING_SHARK_SUPPORTS                1

/**
支持的打包方式：TMF Shark协议

@discussion
受Build Settings中同名环境变量影响并自动更新
*/
#define TS_NETWORKING_TMF_SHARK_SUPPORTS            0


/**
 是否支持云控设备指纹配置；目前私有化部署不支持该选项
 
@discussion
受Build Settings中同名环境变量影响并自动更新
 */
#define TS_ENABLES_FINGERPRINT_CONFIG_CONTROL       1


/**
 是否支持关键API篡改检测
 
@discussion
受Build Settings中同名环境变量影响并自动更新
 */
#define TS_ENABLES_KEYED_API_CHECKING               0


/**
 绑定的App bundle ID

 @discussion
 设置后SDK将只能运行在指定的App上
 */
#define TS_BOUND_BUNDLE_IDENTIFIER                  nil
#define TS_BOUND_BUNDLE_TOKEN                       { 0x00 }


#define TS_DOMAIN_PREFIX_TOKEN                      com.tencent.TuringShield.
#define TS_DOMAIN_PREFIX_CSTRING                    __TS_TO_CSTRING(TS_DOMAIN_PREFIX_TOKEN)
#define TS_DOMAIN_PREFIX_STRING                     __TS_TO_NSSTRING(TS_DOMAIN_PREFIX_TOKEN)
#define TS_DOMAIN_FOR_REACHABLITTY_TEST             8.8.8.8
#define TS_DOMAIN_FOR_REACHABLITTY_TEST_CSTRING     __TS_TO_CSTRING(TS_DOMAIN_FOR_REACHABLITTY_TEST)
#define TS_DOMAIN_FOR_REACHABLITTY_TEST_STRING      __TS_TO_NSSTRING(TS_DOMAIN_FOR_REACHABLITTY_TEST)

/**
 一些临时用的开关
 */
#define TS_ENABLES_SENSOR_RECORDING                 0
#define TS_ENABLES_SENSOR_REPLAYING                 0
#define TS_USES_OLD_MOTION_TRACKER_IF_NEEDED        1
#define TS_ENABLES_CUSTOM_CLIENT_VERSION            0
#define TS_ENABLES_CUSTOM_CHANNEL_ID                0
#define TS_USES_CLASS_ALIAS                         0
#define TS_ACT_AS_TAID_ADVERTISER                   0
#define TS_ACT_AS_TAID_PROVIDER                     1
#define TS_ENABLES_FINGERPRINT_CONFIG_TEST          0
#define TS_API_LEVEL                                TS_TURING_HIGHER_WRAPPER_LEVEL
#define TS_AVOID_USING_IDFA                         1
#define TS_FEATURE_DELEGATION                       0

#define TS_TURING_ID_WRAPPER_APIS                   1
#define TS_TURING_AGE_WRAPPER_APIS                  1


#define TS_SDK_VERSION                              20048
#define TS_SDK_LC_CODE                              O2SCESU3F80YKWH9
#define TS_SDK_LC                                   __TS_TO_NSSTRING(TS_SDK_LC_CODE)
#define TS_SDK_BUILD_DATE                           @__DATE__ @" " @__TIME__
#define TS_ALIAS_SURFIX                             

#endif /* TuringServiceSettings_h */
