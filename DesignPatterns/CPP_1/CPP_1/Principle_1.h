#pragma once

#include <iostream>
#include <ostream>
#include <string>
#include <memory>
#include <vector>
#include <fstream>

#include <initializer_list>
#include <cassert>
#include <tuple>

using std::string;
using std::unique_ptr;
using std::vector;
using std::ofstream;
using std::tuple;
using std::tie;
/*
设计原则：
SRP:	单一职责原则 Single Responsibility Principle
OCP:	开闭原则  Open Closed Principle
LSP:	里式替换原则  Liskov Substitution Principle
ISP:	接口隔离原则 Interface Segregation Principle
DIP:	依赖反转原则  Dependency Inversion Principle  (依赖倒置)
*/


/*
SRP: A class should have only one reason to change
可读性：类单一职责，接口中方法作用更明确，数据成员更少，相互之间关系更紧密
可维护性：代码单一职责，相互独立，修改独立，容易理解，减少出错
可重用性：不需要或很少修改即可复用



注意不要过度设计而生成大量相互关联的类（碎片化）

gather together the things that change for the same reasons. Separate those things that change for different reasons.
需要将因为相同原因而改变的东西放一起，把因为不同原因而改变的东西分开

构造函数不应该有超过5-6个以上的参数

SRP不是将类拆分成尽量小为基础，而是找到聚集和拆分之间的平衡点
*/
namespace SRP_Example_1
{

	class BaseSaveFile						//add for template
	{
	public:
		virtual ~BaseSaveFile() {}
		virtual void AddEntries(const string& entry) = 0;
		vector<string> GetEntries() const { return m_entries; }

	protected:
		vector<string> m_entries;
	};

	class Journal 
					: public BaseSaveFile	//add for template
	{
		string m_title;
		//vector<string> m_entries;			//remove for template

	public:
		explicit Journal(const string& title) : m_title{ title } {}
		void AddEntries(const string& entry)	
								override	//add for template
		{
			static uint32_t count = 1;
			m_entries.push_back(std::to_string(count++) + ": " + entry);
		}
		//auto GetEntries() const { return m_entries; }	//remove for template

		//根据SRP，存储功能从日记类中移出去，日记类只有内容格式相关的数据和操作
		//void save(const string &filename)
		//{
		//	ofstream ofs(filename);
		//	for (auto &s : jo.GetEntries()) {
		//		ofs << s << std::endl;
		//	}
		//}
	};

	//存储相关的应该放到单独的一个实体中
	//可以将其模板化，以接收更多对象
	struct SavingManager {
		static void Save(const Journal& jo, const string &filename) {
			ofstream ofs(filename);
			for (auto &s : jo.GetEntries()) {
				ofs << s << std::endl;
			}
		}

		template<typename T = BaseSaveFile>
		static void Save(const T& t, const string& filename) {
			ofstream ofs(filename);
			for (auto &s : t.GetEntries()) {
				ofs << s << std::endl;
			}
		}
	};

	void Test1()
	{
		Journal journal("Dear HZY");
		journal.AddEntries("I love you so much");
		journal.AddEntries("I miss you today");
		//journal.save("diary.txt")
		SavingManager::Save(journal, "diary.txt");
		SavingManager::Save<Journal>(journal, "diary2.txt");
	}
}


/*
classes should be open for extension, closed for modification
软件实体（模块、类、方法等）应该"对扩展开放，对修改关闭"

可扩展性：需求变动时，通过添加新的代码来扩展模块功能，而不是通过更改已经工作的旧代码
可维护性：接口引入了额外的抽象级别，从而实现松耦合；接口实现是相互独立的，不需要共享任何代码
灵活性：开闭原则也适用于插件和中间件架构，基础或核心模块，可以通过一个公共网关接口插入新的特性和功能

结论：
类不可能永远关闭，总有不可预见的更改需要修改类，然后，当变更是可以预见的，使用OCP抽象出接口，为未来扩展做好准备
*/
namespace OCP_Example_1
{
	enum class EColor {RED, GREEN, BLUE};
	enum class ESize {SMALL, MEDIUM, LARGE};
	struct Product {
		string name;
		EColor color;
		ESize size;
	};

	using Items = vector<Product*>;
	//当需要添加新的过滤函数时，需要修改ProductFilter
	struct ProductFilter {
		static Items ByColor(Items items, const EColor color) {
			Items result;
			for (auto &i : items) {
				if (i->color == color) {
					result.push_back(i);
				}
			}
			return result;
		}
		static Items BySize(Items items, const ESize size) {
			Items result;
			for (auto &i : items) {
				if (i->size == size) {
					result.push_back(i);
				}
			}
			return result;
		}
		static Items ByColorAndSize(Items items, const EColor color, const ESize size) {
			Items result;
			for (auto &i : items) {
				if (i->size == size && i->color == color) {
					result.push_back(i);
				}
			}
			return result;
		}
	};


	//抽象接口
	template<typename T>
	struct Specification {
		virtual ~Specification() = default;
		virtual bool IsSatisfied(T *item) const = 0;
	};

	struct ColorSpecification : Specification<Product> {
		EColor color;
		ColorSpecification(EColor c) : color(c) {}
		bool IsSatisfied(Product *item) const override { return item->color == color; }
	};

	struct SizeSpecification : Specification<Product> {
		ESize size;
		SizeSpecification(ESize s) : size(s) {}
		bool IsSatisfied(Product *item) const override { return item->size == size; }
	};

	template<typename T>
	struct AndSpecification : Specification<T> {
		const Specification<T> &first;
		const Specification<T> &second;

		AndSpecification(const Specification<T> &f, const Specification<T> &s)
			:first(f), second(s) {}

		bool IsSatisfied(T *item) const override 
		{
			return first.IsSatisfied(item) && second.IsSatisfied(item);
		}
	};

	template<typename T>
	AndSpecification<T> operator && (const Specification<T> &first, const Specification<T> &second) {
		return { first, second };
	}


	//TODO...  使用可变参数实现多个Specification判断
	template<typename T>
	struct MultiSpecification : Specification<T>
	{
		vector<Specification<T>> sList;

		template<class... Spec>
		MultiSpecification(const Spec &... specs)
		{
			//std::initializer_list<Specification<T>> list{ specs... };
			//for (auto s : list) {
			//	sList.push_back(s);
			//}
		}

		bool IsSatisfied(T* item) const override
		{
			bool satisfied = false;
			//for (Specification<T> s : sList) {
			//	satisfied = s.IsSatisfied(item);
			//}
			return satisfied;
		}
	};


	template<typename T>
	struct Filter {
		virtual vector<T *> ByFilter(vector<T *> items, const Specification<T> &spec) = 0;
	};

	struct BetterFilter : Filter<Product> {
		vector<Product *> ByFilter(vector<Product *> items, const Specification<Product> &spec)
		{
			vector<Product *> result;
			for (auto &p : items) {
				if (spec.IsSatisfied(p))
					result.push_back(p);
			}
			return result;
		}
	};

	void Test1()
	{
		const Items all{
			new Product{"Apple", EColor::GREEN, ESize::SMALL},
			new Product{"Tree", EColor::GREEN, ESize::LARGE},
			new Product{"House", EColor::BLUE, ESize::LARGE},
		};

		for (auto &p : ProductFilter::ByColor(all, EColor::GREEN))
			std::cout << p->name << " is green" << std::endl;
		for (auto &p : ProductFilter::ByColorAndSize(all, EColor::GREEN, ESize::LARGE))
			std::cout << p->name << " is green and large" << std::endl;

		BetterFilter bf;
		for (auto &x : bf.ByFilter(all, ColorSpecification(EColor::GREEN)))
			std::cout << x->name << " is green" << std::endl;

		auto greenObjs = ColorSpecification(EColor::GREEN);
		auto largeObjs = SizeSpecification(ESize::LARGE);
		for (auto &x : bf.ByFilter(all, greenObjs && largeObjs))
			std::cout << x->name << " is green and large" << std::endl;

		auto spec = ColorSpecification(EColor::BLUE) && SizeSpecification(ESize::LARGE);
		for (auto &x : bf.ByFilter(all, spec))
			std::cout << x->name << " is blue and large" << std::endl;

		for (auto &x : 
			bf.ByFilter(all, 
			MultiSpecification<Product>(ColorSpecification(EColor::BLUE), SizeSpecification(ESize::SMALL))
			))
			std::cout << x->name << " is blue and large" << std::endl;
	}
}

/*
Subtypes must be substitutable for their base types without altering the correctness of the program
子类必须可以替换它们的父类，而不会改变程序的正确性

兼容性：保证多个版本和补丁之间二进制兼容性，客户端代码不受影响
类型安全：继承时能保证类型安全，确保正确的继承
可维护性：代码可复用性，代码能正常执行抽象

如果一个对象始终与所继承的对象具有“is - substitute（代替） - for”关系，那么它可以被设计为从另一个对象继承。


*/
namespace LSP_Example_1
{
	//不推荐在Rectangle中添加Square的判断接口，做特殊处理
	struct Rectangle {
		Rectangle(const uint32_t width, const uint32_t height) : m_width(width), m_height(height) {}

		uint32_t GetWidth() const { return m_width; }
		uint32_t GetHeight() const { return m_height; }

		virtual void SetWidth(const uint32_t width) { this->m_width = width; }
		virtual void SetHeight(const uint32_t height) { this->m_height = height; }

		uint32_t Area() const { return m_width * m_height; }

	protected:
		uint32_t m_width;
		uint32_t m_height;
	};

	//正方形在数学中可以认为是一种特殊的矩形，但是不支持继承关系
	struct Square : Rectangle {
		Square(uint32_t size) : Rectangle(size, size) {}
		void SetWidth(const uint32_t width) override { this->m_width = m_height = width; }
		void SetHeight(const uint32_t height) override { this->m_height = m_width = height; }
	};

	void Test1(Rectangle &r)
	{
		uint32_t w = r.GetWidth();
		r.SetHeight(10);
		assert((w * 10) == r.Area()); //Square不支持！
	}

	//建议实现：
	struct Shape {
		virtual uint32_t Area() const = 0;
	};

	struct Rectangle2 : Shape {
		Rectangle2(const uint32_t width, const uint32_t height) : m_width(width), m_height(height){}
		uint32_t GetWidth() const { return m_width; }
		uint32_t GetHeight() const { return m_height; }
		virtual void SetWidth(const uint32_t width) { this->m_width = width; }
		virtual void SetHeight(const uint32_t height) { this->m_height = height; }

		uint32_t Area() const { return m_width * m_height; }

	private:
		uint32_t m_width;
		uint32_t m_height;
	};

	struct Square2 : Shape {
		Square2(uint32_t size) : m_size(size) {}
		void SetSize(const uint32_t size) { this->m_size = size; }
		uint32_t Area() const override { return m_size * m_size; }

	private:
		uint32_t m_size;
	};

	struct ShapeFactory {
		static Shape* CreateRectangle(uint32_t width, uint32_t height)
		{
			Shape* s = new Rectangle2(width, height);
			return s;
		}
		static Shape* CreateSquare(uint32_t size) 
		{
			Shape* s = new Square2(size);
			return s;
		}
	};
}


/*
Clients should not be forced to depend on interfaces that they do not use.
不应该强迫客户端依赖它们不使用的接口

编译加快：接口中多依赖了不需要的方法时，当方法签名改变时，需要重新编译所有派生类（对于编译型语言，如C++，尤其需要遵守ISP）
可重用性：fat interfaces（带有额外无用方法的接口） 会导致类之间不经意的耦合，从而降低了可复用性
可维护性：通过避免不必要的依赖，系统变得更容易理解，测试更方便，能更快重构
		  别人读代码时能更快更明显地了解类的作用，而不需要从god-interface中推断包含了哪些其他接口

Do I need all the methods on this interface I’m using?

@注意：
ISP和接口大小无关，而是类是否使用它们所依赖的接口中的方法

ISP尽量作为标识代码接口是否健康的指示器，而不是用来作为设计代码的准则
*/
namespace ISP_Example_1
{
	struct Document;
	struct IMachine {
		virtual void Print(Document &doc) = 0;
		virtual void Fax(Document &doc) = 0;
		virtual void Scan(Document &doc) = 0;
	};

	struct MultiFunctionPrinter : IMachine {	//OK
		void Print(Document &doc) override {}
		void Fax(Document &doc) override {}
		void Scan(Document &doc) override {}
	};

	struct FFax : IMachine {
		void Print(Document &doc) override { //Blank 
		}
		void Fax(Document &doc) override { //Blank 
		}
		void Scan(Document &doc) override { //do sth
		}
	};


	//尝试根据角色将接口划分为多个接口
	struct IPrinter {
		virtual void Print(Document &doc) = 0;
	};
	struct IScanner {
		virtual void Scan(Document &doc) = 0;
	};

	struct Printer : IPrinter {
		void Print(Document &doc) override {}
	};

	struct Scanner : IScanner {
		void Scan(Document &doc) override {}
	};

	//@注意：只有接口（只有方法的类）才能使用多继承（为了避免菱形继承）
	struct IMachine2 : IPrinter, IScanner {};
	struct Machine : IMachine2 {				//提供了将抽象组合在一起的灵活性，并且不需要带不必要的东西提供实现
		IPrinter& printer;
		IScanner& scanner;

		Machine(IPrinter &p, IScanner& s) : printer(p), scanner(s) {}
		void Print(Document &doc) override { printer.Print(doc); }
		void Scan(Document &doc) override { scanner.Scan(doc); }
	};
}


/*
High-level modules should not depend on low-level modules. Both should depend on abstractions.
Abstractions should not depend on details. Details should depend on abstractions.
高级模块不应该依赖于低级模块，两者都应该依赖于抽象
抽象不应该依赖于细节，细节应该依赖于抽象

高级模块：描述本质上更抽象的操作，包含更复杂的逻辑，这些模块在我们应用程序中协调低级模块
低级模块：描述更具体和独立的实现，侧重于应用程序的细节和更小的部分，这些模块在高级模块中使用

DIP 也称为 Coding To Interface.

实现DIP的第一步：先设计抽象接口，并在抽象基础上实现高级模块，而不用了解底层模块或其实现

所有低级模块或者子类 都遵循LSP原则，因为低级模块或子类都将通过抽象接口使用，而不是具体类接口


可重用性：减少代码间的耦合
可维护性：通过依赖于抽象而不是具体的实现，我们可以通过不改变项目中的高级模块来降低风险。


结论：
不要直接使用具体对象，除非你有充分的理由这样做。使用抽象。
DIP训练我们从行为的角度来考虑类，而不是构建或实现。
*/
namespace DIP_Example_1
{
	enum class Relationship {PARENT, CHILD, SIBLING};
	struct Person {
		string name;
	};
	struct Relationships {                //Low-level 
		vector<tuple<Person, Relationship, Person>> relations;
		void AddParentAndChild(const Person &parent, const Person &child) {
			relations.push_back({ parent, Relationship::PARENT, child });
			relations.push_back({ child, Relationship::CHILD, parent });
		}
	};

	//当Relationships中的vector改为其他容器时，许多地方都要改动
	struct Research {					//High-Level
		Research(const Relationships &relationships) {
			//for (auto &&[first, rel, second] : relationships.relations) { //C++17
			for(auto rel : relationships.relations) {
			  /*Person first = std::get<0>(rel);
				Relationship rel = std::get<1>(rel);
				Person second = std::get<2>(rel);*/
				if (std::get<0>(rel).name == "Jack" && std::get<1>(rel) == Relationship::PARENT) {
					std::cout << std::get<0>(rel).name << " has a child called " << std::get<2>(rel).name << std::endl;
				}
			}
		}
	};

	void Test1()
	{
		Person parent{ "Jack" };
		Person child1{ "Mark" };
		Person child2{ "Julia" };
		Relationships relationships;
		relationships.AddParentAndChild(parent, child1);
		relationships.AddParentAndChild(parent, child2);
		Research _(relationships);
	}


	struct RelationshipBrowser
	{
		virtual vector<Person> FindAllChildrenOf(const string &name) = 0;
	};

	struct Relationships2 : RelationshipBrowser {                //Low-level 
		vector<tuple<Person, Relationship, Person>> relations;
		void AddParentAndChild(const Person &parent, const Person &child) {
			relations.push_back({ parent, Relationship::PARENT, child });
			relations.push_back({ child, Relationship::CHILD, parent });
		}

		vector<Person> FindAllChildrenOf(const string &name) override {
			vector<Person> result;
			for (auto rel : relations) {
				/*Person first = std::get<0>(rel);
				  Relationship rel = std::get<1>(rel);
				  Person second = std::get<2>(rel);*/
				if (std::get<0>(rel).name == name && std::get<1>(rel) == Relationship::PARENT) {
					result.push_back(std::get<2>(rel));
				}
			}
			return result;
		}
	};

	struct Research2 {					//High-Level
		Research2(RelationshipBrowser &browser) {
			for (auto &child : browser.FindAllChildrenOf("Jack")) {
				std::cout << "Jack has a child called " << child.name << std::endl;
			}
		}
	};

	void Test2()
	{
		Person parent{ "Jack" };
		Person child1{ "Mark" };
		Person child2{ "Julia" };
		Relationships2 relationships;
		relationships.AddParentAndChild(parent, child1);
		relationships.AddParentAndChild(parent, child2);
		Research2 _(relationships);
	}
}