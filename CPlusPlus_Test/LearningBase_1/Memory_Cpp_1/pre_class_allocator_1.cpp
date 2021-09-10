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
		freeStore = p =
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

	(static_cast<Screen*>(p))->next = freeStore;
	freeStore = static_cast<Screen*>(p);
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

void * TestPreClassAllocator::Foo::operator new(size_t size)
{
	Foo* ptr = (Foo*)malloc(size);
	cout << "Foo::operator new(), size= " << size << "\t return: " << ptr << endl;
	return ptr;
}

void TestPreClassAllocator::Foo::operator delete(void * deadObj, size_t size)
{
	cout << "Foo::operator delete(), ptr dead= " << deadObj << " size= " << size << endl;
	free(deadObj);
}

void * TestPreClassAllocator::Foo::operator new[](size_t size)
{
	Foo* ptr = (Foo*)malloc(size);
	cout << "Foo::operator new[](), size= " << size << "\t return: " << ptr << endl;
	return ptr;
}

void TestPreClassAllocator::Foo::operator delete[](void * deadObj, size_t size)
{
	cout << "Foo::operator delete[](), ptr dead= " << deadObj << " size= " << size << endl;
	free(deadObj);
}

void TestPreClassAllocator::Test2()
{
	cout << "\ntest pre class allocator ...\n";
	cout << "sizeof(Foo)= " << sizeof(Foo) << endl;

	{
		Foo* p = new Foo(7);
		delete p;

		Foo* pArray = new Foo[5];
		delete[] pArray;
	}
	{
		cout << "test global expression ::new and ::new[] \n";

		Foo* p = ::new Foo(7);
		::delete p;

		Foo* pArray = ::new Foo[5];
		::delete[] pArray;
	}
}

