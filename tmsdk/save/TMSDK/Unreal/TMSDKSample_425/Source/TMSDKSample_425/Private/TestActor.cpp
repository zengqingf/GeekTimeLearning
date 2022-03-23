// Fill out your copyright notice in the Description page of Project Settings.


#include "TestActor.h"
#include "Engine/Engine.h"

// Sets default values
ATestActor::ATestActor()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

}

// Called when the game starts or when spawned
void ATestActor::BeginPlay()
{
	Super::BeginPlay();
	
	//actor = GetWorld() ? GetWorld()->SpawnActor<AActor>(actorClass, GetTransform()) : nullptr ;
	//actor = GetWorld() ? GetWorld()->SpawnActor<AActor>(actorClass.Get(), GetTransform()) : nullptr;

	BPImplTest2();
	BPNativeImplTest4();
}

// Called every frame
void ATestActor::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

void ATestActor::BPCallTest1()
{
	actor = GetWorld() ? GetWorld()->SpawnActor<AActor>(actorClass.Get(), GetTransform()) : nullptr;
}

bool ATestActor::PureTest3(FVector vec)
{
	return true;
}

void ATestActor::BPNativeImplTest4_Implementation()
{
	GEngine->AddOnScreenDebugMessage(-1, 3.f, FColor::Purple, TEXT("Native CPP"));
}

