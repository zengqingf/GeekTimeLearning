// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Widgets/SUserWidget.h"
#include "Misc/FileHelper.h"


enum class CreateRes
{
	SUCCESS = 0,
	EXISTED,
	BASECLASSFAILED,
	FAILED
};

class CreateLuaViewEditor : public SCompoundWidget
{
public:
	SLATE_BEGIN_ARGS(CreateLuaViewEditor)
	{}

	SLATE_ARGUMENT(UBlueprint*, Parameters)
	SLATE_ARGUMENT(TSharedPtr<SWindow>, WidgetWindow)
	SLATE_END_ARGS()

	void Construct(const FArguments& InArgs);

private:
	void InitDefault();
	bool SaveStringToFileWithNotification(const FString &content,const FString &name,FFileHelper::EEncodingOptions EncodingOptions);
	void SpawnCreateFileNotification(CreateRes res,FString path);
	FReply CreateLuaFile();
	bool ImplementLuaInterface(UBlueprint* Blueprint, const FName& InterfaceClassName);
	void SetModuleNameValueAndEd();
private:
	UBlueprint* mBlueprint;
	TWeakPtr< SWindow > mWindow;
	TSharedPtr<STextBlock> mModuleNameTextBlock;
	FString mModuleNameValue;
	FString mLuaFilePath;
	FString mLuaViewCFileName;
	//FString mLuaFrameFilePath;
	FString mLuaFrameFileName;
	bool mIsNeedCreateViewC = true;
	bool mIsNeedCreateFrame = true;
	FString mGLuaSrcFullPath;
	FString mContentDir;
};
