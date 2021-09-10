#include "cpp_memory_primitives_1.h"

#include <malloc.h>
#include <iosfwd>
#include <complex>

#include <string>
using std::string;

void TestCppMemoryPrimitives::Test1()
{
	void* p1 = malloc(512); //512bytes
	free(p1);

	std::complex<int>* p2 = new std::complex<int>;   //one object
	delete p2;

	void* p3 = ::operator new(512);  //512bytes
	::operator delete(p3);

	//c++ stl allocators
#ifdef _MSC_VER
	//分配3个int
	int* p4 = std::allocator<int>().allocate(3, (int*)0);  //(int*)0 无用
	std::allocator<int>().deallocate(p4, 3);
#endif
#ifdef __BORLANDC__
	int* p4 = std::allocator<int>().allocate(5);
	std::allocator<int>().deallocate(p4, 5);
#endif
#ifdef __GNUC_OLD__ //旧版本
	void* p4 = std::alloc::allocate(512);
	std::alloc::deallocate(p4, 512);
#endif

#ifdef __GNUC__
	void* p4 = std::allocator<int>().allocate(7);
	std::allocator<int>().deallocate((int*)p4, 7);

	void* p5 = __gnu_cxx::__pool_alloc<int>().allocate(9);
	__gnu_cxx::__pool_alloc<int>().deallocate((int*)p5, 9);
#endif
}

//new expression
void TestCppMemoryPrimitives::Test2()
{
	/*
	Complex* pc = new Complex(1, 2);				//new会调用ctor
	
	//编译器转换为===>

	Complex *pc;
	try {
		void* mem = operator new(sizeof(Complex));  //operator new 源码中通过 void* p = malloc(size_t) 实现
		pc = static_cast<Complex*>(mem);
		pc->Complex::Complex(1, 2);		//只有编译器能这样直接调用ctor，
										//并且需要通过指针调用
	}catch (std::bad_alloc) {
		//如果失败就不执行ctor
	}
	*/

	//如果需要直接调用ctor，直接使用placement new (定点new)
	//new(p)Complex(1, 2);
}

//delete expression
void TestCppMemoryPrimitives::Test3()
{
	/*
	Complex* pc = new Complex(1, 2);
	delete pc;

	//编译器转换为===>

	pc->~Complex();					//先调用析构，可以通过指针 调用dtor
	operator delete(pc);			//释放内存， 源码中通过 free(void*) 实现
	*/
}

//call ctor directly
void TestCppMemoryPrimitives::Test4()
{
	cout << "\ntest call ctor directly ...\n";

	string* pstr = new string;
	cout << "str=" << *pstr << endl;
	delete pstr;

	pstr = new string("");
	cout << "str=" << *pstr << endl;
	delete pstr;
 
	pstr->basic_string::basic_string("test1"); //vs2017上 支持直接调用ctor

	cout << "str=" << *pstr << endl;

	pstr->~string();

	//-------------------------

	A* pA = new A;
	cout << pA->id << endl;
	delete pA;

	pA = new A(1);
	cout << pA->id << endl;
	delete pA;

	//placement new
	new(pA)A(2);							//placement new 定点new 
											//即在给定指针位置调用构造函数进行赋值操作
											//没有分配memory
	cout << pA->id << endl;
	//!delete pA;                           //placement new不能这么释放，
											//因为placement new没有分配内存
	pA->~A();	

	A::A(5);								//vs2017上 支持直接调用ctor  
											//临时对象会直接在语句结束时调用dtor
	pA->A::A(3);
	cout << pA->id << endl;
	pA->~A();								//vs2017上 支持直接调用dtor
	//! delete pA;							//不能连续两次调用dtor

	//simulate new
	void* p = ::operator new(sizeof(A));
	cout << "p=" << p << endl;
	pA = static_cast<A*>(p);
	pA->A::A(2);							//vs2017上 支持直接调用ctor
	cout << pA->id << endl;

	//simulate delete
	pA->~A();
	::operator delete(pA);
}

//array new / array delete
/*
1. 尽量对应写，对应调用array delete的目的是，调用每个object的dtor，对于class without ptr member 可能没有影响
2. array new中调用每个object的ctor的内存顺序是由上往下，array delete中调用dtor的内存顺序是由下往上
*/
void TestCppMemoryPrimitives::Test5()
{
	cout << "\ntest placement new and array new... \n";

	size_t size = 3;
	//case 1
	{
		A* buf = new A[size];	//A必须有default ctor
		A* tmp = buf;			//声明一个指针指向A[0]
		cout << "buf=" << buf << " tmp=" << tmp << endl;

		for (int i = 0; i < size; ++i)
		{
			new(tmp++) A(i);			//在指定位置调用ctor赋值
		}
		cout << "buf=" << buf << " tmp=" << tmp << endl;

		delete[] buf;					//dtor 次序逆反，[2] [1] [0]

		cout << "\n\n";
	}

	//case 2
	{
		A* buf = (A*)(new char[sizeof(A) * size]);
		A* tmp = buf;
		cout << "buf=" << buf << " tmp=" << tmp << endl;

		for (int i = 0; i < size; ++i)
		{
			new(tmp++) A(i);			//在指定位置调用ctor赋值
		}
		cout << "buf=" << buf << " tmp=" << tmp << endl;

		//! delete[] buf;		//crash
								//buf 其实是一个 char array, delete[] buf 会让编译器企图唤醒多次A::~A()
								//但是在array memory layout中找不到 与array元素个数相关的信息
								//整个布局错乱导致崩溃

		delete buf;				//只需要调用一次 ~[0]
	}
}

//placement new 定点new
/*
允许将object 构造在 allocated memory（已经分配的内存，即需要一个指针）中

没有placement delete 因为placement new没有分配内存
或者 与placement new 对应的operator delete称为 placement delete

placement new等同于调用ctor

placement new的形式：
	1. new(p)
	2. ::operator new(size, void*)
*/
void TestCppMemoryPrimitives::Test6()
{
	/*
	char* buf = new char[sizeof(Complex) * 3];
	Complex* pc = new(buf)Complex(1, 2);

	delete[] buf;

	//编译器转换为===>

	Complex *pc;
	try {
		void* mem = operator new(sizeof(Complex), buf);				//allocate
											//与普通的alloactor，多了指定内存位置这个指针参数
		pc = static_cast<Complex*>(mem);							//cast
		pc->Complex::Complex(1, 2);									//construct
	}catch(std::bad_alloc) {
		
	}
	*/
}
