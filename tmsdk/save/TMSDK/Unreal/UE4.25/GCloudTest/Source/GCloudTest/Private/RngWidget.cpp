// Fill out your copyright notice in the Description page of Project Settings.


#include "RngWidget.h"
#include "Components/Button.h"
#include "Components/TextBlock.h"
#include "Plugins/TMSDKDefine.h"
#include "Plugins/GCloud//GCloudManager.h"

void URngWidget::NativeConstruct()
{
	Super::NativeConstruct();

	generateRandom();

	generateBtn->OnClicked.AddUniqueDynamic(this, &URngWidget::onGenerateBtnClick);

	//展示版本号
	FString versions = FString::Printf(TEXT("app:v_%s res:v_%s"), *(PluginManager::Instance().GetVersionName()), *(PluginManager::Instance().GCloudMgr().GetResVersionFromConfig()));
	TextVersionName->SetText(FText::FromString(*versions));
}

void URngWidget::onGenerateBtnClick()
{
	generateRandom();
}

void URngWidget::generateRandom()
{
	int32 randomNum = FMath::RandRange(0, 100);
	randomNumberLabel->SetText(FText::AsNumber(randomNum));
}

