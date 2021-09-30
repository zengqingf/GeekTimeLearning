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


/*
测试指针被外部类引用，生命周期结束释放后仍被外部引用着，导致闪退

现象分析：
1.对象指针被delete后，调用这个对象的非静态方法，可以调用，
	但是对象内部的变量都被回收了（且不为默认值）
2.如果对象的类有父类，则其他类的对象中缓存了从外部传入的子类中获得的父类指针，
	外部子类对象被销毁后，可以调用纯父类方法，但是不能调用子类重写的方法
*/

/*
ref:https://www.zhihu.com/question/389546003
虚表：
	
一、函数调用方式：
	1.静态绑定：
	类不含虚表时，编译器在编译期间把函数地址确定好，运行期间直接去调用相应地址的函数

		C++中数据和操作的封装仅针对开发而言，编译器编译后还是面向过程，
		编译器会给成员函数一个额外的类指针参数，运行期间传入对象实际的指针
		类的数据（成员变量）和操作（成员函数）其实还是分离的
		
	2.动态绑定（延迟绑定）
		
		如果不使用虚表（virtual关键字），由于类的数据（成员变量）和操作（成员函数）其实是分离的，
		从对象的内存分布看，只有成员变量，看不到成员函数，
		因为调用哪个函数在编译期间已经确定了，同时编译期间只能识别父类的函数（可能会被子类重写的）

虚表时连续的内存块

面向对象中多态的实现：
	
*/

#include <vector>
class ITA
{
public:
	ITA() {}
	virtual ~ITA() {}
	virtual void Deal() = 0;
	void ITAA() {
		std::cout << "ITAA()" << std::endl;
	}
};

class TA : public ITA
{
public:
	TA() : a(0){}
	TA(int i) :a(i) {}
	~TA() {
		std::cout << "TA dtor call..." << std::endl;
	}

	static void* operator new(size_t);
	static void* operator new(size_t, void*);
	static void operator delete(void *);

	void Deal() override
	{
		std::cout << "Del TestA(): " << a << std::endl; 
	}
	int a = 0;
};

class TB
{
public:
	TB() {}
	~TB() {}

	void StoreTA(TA* a)
	{
		mTAs.push_back(a);
	}

	void ReleaseTA()
	{
		for (ITA* ta : mTAs) {
			if (nullptr != ta) {
				std::cout << "ta not nullptr" << std::endl;
				ta->Deal();
				//ta->ITAA();
				std::cout << "ta address: " << ta << std::endl;
			}
		}
		mTAs.clear();
	}

private :
	std::vector<ITA*> mTAs;
};