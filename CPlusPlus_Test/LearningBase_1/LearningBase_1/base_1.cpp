
#include "base_1.h"
#include <iostream>


int* Base_1::array_int = new int[2] { 0x1, 0x2 };

void* Base::get(const char* e)
{
	return 0;
}

void* Derived::get(const char* e, int index)
{
	return 0;
}

int TestFunc_1::_func(int a, int b = 1, int c = 2)
{
	return a + b * c;
}

void TestFunc_1::Test1()
{
	//编译错误
	//std::cout << _func(1) << std::endl;
}
