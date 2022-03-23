// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Kismet/BlueprintFunctionLibrary.h"
#include "UWAEnum.h"
#include "UWABlueprintFunctionLibrary.generated.h"

/**
* This function library is used for blueprint.
*/
UCLASS()
class UWAGOT_API UUWABlueprintFunctionLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()


	UFUNCTION(BlueprintCallable, Category = "UWAGOT")
		static bool SupportMode(EProfileMode Mode);

	UFUNCTION(BlueprintCallable, Category = "UWAGOT")
		static void SetMode(EProfileMode Mode);

	UFUNCTION(BlueprintCallable, Category = "UWAGOT")
		static void Start();

	UFUNCTION(BlueprintCallable, Category = "UWAGOT")
		static void Stop();

	UFUNCTION(BlueprintCallable, Category = "UWAGOT")
		static void Register();

	UFUNCTION(BlueprintCallable, Category = "UWAGOT")
		static bool WithEditor();
};
