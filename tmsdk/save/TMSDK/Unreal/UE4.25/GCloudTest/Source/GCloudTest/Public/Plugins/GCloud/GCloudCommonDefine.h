// Fill out your copyright notice in the Description page of Project Settings.

//#pragma once

#ifndef _GCLOUD_COMMON_DEFINE_H_
#define _GCLOUD_COMMON_DEFINE_H_

#include "IGCloud.h"
#include "ITDir.h"
#include "GCloudCore/ALog.h"

//腾讯埋点服务 目前设置为只支持android和ios
#if PLATFORM_ANDROID || PLATFORM_IOS
#include "ITDataMaster.h"
#endif

#include "GCloudCore/INetworkChecker.h"
#include "DolphinHelper.h"
#include "gcloud_dolphin_errorcode_check.h"
#include "GCloudDolphinUtility.h"

using GCloud::IGCloud;

using GCloud::InitializeInfo;
using GCloud::EErrorCode;
using GCloud::DolphinHelper;
using gcloud::dolphin::ErrorCodeInfo;
using gcloud::dolphin::IIPSMobileErrorCodeCheck;
using GCloud::GRFileInfo;
using GCloud::GRFileList;
using GCloud::DolphinUtilInitInfo;
using GCloud::GCloudDolphinUtility;

using GCloud::ITDir;
using GCloud::TDirObserver;
using GCloud::Result;
using GCloud::TreeInfo;
using GCloud::NodeWrapper;
using GCloud::TreeCollection;
using GCloud::QueryFriendsResult;
using GCloud::TDirInitInfo;
using GCloud::TreeNodeBase;
using GCloud::LeafNode;
using GCloud::CategoryNode;
using GCloud::SeqId;

using GCloud::LogPriority;

using ABase::INetworkChecker;
using ABase::PingResult;

#endif //_GCLOUD_COMMON_DEFINE_H_