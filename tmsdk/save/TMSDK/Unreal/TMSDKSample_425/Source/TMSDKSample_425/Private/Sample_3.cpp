// Fill out your copyright notice in the Description page of Project Settings.


#include "Sample_3.h"

Sample_3::Sample_3()
{
}

Sample_3::~Sample_3()
{
}

/*
共享指针可以置为null
访问共享指针时，需要判断是否有效
*/
void Sample_3::TestSharedPtr()
{
	TSharedPtr<TestSample3Data> myDataA;//声明
	myDataA = MakeShareable(new TestSample3Data());//分配内存
	if (myDataA.IsValid() || myDataA.Get()) //判断指针是否有效
	{    
		int a = myDataA->a;//访问
		TSharedPtr<TestSample3Data> myTestA = myDataA;//复制指针
		int count = myDataA.GetSharedReferenceCount();//获取共享指针的引用计数
		myTestA.Reset();//销毁对象
	}
}

void Sample_3::TestSharedRef()
{
	TSharedRef<TestSample3Data> myTestB(new TestSample3Data()); //创建共享引用 引用需要初始化
	int a = myTestB->a;
	int b = myTestB->b;
}

void Sample_3::TestSharedPtrAndRef()
{
	TestSample3Data* myTestC = new TestSample3Data();
	TSharedPtr<TestSample3Data> myTestA;
	TSharedRef<TestSample3Data> myTestB(new TestSample3Data());  //创建共享引用 需要初始化
	
	//共享引用转换为共享指针 支持隐式转换
	myTestA = myTestB;
	
	//普通指针转换为共享指针
	myTestA = MakeShareable(myTestC);
	
	//共享指针转换为共享指针，注意，共享指针不能为空
	if (myTestA.IsValid())  
	{
		myTestB = myTestA.ToSharedRef();
	}
}

void Sample_3::TestWeakPtr()
{
	TSharedPtr<TestSample3Data> myTestA = MakeShareable(new TestSample3Data());
	TSharedRef<TestSample3Data> myTestB(new TestSample3Data());
	TWeakPtr<TestSample3Data> myTestC;

	//共享指针转换为弱指针
	TWeakPtr<TestSample3Data> myTestC_A(myTestA);
	//共享引用转换为弱指针
	TWeakPtr<TestSample3Data> myTestC_B(myTestB);

	myTestC = myTestC_A;
	myTestC = myTestC_B;

	//弱指针可以重置为nullptr
	myTestC = nullptr;

	//弱指针转换为共享指针
	TSharedPtr<TestSample3Data> myTestA_A(myTestC.Pin());
	if (myTestA_A.IsValid() || myTestA_A.Get())
	{
		myTestA_A->a;
	}
}

void Sample_3::TestSharedPtrStaticCast()
{
	TSharedPtr<TestSample3Data> myBase;
	TSharedPtr<TestSample3DataDerived> myDerived = MakeShareable(new TestSample3DataDerived());
	myBase = myDerived;//隐式转换

	//基类转换为派生类
	TSharedPtr<TestSample3DataDerived> myDerived_A = StaticCastSharedPtr<TestSample3DataDerived>(myBase);
	if (myDerived_A.IsValid())
	{
		myDerived_A->b;
	}

	//派生类转换为基类 支持隐式转换
	const TSharedPtr<TestSample3Data> myConstBase = MakeShareable(new TestSample3DataDerived);
	TSharedPtr<TestSample3Data> myConstBase_A = ConstCastSharedPtr<TestSample3Data>(myConstBase);
	TSharedPtr<TestSample3DataDerived> myDerived_B = StaticCastSharedPtr<TestSample3DataDerived>(myConstBase_A);

	if (myDerived_B.IsValid())
	{
		myDerived_B->b;
	}

	/*
	共享引用
	TsharedPtr改为TSharedRef，
	StaticCastSharedPtr改为StaticCastSharedRef，
	ConstStaticCastSharedPtr改为ConstStaticCastSharedRef
	*/
}

void Sample_3::TestSharedFromThis()
{
	TSharedPtr<TestSample3DataA> myTestA= MakeShareable(new TestSample3DataA);
	myTestA->Print();

	//解引用
	TestSample3DataA* myTestB = myTestA.Get();
	//普通的指针转换为共享指针（只有当该类是继承自TSharedFromThis<>同时原本是共享指针的才可以使用AsShared()）
	//判断指针是否为空
	if (myTestB)
	{
		//
		TSharedPtr<TestSample3DataA> myTestC = myTestB->AsShared();
	}
}
