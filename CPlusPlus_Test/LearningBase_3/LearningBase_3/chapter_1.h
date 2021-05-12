#pragma once    //不推荐使用 ！！！

//预处理  不属于C++语言  由预处理器处理  不受C++语法规则约束
//g++ -E xxx.cpp -o xxx.i   预编译命令，可以观察xxx.i中处理的结果
#
#if __linux__
#define HAS_LINUX 1
#endif
#

#ifndef _CHAPTER_1_H_
#define _CHAPTER_1_H_

#include <stdint.h>
#include <iostream>
using std::cout;
using std::endl;

static uint32_t calc_table[] = {
#include "test_calc_values.inc"   //通过#include 隐藏细节
};

//使用宏的时候一定要谨慎，时刻记着以简化代码、清晰易懂为目标，不要“滥用”，避免导致源码混乱不堪，降低可读性。
//因为宏的展开、替换发生在预处理阶段，不涉及函数调用、参数传递、指针寻址，没有任何运行期的效率损失，
//所以对于一些调用频繁的小代码片段来说，用宏来封装的效果比 inline 关键字要更好，因为它真的是源码级别的无条件内联。

//宏是没有作用域概念的，永远是全局生效

static void test_define_temp()
{
#define CUBE(a) (a) * (a) * (a) //定义一个求立方的宏

	std::cout << CUBE(10) << std::endl;	 //使用宏简化代码
	std::cout << CUBE(13) << std::endl;

#undef CUBE			//取消定义，只作为临时作用的宏使用

}

#ifdef AUTH_PWD
#undef AUTH_PWD
#endif
#define AUTH_PWD "xxx"  //重新定义宏

//可以使用宏定义常量，需要注意适量，源码阶段（编译之前）真正的常量
#define MAX_BUF_LEN 65535
#define VERSION "1.0.1.0"

//使用宏的（文本替换功能），代替直接定义命名空间
#define BEGIN_NAMESPACE(x) namespace x {
#define END_NAMESPACE(x) }

BEGIN_NAMESPACE(my_ns)
//...
END_NAMESPACE(my_ns)


//条件编译
#ifdef __cplusplus   // 定义了这个宏就是在用C++编译
extern "C" {		 // 函数按照C的方式去处理
#endif
	void a_c_function(int a);
#ifdef __cplusplus
}
#endif


//除了“__cplusplus”，C++ 里还有很多其他预定义的宏，像源文件信息的“FILE”“ LINE”“ DATE”，
//以及一些语言特性测试宏，比如“__cpp_decltype” “__cpp_decltype_auto” “__cpp_lib_make_unique”等。

//gcc g++编译器提供的其他宏定义
//g++ -E -dM - < /dev/null

#if __cplusplus >= 201402      //检查c++标准版本    //201402 ==> c++14
std::cout << "c++14 or later" << std::endl;
#elif __cplusplus >= 201103							//201103 ==> c++11
std::cout << "c++11 or later" << std::endl;
#else   //__cplusplus < 201103						//199711 ==> c++98
# error "c++ is too old"
#endif  //__cplusplus >= 201402 


//通过定制条件编译宏，实现操作系统定制化
//使用 Shell 脚本检测外部环境，生成一个包含若干宏的源码配置文件，再条件编译包含不同的头文件，实现操作系统定制化
static void test_condition_define()
{
#if defined(__cpp_decltype_auto)        //检查是否支持decltype(auto)
	cout << "decltype(auto) enable" << endl;
#else
	cout << "decltype(auto) disable" << endl;
#endif  //__cpp_decltype_auto

#if __GNUC__ <= 4
	cout << "gcc is too old" << endl;
#else   // __GNUC__ > 4
	cout << "gcc is good enough" << endl;
#endif  // __GNUC__ <= 4

#if defined(__SSE4_2__) && defined(__x86_64)
	cout << "we can do more optimization" << endl;
#endif  // defined(__SSE4_2__) && defined(__x86_64)
}


//注释代码

#if 0
//禁用代码
#endif

#if 1
//启用代码
#endif


//尽量把平台相关的代码集中在一起，然后用#include一次包含

#endif //_CHAPTER_1_H_