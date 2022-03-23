// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "HelloLuaActor.generated.h"

UCLASS()
class GCLOUDTEST_API AHelloLuaActor  final : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	AHelloLuaActor();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

public:
	UFUNCTION(BlueprintCallable)
	int GetIndex();

public:
	UPROPERTY(EditAnywhere)
	FString name;
	
private:
	int index;
};
