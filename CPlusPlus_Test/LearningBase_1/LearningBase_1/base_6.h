#pragma once

#include <iostream>
using std::cout;
using std::endl;

/*
link: https://blog.csdn.net/silly1195056983/article/details/111995979

转移语义的定义:
右值引用是用来支持转移语义的。
转移语义可以将资源 ( 堆，系统对象等 ) 从一个对象转移到另一个对象，这样能够减少不必要的临时对象的创建、拷贝以及销毁，能够大幅度提高 C++ 应用程序的性能。
临时对象的维护 ( 创建和销毁 ) 对性能有严重影响。

转移语义是和拷贝语义相对的，可以类比文件的剪切与拷贝，当我们将文件从一个目录拷贝到另一个目录时，速度比剪切慢很多。

通过转移语义，临时对象中的资源能够转移其它的对象里。

在现有的 C++ 机制中，我们可以定义拷贝构造函数和赋值函数。
要实现转移语义，需要定义转移构造函数，还可以定义转移赋值操作符。
对于右值的拷贝和赋值会调用转移构造函数和转移赋值操作符。
如果转移构造函数和转移拷贝操作符没有定义，那么就遵循现有的机制，拷贝构造函数和赋值操作符会被调用。

普通的函数和操作符也可以利用右值引用操作符实现转移语义。
*/

class TestLRValue
{
public:
	void Test_1();

	void ProcessValue(int &i);
	void ProcessValue(int &&i);
	void ForwardValue(int &&i);



	//精准传递：

	template <typename T>
	void ProcessValue_T(T& type) {
		std::cout << "LValue processed: " << type << std::endl;
	}
	//每一个参数必须重载两种类型，T& 和 const T&
	template <typename T> 
	void ForwardValue_T(const T& val) {
		ProcessValue_T(val);
	}
	template <typename T> 
	void ForwardValue_T(T& val) {
		ProcessValue_T(val);
	}

	//同时支持了上面两个重载的ForwardValue_T
	// T&& 的推导规则为: 右值实参为右值引用，左值实参仍然为左值引用
	//精确传递的大前提是，它必须是一个泛型函数，而如果是一个普通的函数，那么必须定义同时左值传参和右值传参，否则就会报错。
	template <typename T> 
	void ForwardValue_TR(T&& val) {
		ProcessValue_T(val);
	}
};


class MyString
{
public:
	MyString() {
		m_data = nullptr;
		m_len = 0;
	}

	MyString(const char *cstr) {
		m_len = strlen(cstr);
		_init_data(cstr);
	}

	MyString(const MyString& str) {
		std::cout << "Copy Constructor is called! source: " << str.m_data << std::endl;
		m_len = str.m_len;
		_init_data(str.m_data);	}

	MyString& operator= (const MyString& str) {
		std::cout << "Copy Assignment is called! source: " << str.m_data << std::endl;
		if (this != &str) {
			m_len = str.m_len;
			_init_data(str.m_data);
		}
		return *this;
	}

	virtual ~MyString() {
		if (m_data) {
			free(m_data);
		}
	}


	//转移构造
	/*
	参数（右值）的符号必须是右值引用符号，即“&&”。

	参数（右值）不可以是常量，因为我们需要修改右值。

	参数（右值）的资源链接和标记必须修改。否则，右值的析构函数就会释放资源。转移到新对象的资源也就无效了。


	右值传参本身就是按引用传参的一个子集
	调用普通的右值传参的函数，并不会导致原来的对象被转移，它只是一种语法兼容，它解决了右值不能按引用传参的语法问题
	*/
	MyString(MyString&& str) {
		std::cout << "Move Constructor is called! source: " << str.m_data << std::endl;
		m_len = str.m_len;
		m_data = str.m_data;
		str.m_len = 0;
		str.m_data = nullptr;
	}

	//转移赋值
	MyString& operator=(MyString&& str) {
		std::cout << "Move Assignment is called! source: " << str.m_data << std::endl;
		if (this != &str) {
			m_len = str.m_len;
			m_data = str.m_data;
			str.m_len = 0;
			str.m_data = nullptr;
		}
		return *this;
	}

private:
	void _init_data(const char *cstr) {
		m_data = new char[m_len + 1];
		memcpy(m_data, cstr, m_len);
		m_data[m_len] = '\0';
	}

private:
	char *m_data;
	size_t m_len;
};