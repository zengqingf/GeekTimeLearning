#pragma once

#include <stdio.h>
#include <utility>			//std::swap
#include "chapter_01.h"

using namespace Cpp_Res_Manager_1;

namespace Smart_Ptr_1
{
	//添加引用计数
	class SharedCount;

	//自定义智能指针，模板
	template <typename T>
	class SmartPtr {
	public:
		explicit SmartPtr(T* ptr = nullptr)
			: m_ptr(ptr)
		{
			//添加引用计数
			if (ptr) {
				m_sharedCount = new SharedCount();
			}
		}

		~SmartPtr() 
		{
			//添加引用计数
			if (m_ptr &&
				!(m_sharedCount->ReduceCount())) 
			{
				delete m_ptr;

				delete m_sharedCount;
			}
		}
		T* Get() const noexcept { return m_ptr; }


	public:
		T& operator*() const noexcept { return *m_ptr; }		//用 * 运算符解引用
		T* operator->() const noexcept { return m_ptr; }		//用 -> 运算符指向对象成员
		operator bool() const noexcept { return m_ptr; }		//像指针一样用在布尔表达式里

	public:
		//SmartPtr(const SmartPtr&) = delete;			  //临时禁用标准拷贝构造
		//SmartPtr& operator=(const SmartPtr&) = delete;  //临时禁用标准拷贝赋值

	//C++98开始 auto_ptr的实现
	//C++17中已经删除auto_ptr
	public:
		/*测试
		SmartPtr(SmartPtr& other)					//拷贝构造函数
		{
			m_ptr = other.release();				//other指针指向空，other原来维护得指针转移到m_ptr中
													//@注意：如果将一个SmartPtr对象引用传递给另一个，这个SmartPtr对象将被释放！！！
		}
		*/
		SmartPtr& operator=(SmartPtr& rhs)			//拷贝赋值
		{
			puts("SmartPtr operator = copy ref");
			SmartPtr(rhs).swap(*this);				//这两处理不需要判断this和&rhs相同
			return *this;							//如果使用if(this != &rhs)提前判断是否自我赋值，
													//在发生异常时不能保证安全性，如在赋值过程中发生异常，此时this对象内容可能已经部分被破坏了，对象不在完整
													//使用swap方法，可以保证强异常安全性
													//赋值分为拷贝和交换，异常只可能出现在拷贝阶段，不会影响this对象（结果只有赋值成功和赋值无效两种状态）
		}

	private:
		/*测试
		T* release()								
		{
			T* ptr = m_ptr;
			m_ptr = nullptr;
			return ptr;
		}
		*/

		void swap(SmartPtr& rhs)
		{
			using std::swap;
			swap(m_ptr, rhs.m_ptr);
			
			//添加引用计数
			swap(m_sharedCount, rhs.m_sharedCount);
		}


	//C++11 unique_ptr基本实现
	public:
		/*测试
		SmartPtr(SmartPtr&& other)				//移动构造函数
		{										//如果提供了移动构造函数但没手动提供拷贝构造函数，拷贝构造函数会自动被禁用
			m_ptr = other.release();
		}
		*/
		SmartPtr& operator=(SmartPtr rhs) noexcept	//和 SmartPtr& operator=(const SmartPtr&) = delete; 冲突，重复定义
		{										    //在构造参数时直接生成新的智能指针，不需要在函数体内构造临时对象
			puts("SmartPtr operator = copy new");
			rhs.swap(*this);
			return *this;
		}
		
	public:
		/*测试
		template <typename U>
		SmartPtr(SmartPtr<U>&& other)			//构造函数模板：可以实现子类指针向基类指针转换；
												//实例化后可以是拷贝构造函数，也可以是移动构造函数（编译器不会默认识别为移动构造函数）
		{										//为万能引用：既能引用左值，也可以引用右值，在【完美转发】中很有用；
			m_ptr = other.release();
		}
		*/

	//添加引用计数
	public:
		SmartPtr(const SmartPtr& other)
		{
			puts("SmartPtr standard copy ctor");
			m_ptr = other.m_ptr;
			if (m_ptr) {
				other.m_sharedCount->AddCount();
				m_sharedCount = other.m_sharedCount;
			}
		}
		
		template<typename U>
		SmartPtr(const SmartPtr<U>& other)
		{
			puts("SmartPtr template copy ctor");
			m_ptr = other.m_ptr;
			if (m_ptr) {
				other.m_sharedCount->AddCount();
				m_sharedCount = other.m_sharedCount;
			}
		}

		template<typename U>
		SmartPtr(SmartPtr<U>&& other)
		{
			puts("SmartPtr template move ctor");
			m_ptr = other.m_ptr;
			if (m_ptr) {
				m_sharedCount = other.m_sharedCount;
				other.m_ptr = nullptr;						//移动构造：不需要调整引用数，认为other经过移动语义后不再指向该共享对象
			}
		}

		//添加引用计数
		long UseCount() const noexcept
		{
			if (m_ptr) {
				return m_sharedCount->GetCount();
			}
			else {
				return 0;
			}
		}


	//指针类型转换
	public:
		template<typename U>
		SmartPtr(const SmartPtr<U>& other, T* ptr) noexcept
		{
			puts("SmartPtr template ptr type cast copy ctor");
			m_ptr = ptr;
			if (m_ptr) {
				other.m_sharedCount->AddCount();
				m_sharedCount = other.m_sharedCount;
			}
		}

	private:
		T* m_ptr;
		SharedCount* m_sharedCount;


		//添加引用计数
		template<typename U>
		friend class SmartPtr;
	};

	template<typename T>
	void Swap(SmartPtr<T>& lhs, SmartPtr<T>& rhs) noexcept
	{
		lhs.swap(rhs);
	}

	//指针类型转换
	template<typename T, typename U>
	SmartPtr<T> DynamicPointerCast(const SmartPtr<U>& other) noexcept
	{
		T* ptr = dynamic_cast<T*>(other.Get());
		return SmartPtr<T>(other, ptr);
	}

	template<typename T, typename U>
	SmartPtr<T> StaticPointerCast(const SmartPtr<U>& other) noexcept
	{
		T* ptr = static_cast<T*>(other.Get());
		return SmartPtr<T>(other, ptr);
	}

	template<typename T, typename U>
	SmartPtr<T> ConstPointerCast(const SmartPtr<U>& other) noexcept
	{
		T* ptr = const_cast<T*>(other.Get());
		return SmartPtr<T>(other, ptr);
	}

	template<typename T, typename U>
	SmartPtr<T> ReinterpretPointerCast(const SmartPtr<U>& other) noexcept
	{
		T* ptr = reinterpret_cast<T*>(other.Get());
		return SmartPtr<T>(other, ptr);
	}

	void Test1()
	{
		puts("----------Smart Ptr Test1-----------");
		puts("---------1--------");
		SmartPtr<shape> ptr1{ create_shape(shape_type::circle) };
		puts("---------2--------");
		SmartPtr<shape> ptr2{ ptr1 };				//禁用拷贝构造后， 编译不通过
													//旧版本c++中，如果禁用了拷贝构造，编译不报错，
													//但是运行时会有未定义行为：同一内存释放两次（一般情况下会导致程序奔溃）
		puts("---------3--------");
		SmartPtr<shape> ptr3;
		//ptr3 = ptr1;								//存在 SmartPtr& operator=(SmartPtr& rhs) 和 SmartPtr& operator=(SmartPtr rhs) 赋值构造时，会产生二义性
		ptr3 = std::move(ptr1);
		puts("---------4--------");
		SmartPtr<shape> ptr4{ std::move(ptr3) };
		puts("---------5--------");
		SmartPtr<shape> ptr5 = SmartPtr<triangle>();
		puts("---------End--------");
	}


	/*
	output:
		----------Smart Ptr Test1-----------
		---------1--------
		base shape default ctor
		circle default ctor
		---------2--------
		SmartPtr standard copy ctor
		---------3--------
		SmartPtr template move ctor
		SmartPtr operator = copy new
		---------4--------
		SmartPtr template move ctor
		---------5--------
		SmartPtr template move ctor
		---------End--------
		circle dtor
		shape dtor
	*/


	//引用计数，简化版本（不考虑多线程等）
	class SharedCount {
	public:
		SharedCount() : m_count(1) {}
		void AddCount()
		{
			++m_count;
		}
		long ReduceCount()
		{
			return --m_count;
		}
		long GetCount() const
		{
			return m_count;
		}
	private:
		long m_count;
	};


	void Test2()
	{
		SmartPtr<circle> ptr1(new circle());
		printf("use count of ptr1 is %ld\n", ptr1.UseCount());
		SmartPtr<shape> ptr2;
		printf("use count of ptr2 is %ld\n", ptr2.UseCount());
		ptr2 = ptr1;
		printf("use count of ptr2 is now %ld\n", ptr2.UseCount());
		if (ptr1) {
			puts("ptr1 is not empty");
		}

		SmartPtr<circle> ptr3 = DynamicPointerCast<circle>(ptr2);
		printf("use count of ptr3 is %ld\n", ptr3.UseCount());
	}

	/*
	output:

	base shape default ctor
	circle default ctor

	use count of ptr1 is 1
	use count of ptr2 is 0

	SmartPtr template copy ctor
	SmartPtr operator = copy new
	use count of ptr2 is now 2

	ptr1 is not empty

	SmartPtr template ptr type cast copy ctor
	SmartPtr template move ctor
	SmartPtr template move ctor
	use count of ptr3 is 3

	circle dtor
	shape dtor
	*/
}