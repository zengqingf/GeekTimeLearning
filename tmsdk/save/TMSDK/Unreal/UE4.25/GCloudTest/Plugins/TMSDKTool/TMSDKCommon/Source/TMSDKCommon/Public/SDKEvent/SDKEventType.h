// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#ifndef _SDKEVENTTYPE_H_
#define _SDKEVENTTYPE_H_

#include "CoreMinimal.h"
/*
ref: TenmoveBattle.BeEvent
*/
namespace TenmoveSDK
{
	enum class TMSDKCOMMON_API SDKEventType
	{
		None = 0,

		//获取屏幕点击
		GetTouchScreenPos,
		GetMultiTouchEvent,			//自定义多点触控

		//游戏更新SDK
		UpdateStart,				//一种更新类型的状态刷新
		UpdateProgress,
		UpdateFinish,
		UpdateError,
		AllUpdateEnd,				//全部更新类型的更新结束

		//游戏目录服（区服信息）
		DirPullFinish,
		DirPullError,
		
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
		/// 实时翻译结果 (string)filed 
		/// </summary>
		GVoiceRealTimeTranslateSucc,
		/// <summary>
		/// 处理语音消息成功，从这里拿fieldid，翻译，时长信息(上层要的信息)
		/// </summary>
		GVoiceDealInfoSucc,
	};
}

#endif //_SDKEVENTTYPE_H_