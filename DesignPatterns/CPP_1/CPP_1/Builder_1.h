#pragma once

/*
Design Patterns: Builder Pattern in Modern C++ by
@IndianWestCoast
https://hackernoon.com/design-patterns-builder-pattern-in-modern-c-x1283uy3
*/

#include <iostream>
#include <string>
#include <vector>
#include <sstream>
#include <memory>

using std::string;
using std::ostream;

using std::vector;
using std::ostringstream;
using std::unique_ptr;

/*
To create/instantiate complex & complicated object piecewise & succinctly by providing an API in a separate entity.

构造一个复杂的对象时，可以使用构建器（建造者）
不使用复杂的构造函数成员或需要多参数的成员
通过分段（分步）构造一个复杂对象，在最后一步返回对象，构建过程是通用的，以便创建同一对象的不同表示


每当创建新对象时，需要设置许多参数，其中一些(或全部)是可选的。


在构建器模式中，代码行数至少增加了一倍。但是，从设计灵活性、构造函数的参数更少或没有参数和可读性更强的代码方面来看，这些努力是值得的。
Builder设计模式还有助于最小化构造函数中的参数数量&因此不需要将可选参数的null传递给构造函数。
在对象构建过程中，不需要太多复杂的逻辑就可以构建不可变对象。
将构造与对象表示分离，使得对象表示切片且精确。独立的构建器实体提供了创建和实例化不同对象表示的灵活性。


不同于抽象工厂模式
工厂批量生产对象，对象可以是继承层级结构中的任意对象（如Point, Point2D, Point3D...）
构建器常用于单个对象进行实例化，分段创建对象
*/

namespace BuilderExample_1
{
	class HtmlBuilder;
	class HtmlElement
	{
		string					m_name;
		string					m_text;
		vector<HtmlElement>		m_childs;
		constexpr static size_t m_indentSize = 4;

		HtmlElement() = default;
		HtmlElement(const string& name, const string& text) :
			m_name(name), m_text(text) {}

		friend class HtmlBuilder;
	
	public:
		string str(int32_t indent = 0) {
			ostringstream oss;
			oss << string(m_indentSize * indent, ' ') << "<" << m_name << ">" << std::endl;

			if (m_text.size()){
				oss << string(m_indentSize * (indent + 1), ' ') << m_text << std::endl;
			}

			for (auto& element : m_childs) {
				oss << element.str(indent + 1);
			}

			oss << string(m_indentSize * indent, ' ') << "</" << m_name << ">" << std::endl;
			return oss.str();
		}

		//编译器在HtmlBuilder实际声明之前，不能创建不完整类型的对象
		//因为编译器需要知道对象大小以便为其分配内存，所以只能先用指针来表示
		static unique_ptr<HtmlBuilder> build(string rootName) { return std::make_unique<HtmlBuilder>(rootName); }
	};

	class HtmlBuilder {
		HtmlElement m_root;
	public:
		HtmlBuilder(string rootName) { m_root.m_name = rootName; }
		HtmlBuilder* addChild(string childName, string childText) {
			m_root.m_childs.emplace_back(HtmlElement{ childName, childText });
			return this;
		}
		string str() { return m_root.str(); }
		operator HtmlElement() { return m_root; }
	};

	void Test1();
}

class PersonBuilder;
/*
除非需要，尽量不要为需要Builder分段创建的类 设计接口或抽象类
*/
class Person
{
private:
	std::string m_name, m_streetAddress, m_postcode, m_city;
	std::string m_companyName, m_position, m_annualIncome;

	//将构造函数私有，强制使用builder创建
	Person(std::string name) : m_name(name) {}

public:
	friend class PersonBuilder;
	friend std::ostream& operator <<(std::ostream& os, const Person& obj);

	//只暴露Create函数创建
	static PersonBuilder Create(std::string name);
};

//通过Builder分离实体Person
class PersonBuilder
{
private:
	Person person;

public:
	PersonBuilder(std::string name) : person(name) {}
	operator Person() const { return std::move(person); }

	PersonBuilder&	Lives();
	PersonBuilder&  At(std::string street_address);
	PersonBuilder&  WithPostcode(std::string post_code);
	PersonBuilder&  In(std::string city);
	PersonBuilder&  Works();
	PersonBuilder&  With(std::string company_name);
	PersonBuilder&  AsJob(std::string position);
	PersonBuilder&  Earning(std::string annual_income);
};

class TestBuilder1
{
public:
	static void Test1();
};