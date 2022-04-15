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

	while ((*dst++ = *src++) != '\0')						//直接修改传入的形参（作为局部变量使用，不好，调试时无法看到最初的值）
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


#include <ctype.h>			//define: isalnum() 返回真的连续字符，否则为空字符
//将数组作为函数参数传递

/*
作为函数形参声明时，以下等同
int func(int *a)
int func(int a[])			--是语法糖
int func(int a[10000])		--是语法糖
*/

/*
C语言不进行数组边界检查

*/

//从英语文本文件中将单词一个个读取
//int get_word(char *buf, int buf_size, FILE *fp)				//char buf[] 只有在声明函数形参时，才能将数组写成 指针形式
//int get_word(char buf[], int buf_size, FILE *fp)
int get_word(char buf[1000], int buf_size, FILE *fp)		   //编译器会把int func(int a[]) 解读为 int func(int *a)， 写成int func(int a[10000])会无视内部值，int a[] 作为函数形参时也是一种语法糖
{
	int len;
	int ch;

	while ((ch = getc(fp)) != EOF && !isalnum(ch))			//跳过空白字符
		;

	if (ch == EOF)
	{
		return EOF;
	}

	len = 0;												//ch存放单词的首字母
	do {
		buf[len] = ch;
		len++;
		if (len > buf_size)
		{
			fprintf(stderr, "word too long. \n");
			exit(1);
		}
	} while ((ch = getc(fp)) != EOF && isalnum(ch));

	buf[len] = '\0';
	return len;
}

void TestPointer_6()
{
	char buf[256];
	while (get_word(buf, 256, stdin) != EOF)
	{
		printf("<<%s>> \n", buf);
	}
}

/*
《征服C指针 第二版》

C 数组没有边界检查
对超出下标范围的内存执行写入操作时，会发生内存损坏；
	可能在程序执行早期就报 Segmentation fault(段错误)或 exit(XXX) 已停止了工作；
	也可能相邻变量内容被破坏，但程序仍继续运行，直到很久后发生错误，会造成巨大影响

数组边界检查即使很小程度上影响效率，但是很有必要在编码时就进行数组边界检查
	不要期望于编译器在编译期间执行
原因：引用数组内容时，使用a[i]语法糖，等同于*(a + i)；
	  将数组作为参数传递给函数时，传递的是指向数组初始元素的指针，此时不会传递数组长度
	  需要人为主动传递，编译器并不知道此数组长度
	  所以需要在编码时进行数组边界检查

即使将指针封装成结构体传递，附带指针本身可取值返回，但是会丢失非调试模式下编译的库与指针的兼容性

总的来说，在现阶段实际可供使用的编译器中，几乎没有能够进行数组边界检查的。
不过，如果是解释器的运行环境，似乎可以进行数组边界检查。		
*/

/*
《征服C指针 第二版》

C99可变长数组(VLA, Varable Length Array)
	惯用名：使用malloc() 分配的数组称为 动态数组


*/
void TestPointer_7()
{
	/* C99 支持 
	int size1, size2, size3;
	printf("input 3 int value: \n");
	scanf("%d%d%d", &size1, &size2, &size3);

	//可变长数组声明
	int array1[size1];
	int array2[size2][size3];

	//对可变长数组进行初始化赋值
	int i;
	for (i = 0; i < size1; i++)
	{
		array1[i] = i;
	}
	int j;
	for (i = 0; i < size2; i++) {
		for (int j = 0; j < size3; j++)
		{
			array2[i][j] = i * size3 + j;
		}
	}

	//键盘输入 3 4 5
	printf("sizeof(array1) .. %zd \n", sizeof(array1));		//12
	printf("sizeof(array2) .. %zd \n", sizeof(array2));		//80

	*/

	//上述VLA中 数组长度在运行时决定
	//一直到ANSI C为止，sizeof返回值都由编译时决定，而在ISO C99中，可以在运行时决定
	//VLA目前只用于非static的局部变量，对于全局变量不可用
	//C99中，无法通过VLA使结构体变长，但可以使用柔性数组成员
	//C11中 VLA降级为可选功能，__STDC_NO_VLA__ 宏将无法使用VLA
}

#endif //TEST_POINTER_TYPE_VARIABLE