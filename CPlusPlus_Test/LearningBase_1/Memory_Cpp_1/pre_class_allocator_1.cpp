#include "pre_class_allocator_1.h"

using namespace TestPreClassAllocator;

Screen* Screen::freeStore = nullptr;
const int Screen::screenChunk = 24;

void * Screen::operator new(size_t size)
{
	//不重载时
	//return malloc(size);

	Screen *p;
	if (!freeStore) {
		//linked list 为空，需要申请一块内存
		size_t chunk = screenChunk * size;
		freeStore = p =									//指针转型
			reinterpret_cast<Screen*>(new char[chunk]);
		//将一大块内存分割成片，当作linked list串接起来
		for (; p != &freeStore[screenChunk - 1]; ++p)
		{
			p->next = p + 1;
		}
		p->next = 0;
	}
	p = freeStore;
	freeStore = freeStore->next;
	return p;
}

void Screen::operator delete(void *p, size_t)
{
	//不重载时
	//free(p);
	//return;

	(static_cast<Screen*>(p))->next = freeStore;    //回收，放到链表头
	freeStore = static_cast<Screen*>(p);			//将链表当前指针指向链表头
}

void Screen::operator delete(void *p)
{
	(static_cast<Screen*>(p))->next = freeStore;
	freeStore = static_cast<Screen*>(p);
}

void TestPreClassAllocator::Test1()
{
	cout << "\ntest pre class allocator... \n";

	cout << sizeof(Screen) << endl;

	size_t const N = 100;
	Screen* p[N];

	for (int i = 0; i < N; ++i)
	{
		p[i] = new Screen(i);
	}

	//输出前10个pointers 比较间隔
	for (int i = 0; i < 10; ++i)
	{
		cout << p[i] << endl;
	}

	for (int i = 0; i < N; ++i)
	{
		delete p[i];
	}
}

Airplane* Airplane::headOfFreeList;
const int Airplane::BLOCK_SIZE = 512;

void * TestPreClassAllocator::Airplane::operator new(size_t size)
{
	//编译器传入size, 但是类继承可能导致这个地方大小不一致，需要做判断
	if (size != sizeof(Airplane))
	{
		return ::operator new(size);
	}
	Airplane* p = headOfFreeList = nullptr;
	if (p) {
		headOfFreeList = p->next;
	}
	else {
		Airplane* newBlock = static_cast<Airplane*>
			(::operator new(BLOCK_SIZE * sizeof(Airplane)));

		for (int i = 1; i < BLOCK_SIZE - 1; ++i)
			newBlock[i].next = &newBlock[i + 1];
		newBlock[BLOCK_SIZE - 1].next = nullptr;

		p = newBlock;
		headOfFreeList = &newBlock[1];
	}
	return p;
}

void TestPreClassAllocator::Airplane::operator delete(void* deadObj, size_t size)
{
	if (deadObj == 0)
	{
		return;
	}
	if (size != sizeof(Airplane)) {
		::operator delete(deadObj);
		return;
	}
	Airplane *carcass = static_cast<Airplane*>(deadObj);
	carcass->next = headOfFreeList;
	headOfFreeList = carcass;

	//TODO 暂时不处理free操作，但未发生内存泄露！
}

void TestPreClassAllocator::Test2()
{
	cout << "\ntest pre class allocator 2... \n";
	cout << sizeof(Airplane) << endl;  //8

	const size_t N = 100;
	Airplane* p[N];
	for (int i = 0; i < N; ++i)
	{
		p[i] = new Airplane;
	}

	p[1]->Set(1000, 'A');
	p[5]->Set(2000, 'B');
	p[9]->Set(3000, 'C');
	cout << p[1] << ' ' << p[1]->GetType() << ' ' << p[1]->GetMiles() << endl;
	cout << p[5] << ' ' << p[5]->GetType() << ' ' << p[5]->GetMiles() << endl;
	cout << p[9] << ' ' << p[9]->GetType() << ' ' << p[9]->GetMiles() << endl;

	for (int i = 0; i < 10; ++i)
	{
		cout << p[i] << endl;
	}
	for (int i = 0; i < N; ++i)
	{
		delete p[i];
	}
}

/********************** 重载 ::operator new / ::operator delete  Start ********************/
/*
尽量不要重载 全局 分配和释放接口，影响太大

void* myAlloc(size_t size)
{
	return malloc(size);
}

void myFree(void* ptr)
{
	return free(ptr);
}

inline void* operator new(size_t size)
{
	cout << "global new() \n";
	return myAlloc(size);
}

inline void* operator new[](size_t size)
{
	cout << "global new[]() \n";
	return myAlloc(size);
}

inline void operator delete(void* ptr)
{
	cout << "global delete() \n";
	myFree(ptr);
}

inline void operator delete[](void* ptr)
{
	cout << "global delete[]() \n";
	myFree(ptr);
}
*/
/********************** 重载 ::operator new / ::operator delete End ********************/

void * TestPreClassAllocator::OverrideClassNewDelete::Foo::operator new(size_t size)
{
	Foo* ptr = (Foo*)malloc(size);
	cout << "Foo::operator new(), size= " << size << "\t return: " << ptr << endl;
	return ptr;
}

void TestPreClassAllocator::OverrideClassNewDelete::Foo::operator delete(void * deadObj, size_t size)
{
	cout << "Foo::operator delete(), ptr dead= " << deadObj << " size= " << size << endl;
	free(deadObj);
}

void * TestPreClassAllocator::OverrideClassNewDelete::Foo::operator new[](size_t size)
{
	Foo* ptr = (Foo*)malloc(size);
	cout << "Foo::operator new[](), size= " << size << "\t return: " << ptr << endl;
	return ptr;
}

void TestPreClassAllocator::OverrideClassNewDelete::Foo::operator delete[](void * deadObj, size_t size)
{
	cout << "Foo::operator delete[](), ptr dead= " << deadObj << " size= " << size << endl;
	free(deadObj);
}

void TestPreClassAllocator::OverrideClassNewDelete::Test2()
{
	cout << "\ntest pre class allocator, override class new delete ...\n";
	cout << "sizeof(Foo)= " << sizeof(Foo) << endl;

	{
		Foo* p = new Foo(7);
		delete p;

		Foo* pArray = new Foo[5];
		delete[] pArray;
	}
	{
		cout << "test global expression ::new and ::new[] \n";

		Foo* p = ::new Foo(7);  //:: 全局作用域 global scope operator::
								//会绕过所有overloaded functions，强制使用 global version
		::delete p;

		Foo* pArray = ::new Foo[5];
		::delete[] pArray;
	}
}

void TestPreClassAllocator::OverridePlacementNew::Test3()
{
	cout << "\ntest test override class placement new ...\n";
	Foo start;
	Foo* p1 = new Foo;
	Foo* p1_1 = new Foo();  //默认参数的话，相同

	delete p1;
	delete p1_1;

	Foo* p2 = new(&start) Foo;
	Foo* p3 = new(100) Foo;
	Foo* p4 = new(100, 'a') Foo;

	Foo* p5 = new(100) Foo(1);  //如果在这里构造函数时中断
								/*
	多个重载版本的delete，正常主动触发delete不会被调用，
	旧版本c++中，当new的ctor中抛出exception时，才会触发对应的重载版本delete
	主要用于清除未能完全创建的object所占用的memory
								*/

	Foo* p6 = new(100, 'a')Foo(1);
	Foo* p7 = new(&start)Foo(1);
	Foo* p8 = new Foo(1);

	//delete p2, p3, p4, p5, p6, p7, p8; //不能连续调用，只会触发一个默认的delete
	delete p2;							 //重载delete不会触发，只会触发默认的delete
	delete p3;
	delete p4;
	delete p5;
	delete p6;
	delete p7;
	delete p8;
}

void * TestPreClassAllocator::TestUniAllocator::UniAllocator::Allocate(size_t size)
{
	obj* p;
	if (!freeStore) {
		size_t chunk = CHUNK * size;
		freeStore = p = (obj*)malloc(chunk);

		for (int i = 0; i < (CHUNK - 1); ++i)
		{
			p->next = (obj*)((char*)p + size);
			p = p->next;
		}
		p->next = nullptr;				//last obj
	}
	p = freeStore;						//指向链表头
	freeStore = freeStore->next;		//当前空闲位置后移一位
	return p;
}

void TestPreClassAllocator::TestUniAllocator::UniAllocator::Deallocate(void * p, size_t)
{
	//将回收的deleted obj 插入到free list前端
	((obj*)p)->next = freeStore;
	freeStore = (obj*)p;
}

void TestPreClassAllocator::TestUniAllocator::UniAllocator::Check()
{
	obj* p = freeStore;
	int count = 0;
	while (p) {
		cout << p << endl;
		p = p->next;
		count++;
	}
	cout << count << endl;
}

TestPreClassAllocator::TestUniAllocator::UniAllocator 
	TestPreClassAllocator::TestUniAllocator::Foo::UniAlloc;			//实现

TestPreClassAllocator::TestUniAllocator::IMPLEMENT_POOL_ALLOC(		//实现
	TestPreClassAllocator::TestUniAllocator::Goo)