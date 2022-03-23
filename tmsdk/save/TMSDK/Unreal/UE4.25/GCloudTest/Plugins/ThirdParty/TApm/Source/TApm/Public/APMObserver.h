
//
//  APMObserver.h
//  APM
//
//  Created by 雍鹏亮 on 2019/10/23.
//  Copyright © 2019 xianglin. All rights reserved.
//


#ifndef APMObserver_hpp
#define APMObserver_hpp

//#import <Foundation/Foundation.h>

class APMObserver {
public:
    virtual void APMOnMarkLevelLoad(const char *sceneId) = 0;

    virtual void APMOnSetQulaity(const char *quality) = 0;

    virtual void APMOnLog(const char *log) = 0;

    virtual void APMOnFpsNotify(const char *fpsString) = 0;

    virtual ~APMObserver() {}
};

#endif /* APMObserver_h */
