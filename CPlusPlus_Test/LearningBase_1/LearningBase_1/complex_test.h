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
4. 在类中的函数（成员函数） 能加const的尽量加			 如果不改变函数内部对象（数据）时 添加const		
5. 构造函数尽量使用初始列





*/




/*******************************************************************    前置声明    ***************************************************************/

#include <cmath>

class ostream;
class complex;

complex&
	__doapl(complex* ths, const complex& r);




/*****************************************************************      类声明      *************************************************************/
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
												   //任何不会改变数据成员的成员函数 建议声明const  
												   //如果不慎修改了数据成员 或者调用了非const 成员函数 编译器会报错 增加程序健壮性

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


//扩展：操作符重载  (operator overloading)   //不同定义位置：成员函数
//操作符实际是一个函数
/* 例： c2 += c1
二元操作符 两个操作数（左数、右数）
编译器会将 += 作用在左数上
如果左数对 += 做定义（重载） 编译器就能执行对应的操作符函数
*/

/*
1. 成员函数

所有成员函数会带有隐藏的参数 this pointer，  但是这个参数不能写到参数列里， 但是函数内可以用到， 同时不同编译器 这个this的位置也不一定相同
例：__doapl  ==>  do assignment plus
inline complex&
complex::operator += (const complex& r)
{
	return __doapl(this, r);
}

inline complex&
complex::operator += (   this,    const complex& r)
{
	return __doapl(this, r);
}

c2 += c1;
即this指代c2的地址 


return by reference 语法分析
传递者无需知道接收者是以reference形式接收的
inline complex&                                       //引用
__doapl(complex* ths, const complex& r)				  //complex* 声明指针
{
	ths->re += r.re;
	ths->im += r.im;
	return *ths;									  //指针所指向的对象 value / object		因为ths是指针 *ths即为指针指向的东西
}
### 以上其实可以传递 《指针》 但是会增加接收者的操作难度 需要处理指针


由于存在连串调用
c3 += c2 += c1  先调用c2 += c1，返回值t21, 然后调用c3 += t21
所以上面的+=重载操作符函数不能返回value 不然无法实现连串调用

*/



/***************************************************************     类定义     **************************************************************/

inline complex&
__doapl(complex* ths, const complex& r)
{
	ths->re += r.re;
	ths->im += r.im;
	return *ths;
}

inline complex&
complex::operator += (const complex& r)
{
	return __doapl(this, r);
}

/*
class body 外的各种定义 （definitions）

2. 全局（全域）函数
不带class名称的函数
区分：

成员函数
inline complex&
complex::operator += (   this,    const complex& r)
{
	return __doapl(this, r);
}

全局函数
inline double
imag(const complex& x)
{
	return x.imag();
}
*/

//扩展：操作符重载  (operator overloading)   //不同定义位置：非成员函数 
/*
全局函数 不用携带隐藏this
为了满足需求，可能需要重载多个

例：
复数 + 复数
inline complex
operator + (const complex& x, const complex& y)
{
	return complex (real(x) + real(y), imag(x) + imag(y));
}

实数 + 复数
inline complex
operator + (const complex& x, double y)
{
	return complex (real(x) + y, imag(x));
}
inline complex
operator + (double x, const complex& y)
{
	return complex ( x + real (y), imag(y));
}

等等。。。

上述函数返回值 必定是 local object  不可能是 return by reference  而是 return by value
不同于原先的例子 （将一个数加到另一个已经存在的数）
现在是两个数加起来 获得一个新的数值（需要创建出来）
如果还是返回引用，在函数结束时，这个创建对象的生命周期就结束了，如果传出去引用，那么引用对象可能是空值

《一个函数能否返回引用：看这个函数是否需要在内部创建临时数据并需要给到外部》


扩展：临时对象，temp object   没有名称
 新语法：typename();
 例：
 inline complex
operator + (double x, const complex& y)
{
	return complex ( x + real (y), imag(y));
}

创建临时对象并且立刻返回
complex ( x + real (y), imag(y));

区别创建对象：
complex c1(2, 1);
complex c2;

创建临时对象：
complex();
complex(4, 5);
complex(2);
注意：执行到下一行 这两个临时对象的生命周期已经结束



扩展：操作符重载
例：复数取正 取反（反向）
inline complex
operator + (const complex& x)
{
	return x;
}

// 修改为返回引用 应该也可以 因为返回的就是原来的值
inline complex&
operator + (const complex& x)
{
	return x;
}

inline complex
operator - (const complex& x)
{
	return complex(-real(x), -imag(x));
}

// complex c1(2, 1);
// complex c2;
// cout << -c1;
// cout << +c1;

例：
//共轭复数：实部相等，虚部正负相反
inline complex
conj (const complex&)
{
	return complex(real(x), -imag(x));
}


扩展：特殊操作符重载 - 全局函数
重载内置操作符
例：
#include <iostream.h>
ostream& 
operator << (ostream& os, const complex& x)
{
	return os << '(' << real(x) << ',' 
			  << imag(x) << ')';
}

C++没有作用在右值上的操作符

例：
complex c1(2, 1);
cout << conj(c1);
// << 作用在 坐标cout上   cout是标准库的对象 （查询标准库 ：类型是 ostream）  不认识 新定义的 conj(c1) 所以不能把 << 的重载函数写成成员函数
//注意：不能把内置存在的操作符 的重载函数 写成成员函数 只能写成全局函数 ！
// 重载<<的函数中的参数 其中 ostream& 指代 cout  const complex& 指代 conj(c1)

// 不能写   ostream& 
			operator << const (ostream& os, const complex& x) 
			{...} ！
// 不能写 const 的原因是 函数内部 往 cout / os 中塞(<<) 需要打印的数据时 已经改了os了

// 返回类型其实可以是 void 但是考虑到使用者会连串输出
// 不能写   void
			operator << (ostream& os, const complex& x)
			{...} ！
例：
cout << c1 << conj(c1);
// 先输出c1 得到的结果再接收 conj(c1)  那返回的需要是 cout  考虑是否是return by value 还是 by reference 因为 os在函数内部不是local对象  即写成 ostream&
// 函数返回前不能加 const  连串中 输出c1 后 还需要改变cout 输出 conj(c1)
// 不能写   const ostream& 
			operator << (ostream& os, const complex& x) 
			{...} ！
//如果函数返回值采用 by value 那加const 没有价值
例:
double real() const { return re;}
写成 const double real() const {return re;}




//如果要实现违背经验的写法 （当然是不推荐的拉）
例：
complex c1(1, 2);
c1 << cout;

则其实也可以在complex中重载操作符 <<   
只能作用在左值上 所以可以作为成员函数 隐藏this
内部实现 可能是
void
complex::operator << (ostream& os)
{
	os << real() << imag();
}


*/


#endif