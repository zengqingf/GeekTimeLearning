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


/*
字符串：
在C中 类型是char[]
在C++中 类型是 const char[]   ==>（支持从数组衰减指针）  const char*

*/
void Base_2::test_char_with_const()
{
	//char *c1 = "a";   //错误 const char* 类型的值不能用于初始化 char*
	const char *c1 = "a";
	std::cout << *c1 << std::endl;
	std::cout << c1 << std::endl;
	c1 = "aa";			//没有修改 *c1的值   但是将c5重新指向"aa"
	std::cout << *c1 << std::endl;
	std::cout << c1 << std::endl;      //output: aa

	//C风格
	char c2[] = "b";				//包括null
	char c3[] = {"c"};				//不包括null
	char c4[] = { 'd', '\0' };		//包括null

	char const *c5 = "e";
	std::cout << *c5 << std::endl;
	std::cout << c5 << std::endl;
	c5 = "ee";    // 没有修改 *c5的值   但是将c5重新指向"ee"
	std::cout << *c5 << std::endl;
	std::cout << c5 << std::endl;	//output: ee

	char const *c6 = "f";
	//*c6 = "ff";   不能修改字符串的值
	std::cout << *c6 << std::endl;
}


//https://stackoverflow.com/questions/30409350/convert-const-char-to-const-wchar-t
wstring Base_2::char_to_wchar(const string& str)
{
	using namespace std;
	const ctype<wchar_t>& ctdacet = use_facet<ctype<wchar_t>>(wstm.getloc());
	for (size_t i = 0; i < str.size(); ++i)
		wstm << ctdacet.widen(str[i]);
	return wstm.str();
}

//https://stackoverflow.com/questions/30409350/convert-const-char-to-const-wchar-t
string Base_2::wchar_to_char(const wstring& wstr)
{
	using namespace std;
	// Incorrect code from the link
	// const ctype<char>& ctfacet = use_facet<ctype<char>>(stm.getloc());
	const ctype<wchar_t>& ctfacet = use_facet<ctype<wchar_t>>(stm.getloc());
	for (size_t i = 0; i < wstr.size(); ++i)
		stm << ctfacet.narrow(wstr[i], 0);
	return stm.str();
}

void Base_2::test_char_wchar()
{
	{
		const char* cstr = "abcdefghijkl";
		const wchar_t* wcstr = char_to_wchar(cstr).c_str();
		std::wcout << wcstr << L'\n';
	}
	{
		const wchar_t* wcstr = L"mnopqrstuvwxyz";
		const char* cstr = wchar_to_char(wcstr).c_str();
		std::cout << cstr << '\n';
	}

	{
		wchar_t wbuf[100];
		std::mbstowcs(wbuf, "/*...*/", 99);
		std::wcout << wbuf << std::endl;
	}
	{
		const char* sz = "/*...*/";
		std::vector<wchar_t> vec;
		size_t len = strlen(sz);
		vec.resize(len + 1);
		mbstowcs(&vec[0], sz, len);
		const wchar_t* wsz = &vec[0];
		std::wcout << wsz << std::endl;
	}

	{
		std::ostringstream oss;
		oss << "One hundred and one: " << 101;
		std::string s = oss.str();
		std::cout << s << '\n';
	}
}

void Base_2::test_ptr_need_check_null(Base_2 *b2ptr_1, Base_2 *b2ptr_2)
{
	if (nullptr != b2ptr_1)
	{
		//delete指针前是否需要判空
		if (nullptr != b2ptr_2)
		{
			//需要提前释放掉内存，否则下一步会导致内存泄露
			delete b2ptr_2;
		}
		b2ptr_2 = b2ptr_1;

		delete b2ptr_1;
		b2ptr_1 = nullptr;
	}
}
