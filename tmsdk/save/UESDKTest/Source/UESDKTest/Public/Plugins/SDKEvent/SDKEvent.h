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
		/// ���뷿�� (string)roomname ,(int)memberid
		/// </summary>
		GVoiceJoinRoomSucc,
		/// <summary>
		/// �˳����� (string)roomname
		/// </summary>
		GVoiceQuitRoomSucc,
		/// <summary>
		/// �����Ա״̬�ı� (string)roomname,(int)memberid,(int)status(0����˵��״̬תΪ��˵��״̬��1���ӷ�˵��״̬����˵��״̬��2��һֱ����˵��״̬)
		/// </summary>
		GVoiceMemberSlateChanger,
		//GVoiceMessage
		GVoiceUploadSucc,
		GVoiceDownloadSucc,
		/// <summary>
		/// ������ (string)filed ,(string)translate info
		/// </summary>
		GVoiceTranslateSucc,
		/// <summary>
		/// ������ (string)filed 
		/// </summary>
		GVoiceNoTranslateSucc,
		/// <summary>
		/// ����������Ϣ�ɹ�����������fieldid�����룬ʱ����Ϣ
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
