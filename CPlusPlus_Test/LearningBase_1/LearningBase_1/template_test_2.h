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

//变长模板类型
template<typename... Ts> class tuple;

//error: 不定长类型列表 通常只能放在参数列表的最后
//template <typename... Ts, typename U> class X {};              // (1) error!
//template <typename... Ts>             class Y {};              // (2)
//template <typename... Ts, typename U> class Y<U, Ts...> {};    // (3)
//template <typename... Ts, typename U> class Y<Ts..., U> {};    // (4) error!