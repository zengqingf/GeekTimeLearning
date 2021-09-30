// LearningBase_1.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

//#include <iostream>

#include <stdio.h>

#include "test_big_little_endian.h"

int main()
{
    //std::cout << "Hello World!\n";
	printf("C Language Learning Start...");

	int i = 10;
	int *p = NULL;                  //给指针赋默认NULL  ==>  空指针
	p = &i;

	printf("int i 的地址是 ： %p\n", p);

	char ch = 'a';
	char *cp = &ch;
	printf("char ch 的地址是 ： %p\n", cp);
	printf("char ch 的值是 ： %c\n", *cp);

	/*
	1. p是一个指针， 存储变量 i 的地址
	2. 指针p 的类型必须和变量 i 的类型一致，因为整型的指针只能存储整型变量的指针地址
	*/

	/*
	指针：
	指针是一个变量，其值是指向变量的地址，即内存位置的直接地址
	一般形式：
		type *var-name;
		
		* 号 用于指定一个变量是指针
		* 号 也用于返回位于操作数所指定地址的变量的值


	所有实际数据类型，不管是整型、浮点型、字符型，还是其他的数据类型，对应< 指针的 值的 类型 >都是一样的，都是一个代表内存地址的长的十六进制数。
	不同数据类型的指针之间唯一的不同是，指针所指向的变量或常量的数据类型不同。

	*/


	//检查一个指针是否为空
	/*
	在大多数的操作系统上，程序不允许访问地址为 0 的内存，因为该内存是操作系统保留的。
	然而，内存地址 0 有特别重要的意义，它表明该指针不指向一个可访问的内存位置。但按照惯例，如果指针包含空值（零值），则假定它不指向任何东西。

	创建指针时，如果不知道指针指向什么，就默认赋值 0

	指针必须初始化，没有初始化的指针 称为  野指针（失控指针）  常见错误 （Segmentation Fault）

	free() 释放内存后 指针需要置为 NULL  不然指针还是指向free()的那块内存   悬空指针

	多级指针
	*/

	int *p1 = 0;

	if (p1)
	{
		printf("指针p不为空\n");
	}
	if (!p1)
	{
		printf("指针p为空\n");
	}


	/*
	指针是用数值表示的地址 可以进行算术运算   
	支持 ++  --  + -



	*/

	//八进制和十六进制表示（打印）
	int x = 100;
	printf("dec = %d, octal = %o, hex = %x\n", x, x, x);    //八进制 %o   十六进制 %x
	printf("dec = %d, octal = %#o, hex = %#x\n", x, x, x);  //会输出 0..   0x..

	// unsigned int （无符号整型） 能表示 比 int 更大的数

	// signed 关键字 强调使用有符号数的意图  
	// short == short int == signed short = signed short int

	//整数溢出
	//常见例子：在超过最大值时  unsigned int 从 0 开始    int从 -2147483648 开始
	int ii = 2147483647;
	unsigned int jj = 4294967295;
	printf("%d, %d, %d\n", ii, ii+1, ii+2);
	printf("%u, %u, %u\n", jj, jj + 1, jj + 2);

	getchar();


	/*测试大端小端*/
	int ival = 0x12345678;
	float fval = (float)ival;
	int *pval = &ival;
	show_int(ival);
	show_float(fval);
	show_pointer(pval);
	/*
	output:
	006FF80C 78			小端
	006FF80D 56
	006FF80E 34
	006FF80F 12

	006FF80C b4			小端
	006FF80D a2
	006FF80E 91
	006FF80F 4d

	006FF80C f8			小端
	006FF80D f8
	006FF80E 6f
	006FF80F 00
	*/
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
