#pragma once
#ifndef _CHAPTER_6_H_
#define _CHAPTER_6_H_

#include <iostream>
#include <map>
#include <vector>
#include <set>

/*
自动 类型 推导

向编译器索取类型
*/

class Chapter_6
{
	//此处不能使用auto
	//auto i = 0;
	//auto x = 1.0;
	//auto str = "hello";


public:

	//auto 自动类型推导  需要携带表达式
	void TestAuto()
	{
		auto i = 0;
		auto x = 1.0;
		auto str = "hello";   //这里会被推到成 const char[6]  而不是 std::string

		std::map<int, std::string> m = { {1, "a"}, {2, "b"} }; //使用auto无法自动推导
		auto iter = m.begin();

		//auto err; //必须有赋值表达式


		
		//auto类型推导只用在 初始化 场合
		//即赋值初始化 或 花括号初始化（初始化列表、Initializer list）
		//auto 总是推导 值类型  不会是 引用
		//auto 可以附加 const、 volatile、 *、& 类型修饰符，得到新类型
		auto		x = 10L; //auto 推导为 long
		auto&		x1 = x;	  //x1 为 long&
		auto*		x2 = &x;  //x2 为 long*
		const auto& x3 = x;   //x3 为 const long&
		auto		x4 = &x3; // auto 推到为 const long*  x4 为 const long*

		auto		x5 = x1;  //x5是值类型 不是x的引用



	/*
	auto :  在变量声明时尽量多使用
			在遍历容器时使用，不需要关心容器的元素类型、迭代器返回值和首末地址  推荐使用 const auto& 或者 auto&
	*/

		std::vector<int> v = {2, 3, 4, 5, 7};
		for (const auto& i : v)               //常引用方式访问元素，避免拷贝代价
		{
			std::cout << i << ",";
		}
		for (auto& i : v)				     //引用方式访问元素，元素可以被改变
		{
			i++;
			std::cout << i << ",";
		}


#if HAS_CPP_14
		// auto作为函数返回值的占位符
		auto get_a_set()
		{
			std::set<int> s = {1, 2, 3};
			return s;
		}


		auto str14 = "hello world"s; //可以推导出std::string
#endif
	}


	//decltype后圆括号内是可用于计算类型的表达式（和sizeof类似）
	//和auto一样，也可以加上 cosnt  *  &修饰
	//decltype已经携带表达式了 后面不需要加表达式了  可以直接声明变量了
	void TestDecltype()
	{
		int x = 0;

		decltype(x)			x1;			//推导为int   x1 为 int
		decltype(x)&		x2 = x;		//x2为 int&    引用必须赋值
		decltype(x)*        x3;			//x3 为 int*
		decltype(&x)		x4;			//x4 为 int*
		decltype(&x)*		x5;			//x5 为 int**

		//decltype 不仅能够推导出值类型，还能够推导出引用类型，也就是表达式的“原始类型”   区分于auto
		decltype(x2)		x6 = x2;	//推导为 int&  x6为 int&  引用必须赋值    推导出了 x2的原始类型  


		//可以把decltype堪称一个真正的类型名
		using int_ptr = decltype(&x);  // int *
		using int_ref = decltype(x)&;  // int &


		//c++14  decltype(auto)
#if HAS_CPP_14
		int x = 0;
		decltype(auto) x1 = (x);  //推导为 int&   因为(expr)为引用类型 @注意

		decltype(auto) x2 = &x;  //int *
		decltype(auto) x3 = x1;  //int &

#endif


		/*
		decltype是 auto 的高级形式，
		更侧重于编译阶段的类型计算，
		所以常用在泛型编程里，获取各种类型，配合 typedef 或者 using 会更加方便。
		当需要一个“特殊类型”的时候

		在定义类时使用，此时auto不能使用
		*/

		// UNIX信号函数的原型
		void(*signal(int signo, void(*func)(int)))(int);
		// 使用decltype获取函数指针
		using sig_func_ptr_t = decltype(&signal);
	}

	public:
		using set_type = std::set<int>;  //集合类型别名
	private:
		set_type  m_set;

		using iter_type = decltype(m_set.begin());		//计算表达式类型，定义别名
		iter_type m_pos;

};

#endif  //_CHAPTER_6_H_