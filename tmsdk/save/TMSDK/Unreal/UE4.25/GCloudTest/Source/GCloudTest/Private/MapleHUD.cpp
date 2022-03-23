// Fill out your copyright notice in the Description page of Project Settings.


#include "MapleHUD.h"
#include "Kismet/GameplayStatics.h"

void AMapleHUD::BeginPlay()
{
	Super::BeginPlay();
	//__super::BeginPlay();

	UMapleView* widget = CreateWidget<UMapleView>(GetWorld(), mapleWidgetClass);
	if (widget != nullptr) {
		widget->AddToViewport();
	}

	//获取控制器
	APlayerController* mainPlayerCtrl = Cast<APlayerController>(UGameplayStatics::GetPlayerController(GetWorld(), 0));
	//设置鼠标输入模式
	if (mainPlayerCtrl != nullptr) {
		mainPlayerCtrl->bShowMouseCursor = true;
		FInputModeGameOnly inputMode;
		inputMode.SetConsumeCaptureMouseDown(true);
		mainPlayerCtrl->SetInputMode(inputMode);
	}
}