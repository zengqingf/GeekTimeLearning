//
//  DDSDKApplicationDelegate.h
//  MGSDKTest
//
//  Created by ZYZ on 2018/7/9.
//  Copyright © 2018年 xyzs. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface DDSDKApplicationDelegate : NSObject

+ (instancetype)sharedInstance;

- (BOOL)application:(UIApplication *)application
            openURL:(NSURL *)url
  sourceApplication:(NSString *)sourceApplication
         annotation:(id)annotation;

#if __IPHONE_OS_VERSION_MAX_ALLOWED > __IPHONE_9_0

- (BOOL)application:(UIApplication *)application
            openURL:(NSURL *)url
            options:(NSDictionary<UIApplicationOpenURLOptionsKey,id> *)options;
#endif
@end
