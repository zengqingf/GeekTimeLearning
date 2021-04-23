#include "base_2.h"
//using namespace std;
using std::cout;
using std::cin;
using std::endl;

void Base_2::swap_by_pointer(int * a, int * b)
{
	int temp = *a;
	*a = *b;
	*b = temp;
}

//形参 int指针p 传递来的 一个地址 （一个指针变量的本身对象的地址，值传递）
//值传递：将一个实参的一个拷贝传递给形参
void Base_2::pass_by_value_pointer(int * p)
{
	if (p != nullptr) {
		cout << p << " " << *p << endl;
	}
	int a = 1;
	p = &a;
	cout << p << " " << *p << endl;
}

void Base_2::pass_by_pointer(int * p)
{
	*p = *p - 1;
	cout << p << " " << *p << endl;
}

void Base_2::pass_by_reference(int & a)
{
	cout << &a << " " << a << endl;
}

void Base_2::pass_by_reference_pointer(int *& p)
{
	int a = 1;
	p = &a;
	cout << "formal parameter : p "<< p << " " << *p << endl;
}

//* *p 表示 指针 p 本身的内存地址
void Base_2::pass_by_pointer_pointer(int * *p)
{
	int a = 1;
	*p = &a; //注意：*p为左值 表示为 形参 指针 p'  （存在的 就是名称不叫 p' 而是 *p）      即 形参p ==>（指向） 形参 *p (p') ==> &a 
	//所以*p为右值时，即为a的地址
	//**p为右值时，即为a的值
	//p为指针本身所占的内存地址
	cout << "formal parameter : p " << p <<" " << &p  << " " << *p << " " << &(*p) << " " << **p << endl;
}

void Base_2::test_swap_by_pointer()
{
	int a = 1, b = 2;
	swap_by_pointer(&a, &b);
	cout << a << " " << b << endl;
	cin.get();
}

void Base_2::test_pass_by_value_pointer()
{
	int *p = nullptr;
	pass_by_value_pointer(p);
	if (p == nullptr) {
		cout << "pointer p is null" << endl;  //实参 指针p 和 pass_by_value_pointer函数的形参p 不是同一个变量
	}

	int a = 2;
	pass_by_value_pointer(&a);
	cout << a << " " << &a << endl;
	pass_by_pointer(&a);
	cout << a << " " << &a << endl;

//	system("pause");
	cin.get();	
}

void Base_2::test_pass_by_reference()
{
	int a = 1;
	cout << &a << " " << a << endl;
	pass_by_reference(a);
	cin.get();
}

void Base_2::test_pass_by_reference_pointer()
{
	int *p = nullptr;
	pass_by_reference_pointer(p);
	if (p != nullptr) {
		cout << "actual parameter : p is not null" << endl;
		//实参 指针p所指的地址    指针p所指的地址对应的值
		cout << p << " " << *p << endl;
		//注意错误点：
		//调用pass_by_reference_pointer后，*p指向的是pass_by_reference_pointer中的变量a，
		//但是变量a在pass_by_reference_pointer调用结束后就被回收了，*p无法访问它，所以结果错了
	}
	cin.get();
}

void Base_2::test_pass_by_pointer_pointer()
{
	int *p = nullptr;
	pass_by_pointer_pointer(&p);  //传递实参指针p的地址 ， 即 形参p ==> (形参 *p (p') == 实参 p） ==> &a 
	if (p != nullptr) {
		cout << "actual parameter : p is not null" << endl;
		cout << p << " " << *p << endl;
		//注意错误点：
		//调用pass_by_reference_pointer后，*p指向的是pass_by_reference_pointer中的变量a，
		//但是变量a在pass_by_reference_pointer调用结束后就被回收了，*p无法访问它，所以结果错了
	}
	cin.get();
}

string Base_2::getStr() const
{
	// TODO: 在此处插入 return 语句
	return test_str;
}

string & Base_2::getRStr()
{
	// TODO: 在此处插入 return 语句
	return this->test_str;
}
