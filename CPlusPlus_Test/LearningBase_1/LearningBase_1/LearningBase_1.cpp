// LearningBase_1.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。

#define _CRT_SECURE_NO_WARNINGS

#include <iostream>
//#include <iostream.h> //for c++ 当前版本不可用

//#include <cstdio>   //for c 可用
//#include <stdio.h>  //for c 可用


#include "complex_test.h"
#include "string_test.h"

using namespace std;


complex global_c(1, 2);//《全局 stack对象》

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


	complex c2(2, 1);   //c2 所占用的空间 来自于 stack    《临时 stack对象》
	complex c3;
	c3 += c2;
	cout << c3.real() << '\n';

	complex *p = new complex(3);  //complex(3)是一个《临时 heap对象》，其所占的空间是 以new 自 heap动态分配而得，并且由 p 指向
	delete p;                     //动态获得的对象，从heap获取，离开作用域时，需要手动delete  否则会内存泄露 （memory leak）
							      //原因：如果不进行delete, 当作用域结束时, p所指的heap object 仍然存在，但是指针p的生命周期结束了，作用域外无法访问到p，没机会再delete p


	static complex c4(2, 1);   //《静态 stack对象》




	//new 操作 ==> 编译后的实现
	complex *pc;
	void* mem = operator new (sizeof(complex));   //operator new 是一个函数名称 ：   内部调用 malloc(n)
	pc = static_cast<complex*>(mem);              //转型
	pc->complex::complex(1, 2);					  //构造函数 (构造函数可以这么调用)
	cout << pc->imag() << endl;

	
	String *ps = new String("Hello");
	//delete ps;

	//delete操作 ==> 编译后的实现              
	ps->~String();								//调用析构函数
	operator delete(ps);                        //operator delete 是一个函数名称：  内部调用 free(ps)

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



/*
内存

stack (栈)   heap (堆)

Stack:  存在于某作用域（scope）的一块内存空间（memory space）
		如：当调用函数时，函数本身就会形成一块stack用来存放它所接收的参数以及返回地址

		在函数本体（function body）内声明的任何变量，其所使用的内存块都是取自上述stack.


Heap:  （system heap） 由操作系统提供的一块Global内存空间，程序可动态（dynamic allocated）从中获取若干区块（blocks）


*/

/*
stack objects 的生命周期
stack object 生命周期 在作用域结束（scope）结束之际结束
又称为 auto object  因为会被自动 清理（调用析构函数）


static local objects 的生命周期
生命周期在作用于结束后 仍然存在，直到整个程序结束


global objects 的生命周期
生命周期在程序结束后才结束， 可以视为一种 static object



heap objects的生命周期
生命周期在被deleted之后结束

*/


/*
new 操作： 先分配memory，再调用ctor

例如：
complex *p = new complex(1, 2);
delete p;

编译器转换为：

complex *p;
void* mem = operator new (sizeof(complex));   //operator new 是一个函数名称 ：   内部调用 malloc(n)
pc = static_cast<complex*>(mem);              //转型
pc -> complex::complex(1,2);				  //构造函数  全名是 complex::complex(pc, 1, 2);  构造函数是成员函数 默认隐藏 this  示例中this即为pc

*/

/*
delete 操作： 先调用dtor （析构函数），再释放memory

如果没有自定义析构 编译器会调用默认的析构函数（空操作）

例如：
String *ps = new String("Hello");
delete ps;

编译器转换为：
String::~String(ps);   //析构函数
operatior delete(ps);  //释放内存

上述由两个delete操作 析构函数内部一个
*/


/*
动态分配所得的内存块 (memory block), in VC  (VC分配的内存块 一定是16的倍数 )

调试模式下和非调试模式 
所占用内存块的大小不同

内存块包含 头和尾cookie字节（4个字节 * 2）
作用：回收内存时，确定需要被回收的内存块的大小

cookie字节的大小 是 当前区块占用内存块的字节数 （如 64 的十六进制表示 40） 用最后一个bit的0和1 作为 占用和释放的标识
可以用最后一位作为标识的原因：分配内存块大小 已经按照16的倍数分配了 最后4个bit都是0 所以可以借用其中一个bit表示

malloc 和 free 函数约定 每个内存块的头尾cookie的值

例如 complex创建时 占用内存 8个字节 （两个double 各为4个字节）
     String创建时 占用内存 4个字节  （一个指针占用 4个字节）

	 complex:
	 debug mode:    8 + (32 + 4) + (4 * 2) = 52 ==> + （4pad * 3） = 64                             
	 00000041         --> 表示给出（被占用）表示为 41  如果是回收 则为 40
	 00790c20
	 00790b80
	 0042ede8
	 0000006d

	 00000002
	 00000004
	 4个0xfd
	 <complex object(8h)>
	 4个0xfd
	 00000000(pad)
	 00000000(pad)
	 00000000(pad)
	 00000041

	 normal mode:   8 + (4 * 2) = 16
	 00000011
	 <complex object(8h)>
	 00000011



     String:
	 debug mode:    4 + (32 + 4) + (4 * 2) = 48
	 00000031    ----> cookie
	 00790c20    ----> debug mode add start
	 00790b80
	 0042ede8
	 0000006d

	 00000002
	 00000004
	 4个0xfd     ----> debug mode add end
	 <String object(4h)>
	 4个0xfd     ----> debug mode add end2
	 00000000(pad)
	 00000000(pad)
	 00000000(pad)
	 00000031   ----> cookie

	 normal mode:   4 + (4 * 2) = 12 ==>  + 4pad = 16
	 00000011
	 <complex object(8h)>
	 00000000(pad)
	 00000011
*/

/*
new array 需要搭配 delete[] 

VC环境下
创建array的方式为 
需要添加array length的内存占用 
如 complex* p = new complex[3];
	
	51h
	debugger header(32 bytes)
	3                         -->  array length
	double
	double
	double
	double
	double
	double
	no man land (debugger header)
	00000000(pad)
	00000000(pad)
	51h

*/