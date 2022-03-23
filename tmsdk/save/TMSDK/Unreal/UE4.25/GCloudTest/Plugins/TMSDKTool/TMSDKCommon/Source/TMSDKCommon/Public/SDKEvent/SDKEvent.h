// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#ifndef _SDKEVENT_H_
#define _SDKEVENT_H_

#include "CoreMinimal.h"

#include <functional>
#include <list>
#include <map>
#include <string>

/*
ref: TenmoveBattle.BeEvent
*/
namespace TenmoveSDK
{
	using std::string;
	using std::list;
	using std::map;

	struct TMSDKCOMMON_API SDKEventParam
	{
		string str_0;
		string str_1;
		string str_2;
		string str_3;
		int int_0 = 0;
		int int_1 = 0;
		float float_0 = 0.0f;
		float float_1 = 0.0f;
		bool bool_0 = false;
		bool bool_1 = false;
	};

	class SDKEventProcessor;
	class TMSDKCOMMON_API SDKEventHandle
	{
	public:
		typedef std::function<void(const SDKEventParam& param)>  SDKDel;
	public:
		SDKEventHandle(int sdkEventName, SDKDel sdkDel, SDKEventProcessor* sdkProcessor)
		{
			this->sdkEventName = sdkEventName;
			this->sdkDel = sdkDel;
			this->sdkProcessor = sdkProcessor;
		}
		void Remove()
		{
			if (sdkProcessor != nullptr)
			{
				canRemove = true;
			}
		}
		
		int EventName() { return sdkEventName; }
		bool CanRemove() { return canRemove; }
		SDKDel SDKDelegate() const { return sdkDel; }
	
	protected:
		int sdkEventName;
		SDKDel sdkDel = nullptr;
		bool canRemove = false;
		SDKEventProcessor* sdkProcessor = nullptr;
	};

	class TMSDKCOMMON_API SDKEventProcessor
	{
	public:
		typedef list<SDKEventHandle*> EventList;
		typedef EventList::iterator EventListIter;
		typedef map<int, EventList*>::iterator EventMapIter;
	public:
		SDKEventHandle* AddEventHandle(int sdkEventName, SDKEventHandle::SDKDel sdkDel);
		void RemoveHandler(SDKEventHandle* handler);
		void HandleEvent(int eventName, const SDKEventParam& eventParam);
		void ClearAll();
	private:
		void _removeEventHandler(EventList* handlerList);
	private:
		map<int, EventList*> m_events;
	};
}

#endif //_SDKEVENT_H_