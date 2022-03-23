// Fill out your copyright notice in the Description page of Project Settings.


#include "Plugins/SDKEvent/SDKEvent.h"

namespace TenmoveSDK
{
	SDKEventHandle* SDKEventProcessor::AddSDKEventHandle(int sdkEventName, SDKEventHandle::SDKDel sdkDel)
	{
		SDKEventHandle* handler = new SDKEventHandle(sdkEventName, sdkDel,this);
		if (events[sdkEventName] == nullptr)
		{
			events[sdkEventName] = new list<SDKEventHandle*>();
		}
		events[sdkEventName]->push_back(handler);
		return handler;
	}

	void SDKEventProcessor::RemoveHandler(SDKEventHandle* handler)
	{
		if (handler != NULL && events.find(handler->sdkEventName) != events.end())
		{
			auto localHandlerList = events[handler->sdkEventName];

			list<SDKEventHandle*>::iterator iter = localHandlerList->begin();
			for (; iter != localHandlerList->end();)
			{
				if (*iter == handler)
					localHandlerList->erase(iter);
				else
					++iter;
			}
		}
	}

	void SDKEventProcessor::HandleEvent(int eventName, SDKEventParam& param)
	{
		bool isDirty = false;

		if (events.count(eventName))
		{
			auto localHandlerList = events[eventName];
			if (localHandlerList != NULL)
			{
				list<SDKEventHandle*>::iterator it = localHandlerList->begin();
				for (; it != localHandlerList->end();)
				{
					//É¾³ý
					auto handler = *it;
					bool needRemove = (handler != NULL && handler->canRemove);

					if (handler == NULL || needRemove)
					{
						if (needRemove)
							delete handler;
						it = localHandlerList->erase(it);
						continue;
					}

					if (handler->sdkDel != NULL)
					{
						handler->sdkDel(param);
					}

					++it;
				}
			}
		}
	}

	void SDKEventProcessor::ClearAll()
	{
		EventMapIter it = events.begin();
		for (; it != events.end(); ++it)
		{
			auto localHandlerList = it->second;
			if (localHandlerList == NULL)
				continue;

			_removeEventHandler(localHandlerList);
			delete localHandlerList;
		}
		events.clear();
	}

	void SDKEventProcessor::_removeEventHandler(EventList* handlerList)
	{
		if (handlerList == NULL)
			return;

		EventListIter it = handlerList->begin();
		for (; it != handlerList->end(); ++it)
		{
			delete* it;
		}

		handlerList->clear();
	}
}
