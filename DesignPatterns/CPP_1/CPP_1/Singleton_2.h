#pragma once

#include <iostream>		//std::cout
#include <thread>		//std::thread
#include <mutex>		//std::mutex
#include <atomic>		//std::atomic

namespace SingletonExample_2
{
	/*
	���߳� vs. ���߳�
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
	
	//�̷߳ǰ�ȫ�汾
	Singleton* Singleton::getInstance() {
		if (nullptr == m_instance) {
			m_instance = new Singleton();
		}
		return m_instance;
	}

	//�̰߳�ȫ�汾�������Ĵ��۸ߣ������ڸ߲���ʱ
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


	//˫���汾�������ã������������ڣ��������еģ����������ڴ��дʱ��������reorder�Ż����� �ڴ�������ʹ���ʱ������˳��ı�
	// new˳������ڴ棬���ù��캯���������ڴ��ַ
	// reorder�󣬿��ܱ�Ϊ�����ڴ棬�����ڴ��ַ���ٵ��ù��캯��
	// ����һ���̷߳����ڴ���ڴ��ַ��ֵ����һ���߳���ռ�󣬷��ֱ�����Ϊ�գ�ֱ�ӷ��س�ȥ������ʱ��δ���ù��캯��

	//C# JavaҲ����ͬ�������� ������volatile�ؼ��֣���ֹ����������reorder�Ż�
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


	//C++ 11 ֮��汾�Ŀ�ƽ̨ʵ�֣�����volatile��
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
		std::_Atomic_thread_fence(std::memory_order_acquire);	//��ȡ�ڴ�fence

		if (nullptr == tmp)
		{
			std::lock_guard<std::mutex> lock(m_mutex);
			tmp = m_instance.load(std::memory_order_relaxed);
			if (nullptr == tmp)
			{
				tmp = new SingletonCpp11;
				std::_Atomic_thread_fence(std::memory_order_release); //�ͷ��ڴ�fence
				m_instance.store(tmp, std::memory_order_relaxed);
			}
		}
		return tmp;
	}
}