#pragma once

/*
link: https://blog.csdn.net/u013412391/article/details/107453635

智能指针测试
*/

#include <iostream>
using std::cout;
using std::endl;

class TestObject //测试用对象
{
public:
	//构造函数
	TestObject()
	{
		cout << "TestObject ctor" << endl;
	}
	//析构函数
	~TestObject()
	{
		cout << "TestObject dtor" << endl;
	}
	//测试函数
	void TestFunc()
	{
		cout << "TestFunc" << endl;
	}
};


template<class T>
class TestSmartPointer
{
private:
	class Counter
	{
	public:
		T* objectPtr;
		int referenceCount;

		Counter(T* ptr)
		{
			objectPtr = ptr;
			referenceCount = 1;
		}

		~Counter()
		{
			delete objectPtr;
		}
	};

private:
	Counter* myCounter;

public:
	TestSmartPointer(T* ptr)
	{
		myCounter = new Counter(ptr);
		cout << "ctor by base ptr" << endl;
	}

	TestSmartPointer(const TestSmartPointer &other)
	{
		myCounter = other.myCounter;
		myCounter->referenceCount++;
		cout << "ctor by test smart ptr, ref count is " << myCounter->referenceCount << endl;
	}

	~TestSmartPointer()
	{
		myCounter->referenceCount--;
		cout << "test smart ptr dtor, ref count is " << myCounter->referenceCount << endl;
		if (myCounter->referenceCount == 0)
		{
			delete myCounter;
		}
	}

	T* operator->() const
	{
		return myCounter->objectPtr;
	}
};