#pragma once
#ifndef _CHAPTER_11_H_
#define _CHAPTER_11_H_

#include <iostream>
#include <string>
#include <cassert>
#include <regex>

using std::string;

using namespace std::literals::string_literals; //打开命名空间 使用 字面量后缀“s” 表示为string

/*
建议只使用string   
涉及到 unicode 编码转换时 尽量不使用c++

*/
class Chapter_11
{
public:

	void TestString()
	{
		string str = "hello";
		assert(str.length() == 5);
		assert(str < "world");
		assert(str.substr(0, 1) == "h");
		assert(str[1] == "e");
		assert(str.find("1") == string::npos);
		assert(str + "," == "hello,");

		//string有类似容器的接口
		//但是string 不同于容器 字符间是强关系，顺序不能随意调换，通常作为整体处理
		//c++中处理字符串需要看作不可变的实体
		//如果需要用到存储字符的容器 考虑  vector<char>
		/*str.size();
		str.begin();
		str.end();
		str.push_back();
		...
		*/
	}

	//为了避免与用户自定义字面量的冲突，后缀“s”不能直接使用，必须用 using 打开名字空间才行
	void TestStr2()
	{
#if __cplusplus >= 201402
		auto str = "std string"s;		// 后缀s，表示是标准字符串，直接类型推导
		assert("time"s.size() == 4);   //标准字符串可以直接调用成员函数
#endif
	}

	//“原始字符串”（Raw string literal）表示形式，
	//比原来的引号多了一个大写字母 R 和一对圆括号
	//避免转义
	void TestStr3()
	{
		auto str = R"(hello:world)";   // 表示原始字符串 hello:world
		auto str1 = R"(char""'')";     // 原样输出: char""''
		auto str2 = R"(\r\n\t\")";     // 原样输出：\r\n\t\"
		auto str3 = R"(\\\$)";         // 原样输出：\\\$
		auto str4 = "\\\\\\$";         // 转义后输出：\\\$

		//特别界定符（delimiter）：在圆括号两边添加最多16个字符
		auto str5 = R"==(R"(xxx)")==";   //原样输出：R"(xxx)"
	}


	//字符串转换函数
	//不同于C的  atoi()  atol()
	void TestStr4()
	{
		//std::atoi("const char *");
		assert(std::stoi("42") == 42);
		assert(std::stol("245") == 245L);
		assert(std::stoll("245") == 245LL);
		assert(std::stod("2.0") == 2.0);

		assert(std::to_string(1993) == "1993");
	}

	//一般情况下，字符串传递只需要const string&
	//如果不得不需要字符串拷贝时（例：使用C字符串，获取子串时）
	void TestStr5()
	{
#if __cplusplus >= 201703
		std::string_view
#endif
	}

	/*
	C++正则匹配算法
	regex_match()：完全匹配一个字符串；
	regex_search()：在字符串里查找一个正则匹配；
	regex_replace()：正则查找再做替换。
	*/
	void TestStr6()
	{
		//函数式编程~~  使用auto 自动推导，隐藏具体类型信息，将来可以随时变化
		auto make_regex = [](const auto& txt)   //生产正则表达式
		{
			return std::regex(txt);
		};

		auto make_match = []()					//生产正则匹配结果
		{
			return std::smatch();
		};

		auto str = "hello:world"s;
		auto reg = make_regex(R"(^(\w+)\:(\w+)$)");   //用原始字符串定义正则表达式
		auto what = make_match();					  //准备获取匹配结果

		assert(regex_match(str, what, reg));          //正则匹配
		for (const auto& x : what) {				  //遍历匹配的子表达式
			std::cout << x << ',';
		}


		auto str1 = "think of future"s;
		auto reg1 = make_regex(R"((\w+)\s(\w+))");
		auto what1 = make_match();

		auto found = regex_search(str1, what1, reg1);  //正则查找
		assert(found);
		assert(!what.empty());
		assert(what[1] == "think");   //@注意：第一个子表达式 序号为 1
		assert(what[2] == "of");	  //第二个子表达式

			
		auto new_str = regex_replace(	//正则替换  返回新字符串	
			str1,						//原字符串不会改动
			make_regex(R"(\w+$)"),		//就地生成正则表达式对象
			"life"						//需要指定替换的文字
		);
		std::cout << new_str << std::endl;
	}

	/*
	正则表达式 成本：
	因为正则表达式只有在运行时才会处理
	检查语法、编译成正则对象 代价高

	建议：
	尽量不要反复创建正则对象，能重用就重用
	在使用循环的时候更要特别注意，一定要把正则提到循环体外
	*/
};


//字符串的视图，成本很低，内部只保存一个指针和长度，无论是拷贝，还是修改，都非常廉价。
class my_string_view final        // 简单的字符串视图类，示范实现
{
public:
	using this_type = my_string_view;     // 各种内部类型定义
	using string_type = std::string;
	using string_ref_type = const string_type&;

	using char_ptr_type = const char*;
	using size_type = size_t;
private:
	char_ptr_type ptr = nullptr;     // 字符串指针
	size_type len = 0;               // 字符串长度
public:
	my_string_view() = default;
	~my_string_view() = default;

	my_string_view(string_ref_type str) noexcept
		: ptr(str.data()), len(str.length())
	{}
public:
	char_ptr_type data() const     // 常函数，返回字符串指针
	{
		return ptr;
	}

	size_type size() const        // 常函数，返回字符串长度
	{
		return len;
	}
};

#endif  //_CHAPTER_11_H_