#pragma once

#include <stdio.h>
#include <assert.h>
#include <string.h>
#include <stdlib.h>

#ifndef TEST_VIRTUAL_MEMORY_ADDRESS
#define TEST_VIRTUAL_MEMORY_ADDRESS

int a;


void TestVirtualAddress_1()
{
	char buf[256];
	printf("& a .. %p \n", (void*)&a);

	printf("input initial value 1 \n");

	//获取用户输入整数值 1(推荐！！！  fgets() 和 sscanf() 组合使用，但fgets()输入字符串超过指定长度，多余部分也会残留在流中)
	fgets(buf, sizeof(buf), stdin);
	//sscanf(buf, "%d", &a);
	sscanf_s(buf, "%d", &a);		//运行两个进程，&a的地址显示为同一个
									/*
									对于应用程序的每个进程都会分配独立的虚拟地址空间。
									这与 C 语言无关，
									而是操作系统和 CPU 协同工作的结果。

									操作系统 负责将物理内存分配给虚拟地址空间
									*/
	for (;;) {
		printf("a .. %d \n", a);
		/*
			getchar()等待输入，每次enter，a增加1
		*/
		getchar();
		a++;
	}
}


void TestVirtualAddress_2()
{
	printf("input initial value 2 \n");

	//获取用户输入整数值 2
	//scanf("%d", &a);
	scanf_s("%d", &a);				
									/*
									存在问题： 第一次输入一个值后回车，会输出两个数值

									scanf() 不是以行为单位来解读输入数据，
									而是以连续输入字符的流来解读（换行符也视为一个字符）

									如输入 20 后 回车， 即20<换行>，其中换行符会残留在流中，循环中getchar()会读取到残留的字符
									*/
	
	/*
	while (scanf_s("%d", &a) != 1)
	{
		printf("输入错误，请再次输入"); //输入错误时，无限循环
	}
	*/

	/*
		fflush() 是用于输出流的，不能用于输入流。
		在 C 语言标准中，fflush() 用于输入流的行为是未定义的。

		未定义：
		未指定：
		实现定义：
	*/
	char buf[256];
	fgets(buf, sizeof(buf), stdin);
	fflush(stdin);

	for (;;) {
		printf("a .. %d \n", a);					
		/*
			getchar()等待输入，每次enter，a增加1
		*/
		getchar();
		a++;
	}
}

/*
操作系统通过虚拟内存方式分配物理内存
	操作系统也会对每块内存区域设置“只读”或者“可读写”等属性，
	表示“这块区域是只读的”或者“这块区域是可读写的”

进程间共享物理内存：
	程序的执行代码等通常是禁止写入的，所以有时需要与其他进程共享物理内存（禁止写入的内存，常用于共享）
	通常通过共享库来共享程序的一部分，在写入前，同时可以共享数据内存空间

内存交换：
	当内存不足时，操作系统将物理内存中当前未被引用的部分撤回硬盘，需要再此引用时，再从硬盘写回到内存

虚拟内存的作用：
	由于应用程序不直接使用物理内存地址，操作系统才可以任意地对内存空间进行分配
	现代应用程序中的内存地址为虚拟内存地址空间
*/



/*

*/
void TestVirtualAddress_3()
{
	static int a;

}
/*C语言变量分类：
1. 命名空间限制：作用域（scope）(代码块内) +  链接（linkage）(static extern)
2. 存储期（storage duration）


全局变量：函数外部定义的变量，多文件均可引用
static变量： 将全局变量的作用域限定在当前源文件内，对于其他源文件不可见（static函数同理）
局部变量：只能在其声明所在的代码块{...} 中被引用，（从C99开始，可以在代码中间位置声明局部变量）
			为局部变量添加static，可以保证离开作用域后，局部变量不被释放



存储期：
静态存储期（static storage duration）:
		全局变量、文件内的static变量、带static限定的局部变量
			生命周期：从程序开始到结束为止，一直存在于内存的同一地址上

自动存储期（auto storage duration）
		自动变量：不带static限定的局部变量
			生命周期：程序进入其所在代码块（一般是程序进入函数时）分配内存，离开该代码块时内存空间被释放
				实现特性：使用栈机制

C语言使用malloc()动态分配内存，使用free()释放内存
*/

/*
关键字：存储类说明符

static 函数内部：用于指定变量存储期
	   函数外部：控制变量作用域

extern  使在别处定义的外部变量在此处可见
auto	默认，无需指定
register 用于给编译器提供优化提示，使变量会被优先分配给寄存器（无法使用&取内存地址），但目前编译器优化程度高，不需要主动使用该说明符
typedef  用于类型名称 而不是变量名

*/


/*
指向函数的指针 和 指向int或char的指针不同，是不能转换为void*
printf() 无法输出指向函数的指针，（%p 可以输出 void*）
*/
int global_variable;
static int file_static_variable;
void func1(void)
{
	int func1_variable;
	static int local_static_variable;

	printf("&func1_variable .. %p\n", (void*)&func1_variable);
	printf("&local_static_variable .. %p\n", (void*)&local_static_variable);
}
void func2(void)
{
	int func2_variable;

	printf("&func2_variable .. %p\n", (void*)&func2_variable);
}

void TestVirtualAddress_4()
{
	int *p;

	//尝试输出函数指针
	printf("func1 .. %p \n", (void*)func1);
	printf("func2 .. %p \n", (void*)func2);

	printf("string literal .. %p \n", (void*)"jz");

	printf("&global_variable .. %p \n", (void*)&global_variable);

	printf("&file_static_variable .. %p \n", (void*)&file_static_variable);

	func1();
	func2();

	p = (int*)malloc(sizeof(int));
	printf("malloc address .. %p \n", (void*)p);
}

/*
  内存区域1： 指向函数的指针 和 字符串字面量 放置接近的内存区域

  内存区域2： 静态变量（文件内的static变量、static局部变量、全局变量）

  内存区域3： 通过malloc()分配的内存空间

  内存区域4： 函数内的自动变量，分配了相同的内存地址
*/


/*
只读内存区： 函数主体（1. 保证执行程序只读，操作系统禁止改写函数，保证了代码可读性；
					   2. 多个相同程序可以共享物理地址上存放的程序，内存不足时，可以直接舍弃程序块，不需要切换到内存交换区？？？）
			 字符串字面量
					 （从gcc 4.0开始，修改字符串字面量的行为设为未定义）

*/

/*
指向函数的指针：
			表达式中的函数 会被解释为 指向函数的指针，即将 func() 写成 func

			指向函数的指针类型*  根据对象函数的返回值和参数而定

			如： int func(double d);
				保存指向函数func的指针的指针变量声明： 
				 int (*func_p)(double);
*/

void func3(double d)
{
	printf("func3: d + 1.0 = %f \n", d + 1.0);
}
void func4(double d)
{
	printf("func4: d + 2.0 = %f \n", d + 2.0);
}
void TestVirtualAddress_5()
{
	void(*func_p)(double);

	func_p = func3;
	func_p(1.0);			//将函数调用符() 作用于指向函数的指针 func_p
							//如 printf("hello. \n");
						    //表达式中 将printf解释为指向函数的指针
							//同理： 数组中 将array[i] 中的 array 看作指向数组初始元素的指针

	func_p = func4;
	func_p(1.0);
}



/*
静态变量
		从程序启动到结束为止 一直存在的变量
		在（虚拟）内存地址空间上，静态变量占有固定区域
		包括 全局变量、文件内static变量、以及带static限定的局部变量
		由于作用域不同、编译和链接时会具有不同含义，但是在运行时作用相似
*/

/*
分割编译和链接
		
*/


#endif //TEST_VIRTUAL_MEMORY_ADDRESS