#pragma once

/*
link: https://sg-first.gitbooks.io/cpp-template-tutorial/content/qian_yan.html
*/

class TestTemplate_1
{
public:
	template<typename T> 
	T Add(T a, T b)
	{
		return a + b;
	}
};


template <typename T>
class TestTemplate_2
{};

template <typename T> 
T TestTemplate_2_foo(TestTemplate_2<T> v);


float test_template_2_data[1024];

template <typename T>
T TestTemplate_2_GetValue(int i)
{
	return static_cast<T>(test_template_2_data[i]);
}

//float a = TestTemplate_2_GetValue(9); //error :  无法根据返回值推断类型，函数调用时，返回值被谁接受还不知道
//float a = TestTemplate_2_GetValue<float>(9); //ok


template <typename SrcT, typename DstT>
DstT c_style_cast(SrcT v)
{
	return (DstT)(v);
}

//int v = 0;
//float i = c_style_cast<float>(v);   //error:  推导模板参数类型有顺序要求
//float i = c_style_cast<int, float>(v); //ok

//ok
//调整模板参数顺序：需要主动指定的放在前面
//template <typename DstT, typename SrcT>
//DstT c_style_cast(SrcT v)
//{
//	return (DstT)(v);
//}



//整型模板参数

//int等整型数（包括bool） 也可以作为Template参数的“类型”
template <int V> 
class TestTemplate_3
{};

//可以定义常数
template<typename T, int Size> 
struct TestTemplate_4
{
	T data[Size];
};
//TestTemplate_4<int, 16> arr;


//因为模板的匹配是在编译的时候完成的，所以实例化模板的时候所使用的参数，也必须要在编译期就能确定


//负责
template <int I> class TestTemplate_5
{
public:
	void foo(int) {}


	template <int I> int Add(int a)
	{
		return a + I;
	}
};

template <uint8_t A, typename B, void* C> class TestTemplate_6 {};
template <bool, void (*A)()> class TestTemplate_7 {};
template < void (TestTemplate_5<3>::*A)(int) > class TestTemplate_8 {};

//template<float A> class TestTemplate_8 {};  //error: 模板参数只能是整数类型

//ok 
//void TestTemplate_FOO()
//{
//		TestTemplate_5<5> a;
//		TestTemplate_6<								  // 模板参数可以是一个无符号八位整数，可以是模板生成的类；可以是一个指针
//			7, TestTemplate_5<5>, nullptr
//		> b;
//		TestTemplate_7<false, &TestTemplate_FOO> c;   //模板参数可以是一个bool类型的常量，函数指针可以作为模板参数
//		TestTemplate_8< &TestTemplate_5<3>::foo > d;  //也可以是成员函数指针
//}



//模板 vs 宏
//1.模板可运算
//2.模板避免了不必要的代码执行路径






//模板匹配规则：模板是从最特殊到最一般形式进行匹配
//双阶段名称查找(two phase name lookup)

template <typename T>
struct TestTemplate_9 
{
};


//typename的使用场景
/*
struct A;
template <typename T> struct B;
template <typename T> struct X {
	typedef X<T> _A; // 编译器当然知道 X<T> 是一个类型。
	typedef X    _B; // X 等价于 X<T> 的缩写
	typedef T    _C; // T 不是一个类型还玩毛

	// ！！！注意我要变形了！！！
	class Y {
		typedef X<T>     _D;          // X 的内部，既然外部高枕无忧，内部更不用说了
		typedef X<T>::Y  _E;          // 嗯，这里也没问题，编译器知道Y就是当前的类型，
									  // 这里在VS2015上会有错，需要添加 typename，
									  // Clang 上顺利通过。
		typedef typename X<T*>::Y _F; // 这个居然要加 typename！
									  // 因为，X<T*>和X<T>不一样哦，
									  // 它可能会在实例化的时候被别的偏特化给抢过去实现了。
	};

	typedef A _G;                   // 嗯，没问题，A在外面声明啦
	typedef B<T> _H;                // B<T>也是一个类型
	typedef typename B<T>::type _I; // 嗯，因为不知道B<T>::type的信息，
									// 所以需要typename
	typedef B<int>::type _J;        // B<int> 不依赖模板参数，
									// 所以编译器直接就实例化（instantiate）了
									// 但是这个时候，B并没有被实现，所以就出错了
};
*/

template <typename T>
struct TestTemplate_10
{
	// X可以查找到原型；
    // X<T>是一个依赖性名称，模板定义阶段并不管X<T>是不是正确的。
	typedef TestTemplate_9<T> type1;  //ok


	// X可以查找到原型；
	// X<T>是一个依赖性名称，X<T>::MemberType也是一个依赖性名称；
	// 所以模板声明时也不会管X模板里面有没有MemberType这回事。
	//加上typename  表示 MemberType 为类型 而非成员变量  VS2017中不加编译会报错
	typedef typename TestTemplate_9<T>::MemberType type2;  //VS下不会报错
	//MSVC中 因为分析模板中成员时没有做任何事 
	//C++是个非常复杂的语言，以至于它的编译器，不可能通过  词法-语法-语义  多趟分析清晰分割。
	//MSVC将包括所有模板成员函数的语法/语义分析工作都挪到了第二个Phase，于是乎连带着语法分析都送进了第二个阶段

	// UnknownType 不是一个依赖性名称
	// 而且这个名字在当前作用域中不存在，所以直接报错。
	//typedef UnknownType type3; //error

	void foo()
	{
		TestTemplate_9<T> obj_1;
		typename TestTemplate_9<T>::MemberType obj_2;
	}
};


/*
模板定义中能够出现以下三类名称：

模板名称、或模板实现中所定义的名称；
和模板参数有关的名称；
模板定义所在的定义域内能看到的名称。
*/




//特化和偏特化

//变长模板类型，可变参数模板
template<typename... Ts> class tuple;

//error: 不定长类型列表 通常只能放在参数列表的最后
//template <typename... Ts, typename U> class X {};              // (1) error!
//template <typename... Ts>             class Y {};              // (2)
//template <typename... Ts, typename U> class Y<U, Ts...> {};    // (3)
//template <typename... Ts, typename U> class Y<Ts..., U> {};    // (4) error!


/*
ref: https://zhuanlan.zhihu.com/p/149405532
可变参数模板(variadic template)为一个接受可变数目参数的模板函数或模板类。
参数包(parameter packet)可变数目的参数。
模板参数包(template parameter packet)表示零个或多个模板参数。
函数参数包(function parameter packet)表示零个或多个函数参数。


用class...或typename...指出接下来的参数表示零个或多个类型的列表。
一个类型名后面跟一个省略号表示零个或多个给定类型的非类型参数的列表(可以是一个函数的实参列表).
	// 用class 也一样
	template <typename T, typename... Args>
	// 如果函数参数列表中一个参数的类型是一个模板参数包,
	// 则此参数也是一个函数参数包
	void func(const T& t, const Args&... rest);


编译器从函数实参推断模板参数类型.对可变参数模板,编译器还会推断包中的参数数目.
用  sizeof...()   可以获取  模板参数包的参数个数  和  函数参数包的参数个数.


使用initializer_list可以定义一个接受可变数目实参的函数,但这些参数必须具有同一类型。
*/

#include <string>
#include <ostream>
#include <sstream>
using std::string;
using std::ostream;
using std::ostringstream;
namespace Template_VariableParam_1
{
	template <typename T, typename... Args>
	void Foo(const T &t, const Args &... rest)
	{
		std::cout << sizeof...(Args) << " " << sizeof...(rest)  << std::endl;
	}

	void Test1()
	{
		int i = 1;
		double d = 3.14;
		string s = "hello";
		Foo(i, s, 12, d);
		Foo(s, 34, "world");
		Foo(d, s);
		Foo("hello world");
		Foo(d, "abc", "efg");
	}


	template<typename T>
	ostream &print(ostream& os, const T &t)
	{
		return os << t << std::endl;
	}

	//cosnt Args& ... rest 
	//rest 类型都是 const Type&
	template<typename T, typename ... Args>
	ostream &print(ostream& os, const T& t, const Args& ... rest)
	{
		os << t << ",";
		return print(os, rest...);  //可变参数函数通常为递归调用，每一步处理最前面的一个实参，然后用剩余参数调用自身
									//最后一个参数不会调用这个可变版本的print（从输出结果的逗号推断）
	}

	void Test2()
	{
		print(std::cout, 12, 1.34, "hello");
		print(std::cout, 1.34, "hello");
		print(std::cout, "hello"); /*
								同时匹配非可变参数模板和可变参数模板时，非可变参数模板更特例化，选择调用非可变参数模板
								   */
		/*
		非可变参版本的声明必须在可变参函数之前,否则,可变参版本的print将会一直调用自身,直到耗尽参数包,用一个os参数去调用print(无法通过编译).
		对一个没有<<运算符的类型的对象调用print，编译都无法通过,从模板实例化出的对应版本的函数终究会用这个参数调用os << myclass.
		对于print(std::cout, "hello"),两个版本的print都提供同样好的参数匹配, 但是非可变参数模板比可变参数模板更加特例化, 因此编译器选择非可变参数版本(16.3 P615).
		*/
	}

	/*
	参数包扩展：上例中rest为参数包，可以获取其大小，也可以扩展它

	扩展包就是对包中的每一个元素都应用一个指定的模式,并得到展开后的逗号分隔的列表, 这里的模式通常为一些类型限定修饰符.通过在模式右边放一个省略号(...)触发扩展操作。
	上例中 return print(os, rest...);
	*/

	template <typename T>
	string debugRep(const T &t)
	{
		ostringstream oss;
		oss << t;
		return oss.str();
	}

	template<typename ... Args>
	ostream &errorMsg(ostream &os, const Args &... rest)
	{
		return print(os, debugRep(rest)...); //将参数包rest用debugRep扩展
											 //扩展结果，debugRep(fname), debugRep(404)...

		//return print(os, debugRep(rest...));  //compile error: 没有匹配的参数列表
												//展开参数包: debugRep(a1, a2, a3...an) 没有匹配的参数列表
	}

	void Test3()
	{
		errorMsg(std::cerr, "fname", 404, 13.14, "helloworld", false);
	}


	/*
	转发参数包
		用可变参数实例化一个类,通常一个类有多个不同参数列表的构造函数,
		这就要求我们必须将不同数量不同类型的参数原封不动地传递给这个类的构造函数。

	保持类型信息：
		保持实参中的类型信息——将emplace_back()的参数定义为模板类型参数的右值引用。
		将这些参数传递给construct时，用forward保持实参原始类型。

	示例：stl中容器的emplace_back()
	*/
}

#include <iostream>
namespace Template_VariableParam_2
{
	//fold expression 

	/*ref:
			https://en.cppreference.com/w/cpp/language/parameter_pack
			
			https://en.cppreference.com/w/cpp/language/fold
			https://www.fluentcpp.com/2021/03/12/cpp-fold-expressions/
			https://www.modernescpp.com/index.php/fold-expressions
			https://www.foonathan.net/2020/05/fold-tricks/
			https://subscription.packtpub.com/book/application-development/9781787120495/1/ch01lvl1sec16/implementing-handy-helper-functions-with-fold-expressions
	*/


	//variadic constructor

	struct Foo
	{
		Foo(const Foo &)
		{
			std::cout << "copy ctor\n";
		}

		template<typename ... Args>
		Foo(Args&&... args)
		{
			std::cout << "variadic ctor\n";
		}
	};

	void Test1()
	{
		Foo f1;
		Foo f2(f1);
		/*
		output:
		variadic ctor
		variadic ctor

		两次都是调用可变参数构造
		*/
	}

	struct Foo2
	{
		Foo2() {
			std::cout << "default ctor\n";
		}
		Foo2(const Foo2& x)
		{
			std::cout << "copy ctor\n";
		}

		template<typename T>
		Foo2(T&& x)
		{
			std::cout << "template ctor\n";
		}
	};

	struct Foo3
	{
		Foo3() {
			std::cout << "default ctor\n";
		}
		Foo3(const Foo3& x)
		{
			std::cout << "copy ctor\n";
		}

		Foo3(Foo3& x)
		{
			std::cout << "copy ctor 2\n";
		}

		template<typename T>
		Foo3(T&& x)
		{
			std::cout << "template ctor\n";
		}
	};

	void Test2()
	{
		Foo2 f1;
		Foo2 f2(f1);
		/*
		output:
		default ctor
		template ctor
		
		调用了template 构造
		需要调用拷贝构造，需要将f1转换为const 
		*/
		const Foo2 f3;
		Foo2 f4(f3);
		/*
		output:
		default ctor
		copy ctor
		*/

		Foo3 f5;
		Foo3 f6(f5);
		/*
		output:
		default ctor
		copy ctor 2
		*/
	}
}

//TODO
#include <type_traits>
#include <initializer_list>
#include <iostream>
namespace Template_VariableParam_3
{
	/*
	namespace detail {
    enum class enabler {};
	}
	template<size_t dim>
	class templateClass
	{
	public:
		template<class... DimArgs, typename std::enable_if<sizeof...(DimArgs)==dim, detail::enabler>::type...>
		templateClass(DimArgs... dimensions) {
		// Use integers passed to the constructor, one for each dimension
		}
	};

	templateClass<2> temp(5, 2);     // works
	templateClass<3> temp(5, 2, 4);  // works
	templateClass<1> temp(5,2);      // would give compile-time error
	*/

	/*
	namespace Detail {
		enum class Enabler {};
	}

	template <bool Condition>
	using EnableIf = typename std::enable_if_t<Condition, Detail::Enabler>::type;

	template<size_t meshDim>
	class Mesh
	{
	public:
		template<class... DimArgs, EnableIf<sizeof...(DimArgs)==meshDim>...>
		Mesh(DimArgs... dimensions) {
			std::initializer_list<int> list{dimensions...};
			for (int i : list) { 
				std::cout << "mesh index = " << i << std::endl;
			}
		}
	};

	void Test1()
	{
		Mesh<2> mesh(3, 4);
	}*/
}



//TODO
namespace Template_VariableParam_4
{
	template<typename T, int ID = 0>
	class StrongAlias {
		T value;
	public:
		// doesn't work
		template<typename... Args, typename = std::enable_if_t<std::is_constructible_v<T, Args...>>>
		StrongAlias(Args&&... args) noexcept(std::is_nothrow_constructible_v<T, Args...>)
			: value(std::forward<Args>(args)...) {}
	};


	//只需将 std::enable_if_t 更改为非类型模板参数:
	template<typename T, int ID = 0>
	class StrongAlias2 {
		T value;
	public:
		template<typename... Args, std::enable_if_t<std::is_constructible_v<T, Args...>, int> = 0>
		StrongAlias2(Args&&... args) noexcept(noexcept(T(std::declval<Args>()...)))
			: value(std::forward<Args>(args)...) {}
	};
}