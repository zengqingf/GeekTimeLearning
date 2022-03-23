#pragma once

#ifndef _TMSDKEVENTMANAGER_H_
#define _TMSDKEVENTMANAGER_H_

#include "CoreMinimal.h"
#include "SDKEvent/SDKEvent.h"

namespace TenmoveSDK
{
	class TMSDKCOMMON_API TMSDKEventManager
	{
	public:

		static TMSDKEventManager& GetInstance();

	public:
		SDKEventHandle* RegisterEvent(SDKEventType type, SDKEventHandle::SDKDel sdkDel);
		void RemoveEvent(SDKEventHandle* handler);
		void TriggerEvent(SDKEventType type);
		void TriggerEvent(SDKEventType type, const SDKEventParam& param);
		void ClearEvent();

	private:
		TMSDKEventManager();
		~TMSDKEventManager();
		SDKEventProcessor* m_eventProcessor;
	};

#define SDK_EVENT_FUNC(func) [&](const SDKEventParam& __param) { func(__param); }
}

#endif //_TMSDKEVENTMANAGER_H_