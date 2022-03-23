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

#include "UnLuaEditorStyle.h"
#include "Interfaces/IPluginManager.h"
#include "Styling/SlateStyleRegistry.h"

#define IMAGE_BRUSH(RelativePath, ...) FSlateImageBrush(RootToContentDir(RelativePath, TEXT(".png")), __VA_ARGS__)
#define BOX_BRUSH(RelativePath, ...) FSlateBoxBrush(RootToContentDir(RelativePath, TEXT(".png")), __VA_ARGS__)
#define BORDER_BRUSH(RelativePath, ...) FSlateBorderBrush(RootToContentDir(RelativePath, TEXT(".png")), __VA_ARGS__)
#define TTF_FONT(RelativePath, ...) FSlateFontInfo(RootToContentDir(RelativePath, TEXT(".ttf")), __VA_ARGS__)
#define OTF_FONT(RelativePath, ...) FSlateFontInfo(RootToContentDir(RelativePath, TEXT(".otf")), __VA_ARGS__)

FUnLuaEditorStyle::FUnLuaEditorStyle()
    : FSlateStyleSet("UnLuaEditorStyle")
{
    const FVector2D Icon40x40(40.0f, 40.0f);

    SetContentRoot(IPluginManager::Get().FindPlugin("UnLua")->GetBaseDir() / TEXT("Resources"));

    Set("UnLuaEditor.CreateLuaTemplate", new IMAGE_BRUSH("icon_luatemplate_40x", Icon40x40));

    Set("UnLuaEditor.CreateUserWidgetBaseLuaTemplate", new IMAGE_BRUSH("icon_luatemplate_40x", Icon40x40));

    FSlateStyleRegistry::RegisterSlateStyle(*this);
}

FUnLuaEditorStyle::~FUnLuaEditorStyle()
{
    FSlateStyleRegistry::UnRegisterSlateStyle(*this);
}

#undef IMAGE_BRUSH
#undef BOX_BRUSH
#undef BORDER_BRUSH
#undef TTF_FONT
#undef OTF_FONT
