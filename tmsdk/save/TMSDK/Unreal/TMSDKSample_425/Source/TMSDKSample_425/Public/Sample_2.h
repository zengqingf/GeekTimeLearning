// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "TMSDKEventManager.h"

using namespace TenmoveSDK;

typedef TJsonWriterFactory< TCHAR, TCondensedJsonPrintPolicy<TCHAR> > FCondensedJsonStringWriterFactory;
typedef TJsonWriter< TCHAR, TCondensedJsonPrintPolicy<TCHAR> > FCondensedJsonStringWriter;

typedef TJsonWriterFactory< TCHAR, TPrettyJsonPrintPolicy<TCHAR> > FPrettyJsonStringWriterFactory;
typedef TJsonWriter< TCHAR, TPrettyJsonPrintPolicy<TCHAR> > FPrettyJsonStringWriter;

/**
 * 
 */
class TMSDKSAMPLE_425_API Sample_2
{
public:
	Sample_2();
	~Sample_2();

	void Init();
	void Test1();
	void Test2();
	void Test3();

private:
	void _onTouchScreenPos(const SDKEventParam& param);
};