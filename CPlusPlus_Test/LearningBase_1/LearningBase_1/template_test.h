#pragma once

/*
区分定义和声明

<变量>
声明：指出存储类型，并给存储单元指定名称  （引用性声明）

定义：分配内存空间，还可为变量指定初始值  （定义性声明）

声明不是定义，定义也是声明

extern关键字：声明变量名，而不是定义，不会分配内存空间；
			  告诉编译器变量在其他地方定义
			  变量可以在多个文件中声明

非静态变量，并且为一般内置类型，如int val;   声明了变量val，同时也是定义，也会分配内存空间
加上extern才是声明

extern声明的外部变量，指定初始值后，也是定义，注意定义必须位于函数外部，如 extern int val = 1;
extern声明的局部变量，不能设置初始值
如
int main()
{
extern int a = 20; //错误！
}




<函数>
函数原型（函数声明）：只有函数头
函数定义：带函数体

函数原型的返回值类型必须和函数定义的返回值类型相同
函数原型的形参表的类型于顺序必须和函数定义中的相同
函数原型可以不写形参名称，形参名称可以和原函数不一样（形参类型必须写）
函数原型中可以不写形参（空形参）
	C语言中：
		int func();		//表示可以有多个参数
		int func(void); //表示没有参数
	C++中：
		上述两个写法都表示没有参数

函数原型必须在调用函数前，函数定义可以写在调用函数之后
extern标识函数声明的作用：可以在外部调用函数
*/


/*
static 声明变量
1.对局部变量用static声明，使该变量在本函数调用结束后不释放，整个程序执行期间始终存在，使其存储期为程序的全过程。
2.全局变量用static声明，则该变量的作用域只限于本文件模块(即被声明的文件中)。


static 内部机制
将变量存储在程序的静态存储区，而非栈空间
静态数据成员按定义出现的先后顺序依次初始化
静态成员嵌套时，要保证所嵌套的成员已经初始化

static优势：
	有效节省内存，它是其类的全部对象所共有的；
	提高效率，只需要更新一次，保证其他对象获取到相同值；（有利有弊）
	不会破坏封装

类的对象构建过程不会动到static变量和函数，static类成员存在于内存的静态区，程序载入进内存的时候它就存在，和类对象生命周期不同
static数据成员需要在程序一开始就初始化（定义），不能在类的非static成员函数中进行空间分配
			不能在类的声明中定义（只能声明数据成员），也崩在头文件的类声明的外部定义，会导致多个使用该类的源文件里，对其反复定义
			必须在类定义体的外部定义（即初始化）（只定义一次）



*/




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

//静态成员函数没有this指针，不能被声明为const，不能被声明为virtual
*/

/*
静态函数调用

1. 通过object调用
	Account a;
	a.set_rate(xxx);                //注意 this不会放入 所以使用  .  代替 ->

2. 通过class name 调用
	Account::set_rate(yyy);

*/

#include <iostream>

/*
<单例>
ctor放到private中
*/

class TestPrivateDtor
{
private:
	~TestPrivateDtor() {
		std::cout << "TestPrivateDtor dtor!" << std::endl;
	}

public:
	friend void DestructTest(TestPrivateDtor*);
};

//不能在同一个文件里定义friend method
//需要加inline修饰
inline void DestructTest(TestPrivateDtor* ptr)
{
	delete ptr;
	ptr = nullptr;
}

//饿汉1
class SingletonA {

public:
	static SingletonA & getInstance() { return a; }
	void setup();

	void print() { std::cout << "Singleton A print!" << std::endl; }
	//~SingletonA() { std::cout << "Singleton A dtor!" << std::endl; }

private:

	//also able
	~SingletonA() { std::cout << "Singleton A dtor!" << std::endl; }

	SingletonA() {}
	SingletonA(const SingletonA &rhs) {}
	SingletonA & operator=(const SingletonA &other) {}
	static SingletonA a;							//声明 非定义
};

//SingletonA::getInstance().setup();

//优化  懒汉1
class SingletonAA {
public :
	static SingletonAA & getInstance();
	void setup();

	void print() { std::cout << "Singleton AA print!" << std::endl; }
	//~SingletonAA() { std::cout << "Singleton AA dtor!" << std::endl; }


private:
	SingletonAA() {}

	//also able
	~SingletonAA() { std::cout << "Singleton AA dtor!" << std::endl; }

	SingletonAA(const SingletonAA &rhs) {}
	SingletonAA & operator=(const SingletonAA &other) {}
};


//懒汉 --- 同时返回出去 引用 而非 指针
class SingletonA3 {
public:
	static SingletonA3& getInstance()
	{
		if (nullptr == instance) {
			instance = new SingletonA3();
		}
		return *instance;
	}

private:
	SingletonA3() {}
	static SingletonA3* instance;
};


//C++ 11
//线程安全
//懒汉 --- 局部静态变量 初始化仅进行一次  多线程环境下也是这样
class SingletonA4 {
public:
	static SingletonA4& getInstance()
	{
		static SingletonA4 *instance = new SingletonA4();
		return *instance;
	}

private:
	//私有构造，保证单例唯一
	SingletonA4() {}
};

//懒汉 2
//@注意： instance不会被清理！
class SingletonB {
public:
	static SingletonB * getInstance();
	void print() { std::cout << "Singleton B print!" << std::endl; }

	//main函数结束：无法执行SingletonB dtor
	//~SingletonB() { std::cout << "Singleton B dtor!" << std::endl; }

private:

	//main函数结束：无法执行SingletonB dtor
	~SingletonB() { std::cout << "Singleton B dtor!" << std::endl; }

	SingletonB() {}
	SingletonB(const SingletonB &rhs) {}
	SingletonB & operator=(const SingletonB &other) {}

	static SingletonB *instance; //声明（引用性声明）
};


/*
单例优化清理： 优化SingletonB
让这个类自己知道在合适的时候把自己删除，或者说把删除自己的操作挂在操作系统中的某个合适的点上，使其在恰当的时候被自己主动运行。

语言特性：
程序在结束的时候，系统会自己主动析构全部的全局变量。其实，系统也会析构全部的类的静态成员变量，就像这些静态成员也是全局变量一样
*/
class SingletonC 
{
public:
	static SingletonC * getInstance();
	void print() { std::cout << "Singleton C print!" << std::endl; }
	//~SingletonC() { std::cout << "Singleton C dtor!" << std::endl; }

private:
	SingletonC() { std::cout << "Singleton C ctor!" << std::endl;  }
	SingletonC(const SingletonC &rhs) {}

	//also able
	~SingletonC() { std::cout << "Singleton C dtor!" << std::endl; }

	SingletonC & operator=(const SingletonC &other) {}

	static SingletonC *instance;

	//程序执行结束时，系统会调用SingletonC的静态成员Garbo的析构函数，该析构函数会删除单例的唯一实例。
	class CGarbo //嵌套类作用：析构时删除SingletonC实例
	{
	public:
		~CGarbo()
		{
			if (SingletonC::instance != nullptr)
				delete SingletonC::instance;
		}
	};
	static CGarbo garbo_;  //声明（引用性声明）一个静态成员变量，当SingletonC的生命周期结束，garbo_对象也会销毁，会调用其析构函数
};

class SingletonCC1;
class SingletonC1
{
public:
	static SingletonC1 * getInstance();
	void print() { std::cout << "Singleton C1 print!" << std::endl; }
	//~SingletonC() { std::cout << "Singleton C dtor!" << std::endl; }

protected:
	SingletonC1() { std::cout << "Singleton C1 ctor!" << std::endl; }
	SingletonC1(const SingletonC1 &rhs) {}
	//also able
	~SingletonC1() { std::cout << "Singleton C1 dtor!" << std::endl; }
	SingletonC1 & operator=(const SingletonC1 &other) {}


private:
	static SingletonC1 *instance;

	//程序执行结束时，系统会调用SingletonC的静态成员Garbo的析构函数，该析构函数会删除单例的唯一实例。
	class CGarbo //嵌套类作用：析构时删除SingletonC实例
	{
	public:
		~CGarbo()
		{
			if (SingletonC1::instance != nullptr)
				delete SingletonC1::instance;
		}
	};
	static CGarbo garbo_;  //声明（引用性声明）一个静态成员变量，当SingletonC的生命周期结束，garbo_对象也会销毁，会调用其析构函数
};

class SingletonCC1 : public SingletonC1
{
public:
	static SingletonCC1 * getInstance();

private:
	SingletonCC1() { std::cout << "Singleton CC1 ctor!" << std::endl; }
	SingletonCC1(const SingletonCC1 &rhs) {}
	//also able
	~SingletonCC1() { std::cout << "Singleton CC1 dtor!" << std::endl; }

	static SingletonCC1 *instance_;

	//程序执行结束时，系统会调用SingletonC的静态成员Garbo的析构函数，该析构函数会删除单例的唯一实例。
	class CGarbo //嵌套类作用：析构时删除SingletonC实例
	{
	public:
		~CGarbo()
		{
			if (SingletonCC1::instance_ != nullptr)
				delete SingletonCC1::instance_;
		}
	};
	static CGarbo garbo__;  //声明（引用性声明）一个静态成员变量，当SingletonC的生命周期结束，garbo_对象也会销毁，会调用其析构函数
};


//单例注册表 (registry of singleton)
#include<vector>
class SingletonD;
class NameSingletonPair
{
public:
	char *name;
	SingletonD *singletonD;
};

class SingletonD {
public:
	static void Register(const char* name, SingletonD *);
	static SingletonD * getInstance();
protected:
	static SingletonD * Lookup(const char* name);
private:
	static SingletonD *instance_;
	static std::vector<NameSingletonPair*> * _registry;
};

/*
应用：

MySingleton::MySingleton() {
	Sinleton::Register("MySingleton",this);
}
*/










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