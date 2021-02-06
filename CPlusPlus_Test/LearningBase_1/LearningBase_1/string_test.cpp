#ifndef __STRING_TEST__
#define __STRING_TEST__

class String
{
public:
	String(const char* cstr = 0);			//接收一个指针

	//Big Three 三个特殊函数
	String(const String& str);				//拷贝构造：自定义，接收一个自身类型的String引用
	String& operator=(const String& str);   //拷贝赋值：自定义，赋值一个自身类型的String引用
	~String();								//析构函数   触发时机：1.对象离开作用域

	inline char* get_c_str() const { return m_data; }   //inline关键字可以省略 最好加 函数够简单 所以编译器应该会标记为inline
														//因为不会改变m_data 这个指针 所以 const 不能忘记加

private:
	char* m_data;                 //字符串内部数据 存一个指针   指向一个内存空间
								  //而不是存一个数组 放一个数组存字符。。。
};


/*
C / C++
字符串表示：
指针指向一个内存地址的头部 末尾有\0这个结束符号

语言 字符串长度表示：
1. 指针指向内存地址 末尾有\0 （ 不一定是0吧, C/C++ 是 \0 ）这个结束符号  可以计算出长度  (如 C/C++)
2. 指针指向内存地址 前部有一个长度的值  （如，pascal）

*/
inline 
String::String(const char* cstr = 0)
{
	//动态分配内存，一般类里有指针，一般需要进行动态分配
	if (cstr) {
		m_data = new char[strlen(cstr) + 1];		//+1 表示还有一个结束符号 \0   new出来的是一块m_data指针指向的内存空间， 长度 = 传入的字符串长度 + 1
		strcpy(m_data, cstr);
	}
	else {
		m_data = new char[1];
		*m_data = '\0';								//字符表示的结束符号
	}
}

inline 
String::~String()
{
	//因为存在动态分配的内存，为了避免内存泄露，需要在析构函数里清理掉这块分配的内存
	delete[] m_data;								//delete vs delete[]      https://www.runoob.com/note/15971
													//delete ptr -- 代表用来释放内存，且只用来释放ptr指向的内存。
													//delete[] rg -- 用来释放rg指向的内存，！！还逐一调用数组中每个对象的 destructor！！
}


//拷贝构造
inline 
String::String(const String& str)
{
	m_data = new char[ strlen(str.m_data) + 1 ];			//直接拿另一个object的private 数据，因为对象之间互为友元
	strcpy(m_data, str.m_data);
}


//拷贝赋值 （copy assignment operator）
inline 
String& String::operator+=(const String& str)
{
	if (this == &str)										//检测自我赋值 （self assignment）
		return *this;										//因为是成员函数 所以会有this pointer传入    
															//str是传入的引用 即 对象本身， 所以通过取地址& 获取对象地址

	delete[] m_data;										//删除目标被赋值对象现有的 
	m_data = new char[ strlen(str.m_data) + 1 ];			//创建一份足够大的空间 和赋值对象长度一样 并 加上 末尾标记
	strcpy(m_data, str.m_data);								//拷贝赋值对象到被赋值对象上
	return *this;
}


/*
1. 类带指针，类中的拷贝操作需要自定义，不能使用编译器内置的（拷贝构造、拷贝赋值）
2. 指针离开作用域，需要删除它
例：
作用域 scope
{
	String s1();								//离开作用域时，会调用析构函数
	String s2("hello");							//离开作用域时，会调用析构函数

	String* p = new String("hello");
	delete p;									//离开作用域时 需要删除指针p    离开作用域时，会调用析构函数
}

3.class with pointer members (带指针成员的类) 必须有 copy ctor (拷贝构造) 和 copy op= (拷贝赋值)
如果不自定义 copy ctor 和 copy op=  则会使用编译器提供的拷贝和赋值操作

1）可能出现内存泄露
例：
String a("hello");  //hello\0
String b("world");	//world\0

使用default copy ctor 或者 default copy op= 
b = a;  //a和b都指向 hello\0		b作为别名alias					不建议多个指针指向同一个对象，修改一个，会影响其他      
		//world\0这块内存块没有指针指向 泄露！
		//属于 浅拷贝   编译器默认提供的方式


4. 深拷贝 vs. 浅拷贝
深拷贝：创建一份新的内存空间，将蓝本拷贝过去
浅拷贝：不创建新的内存空间，只是拷贝指针
例：
String s1("hello");
String s2(s1);
//String s2 = s1;  //创建s2 把s1赋值给s2  不同于拷贝赋值


5. 拷贝赋值 必须检查 自我赋值的情况 
1）提高效率
2）因为拷贝赋值 第一步就是删除目标本身 所以如果是自我赋值 就无法进行第二步骤了
例：
{
	String s1("hello");
	String s2(s1);
	s2 = s1;
}





*/


#endif // !__STRING_TEST__
