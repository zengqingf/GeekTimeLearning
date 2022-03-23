#pragma once
//
//  TDataMaster_CS.cpp
//  TDM
//
//  Created by Morris on 16/8/19.
//  Copyright © 2016年 GCloud. All rights reserved.
//


#include <map>
#include <GCloudCore/TDataMasterDefines.h>

#define GCLOUD_API TDM_EXPORT

_TDM_Name_Space
{

#ifdef  __cplusplus
extern "C"
{
#endif
	class IEvent
	{
	public:
		virtual ~IEvent() {}

	public:
		virtual void Add(const char* key, const char* value, const int valueLen) = 0;
		virtual void Add(int key, const char* value, const int valueLen) = 0;
		virtual void Add(int key, int64_t value) = 0;

	public:
		virtual void Report() = 0;
	};

	GCLOUD_API void TDM_Initialize(const char* appId, const char* appChannel, bool isTest);

	GCLOUD_API IEvent* TDM_Create_Event(int srcId, const char* eventName);

	GCLOUD_API void TDM_Destroy_Event(IEvent** ppEvent);

	#ifdef  __cplusplus
	}
	#endif

}
