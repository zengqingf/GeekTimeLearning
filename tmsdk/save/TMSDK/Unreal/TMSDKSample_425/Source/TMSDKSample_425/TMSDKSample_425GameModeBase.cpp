// Copyright Epic Games, Inc. All Rights Reserved.


#include "TMSDKSample_425GameModeBase.h"

ATMSDKSample_425GameModeBase::ATMSDKSample_425GameModeBase()
{
	Sample_1 sp_1;
	sp_1.Init();

	Sample_2 sp_2;
	sp_2.Init();
	sp_2.Test3();

	Sample_3 sp_3;
	sp_3.TestSharedPtr();
	sp_3.TestSharedRef();


	UWorld* world = GetWorld();
	if (world)
	{
		APlayerController* pc = world->GetFirstPlayerController();
		if (pc)
		{
			ULocalPlayer* localPlayer = pc->GetLocalPlayer();
			if (localPlayer && localPlayer->ViewportClient)
			{
				FVector2D mousePos;
				if (localPlayer->ViewportClient->GetMousePosition(mousePos))
				{
					UE_LOG(LogTemp, Log, TEXT("### mouse pos x : %f,  y : %f"), mousePos.X, mousePos.Y);
				}
			}
		}
	}

	Sample_4 sp_4;
	//sp_4.CreateTestHttpLoginReq();
	//sp_4.CreateUploadFileReq();
	sp_4.CreateZipFile();


	//Sample_5 sp_5;
	//sp_5.Test1();

	Sample_6 sp_6;
}