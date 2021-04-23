#pragma once

/*
指针 vs. 引用

一、指针和引用的定义和性质区别
1.指针是一个变量，只不过这个变量存储的是一个地址，指向内存的一个存储单元
	而引用跟原来的变量实质上是同一个东西，只不过是原变量的一个别名而已；不分配内存空间；
	对一个对象的引用，就是直接对这个对象的操作，对引用的赋值修改是修改引用所关联对象的值，而不是把引用和其他对象关联

2.可以有指针常量，但是没有引用常量（引用的指向本来就会改变）（ &ptr 引用底层是：T * const ptr 常量指针实现的，不能更改指向）
	常量引用（对常量的引用，即对const T的引用）  const T &ref   是对底层的const  引用本身不是对象 
	常量指针 const T *ptr  
	指针常量 T *const ptr

3.指针可以有多级，但是引用只能是一级（int **p；合法 而 int &&a是不合法的）

4.指针的值可以为空，但是引用的值不能为NULL，

  引用在定义的时候必须初始化 （指针建议初始化，但编译器不会告警）

5.指针的值在初始化后可以改变，即指向其它的存储单元，而引用在进行初始化后就不会再改变了

6."sizeof引用"得到的是所指向的变量(对象)的大小，而"sizeof指针"得到的是指针本身的大小

7.指针和引用的自增(++)运算意义不一样

8.底层实现：引用通过指针实现的

9.存在指针数组，不存在引用数组 (因为引用没有内存分配)  （int *a [3] = { &x, &y, &z};   //定义了一个有三个 整型指针变量 的 指针数组 a）




二、引用的使用场合
1. 引用型参数（函数形参只是实参的别名，形参和实参是同一个对象）
	不存在对象复制，避免了复制对象产生的开销
	修改形参的同时也会修改实参
	传递const 可以避免对实参进行修改 （不存在拷贝构造，提高执行效率；避免对象切割）

2.引用型返回值（从函数返回引用，需要保证在函数返回后，被引用的对象是有效的，不能返回函数体内的局部对象的引用（这个局部对象在离开作用域后会被析构掉））

	

*/

/*
二、指针和引用作为函数参数进行传递时的区别

1.用指针值传递参数，可以实现对实参进行改变的目的，是因为传递过来的是实参的地址，
	因此使用*a实际上是取存储实参的内存单元里的数据，即是对实参进行改变，因此可以达到目的。
  用指针地址传递参数
	形参为指针变量，将实参的地址传递给函数，可以在函数中修改实参的值，调用时为形参指针变量分配内存，结束时释放指针变量

2.用引用传递参数，传递给形参的不再是实参的拷贝，而是实参本身，引用给实参声明了一个别名，传递后对形参的修改就是对实参的修改
	引用传递在时间和空间上更高效
	不会为形参分配内存空间，函数结束后形参不会被释放

3.普通值传递参数 (传递给形参的是实参的一份拷贝，对形参的修改不是对实参的修改)
	会为形参重新分配内存空间，将实参的值拷贝给形参，形参的值不会影响实参的值，函数调用结束后形参被释放
*/
#include <iostream>
//using namespace std; //不要在头文件中 using namespace  会造成命名空间污染

#include <string>
using std::string;
class Base_2
{
private:
	void swap_by_pointer(int *a, int *b);
	void pass_by_value_pointer(int *p);
	void pass_by_pointer(int *p);
	void pass_by_reference(int &a);
	void pass_by_reference_pointer(int *&p);
	void pass_by_pointer_pointer(int **p);

	string test_str;

public:
	void test_swap_by_pointer();
	void test_pass_by_value_pointer();
	void test_pass_by_reference();
	void test_pass_by_reference_pointer();
	void test_pass_by_pointer_pointer();

	string getStr() const;
	string & getRStr();
};