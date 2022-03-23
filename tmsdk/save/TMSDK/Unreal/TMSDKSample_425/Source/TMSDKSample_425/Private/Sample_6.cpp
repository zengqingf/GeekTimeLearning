// Fill out your copyright notice in the Description page of Project Settings.


#include "Sample_6.h"

#include "TMSDKCommon.h"
#include "CryptoKeys/Private/CryptoKeysHelpers.h"

Sample_6::Sample_6()
{
	//@注意：不使用：FTMSDKCommonModule::GetSampleDelegate()，调用会失效！！！
	FTMSDKCommonModule::OnSample.AddRaw(this, &Sample_6::_tmSDKCommonDelTest);

	//临时测试调用
	FTMSDKCommonModule::TestCallMain();



	/*
	*测试外部模块访问
	1、在动态链接情况下，根据名字直接找对应的dll就行了，做为合法ue4模块的dll，必定要导出一些约定的函数，来返回自身IModuleInterface指针。
	2、在静态链接时，去一个叫StaticallyLinkedModuleInitializers的Map里找，这就要求所有模块已把自己注册到这个Map里。
	*/
	IModuleInterface* iTMsdkCommon = FModuleManager::Get().LoadModule("TMSDKCommon");



	//FString cryKey = "yuUvoLXewsjKt3vh0en/ALA6mUXAsgEppvGnyWCbSLM=";
	FString cryKey;
	//CryptoKeysHelpers::GenerateEncryptionKey(cryKey);
	GConfig->GetString(TEXT("/Script/CryptoKeys.CryptoKeysSettings"), TEXT("EncryptionKey"), cryKey,FPaths::ProjectConfigDir()/"DefaultCrypto.ini");
	TArray<uint8> key;
	FBase64::Decode(cryKey, key);
	check(key.Num() == 32);
	for (int32 Index = 0; Index < 8; ++Index)
	{
		if (key[Index] == 0)
		{
			UE_LOG(LogTemp, Log, TEXT("#### base64 key okok"));
			break;
		}
	}
	
	//key.Add('\0');
	//FString res =
	//	FString::Printf(TEXT("%s"), ANSI_TO_TCHAR(reinterpret_cast<const char*>(key.GetData(), key.Num())));
	const std::string stdStr(reinterpret_cast<const char*>(key.GetData()), key.Num());
	FString res = stdStr.c_str();
	
	UE_LOG(LogTemp, Log, TEXT("#### %s"), *res);
}

Sample_6::~Sample_6()
{
}

void Sample_6::_tmSDKCommonDelTest()
{
	UE_LOG(LogTemp, Log, TEXT("### 跨模块调用 -- TMSDKCommon module call TMSDKSample moudle"))
}
