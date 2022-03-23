// Fill out your copyright notice in the Description page of Project Settings.


#include "CreateLuaViewEditor.h"
#include "UnLuaInterface.h"
#include"Blueprint/IUserListEntry.h"

//#include "UI/UserWidgetBase.h"
#include "Animation/AnimInstance.h"
#include "Blueprint/UserWidget.h"
#include "Widgets/Layout/SScrollBox.h"
#include "Widgets/Layout/SExpandableArea.h"

#include "Widgets/DeclarativeSyntaxSupport.h"
#include "Widgets/Input/STextComboBox.h"
#include "Widgets/Input/SFilePathPicker.h"
#include "Widgets/Input/SDirectoryPicker.h"
#include "Framework/Notifications/NotificationManager.h"
#include "Interfaces/IPluginManager.h"
#include "Widgets/Notifications/SNotificationList.h"

#include "Kismet2/BlueprintEditorUtils.h"

// TSharedRef<CreateLuaViewEditor> CreateLuaViewEditor::New()
// {
// 	return MakeShareable(new CreateLuaViewEditor());
// }

#define LOCTEXT_NAMESPACE "UnLuaEditorCore"

void CreateLuaViewEditor::InitDefault()
{
	FString GLuaSrcRelativePath = TEXT("Data/Script/");
	mGLuaSrcFullPath = FPaths::ConvertRelativePathToFull(FPaths::ProjectContentDir() + GLuaSrcRelativePath);
	mContentDir = IPluginManager::Get().FindPlugin(TEXT("UnLua"))->GetContentDir();
	if(mBlueprint)
	{
		UClass *Class = mBlueprint->GeneratedClass;
		FString ClassName = Class->GetName();
		FString OuterPath = Class->GetPathName();
		if (!Class->ImplementsInterface(UUnLuaInterface::StaticClass()))
		{
			Class->Interfaces.Add(FImplementedInterface(UUnLuaInterface::StaticClass(), 0, false));
			FBlueprintEditorUtils::ImplementNewInterface(mBlueprint,FName(TEXT("UnLuaInterface")));  //add UnLuaInterface接口
			//FBlueprintEditorUtils::AddLocalVariable(mBlueprint)
		}
		// UFunction *Func = Class->FindFunctionByName(FName("GetModuleName"));    // find UFunction 'GetModuleName'. hard coded!!!
		// if (Func)
		// {
		// 	if (Func->GetNativeFunc() && IsInGameThread())
		// 	{
		// 		FString ModuleName;
		// 		UObject *DefaultObject = Class->GetDefaultObject();             // get CDO
		// 		DefaultObject->UObject::ProcessEvent(Func, &ModuleName);        // force to invoke UObject::ProcessEvent(...)
		// 		UClass *OuterClass = Func->GetOuterUClass();                    // get UFunction's outer class
		// 		//Class = OuterClass == InterfaceClass ? Class : OuterClass;      // select the target UClass to bind Lua module
		// 		if (ModuleName.Len() < 1)
		// 		{
		// 			ModuleName = Class->GetName();
		// 		}
		// 	}
		// }
		int32 LastIndex;
		if (OuterPath.FindLastChar('/', LastIndex))
		{
			OuterPath = OuterPath.Left(LastIndex + 1);
		}
		OuterPath = OuterPath.RightChop(6);         // ignore "/Game/"
		mLuaFilePath = FString::Printf(TEXT("%s%s"), *mGLuaSrcFullPath, *OuterPath);
		mLuaFilePath = mLuaFilePath.Replace(TEXT("/Resources/"), TEXT("/"));
		mLuaFilePath = mLuaFilePath.Replace(TEXT("/Widgets/"), TEXT("/Frame/"));
		//mLuaFilePath = FString::Printf(TEXT("%s%s"), *mGLuaSrcFullPath, *OuterPath);

		mLuaViewCFileName = FString::Printf(TEXT("%s.lua"),*ClassName);
		mLuaFrameFileName = mLuaViewCFileName.Replace(TEXT("View_C.lua"), TEXT(".lua"));
		mLuaFrameFileName = mLuaFrameFileName.Replace(TEXT("View"), TEXT(""));

		
		SetModuleNameValueAndEd();
	}
	//E:/_TMWorkSpace/A8/Client/NextGenGame/Content/Data/Script/Resources/UI/Widgets/ChatFrame/ChatFrameView_C.lua
	//ChatFrameView_C
}


void CreateLuaViewEditor::Construct(const FArguments& InArgs)
{
	mBlueprint = InArgs._Parameters;
	if (mBlueprint)
	{
		InitDefault();
		UClass *Class = mBlueprint->GeneratedClass;
		FString ClassName = Class->GetName();
		FString OuterPath = Class->GetPathName();
		this->ChildSlot
		[
			SNew(SScrollBox)
			+ SScrollBox::Slot()
			[
				SNew(SVerticalBox)
				+ SVerticalBox::Slot().AutoHeight().Padding(15,15,0,0)
				[
					SNew(SHorizontalBox)
					+SHorizontalBox::Slot().AutoWidth().HAlign(HAlign_Right)
					[
						SNew(STextBlock).Text(FText::FromString(FString::Printf(TEXT("蓝图文件:%s"),*OuterPath)))
					]
				]
				+ SVerticalBox::Slot().AutoHeight().Padding(15,15,0,0)
				[
					SNew(SHorizontalBox)
					+SHorizontalBox::Slot().AutoWidth().HAlign(HAlign_Right)
					[
						SNew(STextBlock).Text(FText::FromString(TEXT("GetModuleName: ")))
					]
					+SHorizontalBox::Slot().AutoWidth().HAlign(HAlign_Right)
					[
						SAssignNew(mModuleNameTextBlock, STextBlock).Text(FText::FromString(mModuleNameValue))
					]
					+SHorizontalBox::Slot().AutoWidth().HAlign(HAlign_Right)
					[
						SNew(SButton).Text(FText::FromString(TEXT("复制路径")))
						.OnClicked_Lambda([this]()
						{
							FWindowsPlatformMisc::ClipboardCopy(*mModuleNameValue);
							return FReply::Handled();
						})
					]
				]
				+SVerticalBox::Slot().Padding(15,15,0,0)
				[
					SNew(SHorizontalBox)
					+ SHorizontalBox::Slot().AutoWidth()
					[
						SNew(STextBlock).Text(FText::FromString(UTF8_TO_TCHAR("是否创建ViewC文件")))
					]+ SHorizontalBox::Slot().Padding(15,0,0,0)
					[
						SNew(SCheckBox).IsChecked(1).OnCheckStateChanged_Lambda([this](ECheckBoxState state)
						{
							mIsNeedCreateViewC = state == ECheckBoxState::Checked;
						})
					]
					+ SHorizontalBox::Slot().AutoWidth()
					[
						SNew(STextBlock).Text(FText::FromString(UTF8_TO_TCHAR("是否创建Frame文件")))
					]+ SHorizontalBox::Slot().Padding(15,0,0,0)
					[
						SNew(SCheckBox).IsChecked(1).OnCheckStateChanged_Lambda([this](ECheckBoxState state)
						{
							mIsNeedCreateFrame = state == ECheckBoxState::Checked;
						})
					]
				]
				+ SVerticalBox::Slot().AutoHeight().Padding(15,15,0,0)
				[
					SNew(SHorizontalBox)
					+SHorizontalBox::Slot().AutoWidth().HAlign(HAlign_Right)
					[
						SNew(STextBlock).Text(FText::FromString(FString::Printf(TEXT("保存文件路径: "))))
					]
					+ SHorizontalBox::Slot().FillWidth(0.5)
					[
						SNew(SDirectoryPicker).ToolTipText(FText::FromString(TEXT("打开目录")))
						.Directory(mLuaFilePath)
						.OnDirectoryChanged_Lambda([this](const FString& path)
						{
							mLuaFilePath = path + "/";
							SetModuleNameValueAndEd();
							UE_LOG(LogTemp,Warning,TEXT("mLuaFrameFilePath %s"),*mLuaFilePath);
						})
											
					]
				]
				+ SVerticalBox::Slot().AutoHeight().Padding(15,15,0,0)
				[
					SNew(SHorizontalBox)
					+SHorizontalBox::Slot().AutoWidth().HAlign(HAlign_Right)
					[
						SNew(STextBlock).Text(FText::FromString(TEXT("ViewC文件名: ")))
					]
					+ SHorizontalBox::Slot().AutoWidth().Padding(10,0,0,0)
					[
						SNew(SEditableTextBox).MinDesiredWidth(50).
						BackgroundColor(FSlateColor(FLinearColor(0.5f, 0.5f, 0.5f))).Text(FText::FromString(mLuaViewCFileName))
						.OnTextChanged_Lambda([this](const FText& text)
						{
							mLuaViewCFileName = text.ToString();
							SetModuleNameValueAndEd();
						})
					]
				]
				+ SVerticalBox::Slot().AutoHeight().Padding(15,15,0,0)
				[
					SNew(SHorizontalBox)
					+SHorizontalBox::Slot().AutoWidth().HAlign(HAlign_Right)
					[
						SNew(STextBlock).Text(FText::FromString(TEXT("Frame文件名: ")))
					]
					+ SHorizontalBox::Slot().AutoWidth().Padding(10,0,0,0)
					[
						SNew(SEditableTextBox).MinDesiredWidth(50).
						BackgroundColor(FSlateColor(FLinearColor(0.5f, 0.5f, 0.5f))).Text(FText::FromString(mLuaFrameFileName))
						.OnTextChanged_Lambda([this](const FText& text)
						{
							mLuaFrameFileName = text.ToString();
						})
					]
				]
				+SVerticalBox::Slot().AutoHeight().Padding(15,15,0,0)
				[
					SNew(SButton).Text(FText::FromString(TEXT("创建文件")))
					.OnClicked_Raw(this,&CreateLuaViewEditor::CreateLuaFile)
				]
			]
		];
	}

}
FReply CreateLuaViewEditor::CreateLuaFile()
{
	if(mBlueprint)
	{
		UClass* Class = mBlueprint->GeneratedClass;
		FString ViewCFileFUllPath = FString::Printf(TEXT("%s%s"),*mLuaFilePath,*mLuaViewCFileName); 
		FString FrameFileFUllPath = FString::Printf(TEXT("%s%s"),*mLuaFilePath,*mLuaFrameFileName);
		
		FString TemplateName;
		FString TemplateNameView;
		if (Class->IsChildOf(AActor::StaticClass()))
		{
			// default BlueprintEvents for Actor
			TemplateName = mContentDir + TEXT("/ActorTemplate.lua");
		}
		// else if (Class->IsChildOf(UUserWidgetBase::StaticClass()))
		// {
		// 	TemplateName = mContentDir + TEXT("/UserWidgetBaseFrameTemplate.lua");
		// 	TemplateNameView = mContentDir + TEXT("/UserWidgetBaseViewTemplate.lua");
		// }
		else if (Class->IsChildOf(UUserWidget::StaticClass()))
		{
			// default BlueprintEvents for UserWidget (UMG)
			TemplateName = mContentDir + TEXT("/UserWidgetTemplate.lua");
			TemplateNameView = mContentDir + TEXT("/UserWidgetTemplate.lua");
		}
		else if (Class->IsChildOf(UAnimInstance::StaticClass()))
		{
			// default BlueprintEvents for AnimInstance (animation blueprint)
			TemplateName = mContentDir + TEXT("/AnimInstanceTemplate.lua");
		}
		else if (Class->IsChildOf(UActorComponent::StaticClass()))
		{
			// default BlueprintEvents for ActorComponent
			TemplateName = mContentDir + TEXT("/ActorComponentTemplate.lua");
		}
		if (mIsNeedCreateViewC)
		{
			if(FPaths::FileExists(ViewCFileFUllPath))
			{
				SpawnCreateFileNotification(CreateRes::EXISTED,ViewCFileFUllPath);
			}
			else
			{
				FString nameView = mLuaViewCFileName.Replace(TEXT(".lua"), TEXT(""));
				//生成ViewC文件
				FString ContentView;
				FFileHelper::LoadFileToString(ContentView, *TemplateNameView);
				ContentView = ContentView.Replace(TEXT("TemplateName"), *nameView);
				SaveStringToFileWithNotification(ContentView, *ViewCFileFUllPath,FFileHelper::EEncodingOptions::ForceUTF8);
			}
		}
		if (mIsNeedCreateFrame)
		{
			// if(!Class->IsChildOf(UUserWidgetBase::StaticClass()))
			// {
			// 	SpawnCreateFileNotification(CreateRes::BASECLASSFAILED,Class->GetPathName());
			// 	return FReply::Handled();
			// }
			if(FPaths::FileExists(FrameFileFUllPath))
			{
				SpawnCreateFileNotification(CreateRes::EXISTED,FrameFileFUllPath);
			}
			else
			{
				FString FrameClassPathFileName = FString::Printf(TEXT("%sUI/UIFrameClassPath.lua"), *mGLuaSrcFullPath);
				//UIFrameClassPath脚本中插入frame路径
				FString nameFrame = mLuaFrameFileName.Replace(TEXT(".lua"), TEXT(""));
				FString FramePathTitle = FString::Printf(TEXT("UIFrameClassPath.%s"), *nameFrame);
				FString FramePathContent;
				FFileHelper::LoadFileToString(FramePathContent, *FrameClassPathFileName);
				FString FramePathTemp = FrameFileFUllPath.Replace(*mGLuaSrcFullPath, TEXT(""));
				FramePathTemp = FramePathTemp.Replace(TEXT(".lua"), TEXT(""));
				FramePathTemp = FString::Printf(TEXT("UIFrameClassPath.%s = \"%s\"\n--luatemplete自动生成frame路径插入点(勿删)"), *nameFrame, *FramePathTemp);
				if (FramePathContent.Contains(FramePathTitle))
				{
					UE_LOG(LogTemp, Warning, TEXT(" UIFrameClassPath 已经存在路径 :(%s)"), *FramePathTitle);
				}
				else
				{
					FramePathContent = FramePathContent.Replace(TEXT("--luatemplete自动生成frame路径插入点(勿删)"), *FramePathTemp);
					SaveStringToFileWithNotification(FramePathContent, *FrameClassPathFileName, FFileHelper::EEncodingOptions::ForceUTF8);
				}
				//生成Frame文件
				FString Content;
				FString nameview = Class->GetName();
				int32 LastIndex;
				FString blueprintPath = Class->GetPathName();
				if (blueprintPath.FindLastChar('/', LastIndex))
				{
					blueprintPath = blueprintPath.Left(LastIndex + 1);
				}
				nameview = nameview.Replace(TEXT("_C"), TEXT(""));
				blueprintPath = FString::Printf(TEXT("\"%s%s\" --蓝图文件路径"), *blueprintPath, *nameview);
				FFileHelper::LoadFileToString(Content, *TemplateName);
				Content = Content.Replace(TEXT("TemplateName"), *nameFrame);
				Content = Content.Replace(TEXT("--蓝图文件路径"), *blueprintPath);
				SaveStringToFileWithNotification(Content, *FrameFileFUllPath, FFileHelper::EEncodingOptions::ForceUTF8);
			}
		}
	}
	SetModuleNameValueAndEd();
	return FReply::Handled();
	
}

bool CreateLuaViewEditor::ImplementLuaInterface(UBlueprint* Blueprint, const FName& InterfaceClassName)
{
	return true;
}

inline void CreateLuaViewEditor::SetModuleNameValueAndEd()
{
	mModuleNameValue = FString::Printf(TEXT("%s%s"),*mLuaFilePath,*mLuaViewCFileName);
	mModuleNameValue = mModuleNameValue.Replace(TEXT("/"),TEXT("."));

	FString L;
	FString R;
	if (mModuleNameValue.Split(TEXT("Data.Script."),&L,&R))
	{
		mModuleNameValue = R;
	}
	if (mModuleNameValue.Contains(TEXT(".lua")))
	{
		mModuleNameValue = mModuleNameValue.Replace(TEXT(".lua"),TEXT(""));
	}
	if (mModuleNameTextBlock.IsValid())
	{
		mModuleNameTextBlock->SetText(FText::FromString(mModuleNameValue));
	}
}

void CreateLuaViewEditor::SpawnCreateFileNotification(CreateRes res,FString path)
{
	FString NotificationString;
	switch (res)
	{
	case CreateRes::SUCCESS:
		NotificationString = TEXT("保存文件成功");
		break;
	case CreateRes::EXISTED:
		NotificationString = TEXT("文件已经存在");
		break;
	case CreateRes::BASECLASSFAILED:
		NotificationString = TEXT("蓝图基类不匹配，需要继承UserWidgetBase");
		break;
	case CreateRes::FAILED:
		NotificationString = TEXT("保存文件失败");
		break;
	}
	FText NotificationMessage = FText::FromString(FString::Printf(TEXT("%s\n%s"),*NotificationString,*path));
	FNotificationInfo Info(NotificationMessage);
	Info.ExpireDuration = 8.0f;
	Info.bUseLargeFont = false;
	
	FSlateNotificationManager::Get().AddNotification(Info);
	//FPlatformProcess::ExploreFolder(*path);
}

bool CreateLuaViewEditor::SaveStringToFileWithNotification(const FString &content,const FString &name,FFileHelper::EEncodingOptions EncodingOptions)
{
	 if(FFileHelper::SaveStringToFile(content, *name, EncodingOptions))
	 {
	 	SpawnCreateFileNotification(CreateRes::SUCCESS,name);
	 	return true;
	 }
	 SpawnCreateFileNotification(CreateRes::FAILED,name);
	return false;
}

#undef LOCTEXT_NAMESPACE

