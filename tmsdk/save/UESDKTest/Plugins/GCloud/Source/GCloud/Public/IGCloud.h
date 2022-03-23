//
//  IGCloud.h
//  GCloud
//
//  Created by vforkk on 14/1/14.
//  Copyright (c) 2014 TSF4G. All rights reserved.
//

#ifndef GCloud_IGCloud_h
#define GCloud_IGCloud_h
#include "GCloudPublicDefine.h"
//#include "IConnectorOld.h"
//#include "IAccountService.h"

#include <string>

namespace GCloud {
	
    class EXPORT_CLASS IGCloud
    {
    protected:
        IGCloud(){}
        virtual ~IGCloud(){}
        
    public:
        static IGCloud& GetInstance();
        ///////////////////////////////////////////////////////////////////////////////
        // Required
        ///////////////////////////////////////////////////////////////////////////////
    public:

        virtual EErrorCode Initialize(const InitializeInfo& initInfo) = 0;

        virtual void SetUserInfo(const UserInfo& userInfo) = 0;

    public:
        virtual void SetLogger(LogPriority priority) = 0;
        virtual const char* GetVersion()= 0;
        
    };
    
    
    void DisableOneSDK();
#if defined(_WIN32) || defined(_WIN64)
//    inline IGCloud* GetIGCloudInstance(HMODULE hDll)
//    {
//        typedef IGCloud*( *CreateFunction)();
//        CreateFunction pFunc = NULL;
//        pFunc = (CreateFunction)GetProcAddress(hDll, "gcloud_getInstance");
//        if(pFunc == NULL)
//        {
//            return NULL;
//        }
//        return pFunc();
//    }
    
#endif
}

#endif
