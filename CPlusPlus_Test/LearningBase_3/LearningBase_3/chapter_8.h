#pragma once
#ifndef _CHAPTER_8_H_
#define _CHAPTER_8_H_

#include <iostream>
#include <string>
#include <memory>
#include <cassert>

using std::string;
using std::unique_ptr;
using std::shared_ptr;

/*
C++ 中的垃圾回收 不同于Java C# Go等的严格意义上的垃圾回收（标准的垃圾回收）
而是指广义上的垃圾回收（构造、析构 以及 RAII惯用法(Resource Acquisition Is Initialization 资源获取即初始化)）

指针： 本质上是一个内存地址索引，代表了一块内存区域，能够直接读写内存
	完全映射了计算机硬件，操作效率高
	可能引起的问题：
		访问无效数据、指针越界、内存分配后没有及时释放（会引起运行错误、内存泄露、资源丢失等问题）
	改善方法：
		使用C++提供的垃圾回收（构造、析构 以及 RAII惯用法）

		原始指针：
		应用代理模式，包装裸指针，在引用对象的构造函数里初始化，在其析构函数里释放；
		当对象失效销毁时，C++就会自动调用析构，完成内存释放，资源回收

		智能指针：
		方便对每一个资源包装了上述垃圾回收代码结构
		完全实践了RAII，包装了裸指针，而且重载了 * 和 -> 操作符，使用方式同原始指针


推荐:
	智能指针的适用场景：自动资源管理
	搭建基本的数据结构不建议使用智能指针
	
	尽量使用智能指针代替裸指针（new 和 delete）来操作内存
	同时建议使用工厂方法创建智能指针

	涉及到使用系统API的时 避免不了需要使用裸指针，可以使用 RAII 来管理 或者 定制shared_ptr的删除函数

	在追求性能的环境下，推荐使用unique_ptr 代替 裸指针，速度几乎和裸指针相同，并且没有引用计数成本
*/


/*
注意 如果在头文件中使用这样的方式进行访问 Node的对象的成员变量或者方法  
会出现“使用了未定义类型” 的错误

e.g.
class B;
这样的声明方式只能用于 指针参数和指针变量  e.g.  funcA(B* inB),  B* m_pB;
不能用于对象的定义  e.g. B m_b   或者  B m_b  m_b.funcB()

因为class B仅仅是声明

如果把访问放到cpp实现文件中的话，可以避免这个问题
写在头文件一般也只是测试用，会导致过多的代码，也无法实现成 inline函数
*/
//class Node;

#define USE_WEAK_PTR 0

//测试shared_ptr的循环引用情况
class Node final
{
public:
	Node() = default;
	~Node() {
		std::cout << "node dtor" << std::endl;
	}

public:
	using this_type = Node;
#if USE_WEAK_PTR
	//使用weak_ptr 打破循环引用，只观察指针，不会增加引用计数（弱引用）
	//但在需要时 可以调用成员函数 lock() 获取shared_ptr（强引用）
	using shared_ptr = std::weak_ptr<this_type>;

#else
	using shared_ptr = std::shared_ptr<this_type>;
#endif

public:
	shared_ptr next;        //指向下一个节点
};


class Chapter_8
{
	/*
	智能指针分为 unique_ptr shared_ptr weak_ptr
	*/

public:

	/*
	unique_ptr 只是一个对象，不要企图对它调用delete，会自动管理初始化时的指针，在离开作用域时析构释放内存
	没有定义加减运算、不能随意移动指针地址，避免了指针越界等危险操作

	使用建议：尽量不要对unique_ptr 执行赋值操作，不需要管理它
	*/
	void TestUniquePtr()
	{
		unique_ptr<int> ptr1(new int(10));   
		assert(*ptr1 == 10);							//可以使用*取内容
		assert(ptr1 != nullptr);						//可以判断是否为空指针

		unique_ptr<string> ptr2(new string("hello"));
		assert(*ptr2 == "hello");						
		assert(ptr2->size() == 5);						//可以使用->调用成员函数

		//编译不通过，无法操作
		//ptr1++;
		//ptr2 += 2;
		//delete ptr1;

		//未初始化的智能指针，相当于操作了空指针，运行时会产生致命错误(core dump等)
		//		unique_ptr<int> ptr3;
		//		*ptr3 = 12;
		//可以使用工厂函数 创建智能指针 会在创建指针的时候初始化
		//同时使用 auto 简化变量声明
#if __cplusplus >= 201402
		auto ptr3 = make_unique<int>(12);
		assert(ptr3 && *ptr3 == 42);

		auto ptr4 = make_unique<string>("god of war");
		assert(!ptr4->empty());
#endif

		//unique_ptr 表示指针的所有权是唯一的，不允许共享，任何时候只能被一个对象持有
		//unique_ptr 应用了c++ 的转移move 语义，同时禁止了拷贝拷贝赋值，
		//			如果需要向另一个unique_ptr赋值时，需要用 std::move()显式声明所有权转移，转移后原unique_ptr变为了空指针

		auto ptr5 = my_make_unique<int>(12);
		assert(ptr5 && *ptr5 == 42);
		auto ptr6 = std::move(ptr5);					//使用move()转移所有权
		assert(!ptr5 && ptr6);							//ptr5变为空指针
	}

	//C++ 11 实现 make_unique
	template<class T, class... Args>						//可变参数列表
	std::unique_ptr<T>										//返回智能指针
		my_make_unique(Args&&... args)						//可变参数模板的入口函数
	{
		return std::unique_ptr<T>(							//构造智能之指针
			new T(std::forward<Args>(args)...));			//完美转发
	}


	/*
	shared_ptr 的所有权可以被安全共享，即支持拷贝赋值，允许被多个对象持有，和原始指针一样

	使用引用计数实现安全共享
	引用计数最开始的时候是 1，表示只有一个持有者。
	如果发生拷贝赋值——也就是共享的时候，引用计数就增加，而发生析构销毁的时候，引用计数就减少。
	只有当引用计数减少到 0，也就是说，没有任何人使用这个指针的时候，它才会真正调用 delete 释放内存。

	使用推荐：
	可以在任何场合替换原始指针，不用担心资源回收问题

	由于引用计数的存在，存储和管理成本远大于 unique_ptr，除极端情况下，shared_ptr开销都很小

	运行阶段，引用计数的变动很复杂，很难确定shared_ptr的释放时机
	如果对象的析构函数存在复杂、严重阻塞的操作，一旦shared_ptr在某个不确定的时间点析构释放资源，就会阻塞整个进程或者线程
	类似于Java / C#中的GC Stop the world（卡顿）
	*/
	void TestSharedPtr()
	{
		shared_ptr<int> ptr1(new int(10));
		assert(*ptr1 = 10);

		shared_ptr<string> ptr2(new string("hello"));  // string智能指针
		assert(*ptr2 == "hello");                      // 可以使用*取内容

#if __cplusplus >= 201402
		auto ptr3 = make_shared<int>(42);  // 工厂函数创建智能指针
		assert(ptr3 && *ptr3 == 42);       // 可以判断是否为空指针

		auto ptr4 = make_shared<string>("zelda");  // 工厂函数创建智能指针
		assert(!ptr4->empty());                   // 可以使用->调用成员函数
#endif

		assert(ptr1 && ptr1.unique());				//此时指针有效且唯一

		auto ptr5 = ptr1;							//直接拷贝赋值，不需要move()
		assert(ptr1 && ptr5);						//此时两个智能指针均有效

		assert(ptr1 == ptr5);						//share_ptr可以直接比较
		//两个智能指针均不唯一，且引用计数为2
		assert(!ptr1.unique() && ptr1.use_count() == 2);
		assert(!ptr5.unique() && ptr5.use_count() == 2);
	}

	//C++ 11 实现 make_shared
	template<class T, class... Args>						//可变参数列表
	std::shared_ptr<T>										//返回智能指针
		my_make_shared(Args&&... args)						//可变参数模板的入口函数
	{
		return std::shared_ptr<T>(							//构造智能之指针
			new T(std::forward<Args>(args)...));			//完美转发
	}


	void TestSharedPtrCircularRef()
	{
		auto n1 = my_make_shared<Node>();
		auto n2 = my_make_shared<Node>();
		assert(n1.use_count() == 1);
		assert(n2.use_count() == 1);

		//循环引用
		n1->next = n2;
		n2->next = n1;

		//引用计数无法减少到0，无法销毁，造成内存泄露
		assert(n1.use_count() == 2);
		assert(n2.use_count() == 2);
	}

#if USE_WEAK_PTR
	void TestWeakPtrCircularRef()
	{
		auto n1 = my_make_shared<Node>();
		auto n2 = my_make_shared<Node>();
		assert(n1.use_count() == 1);
		assert(n2.use_count() == 1);

		//循环引用
		n1->next = n2;
		n2->next = n1;

		//因为使用了weak_ptr 引用计数为1
		//打破循环引用，不会导致内存泄露
		assert(n1.use_count() == 1);
		assert(n2.use_count() == 1);

		
		//weak_ptr 作用是 弱引用 不一定要持有对象 只是偶尔 看看对象是否存在  对象可以不存在
		if (!n1->next.expired()) { //判断weak_ptr是否为空 即是否有效
			auto ptr = n1->next.lock();  //获取shared_ptr  如果weak_ptr不为空，可以获取其关联的强引用shared_ptr
			assert(ptr == n2);
		}
	}
#endif


	//扩展：？
	/*
	定制智能指针的释放和内存管理
	可以在智能指针的模板参数里定制删除函数，执行特别的资源释放，而非简单的delete   
	*/
};


/**************************************** 测试案例 ******************************************/

class A
{
public:
	A() { printf("create A();\n"); }
	~A() { printf("delete A();\n"); }
};

class C;
class B
{
public:
	B() { printf("create B();\n"); }
	~B() { printf("delete B();\n"); }

	std::shared_ptr<C> pc;
};

class C
{
public:
	C() { printf("create C();\n"); }
	~C() { printf("delete C();\n"); }

	std::shared_ptr<B> pb;
};

class E;
class D
{
public:
	D() { printf("create D();\n"); }
	~D() { printf("delete D();\n"); }

	std::shared_ptr<E> pe;
};

class E
{
public:
	E() { printf("create E();\n"); }
	~E() { printf("delete E();\n"); }

	std::weak_ptr<D> pd;
};

void smart_ptr_sample()
{
	std::cout << "base use \n";
	{
		auto pa = std::make_shared<A>();
	}

	std::cout << "\n===================================\n";
	std::cout << "ref cycle \n";
	{

		auto pb = std::make_shared<B>();
		auto pc = std::make_shared<C>();

		pb->pc = pc;
		pc->pb = pb;
	}

	std::cout << "\n===================================\n";
	std::cout << "ref cycle (break one ref)\n";
	{
		auto pb = std::make_shared<B>();
		auto pc = std::make_shared<C>();

		pb->pc = pc;
		pc->pb = pb;
		pb->pc = nullptr;
	}

	std::cout << "\n===================================\n";
	std::cout << "use weak_ptr\n";
	{
		auto pd = std::make_shared<D>();
		auto pe = std::make_shared<E>();

		pd->pe = pe;
		pe->pd = pd;
	}
}

/**************************************** 测试案例 ******************************************/

#endif  //_CHAPTER_8_H_ 