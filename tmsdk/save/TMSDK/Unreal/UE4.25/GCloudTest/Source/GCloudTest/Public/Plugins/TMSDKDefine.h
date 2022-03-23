// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#ifndef _TMSDK_DEFINE_H_
#define _TMSDK_DEFINE_H_

#include "PluginManager.h"
#include "PluginCommon.h"

using TenmoveSDK::SDKInfo;
using TenmoveSDK::SDKComponentType;
using TenmoveSDK::SDKComponent;
using TenmoveSDK::SDKComponentCreator;

using TenmoveSDK::SDKEventType;
using TenmoveSDK::SDKEventParam;
using TenmoveSDK::SDKEventHandle;
using TenmoveSDK::TMSDKEventManager;

using TenmoveSDK::PermissionType;


#define ENUM_TO_STR(ENUM) std::string(#ENUM)
#define SDK_EVENT_FUNC(func) [&](const SDKEventParam& __param) { func(__param); }

#endif //_TMSDK_DEFINE_H_