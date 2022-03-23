// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#ifndef _ITMSDK_H_
#define _ITMSDK_H_

#include "CoreMinimal.h"
/**
 * 
 */
class TMSDK_API ITMSDK
{
public:
	virtual ~ITMSDK() {}
	virtual void Init() = 0;
	virtual void UnInit() = 0;
	virtual FString Version() const = 0;
};


#endif //_ITMSDK_H_