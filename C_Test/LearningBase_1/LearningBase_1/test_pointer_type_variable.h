#pragma once

#include <stdio.h>
#include <assert.h>
#include <string.h>

#ifndef TEST_POINTER_TYPE_VARIABLE
#define TEST_POINTER_TYPE_VARIABLE

void TestPointer_1()
{
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
}


/*
变量不一定按照声明的顺序保存在内存中

%p 接收的 是 void* 类型

void* 可以指向任何类型的指针类型


*/
void TestPointer_2()
{
	int a = 5;
	int b = 10;
	int* p_a = NULL;

	printf("&a .. %p \n", (void*)&a);
	printf("&b .. %p \n", (void*)&b);
	printf("&p_a .. %p \n", (void*)&p_a);

	p_a = &a;									//表达式含义为：p_a指向a
												//&a为a的地址，也称为指向a的指针
	printf("p_a .. %p \n", (void*)p_a);
	printf("&p_a .. %p \n", (void*)&p_a);
	printf("*p_a .. %d \n", *p_a);

	*p_a = 10;
	printf("a .. %d \n", a);
}


/*
指针是用数值表示的地址 可以进行算术运算
支持 ++  --  + -

对指针加1，地址增加该指针所指向的类型的长度
对指针加n，指针前进”该指针锁指向类型的长度“ x n
*/
void TestPointer_3()
{
	int a = 0;
	int* p_a = NULL;

	p_a = &a;
	printf("p_a .. %p \n", (void*)p_a);         // p_a .. 00D5F668

	p_a++;
	printf("p_a .. %p \n", (void*)p_a);			// p_a .. 00D5F66C			-- 相差 1个 4字节（int长度）

	printf("p_a .. %p \n", (void*)(p_a + 3));	// p_a .. 00D5F678			-- 相差 3个 4字节

	//int* p_b1 = 3;
	//int* p_b2 = 1;

	//printf("p_b1 .. %p \n", (void*)(p_b1));
	//printf("*p_b1 .. %p \n", *p_b1);
	//printf("&p_b1 .. %p \n", (void*)&p_b1);
	//printf("p_b2 .. %p \n", (void*)(p_b2));
	//printf("*p_b2 .. %p \n", *p_b2);
	//printf("&p_b2 .. %p \n", (void*)&p_b2);
}

/*
1.数组下标运算符[]与数组毫无关系
2.在表达式中，数组都会被解读成指向其初始元素的指针
3.p[i] 时 *(p + i) 的简便写法，[i]是语法糖 syntax sugar
4.数组声明中的[]和下标运算符[] 含义不同，表达式中的*和声明中的* 含义不同
5.*(p + i) 可以写成 *(i + p)
	p[i] 可以写成 i[p]
*/
void TestPointer_4()
{
	int array[5];
	int i;
	for (i = 0; i < 5; i++)
	{
		array[i] = i;
	}

	for (i = 0; i < 5; i++)
	{
		printf("%d \n", array[i]);
	}

	for (i = 0; i < 5; i++)
	{
		printf("&array[%d]  ... %p \n",i, (void*)&array[i]);
	}

	int* p = NULL;
	for (p = &array[0]; p != &array[5] ; p++)			//使用p++ 指针p会前进 sizeof(int)个字节
	{
		printf("%d \n", *p);							//在早期的c编译器中，使用指针运算 比数组下标更能遍历效率更快
														//*p多次出现，但只需要在循环结束时执行一次乘法和加法运算
														//而使用array[i]遍历，需要执行array + (i * sizeof(元素))个乘法和加法运算
	}

	p = &array[0];
	for (i = 0; i < 5; i++)
	{
		printf("%d \n", *(p + i));
	}

	p = array;											//c中，数组后无论有无[]，在表达式中，数组都会解读成指向数组初始元素的指针
	for (i = 0; i < 5; i++)
	{
		printf("%d \n", *(p + i));
	}
	for (i = 0; i < 5; i++)
	{
		printf("%d \n", *(i + p));						//*(p + i) == *(i + p)
	}

	p = array;											//c中，数组后无论有无[]，在表达式中，数组都会解读成指向数组初始元素的指针
	for (i = 0; i < 5; i++)
	{
		printf("%d \n", p[i]);							//p[i] 时 *(p + i) 的简便写法，此时p[i] 即 array[i]，array[i] 等价于 *(array + i)
	}
	for (i = 0; i < 5; i++)
	{
		printf("%d \n", i[p]);							//p[i]  == i[p]，不推荐
	}
}

//调用前需要提前定义，否则会报：“TestStrCpy” : “char * (char *, const char *)”与“int()”的间接寻址级别不同

//strcpy能把strSrc的内容复制到strDest，为什么还要char * 类型的返回值？
//为了实现链式表达式
char* TestStrCpy(char *dst, const char *src)
{
	assert((dst != NULL) && (src != NULL));
	char* address = dst;

	while ((*dst++ = *src++) != '\0')
		;

	return address;
}

void TestPointer_5()
{
	char* strDst = (char*)malloc(12);
	int len = strlen(TestStrCpy(strDst, "hello world"));
	printf("strDst len=%d, content=%s", len, strDst);
	free(strDst);
}

#endif //TEST_POINTER_TYPE_VARIABLE