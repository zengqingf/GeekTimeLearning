// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

#include "IBuglyAgent.h"

/**
 * 
 */
class FBuglyAgent : public IBuglyAgent
{
public:
	FBuglyAgent();
	virtual ~FBuglyAgent() {}

	virtual void InitReport() override;
	virtual void SetUserId(const FString& userId) override;

private:
	FString appId;
	bool bDebug;
};
