// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Blueprint/UserWidget.h"
#include "DeviceInfoWidgetBase.generated.h"

/**
 * 
 */
UCLASS()
class UESDKTEST_API UDeviceInfoWidgetBase : public UUserWidget
{
	GENERATED_BODY()

public:
	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Transient)
	FString TimeString;

	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Transient)
	FString NetWorkTypeStr;

public:
	UFUNCTION(BlueprintCallable, Category="DeviceInfo")
	FString GetNetWorkType();

	UFUNCTION(BlueprintCallable, Category = "DeviceInfo")
	FString GetTime();

	UFUNCTION(BlueprintCallable, Category = "DeviceInfo")
	int32 GetBatteryLevel();



	
};
