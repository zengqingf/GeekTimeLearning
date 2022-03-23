// Fill out your copyright notice in the Description page of Project Settings.


#include "SDKEvent/SDKEvent.h"

namespace TenmoveSDK
{
	SDKEventHandle* SDKEventProcessor::AddEventHandle(int sdkEventName, SDKEventHandle::SDKDel sdkDel)
	{
		SDKEventHandle* handler = new SDKEventHandle(sdkEventName, sdkDel,this);
		if (nullptr == m_events[sdkEventName])
		{
			m_events[sdkEventName] = new EventList();
		}
		m_events[sdkEventName]->push_back(handler);
		return handler;
	}

	void SDKEventProcessor::RemoveHandler(SDKEventHandle* handler)
	{
		if (handler != nullptr && m_events.find(handler->EventName()) != m_events.end())
		{
			auto localHandlerList = m_events[handler->EventName()];
			if (nullptr == localHandlerList || localHandlerList->empty())
				return;
			EventListIter iter = localHandlerList->begin();
			for (; iter != localHandlerList->end();)
			{
				if (*iter == handler)
					iter = localHandlerList->erase(iter);
				else
					++iter;
			}
			
			delete handler;
			handler = nullptr;
		}
	}

	void SDKEventProcessor::HandleEvent(int eventName, const SDKEventParam& param)
	{
		if (m_events.count(eventName))
		{
			auto localHandlerList = m_events[eventName];
			if (localHandlerList != nullptr)
			{
				EventListIter it = localHandlerList->begin();
				for (; it != localHandlerList->end();)
				{
					auto handler = *it;
					//如果handler为空或者需要被移除
					if (nullptr == handler || handler->CanRemove()) {
						it = localHandlerList->erase(it);
						if (handler != nullptr) {
							delete handler;
							handler = nullptr;
						}
					}
					else 
					{
						if (handler->SDKDelegate() != nullptr)
						{
							handler->SDKDelegate()(param);
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
			if (nullptr == localHandlerList)
				continue;

			_removeEventHandler(localHandlerList);
			if (localHandlerList != nullptr)
			{
				delete localHandlerList;
				localHandlerList = nullptr;
			}
		}
		m_events.clear();
	}

	void SDKEventProcessor::_removeEventHandler(EventList* handlerList)
	{
		if (nullptr == handlerList)
			return;

		EventListIter it = handlerList->begin();
		for (; it != handlerList->end(); ++it)
		{
			if (*it != nullptr)
			{
				delete* it;
				*it = nullptr;
			}
		}

		handlerList->clear();
	}
}
