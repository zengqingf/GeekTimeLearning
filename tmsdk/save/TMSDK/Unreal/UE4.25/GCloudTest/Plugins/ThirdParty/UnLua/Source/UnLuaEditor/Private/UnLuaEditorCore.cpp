// Tencent is pleased to support the open source community by making UnLua available.
// 
// Copyright (C) 2019 THL A29 Limited, a Tencent company. All rights reserved.
//
// Licensed under the MIT License (the "License"); 
// you may not use this file except in compliance with the License. You may obtain a copy of the License at
//
// http://opensource.org/licenses/MIT
//
// Unless required by applicable law or agreed to in writing, 
// software distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and limitations under the License.

#include "UnLuaPrivate.h"
#include "Misc/FileHelper.h"
#include "Engine/Blueprint.h"
#include "Blueprint/UserWidget.h"
#include "Animation/AnimInstance.h"
#include "GameFramework/Actor.h"
#include "Interfaces/IPluginManager.h"

#include "Widgets/Notifications/SNotificationList.h"
#include "Framework/Notifications/NotificationManager.h"

//#include "UI/UserWidgetBase.h"

enum class CreateRes
{
    SUCCESS = 0,
    EXISTED,
    BASEFAILED,
    FAILED
};

void SpawnCreateFileNotification(CreateRes res,FString path)
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
    case CreateRes::BASEFAILED:
        NotificationString = TEXT("保存文件失败,蓝图基类不匹配");
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
}

bool SaveStringToFileWithNotification(const FString &content,const FString &name,FFileHelper::EEncodingOptions EncodingOptions)
{
    if(FFileHelper::SaveStringToFile(content, *name, EncodingOptions))
    {
        SpawnCreateFileNotification(CreateRes::SUCCESS,name);
        return true;
    }
    SpawnCreateFileNotification(CreateRes::FAILED,name);
    return false;
}

// create Lua template file for the selected blueprint
bool CreateLuaTemplateFile(UBlueprint *Blueprint)
{
    if (Blueprint)
    {
        UClass *Class = Blueprint->GeneratedClass;
        FString ClassName = Class->GetName();
        FString OuterPath = Class->GetPathName();
        int32 LastIndex;
        if (OuterPath.FindLastChar('/', LastIndex))
        {
            OuterPath = OuterPath.Left(LastIndex + 1);
        }
        OuterPath = OuterPath.RightChop(6);         // ignore "/Game/"
        FString FileName = FString::Printf(TEXT("%s%s%s.lua"), *GLuaSrcFullPath, *OuterPath, *ClassName);
        if (FPaths::FileExists(FileName))
        {
            UE_LOG(LogUnLua, Warning, TEXT("Lua file (%s) is already existed!"), *ClassName);
            SpawnCreateFileNotification(CreateRes::EXISTED,FileName);
            return false;
        }

        static FString ContentDir = IPluginManager::Get().FindPlugin(TEXT("UnLua"))->GetContentDir();

        FString TemplateName;
        if (Class->IsChildOf(AActor::StaticClass()))
        {
            // default BlueprintEvents for Actor
            TemplateName = ContentDir + TEXT("/ActorTemplate.lua");
        }
        else if (Class->IsChildOf(UUserWidget::StaticClass()))
        {
            // default BlueprintEvents for UserWidget (UMG)
            TemplateName = ContentDir + TEXT("/UserWidgetTemplate.lua");
        }
        else if (Class->IsChildOf(UAnimInstance::StaticClass()))
        {
            // default BlueprintEvents for AnimInstance (animation blueprint)
            TemplateName = ContentDir + TEXT("/AnimInstanceTemplate.lua");
        }
        else if (Class->IsChildOf(UActorComponent::StaticClass()))
        {
            // default BlueprintEvents for ActorComponent
            TemplateName = ContentDir + TEXT("/ActorComponentTemplate.lua");
        }

        FString Content;
        FFileHelper::LoadFileToString(Content, *TemplateName);
        Content = Content.Replace(TEXT("TemplateName"), *ClassName);
        if (FFileHelper::SaveStringToFile(Content, *FileName))
        {
            SpawnCreateFileNotification(CreateRes::SUCCESS,FileName);
            return true;
        }
        SpawnCreateFileNotification(CreateRes::FAILED,FileName); 
    }
    return false;
}

// create UserWidgetBase Lua template file for the selected blueprint
bool CreateUserWidgetBaseLuaTemplateFile(UBlueprint* Blueprint)
{
	if (Blueprint)
	{
		UClass* Class = Blueprint->GeneratedClass;
		
		FString ClassName = Class->GetName();
		FString OuterPath = Class->GetPathName();
		int32 LastIndex;
		if (OuterPath.FindLastChar('/', LastIndex))
		{
			OuterPath = OuterPath.Left(LastIndex + 1);
		}
		OuterPath = OuterPath.RightChop(6);         // ignore "/Game/"
		FString FileName = FString::Printf(TEXT("%s%s%s.lua"), *GLuaSrcFullPath, *OuterPath, *ClassName);
		bool isNormalExit = false;
		if (FPaths::FileExists(FileName))
		{
			UE_LOG(LogUnLua, Warning, TEXT("CreateUWB----------------- Lua file (%s) is already existed!"), *ClassName);
			isNormalExit = true;
		}

		FString FrameClassPathFileName = FString::Printf(TEXT("%sUI/UIFrameClassPath.lua"), *GLuaSrcFullPath);
		bool isNeedView = false;
		bool isFrameExit = false;

		/*
		if (Class->IsChildOf(UUserWidgetBase::StaticClass()))
		{
			isNeedView = true;
			FileName = FileName.Replace(TEXT("/Resources/"), TEXT("/"));
			FileName = FileName.Replace(TEXT("/Widgets/"), TEXT("/Frame/"));
			FileName = FileName.Replace(TEXT("_C.lua"), TEXT(".lua"));

			
			if (FPaths::FileExists(FileName))
			{
				UE_LOG(LogUnLua, Warning, TEXT("CreateUWB----------------- Lua file Frame (%s) is already existed!"), *ClassName);
				SpawnCreateFileNotification(CreateRes::EXISTED,FileName);
				isFrameExit = true;
			}
		}
		else
		{
			SpawnCreateFileNotification(CreateRes::BASEFAILED,OuterPath);
			return false;
		}

		bool isViewExit = false;
		FString FileNameView = FileName.Replace(TEXT(".lua"), TEXT("View_C.lua"));
		if (FPaths::FileExists(FileNameView))
		{
			UE_LOG(LogUnLua, Warning, TEXT("CreateUWB----------------- Lua file View (%s) is already existed!"), *ClassName);
			SpawnCreateFileNotification(CreateRes::EXISTED,FileNameView);
			isViewExit = true;
		}
		UE_LOG(LogUnLua, Log, TEXT("CreateUWB----------------- isNeedView:(%d) isViewExit:(%d)"), isNeedView, isViewExit);

		static FString ContentDir = IPluginManager::Get().FindPlugin(TEXT("UnLua"))->GetContentDir();
		UE_LOG(LogUnLua, Log, TEXT("CreateUWB----------------- ContentDir:(%s)  FileName(%s)"), *ContentDir, *FileName);

		FString TemplateName;
		FString TemplateNameView;
		if (Class->IsChildOf(AActor::StaticClass()))
		{
			// default BlueprintEvents for Actor
			TemplateName = ContentDir + TEXT("/ActorTemplate.lua");
		}
		else if (Class->IsChildOf(UUserWidgetBase::StaticClass()))
		{
			TemplateName = ContentDir + TEXT("/UserWidgetBaseFrameTemplate.lua");
			TemplateNameView = ContentDir + TEXT("/UserWidgetBaseViewTemplate.lua");
		}
		else if (Class->IsChildOf(UUserWidget::StaticClass()))
		{
			// default BlueprintEvents for UserWidget (UMG)
			TemplateName = ContentDir + TEXT("/UserWidgetTemplate.lua");
		}
		else if (Class->IsChildOf(UAnimInstance::StaticClass()))
		{
			// default BlueprintEvents for AnimInstance (animation blueprint)
			TemplateName = ContentDir + TEXT("/AnimInstanceTemplate.lua");
		}
		else if (Class->IsChildOf(UActorComponent::StaticClass()))
		{
			// default BlueprintEvents for ActorComponent
			TemplateName = ContentDir + TEXT("/ActorComponentTemplate.lua");
		}

		if (isNeedView)
		{
			FString nameTemp = FString::Printf(TEXT("%s.lua"), *ClassName);
			FString nameFrame = nameTemp.Replace(TEXT("_C.lua"), TEXT(""));

			FString ContentFramePath;
			FString framePathTitle = FString::Printf(TEXT("UIFrameClassPath.%s"), *nameFrame);
			FFileHelper::LoadFileToString(ContentFramePath, *FrameClassPathFileName);
			FString framePathTemp = FileName.Replace(*GLuaSrcFullPath, TEXT(""));
			framePathTemp = framePathTemp.Replace(TEXT(".lua"), TEXT(""));
			framePathTemp = FString::Printf(TEXT("UIFrameClassPath.%s = \"%s\"\n--luatemplete自动生成frame路径插入点(勿删)"), *nameFrame, *framePathTemp);
			if (ContentFramePath.Contains(framePathTitle))
			{
				UE_LOG(LogUnLua, Warning, TEXT("CreateUWB----------------- The type already exit  framePathTitle:(%s)"), *framePathTitle);
			}
			else
			{
				ContentFramePath = ContentFramePath.Replace(TEXT("--luatemplete自动生成frame路径插入点(勿删)"), *framePathTemp);
				SaveStringToFileWithNotification(ContentFramePath, *FrameClassPathFileName, FFileHelper::EEncodingOptions::ForceUTF8);
			}

			FString Content;
			FString blueprintPath = Class->GetPathName();
			if (blueprintPath.FindLastChar('/', LastIndex))
			{
				blueprintPath = blueprintPath.Left(LastIndex + 1);
			}
			blueprintPath = FString::Printf(TEXT("\"%s%s\" --蓝图文件路径"), *blueprintPath, *nameFrame);
			FFileHelper::LoadFileToString(Content, *TemplateName);
			Content = Content.Replace(TEXT("TemplateName"), *nameFrame);
			Content = Content.Replace(TEXT("--蓝图文件路径"), *blueprintPath);

			FString nameView = FString::Printf(TEXT("%sView_C"), *nameFrame);
			FString ContentView;
			FFileHelper::LoadFileToString(ContentView, *TemplateNameView);
			ContentView = ContentView.Replace(TEXT("TemplateName"), *nameView);

			if (!isViewExit && !isFrameExit)
			{
				UE_LOG(LogUnLua, Log, TEXT("CreateUWB----------------- CreateFrame:(%s)  CreateView:(%s) Success!"), *nameFrame, *nameView);
				return SaveStringToFileWithNotification(Content, *FileName, FFileHelper::EEncodingOptions::ForceUTF8) && SaveStringToFileWithNotification(ContentView, *FileNameView, FFileHelper::EEncodingOptions::ForceUTF8);
			}
			else if (!isViewExit)
			{
				UE_LOG(LogUnLua, Log, TEXT("CreateUWB----------------- CreateView:(%s) Success!"), *nameView);
				return SaveStringToFileWithNotification(ContentView, *FileNameView, FFileHelper::EEncodingOptions::ForceUTF8);
			}
			else if (!isFrameExit)
			{
				UE_LOG(LogUnLua, Log, TEXT("CreateUWB----------------- CreateFrame:(%s) Success!"), *nameFrame);
				return SaveStringToFileWithNotification(Content, *FileName, FFileHelper::EEncodingOptions::ForceUTF8);
			}
		}
		else if (!isNormalExit)
		{
			FString Content;
			FFileHelper::LoadFileToString(Content, *TemplateName);
			Content = Content.Replace(TEXT("TemplateName"), *ClassName);
			return SaveStringToFileWithNotification(Content, *FileName, FFileHelper::EEncodingOptions::ForceUTF8);
		}
		*/
	}
	return false;
}