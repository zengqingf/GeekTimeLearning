// Fill out your copyright notice in the Description page of Project Settings.


#include "MapleView.h"

/*
UE4 C++ —— UMG和C++交互
https://blog.csdn.net/niu2212035673/article/details/82792910

交互方法
一，强转子集
GetRootWidget()        //获取根节点
GetChildAt()            //获取子节点
UMG控件呈树状结构，根据根节点可以获取到所有的子节点
二，反射绑定
UPROPERTY(Meta = (BindWidget))
UButton *ButtonOne;
绑定的类型和名称必须和蓝图内的一致
三，根据控件名获取
GetWidgetFromName()
获取到UWidget*类型，强转成指定类型

*/

UMapleView::UMapleView(const FObjectInitializer& objInitializer) : Super(objInitializer)
{
	UE_LOG(LogTemp, Log, TEXT("### maple view initializer..."));

	testOneBtn = nullptr;
	rootPanel = nullptr;
}

void UMapleView::NativeConstruct()
{
	Super::NativeConstruct();

	//获取到UWidget*类型，强转成指定类型
	if (UButton* btn = Cast<UButton>(GetWidgetFromName("Button_Connect")))
	{
		FScriptDelegate del;
		del.BindUFunction(this, "onQueryDirBtnClick");
		btn->OnClicked.Add(del);
	}
}

bool UMapleView::Initialize()
{
	if (!Super::Initialize()) {
		return false;
	}

	//注意判空
	if (testOneBtn)
	{
		testOneBtn->OnClicked.__Internal_AddDynamic(this, &UMapleView::onTestOneBtnClick, FName("onTestOneBtnClick"));
	}
	
	rootPanel = Cast<UCanvasPanel>(GetRootWidget());
	if (rootPanel)
	{
		//UMG控件呈树状结构，根据根节点可以获取到所有的子节点
		UButton* testTwoBtn = Cast<UButton>(rootPanel->GetChildAt(3));
		if (testTwoBtn) {
			testTwoBtn->OnClicked.__Internal_AddDynamic(this, &UMapleView::onTestTwoBtnClick, FName("onTestTwoBtnClick"));
		}
	}

	return true;
}


void UMapleView::onQueryDirBtnClick()
{
	UE_LOG(LogTemp, Log, TEXT("### maple view query btn click..."));
	
	if (GEngine) {
		GEngine->AddOnScreenDebugMessage(-1, 20, FColor::Yellow, "onQueryDirBtnClick");
	}

	OnQueryDirService.Broadcast();
}

void UMapleView::onTestOneBtnClick()
{
	if (GEngine) {
		GEngine->AddOnScreenDebugMessage(-1, 20, FColor::Yellow, "onTestOneBtnClick");
	}
}

void UMapleView::onTestTwoBtnClick()
{
	if (GEngine) {
		GEngine->AddOnScreenDebugMessage(-1, 20, FColor::Yellow, "onTestTwoBtnClick");
	}
}
