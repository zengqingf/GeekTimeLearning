#pragma once

#ifndef _CHAPTER_10_H_
#define _CHAPTER_10_H_

#include <iostream>
#include <string>
#include <vector>
#include <algorithm>
#include <functional>

/*
函数指针

C:     C中的函数均为开放式， 函数指针赋值和调用方式
		int(*pfunc1)() = fun1;				//赋值1
		int a = pfunc1();					//调用1

		void(*pfunc2)(int, char) = NULL;   
		pfunc2 = &fun2;						//赋值2
		pfunc2(1, 'a');						//调用2
		(*pfunc2)(2, 'b');					//调用3

		返回类型 (*函数指针名称)(参数类型,参数类型,参数类型，…);

C++:
		返回类型 （类名称::*函数成员名称）（参数类型，参数类型，参数类型，….);


		C++中函数虽然有函数类型，但是没有对应类型的变量，不能直接操作，需要通过指针间接操作（函数指针）
		C++中函数都是全局的，没有生存周期概念（static、命名空间作用较弱，只能简单限制了应用范围，避免名字冲突）

lambda:  （闭包 closure: 活的代码块（函数），在定义时保存了定义时捕获的外部变量，就可以跳离定义点，把这段代码"打包"传递到其他地方执行, 例：多个函数传递到一个统一函数中执行，函数不需要按顺序执行）
		lambda表达式是一个变量，方便在调用点就地定义函数，限制其作用域和生命周期，实现函数局部化
		可以把多个简单功能的小lambda表达式组合，变成一个复杂的大lambda表达式，实现功能复合
		lambda表达式可以捕获外部变量，在内部代码直接操作

		使用环境：
			可以代替普通函数
*/
class Chapter_10
{
private:
	void _testFunc(int x)
	{
		std::cout << x + x << std::endl;
	}
	void _testFunc2(double a) const
	{
		std::cout << a * a << std::endl;
	}
	static std::string _testFunc3(const char buf[])
	{
		std::cout << buf << std::endl;
	}


	float _add(float a, float b) { return a + b; }
	float _minus(float a, float b) { return a - b; }

public:

	/*
	函数名就相当于函数指针，但对于成员函数，就必须要加上&

	两者运行效果一样
	auto pfunc = &my_square;
	auto pfunc = my_square;
	*/

	//测试成员函数的函数指针
	void TestFuncPtr()
	{
		void(Chapter_10::*pfunc)(int) = &Chapter_10::_testFunc;  // 一定要加类名 和 取地址符
		Chapter_10 c10;
		(c10.*pfunc)(3);										 //调用时一定要加类的对象名 和 *

		//可以使用auto 简化
		auto p = &Chapter_10::_testFunc;
		(c10.*p)(4);
	}

	void TestFunc2Ptr()
	{
		void(Chapter_10::*pfunc2)(double)const = NULL;			//一定要加const
		pfunc2 = &Chapter_10::_testFunc2;
		Chapter_10 c10_2;
		(c10_2.*pfunc2)(2.33);
	}

	//类似于C的指针函数
	//https://www.cnblogs.com/TenosDoIt/p/3164081.html
	void TestFunc3Ptr()
	{
		std::string(*pfunc3)(const char buf[]) = NULL;
		pfunc3 = Chapter_10::_testFunc3;					//可以不加取地址符号
		pfunc3("hello");
		pfunc3 = &Chapter_10::_testFunc3;
		(*pfunc3)("world");
	}


	//C++ 函数指针作为参数
	void TestFunc4Ptr(void(Chapter_10::*pfunc)(int), int a)
	{
		Chapter_10 c10;
		(c10.*pfunc)(a);
	}

	//C++ 函数指针作为返回值
	// 函数参数为int \ char 
	// 函数返回值为函数指针 void(Chapter_10::*)(double)
	void (Chapter_10::*pfunc2(int a, char c))(double d) const
	{
		std::cout << "return func ptr" << std::endl;
		return _testFunc2;
	}
	/*
	使用
	void (Chapter_10::*p)(double) = pfunc2(1, 'a');
	p(2.33);
	*/

	typedef float(Chapter_10::*pfuncType)(float, float);

	//测试函数指针数组
	void TestFunc5Ptr()
	{
		float(Chapter_10::*pfuncArray[2])(float, float) = { &_add, &_minus };
		Chapter_10 c10;
		double k = (c10.*pfuncArray[0])(2.33, 3.33);
		k = (c10.*pfuncArray[1])(3.33, 2.33);


		//简化函数指针类型  
		//当函数指针较简单时，也可以使用auto进行简化
		pfuncType p = &_add;
		(c10.*p)(2.33, 3.33);

		pfuncType pArray[2] = { &_add, &_minus };
		//...
	}


	//lambda表达式
	//每个lambda表达式都有一个独特的类型，只有编译器才知道，声明定义时必须使用auto，推荐匿名使用lambda表达式
	//lambda表达式调用完后就不存在了（除非被拷贝保存下来），减少了影响范围
	//lambda表达式可以嵌套定义
	//lambda表达式可以在函数内外都可以定义
public:

	void TestLambda()
	{
		auto f1 = []() {};  //相当于空函数

		auto f2 = []()										//必须使用
		{
			std::cout << "lambda f2" << std::endl;

			auto f3 = [](int x)								//嵌套定义lambda表达式
			{
				return x * x;
			}; // lambda f3									//使用注释显式说明表达式结束

			std::cout << f3(10) << std::endl;

		};//lambda f2										//使用注释显式说明表达式结束



		std::vector<int> v = { 5, 2, 1, 13, 14 };
		std::cout << *std::find_if(begin(v), end(v),	//标准库中的查找算法
			[](int x)									//匿名lambda表达式，不需要auto赋值
			{
				return x >= 5;							// 用做算法的谓词判断条件
			}											// lambda表达式结束
		)
			<< std::endl;								// 语句执行完，lambda表达式就不存在了
	}


	/*
	lambda 的变量捕获

	“[=]”表示按值捕获所有外部变量，表达式内部是值的拷贝，并且不能修改；
	“[&]”是按引用捕获所有外部变量，内部以引用的方式使用，可以修改；
	在“[]”里明确写出外部变量名，指定按值或者按引用捕获

	使用“[=]”按值捕获的时候，lambda 表达式使用的是变量的独立副本，非常安全。
	而使用“[&]”的方式捕获引用就存在风险，当 lambda 表达式在离定义点“很远的地方”被调用的时候，
				引用的变量可能发生了变化，甚至可能会失效，导致难以预料的后果。

	使用建议：
		对于“就地”使用的小 lambda 表达式，可以用“[&]”来减少代码量，保持整洁；
		对于非本地调用、生命周期较长的 lambda 表达式应慎用“[&]”捕获引用，
		最好是在“[]”里显式写出变量列表，避免捕获不必要的变量。
	*/
	void TestLambda2()
	{
		int x = 33;
		auto f1 = [=]()		//用= 表示按值捕获
		{
			//x += 10;      //x只读， 不允许修改
		};

		auto f2 = [&]()		//用& 表示按引用捕获
		{
			x += 10;		//x是引用，可以修改
		};

		auto f3 = [=, &x]()  //用&表示按引用捕获 x ， 其他按值捕获
		{
			x += 20;		  //x是引用 可以修改
		};
	}

private:
	int m_x = 0;

public:
	auto Print()												//返回一个lambda表达式 供外部使用
	{
		return [this]()											//显式捕获 this指针
		{
			std::cout << "member = " << m_x << std::endl;
		};
	}
	/*
	调用：
	Chapter_10 c10;
	auto f = c10.Print();
	f();
	*/


	//lambda泛型化
	void TestLambda3()
	{
#if __cplusplus >= 201402
		auto f = [](const auto& x)  //参数使用auto声明，泛型化       
							//同时注意 用到了const &  避免了对象拷贝， 但在[]捕获中不能捕获常量引用
		{
			return x + x;
		};

		std::cout << f(3) << std::endl;
		std::cout << f(2.33) << std::endl;
		std::string str = "hello";
		std::cout << f(str) << std::endl;
#endif
	}


	//function + lambda  实现成员函数
	public:
		using func_type = std::function<void()>;

	public:
		func_type Print = [this]()
		{
			std::cout << "value = " << m_value << std::endl;
			std::cout << "hello function+lambda" << std::endl;
		};
	private:
		int m_value = 10;

	/*
	调用
	Chapter_10 c10;
	c10.Print()
	*/



	//std::function 的默认值
	//用 " {} " 表示
	public:
		void func(const std::function<void()>& f = {}) {
			if (f) {
				f();
			}
		}
};


#endif //_CHAPTER_10_H_ 