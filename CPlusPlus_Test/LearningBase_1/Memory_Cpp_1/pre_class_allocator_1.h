#pragma once

#ifndef _PRE_CLASS_ALLOCATOR_1_H_
#define _PRE_CLASS_ALLOCATOR_1_H_

#include <cstddef>
#include <iostream>
#include <string>
#include <complex>
using std::cout;
using std::endl;
using std::string;
using std::complex;

namespace TestPreClassAllocator
{
	/*
	内存管理：
	1.减少malloc调用（即使malloc效率比较高）：事先分配一大块内存，减少cookie和malloc调用消耗
	2.
	*/

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
		Screen* next;					//为了链式存储（增加了4字节指针）
		static Screen* freeStore;
		static const int screenChunk;

	private:
		int m_i;					   //数据为4字节
	};

	void Test1();


	class Airplane
	{
	private:
		struct AirplaneRep {	//由于内存对齐，将 4 + 1 个字节看成 8 个字节
			unsigned long miles;
			char type;			
		};
	private:
		union {
			AirplaneRep rep;	
			Airplane* next;//通过union，将rep的前四个字节当成指针next，
						   //没有额外新增指针空间（嵌入式指针）	
		};

	public:
		unsigned long GetMiles() const { return rep.miles; }
		char GetType() const { return rep.type; }
		void Set(unsigned long m, char t)
		{
			rep.miles = m;
			rep.type = t;
		}
	public:
		static void* operator new(size_t size);
		static void operator delete(void* deadObj, size_t size);
	private:
		static const int BLOCK_SIZE;
		static Airplane* headOfFreeList;
	};

	void Test2();

	namespace TestUniAllocator
	{
		class UniAllocator
		{
		private:
			struct obj {
				struct obj* next;   //embedded pointer
			};

		public:
			void* Allocate(size_t);
			void Deallocate(void*, size_t);
			void Check();

		private:
			obj* freeStore = nullptr;
			const int CHUNK = 5;  //小一点方便观察结果
		};

		class Foo {
		public:
			long L;
			string Str;
			static UniAllocator UniAlloc;

		public:
			Foo(long l) : L(l) {}
			static void* operator new(size_t size) {
				return UniAlloc.Allocate(size);
			}
			static void operator delete(void* pdead, size_t size) {
				return UniAlloc.Deallocate(pdead, size);
			}
		};

#define DECLARE_POOL_ALLOC() \
public:\
	void* operator new(size_t size) { return uniAlloc.Allocate(size); }\
	void operator delete(void* p) { uniAlloc.deallocate(p, 0);}\
protected:\
	static UniAllocator uniAlloc;\

#define IMPLEMENT_POOL_ALLOC(class_name)\
UniAllocator class_name::uniAlloc;\

		//使用宏
		class Goo {
			DECLARE_POOL_ALLOC()
		public:
			complex<double> C;
			string Str;
		public:
			Goo(const complex<double>& x) : C(x) {}
		};

	}

	namespace OverrideClassNewDelete
	{
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
	} //namespace OverrideClassNewDelete

	namespace OverridePlacementNew
	{
		class Bad {};
		class Foo
		{
		public:
			Foo() { cout << "Foo::Foo()" << endl; }
			Foo(int i) : m_i(i)
			{
				cout << "Foo::Foo(int), i=" << m_i << endl;

				//会中断程序，一般会触发对应的placement delete
				//throw Bad();
			}

			// operator new 重载
			void* operator new(size_t size)
			{
				cout << "operator new(size_t size), size= " << size << endl;
				return malloc(size);
			}

			void* operator new(size_t size, void* start)
			{
				cout << "operator new(size_t size, void* start), size= " 
					<< size << " start= " << start << endl;
				return start;
 			}

			void* operator new(size_t size, long extra)
			{
				cout << "operator new(size_t size, long extra), size= "
					<< size << " extra= " << extra << endl;
				return malloc(size + extra);
			}

			void* operator new(size_t size, long extra, char init)
			{
				cout << "operator new(size_t size, long extra, char init), size= "
					<< size << " extra= " << extra << " init= " << init << endl;
				return malloc(size + extra);
			}

			//@注意： 必须以size_t为第一参数
			/*! void* operator new(long extra, char init)
			{
				cout << "operator new(size_t size, long extra, char init), size= "
					<< size << " extra= " << extra << " init= " << init << endl;
				return malloc(size + extra);
			}*/


			void operator delete(void*, size_t)
			{
				cout << "operator delete(void*, size_t)" << endl;
			}
			
			void operator delete(void*, void*)
			{
				cout << "operator delete(void*, void*)" << endl;
			}

			void operator delete(void*, long)
			{
				cout << "operator delete(void*, long)" << endl;
			}
			//delele没有对应placement new 编译不会报错
			void operator delete(void*, long, char)
			{
				cout << "operator delete(void*, long, char)" << endl;
			}

		private:
			int m_i;
		};

		void Test3();
	}

}	//namespace TestPreClassAllocator

#endif //_PRE_CLASS_ALLOCATOR_1_H_