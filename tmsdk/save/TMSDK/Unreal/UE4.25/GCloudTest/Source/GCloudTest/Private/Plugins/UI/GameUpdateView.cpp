// Fill out your copyright notice in the Description page of Project Settings.


#include "Plugins/UI/GameUpdateView.h"
#include "Components/TextBlock.h"
#include "Components/ProgressBar.h"

void UGameUpdateView::SetUpdateProgressBar(float percent)
{
	if (updateProgressBar)
		updateProgressBar->SetPercent(percent);
}

void UGameUpdateView::SetUpdateProgressDesc(const FString& str)
{
	if (updateProgressDesc)
		updateProgressDesc->SetText(FText::FromString(str));
}

void UGameUpdateView::SetUpdateProgressPercent(const FString& str)
{
	if (updateProgressPercent)
		updateProgressPercent->SetText(FText::FromString(str));
}

