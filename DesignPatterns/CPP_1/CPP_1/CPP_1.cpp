// CPP_1.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include <iostream>

#include "FactoryMethod_1.h"
#include "AbstractFactory_1.h"
#include "Builder_1.h"
#include "Singleton_1.h"
#include "Factory_2.h"
#include "Prototype_1.h"

#include "Principle_1.h"

#include "Adapter_1.h"

/*
ref:
https://refactoringguru.cn/design-patterns
*/


int main()
{
	std::cout << "Hello Cpp World !\n";
	
	//创建型
	//Creator_FactoryMethod::TestFactoryMethod_1();
	//Creator_AbstractFactory::TestAbstractFactory_1();

	//BuilderExample_1::Test1();
	//TestBuilder1::Test1();

	//SingletonExample_1::Test2();
	//MultitonExample_1::Test1();

	//FactoryExample_1::Test1();
	//FactoryExample_3::Test1();
	//FactoryExample_4::Point2D* p2D = new FactoryExample_4::Point2D;
	//FactoryExample_4::Test1(p2D);
	//delete p2D;
	//FactoryExample_4::Test2();

	//PrototypeExample_1::Test3();
	//PrototypeExample_2::Test1();


	//设计原则
	//SRP_Example_1::Test1();
	//OCP_Example_1::Test1();
	//DIP_Example_1::Test1();
	//DIP_Example_1::Test2();


	//结构型
	//Adapter_Exampler_1::Test1();
	Adapter_Example_1::Test2();

	return 0;
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
