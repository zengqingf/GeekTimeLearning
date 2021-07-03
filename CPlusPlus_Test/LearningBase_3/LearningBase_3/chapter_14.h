#pragma once
#ifndef _CHAPTER_14_H_
#define _CHAPTER_14_H_

#include <iostream>
#include <string>

#include <thread>   //std::thread  std::this_thread
#include <mutex>	//std::call_once std::once_flag
#include <atomic>   //std::atomic
#include <future>   //std::async

using std::string;

/*
C++中 就语言本身来说  线程是一个能独立运行的函数

任何程序一开始就有一个主线程，它从 main() 开始运行。
主线程可以调用接口函数，创建出子线程。
子线程会立即脱离主线程的控制流程，单独运行，但共享主线程的数据。
程序创建出多个子线程，执行多个不同的函数，也就成了多线程。

多线程的作用：
	任务并行，避免I/O阻塞，充分利用CPU，提高界面响应速度 等

多线程可能引发的问题：
	同步，死锁，数据竞争，系统调度开销 等


多线程开发原则：
	读而不写，不会有数据竞争
	（读取 const 变量总是安全的，对类调用 const 成员函数、对容器调用只读算法也总是线程安全的）

	最好的并发就是没有并发，最好的多线程就是没有线程。
	（在大的、宏观的层面上“看得到”并发和线程，而在小的、微观的层面上“看不到”线程，减少死锁、同步等恶性问题的出现几率）
*/
class Chapter_14
{
public:

	void TestCase1()
	{
		auto f = []()
		{
			std::cout << "tid = " << std::this_thread::get_id() << std::endl;
		};

		std::thread t1(f);				//启动一个线程  运行函数f
	}


	/*
	C++ 多线程开发实践
	四个工具：
	仅调用一次、线程局部存储、原子变量、线程对象
	*/


	//仅调用一次
	//完全消除 初始化 并发冲突
	//调用位置不存在并发和线程
	void TestCase2()
	{
		static std::once_flag flag;                //全局初始化标志

		auto f = []()
		{
			std::call_once(flag,
				[]() {
				std::cout << "only once" << std::endl;
				}
			);
		};

		std::thread t1(f);
		std::thread t2(f);
	}

	//线程局部存储
	//解决数据竞争场景
	void TestCase3()
	{
		//使用线程内的局部存储数据，实现线程独占
		//替换全局（或局部静态）变量
		static int n = 0;
		//thread_local int n = 0;

		auto f = [&](int x)
		{
			n += x;
			std::cout << n << ",";              //std::endl; 如果使用thread_local   输出为 1020  而非  10\n20
		};

		std::thread t1(f, 10);
		std::thread t2(f, 20);

		//在线程里启动一个异步任务，不关心返回值
		t2.detach();
	}


	//原子变量
	//针对非独占的 必须共享的数据，解决同步问题
	/*
	要想保证多线程读写共享数据的一致性，关键是要解决同步问题，不能让两个线程同时写，也就是“互斥”
	互斥量 mutex 针对大数据

	原子化 针对小数据
	*/
	void TestCase4()
	{
		//模板类 atomic 的特化形式，包装了原始的类型，具有相同的接口，
		//用起来和 bool、int 几乎一模一样，但却是原子化的，多线程读写不会出错
		//原子变量禁用了拷贝构造函数，初始化时不能用 = 的赋值形式  只能用圆括号和花括号

		//以下std中都有
		using atomic_bool = std::atomic<bool>;
		using atomic_int = std::atomic<int>;
		using atomic_long = std::atomic<long>;

		atomic_int  x {0};
		atomic_long y {1000L};


		//原子操作：整数模拟运算
		assert(++x == 1);
		y += 200;
		assert(y < 2000);

		//原子操作：store、load、fetch_add、fetch_sub、exchange
		//CAS （Compare And Swap）compare_exchange_weak/compare_exchange_strong  
		//TAS （Test And Set）    atomic_flag  不完全是bool的特化(atomic)  没有store load操作  只是用来实现TAS 保证绝对无锁

		//原理变量的作用：
			//当作线程安全的全局计数器或者标志位
			//实现高效的无锁数据结构(lock-free)  容易出现 ABA问题 一个线程不知道在它被抢占期间有没有修改过共享的变量  ==>不建议自己实现


		static std::atomic_flag flag{false};
		static std::atomic_int n;

		auto f = [&]()								//捕获引用
		{
			auto value = flag.test_and_set();		//TAS检查原子标志			
			if (value) {
				std::cout << "flag has been set." << std::endl;
			}
			else {
				std::cout << "set flag by " << std::this_thread::get_id() << std::endl;
			}

			n += 100;						     //原子变量加法

			using namespace std::chrono_literals;			
			std::this_thread::sleep_for(		 //线程睡眠	
				n.load() * 10ms);				 //使用时间字面量
			std::cout << n << std::endl;
		};

		std::thread t1(f);
		std::thread t2(f);

		t1.join();							   //等待线程结束
		t2.join();
	}

	//尽量不使用 thread
	//使用async代替
	void TestCase5()
	{
		auto task = [](auto x)
		{
			using namespace std::chrono_literals;
			std::this_thread::sleep_for(x * 1ms);
			std::cout << "sleep for " << x << std::endl;
			return x;
		};
		auto f = std::async(task, 10);				//启动一个异步任务
		f.wait();									//等待任务完成

		assert(f.valid());							//确定已经完成了任务
		std::cout << f.get() << std::endl;			//获取任务结果
		assert(!f.valid());
	}

	void TestCase6(int(*fp)(int a))
	{
		auto f1 = std::async(fp, 20);
		std::cout << "future async ..." << std::endl;
		f1.wait();
		std::cout << f1.get() << std::endl;
	}

	static int Fib(int n)
	{
		if (n < 2)
		{
			return 1;
		}

		return Fib(n - 1) + Fib(n - 2);
	}
};


#endif // _CHAPTER_14_H_