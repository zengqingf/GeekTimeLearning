#pragma once

/*
https://www.cnblogs.com/noticeable/p/8576100.html
结构体：

结构是由基本数据类型构成的、并用一个标识符来命名的各种变量的组合。结构中可以使用不同的数据类型。
(整型、浮点型、字符型、指针型、无值型)

结构是一种新的数据类型， 同样可以有结构数组和结构指针。

结构成员


定义：
1.结构体类型

2.结构体变量

<---------------------------------------------------------
vs. class
结构体和类的区别（唯一区别）
（1）class 中默认的成员访问权限是 private 的，而 struct 中则是 public 的。　　
（2）从 class 继承默认是 private 继承，而从 struct 继承默认是 public 继承。  (struct也支持继承)

1) 对类及对象进行建模时，使用class。
2) 对数据结构进行建模时，使用struct。
---------------------------------------------------------->

3.结构体存储和填充

结构体的大小与内存对齐

一般来说结构体变量的大小等于它包含所有变量的总大小

但是 结构体的大小不是结构体元素单纯相加就行的，

因为我们现在主流的计算机使用的都是 32Bit 字长的 CPU，对这类型的 CPU 取 4 个字节的数要比取一个字节要高效，也更方便。
所以在结构体中每个成员的首地址都是4的整数倍的话，取数据元素是就会相对更高效，这就是内存对齐的由来。
每个特定平台上的编译器都有自己的默认"对齐系数"(也叫对齐模数)。
程序员可以通过预编译命令 #pragma pack(n)，n=1,2,4,8,16 来改变这一系数，其中的n就是你要指定的"对齐系数"。　　
规则：
1、数据成员对齐规则：结构(struct)(或联合(union))的数据成员，第一个数据成员放在 offset 为 0 的地方，
	以后每个数据成员的对齐按照 #pragma pack指定的数值和这个数据成员自身长度中，比较小的那个进行。　　
2、结构(或联合)的整体对齐规则：在数据成员完成各自对齐之后，结构(或联合)本身也要进行对齐，
	对齐将按照< span class="marked">#pragma pack 指定的数值和结构(或联合)最大数据成员长度中，比较小的那个进行。　　
3、结合1、2颗推断：当#pragma pack的n值等于或超过所有数据成员长度的时候，这个n值的大小将不产生任何效果。


加载到内存时结构体类型变量与数组非常相似
所有成员字段占用连续的内存位置


4.结构体赋值 - 复制结构体

5.结构体传值，传指针

6.结构体指针 强制转换

7.class中定义内部struct
*/


/*
C++ 中不要再使用 typedef struct{...}xxx 来定义结构体了，这是传统C的做法
*/


//结构体类型定义1
typedef struct LNodeA {	
	int data; //数据域
	struct LNodeA *next; //指针域					//自引用结构体
} *LinkListA;

//结构体类型定义2
struct LNodeB {
	int data;
	struct LNodeB *next;
};
typedef struct LNodeB *LinkListB;  //定义结构指针,（指向结构中第一个成员的首地址）


typedef struct _LNodeBB {

}*LNodeBB;
typedef LNodeBB *LinkListBB;


//结构体类型变量定义1
struct LNodeC {
	int data;
	struct LNodeC *next;                  
}LNode_C;

//结构体类型变量定义2
struct LNodeD {
	int data;
	struct LNodeD *next;
};
struct LNodeD LNode_D;


//定义结构体类型
typedef struct LNodeE {
	int data;
	struct LNodeE *next;
}LNODEE;


struct STest6
{
	int data;
};

struct STest7_1
{
	char sign;
	char version;
};

struct STest7_2
{
	char id;
	char platform;
};

struct STest7_3
{
	struct STest7_1 st7_1;
	struct STest7_2 st7_2;
	char data[100];
};

#include <stdio.h>
#include <stdlib.h>

struct StructTest1
{
	StructTest1 *left;
	StructTest1 *right;
	int val;
	StructTest1() {}
	StructTest1(int val) :left(nullptr), right(nullptr), val(val) {}
};

class Struct_Test_1
{
public:
	//也可以是protected: private:
	struct InnerStruct1
	{
		double x;
		double y;
	};	

public:
	void Test1();
	void Test2();
	void Test3();
	void Test4();
	void Test5();
	void Test6();
	void Test6_1(struct STest6 t);
	void Test6_2(struct STest6 *t);
	void Test6_3(struct STest6 **t);
	void Test7();
	struct STest7_1 * Test7_1(struct STest7_3 *d);
	struct STest7_2 * Test7_2(struct STest7_3 *d);
};