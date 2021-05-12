#include "base_6.h"

void TestLRValue::Test_1()
{
	int a = 0;
	ProcessValue(a);

	//move() 只执行类型转换，不会调用move构造函数和move赋值函数
	ProcessValue(std::move(a));

	ProcessValue(1);
	ForwardValue(2);
}


//左值和右值传参，被编译器判定为合法的重载
void TestLRValue::ProcessValue(int & i)
{
	std::cout << "LValue processed: " << i << std::endl;
}

//左值和右值传参，被编译器判定为合法的重载
void TestLRValue::ProcessValue(int && i)
{
	std::cout << "RValue processed: " << i << std::endl;
}

//如果临时对象通过一个接受右值的函数传递给另一个函数时，就会变成左值，因为这个临时对象在传递过程中，变成了命名对象。
//（所谓的左值和右值仅仅是针对一次函数调用的，多次传参其实就会发生左右值的性质变化）
void TestLRValue::ForwardValue(int && i)
{
	//接受了右值，但是传递给ProcessValue时 i又是左值了
	ProcessValue(i);
}

