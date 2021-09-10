#pragma once

#ifndef _PRE_CLASS_ALLOCATOR_1_H_
#define _PRE_CLASS_ALLOCATOR_1_H_

#include <cstddef>
#include <iostream>
#include <string>
using std::cout;
using std::endl;
using std::string;

namespace TestPreClassAllocator
{
	class Screen
	{
	public:
		Screen(int x) : m_i(x) {};
		int get() { return m_i; }

		void* operator new(size_t);
		void* operator new(size_t, void* loc) { return loc; }	//placement new
		void operator delete(void*, size_t);
		void operator delete(void*);     //vs2017 两个operator delete可以共存

	private:
		Screen* next;
		static Screen* freeStore;
		static const int screenChunk;

	private:
		int m_i;
	};

	void Test1();

	class Foo
	{
	public:
		int id;
		long data;
		string str;

	public:
		//必须是静态的，c++默认可以不写
		static void* operator new(size_t size);
		static void operator delete(void* deadObj, size_t size);
		static void* operator new[](size_t size);
		static void operator delete[](void* deadObj, size_t size);

		Foo() : id(0)
		{
			cout << "default ctor, this=" << this << " id=" << id << endl;
		}
		Foo(int i) : id(i)
		{
			cout << "ctor, this=" << this << " id=" << id << endl;
		}
		virtual 
		~Foo()
		{
			cout << "dtor, this=" << this << " id=" << id << endl;
		}
	};

	void Test2();

}	//namespace TestPreClassAllocator

#endif //_PRE_CLASS_ALLOCATOR_1_H_