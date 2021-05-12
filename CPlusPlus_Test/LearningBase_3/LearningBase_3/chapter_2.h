#pragma once

#ifndef _CHAPTER_2_H_
#define _CHAPTER_2_H_

#include <iostream>
using std::cout;
using std::endl;

template<int N>
struct fib                   // 递归计算斐波那契数列
{
	static const int value =
		fib<N - 1>::value + fib<N - 2>::value;
};

template<>
struct fib<0>                // 模板特化计算fib<0>
{
	static const int value = 1;
};

template<>
struct fib<1>               // 模板特化计算fib<1>
{
	static const int value = 1;
};

static void test_fib()
{
	// 调用后输出2，3，5，8
	cout << fib<2>::value << endl;
	cout << fib<3>::value << endl;
	cout << fib<4>::value << endl;
	cout << fib<5>::value << endl;
}



//属性    standard from c++11    编译器更早提供了一些
[[noreturn]]									//属性标签
static int func(bool flag)						//函数绝不会返回任何值
{	
	throw std::runtime_error("XXX");
}

[[deprecated("deadline:2021-12-31")]] // c++14 or later
static int old_func() {}


//静态断言 

//一般断言，运行时判断，可以读取运行时变量
#include <assert.h>
static void func_assert(int i)
{
	assert(i > 0 && "i must be greater than zero");
	int *p = &i;
	assert(p != nullptr);

	//静态断言，编译时生效，运行时不可见
	//static_assert();
}

//使用静态断言，保证模板参数必须 >= 0
template<int N>
struct fib
{
	static_assert(N >= 0, "N >= 0");

	static const int value =
		fib<N - 1>::value + fib<N - 2>::value;
};

//需要保证程序只在64bit系统上运行，程序存在long，则需要保证long大小大于8byte
static void func_static_assert()
{
	static_assert(
		sizeof(long) >= 8, "must run on x64"
		);

	static_assert(
		sizeof(int) == 4, "int must be 32bit"
		);

	//不能读取运行期变量
	//char* p = nullptr;
	//static_assert(p == nullptr, "some error.");  // 错误用法
}

template<typename T>
struct TestStaticAssert
{

	// 假设T是一个模板参数，即template<typename T>

	//把类当成函数，把模板参数当成函数参数，把“::”当成 return 返回值
	static_assert(
		is_integral<T>::value, "int");

	static_assert(
		is_pointer<T>::value, "ptr");

	static_assert(
		is_default_constructible<T>::value, "constructible");
};


#endif //_CHAPTER_2_H_