// Fill out your copyright notice in the Description page of Project Settings.

#include "UWABlueprintFunctionLibrary.h"
#include "ProfileManager.h"

void UUWABlueprintFunctionLibrary::SetMode(EProfileMode Mode)
{
	UProfileManager::Get()->SetMode(Mode);
}

bool UUWABlueprintFunctionLibrary::SupportMode(EProfileMode Mode)
{
	return UProfileManager::Get()->SupportMode(Mode);
}

void UUWABlueprintFunctionLibrary::Start()
{
	UProfileManager::Get()->Start();
}

void UUWABlueprintFunctionLibrary::Stop()
{
	UProfileManager::Get()->Stop();
}

void UUWABlueprintFunctionLibrary::Register()
{
	UProfileManager::Get()->RegisterCmd();
}

bool UUWABlueprintFunctionLibrary::WithEditor()
{
#if WITH_EDITOR
	return true;
#else
	return false;
#endif
}