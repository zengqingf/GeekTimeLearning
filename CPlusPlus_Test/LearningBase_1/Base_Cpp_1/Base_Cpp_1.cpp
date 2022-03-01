// Base_Cpp_1.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include <iostream>

#include "base_1.h"
#include "base_2.h"
#include "base_4.h"
#include "base_5.h"
#include "base_6.h"
#include "base_7.h"

#include <math.h>

int main()
{
    std::cout << "Hello World!\n";

	//base_2.h 测试传值 vs. 传引用 vs. 传指针
	Base_2 b22;
	b22.test2();
	return 0;

	//base_7.h 测试运算符结合次序和运算顺序的关系
	//TestOperator_1 to_1;
	//to_1.Test1();

	//base_7.h 测试数学计算
	TestMath_1 tm_1;
	//tm_1.Test1();
	tm_1.Test2();
	return 0;


	//base_1.h 测试指针外部存储导致野指针问题
	TA* testA = new TA();
	std::cout << "testA address 1: " << testA << std::endl;
	TB testB;
	testB.StoreTA(testA);
	delete testA;
	std::cout << "testA address 2: " << testA << std::endl;

	TA* teatA2 = new(testA)TA;
	teatA2->Deal();
	//! testA->Deal(); error:
	//testA = nullptr;
	testB.ReleaseTA();
	return 0;


	//base_4.h type cast


	//base_2.h  check null before delte ptr
	Base_2 *b2ptr_1 = new Base_2();
	Base_2 *b2ptr_2 = new Base_2();
	Base_2::test_ptr_need_check_null(b2ptr_1, b2ptr_2);
	return 0;


	//base_1.h  explicit测试
	//不加explicit
	TestExplicit_1::Print(1);  //隐式转型
	TestExplicit_1 te1_1 = 2;  //赋值初始化--->使用 TestExplicit_1::TestExplicit_1(int x, int y = 0)
	TestExplicit_1::Print(te1_1);
	TestExplicit_1 te1_2(3);   //直接初始化--->使用 TestExplicit_1::TestExplicit_1(int x, int y = 0)
	TestExplicit_1::Print(te1_2);
	//添加explicit
	//TestExplicit_2::Print(3);  compile error 隐式转型 不使用TestExplicit_2::TestExplicit_2(int x, int y = 0)
	//TestExplicit_2 te2_1 = 3; compile error  赋值初始化 不使用TestExplicit_2::TestExplicit_2(int x, int y = 0)
	TestExplicit_2 te2_1 = (TestExplicit_2)4;  //显示转型，进行static_cast
	TestExplicit_2::Print(TestExplicit_2(4));
	TestExplicit_2 te2_2(5);   //直接初始化 使用TestExplicit_2::TestExplicit_2(int x, int y = 0)
	TestExplicit_2::Print(te2_2);
	return 0;


	//base_2 测试char wchar
	Base_2 base_22;
	base_22.test_char_wchar();
	return 0;


	//base_6  左值右值测试
	TestLRValue tlrv_1;
	tlrv_1.Test_1();
	return 0;


	//base_5 智能指针测试
	//1. 栈中创建对象
	TestObject *ptr1;
	{
		TestObject test1;
		ptr1 = &test1;
	}
	ptr1->TestFunc();  //test已经被销毁了

	//2. 在堆中创建对象
	{
		TestObject* ptr2 = new TestObject();
		ptr2->TestFunc();  //创建的对象 离开作用域 也不会被销毁  需要找合适的时机来销毁
	}

	//3.自定义智能指针测试
	{
		TestObject* ptr2 = new TestObject();
		TestSmartPointer<TestObject> smartPtr1 = ptr2;
		smartPtr1->TestFunc();
		{
			TestSmartPointer<TestObject> smartPtr2 = smartPtr1;
			{
				TestSmartPointer<TestObject> smartPtr3 = smartPtr2;
			}
			cout << "SmartPtr3 leave action scope" << endl;
		}
		cout << "SmartPtr2 leave action scope" << endl;
	}
	cout << "SmartPtr1 leave action scope" << endl;

	return 0;


	//base_4 类型转换测试
	std::vector<BaseOfBase*> vpbb;
	Base_CC<int>            bi;
	Derived_CC<int>         di;
	Base_CC<std::string>    bs;
	Derived_CC<std::string> ds;

	bi.val = 1;
	di.val = 2;
	bs.val = "foo";
	ds.val = "bar";

	vpbb.push_back(&bi);
	vpbb.push_back(&di);
	vpbb.push_back(&bs);
	vpbb.push_back(&ds);

	for (auto const & pbb : vpbb)
		pbb->do_something();
	return 0;

	Base_BB<Derived_BB> *b = new Derived_BB();
	std::cout << b->method<bool>() << std::endl;
	return 0;


	//base_2 指针和引用测试
	Base_2 base_2;
	//base_2.test_swap_by_pointer();
	//base_2.test_pass_by_value_pointer();
	//base_2.test_pass_by_reference();
	//base_2.test_pass_by_reference_pointer();
	//base_2.test_pass_by_pointer_pointer();
	base_2.test_char_with_const();
	return 0;


	//Base_1 基类派生类
	Derived der;
	der.get("", 0);
	der.get("");
	return 0;


	//测试c / c++输出
	int i = 8;
	cout << "i=" << i << endl;
	//for c
	printf("i = %d \n", i);
	return 0;


	//测试new int() vs. new int
	int *p_1 = new int();
	cout << *p_1 << endl; // 输出0 默认构造函数
	int *q_1 = new int;
	cout << *q_1 << endl; //输出一个很大的数
	delete p_1;
	delete q_1;
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
