#pragma once

class Account {
public:
	static double m_rate;                             //声明 （变量没有获得内存，只属于class范畴）   
													  //静态对象的初始化规范 ， 需要在class外部（gcc / clang并且不能在头文件中）进行定义初始值 （不一定需要设初始值！）
	static void set_rate(const double& x) { m_rate = x; }
};

/*
<<static  静态>>

complex的组成结构
---
data members
static data members
---
member functions
static member functions
---

明确 this pointer概念

例：
complex c1,c2,c3;
cout << c1.real();
cout << c2.real();

===>  to C

complex c1,c2,c3;
cout << complex::real(&c1);
cout << complex::real(&c2);

//非静态函数：相同的函数，传入不同的对象的地址 （this）
//静态对象 静态函数 只存在一份  没有this pointer  所以不能处理非静态对象（数据）
*/

/*
静态函数调用

1. 通过object调用
	Account a;
	a.set_rate(xxx);                //注意 this不会放入 所以使用  .  代替 ->

2. 通过class name 调用
	Account::set_rate(yyy);

*/

//ctor放到private中

/*
单例
例子：
*/
class SingletonA {

public:
	static SingletonA& getInstance() { return a; }
	void setup();
private:
	SingletonA();
	SingletonA(const SingletonA& rhs);
	static SingletonA a;
};

//SingletonA::getInstance().setup();

//优化  懒汉
class SingletonAA {
public :
	static SingletonAA& getInstance();
	void setup();

private:
	SingletonAA();
	SingletonAA(const SingletonAA& rhs);
};

SingletonAA& SingletonAA::getInstance()
{
	static SingletonAA a; //自c以来的规范： 只有调用到这个函数   static a才创建
	return a;
}


//扩展：模板类
/*
//模板声明 T
template<typename T>                           ------------------->  typename
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
	complex<double> c1(2.5, 1.5);
	complex<int> c2(2, 6);
}
*/


//扩展：模板方法、模板函数
/*

class stone
{
public:
	stone(int w, int h, int we)
		:_w(w), _h(h), _weight(we)
	{}
	bool operator < (const stone& rhs) const
	{
		return _weight < rhs._weight;
	}

	template <class T>
	inline const T& min(const T& a, const T& b)
	{
		return b < a ? b : a;
	}

private:
	int _w, _h, _weight;
};


模板方法 使用情况：
stone r1(2, 3), r2(3, 3), r3;
r3 = min(r1, r2);

和模板类不同，不需要进行声明<类型>
编译器的实参推导

编译器推导过程：
根据 operator < 作用在  实参b上， 编译器查询 b对应的类型stone 然后验证stone是否有对象 operator< 方法（或者是重载方法）可以调用

编译器推导结果：
T为Stone  于是调用stone::operator<

补充：因为operator< 重载实现在 Stone类中  所以实现了具体调用方法min 和 具体实现方法operator< 的分离
*/



//namespace
/*
使用：

using namespace std;

cout>>...;
cin<<...;


using std::cout;

std::cin>>...;
cout <<...;

*/