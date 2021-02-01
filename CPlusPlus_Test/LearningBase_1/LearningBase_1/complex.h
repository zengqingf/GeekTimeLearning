//头文件防卫式声明

//方式1. 由编译器提供保证  同一文件不会被包含多次
#pragma once  

//方式2. 防御式  防卫式  声明  guard
#ifndef __COMPLEX__
#define __COMPLEX__

/*
类声明的检查

1. 数据都放在private
2. 参数尽量通过reference传递  看情况而定是否需要加const （首先考虑传递引用）
3. 返回值尽量通过reference传递							（首先考虑传递引用）
4. 在类中的函数 能加const的尽量加
5. 构造函数尽量使用初始列





*/




//前置声明

#include <cmath>

class ostream;
class complex;

complex&
	__doapl(complex* ths, const complex& r);




//类声明
class complex										//class head
{													//class body start

//access level访问级别  可以交错写 不一定需要写如下两端
public:
	//ctor :  参数可以携带默认值 默认实参
	complex(double r = 0, double i = 0)
		: re(r), im(i)							   //初始列，初值列(initialization list)  效率高于 在构造函数内部进行赋值操作 （赋初始值 assignments）
												   //变量赋值有两个阶段，初始化， 赋值
	{}

	//complex() : re(0), im(0) {}				   //这个构造函数和上面带默认值的 无法同时存在  不符合函数重载

	complex& operator += (const complex&);		   //参数-引用传递 (并且是 to const 不会被接收方修改影响自身) 
												   //返回值传递，引用传递

	//下面两个函数很可能会被编译器定义成inline
	double real() const { return re; }			   //const  位置在() 和 {} 之间
	double imag() const { return im; }			   //作用  如果函数不改变数据内容 添加const 如取得类中参数 但是是需要取得 不需要改变

	//重载
	void real(double r) { re = r; }


	//test friend func
	//int func(const complex& param) { return param.re + param.im; } // 同一个class的各个object 彼此互为友元 	c2.func(c1);

//数据尽量放在private
private:
	double re, im;

protected:
	double test;

	friend complex& __doapl(complex*, const complex&); //友元 可以加到函数上   这个函数可以自由取得 friend 的 private的成员
														//也可以通过函数调用拿数据， 但没有直接拿快， 但是friend打破了封装的特性

};

//扩展：模板类
/*
//模板声明 T
template<typename T>
class complex
{
public:
	complex(T r = 0, T i = 0)
		: re(r), im(i)
	{}
	complex& operator += (const complex&);
	T real() const { return re; }
	T imag() const { return im; }

private:
	T re, im;

	friend complex& __doapl(complex*, const complex&);
};

//模板使用
{
	complex<T> c1(2.5, 1.5);
	complex<T> c2(2, 6);
}
*/


//扩展：内联函数（inline） 执行效率高 
//含义：函数在class body内定义完成，便自动成为inline候选，编译器会进行选择
//函数太复杂 可能就无法被编译器处理为inline
//例：在class body 外定义的，但是内部实现简单，主动声明成inline 进入候选
/*
inline double imag(const complex& x)
{
	return x.imag();
}*/


//扩展：构造函数
//下面都是创建对象的方式
/*
complex c1(2, 1);
complex c2;
complex* p = new complex(4);
*/

//如果把构造函数放在private中 常见的是单例模式 Singleton
/*
class A {
public:
	static A& getInstance();
	setup() {...}

private:
	A();
	A(const A& rhs);
	...
};

A& A::getInstance()
{
	static A a;
	return a;
}

调用
A::getInstance().setup();

*/

//扩展：函数重载(overloading) ，包括构造函数
//编译后的实际名称不一样
//函数重载常常用于构造函数
//如果函数参数带有默认值，要注意和没有参数的函数能不能共存重载


//扩展：const 作用
/*
	const complex c1(2, 1); //创建一个const 对象
	cout << c1.real();      //real() 需要const 声明
	cout << c1.imag();	    //imag() 需要const 声明
*/


//扩展： 参数传递
//pass by value vs. pass by reference(to const)
//引用就是通过指针实现的
//传递引用就是传递指针 效率快
//c++ 建议 尽量全部参数都传递引用
//但是 如果传递引用 会引起别人的不确定修改 而影响传递方  可以选择 传递 reference (to const)
//细节上 如果参数只有一两个字节，而指针有四个字节，那可以传递值的方式

//扩展： 返回值传递
//return by value vs. return by reference(to const)
//返回值的传递也尽量传递引用


//什么情况下可以传值 什么情况下可以传引用
/*
如果函数生命周期中，需要创建临时对象存放数据，函数结束时，临时对象不应该被传递到外部被引用，这种情况下不能传递引用
函数的参数 如第一参数会被改动 第二参数不会被改动 需要将第二参数附加到第一参数上时 可以传递引用


//函数的数据操作有两种情况
//情况1：函数操作后的数据存放到某个参数中
inline complex&
__doapl(complex* ths, const complex& r)
{
	ths->re += r.re;
	ths->im += r.im;
	return *ths;
}
//情况2：函数创建新空间存放操作后的数据
//不可行 c3的生命周期在函数结束时已经结束 是local概念 不能把引用传出去
inline complex&
__doapl(complex* ths, const complex& r)
{
	complex c3;
	c3.real(r.re);
	return c3;
}
*/


//扩展: 友元
//同一个class的各个object 彼此互为友元
//例：int func(const complex& param) { return param.re + param.im; }  直接拿成员
//    c2.func(c1);


//扩展：操作符重载
//操作符实际是一个函数
//两种方式
/*
1. 成员函数




*/






//类定义


#endif