//
//  ApolloSdkFactoryBase.h
//  Apollo
//
//  Created by vforkk on 9/9/14.
//  Copyright (c) 2014 TSF4G. All rights reserved.
//

#ifndef __Apollo__ApolloSdkFactoryBase__
#define __Apollo__ApolloSdkFactoryBase__
#include "GCloudPlugin.h"
#include "ServiceBase.h"
#include "GCloudPublicDefine.h"
#include <vector>
using namespace std;

namespace GCloud {
    
    class EXPORT_CLASS ISdkFactory
    {
    public:
//        virtual IService* GetService(ServiceType type) = 0;
    };
    
    class EXPORT_CLASS CSdkFactoryBase : public CPluginBase, public ISdkFactory
    {
    public:
        CSdkFactoryBase();
        ~CSdkFactoryBase();
        
    public:
        virtual const char* GetName() = 0;
        virtual const char* GetVersion() = 0;
        
        
    };
    
}

#endif /* defined(__Apollo__ApolloSdkFactoryBase__) */
