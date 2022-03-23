// Fill out your copyright notice in the Description page of Project Settings.


#include "SDKEvent/SDKEvent.h"

namespace TenmoveSDK
{
	SDKEventHandle* SDKEventProcessor::AddEventHandle(int sdkEventName, SDKEventHandle::SDKDel sdkDel)
	{
		SDKEventHandle* handler = new SDKEventHandle(sdkEventName, sdkDel,this);
		if (m_events[sdkEventName] == nullptr)
		{
			m_events[sdkEventName] = new EventList();
		}
		m_events[sdkEventName]->push_back(handler);
		return handler;
	}

	void SDKEventProcessor::RemoveHandler(SDKEventHandle* handler)
	{
		if (handler != nullptr && m_events.find(handler->sdkEventName) != m_events.end())
		{
			auto localHandlerList = m_events[handler->sdkEventName];
			if (localHandlerList == nullptr || localHandlerList->empty())
				return;
			EventListIter iter = localHandlerList->begin();
			for (; iter != localHandlerList->end();)
			{
				if (*iter == handler)
					localHandlerList->erase(iter++);
				else
					++iter;
			}
		}
	}

	void SDKEventProcessor::HandleEvent(int eventName, const SDKEventParam& param)
	{
		bool isDirty = false;

		if (m_events.count(eventName))
		{
			auto localHandlerList = m_events[eventName];
			if (localHandlerList != nullptr)
			{
				EventListIter it = localHandlerList->begin();
				for (; it != localHandlerList->end();)
				{
					//删除
					auto handler = *it;
					//如果handler为空或者需要被移除
					if (handler == nullptr || handler->canRemove) {
						delete handler;
						handler = nullptr;
						localHandlerList->erase(it++);
					}
					else 
					{
						if (handler->sdkDel != nullptr)
						{
							handler->sdkDel(param);
						}
						++it;
					}
				}
			}
		}
	}

	void SDKEventProcessor::ClearAll()
	{
		EventMapIter it = m_events.begin();
		for (; it != m_events.end(); ++it)
		{
			auto localHandlerList = it->second;
			if (localHandlerList == nullptr)
				continue;

			_removeEventHandler(localHandlerList);
			delete localHandlerList;
			localHandlerList = nullptr;
		}
		m_events.clear();
	}

	void SDKEventProcessor::_removeEventHandler(EventList* handlerList)
	{
		if (handlerList == nullptr)
			return;

		EventListIter it = handlerList->begin();
		for (; it != handlerList->end(); ++it)
		{
			delete* it;
			*it = nullptr;
		}

		handlerList->clear();
	}
}
