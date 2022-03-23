// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

#include <functional>
#include <list>
#include <map>

using namespace std;

namespace TenmoveSDK 
{
	enum class SDKEventType
	{
		
		//GVoiceRealTime
		/// <summary>
		/// 加入房间 (string)roomname ,(int)memberid
		/// </summary>
		GVoiceJoinRoomSucc,
		/// <summary>
		/// 退出房间 (string)roomname
		/// </summary>
		GVoiceQuitRoomSucc,
		/// <summary>
		/// 房间成员状态改变 (string)roomname,(int)memberid,(int)status(0：从说话状态转为非说话状态；1：从非说话状态进入说话状态；2：一直处于说话状态)
		/// </summary>
		GVoiceMemberSlateChanger,
		//GVoiceMessage
		GVoiceUploadSucc,
		GVoiceDownloadSucc,
		/// <summary>
		/// 翻译结果 (string)filed ,(string)translate info
		/// </summary>
		GVoiceTranslateSucc,
		/// <summary>
		/// 翻译结果 (string)filed 
		/// </summary>
		GVoiceNoTranslateSucc,
		/// <summary>
		/// 处理语音消息成功，从这里拿fieldid，翻译，时长信息
		/// </summary>
		GVoiceDealInfoSucc,
	};

	struct SDKEventParam
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
	class SDKEventHandle
	{
	public:
		typedef std::function<void(SDKEventParam& param)>  SDKDel;
	public:
		SDKEventHandle(int sdkEventName, SDKDel sdkDel, SDKEventProcessor* sdkProcessor)
		{
			this->sdkEventName = sdkEventName;
			this->sdkDel = sdkDel;
			this->sdkProcessor = sdkProcessor;
		}
		void Remove()
		{
			if (sdkProcessor != NULL)
			{
				canRemove = true;
				//processor.RemoveHandler(this);
			}
		}
	public:
		int sdkEventName;
		SDKDel sdkDel = NULL;
		bool canRemove = false;
		SDKEventProcessor* sdkProcessor = NULL;
	};

	class SDKEventProcessor
	{
	public:
		typedef list<SDKEventHandle*> EventList;
		typedef list<SDKEventHandle*>::iterator EventListIter;
		typedef map<int, EventList*>::iterator EventMapIter;
	public:
		SDKEventHandle* AddSDKEventHandle(int sdkEventName, SDKEventHandle::SDKDel sdkDel);
		void RemoveSDKEventHandle(int sdkEventName, SDKEventHandle::SDKDel sdkDel);
		void RemoveHandler(SDKEventHandle* handler);
		void HandleEvent(int eventName, SDKEventParam& param);
		void ClearAll();
	protected:
		void _removeEventHandler(EventList* handlerList);

	public:
		map<int, list<SDKEventHandle*>*> events;
	};
}
