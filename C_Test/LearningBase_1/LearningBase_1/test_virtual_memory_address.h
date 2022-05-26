#pragma once

#include <stdio.h>
#include <assert.h>
#include <string.h>

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
			生命周期：从程序开始到结束为止



*/

/*


*/
#endif //TEST_VIRTUAL_MEMORY_ADDRESS