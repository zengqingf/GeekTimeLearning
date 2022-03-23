// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Blueprint/UserWidget.h"
#include "Plugins/GCloud_Maple.h"

#include "WGCloud_Maple.generated.h"

/**
 * 
 */
//typedef TMap<FString, TArray<ServerInfo>> ServerDir;
//DECLARE_DELEGATE_OneParam(FDelegateServerDirRes, ServerDir);


UCLASS()
class UESDKTEST_API UWGCloud_Maple : public UUserWidget
{
	GENERATED_BODY()
public:
	UWGCloud_Maple(const FObjectInitializer& ObjectInitializer);
	~UWGCloud_Maple();

	virtual bool Initialize() override;
	UFUNCTION(BlueprintCallable)
	void OnInit();

	UFUNCTION(BlueprintCallable)
	void OnQueryTree();

	UFUNCTION(BlueprintCallable)
	void OnQueryLeaf();

	UFUNCTION(BlueprintCallable)
	void OnTestPost();

	//static FDelegateServerDirRes DelegateServerDirRes;
private:
	void Tick();
	GCloud_Maple * gcmaple;
	FTimerHandle timerHandler_;

	void OnGetServerDirInfo(TMap<FString, TArray<ServerInfo>> dir);

	
};
