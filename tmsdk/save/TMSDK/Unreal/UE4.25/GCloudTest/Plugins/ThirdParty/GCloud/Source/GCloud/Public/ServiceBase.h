//
//  GCloudServiceBase.h
//  GCloud
//
//  Created by vforkk on 27/3/14.
//  Copyright (c) 2014 TSF4G. All rights reserved.
//

#ifndef __GCloud__GCloudServiceBase__
#define __GCloud__GCloudServiceBase__
#include <vector>
#include "GCloudCore/ATargetBase.h"
#include "GMutex.h"

namespace GCloud {
    
    class EXPORT_CLASS IServiceObserver
    {
    protected:
        IServiceObserver(){}
        virtual ~IServiceObserver(){}
    };
    
    class EXPORT_CLASS IService
    {
    public:
        virtual ~IService(){}
        
    public:
        virtual void AddObserver(IServiceObserver* pObserver) = 0;
        virtual void RemoveObserver(IServiceObserver* pObserver) = 0;
    };
    
    class EXPORT_CLASS CServiceBase : public ABase::CTargetBase, virtual public IService
    {
    protected:
        CServiceBase(){}
        virtual ~CServiceBase(){}
        
    public:
        virtual void AddObserver(IServiceObserver* pObserver);
        virtual void RemoveObserver(IServiceObserver* pObserver);
        
    protected:
        std::vector<IServiceObserver*> m_vecObservers;
		GCloud::CMutex m_mutex;
    };
    
    
#define FOR_VECTOR(TYPE, ITER, OBJ) for(std::vector<TYPE>::iterator ITER = (OBJ).begin(); ITER != (OBJ).end(); ITER++)
    
#define FOR_OBSERVERS_BEGIN(T, POBSERVER) std::vector<IServiceObserver*> vecObservers = m_vecObservers;\
    FOR_VECTOR(IServiceObserver*, __iter, vecObservers){ \
    T POBSERVER = (T)(*__iter);\
    if (POBSERVER == NULL)\
    {\
        continue;\
    }
    
    
#define FOR_OBSERVERS_END }
}

#endif /* defined(__GCloud__GCloudServiceBase__) */
