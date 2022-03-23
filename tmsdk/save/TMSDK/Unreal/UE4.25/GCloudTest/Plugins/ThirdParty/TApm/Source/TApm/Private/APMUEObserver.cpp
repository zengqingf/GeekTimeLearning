//
//  APMUnityObserver.cpp
//  APM
//
//  Created by 雍鹏亮 on 2019/10/30.
//  Copyright © 2019 xianglin. All rights reserved.
//

#include "APMUEObserver.h"
#include "TApmHelper.cpp"

using namespace GCloud::APM;

#if PLATFORM_IOS

APMUEObserver* APMUEObserver::m_pInst = NULL;
APMUEObserver* APMUEObserver::GetInstance()
{
    if (!m_pInst) {
        m_pInst = new APMUEObserver();
    }
    return m_pInst;
}

void APMUEObserver::APMOnMarkLevelLoad(const char *sceneId) {
    if (sceneId != NULL)
    {

    }
}

void APMUEObserver::APMOnSetQulaity(const char *quality)
{
    if (quality != NULL)
    {

    }
}

void APMUEObserver::APMOnLog(const char *log) {
    if (log != NULL)
    {
        if (TApmHelper::sLogObserver)
        {
            TApmHelper::sLogObserver(log);
        }
    }
}

void APMUEObserver::APMOnFpsNotify(const char *fpsString) {
    if (fpsString != NULL)
    {

    }
}
#endif
