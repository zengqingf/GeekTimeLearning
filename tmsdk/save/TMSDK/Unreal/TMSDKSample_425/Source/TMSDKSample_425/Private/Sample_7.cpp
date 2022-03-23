﻿#include "Sample_7.h"
#include "PriorityQueue.h"

Sample_7::Sample_7()
{
}

Sample_7::~Sample_7()
{
}

void Sample_7::TArrayForeach()
{
	auto printFunc1 = [&](TArray<FString>& _arr)->void{
		for (int32 i = 0; i < _arr.Num(); i++)
		{
			//FString str = FString::Printf(TEXT("--- index:%d, value:%s "), i, *_arr[i]);
			//GEngine->AddOnScreenDebugMessage(0, 3.0f, FColor::Red, str);
			UE_LOG(MyContaimTest, Warning, TEXT("--- index:%d, value:%s "),i, *_arr[i]);
		}
	};

	auto printFunc2 = [&](TArray<FString>& _arr)->void {
		for (const FString& val : _arr)
		{
			UE_LOG(MyContaimTest, Warning, TEXT("--- value111:%s"), *val);
		}
	};

	auto printFunc3 = [&](TArray<FString>& _arr)->void {
		for (TArray<FString>::TConstIterator iter = _arr.CreateConstIterator(); iter; ++iter)
		{
			UE_LOG(MyContaimTest, Warning, TEXT("--- value222:%s"), *(*iter));
		}
	};

}

void Sample_7::TArrayAddItem()
{
	// 创建一个数组
	TArray<int32> IntArray;

	// 通过同一个元素填充初始化
	IntArray.Init(10, 5);
	// IntArray == [10, 10, 10, 10, 10]

	// 增加新元素
	// Add会引入临时对象，优先使用Emplace
	TArray<FString> StrArr;
	StrArr.Add(TEXT("Hello"));
	StrArr.Emplace(TEXT("World"));
	// StrArr == ["Hello", "World"]
	//printFunc1(StrArr);

	// 追加多个元素
	FString Arr[] = { TEXT("of"), TEXT("Tomorrow") };
	StrArr.Append(Arr, ARRAY_COUNT(Arr));
	// StrArr == ["Hello", "World", "of", "Tomorrow"]
	//printFunc1(StrArr);

	// 只有容器中不存在该元素的时候，才添加
	StrArr.AddUnique(TEXT("!"));
	// StrArr = ["Hello", "World", "of", "Tomorrow", "!"]
	//printFunc1(StrArr);
	StrArr.AddUnique(TEXT("!"));
	// StrArr没有变
	//printFunc1(StrArr);

	// 插入
	StrArr.Insert(TEXT("Brave"), 1);
	// StrArr == ["Hello","Brave","World","of","Tomorrow","!"]
	//printFunc1(StrArr);

	// 直接设置数组的元素个数
	// 如果大于当前值，那么使用元素类型的默认构造函数创建新元素
	// 如果下雨当前值，相当于删除元素
	StrArr.SetNum(8);
	// StrArr == ["Hello","Brave","World","of","Tomorrow","!","",""]
	//printFunc1(StrArr);

	StrArr.SetNum(6);
	// StrArr == ["Hello","Brave","World","of","Tomorrow", "!"]
	//printFunc1(StrArr);
	//printFunc2(StrArr);
	//printFunc3(StrArr);
}

void Sample_7::TArraySort()
{
	TArray<FString> StrArr;
	// 排序（快排序，不稳定的）
	// 默认按照operator <
	StrArr.Sort();
	// StrArr == ["!","Brave","Hello","of","Tomorrow","World"]

	// 自定义排序规则
	StrArr.Sort([](const FString& A, const FString& B) {
		return A.Len() < B.Len();
	});
	// StrArr == ["!","of","Hello","Brave","World","Tomorrow"]
	//printFunc1(StrArr);

	// 堆排序（也不稳定的）
	StrArr.HeapSort([](const FString& A, const FString& B) {
		return A.Len() < B.Len();
	});
	// StrArr == ["!","of","Brave","Hello","World","Tomorrow"]
	//printFunc1(StrArr);

	// 合并排序（稳定的）
	StrArr.StableSort([](const FString& A, const FString& B) {
		return A.Len() < B.Len();
	});
	// StrArr == ["!","of","Brave","Hello","World","Tomorrow"]
	//printFunc1(StrArr);
}

void Sample_7::TArrayIndex()
{
	TArray<FString> StrArr;
	// 查询
    // 数量
    int32 Count = StrArr.Num();
	// Count == 6

    // 直接访问
    // 如果容器是const，那么返回的指针也是const的
    FString* StrPtr = StrArr.GetData();
    // StrPtr[0] == "!"
    // StrPtr[1] == "of"
    // ...

    // 一个元素的大小
    uint32 ElementSize = StrArr.GetTypeSize();
    // ElementSize == sizeof(FString)

    // 判断索引
    bool bValidM1 = StrArr.IsValidIndex(-1);
    // bValidM1 == false

    // operator[] 返回索引
    //StrArr[3] = StrArr[3].ToUpper();
    // StrArr == ["!","of","Brave","HELLO","World","Tomorrow"]

    // 从后面访问
    FString ElemEnd = StrArr.Last();
    FString ElemEnd0 = StrArr.Last(0);
    FString ElemEnd1 = StrArr.Last(1);
    FString ElemTop = StrArr.Top();
    // ElemEnd  == "Tomorrow"
    // ElemEnd0 == "Tomorrow"
    // ElemEnd1 == "World"
    // ElemTop  == "Tomorrow"

    // 查询是否存在某元素
    bool bHello = StrArr.Contains(TEXT("Hello"));
    bool bGoodbye = StrArr.Contains(TEXT("Goodbye"));
    // bHello   == true
    // bGoodbye == false

    // 自定义查询规则
    bool bLen5 = StrArr.ContainsByPredicate([](const FString& Str) {
        return Str.Len() == 5;
    });
    bool bLen6 = StrArr.ContainsByPredicate([](const FString& Str) {
        return Str.Len() == 6;
    });
    // bLen5 == true
    // bLen6 == false

    // 查找，返回索引
    int32 Index;
    if (StrArr.Find(TEXT("Hello"), Index))
    {
        // Index == 3
    }
    int32 IndexLast;
    if (StrArr.FindLast(TEXT("Hello"), IndexLast))
    {
        // IndexLast == 3, because there aren't any duplicates
    }
    // 还可以直接返回index, 如果找不到会返回INDEX_NONE
    int32 Index2 = StrArr.Find(TEXT("Hello"));
    int32 IndexLast2 = StrArr.FindLast(TEXT("Hello"));
    int32 IndexNone = StrArr.Find(TEXT("None"));
    // Index2     == 3
    // IndexLast2 == 3
    // IndexNone  == INDEX_NONE

    // 还可以用IndexOfByKey, 采用operator==(ElementType, KeyType)比较
    int32 Index3 = StrArr.IndexOfByKey(TEXT("Hello"));
    // Index == 3

    int32 Index4 = StrArr.IndexOfByPredicate([](const FString& Str) {
        return Str.Contains(TEXT("r"));
    });
    // Index == 2

    // 除了返回索引，也可以返回指针
    auto* OfPtr = StrArr.FindByKey(TEXT("of"));
    auto* ThePtr = StrArr.FindByKey(TEXT("the"));
    // OfPtr  == &StrArr[1]
    // ThePtr == nullptr

    auto* Len5Ptr = StrArr.FindByPredicate([](const FString& Str) {
        return Str.Len() == 5;
    });
    auto* Len6Ptr = StrArr.FindByPredicate([](const FString& Str) {
        return Str.Len() == 6;
    });
    // Len5Ptr == &StrArr[2]
    // Len6Ptr == nullptr

}

void Sample_7::TArrayRemoveItem()
{
	TArray<FString> StrArr;
	// 删除所有匹配的元素
    StrArr.Remove(TEXT("hello"));
    // StrArr == ["!","of","Brave","World","Tomorrow"]
    StrArr.Remove(TEXT("goodbye"));
    // StrArr is unchanged, as it doesn't contain "goodbye"

    // 删除最后的元素
    StrArr.Pop();
    // StrArr == ["!", "of", "Brave", "World"]

    // 删除第一个匹配元素
    TArray<int32> ValArr;
    int32 Temp[] = { 10, 20, 30, 5, 10, 15, 20, 25, 30 };
    ValArr.Append(Temp, ARRAY_COUNT(Temp));
    // ValArr == [10,20,30,5,10,15,20,25,30]

    ValArr.Remove(20);
    // ValArr == [10,30,5,10,15,25,30]
    ValArr.RemoveSingle(30);
    // ValArr == [10,5,10,15,25,30]
    // 通过索引删除
    ValArr.RemoveAt(2); // Removes the element at index 2
                        // ValArr == [10,5,15,25,30]
    //ValArr.RemoveAt(99);
    // This will cause a runtime error as
    // there is no element at index 99

    // 条件删除
    ValArr.RemoveAll([](int32 Val) {
        return Val % 3 == 0;
    });
    // ValArr == [10,5,25]

    // 如果删除元素之后无顺序要求，可以用更高效的方法
    //原理：把最后的元素移动到被删除的地方，这样就无需移动 被删除元素以后的元素
    TArray<int32> ValArr2;
    for (int32 i = 0; i != 10; ++i)
        ValArr2.Add(i % 5);
    // ValArr2 == [0,1,2,3,4,0,1,2,3,4]

    ValArr2.RemoveSwap(2); //删除等于2的元素
    // ValArr2 == [0,1,4,3,4,0,1,3]

    ValArr2.RemoveAtSwap(1); //删除索引为1的元素
    // ValArr2 == [0,3,4,3,4,0,1]

    ValArr2.RemoveAllSwap([](int32 Val) { //删除所有3的倍数
        return Val % 3 == 0;
    });
    // ValArr2 == [1,4,4]

    // 清空
    ValArr2.Empty();
    // ValArr2 == []

}

void Sample_7::TArrayMoveOperator()
{
	TArray<int32> ValArr3;
	ValArr3.Add(1);
	ValArr3.Add(2);
	ValArr3.Add(3);

	auto ValArr4 = ValArr3;
	// ValArr4 == [1,2,3];
	ValArr4[0] = 5;
	// ValArr3 == [1,2,3];
	// ValArr4 == [5,2,3];

	ValArr4 += ValArr3;
	// ValArr4 == [5,2,3,1,2,3]

	// move语义，源数组会被清空
	ValArr3 = MoveTemp(ValArr4);
	// ValArr3 == [5,2,3,1,2,3]
	// ValArr4 == []

	TArray<FString> FlavorArr1;
	FlavorArr1.Emplace(TEXT("Chocolate"));
	FlavorArr1.Emplace(TEXT("Vanilla"));
	// FlavorArr1 == ["Chocolate","Vanilla"]

	auto FlavorArr2 = FlavorArr1;
	// FlavorArr2 == ["Chocolate","Vanilla"]
	bool bComparison1 = FlavorArr1 == FlavorArr2;
	// bComparison1 == true
	if (bComparison1)
		UE_LOG(MyContaimTest, Warning, TEXT("--- FlavorArr1 == FlavorArr2"));

	for (auto& Str : FlavorArr2)
	{
		Str = Str.ToUpper();
	}
	// FlavorArr2 == ["CHOCOLATE","VANILLA"]

	bool bComparison2 = FlavorArr1 == FlavorArr2;
	// bComparison2 == true, because FString comparison ignores case
	if (bComparison2)
		UE_LOG(MyContaimTest, Warning, TEXT("--- FlavorArr1 == FlavorArr2 too"));

	Exchange(FlavorArr2[0], FlavorArr2[1]); //交换两个元素
	// FlavorArr2 == ["VANILLA","CHOCOLATE"]
	bool bComparison3 = FlavorArr1 == FlavorArr2;
	// bComparison3 == false, because the order has changed

}

void Sample_7::TArrayHeapSort()
{
	// 堆
	TArray<int32> HeapArr;
	for (int32 Val = 10; Val != 0; --Val)
		HeapArr.Add(Val);
	// HeapArr == [10,9,8,7,6,5,4,3,2,1]
	HeapArr.Heapify();
	// HeapArr == [1,2,4,3,6,5,8,10,7,9]

	HeapArr.HeapPush(4);
	// HeapArr == [1,2,4,3,4,5,8,10,7,9,6]
	int32 TopNode;
	HeapArr.HeapPop(TopNode);
	// TopNode == 1
	// HeapArr == [2,3,4,6,4,5,8,10,7,9]
	HeapArr.HeapRemoveAt(1);
	// HeapArr == [2,4,4,6,9,5,8,10,7]
	int32 Top = HeapArr.HeapTop();
	// Top == 2

	
	PriorityQueue<int> _queue{[](int left,int right)
	{
		return left > right;
	}};
}

void Sample_7::TArrayLength()
{
	// slack
	// GetSlack() is equivalent to Max() - Num():
	TArray<int32> SlackArray;
	// SlackArray.GetSlack() == 0
	// SlackArray.Num()      == 0
	// SlackArray.Max()      == 0
	//printFunc1(SlackArray);

	SlackArray.Add(1);
	// SlackArray.GetSlack() == 3
	// SlackArray.Num()      == 1
	// SlackArray.Max()      == 4
	//printFunc1(SlackArray);

	SlackArray.Add(2);
	SlackArray.Add(3);
	SlackArray.Add(4);
	SlackArray.Add(5);
	// SlackArray.GetSlack() == 17
	// SlackArray.Num()      == 5
	// SlackArray.Max()      == 22
	//printFunc1(SlackArray); //重新分配内存的公式Retval = NumElements + 3*NumElements/8 + 16;

	SlackArray.Empty(); //内存也清空
	// SlackArray.GetSlack() == 0
	// SlackArray.Num()      == 0
	// SlackArray.Max()      == 0
	//printFunc1(SlackArray);

	SlackArray.Empty(3);//只是清空元素，内存还在
	// SlackArray.GetSlack() == 3
	// SlackArray.Num()      == 0
	// SlackArray.Max()      == 3

	SlackArray.Add(1);
	SlackArray.Add(2);
	SlackArray.Add(3);
	// SlackArray.GetSlack() == 0
	// SlackArray.Num()      == 3
	// SlackArray.Max()      == 3

	SlackArray.Reset(0); //只是清空元素，内存还在
	// SlackArray.GetSlack() == 3
	// SlackArray.Num()      == 0
	// SlackArray.Max()      == 3
	SlackArray.Reset(10);//大于当前的Max（10>3），重新分配内存
	// SlackArray.GetSlack() == 10
	// SlackArray.Num()      == 0
	// SlackArray.Max()      == 10

	SlackArray.Add(5);
	SlackArray.Add(10);
	SlackArray.Add(15);
	SlackArray.Add(20);
	// SlackArray.GetSlack() == 6
	// SlackArray.Num()      == 4
	// SlackArray.Max()      == 10
	SlackArray.Shrink(); //缩减内存到当前元素的个数
	// SlackArray.GetSlack() == 0
	// SlackArray.Num()      == 4
	// SlackArray.Max()      == 4

}

void Sample_7::TMapForeach()
{
	auto printFunc1 = [&](TMap<int32, FString>& _map)->void {
		for (TPair<int32, FString>& element : _map)
		{
			UE_LOG(MyContaimTest, Warning, TEXT("--- key:%d, value111:%s "),
				element.Key, *element.Value);
		}
	};

	auto printFunc2 = [&](TMap<int32, FString>& _map)->void {
		for (TMap<int32, FString>::TConstIterator iter = _map.CreateConstIterator(); iter; ++iter)
		{
			UE_LOG(MyContaimTest, Warning, TEXT("--- key:%d, value222:%s "),
				iter->Key, *iter->Value);
		}
	};

	auto printFunc3= [&](TMap<int32, FString>& _map)->void {
		for (TMap<int32, FString>::TConstIterator iter(_map); iter; ++iter)
		{
			UE_LOG(MyContaimTest, Warning, TEXT("--- key:%d, value222:%s "),
				iter->Key, *iter->Value);
		}
	};
}

void Sample_7::TMapAddItem()
{
	// 创建
	// key比较使用==
	// hashcode计算使用GetTypeHash
	TMap<int32, FString> FruitMap;

	FruitMap.Add(5, TEXT("Banana"));
	FruitMap.Add(2, TEXT("Grapefruit"));
	FruitMap.Add(7, TEXT("Pineapple"));
	// FruitMap == [
	//  { Key: 5, Value: "Banana"     },
	//  { Key: 2, Value: "Grapefruit" },
	//  { Key: 7, Value: "Pineapple"  }
	// ]
	FruitMap.Add(2, TEXT("Pear")); //相同key值，顶掉value
	// FruitMap == [
	//  { Key: 5, Value: "Banana"    },
	//  { Key: 2, Value: "Pear"      },
	//  { Key: 7, Value: "Pineapple" }
	// ]

	FruitMap.Add(4);//没有value值，会构造一个默认值进去
	// FruitMap == [
	//  { Key: 5, Value: "Banana"    },
	//  { Key: 2, Value: "Pear"      },
	//  { Key: 7, Value: "Pineapple" },
	//  { Key: 4, Value: ""          }
	// ]

	FruitMap.Emplace(3, TEXT("Orange"));
	// FruitMap == [
	//  { Key: 5, Value: "Banana"    },
	//  { Key: 2, Value: "Pear"      },
	//  { Key: 7, Value: "Pineapple" },
	//  { Key: 4, Value: ""          },
	//  { Key: 3, Value: "Orange"    }
	// ]

	TMap<int32, FString> FruitMap2;
	FruitMap2.Emplace(4, TEXT("Kiwi"));
	FruitMap2.Emplace(9, TEXT("Melon"));
	FruitMap2.Emplace(5, TEXT("Mango"));
	FruitMap.Append(FruitMap2); //已有的会顶掉，没有就完后叠
	// FruitMap == [
	//  { Key: 5, Value: "Mango"     },
	//  { Key: 2, Value: "Pear"      },
	//  { Key: 7, Value: "Pineapple" },
	//  { Key: 4, Value: "Kiwi"      },
	//  { Key: 3, Value: "Orange"    },
	//  { Key: 9, Value: "Melon"     }
	// ]
	//printFunc1(FruitMap);
}

void Sample_7::TMapFindItem()
{
	TMap<int32, FString> FruitMap;
	//---------- 通过key查找到value
    // 查询
    int32 Count = FruitMap.Num();
    // Count == 6

    FString Val7 = FruitMap[7];
    // Val7 == "Pineapple"
    //FString Val8 = FruitMap[8]; // assert!//查找不存在的会造成运行时崩溃

    bool bHas7 = FruitMap.Contains(7);
    bool bHas8 = FruitMap.Contains(8);
    // bHas7 == true
    // bHas8 == false

    FString* Ptr7 = FruitMap.Find(7); //返回的是value的指针
    FString* Ptr8 = FruitMap.Find(8);
    // *Ptr7 == "Pineapple"
    //  Ptr8 == nullptr

    FString& Ref7 = FruitMap.FindOrAdd(7); //返回的是引用
    // Ref7     == "Pineapple"
    // FruitMap == [
    //  { Key: 5, Value: "Mango"     },
    //  { Key: 2, Value: "Pear"      },
    //  { Key: 7, Value: "Pineapple" },
    //  { Key: 4, Value: "Kiwi"      },
    //  { Key: 3, Value: "Orange"    },
    //  { Key: 9, Value: "Melon"     }
    // ]

    FString& Ref8 = FruitMap.FindOrAdd(8); //不存在则构造一个添加进去，返回引用
    // Ref8     == ""
    // FruitMap == [
    //  { Key: 5, Value: "Mango"     },
    //  { Key: 2, Value: "Pear"      },
    //  { Key: 7, Value: "Pineapple" },
    //  { Key: 4, Value: "Kiwi"      },
    //  { Key: 3, Value: "Orange"    },
    //  { Key: 9, Value: "Melon"     },
    //  { Key: 8, Value: ""          }
    // ]

    FString Val10 = FruitMap.FindRef(7); //存在则复制拷贝到变量中，不存在则变量自己构造
    FString Val9 = FruitMap.FindRef(6);
    // Val10    == "Pineapple"
    // Val9     == ""
    // FruitMap == [
    //  { Key: 5, Value: "Mango"     },
    //  { Key: 2, Value: "Pear"      },
    //  { Key: 7, Value: "Pineapple" },
    //  { Key: 4, Value: "Kiwi"      },
    //  { Key: 3, Value: "Orange"    },
    //  { Key: 9, Value: "Melon"     },
    //  { Key: 8, Value: ""          }
    // ]

    //---------- 通过value查找到key
    const int32* KeyMangoPtr = FruitMap.FindKey(TEXT("Mango"));
    const int32* KeyKumquatPtr = FruitMap.FindKey(TEXT("Kumquat"));
    // *KeyMangoPtr   == 5
    //  KeyKumquatPtr == nullptr

    TArray<int32>   FruitKeys;
    TArray<FString> FruitValues;
    FruitMap.GenerateKeyArray(FruitKeys); //生成key、value数组
    FruitMap.GenerateValueArray(FruitValues);
    // FruitKeys   == [ 5,2,7,4,3,9,8 ]
    // FruitValues == [ "Mango","Pear","Pineapple","Kiwi","Orange",
    //                  "Melon","" ]
}

void Sample_7::TMapRemoveItem()
{
	TMap<int32, FString> FruitMap;
	FruitMap.Remove(8);
	// FruitMap == [
	//  { Key: 5, Value: "Mango"     },
	//  { Key: 2, Value: "Pear"      },
	//  { Key: 7, Value: "Pineapple" },
	//  { Key: 4, Value: "Kiwi"      },
	//  { Key: 3, Value: "Orange"    },
	//  { Key: 9, Value: "Melon"     }
	// ]

	FString Removed7 = FruitMap.FindAndRemoveChecked(7); //查找并移除
	// Removed7 == "Pineapple"
	// FruitMap == [
	//  { Key: 5, Value: "Mango"  },
	//  { Key: 2, Value: "Pear"   },
	//  { Key: 4, Value: "Kiwi"   },
	//  { Key: 3, Value: "Orange" },
	//  { Key: 9, Value: "Melon"  }
	// ]

	//FString Removed8 = FruitMap.FindAndRemoveChecked(8); // assert!不存在，运行时崩溃

	FString Removed;
	bool bFound2 = FruitMap.RemoveAndCopyValue(2, Removed); //存在这个拷贝数据，返回true
	// bFound2  == true
	// Removed  == "Pear"
	// FruitMap == [
	//  { Key: 5, Value: "Mango"  },
	//  { Key: 4, Value: "Kiwi"   },
	//  { Key: 3, Value: "Orange" },
	//  { Key: 9, Value: "Melon"  }
	// ]

	bool bFound8 = FruitMap.RemoveAndCopyValue(8, Removed); //不存在，则不改变Removed的值，返回false
	// bFound8  == false
	// Removed  == "Pear", i.e. unchanged
	// FruitMap == [
	//  { Key: 5, Value: "Mango"  },
	//  { Key: 4, Value: "Kiwi"   },
	//  { Key: 3, Value: "Orange" },
	//  { Key: 9, Value: "Melon"  }
	// ]

	TMap<int32, FString> FruitMapCopy = FruitMap;
	// FruitMapCopy == [
	//  { Key: 5, Value: "Mango"  },
	//  { Key: 4, Value: "Kiwi"   },
	//  { Key: 3, Value: "Orange" },
	//  { Key: 9, Value: "Melon"  }
	// ]

	FruitMapCopy.Empty();//清空
	// FruitMapCopy == []
}

void Sample_7::TMapSort()
{
	TMap<int32, FString> FruitMap;
	// 排序
	FruitMap.KeySort([](int32 A, int32 B) {
		return A > B; // sort keys in reverse
	});
	// FruitMap == [
	//  { Key: 9, Value: "Melon"  },
	//  { Key: 5, Value: "Mango"  },
	//  { Key: 4, Value: "Kiwi"   },
	//  { Key: 3, Value: "Orange" }
	// ]

	FruitMap.ValueSort([](const FString& A, const FString& B) {
		return A.Len() < B.Len(); // sort strings by length
	});
	// FruitMap == [
	//  { Key: 4, Value: "Kiwi"   },
	//  { Key: 5, Value: "Mango"  },
	//  { Key: 9, Value: "Melon"  },
	//  { Key: 3, Value: "Orange" }
	// ]
	//printFunc3(FruitMap);
}

void Sample_7::TMapMoveOperator()
{
	TMap<int32, FString> FruitMap;
	// 操作符
    TMap<int32, FString> NewMap = FruitMap;
    NewMap[5] = "Apple";
    NewMap.Remove(3);
    // NewMap == [
    //  { Key: 4, Value: "Kiwi"  },
    //  { Key: 5, Value: "Apple" },
    //  { Key: 9, Value: "Melon" }
    // ]

    FruitMap = MoveTemp(NewMap); //move
    // FruitMap == [
    //  { Key: 4, Value: "Kiwi"  },
    //  { Key: 5, Value: "Apple" },
    //  { Key: 9, Value: "Melon" }
    // ]
    // NewMap == []

    // Slack
    FruitMap.Reset();//内存也清空
    // FruitMap == [<invalid>, <invalid>, <invalid>]
    // printFunc1(FruitMap);

    FruitMap.Reserve(10);//预设了10个大小的内存，add时候从后往前叠；没有FruitMap.Reserve(10);则是从前往后叠
    for (int32 i = 0; i != 10; ++i)
    {
        FruitMap.Add(i, FString::Printf(TEXT("Fruit%d"), i));
    }
    // FruitMap == [
    //  { Key: 9, Value: "Fruit9" },
    //  { Key: 8, Value: "Fruit8" },
    //  ...
    //  { Key: 1, Value: "Fruit1" },
    //  { Key: 0, Value: "Fruit0" }
    // ]
    //printFunc1(FruitMap);

    for (int32 i = 0; i != 10; i += 2)
    {
        FruitMap.Remove(i); //移除后，还是有内存占用
    }
    // FruitMap == [
    //  { Key: 9, Value: "Fruit9" },
    //  <invalid>,
    //  { Key: 7, Value: "Fruit7" },
    //  <invalid>,
    //  { Key: 5, Value: "Fruit5" },
    //  <invalid>,
    //  { Key: 3, Value: "Fruit3" },
    //  <invalid>,
    //  { Key: 1, Value: "Fruit1" },
    //  <invalid>
    // ]
    //printFunc1(FruitMap);

    FruitMap.Shrink();//移除最后又一个有效内存 后的 所有无效的内存占用
    // FruitMap == [
    //  { Key: 9, Value: "Fruit9" },
    //  <invalid>,
    //  { Key: 7, Value: "Fruit7" },
    //  <invalid>,
    //  { Key: 5, Value: "Fruit5" },
    //  <invalid>,
    //  { Key: 3, Value: "Fruit3" },
    //  <invalid>,
    //  { Key: 1, Value: "Fruit1" }
    // ]
    //printFunc1(FruitMap);

    FruitMap.Compact(); //把无效的都丢到后面
    // FruitMap == [
    //  { Key: 9, Value: "Fruit9" },
    //  { Key: 7, Value: "Fruit7" },
    //  { Key: 5, Value: "Fruit5" },
    //  { Key: 3, Value: "Fruit3" },
    //  { Key: 1, Value: "Fruit1" },
    //  <invalid>,
    //  <invalid>,
    //  <invalid>,
    //  <invalid>
    // ]
    FruitMap.Shrink();
    // FruitMap == [
    //  { Key: 9, Value: "Fruit9" },
    //  { Key: 7, Value: "Fruit7" },
    //  { Key: 5, Value: "Fruit5" },
    //  { Key: 3, Value: "Fruit3" },
    //  { Key: 1, Value: "Fruit1" }
    // ]
}


struct FMyStruct
{
	// String which identifies our key
	FString UniqueID;

	// Some state which doesn't affect struct identity
	float SomeFloat;

	explicit FMyStruct(float InFloat)
		: UniqueID(FGuid::NewGuid().ToString())
		, SomeFloat(InFloat)
	{
	}
};

template <typename ValueType>
struct TMyStructMapKeyFuncs :
	BaseKeyFuncs<
	TPair<FMyStruct, ValueType>,
	FString
	>
{
private:
	typedef BaseKeyFuncs<
		TPair<FMyStruct, ValueType>,
		FString
	> Super;

public:
	typedef typename Super::ElementInitType ElementInitType;
	typedef typename Super::KeyInitType     KeyInitType;

	static KeyInitType GetSetKey(ElementInitType Element)
	{
		return Element.Key.UniqueID;
	}

	static bool Matches(KeyInitType A, KeyInitType B)
	{
		return A.Compare(B, ESearchCase::CaseSensitive) == 0;
	}

	static uint32 GetKeyHash(KeyInitType Key)
	{
		return FCrc::StrCrc32(*Key);
	}
};

void Sample_7::TMapCustomKey()
{
	// 自定义key
	TMap < FMyStruct, int32,
		FDefaultSetAllocator, TMyStructMapKeyFuncs < int32 >> MyMapToInt32;

	// Add some elements
	MyMapToInt32.Add(FMyStruct(3.14f), 5);
	MyMapToInt32.Add(FMyStruct(1.23f), 2);

	// MyMapToInt32 == [
	//  {
	//      Key: {
	//          UniqueID:  "D06AABBA466CAA4EB62D2F97936274E4",
	//          SomeFloat: 3.14f
	//      },
	//      Value: 5
	//  },
	//  {
	//      Key: {
	//          UniqueID:  "0661218447650259FD4E33AD6C9C5DCB",
	//          SomeFloat: 1.23f
	//      },
	//      Value: 5
	//  }
	// ]
}

void Sample_7::TMultiMapFindItem()
{
	TMultiMap<int32, FString> mtMap1;
	mtMap1.Add(5, TEXT("aaa"));
	mtMap1.Add(3, TEXT("bbb"));
	mtMap1.Add(7, TEXT("ccc"));
	mtMap1.Add(6, TEXT("ddd")); //添加三个相同的key值得键值对
	mtMap1.Add(6, TEXT("eee"));
	mtMap1.Add(6, TEXT("fff"));

	// printFunc1(mtMap1);
	//printFunc2(mtMap1);
	//printFunc1(mtMap1)
	TArray<FString> values;
	mtMap1.MultiFind(6, values); //找出所以key为6的value，并丢到values数组中
	// values == ["fff","eee","ddd"]
	// printArrFunc1(values);

	//mtMap1.Empty();
}

void Sample_7::UE4Array()
{
	FString strVec[2] = {"aa", "bbb"};
	int32 num = ARRAY_COUNT(strVec); //使用宏ARRAY_COUNT统计数组里的个数
	int32 num2 = UE_ARRAY_COUNT(strVec);
}

void Sample_7::RemoveItemInForEach()
{
	/*
	for (int32 i = 0;i < mArr.Num (); ++i)
	{
		if (mArr [i] == 222)
		{
			mArr.RemoveAt (i);
		}
	}

	for (auto Iter = map1.CreateIterator (); Iter;++ Iter)
	{
		//UE_LOG(GlobalFuncLogger, Warning , TEXT("--- %d, %s"), Iter->Key , *Iter-> Value);
		if (Iter ->Key == 3)
		{
			Iter.RemoveCurrent ();
		}
	}
	*/
}
