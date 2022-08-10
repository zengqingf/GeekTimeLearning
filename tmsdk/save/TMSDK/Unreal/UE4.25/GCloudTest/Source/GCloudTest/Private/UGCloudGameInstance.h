// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Engine/GameInstance.h"
#include "IPlatformFilePak.h"
#include "UGCloudGameInstance.generated.h"

/**
 * 
 */
UCLASS()
class UUGCloudGameInstance : public UGameInstance
{
	GENERATED_BODY()
	

public:
	UUGCloudGameInstance();
	~UUGCloudGameInstance();

	void Init() override;
	void Shutdown() override;
	bool Tick(float DeltaSeconds);

	void OpenLevel(FName levelName);

	static UUGCloudGameInstance* GetInstance() { return m_instance; }


public:

	//UE4.Editor: 自定义控制台可调用函数

	/*
	UFUNCTION里面标记为Exec宏的函数就可以在控制台调用执行，
	在UGameInstance的派生类里面这样声明的可以直接调用访问，
	*/
	UFUNCTION(Exec, Category = "Console")
	void ConsoleFunction(FString info);

	/*
	在其他场景类里面声明的要在UGameInstance的派生类里面重写ProcessConsoleExec方法调用一下才可以在控制台调用。
	*/
	virtual bool ProcessConsoleExec(const TCHAR* Cmd, FOutputDevice& Ar, UObject* Executor) override;


private:
	static UUGCloudGameInstance* m_instance;


	void _mountPakWithMountPoint(FString pakName, FString mountPoint);
	void _initEncrypt(uint8* key);
	
protected:
	FDelegateHandle tickDelegateHandle;


	//ref : https://blog.csdn.net/JMcc_/article/details/104442383
	IPlatformFile* platformFile = nullptr;			//初始文件系统
	TSharedPtr<FPakPlatformFile> pakPlatformFile;	//pak文件系统
};
