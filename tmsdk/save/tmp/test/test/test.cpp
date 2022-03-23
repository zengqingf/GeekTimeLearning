// test.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include <iostream>
#include <string>
#include "stddef.h"

using namespace std;

class A
{
public:
	A();
	~A();
	int a;
	bool b;
	long long c;

	virtual void Name() = 0;
};

A::A():
	a(INT_MIN),
	c(LLONG_MIN)
{
	cout << "asdfasdfsadf" << endl;

}

A::~A()
{
}


class B : public A
{
public:
	B();
	~B();
	virtual void Name() override {
		cout << "BBBB" << endl;

	}
private:

};

template<typename T1, typename T2>
T1 ConvertType(T2* input)
{
	return static_cast<T1>(*input);
}
#define BASEADDR_OFFSET(baseAddr,type,membertype,membername) auto membername##_offset = offsetof(type,membername); \
auto membername##_ptr = reinterpret_cast<membertype*>(baseAddr + membername##_offset)
B::B()
{
	
	//auto baseAddr = (char*)&*this;
	auto baseAddr = reinterpret_cast<char*>(this);

// 	auto a_offset = offsetof(A, a);
// 	auto b_offset = offsetof(A, b);
// 	auto c_offset = offsetof(A, c);
// 	auto a_Ptr = reinterpret_cast<int*>(baseAddr + a_offset);
// 	auto b_Ptr = reinterpret_cast<bool*>(baseAddr + b_offset);
// 	auto c_Ptr = reinterpret_cast<long long*>(baseAddr + c_offset);
	BASEADDR_OFFSET(baseAddr, A, int, a);
	BASEADDR_OFFSET(baseAddr, A, bool, b);
	BASEADDR_OFFSET(baseAddr, A, long long, c);

	*a_ptr = INT_MAX;
	*b_ptr = true;
	*c_ptr = LLONG_MAX;


}

B::~B()
{
}
class C : public A
{
public:
	C();
	using A::A;

	~C();
	virtual void Name() override {
		cout << "CCCC" << endl;

	}
private:

};

C::C()
{
}

C::~C()
{
}
#include <windows.h>

int main()
{
	auto b = new B();
	auto c = new C();
	cout << "sleep start" << endl;

	Sleep(10 * 1000);
	cout << "sleep end" << endl;

	int* a = nullptr;
	a[1] =1;

	cout << typeid(b).name() << endl;

	memcpy(b, c, sizeof(c));
	auto b_ptr = (C*)(&*b);
	//auto b_ptr = (C*)(&*b);
	//*b_ptr = *c;
	cout << typeid(b_ptr).name() << endl;

	b_ptr->Name();
	cout << "123" << endl;

}

// 运行程序: Ctrl + F5 或调试 >“开始执行(不调试)”菜单
// 调试程序: F5 或调试 >“开始调试”菜单

// 入门使用技巧: 
//   1. 使用解决方案资源管理器窗口添加/管理文件
//   2. 使用团队资源管理器窗口连接到源代码管理
//   3. 使用输出窗口查看生成输出和其他消息
//   4. 使用错误列表窗口查看错误
//   5. 转到“项目”>“添加新项”以创建新的代码文件，或转到“项目”>“添加现有项”以将现有代码文件添加到项目
//   6. 将来，若要再次打开此项目，请转到“文件”>“打开”>“项目”并选择 .sln 文件
