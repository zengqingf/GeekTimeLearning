// Fill out your copyright notice in the Description page of Project Settings.


#include "WGCloud_Maple.h"
#include "IGCloud.h"
#include "Plugins/HttpRequestUtility.h"



UWGCloud_Maple::UWGCloud_Maple(const FObjectInitializer& ObjectInitializer) :Super(ObjectInitializer)
{
	gcmaple = new GCloud_Maple();
	gcmaple->DelegateServerDirRes.BindUObject(this,&UWGCloud_Maple::OnGetServerDirInfo);
}
UWGCloud_Maple::~UWGCloud_Maple()
{

}

bool UWGCloud_Maple::Initialize()
{
	if (!UUserWidget::Initialize()) {
		return false;
	}
	//PluginsManager::GetInstance()->SetBlueprintObj(this);
	return true;
}

void UWGCloud_Maple::OnInit()
{
	UE_LOG(LogTemp,Warning,TEXT("version %s"), FApp::GetBuildVersion());

	gcmaple->Init();
	GetWorld()->GetTimerManager().SetTimer(timerHandler_, this, &UWGCloud_Maple::Tick, 0.5F, true);
}

void UWGCloud_Maple::OnQueryTree()
{
	gcmaple->QueryTree(1);
}

void UWGCloud_Maple::OnQueryLeaf()
{
	gcmaple->QueryLeaf(1,4);
}

void UWGCloud_Maple::OnTestPost()
{
	FString url = "https://puberp.superboss.cc/as/order/deal/list";
	HttpRequestUtility * req = NewObject<HttpRequestUtility>();
	req->StartPost(url);
}

void UWGCloud_Maple::Tick()
{
	gcmaple->Update();
}

void UWGCloud_Maple::OnGetServerDirInfo(TMap<FString, TArray<ServerInfo>> dir)
{
	for (auto rd : dir)
	{
		UE_LOG(LogTemp, Warning, TEXT("Contains  Property: Key %s"), *rd.Key);
		for (auto d : rd.Value)
		{
			UE_LOG(LogTemp, Warning, TEXT("Contains: ID %d, name: %s flag: %d  url:%s "), d.ID, *d.ServerName, d.Flag, *d.Url);
		}
	}
}
