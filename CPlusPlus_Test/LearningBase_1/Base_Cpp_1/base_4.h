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

	void Test1() override {}
	void Test2() {}
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


/*
ref: https://zhuanlan.zhihu.com/p/99532906
类型擦除：类型擦除就是将原有类型消除或者隐藏

多态：派生类隐式转换为基类，再通过基类去调用多态
模板：对不同类型的共同行为进行抽象，不同类型之间不需要通过继承即可获得共同行为，但是基本类型在使用时还是需要指定
容器：boost.variant可以指定容纳多种类型
	typedef boost::variant<double, int, uint32_t, sqlite3_int64, char*, blob, NullType>Value;
	vector<Value> vec;
	vec.push_back(1); vec.push_back("test"); vec.push_back({"hello", 5});
	类型需要事先指定，并且只能指定预先设置好的类型组

通用类型：boost::any 类似于c#和java中的object类型，取值时还是需要指定具体类型
闭包（lambda）
*/

#include <map>
#include <functional>
namespace TypeErasure_1
{
	class TestTypeErasure
	{
		std::map<int, std::function<void()>> m_freeMap;
		template<typename R, typename T>
		R GetResult()
		{
			//R result = GetTable<R, T>();
			//m_freeMap.insert(std::make_pair(result.sequenceId, [this, result]() //()可以省略
			//{
			//	FreeResultById(result);
			//}));
		}

		bool FreeResultById(int& memId) {
			auto it = m_freeMap.find(memId);
			if (it == m_freeMap.end())
				return false;
			it->second();
			m_freeMap.erase(memId);
			return true;
		}
	};
}