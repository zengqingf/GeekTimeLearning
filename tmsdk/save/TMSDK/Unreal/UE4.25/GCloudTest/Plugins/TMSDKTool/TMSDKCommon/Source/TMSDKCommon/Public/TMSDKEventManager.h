#pragma once

#ifndef _TMSDKEVENTMANAGER_H_
#define _TMSDKEVENTMANAGER_H_

#include "CoreMinimal.h"
#include "SDKEvent/SDKEvent.h"
#include "SDKEvent/SDKEventType.h"

namespace TenmoveSDK
{
	class TMSDKCOMMON_API TMSDKEventManager
	{
	//public:
	//	static TMSDKEventManager& GetInstance();

	public:
		SDKEventHandle* RegisterEvent(SDKEventType type, SDKEventHandle::SDKDel sdkDel);
		void RemoveEvent(SDKEventHandle* handler);
		void TriggerEvent(SDKEventType type);
		void TriggerEvent(SDKEventType type, const SDKEventParam& param);
		void ClearEvent();

	private:
		SDKEventProcessor* m_eventProcessor;

	public:
		TMSDKEventManager();
		~TMSDKEventManager();

	//private:
	//	static TMSDKEventManager* instance;
	};
}

#endif //_TMSDKEVENTMANAGER_H_