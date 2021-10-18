#pragma once
#ifndef _CHAPTER_9_H_
#define _CHAPTER_9_H_

#include <iostream>
#include <string>
#include <stdexcept>

/*
异常是C++为了处理错误而提出的一种解决方案，处理错误的解决方案并非只有异常这一种

错误码是早期处理异常的一种解决方案，缺点在于会和正常业务逻辑代码混杂在一起，同时错误码可以被忽略，不被处理

异常：
	1. 处理流程是完全独立的
	2. 不能被忽略，必须被处理
	3. 相比于C中使用错误码，根据C++理念和特点，建立使用异常来处理

异常的抛出和处理 需要特别的栈展开（stack unwind）  
如果异常位置很深，又没被处理，或者频繁地抛出异常，会影响性能，影响正常逻辑的处理速度

使用异常的判断：
1. 不允许被忽略的错误；
2. 极少数情况下才会发生的错误；								（例：读写文件很少出错，建议使用异常而非错误码处理；socket通信受网络环境原因极其不稳定，建议使用错误码并重试）
3. 严重影响正常流程，很难恢复到正常状态的错误；				（例：构造函数内部初始化失败，无法创建，后续逻辑无法继续，建议用异常处理）
4. 无法本地处理，必须“穿透”调用栈，传递到上层才能被处理的错误。



C++中没有finally最终执行的代码块


建议对明确要抛出异常的函数声明为 noexcept(false)
封装库开放外部接口时，尽量使用错误码而非异常

//from geektime 评论区 
涉及磁盘操作的最好使用异常+调用栈，
涉及业务逻辑的最好利用日志+调用栈，
涉及指针和内存分配的还是用日志+调用栈吧，这种coredump一般是内存泄露和内存不够引起的。

*/
class Chapter_9
{
public:

	/*
	坑：异常只能按照 catch 块在代码里的顺序依次匹配，而不会去找最佳匹配。

	建议只使用一个catch块，绕过这个坑
	*/

	void TestException1()
	{
		try
		{
			_raise("error occurred");				 //使用封装的raise 抛出异常
		}
		catch (const std::exception& e)				//注意使用const &形式，避免函数拷贝
		{
			std::cout << e.what() << std::endl;		
		}
	}

	/*
	function-try 形式

	减少了一层缩进
	*/
	void TestException2()
	try
	{
		_raise("func error...");
	}
	catch (const std::exception e)
	{
		std::cout << e.what() << std::endl;
	}


	/*
	noexcept 编译阶段指令 修饰函数 
	表明当前函数不会抛出异常
	编译器不会对函数进行优化，不会添加栈展开的额外代码，消除异常处理的成本

	重要的构造函数（普通构造、拷贝构造、转移构造）、析构函数 尽量声明为 noexcept
	析构要保证绝不会抛出异常
	*/
	void TestNoException() noexcept       //声明不会抛出异常
	{
		std::cout << "noexcept" << std::endl;


		_raise("不是强保证，不能保证函数不会抛出异常");
	}

private:
	/*
	抛出异常时，建议封装一个中间层函数 来调用
	*/
	[[noreturn]]							 //标签属性
	void _raise(const char* msg)			 //封装throw  没有返回值
	{
		throw MyException(msg);              //抛出异常 也可以有更多的逻辑
	}
};

/*
C++设计了一个配套的异常类型体系
stdexcept

建立不要从基类扩展出自定义异常类，可以从第二层开始继承
*/
class MyException : public std::runtime_error
{
public:
	using this_type = MyException;
	using super_type = std::runtime_error;
public:
	MyException(const char* msg) :
		super_type(msg)							//别名也可以用于构造
	{}

	MyException() = default;
	~MyException() = default;
private:
	int code = 0;
};


#endif  //_CHAPTER_9_H_ 