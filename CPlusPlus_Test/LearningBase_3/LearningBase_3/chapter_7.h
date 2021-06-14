#pragma once
#ifndef _CHAPTER_7_H_
#define _CHAPTER_7_H_

#include <iostream>
#include <string>


/*
const vs. volatile  vs. mutable

const
它是一个类型修饰符，可以给任何对象附加上“只读”属性，保证安全；
它可以修饰引用和指针，“const &”可以引用任何类型，是函数入口参数的最佳类型；
它还可以修饰成员函数，表示函数是“只读”的，const 对象只能调用 const 成员函数。


volatile
它表示变量可能会被“不被察觉”地修改，禁止编译器优化，影响性能，应当少用。


mutable
它用来修饰成员变量，允许 const 成员函数修改，mutable 变量的变化不影响对象的常量性，但要小心不要误用损坏对象。

*/
class Chapter_7
{
	/*
	const  (理解为  read only) 修饰的是  运行时 只读变量  
	但是在运行时，可以被改变，也可以强制写入

	编译器识别到const, 会进行优化，将const出现的地方都替换成原始值
	*/
	const int MAX_LEN = 1024;
	const std::string NAME = "helloworld";

	//如果不添加 volatile  即使用指针修改了常量的值，但这个值在运行阶段不会用到，因为在编译阶段被编译器优化掉了
	const volatile int VOLATILE_MAX_LEN = 1024;

	int m_value;

private:

	/*
	volatile

	不稳定的、易变的

	c++里，表示变量的值可能会被特殊不易察觉的方式修改（如操作系统信号，外界其他的代码）
	所以要禁止编译器做任何形式的优化，使用时需要正常取值

	变量被volatile修饰后，编译器在生成二进制机器码时，不会再做可能有副作用的优化，即不会把变量替换为具体值，而是去内存里取值
	（当通过指针修改这个值时，获取到的就是修改后的值了）


	*/
	void _testVolatile()
	{
		auto volatile_ptr = (int*)(&VOLATILE_MAX_LEN);
		*volatile_ptr = 2048;
		std::cout << VOLATILE_MAX_LEN << std::endl;
	}


	void _testConst()
	{
		int x = 100;
		const int& rx = x;   //常量引用
		const int* px = &x;  //常量指针

		/*const&  万能引用  推荐使用
		
		可以引用任何类型，值、指针、左引用、右引用等

		编译器会检查对添加了const的变量的写操作，发出警告，在编译阶段防止有意或者无意的修改
		*/
		
		std::string name = "uncharted";
		const std::string* ps1 = &name;			// 表示 指向常量 的指针，即指针指向的是一个只读变量
		//*ps1 = "spiderman";  指针指向的变量不允许修改


		//以下两种不推荐使用
		std::string* const ps2 = &name;			// 表示 指向变量 的指针，指针本身不能修改，指向的变量可以被修改
		*ps2 = "spiderman";
		std::string name2 = "name2";
		//ps2 = &name2;		   指针本身不允许修改

		const std::string* const ps3 = &name;	// 
	}

public:

	//C++ 中函数不是变量， 除了lambda表达式
	//const表示函数执行过程是const的，不会修改对象状态（即成员变量）
	//对成员变量只是一个只读操作

	//同时，当有常量引用或者常量指针 关联的对象，那么对它的相关操作也必须是const
	//编译器会检查const对象相关代码，如果成员函数不是const的，就不允许被调用了

	//成员函数有一个隐含的this指针，从语义上来说 const实际传入的是const this指针， 但受c++语法限制，后置了const
	int get_value() const			//const 修饰的成员函数
	{
		return m_value;
	}


private:
	using mutex_type = decltype(m_value);
	//mutable只能修饰类中的成员变量 表示变量即使在const对象里，也是可以被修改
	//标记为 mutable 的成员不会改变对象的状态，也就是不影响对象的常量性，所以允许 const 成员函数改写 mutable 成员变量

	//例如：对象内部使用了一个mutex来保证线程安全，或者有一个缓冲区来暂存数据，再或者有一个原子变量做引用计数等
	//对于这些有特殊作用的成员变量，你可以给它加上 mutable 修饰，解除 const 的限制，让任何成员函数都可以操作它
	mutable mutex_type m_mutex;   //mutable变量

public:
	void saveData() const
	{
		//添加mutable 
		m_mutex = 1;
	}

};

#endif  //_CHAPTER_7_H_