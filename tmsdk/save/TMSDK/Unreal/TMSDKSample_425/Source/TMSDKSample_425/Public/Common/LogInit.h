#pragma once

#include "LogMgr.h"

//bool Log_Init(const char *appName, const char* cfgFile);
//void Log_Flush();
//void Log_Uninit();

#ifdef WIN32
#define __OS_PATH_SEP__ '\\'
#else
#define __OS_PATH_SEP__ '/'
#endif

#ifdef LOG_FULL_FILE_NAME
#define __FILENAME__ __FILE__
#else
#define __FILENAME__ (strrchr(__FILE__, __OS_PATH_SEP__) + 1)
#endif

#define UE4_Log(Type, UELogLevel, format, ...)\
{const FString Msg = FString::Printf(TEXT(format), ##__VA_ARGS__); \
UE_LOG(LogTemp, UELogLevel, TEXT("[%s]%s"), TEXT(Type), *Msg); \
FString t = TEXT(Type); \
} \
//if (t == "ERROR") \
//{ \
//TArray<FStringFormatArg> arg; \
//arg.Add(Msg); \
//arg.Add(FStringFormatArg(__FILE__)); \
//arg.Add(FStringFormatArg(__FUNCTION__)); \
//arg.Add(FStringFormatArg(__LINE__)); \
//PluginsManager::PostLog(FString::Format(TEXT("{0}({1}:{3})[{2}()]"), arg)); \
//}


#define UE4_LogDebug(Format, ...) UE4_Log("DEBUG", Log, Format,  ##__VA_ARGS__)
#define UE4_LogInfo(Format, ...) UE4_Log("INFO", Log, Format,  ##__VA_ARGS__)
#define UE4_LogWarn(Format, ...) UE4_Log("WARNING", Warning, Format,  ##__VA_ARGS__)
#define UE4_LogError(Format, ...) UE4_Log("ERROR", Error, Format,  ##__VA_ARGS__)
#define UE4_LogFatal(Format, ...) UE4_Log("FATAL", Error, Format,  ##__VA_ARGS__)


#if TMLOG_DEBUG
#define LogDebug(LogFormat, ...) if (!LogMgr::Instance()->IsEnabled(LOG_DEBUG)) {false;} else UE4_LogDebug(LogFormat, ##__VA_ARGS__)
#else
#define LogDebug(LogFormat, ...) {}
#endif

#if TMLOG_INFO
#define LogInfo(LogFormat, ...)	 if (!LogMgr::Instance()->IsEnabled(LOG_INFO))  { false;} else UE4_LogInfo(LogFormat, ##__VA_ARGS__)
#else
#define LogInfo(LogFormat, ...) {}
#endif

#if TMLOG_WARN
#define LogWarn(LogFormat, ...)  if (!LogMgr::Instance()->IsEnabled(LOG_WARN))  { false;} else UE4_LogWarn(LogFormat, ##__VA_ARGS__)
#else
#define LogWarn(LogFormat, ...) {}
#endif

#if TMLOG_ERROR
#define LogError(LogFormat, ...) if (!LogMgr::Instance()->IsEnabled(LOG_ERROR)) {false;} else UE4_LogError(LogFormat, ##__VA_ARGS__)
#else
#define LogError(LogFormat, ...) {}
#endif

#if TMLOG_FATAL
#define LogFatal(LogFormat, ...) if (!LogMgr::Instance()->IsEnabled(LOG_FATAL))  {false;} else UE4_LogFatal(LogFormat, ##__VA_ARGS__)
#else
#define LogFatal(LogFormat, ...) {}
#endif

/*
#define LogDebug(LogFormat, ...) !LogMgr::Instance()->IsEnabled(LOG_DEBUG) ? false : LogMgr::Instance()->Log(LOG_INFO , "%s(%s:%d) " LogFormat, __FUNCTION__, __FILENAME__, __LINE__, ## __VA_ARGS__)
#define LogInfo(LogFormat, ...)	 !LogMgr::Instance()->IsEnabled(LOG_INFO)  ? false : LogMgr::Instance()->Log(LOG_DEBUG, "%s(%s:%d) " LogFormat, __FUNCTION__, __FILENAME__, __LINE__, ## __VA_ARGS__)
#define LogWarn(LogFormat, ...)  !LogMgr::Instance()->IsEnabled(LOG_WARN)  ? false : LogMgr::Instance()->Log(LOG_WARN,  "%s(%s:%d) " LogFormat, __FUNCTION__, __FILENAME__, __LINE__, ## __VA_ARGS__)
#define LogError(LogFormat, ...) !LogMgr::Instance()->IsEnabled(LOG_ERROR) ? false : LogMgr::Instance()->Log(LOG_ERROR, "%s(%s:%d) " LogFormat, __FUNCTION__, __FILENAME__, __LINE__, ## __VA_ARGS__)
#define LogFatal(LogFormat, ...) !LogMgr::Instance()->IsEnabled(LOG_FATAL) ? false : LogMgr::Instance()->Log(LOG_FATAL, "%s(%s:%d) " LogFormat, __FUNCTION__, __FILENAME__, __LINE__, ## __VA_ARGS__)
*/
/*
#define END      LogEnd()
#define SSDebug  !LogMgr::Instance()->IsEnabled(LOG_DEBUG) ? LogMgr::Stream(LOG_DEBUG) : LogMgr::Stream(LOG_DEBUG) << __FUNCTION__ << "(" << __FILENAME__ << ':' << __LINE__ << ") "
#define SSInfo   !LogMgr::Instance()->IsEnabled(LOG_INFO)  ? LogMgr::Stream(LOG_INFO)  : LogMgr::Stream(LOG_INFO) << __FUNCTION__ << "(" << __FILENAME__ << ':' << __LINE__ << ") "
#define SSWarn   !LogMgr::Instance()->IsEnabled(LOG_WARN)  ? LogMgr::Stream(LOG_WARN)  : LogMgr::Stream(LOG_WARN) << __FUNCTION__ << "(" << __FILENAME__ << ':' << __LINE__ << ") "
#define SSError  !LogMgr::Instance()->IsEnabled(LOG_ERROR) ? LogMgr::Stream(LOG_ERROR) : LogMgr::Stream(LOG_ERROR) << __FUNCTION__ << "(" << __FILENAME__ << ':' << __LINE__ << ") "
#define SSFatal  !LogMgr::Instance()->IsEnabled(LOG_FATAL) ? LogMgr::Stream(LOG_FATAL) : LogMgr::Stream(LOG_FATAL) << __FUNCTION__ << "(" << __FILENAME__ << ':' << __LINE__ << ") "
*/