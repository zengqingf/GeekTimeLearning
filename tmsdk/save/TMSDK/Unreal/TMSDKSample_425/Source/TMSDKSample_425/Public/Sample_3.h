// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

/**
* link: https://zhuanlan.zhihu.com/p/94198883
* 
 * 避免将数据作为TSharedRef或TSharedRef参数传到函数，
	 此操作将因取消引用和引用计数而产生开销。
	 相反，建议将引用对象作为const &进行传递。
 * 
 * 
 * 共享指针与虚幻对象(UObject及其衍生类)不兼容。引擎具有UObject管理的单独内存管理系统，两个系统未互相重叠。
 * 
 * 
 * 可将共享指针向前声明为不完整类型。
 */
class TMSDKSAMPLE_425_API Sample_3
{
public:
	Sample_3();
	~Sample_3();

	void TestSharedPtr();
	void TestSharedRef();
	void TestSharedPtrAndRef();
	void TestWeakPtr();
	void TestSharedPtrStaticCast();
	void TestSharedFromThis();
};


class TestSample3Data
{
public:
	int a;
	float b;
};

class TestSample3DataDerived : public TestSample3Data
{
public:
	int c;
};


/*
TSharedFromThis是支持线程安全的。
TSharedFromThis里面会保存一个弱引用，可以通过这个弱引用转换为共享指针。
*/
class TestSample3DataA : public TSharedFromThis<TestSample3DataA>
{
public:
	void Print() {}
};