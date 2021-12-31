#include "base_7.h"


/*
表达式的结合次序取决于表达式中各种运算符的优先级！

&&比||优先级高，仅仅确定了这一层括号，而运算顺序仍然是从左到右，因此||后面的语句短路。

优先级只是决定了表达式的结合次序，而不是运算顺序！
*/
void TestOperator_1::Test1()
{
	int x = 2;
	int y = 1 || (x = 0) && (x += 1);
	std::cout << "x = " << x << " y = " << y << std::endl;
	//output: x = 2 y = 1

	int z = 1 || ((x = 2) && (x += 1));
	std::cout << "x = " << x << " z = " << y << std::endl;
	//output: x = 2 z = 1
}


/*
C++ 取整

floor(x)返回的是小于或等于x的最大整数。  向下取整
ceil(x)返回的是大于x的最小整数。		 向上取整
trunc(x)返回的是x舍取小数位后的整数。
*/
void TestMath_1::Test1()
{
	std::cout << std::floor(-5.5) << std::endl;//-6
	std::cout << std::floor(5.9) << std::endl;//5
	std::cout << std::ceil(-5.5) << std::endl;//-5
	std::cout << std::ceil(5.1) << std::endl;//6
	std::cout << std::trunc(-5.5) << std::endl;//-5
	std::cout << std::trunc(5.5) << std::endl;//5
	std::cout << std::trunc(5.4) << std::endl;//5
	std::cout << std::trunc(5.6) << std::endl;//5
	std::cout << std::trunc(-5.4) << std::endl;//-5
	std::cout << std::trunc(-5.6) << std::endl;//-5
}

//计算整数位数
void TestMath_1::Test2()
{
	using std::cout;
	using std::endl;
	//int n = 1 + (int)log10(num);  //返回类型double，num需大于0
	//log10 返回x的普通对数(以10为底)。

	double param, result;
	param = 1000.0;
	result = log10(param);
	printf("log10(%f) = %f\n", param, result);		//log10(1000.000000) = 3.000000

	cout << log10(101001) << endl;		//5.00433
	cout << log10(1010011) << endl;		//6.00433
	cout << log10(201001) << endl;		//5.3032
	cout << log10(2010011) << endl;		//6.3032

	cout << 1 + log10(1) << endl;	//1
	cout << 1 + log10(0) << endl;	//-inf
	cout << 1 + log10(-1) << endl;	//-nam(ind)
	//!cout << (int)log10(-2147483648) << endl;
	cout << 1 + (int)log10(2147483647) << endl; //10
	cout << 1 + (int)log10(2147483648) << endl; //10
	cout << 1 + (int)log10(214748364856464) << endl; //15


	int  len;
	char b[256];
	int a = 12345678;
	len = printf("%d:", a);		//会打印出"%d个数"的内容，注意如果你在"%d"中添加字符会增加位数
	printf("%d  ", len);			//输出位数为9, 9-1=8
	len = sprintf_s(b, "%d--", a);    //不打印"%d--"的内容，注意如果你在"%d"中添加字符会增加位数
	printf("%d", len);				//输出为10,10-2=8
}
