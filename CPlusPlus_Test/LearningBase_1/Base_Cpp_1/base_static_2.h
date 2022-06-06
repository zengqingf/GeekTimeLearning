#pragma once

#include <iostream>
using std::clog;
using std::endl;

struct Bar {
	Bar() {
		clog << "constructing Num object" << endl;
	}

	int i = 0, j = 0;

	~Bar() {
		clog << "destructing Num object" << endl;
	}
};

void foo() {
	clog << "foo() started" << endl;
	static Bar b;
	return;
}

void test_static_2() {
	if (true) {
		clog << "if-statement started" << endl;
		foo();
	}

	clog << "if-statement exited" << endl;
}


/*
ref: https://blog.csdn.net/xbwueric3/article/details/81172917
在windows下，由于栈限制为1M，代码可能会栈溢出，因此通过单步调试我们可以知道，局部变量是在进入函数时分配栈内存。

编译器在编译的过程中，遇到函数调用时，会加入几条汇编指令。这些汇编指令的作用是：
1 分配一段栈空间，用于存放被调函数的参数和局部变量。
2 call被用函数。
3 当被调函数返回时释放掉这段分配栈空间。
*/
void test_local_1() {
	int buf[1024 * 1024];
}


/*
ref:https://blog.csdn.net/xbwueric3/article/details/81172917
全局变量、文件域的静态变量和类的静态成员变量在main执行之前的静态初始化过程中分配内存并初始化；
局部静态变量（一般为函数内的静态变量）在第一次使用时分配内存并初始化。这里的变量包含内置数据类型和自定义类型的对象。

修改为：
局部静态变量的内存分配应该和全局变量一样，在main函数之前分配好，在第一次调用的时候只是初始化。
如果注释掉 B A::b(即不对b进行显式初始化)，结果就不会调用B的构造函数。因此对于类的静态成员来说，static B b只是声明，B A::b才是定义，而类中如果是B b则是声明且定义。
*/
class B
{
public:
	B() { cout << "class B construct" << endl; }
	~B() { cout << "class B destruct" << endl; }
};

class A
{
public:
	A() { cout << "class A construct" << endl; }
	~A() { cout << "class B destruct" << endl; }

	static B b;
};

B A::b;

void goo()
{
	static A a;
}

void test_static_3()
{
	goo();
	goo();
}