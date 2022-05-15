#pragma once
#include "thread"

class BaseStatic_1
{
	public:
		BaseStatic_1()
		{
			//sleep(10); // 故意让初始化过程放慢
			std::this_thread::sleep_for(std::chrono::milliseconds(10));	// 故意让初始化过程放慢
			m_num = 1;
		};
		~BaseStatic_1() {};

		void print(int index) { printf("[%d] - %d \n", index, m_num); }

	private:
		int m_num;
};

void func(int index)
{
	//static BaseStatic_1 bs; // 静态局部变量，默认构造
	BaseStatic_1 b;
	static BaseStatic_1 bs = b; // 静态局部变量，拷贝构造

	bs.print(index);
}

void test_static()
{
	// 三个线程同时执行
	//boost::thread trd1(boost::bind(&func, 1));
	//boost::thread trd2(boost::bind(&func, 2));
	//boost::thread trd3(boost::bind(&func, 3));

	std::thread t1(func, 1);
	std::thread t2(func, 2);
	std::thread t3(func, 3);

	//sleep(1000);
	//std::this_thread::sleep_for(std::chrono::milliseconds(1000));
}