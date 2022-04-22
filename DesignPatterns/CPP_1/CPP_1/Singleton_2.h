#pragma once

#include <iostream>		//std::cout
#include <thread>		//std::thread
#include <mutex>		//std::mutex
#include <atomic>		//std::atomic

namespace SingletonExample_2
{
	/*
	单线程 vs. 多线程
	*/

	class Singleton {
	private:
		Singleton() {}
		Singleton(const Singleton& other);

	public:
		static Singleton* getInstance();
		static Singleton* m_instance;
	};

	Singleton* Singleton::m_instance = nullptr;
	
	//线程非安全版本
	Singleton* Singleton::getInstance() {
		if (nullptr == m_instance) {
			m_instance = new Singleton();
		}
		return m_instance;
	}

	//线程安全版本，但锁的代价高，尤其在高并发时
	Singleton* Singleton::getInstance() {
		std::mutex mtx;
		mtx.lock();
		if (nullptr == m_instance)
		{
			m_instance = new Singleton();
		}
		mtx.unlock;
		return m_instance;
	}


	//双检查版本（不可用！！！），由于（几乎所有的）编译器在内存读写时，都可能reorder优化导致 内存对象分配和创建时的流程顺序改变
	// new顺序分配内存，调用构造函数，返回内存地址
	// reorder后，可能变为分配内存，返回内存地址，再调用构造函数
	// 导致一个线程分配内存后，内存地址赋值，另一个线程抢占后，发现变量不为空，直接返回出去，但此时还未调用构造函数

	//C# Java也存在同样的问题 引入了volatile关键字，阻止编译器进行reorder优化
	Singleton* Singleton::getInstance() {

		if (nullptr == m_instance)
		{
			std::mutex mtx;
			mtx.lock();
			if (nullptr == m_instance)
			{
				m_instance = new Singleton();
			}
			mtx.unlock;
		}
		return m_instance;
	}


	//C++ 11 之后版本的跨平台实现（类似volatile）
	class SingletonCpp11 {
	private:
		SingletonCpp11() {}
		SingletonCpp11(const SingletonCpp11& other);

	public:
		static SingletonCpp11* getInstance();
		static std::atomic<SingletonCpp11*> m_instance;
		static std::mutex m_mutex;
	};

	SingletonCpp11* SingletonCpp11::getInstance() {
		SingletonCpp11* tmp = m_instance.load(std::memory_order_relaxed);
		std::_Atomic_thread_fence(std::memory_order_acquire);	//获取内存fence

		if (nullptr == tmp)
		{
			std::lock_guard<std::mutex> lock(m_mutex);
			tmp = m_instance.load(std::memory_order_relaxed);
			if (nullptr == tmp)
			{
				tmp = new SingletonCpp11;
				std::_Atomic_thread_fence(std::memory_order_release); //释放内存fence
				m_instance.store(tmp, std::memory_order_relaxed);
			}
		}
		return tmp;
	}
}