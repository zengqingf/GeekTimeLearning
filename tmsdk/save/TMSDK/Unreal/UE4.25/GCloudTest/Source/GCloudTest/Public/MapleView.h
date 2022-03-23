// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Blueprint/UserWidget.h"
#include "Components/Button.h"
#include "Components/CanvasPanel.h"
#include "MapleView.generated.h"

DECLARE_EVENT(UMapleView, FQueryDirService)

/**
 * 
 */
UCLASS()
class GCLOUDTEST_API UMapleView : public UUserWidget
{
	GENERATED_BODY()
	
public:
	UMapleView(const FObjectInitializer& objInitializer);

	FQueryDirService OnQueryDirService;

	virtual bool Initialize() override;

protected:

	virtual void NativeConstruct() override;

	UFUNCTION()
	void onQueryDirBtnClick();

	UFUNCTION()
	void onTestOneBtnClick();

	UFUNCTION()
	void onTestTwoBtnClick();

public:

	//绑定的类型和名称必须和蓝图内的一致
	UPROPERTY(Meta=(BindWidget))
	UButton *testOneBtn;

	UCanvasPanel *rootPanel;
};
