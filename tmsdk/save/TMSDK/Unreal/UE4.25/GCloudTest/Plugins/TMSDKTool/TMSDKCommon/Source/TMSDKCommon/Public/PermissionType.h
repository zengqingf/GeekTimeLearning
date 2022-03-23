// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#ifndef _PERMISSION_TYPE_H_
#define _PERMISSION_TYPE_H_

#include "CoreMinimal.h"
/*
ref: TenmoveBattle.BeEvent
*/
namespace TenmoveSDK
{
	enum class TMSDKCOMMON_API PermissionType
	{
		None = 0,

		RecordVoice,
		SDCard,
	};
}

#endif //_PERMISSION_TYPE_H_