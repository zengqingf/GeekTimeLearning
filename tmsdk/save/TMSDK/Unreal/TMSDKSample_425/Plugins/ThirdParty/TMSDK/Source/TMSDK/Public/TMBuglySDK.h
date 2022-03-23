// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#ifndef _TMBUGLYSDK_H_
#define _TMBUGLYSDK_H_

#include "CoreMinimal.h"
#include "ITMSDK.h"

/**
 * 
 */
class TMSDK_API TMBuglySDK : public ITMSDK
{
public:
	TMBuglySDK();
	~TMBuglySDK();

	 void Init() override;
	 void UnInit() override;
	 FString Version() const override;

	 void SetUserId(const FString& userId);
};


#endif //_TMBUGLYSDK_H_