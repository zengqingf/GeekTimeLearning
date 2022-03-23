// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Kismet/BlueprintFunctionLibrary.h"
#include "HelloLuaUtils.generated.h"

/**
 * 
 */
UCLASS()
class GCLOUDTEST_API UHelloLuaUtils final : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

	UFUNCTION(BlueprintCallable)
	static int GetInt();
};
