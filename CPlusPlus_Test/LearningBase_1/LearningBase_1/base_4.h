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
};
