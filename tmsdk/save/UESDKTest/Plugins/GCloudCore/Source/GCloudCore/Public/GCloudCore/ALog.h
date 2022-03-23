#ifndef __ABaseLOG_H__
#define __ABaseLOG_H__

#include "GCloudPluginManager/Service/Log/ILogger.h"

namespace ABase
{
    //for GCloud user
    void PrintLogToConsoleFormat(GCloud::ALogPriority pri, const char *file, unsigned int line, const char *function,
                const char *fmt, ...);

    void PrintLogToConsoleFormat2(GCloud::ALogPriority pri, const char *file, unsigned int line, const char *function, const char *log);

    void XLog(GCloud::ALogPriority pri, const char *file, unsigned int line, const char *function, const char *fmt, ...);


    
    //intelliDevLog
    void XLogDetail(GCloud::ALogPriority pri, const char *file, unsigned int line, const char *function,  const char *module, const char *session,GCloud::IntelliDevStage stage, GCloud::IntelliDevStatus status, int errCode, int errCode2,const char *errorMsg,const char *userInfo,const char *resv, const char *fmt, ...);
    //
    void SetABaseLogLevel(GCloud::ALogPriority pri);

    bool ACheckLogLevel(GCloud::ALogPriority pri);

    typedef void (*ABaseLogCallback)(GCloud::ALogPriority pri, const char* msg);

    void SetABaseLogCallback(ABaseLogCallback callback);

    void SetConsoleOutput(bool isOpen);

    void GCloudLogInit();


}





#define XLogInfo(fmt,...)    do{ABase::XLog(GCloud::kLogInfo, __FILE__, __LINE__, __FUNCTION__,fmt,##__VA_ARGS__);}while(0)
#define XLogDebug(fmt,...)   do{ABase::XLog(GCloud::kLogDebug, __FILE__, __LINE__, __FUNCTION__,fmt,##__VA_ARGS__);}while(0)
#define XLogWarning(fmt,...) do{ABase::XLog(GCloud::kLogWarning, __FILE__, __LINE__, __FUNCTION__,fmt,##__VA_ARGS__);}while(0)
#define XLogError(fmt,...)   do{ABase::XLog(GCloud::kLogError, __FILE__, __LINE__, __FUNCTION__,fmt,##__VA_ARGS__);}while(0)
#define XLogEvent(fmt,...)   do{ABase::XLog(GCloud::kLogEvent, __FILE__, __LINE__, __FUNCTION__,fmt,##__VA_ARGS__);}while(0)

#define PrintToConsoleInfo(fmt,...)    do{ABase::PrintLogToConsoleFormat(GCloud::kLogInfo, __FILE__, __LINE__, __FUNCTION__,fmt,##__VA_ARGS__);}while(0)
#define PrintToConsoleDebug(fmt,...)   do{ABase::PrintLogToConsoleFormat(GCloud::kLogDebug, __FILE__, __LINE__, __FUNCTION__,fmt,##__VA_ARGS__);}while(0)
#define PrintToConsoleWarning(fmt,...) do{ABase::PrintLogToConsoleFormat(GCloud::kLogWarning, __FILE__, __LINE__, __FUNCTION__,fmt,##__VA_ARGS__);}while(0)
#define PrintToConsoleError(fmt,...)   do{ABase::PrintLogToConsoleFormat(GCloud::kLogError, __FILE__, __LINE__, __FUNCTION__,fmt,##__VA_ARGS__);}while(0)
#define PrintToConsoleEvent(fmt,...)   do{ABase::PrintLogToConsoleFormat(GCloud::kLogEvent, __FILE__, __LINE__, __FUNCTION__,fmt,##__VA_ARGS__);}while(0)
//intelliDevLog
#define XLogDetailed(module, session, stage, status,errCode,errCode2,errorMsg,userInfo,resv,fmt,...)  do{ABase::XLogDetail(GCloud::kLogInfo, __FILE__, __LINE__, __FUNCTION__, module, session, stage, status,errCode,errCode2,errorMsg,userInfo,resv,fmt,##__VA_ARGS__);}while(0)
//
#endif
