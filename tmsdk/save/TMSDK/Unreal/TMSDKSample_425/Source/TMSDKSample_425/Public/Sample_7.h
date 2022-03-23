// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

DECLARE_LOG_CATEGORY_EXTERN(MyContaimTest, Log, All);
DEFINE_LOG_CATEGORY(MyContaimTest);

/**
* UE4容器TArray、TMap的使用
* ref: https://www.cnblogs.com/wodehao0808/p/8336966.html
*/
class TMSDKSAMPLE_425_API Sample_7
{
	public:
	Sample_7();
	~Sample_7();

	private:
	void TArrayForeach();
	void TArrayAddItem();
	void TArraySort();
	void TArrayIndex();
	void TArrayRemoveItem();
	void TArrayMoveOperator();
	void TArrayHeapSort();
	void TArrayLength();


	void TMapForeach();
	void TMapAddItem();
	void TMapFindItem();
	void TMapRemoveItem();
	void TMapSort();
	void TMapMoveOperator();
	void TMapCustomKey();
	void TMultiMapFindItem();

	void UE4Array();

	void RemoveItemInForEach();
};
