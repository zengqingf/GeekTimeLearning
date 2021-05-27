#pragma once

#ifndef _CHAPTER_3_H_
#define _CHAPTER_3_H_

#include <set>
#include <vector>
#include <string>
#include <iostream>
using std::cout;
using std::endl;

/*
面向对象
核心是：抽象和封装
衍生出：继承和多态（不完全符合现实世界）

设计类时尽量少用继承和虚函数

内部嵌套类：高内聚-->强耦合
定义一个新的名字空间，把内部类都“提”到外面，降低原来类的耦合度和复杂度。
*/

//final 标识符：非关键字，用于类定义中，显式禁用继承
class DemoClass final
{
};



//建议只用Public继承，避免使用virtual（虚继承）、protected
class Interface        // 接口类定义，没有final，可以被继承
{};

class Implement final : // 实现类，final禁止再被继承
	public Interface    // 只用public继承
{};


//Big Three   拷贝构造、拷贝赋值、析构函数
//C++11  转移构造、转移赋值
//现代C++   三个析构、两个赋值、一个析构

//C++11新增 =default（要求编译器使用默认实现）  =delete（禁用某个函数）
//C++有隐式构造和隐式转型的规则，如果类中有单构造函数或者转型操作符(operator)，为防止意外类型转换，使用explicit标记函数为显式
class DemoClass2 final
{
public:
	DemoClass2() = default;  // 明确告诉编译器，使用默认实现
	~DemoClass2() = default;  // 明确告诉编译器，使用默认实现

	DemoClass2(const DemoClass2&) = delete;              // 禁止拷贝构造
	DemoClass2& operator=(const DemoClass2&) = delete;   // 禁止拷贝赋值

	//显示单参构造函数
	explicit DemoClass2(const std::string& str) {}
	//显示转型为bool
	explicit operator bool() {}                         //默认operator bool() {} 为隐式转换
};


//C++11 新特性

//委托构造：一个构造函数直接调用另一个构造函数
class DemoDelegating final
{
private:
	int a;
public:
	DemoDelegating(int x) : a(x) {}
	DemoDelegating() : DemoDelegating(0) {}
	DemoDelegating(const std::string& s) : DemoDelegating(std::stoi(s)) {}
};


//成员变量初始化：避免在构造函数初始列中添加长串成员变量
class DemoInit final                    // 有很多成员变量的类
{
private:
	int                 a = 0;          // 整数成员，赋值初始化
	std::string         s = "hello";    // 字符串成员，赋值初始化
	std::vector<int>    v{ 1, 2, 3 };   // 容器成员，使用花括号的初始化列表
public:
	DemoInit() = default;               // 默认构造函数
	~DemoInit() = default;              // 默认析构函数
public:
	DemoInit(int x) : a(x) {}           // 可以单独初始化成员，其他用默认值
};


//类型别名
using uint_t = unsigned int;     //using别名
typedef unsigned int uint_t;     //等价于上行

//在类中可以对外部类型 （标准库、第三方库、自定义类型， 使用using起别名）
//引入语法层面的宏定义，可以随机更换成其他类型
class KafkaConfig {};
class DemoClass3 final
{
public:
	using this_type = DemoClass;					// 给自己也起个别名
	using kafka_conf_type = KafkaConfig;			// 外部类起别名

public:
	using string_type = std::string;				// 字符串类型别名      可以替换为 string_view  c++17
	using uint32_type = uint32_t;					// 整数类型别名

	using set_type = std::set<int>;					// 集合类型别名		   可以替换为 unordered_set 
	using vector_type = std::vector<std::string>;	// 容器类型别名

private:
	string_type     m_name = "tom";					// 使用类型别名声明变量
	uint32_type     m_age = 23;						// 使用类型别名声明变量
	set_type        m_books;						// 使用类型别名声明变量

private:
	kafka_conf_type m_conf;							// 使用类型别名声明变量
};



#endif //_CHAPTER_3_H_