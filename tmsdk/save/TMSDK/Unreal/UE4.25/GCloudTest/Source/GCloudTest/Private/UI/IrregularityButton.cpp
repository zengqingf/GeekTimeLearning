// Fill out your copyright notice in the Description page of Project Settings.


#include "UI/IrregularityButton.h"

#define LOCTEXT_NAMESPACE "UMG"

#pragma region >>> SIrregularityButton <<<
FReply SIrregularityButton::OnMouseMove(const FGeometry& MyGeometry, const FPointerEvent& MouseEvent)
{
	if (nullptr == AdvancedHitTexture || nullptr == AdvancedHitTexture->PlatformData)
		return SButton::OnMouseMove(MyGeometry, MouseEvent);

	bool moveLeave = false;
	FVector2D localPos = MyGeometry.AbsoluteToLocal(MouseEvent.GetScreenSpacePosition());
	localPos.X = floor(localPos.X);
	localPos.Y = floor(localPos.Y);
	localPos /= MyGeometry.GetLocalSize();
	int imgWidth = AdvancedHitTexture->PlatformData->SizeX;
	localPos.X *= imgWidth;
	localPos.Y *= AdvancedHitTexture->PlatformData->SizeY;
	int bufPos = (localPos.Y * imgWidth) + localPos.X;
	FColor* imgColor = static_cast<FColor*>((AdvancedHitTexture->PlatformData->Mips[0]).BulkData.Lock(LOCK_READ_ONLY));
	if (nullptr == imgColor)
	{
		moveLeave = true;
	}
	else
	{
		if (imgColor[bufPos].A <= AdvancedHitAlpha)
		{
			moveLeave = true;
		}
	}

	AdvancedHitTexture->PlatformData->Mips[0].BulkData.Unlock();
	if (moveLeave != bIsHovered)
	{
		bIsHovered = !moveLeave;
		if (bIsHovered)
		{
			SButton::OnMouseEnter(MyGeometry, MouseEvent);
		}
		else
		{
			SButton::OnMouseLeave(MouseEvent);
		}
	}
	return SButton::OnMouseMove(MyGeometry, MouseEvent);
}

FCursorReply SIrregularityButton::OnCursorQuery(const FGeometry& MyGeometry, const FPointerEvent& CursorEvent) const
{
	if (!bIsHovered)
		return FCursorReply::Unhandled();

	TOptional<EMouseCursor::Type> theCursor = GetCursor();
	return (theCursor.IsSet()) ? FCursorReply::Cursor(theCursor.GetValue()) : FCursorReply::Unhandled();
}

#pragma endregion


#pragma region >>> UIrregularityButton <<<
void UIrregularityButton::SynchronizeProperties()
{
	Super::SynchronizeProperties();
	(static_cast<SIrregularityButton*>(MyButton.Get()))->SetAdvanceHitTexture(AdvancedHitTexture);
	(static_cast<SIrregularityButton*>(MyButton.Get()))->SetAdvancedHitAlpha(AdvancedHitAlpha);
}

TSharedRef<SWidget> UIrregularityButton::RebuildWidget()
{
	TSharedRef<SIrregularityButton> irregularBtn = SNew(SIrregularityButton)
		.OnClicked(BIND_UOBJECT_DELEGATE(FOnClicked, SlateHandleClicked))
		.OnPressed(BIND_UOBJECT_DELEGATE(FSimpleDelegate, SlateHandlePressed))
		.OnReleased(BIND_UOBJECT_DELEGATE(FSimpleDelegate, SlateHandleReleased))
		.OnHovered_UObject(this, &ThisClass::SlateHandleHovered)
		.OnUnhovered_UObject(this, &ThisClass::SlateHandleUnhovered)
		.ButtonStyle(&WidgetStyle)
		.ClickMethod(ClickMethod)
		.TouchMethod(TouchMethod)
		.IsFocusable(IsFocusable)
		;

	irregularBtn->SetAdvanceHitTexture(AdvancedHitTexture);
	irregularBtn->SetAdvancedHitAlpha(AdvancedHitAlpha);
	MyButton = irregularBtn;
	if (GetChildrenCount() > 0)
	{
		Cast<UButtonSlot>(GetContentSlot())->BuildSlot(MyButton.ToSharedRef());
	}
	return MyButton.ToSharedRef();
}
#pragma endregion