// Fill out your copyright notice in the Description page of Project Settings.


#include "UGCloudGameInstance.h"
#include "Plugins/PluginManager.h"

#include "UtilityMarcoDefine.h"

#include "Kismet/KismetSystemLibrary.h"
#include "Kismet/GameplayStatics.h"

#include "HAL/PlatformFilemanager.h"
#include "UnLua/HelloLuaActor.h"

UUGCloudGameInstance* UUGCloudGameInstance::m_instance = nullptr;

UUGCloudGameInstance::UUGCloudGameInstance()
{
	//加密pak 注释暂时不用
	//FCoreDelegates::GetPakEncryptionKeyDelegate().BindUObject(this, &UUGCloudGameInstance::_initEncrypt);

	//保存初始文件系统指针
	platformFile = &FPlatformFileManager::Get().GetPlatformFile();
}

UUGCloudGameInstance::~UUGCloudGameInstance()
{

}

void UUGCloudGameInstance::Init()
{
	Super::Init();

	UE_LOG(LogTemp, Warning, TEXT("### gcloud game ins init 1"));

	m_instance = this;

	tickDelegateHandle = FTicker::GetCoreTicker().AddTicker(FTickerDelegate::CreateUObject(this, &UUGCloudGameInstance::Tick));

	PluginManager::Instance().InitPlugins();

	//pak文件系统指针
	pakPlatformFile = MakeShareable(new FPakPlatformFile());
	//在pak文件系统设置parent文件系统
	pakPlatformFile->Initialize(platformFile, TEXT(""));
	//将PakPlatformFile设置为最顶层，即首先查找的PakPlatformFile内的文件
	//再查找PakPlatformFile的Parent的文件，以此类推
	FPlatformFileManager::Get().SetPlatformFile(*pakPlatformFile);

	UE_LOG(LogTemp, Warning, TEXT("### gcloud game ins init 2"));
}

void UUGCloudGameInstance::Shutdown()
{
	UE_LOG(LogTemp, Warning, TEXT("### gcloud game ins shutdown 1"));


	FTicker::GetCoreTicker().RemoveTicker(tickDelegateHandle);

	PluginManager::Instance().UninitPlugins();

	//SAFE_DELETE_PTR(testGCloudMgr);

	UE_LOG(LogTemp, Warning, TEXT("### gcloud game ins shutdown 2"));

	Super::Shutdown();
}

bool UUGCloudGameInstance::Tick(float DeltaSeconds)
{
	PluginManager::Instance().TickPlugins(DeltaSeconds);

	return true;
}

void UUGCloudGameInstance::OpenLevel(FName levelName)
{
	UGameplayStatics::OpenLevel(GetWorld(), levelName);
}

void UUGCloudGameInstance::ConsoleFunction(FString info)
{
	GEngine->AddOnScreenDebugMessage(-1, 5.f, FColor::Purple, info);
}

bool UUGCloudGameInstance::ProcessConsoleExec(const TCHAR* Cmd, FOutputDevice& Ar, UObject* Executor)
{
	bool res = Super::ProcessConsoleExec(Cmd, Ar, Executor);
	if (!res)
	{
		/*
		for (TActorIterator<AHelloLuaActor> it(GetWorld()); it; ++it)
		{

		}
		*/
	}
	return res;
}

void UUGCloudGameInstance::_mountPakWithMountPoint(FString pakName, FString mountPoint)
{
	if (FPaths::FileExists(pakName) && pakPlatformFile.IsValid())
	{
		//避免重复挂载
		TArray<FString> existPaks;
		pakPlatformFile->GetMountedPakFilenames(existPaks);
		if (existPaks.Find(pakName) >= 0) {
			return;
		}
		
		if (pakPlatformFile->Mount(*pakName, 0, *mountPoint)) {
			GEngine->AddOnScreenDebugMessage(INDEX_NONE, 60.f, FColor::Red, *FString::Printf(TEXT("Mount OK %s %s"), *pakName, *mountPoint));
		}
		else
		{
			GEngine->AddOnScreenDebugMessage(INDEX_NONE, 60.f, FColor::Red, *FString::Printf(TEXT("Mount File %s %s"), *pakName, *mountPoint));
		}
	}
}

void UUGCloudGameInstance::_initEncrypt(uint8* key)
{
	FString KeyStr = TEXT("kDIta832HPHzaP+M8bDKrTGxnlU8QGYcEAWN5AAiLYI=");
	TArray<uint8> KeyBase64Ary;
	FBase64::Decode(KeyStr, KeyBase64Ary);
	char* KeyU8 = TCHAR_TO_UTF8(*KeyStr);
	FMemory::Memcpy(key, KeyBase64Ary.GetData(), FAES::FAESKey::KeySize);
}