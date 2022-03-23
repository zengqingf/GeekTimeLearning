// Fill out your copyright notice in the Description page of Project Settings.


#include "Unlua/HelloLuaActor.h"

// Sets default values
AHelloLuaActor::AHelloLuaActor()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

}

// Called when the game starts or when spawned
void AHelloLuaActor::BeginPlay()
{
	Super::BeginPlay();
	
}

// Called every frame
void AHelloLuaActor::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

int AHelloLuaActor::GetIndex()
{
	return ++index;
}
