// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "TestActor.generated.h"

UENUM(BlueprintType)
enum class EMyEnum : uint8 
{
	EType1 UMETA(DisplayName = "Play"),
	EType2 UMETA(DisplayName = "Game")
};

//USTRUCT()    告知UHT把类添加到反射系统中

//BlueprintType   Blueprintable   用于加到UObject中 使UObject支持基于它生成蓝图类

UCLASS()
class TMSDKSAMPLE_425_API ATestActor : public AActor
{
	//UE4 将这个标记替换为该类型生成所有必要的样板代码
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	ATestActor();


	UPROPERTY(VisibleAnywhere, BlueprintReadWrite, Category = "Level1 | Level2")
	int visibleOnly;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	EMyEnum testEnum;

	UPROPERTY(BlueprintReadWrite)
	TArray<int32> intArray;

	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite)
	int32 testInt = 1;

	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite)
		//UClass* actorClass; //等同
	TSubclassOf<AActor> actorClass; //TSubclassOf可以用于筛选

	UPROPERTY(EditInstanceOnly, BlueprintReadWrite)
	AActor* actor;

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;



	UFUNCTION(BlueprintCallable)
	void BPCallTest1();

	//不需要在C++中实现
	UFUNCTION(BlueprintImplementableEvent)
	void BPImplTest2();

	//只读操作 
	UFUNCTION(BlueprintPure)
	bool PureTest3(FVector vec);

	//优先用蓝图内的实现，也可以在C++中实现
	UFUNCTION(BlueprintNativeEvent)
	void BPNativeImplTest4();
	//这个是BPNativeImplTest4的实现的一种写法！
	virtual void BPNativeImplTest4_Implementation();
};



//Sample_4
//https://www.cnblogs.com/FlyingZiming/p/14560156.html
USTRUCT()
struct FRequest_Login {
	GENERATED_BODY()

		UPROPERTY()
		FString email;
	UPROPERTY()
		FString password;

	FRequest_Login() {}
};

USTRUCT()
struct FResponse_Login {
	GENERATED_BODY()

		UPROPERTY()
		int id;
	UPROPERTY()
		FString name;
	UPROPERTY()
		FString hash;

	FResponse_Login() {}
};