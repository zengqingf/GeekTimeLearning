// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Blueprint/UserWidget.h"
#include "GameUpdateView.generated.h"

/**
 * 
 */
UCLASS()
class GCLOUDTEST_API UGameUpdateView : public UUserWidget
{
	GENERATED_BODY()

public:
	FORCEINLINE class UProgressBar* GetUpdateProgtessBar() { return updateProgressBar; }

	void SetUpdateProgressBar(float percent);
	void SetUpdateProgressDesc(const FString& str);
	void SetUpdateProgressPercent(const FString& str);

protected:

	UPROPERTY(meta = (BindWidget))
		class UProgressBar* updateProgressBar;

	UPROPERTY(meta = (BindWidget))
		class UTextBlock* updateProgressDesc;

	UPROPERTY(meta = (BindWidget))
		class UTextBlock* updateProgressPercent;
};
