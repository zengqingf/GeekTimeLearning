#pragma once

/*
c++  类型转换

　　去const和volatile属性用const_cast。
　　基本类型转换用static_cast。
　　多态类之间的类型转换用daynamic_cast。
　　不同类型的指针类型转换用reinterpreter_cast。
*/

#include <iostream>

#include <algorithm>

class Base_A
{
public:
	int m_iNum;
	virtual void Test1() { }
};

class Base_AA
{
public:
	virtual void Test2() {}
};

class Derived_B : public Base_A
{
public:
	char *m_szName[100];
};

class Derived_C : public Base_A
{

};

//多继承时的向下转型
class Derived_D : public Base_A, public Base_AA
{
	void Test1() override {}
	void Test2() override {}
};

class TypeCast_Test_1
{
public:
	void Test1();

	//static_cast : https://zh.cppreference.com/w/cpp/language/static_cast
	void Test2();
};



//测试 函数模板 子类override父类
template<typename D>
class Base_BB
{
	template<typename T>
	std::string _method() { return "Base"; }
public:

	template<typename T>
	std::string method()
	{
		return static_cast<D&>(*this).template _method<T>();
	}
};

class Derived_BB : public Base_BB<Derived_BB>
{
	friend class Base_BB<Derived_BB>;

	template<typename T>
	std::string _method() { return "Derived"; }
public:
	//...
};



//测试2 函数模板 子类override父类
struct BaseOfBase
{
	virtual void do_something() = 0;
};

template <typename T>
struct Base_CC : public BaseOfBase
{
	T val;

	void do_something() override
	{
		std::cout << "Base::do_something() [" << val << "]\n";
	};
};

template <typename T>
struct Derived_CC : public Base_CC<T>
{
	void do_something() override
	{
		std::cout << "Derived::do_something() [" << this->val << "]\n";
	}
};
