//
//  APMUEObserver.hpp
//  APM
//
//  Created by 雍鹏亮 on 2019/10/30.
//  Copyright © 2019 xianglin. All rights reserved.
//

#ifndef APMUEObserver_hpp
#define APMUEObserver_hpp
#if PLATFORM_IOS

#include "APMObserver.h"

class APMUEObserver : public APMObserver {
private:
    static APMUEObserver * m_pInst;
public:
    static APMUEObserver *GetInstance();

    void APMOnMarkLevelLoad(const char *sceneId);

    void APMOnSetQulaity(const char *quality);

    void APMOnLog(const char *log);

    void APMOnFpsNotify(const char *fpsString);

};
#endif

#endif /* APMUEObserver */
