// ------------------------------------------------------------------------------
// File: ITGPAService
// Module: TGPA
// Version: 1.0
// Author: zohnzliu
// ------------------------------------------------------------------------------

#include "Engine.h"

#pragma once

#ifndef PUBLIC_ITGPASERVICE_H
#define PUBLIC_ITGPASERVICE_H

namespace GCloud {
namespace TGPA {

enum GameDataKey {
    OpenID = 0,                 // openid, deprecated
    MainVersion = 1,            // 游戏的主版本号
    SubVersion = 2,             // 游戏的资源版本号
    TimeStamp = 3,              // 时间戳, deprecated
    Scene = 4,                  // 游戏侧需要保障的游戏场景
    FPS = 5,                    // 游戏的FPS
    FrameMiss = 6,              // 游戏丢帧数
    FPSTarget = 7,              // 游戏的相应地图限制的最高帧率
    PictureQuality = 8,         // 游戏的画面质量
    EffectQuality = 9,          // 游戏的特效质量
    Resolution = 10,            // 游戏的分辨率
    RoleCount = 11,             // 游戏的同屏人数
    NetDelay = 12,              // 游戏的网络延迟
    Recording = 13,             // 录屏状态，for 王者荣耀
    UrgentSignal = 14,          // 紧急信号, deprecated
    ServerIP = 15,              // 服务器IP
    RoleStatus = 16,            // 角色死亡与否状态，for 王者荣耀
    EffectTransparent = 30,     // 队员特效透明度, for DNF
    TownRolesCount = 31,        // 城镇角色显示数量, for DNF
    SimplyfyTownRoles = 32,     // 城镇角色简化模式，for DNF
    ShowUIDetails = 33,         // 界面详细显示，for DNF
    PERF_FIRST_MODE = 34,  PerfFirstMode = 34,     // 性能优先模式，for DNF
    SHOW_OVERLAP_ROLES = 35, ShowOverlapRoles = 35,    // 重叠时角色显示与否，for DNF
    VIEW_SCALE = 36, ViewScale = 36,             // 比例调整，for DNF
    Dead = 39,                  // 死亡后复活时间，for 王者荣耀
    SceneType = 40,             // 场景类型, 标记游戏模式
    LoadTrunk = 41,             // 局部地图加载，for 王者荣耀
    BloomArea = 42,             // 吃鸡模式的轰炸区标记，for 王者荣耀
    MTR = 43,                   // 多核多线程渲染开启状态，for 王者荣耀
    KillReport = 44,            // 游戏的杀人数播报类型，for 王者荣耀
    LightThreadTid = 50,        // 游戏的轻负载线程
    HeavyThreadTid = 51,        // 游戏的重负载线程
    RoleOutline = 52,           // 角色描边状态，for 王者荣耀
    PictureStyle = 53,          // 画面风格，鲜艳、写实等，for 和平精英
    AntiAliasing = 54,          // 抗锯齿是否开启，for 和平精英
    ServerPort = 55,            // 服务器地址端口，for 和平精英
    SocketType = 56,            // 数据包类型，for 和平精英
    Shadow = 57,                // 阴影是否开启，for 和平精英
    Vulkan = 58,                // 是否使用Vulcan, for 和平精英
    ThreadMatchRules = 74,	    // 线程匹配规则
    ResourceUpdateProgress = 75,	// 资源更新进度. 0-开始, 100-更新完成, 1-99为更新进度百分比
    ResourceUpdateTitle = 84,       // 资源更新标题
    ResourceUpdateIcon = 85,    // 资源更新小图标
    ResetThreadTid = 5101       // 取消对应Tid的线程的绑核操作
};

class ITGPACallback {
public:
    virtual void notifySystemInfo(FString json) = 0;
};

class ITGPAService {
public:

    // 是否打开调试日志，需要在初始化之前调用
    virtual void EnableLog(bool enable) = 0;

    // 开启Debug环境，正式发布请确保不调用
    virtual void EnableDebugMode() = 0;

    // 初始化SDK，调用功能接口前必须调用, 需要在登录前进行调用
    virtual void Init() = 0;

    // 注册回调，用于接收TGPA/厂商系统传递过来的数据信息
    virtual void RegisterCallback(GCloud::TGPA::ITGPACallback *callback) = 0;

    // 发送游戏的FPS数据，需要每s统计并调用
    virtual void UpdateGameFps (float value) = 0;

    // 发送游戏的信息, 给厂商系统侧，用于性能调优
    virtual void UpdateGameInfo(const int key, const FString& value) = 0;

    // 发送游戏的信息, 给厂商系统侧，用于性能调优
    virtual void UpdateGameInfo(const int key, const int value) = 0;

    // 发送游戏的信息, 不发送给厂商系统，TGPA内部使用
    virtual void UpdateGameInfo(const FString& key, const FString& value) = 0;

    // 批量发送游戏的信息
    virtual void UpdateGameInfo(const FString& key, const TMap<FString, FString>& mapData) = 0;

    // 推荐配置获取接口
    virtual FString GetOptCfgStr() = 0;

    // 检测当前设备是否是真机
    virtual FString CheckDeviceIsReal() = 0;

    // 从TGPA侧获取数据
    virtual FString GetDataFromTGPA(const FString& key, const FString& value) = 0;
};
} // namespace TGPA
} // namespace GCloud

#endif // GCLOUD_PLUGIN_ITGPASERVICE_H
