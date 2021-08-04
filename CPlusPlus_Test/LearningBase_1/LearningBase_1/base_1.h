#pragma once

class Base_1
{
public:
	static int* array_int;
};



/*
在重载父类的同名虚函数时会出现hides overloaded virtual function编译警告。

重载的虚函数被隐藏了

首先是发生了重载，子类重载了父类的函数，其次被重载的是虚函数，这时这个被重载的父类的虚函数将会被隐藏。
*/

struct Base
{
	virtual void* get(const char* e);
	//virtual void * get(char* e, int index);
};

struct Derived: public Base
{
	using Base::get;
	virtual void* get(const char* e, int index);

//放到私有位置 避免调用Base的get
//private:
//	using Base::get;
};



/*
函数的默认参数：

	1.如果某个参数是默认参数，那么它后面的参数必须都是默认参数
	2.默认参数可以放在函数声明或者定义中，但只能放在二者之一
	3.函数重载时谨慎使用默认参数值

*/
class TestFunc_1
{
private:
	static int getValue()
	{
		return 1;
	}

	int _func(int a, int b = getValue())
	{
		return a + b;
	}

	int _func(int a, int b, int c);

public:
	void Test1();
};



/*
explicit keyword

1.指定构造函数或转换函数（C++11),推导指引（C++17）为显示，不能进行隐式转换和复制初始化
2.可以与const表达式一起使用，当且仅当const表达式为true时，函数为显示
*/
#include <iostream>
class TestExplicit_1
{
public:
	int m_x, m_y;
	TestExplicit_1(int x = 0, int y = 0)
		:m_x(x), m_y(y) {}

	operator bool() const { return true; }

	static void Print(const TestExplicit_1& te)
	{
		std::cout << "(" << te.m_x << ","
			<< te.m_y << ")" << std::endl;
	}
};

class TestExplicit_2
{
public:
	int m_x, m_y;
	explicit TestExplicit_2(int x = 0, int y = 0)
		:m_x(x), m_y(y) {}

	explicit operator bool() const { return true; }

	static void Print(const TestExplicit_2& te)
	{
		std::cout << "(" << te.m_x << ","
			<< te.m_y << ")" << std::endl;
	}
};