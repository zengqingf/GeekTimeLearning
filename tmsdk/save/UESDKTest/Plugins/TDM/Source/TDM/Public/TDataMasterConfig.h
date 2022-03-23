//
//  TDataMasterConfig.h
//  TDataMaster
//
//  Created by Morris on 17/1/5.
//  Copyright © 2017年 TDataMaster. All rights reserved.
//

#ifndef TDataMasterConfig_h
#define TDataMasterConfig_h

#include <string>
#include <vector>

#include "TDataMasterDefines.h"
#include "TDataMasterCommon.h"
#include "TMutex.h"

_TDM_Name_Space
{
#define Default_Block_Bytes     (1 << 15)       // 32KB
#define Default_MAX_Speed       (1 << 16)       // 64KB/s

#define TDM_DEFAULT_KV_MAX_LOCAL_FILE_SIZE  262144
#define TDM_DEFAULT_BIN_MAX_LOCAL_FILE_SIZE  524288

#define TDM_DEFAULT_KV_MAX_LOCAL_FILE_NUM 10
#define TDM_DEFAULT_BIN_MAX_LOCAL_FILE_NUM 80

#define TDM_DEFAULT_MAX_RETRY_TIMES 3

#define TDM_VALUE_NOT_SET 0
#define TDM_VALUE_ON 1
#define TDM_VALUE_OFF -1
#define TDM_LOCAL_VALUE_ON "1"
#define TDM_LOCAL_VALUE_OFF "-1"
#define TDM_LOCAL_VALUE_NOT_SET "0"
#define TDM_STARTUP_EVENT_DELAY_DEFAULT 5
#define CONFIG_KEY_IOS_14_IDFA_ENABLE "iOS14IDFAEnable"
    
    class TDataMasterConfig
    {
    public:
        static TDataMasterConfig * GetInstance();
        static void ReleaseInstance();

        bool SetConfiguration(const void* data, int size);
        bool SetHTTPConfiguration(const std::string& respBody);

        ReportType GetReportType(EventID id, bool vip);

        uint32_t GetBlockSize();
        uint32_t GetMAXSpeed();

        const char * GetSessionID();
        bool GetEnableReportDebug();
        int GetEnableHTTPRreport();
        int GetDeviceInfoSwitch();
        int GetStartupEventDelay();
        bool IsCompressReport();
        std::string GetHTTPToken();

        std::string GetHTTPKVUrl();
        std::string GetHTTPBinUrl();

        uint32_t GetMaxRetryTimes();
        uint32_t GetMaxHTTPDBReadNum();

        uint32_t GetKVMaxLocalFileNum();
        uint32_t GetKVMaxLocalFileSize();
        uint32_t GetKVMaxLocalFileNumAfterClear();
        uint32_t GetKVConTimeOut();
        uint32_t GetKVReqTimeOut();
        uint32_t GetKVReportInterval();

        uint32_t GetBinMaxLocalFileNum();
        uint32_t GetBinMaxLocalFileSize();
        uint32_t GetBinMaxLocalFileNumAfterClear();
        uint32_t GetBinConTimeOut();
        uint32_t GetBinReqTimeOut();
        uint32_t GetBinReportInterval();
        std::string GetRemoteConfig(const std::string &key);

    private:
        TDataMasterConfig();
        TDataMasterConfig(const TDataMasterConfig& t){}
        TDataMasterConfig& operator=(const TDataMasterConfig& t){return *this;}
        ~TDataMasterConfig();

        void OnSetConfiguration();
        void SetDeviceInfoSwitch(int val);
        void SetRemoteConfig(const std::string &key, const std::string &value);

        
    private:
        static TDataMasterConfig* m_pInstance;
        static CMutex m_Mutex;
        
        std::string m_SessinID;
        std::vector<int32_t> m_EventBlacklist;
        std::string m_HTTPToken;

        std::string m_HTTPKVUrl;
        std::string m_HTTPBinUrl;
        
        bool m_EnableReport;
        bool m_SWUserData;
        bool m_SWSDKData;
        bool m_SWSysData;
        bool m_SWStartData;
        bool m_SWApolloData;
        bool m_EnableReportDebug;
        bool m_IsCompressReport;
        int m_ReportSwitch; //  data.config.client_switch    0 未配置， 1 打开，-1 关闭
        int m_DeviceInfoSwitch;
        int m_StartupEventDelay;
        
        uint32_t m_BlockSize;
        uint32_t m_MAXSpeed;

        uint32_t m_MaxRetryTimes;
        uint32_t m_MaxHTTPDBReadNum;

        uint32_t m_KVMaxLocalFileNum;
        uint32_t m_KVMaxLocalFileSize;
        uint32_t m_KVMaxLocalFileNumAfterClear;
        uint32_t m_KVConTimeOut;
        uint32_t m_KVReqTimeOut;
        uint32_t m_KVReportInterval;

        uint32_t m_BinMaxLocalFileNum;
        uint32_t m_BinMaxLocalFileSize;
        uint32_t m_BinMaxLocalFileNumAfterClear;
        uint32_t m_BinConTimeOut;
        uint32_t m_BinReqTimeOut;
        uint32_t m_BinReportInterval;
        std::map<std::string, std::string> customValues;
    };
}



#endif /* TDataMasterConfig_h */
