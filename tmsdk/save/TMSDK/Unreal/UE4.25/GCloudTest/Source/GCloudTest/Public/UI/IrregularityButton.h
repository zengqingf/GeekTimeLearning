// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Runtime/UMG/Public/UMG.h"
#include "Runtime/UMG/Public/UMGStyle.h"
#include "Components/Button.h"
#include "IrregularityButton.generated.h"


class GCLOUDTEST_API SIrregularityButton : public SButton
{
public:
	SIrregularityButton() : AdvancedHitTexture(nullptr), AdvancedHitAlpha(0) {}

	void SetAdvanceHitTexture(UTexture2D* inTexture)
	{
		AdvancedHitTexture = inTexture;
	}

	void SetAdvancedHitAlpha(int inAlpha)
	{
		AdvancedHitAlpha = inAlpha;
	}

	virtual FReply OnMouseButtonDown(const FGeometry& MyGeometry, const FPointerEvent& MouseEvent) override
	{
		if (!bIsHovered)
			return FReply::Unhandled();
		return SButton::OnMouseButtonDown(MyGeometry, MouseEvent);
	}

	virtual FReply OnMouseButtonDoubleClick(const FGeometry& InMyGeometry, const FPointerEvent& InMouseEvent) override
	{
		if (!bIsHovered) 
			return FReply::Unhandled();
		return SButton::OnMouseButtonDoubleClick(InMyGeometry, InMouseEvent);
	}

	virtual FReply OnMouseButtonUp(const FGeometry& MyGeometry, const FPointerEvent& MouseEvent) override
	{
		if (!bIsHovered) 
			return FReply::Unhandled();
		return SButton::OnMouseButtonUp(MyGeometry, MouseEvent);
	}

	virtual FReply OnMouseMove(const FGeometry& MyGeometry, const FPointerEvent& MouseEvent) override;

	virtual void OnMouseEnter(const FGeometry& MyGeometry, const FPointerEvent& MouseEvent) override
	{
		if (AdvancedHitTexture) 
			return;
		return SButton::OnMouseEnter(MyGeometry, MouseEvent);
	}

	virtual void OnMouseLeave(const FPointerEvent& MouseEvent) override
	{
		return SButton::OnMouseLeave(MouseEvent);
	}

	virtual FCursorReply OnCursorQuery(const FGeometry& MyGeometry, const FPointerEvent& CursorEvent) const override;

	virtual TSharedPtr<IToolTip> GetToolTip() override 
	{ 
		return (bIsHovered ? SWidget::GetToolTip() : NULL); 
	}

public:
	UTexture2D* AdvancedHitTexture;
	int AdvancedHitAlpha;
};

/**
 * 
 */
UCLASS()
class GCLOUDTEST_API UIrregularityButton : public UButton
{
	GENERATED_BODY()


public:
	UIrregularityButton(const FObjectInitializer& ObjectInitializer) : AdvancedHitTexture(nullptr), AdvancedHitAlpha(0), Super(ObjectInitializer)
	{}

	UFUNCTION(BlueprintCallable, Category = "AdvanceHitTest")
	void SetAdvancedHitTexture(UTexture2D* InTexture)
	{
		AdvancedHitTexture = InTexture;
		if (MyButton.IsValid())
		{
			static_cast<SIrregularityButton*>(MyButton.Get())->SetAdvanceHitTexture(AdvancedHitTexture);
		}
	}

	UFUNCTION(BlueprintCallable, Category = "AdvanceHitTest")
	void SetAdvancedHitAlpha(int InAlpha)
	{
		AdvancedHitAlpha = InAlpha;
		if (MyButton.IsValid())
		{
			static_cast<SIrregularityButton*>(MyButton.Get())->SetAdvancedHitAlpha(AdvancedHitAlpha);
		}
	}

	virtual void SynchronizeProperties() override;
	virtual TSharedRef<SWidget> RebuildWidget() override;


public:
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category="AdvancedHitTest")
	UTexture2D* AdvancedHitTexture;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category="AdvancedHitTest", meta=(ClampMin = "0.0", ClampMax = "255.0", UIMin = "0.0", UIMax = "255.0"))
	int AdvancedHitAlpha;
};
