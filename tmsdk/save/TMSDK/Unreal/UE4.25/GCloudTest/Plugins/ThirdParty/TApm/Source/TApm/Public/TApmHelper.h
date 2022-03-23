//------------------------------------------------------------------------------
//
// File: TApmHelper.h
// Module: ApmSDK
// Version: 1.0
// Author: vincentwgao
//
//------------------------------------------------------------------------------

#ifndef _APM_INTERFACE_H
#define _APM_INTERFACE_H

#if PLATFORM_WINDOWS
#define APM_API  __declspec(dllexport)
#else
#define APM_API
#endif

#include "CoreMinimal.h"
#include <string>
#include <map>

namespace GCloud
{
    namespace APM
    {
        class APM_API TApmCallback
        {
        public:
            //TApmCallback(){};

            virtual ~TApmCallback() {};
            /// <summary> 初始化接口回调
            ///
            /// </summary>
            /// <param name="succcess">状态，true为成功， false为失败</param>
            /// <param name="msg">如果为false，错误信息</param>
            /// <returns></returns>
            virtual void OnInitContext(bool succcess, const char* msg) = 0;

            /// <summary> 场景开始
            ///
            /// </summary>
            /// <param name="succcess">状态，true为成功， false为失败</param>
            /// <param name="msg">如果为false，错误信息</param>
            /// <param name="sceneName">场景名息</param>
            /// <returns></returns>
            virtual void OnMarkLevelLoad(bool succcess, const char*  msg, const char*  sceneName) = 0;

            /// <summary> 场景记载结束回调
            ///
            /// </summary>
            /// <param name="succcess">状态，true为成功， false为失败</param>
            /// <param name="msg">如果为false，错误信息</param>
            /// <param name="sceneName">场景名息</param>
            /// <param name="loadedTime">加载时间mills</param>
            /// <returns></returns>
            virtual void OnMarkLevelLoadCompleted(bool succcess, const char*  msg, const char* sceneName, int loadedTime) = 0;

            /// <summary> 场景结束回调
            ///
            /// </summary>
            /// <param name="succcess">状态，true为成功， false为失败</param>
            /// <param name="msg">如果为false，错误信息</param>
            /// <param name="sceneName">场景名息</param>
            /// <param name="totalSceneTime">总时间</param>
            /// <param name="totalFrames">总帧数</param>
            /// <param name="maxPss">最大PSS</param>
            /// <returns></returns>
            virtual void OnMarkLevelFin(bool succcess, const char* msg, const char* sceneName, int totalSceneTime, int totalFrames, int maxPss) = 0;


        };

        typedef void(*LogObserver)(const char* log);
        class APM_API TApmHelper
        {

        public:

            /* 获取SDK的版本信息 */
            static const char* GetSDKVersion();

            /*  初始化
                InitContext("12345678", false);
                返回状态码
             */
            static int InitContext(const char *appId, bool debug = false);

            /* deprecated
            */
            static int InitContext(const char *appId, const char *engine, bool debug);

            /* 开启调试日志  deprecated*/
            static void EnableDebugMode();

            /* 设置日志回调 */
            static void SetLogObserver(LogObserver logObserver);
            static LogObserver sLogObserver;

            /* 画质设定 */
            static void SetQuality(int quality);
            /* 设置用户openid */
            static void SetUserId(const char* userId);

            /* 设置用户openid */
            //static void SetUserId(const char* userId);

            /* 标记场景开始 */
            static void MarkLevelLoad(const char* sceneName);

            /* 标记加载场景完毕 */
            static void MarkLevelLoadCompleted();

            /* 标记场景结束 */
            static void MarkLevelFin();

            /* 上传网络延时 */
            static void PostNTL(int latency);

            /* 场景内加标记 */
            static void BeginExtTag(const char* TagName);

            /* 场景内结束标记 */
            static void EndExtTag(const char* TagName);

            /* 场景内加标记 */
            static void BeginTag(const char* TagName);

            /* 场景内结束标记 */
            static void EndTag();

            /* 动态判定机型的档位 */
            static void GetDeviceLevelSyncFromServerByQcc(const char* configName, const char* filePath);
#if PLATFORM_IOS
            // ios
            static int GetDeviceLevelByQcc(const char* configName, const char* filePath);
#else

            static int GetDeviceLevelByQcc(const char* configName, const char* gpuFamily);
#endif
            /* 设置国家地区，用于海外发行的游戏 deprecated*/
            static void SetLocale(const char* Locale);

            /* 设置目标帧率，APM后台会对不同的目标帧率做区分统计 */
            static void SetTragetFrameRate(int target);

            /* 上报回扯信息，distance表示回扯的距离 deprecated*/
            static void PostLagStatus(float distance);

            /* APP启动初始化完成的标记，可以用于统计App的启动时间
            启动时间 = MarkAppFinishLaunch被调用的时间戳 - InitContext被调用的时间戳 */
            static void MarkAppFinishLaunch();

            /* 上报游戏内的事件，比如预定义的，上载具，下载具，天气系统等，
            info参数可以添加一些额外的信息，比如载具的信息，天气系统的信息
            通过Event接口上传的信息APM后台会进行各维度的统计分析 */
            static void PostEvent(int key, const char* info = "nA");

            /* 手动设置版本号，忽略TAPM的自动获取的版本号 */
            static void SetVersionIden(const char* versionName);

            /* 将Pss的获取修改为手动模式，默认是自动采集 deprecated*/
            static void SetPssManualMode();

            /* 手动模式下，请求pss采集 deprecated*/
            static void RequestPssSample();

            /* TICK中上传帧数 */
            static void PostFrame(float deltatime);

            enum StreamStep
            {
                kStreamStepLaunch = 1, kStreamStepCheckUpdate, kStreamStepUpdating, kStreamStepPlatform, kStreamStepMSDKAuth, kStreamStepFetchZone, kStreamStepGameStart, kStreamStepEnterLobby
            };

            enum StreamStatus
            {
                kStreamStatusSuccess, kStreamStatusFailed
            };

            ///* 上传登录流程信息
            //kStreamStepLaunch 启动游戏成功
            //kStreamStepCheckUpdate 检查更新
            //kStreamStepUpdating 更新中
            //kStreamStepPlatform 选择平台
            //kStreamStepMSDKAuth 登录验证
            //kStreamStepFetchZone Zone列表信息
            //kStreamStepGameStart 开始游戏
            //kStreamStepEnterLobby 进入大厅
            //登录流程信息上报 deprecated*/
            static void PostStreamEvent(int stepId, int status, int code, const char* info = "NA");

            /* 上传玩家的坐标信息和朝向信息，APM后台会对上传的坐标点的性能进行检查，对于性能低点会绘制成性能热力图，用于聚合分析 */
            static void PostTrackState(float x, float y, float z, float pitch, float yaw, float roll);

            // 上传浮点型数据，
            // 后台将属于同一个category的key做聚合分析
            // <param name="category">类别</param>
            // <param name="key">键值</param>
            // <param name="a">实际值</param>
            //
            // PostValueF("FuntionTime", "Update", 0.33f)
            // FuntionTime表示定义一个“函数执行耗时”的类别
            // "Update"表示具体的函数名
            // "0.33f"表示函数的执行耗时
            // APM后台服务器会自动的根据上传的信息，做统计分析
            // 在页面中，选择类别Category以及key的信息，查看相关数据的统计分析结果
            static void PostValueF(const char* category, const char* key, float a);

            //上传浮点型数据
            // 同PostValueF，但是可以上传两个有关联的值，这两个值会作为一个二元组，一起过统计分析
            static void PostValueF(const char* category, const char* key, float a, float b);

            // 上传浮点型数据
            // 同PostValueF，但是可以上传三个有关联的值，这两个值会作为一个三元组，一起过统计分析
            static void PostValueF(const char* category, const char* key, float a, float b, float c);

            // 上传整型数据
            // 功能等同于PostValueF
            static void PostValueI(const char* category, const char* key, int a);

            // 上传整型数据
            // 功能等同于PostValueF
            static void PostValueI(const char* category, const char* key, int a, int b);

            // 上传整型数据
            // 功能等同于PostValueF
            static void PostValueI(const char* category, const char* key, int a, int b, int c);

            // 上传整型数据
            // 功能等同于PostValueF
            static void PostValueS(const char* category, const char* key, const char* value);

            // <summary> 开始结构体定义
            // 结构体定义为自定义数据提供了强大的扩展能力，
            // 整个结构体的数据会进行整体的统计分析，
            // 提供了复合键值的能力以及上传多维度数据的能力。
            // 类比的说，通过BeginTupleWrap上传的数据，在后台会统计分析结果存为一张二维表，
            // 表名为tupleName，
            // BeginTupleWrap和EndTupleWrap中上传的key的值为列名，value为各个列的值
            // category为TupleKeyIden的key值为复合主键
            // category为TupleValueIden的key为具体的值
            // </summary>

            // <example>
            // 如上传函数耗时，但是函数的耗时是分模块的(比如不同的模块中定义了相同的函数名),
            // 再者函数耗时包含两个维度，自身函数耗时以及子函数耗时
            // 
            // BeginTupleWrap("FuntionCostAnalyse") //开始上传一个结构体， 结构体的名字为FuntionCostAnalyse
            // PostValueS(TUPLE_KEY, "ModuleName", "ModuleA");//通过TUPLE_KEY表明这是一个键值，键名为"TupleKeyIden"，对应的值为"ModuleA"
            // PostValueS(TUPLE_KEY, "FunctionName", "Update");//通过TUPLE_KEY表明这是一个键值，键名为"FunctionName"，对应的值为"Update"
            //  
            // PostValueF(TUPLE_VALUE, "SelfCost", 0.33f);
            // PostValueF(TUPLE_VALUE, "childrenCost", 0.16f);
            // EndTupleWrap()
            // 
            // 数据库的表模型为:
            // TableName:
            // |————————————|——————————————|——————————|——————————————|
            // | ModuleName | FunctionName | SelfCost | childrenCost |
            // |————————————|——————————————|——————————|——————————————|
            // |   ModuleA  |    Update    |    0.33  |     0.16     |
            // |————————————|——————————————|——————————|——————————————|
            // |   ModeuleB |    Update    |    0.33  |     0.20     |
            // |————————————|——————————————|——————————|——————————————|
            // primaryKey(ModuleName, FunctionName)
            // 
            // PostValueI(string category, string key, int a, int b)
            // 或者 PostValueI(string category, string key, int a, int b, int c)
            // 函数可用BeginTupleWrap，PostValueI替代
            // 
            // 通过PostValueI系列函数上传的数据，在后台也会抽成为一张二维表，
            // 表名为category，主键为key，可有多个维度的值（一个，两个，三个）
            //  
            // </example>
            //
            // <remarks>
            // 位于BeginTupleWrap和EndTupleWrap中的PostValueXXX系列函数，
            // 其category必须为TUPLE_KEY或者TUPLE_VALUE常量值
            // 否则后台不会解析
            // </remarks>
            static void BeginTupleWrap(const char* key);

            static void EndTupleWrap();

            // 开始剔除，适用于MMO游戏，将挂机的数据剔除
            static void BeginExclude();
            // 剔除结束，适用于MMO游戏，将挂机的数据剔除
            static void EndExclude();

            // 初始化漏斗环境
            static void InitStepEventContext();

            /**
            * eventCategory：关键路径名
            * stepId：步骤id
            * status：成功与否，0表示成功，非0表示失败，这会关系到后续的统计分析
            * code：项目自定义代码
            * msg：项目自定义消息
            * extraKey：如果extra被设置，则后台聚合的时候则使用此key
            * authorize：是否为鉴权步骤
            * finish：是否结束
            */
            static void PostStepEvent(FString eventCategory, int stepId, int status, int code, FString msg, FString extraKey = "NA", bool authorize = false, bool finish = false);

            // 在不同启动过程中进行事件的关联
            static void LinkLastStepEventSession(FString eventCategory);

            // 释放漏斗环境中的所有资源
            static void ReleaseStepEventContext();

            // 设置机型的档位，以代替TDR三档位的聚合计算
            static void SetDeviceLevel(int deviceLevel);

            // 设置目标帧率
            /**
             * target：目标帧率
             */
            static void SetTargetFramerate(int target);

            static const char* TUPLE_KEY;

            static const char* TUPLE_VALUE;

            /*deprecated */
            static void SetCallback(TApmCallback *callback);

            static char* GetErrorMsg(int code);
            /*
            玩家染色事件
            */
            static void PostMarkEvent(int key, const char* value);

            /**
             * 设置启动时间
             * time:启动时积极
             */
            static void SyncServerTime(unsigned time);

            /**
             * 关闭CDNDomain
             */
            static void DisableDomCDNDomain();

            /*deprecated */
            static char* GetCurrentSceneName();
            /*deprecated */
            static int GetSceneLoadedTime();
            /*deprecated */
            static int GetMaxPss();
            /*deprecated */
            static const char* getCurrentSceneName();
            /*deprecated */
            static int GetTotalTime();
            /*deprecated */
            static int GetTotalFrames();

            //标记启动完成 用于计算启动耗时
            static void MarkStartUpFinish();
        private:
            static bool bEnableApm;
            static TApmCallback *sCallback;
        };
    }
}


#endif
