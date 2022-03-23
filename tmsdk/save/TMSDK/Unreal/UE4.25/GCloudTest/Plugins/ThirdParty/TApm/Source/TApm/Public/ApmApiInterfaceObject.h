//
//  APM.h
//  APM
//
//  Created by vincentwgao on 2018/9/14.
//  Copyright © 2018年 xianglin. All rights reserved.
//
#ifndef ONESDK_TRI_IOS_APM_PORTAL_APMAPIINTERFACEOBJECT_H_
#define ONESDK_TRI_IOS_APM_PORTAL_APMAPIINTERFACEOBJECT_H_
//  #include <string>
//  #include <vector>
#include "APMObserver.h"

//  typedef struct
//  {
//    std::string key;
//    std::string value;
//
//  } APMDictionary;

#if defined(__cplusplus)
extern "C" {
#endif

/// 初始化SDK，调用其他API之前必须先做初始化操作
/// @param appId  appId
/// @param engine 上层使用引擎，如：UE4、Unity等
/// @param debug 是否为调试模式
int apm_initContext(const char *appId, const char *engine, bool debug);


/// 设置一个事件观察者
/// @param observer 事件观察者
void apm_setObserver(APMObserver *observer);


/// 是否允许为debug模式
void apm_enableDebugMode();


/// 设置服务器信息
/// @param zoneId 大区Id
/// @param roomIp 房间服务器ip
void apm_setServerInfo(const char *zoneId, const char *roomIp);


/// 进入游戏场景
/// @param sceneId 场景Id
void apm_markLevelLoad(const char *sceneId);

/// 结束游戏场景
void apm_markLevelFin();

/// 游戏进度条加载结束
void apm_markLevelLoadCompleted();

/// 设置玩家登陆后的openid
/// @param openId  openID 玩家登陆后的openid
void apm_setOpenId(const char *openId);


/// 设置游戏画质
/// @param quality 游戏画质, 默认为0
void apm_setQulaity(int quality);


/// 自定义数据流事件上报接口
/// @param eventCategory 事件名称
/// @param stepId 事件步骤
/// @param status 事件状态
/// @param code 事件状态码
/// @param msg 事件详细信息
/// @param extraKey 事件额外信息
void apm_postStepEvent(const char *eventCategory, int stepId, int status, int code,
                       const char *msg, const char *extraKey, bool authorize, bool finish);

//  APM features
void apm_postCoordinates(float x, float y, float z, float pitch, float yaw, float roll);

int apm_checkDCLSByQcc(const char *absolutePath, const char *configName);

int apm_checkDCLSByQccSync(const char *absolutePath, const char *configName);

void apm_postFrame(float deltaTime);

void apm_postNetLatency(int latency);

void apm_beginTupleWrap(const char *category);

void apm_endTupleWrap();

void apm_postValueF1(const char *category, const char *key, float a);

void apm_postValueF2(const char *category, const char *key, float a, float b);

void apm_postValueF3(const char *category, const char *key, float a, float b, float c);

void apm_postValueI1(const char *category, const char *key, int a);

void apm_postValueI2(const char *category, const char *key, int a, int b);

void apm_postValueI3(const char *category, const char *key, int a, int b, int c);

void apm_postValueS(const char *category, const char *key, const char *value);

void apm_setDefinedDeviceClass(int deviceClass);

void apm_setTargetFramerate(int target);

void apm_beginTag(const char *tagName);

void apm_endTag();

void apm_beginExtTag(const char *tagName);

void apm_endExtTag(const char *tagName);

void apm_setVersionIden(const char *versionName);

void apm_beignExclude();

void apm_endExclude();

// 获取的错误信息字符串使用完毕后需要free操作
const char * apm_getErrorMsg(int errorCode);

void apm_linkSession(const char* eventName);

void apm_initStepEventContext();

void apm_releaseStepEventContext();

void apm_postEventIS(int key, const char* value);

const char* apm_getSDKVersion();

void apm_log(const char * log);


void apm_PostTextureInfo(const char *name, int width, int height, int format,
                         bool isReadable, int MipmapLevel);

void apm_PostCpuTime(int x, int y);

int apm_GetPostCpuFlag();

int apm_GetPostGpuFlag();

void apm_SetAppPauseState(int flag);

void apm_setEngineMetaInfo(int engineType, const char *engineVersion, int grapohicsApi,
                           const char *vendor, const char *render, const char *version,
                           int gragraphicsMemSize, int graphicsMT, int supportRendertargetCount,
                           int isOpenGLES);

void apm_blockDomesticCDNURL(void);

void apm_postMemoryWarning();

void apm_syncServerTime(unsigned int serverTime);

unsigned int apm_getNativeFrameIdx(void);

void apm_startUpFinish(void);


#if defined(__cplusplus)
}
#endif
#endif  // ONESDK_TRI_IOS_APM_PORTAL_APMAPIINTERFACEOBJECT_H_
