// Copyright Epic Games, Inc. All Rights Reserved.

#include "testprojectGameMode.h"
#include "testprojectPawn.h"

AtestprojectGameMode::AtestprojectGameMode()
{
	// set default pawn class to our character class
	DefaultPawnClass = AtestprojectPawn::StaticClass();
}

