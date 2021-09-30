#pragma once


/*在栈上分配内存，编译器会自动释放内存

C++中POD(plain old data, 简单类型)，会在栈帧结束时，编译器会自动调用析构函数
对于有构造和析构函数的非POD类型，也可以在栈上进行内存分配，编译器会在生成代码合适的位置，插入对构造和析构函数的调用
编译器自动调用析构函数也会发生在函数执行发生异常的时候===>称为栈展开(stack unwinding): 
*/

/*
RAII (Resource Acquisition Is Initialization): 资源分配即初始化; 把资源的有效期跟持有资源的对象的生命周期绑定到一起。它靠构造函数来完成资源的分配，并利用析构函数来完成资源的释放。
new 的时候先分配内存（失败时整个操作失败并向外抛出异常，通常是 bad_alloc），
然后在这个结果指针上构造对象（注意上面示意中的调用构造函数并不是合法的 C++ 代码）；
构造成功则 new 操作整体完成，否则释放刚分配的内存并继续向外抛构造函数产生的异常。

delete 时则判断指针是否为空，在指针不为空时调用析构函数并释放之前分配的内存。

RAII作用：
释放内存
关闭文件（fstream的析构内）
释放同步锁
释放其他重要系统资源
*/

/*
强调栈是 C++ 里最“自然”的内存使用方式，并且，使用基于栈和析构函数的 RAII，可以有效地对包括堆内存在内的系统资源进行统一管理。

凡生命周期超出当前函数的，一般需要用堆（或者使用对象移动传递）
反之，生命周期在当前函数内的，就该用栈
*/

#include <stdio.h>
#include <mutex>

namespace Cpp_Res_Manager_1
{
	class Obj {
	public:
		Obj() { puts("Obj() ctor"); }
		//不管是否发生异常，在栈上分配的Obj的析构函数都会被执行
		~Obj() { puts("~Obj() dtor"); }  
	};

	void foo(int n)
	{
		Obj obj;
		if (n == 42)
			throw "life, the universe and everything";
	}

	void Test1()
	{
		try {
			foo(41);
			foo(42);
		}
		catch (const char* s) {
			puts(s);
		}
	}

	//考虑释放资源的代码是否会执行（尽量使用自动释放资源的方法）
	void Test2()
	{
		std::mutex mtx;
		std::lock_guard<std::mutex> guard(mtx);
		//...需要做的同步工作

	    //不推荐
		mtx.lock();
		//...需要做的同步工作
		//...如果发生异常或提前返回，同步锁不会释放
		mtx.unlock();
	}



	enum class shape_type {
		circle,
		triangle,
		rectangle,
		//…
	};

	class shape { 
	public:
		virtual ~shape() {
			puts("shape dtor");
		}
		void print() {
			puts("this is a shape");
		}
	};
	class circle : public shape { 
	public:
		~circle() {
			puts("circle dtor");
		}
	};
	//如果{}中放了/**/ 会编译不过
	class triangle : public shape { };  
	class rectangle : public shape { 
	public:
		~rectangle() {
			puts("rectangle dtor");
		}
	};


	//工厂内部创建的对象，没有自动管理释放
	//返回必须是指针，如果返回对象，可能返回派生类对象导致对象切片（object slicing），不是语法，是对象复制的语义错误
	shape* create_shape(shape_type type)
	{
		//…
		switch (type) {
		case shape_type::circle:
			return new circle();
		case shape_type::triangle:
			return new triangle();
		case shape_type::rectangle:
			return new rectangle();
			//...
		}
	}


	//将返回值存到栈中，利用RAII去自动管理释放
	class shape_wrapper {
	public:
		explicit shape_wrapper(
			shape* ptr = nullptr)
			: ptr_(ptr) {}
		~shape_wrapper()
		{
			delete ptr_;
		}
		shape* get() const { return ptr_; }
	private:
		shape* ptr_;
	};

	void Test3()
	{
		//…
		shape_wrapper ptr_wrapper(
			create_shape(shape_type::circle));   
		//…

		shape* sptr = create_shape(shape_type::rectangle);
		delete sptr;
		sptr->print();
		sptr = nullptr;
		sptr->print();

	}//离开函数栈帧时，编译器调用shape_wrapper的析构函数，释放内存
}