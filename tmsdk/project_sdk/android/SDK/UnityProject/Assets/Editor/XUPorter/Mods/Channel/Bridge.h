//
//  Bridge.hpp
//  PlatformDemo
//
//  Created by Yu Lekai on 03/11/2016.
//  Copyright © 2016 Eason. All rights reserved.
//

#ifndef XYBridge_hpp
#define XYBridge_hpp

# define _DLLExport __declspec (dllexport)

#ifdef __cplusplus
extern "C"{
#endif
    
    void _Init(const char *apkInfoString , const char *iOSInfoExtra, bool debug);
    void _OpenLogin();
    void _Pay(const char* payIofoString , const char* userIofoString);
    void _OpenMobileBind();
    void _CheckIsBindPhoneNum();
    void _ReportRoleInfo(const char *roleInfo ,int param);
    void _SetRoleLoginGame();
    void _SetRoleLogoutGame();
    void _GetNewVersionInAppstore();
    bool _IsLoginSmallGame();
    bool _IsSDKInited();
    
#ifdef __cplusplus
} // extern "C"
#endif

#endif /* XYBridge_hpp */
