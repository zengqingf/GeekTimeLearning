// LearningBase_1.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include <iostream>
//#include <iostream.h> //for c++ 当前版本不可用

//#include <cstdio>   //for c 可用
//#include <stdio.h>  //for c 可用


#include "complex_test.h"

using namespace std;

int main()
{
    std::cout << "Hello World!\n";
	int i = 8;
	cout << "i=" << i << endl;
	
	//for c
	printf("i = %d \n", i);


	const complex c1(2, 1);
	cout << c1.real();  //real() 需要const 声明
	cout << c1.imag();


	complex c2(2, 1);
	complex c3;
	c3 += c2;
	cout << c3.real();
	

	return 0;
}

// 运行程序: Ctrl + F5 或调试 >“开始执行(不调试)”菜单
// 调试程序: F5 或调试 >“开始调试”菜单

// 入门使用技巧: 
//   1. 使用解决方案资源管理器窗口添加/管理文件
//   2. 使用团队资源管理器窗口连接到源代码管理
//   3. 使用输出窗口查看生成输出和其他消息
//   4. 使用错误列表窗口查看错误
//   5. 转到“项目”>“添加新项”以创建新的代码文件，或转到“项目”>“添加现有项”以将现有代码文件添加到项目
//   6. 将来，若要再次打开此项目，请转到“文件”>“打开”>“项目”并选择 .sln 文件



/*

C++

类的大致分类
1. 基于对象 Object based      不带指针  大多数不用写析构函数
2. 面向对象 Object Oriented	  带指针

 
代码形式
1. 头文件   .h
2. 实现文件 .cpp
3. 标准库  .h
扩展名不一定是上述 也可以是.hpp 等其他 或者 没有扩展名

引用方式
#include<iostream.h>  引用标准库
#include "complex.h"  引用自定义头文件

*/

