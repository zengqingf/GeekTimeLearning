// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

/**
 * 
 */
class BUGLY_API IBuglyAgent
{
public:
	virtual void InitReport() = 0;
	virtual void SetUserId(const FString& userIdddddddd) = 0;
};
