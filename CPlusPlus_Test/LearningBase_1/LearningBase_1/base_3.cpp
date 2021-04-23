#include "base_3.h"

#include <malloc.h>
#include <string>

//结构体类型定义  使用
//LinkListA LLA;  //LL定义为 struct LNode类型的指针变量
//LinkListB LLB;

//LNode_C 和 D 定义为 sturct LNode类型的变量
//LNodeC LNode_C;
//LNodeD LNode_D;

void Struct_Test_1::Test1()
{
	//定义无名结构
	struct
	{
		char name[8];
		int age;
		char sex[4];
	}student;


	//定义结构数组
	struct
	{
		char name[9];
		int age;
		char sex[5];
	}students[40] = { "xiaoming", 28, "male" };
	//结构体初始化 推荐方法
	//通过集合符号对结构体变量进行初始化。在集合符号中，值以有序形式，用逗号隔开，左右用花括号括起来。
	//成员字段按指定顺序查值并初始化
	//注意：需要在定义时加 ！！！
	//注意：字符串 末尾有一个字节： \0


	printf("%s", student.name);
}

void Struct_Test_1::Test2()
{
	//定义结构指针，使用前需要初始化！！！
	struct Student
	{
		char name[8];
		int age;
		char sex[4];
		char addr[40];
	}*student;

	//使用前初始化
	student = (struct Student*)malloc(sizeof(struct Student));
	printf("%s", student->addr);
}

void Struct_Test_1::Test3()
{
	//嵌套结构
	struct addr
	{
		char city[20];
		unsigned long zipcode;
		char tel[14];
	};

	struct Student
	{
		char name[8];
		int age;
		char sex[4];
		struct addr address;
	}student;

	//student需要定义
	//printf("%s", student.address.zipcode);
}

void Struct_Test_1::Test4()
{
	StructTest1 *st1;
	st1 = new StructTest1(3);      //必须初始化结构体指针
	st1->val = 4;
	(*st1).val = 5;					//两种获取方法
	printf("%d", st1->val);
}

void Struct_Test_1::Test5()
{
	//结构体赋值 （复制）
	//将结构体变量赋值给另一个结构体变量的工作于正常赋值一样，将各个成员变量从一个结构体复制到另一个结构体
	struct data
	{
		int i;
		char c;
		int j;
		int arr[2];
	};
	struct datawptr
	{
		int i;
		char *c;
	};

	struct datawptr dptr1;
	struct datawptr dptr2;
	struct data svar1;
	struct data svar2;
	svar1.c = 'a';
	svar1.i = 1;
	svar1.j = 2;
	svar1.arr[0] = 10;
	svar1.arr[1] = 20;
	svar2 = svar1;
	printf("Value of second variable \n");
	printf("Member c=%c\n", svar2.c);
	printf("Member c=%c\n", svar2.c);
	printf("Member c=%c\n", svar2.c);
	printf("Member c=%c\n", svar2.c);
	printf("Member c=%c\n", svar2.c);
	dptr1.i = 10;
	dptr1.c = (char*)malloc(sizeof(char));
	*(dptr1.c) = 'c';
	dptr2.c = (char*)malloc(sizeof(char));
	dptr2 = dptr1;
	printf("int member=%d\n", dptr2.i);
	printf("char ptr member =%c\n", *(dptr2.c));

	/*
	也可使用库函数memcpy()赋值操作实现相同效果
	但是当data结构体包含指针类型成员时要小心，因为赋值操作符不仅仅复制值，也复制指针变量的值（即结构体指针指向其他变量的地址）
	之后，当被赋值变量修改了该地址的存储值，会导致最终修改了源变量地址的存储值
	*/

	dptr1.i = 10;
	dptr1.c = (char*)malloc(sizeof(char));
	*(dptr1.c) = 'c';
	dptr2.c = (char*)malloc(sizeof(char));
	memcpy(&dptr2, &dptr1, sizeof(struct datawptr));

	printf("int member value of 2nd variable =%d\n", dptr2.i);
	printf("char ptr member of 2nd variable =%c\n", *(dptr2.c));
	printf("value of char ptr in 1st variable=%p\n", dptr1.c);
	printf("value of char ptr in 2nd variable=%p\n", dptr2.c);
	printf("changing value of 2nd memeber in 2nd variable (dptr2)\n");
	*(dptr2.c) = 'a';
	printf("value of char ptr of 2nd variable =%c and 1st variable =%c\n", *(dptr2.c), *(dptr1.c));

	/*
	如果我们试图分别释放两个变量的内存，会产生段错误（分段错误），
	因为通过第一个变量第一次调用free会再次释放内存，
	通过第二个变量第二次调用free会导致段错误，原因为试图第二次释放相同的内存空间
	*/
}

void Struct_Test_1::Test6()
{
	struct STest6 *st = nullptr;
	Test6_2(st);
	Test6_3(&st);
}

void Struct_Test_1::Test6_1(STest6 t)
{
}

void Struct_Test_1::Test6_2(STest6 * t)
{
	t = (struct STest6*)malloc(sizeof(struct STest6));
	t->data = 9;
}

/*
以传递指针变量给函数企图修改结构体的做法是错误的，这种方法在函数调用中很有效，
但是上述情况中传递的是指针变量的值。所以调用Test6_2()后，变量n1仍然指向NULL。
需要修改如下
*/

void Struct_Test_1::Test6_3(STest6 ** t)
{
	*t = (struct STest6*)malloc(sizeof(struct STest6));
	(*t)->data = 10;
}

void Struct_Test_1::Test7()
{
	struct STest7_3 *img = new STest7_3();
	struct STest7_1 *sign = Test7_1(img);
	struct STest7_2 *idval = Test7_2(img);
}

STest7_1 * Struct_Test_1::Test7_1(STest7_3 * d)
{
	struct STest7_1* sig = (struct STest7_1*)d;
	return sig;
}

STest7_2 * Struct_Test_1::Test7_2(STest7_3 * d)
{
	struct STest7_2* idv = (struct STest7_2*)d;
	return idv;
}


