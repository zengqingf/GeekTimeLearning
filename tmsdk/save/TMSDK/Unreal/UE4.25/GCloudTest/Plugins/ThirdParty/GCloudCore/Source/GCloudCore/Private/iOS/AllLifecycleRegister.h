#ifndef AllLifecycleRegister_h
#define AllLifecycleRegister_h


#import <GCloudCore/GCloudAppLifecycleObserver.h>

//TDM
#import "PluginReportLifecycle.h"
REGISTER_LIFECYCLE_OBSERVER(PluginReportLifecycle);

//APM
#import "APMLifecycle.h"
REGISTER_LIFECYCLE_OBSERVER(APMLifecycle);

//GCloud
#import "GCloudAppLifecycleListener.h"
REGISTER_LIFECYCLE_OBSERVER(GCloudAppLifecycleListener);

//GVoice
//#import "PluginGVoiceLifecycle.h"
//REGISTER_LIFECYCLE_OBSERVER(PluginGVoiceLifecycle);

//TGPA
#import "TGPAPluginLifecycle.h"
REGISTER_LIFECYCLE_OBSERVER(TGPAPluginLifecycle);


#endif