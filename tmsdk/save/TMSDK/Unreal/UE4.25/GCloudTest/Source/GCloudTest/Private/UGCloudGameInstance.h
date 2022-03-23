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
