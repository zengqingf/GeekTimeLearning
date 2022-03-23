#ifndef AllLifecycleRegister_h
#define AllLifecycleRegister_h


#import <GCloudCore/GCloudAppLifecycleObserver.h>

//TDM
#import "PluginReportLifecycle.h"
REGISTER_LIFECYCLE_OBSERVER(PluginReportLifecycle);

//GVoice
#import "PluginGVoiceLifecycle.h"
REGISTER_LIFECYCLE_OBSERVER(PluginGVoiceLifecycle);


#endif