// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Blueprint/UserWidget.h"
#include "RngWidget.generated.h"

/**
 * 
 */
UCLASS()
class GCLOUDTEST_API URngWidget : public UUserWidget
{
	GENERATED_BODY()
	

protected:
	virtual void NativeConstruct() override;


	//添加class后， 可以加引用头文件移到cpp中
	UPROPERTY( meta = ( BindWidget ) )
	class UTextBlock* randomNumberLabel;

	//添加class后， 可以加引用头文件移到cpp中
	UPROPERTY(meta = (BindWidget))
	class UButton* generateBtn;

	UPROPERTY(Meta = (BindWidget))
	class UTextBlock* TextVersionName;

	UFUNCTION()
	void onGenerateBtnClick();


	void generateRandom();
};
