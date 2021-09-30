#pragma once

/*
To create a new object cheaply with the help of an already constructed or pre-initialized stored object.
通过克隆预配置对象快速创建对象

原型模式主要是改善反复多次类型创建对象
	

当对象实例化代价高昂时，原型是有用的，因此可以避免代价高昂的“从头创建”，并支持预初始化原型的廉价克隆。
与定义新类不同，原型通过对象组合定义新行为，并在实例化时为对象的数据成员指定值，从而提供了创建高度动态系统的灵活性。
特别在C++中，原型设计模式有助于在不知道对象类型的情况下创建对象的副本。
*/


#include <iostream>
#include <ostream>
#include <string>
#include <memory>

using std::string;
using std::unique_ptr;

namespace PrototypeExample_1
{
	struct Office {
		string m_street;
		string m_city;
		int32_t m_cubical;

		Office(string s, string c, int32_t n) :m_street(s), m_city(c), m_cubical(n) {}
		Office(const Office& rhs) {
			m_street = rhs.m_street;
			m_city = rhs.m_city;
			m_cubical = rhs.m_cubical;
			std::cout << "call Office copy ctor " << std::endl;
		}
	};

	struct Emplpoyee {
		string m_name;
		Office m_office;
		Emplpoyee(string n, Office o) :m_name(n), m_office(o) {}
	};

	void Test1()
	{
		Emplpoyee john{ "John E", Office{"123", "HZ", 123} }; //反复创建Office 如果有大量同Office的对象需要创建
		Emplpoyee jone{ "Jone D", Office{"124", "HZ", 124} };
		Emplpoyee jack{ "Jack F", Office{"124", "HZ", 125} };
	}


	struct EmployEE
	{
		string m_name;
		const Office* m_office;
		EmployEE(string n, Office* o) : m_name(n), m_office(o) {}
	};
	static Office hz123Office{"123", "HZ", 123};
	static Office hz124Office{ "124", "HZ", 124 };

	void Test2()
	{
		EmployEE john{ "John E", &hz123Office };
		EmployEE jone{ "Jone D", &hz124Office };
		EmployEE jack{ "Jack F", &hz124Office };
	}

	//当涉及指针和引用的间接赋值时，需要自定义实现拷贝赋值函数

	class EmployeeFactory;
	class Employee {
		string m_name;
		Office* m_office;

		Employee(string n, Office *o) :m_name(n), m_office(o) {}

		friend class EmployeeFactory;

	public:
		friend ostream& operator <<(ostream& os, const Employee& ee) {
			return os << ee.m_name << " work at " <<
				ee.m_office->m_street << " " << ee.m_office->m_city << " seat @" << ee.m_office->m_cubical;		
		}


		Employee(const Employee& rhs) : m_name(rhs.m_name),
			m_office{ 
			new Office{ *rhs.m_office } 
			}
		{
			std::cout << "call Employee copy ctor" << std::endl;
		}

		Employee& operator=(const Employee& rhs) {

			std::cout << "call Employee = ctor" << std::endl;

			if (this == &rhs)
				return *this;

			m_name = rhs.m_name;
			if (m_office != nullptr) {
				delete m_office;
				m_office = nullptr;
			}
			m_office = new Office{ *rhs.m_office };
			//m_office = new Office( *rhs.m_office ); //同上{} ()也是调用拷贝构造
			return *this;
		}
	};

	/*
	原型工厂中，在拷贝构造函数中创建实例，多数参数一旦创建定义后就不在改变，所以没有必要调用函数反复获取这些参数
	通过拷贝构造，可以把多数参数直接拷贝
	即主要的消耗不是具体的参数，而是创建（获取）这些参数值的函数（方法）
	*/
	class EmployeeFactory {
		static Employee main;
		static Employee aux;
		static unique_ptr<Employee> NewEmployee(string n, int32_t c, Employee &proto) {
			auto e = std::make_unique<Employee>(proto);
			e->m_name = n;
			e->m_office->m_cubical = c;
			return e;
		}

	public:
		static unique_ptr<Employee> NewMainOfficeEmployee(string name, int32_t cubical) {
			return NewEmployee(name, cubical, main);
		}
		static unique_ptr<Employee> NewAuxOfficeEmployee(string name, int32_t cubical) {
			return NewEmployee(name, cubical, aux);
		}

		static void ChangeMainOfficeToAux() {
			main = aux;
		}
	};

	Employee EmployeeFactory::main{ "", new Office("122", "HZ", 122) };
	Employee EmployeeFactory::aux{ "", new Office("124", "HZ", 124) };

	void Test3()
	{
		auto jane = EmployeeFactory::NewMainOfficeEmployee("Jane E", 123);
		auto jack = EmployeeFactory::NewAuxOfficeEmployee("Jack D", 125);
		std::cout << *jane << std::endl << *jack << std::endl;

		std::cout << "change main office to aux..." << std::endl;
		EmployeeFactory::ChangeMainOfficeToAux();
		std::cout << *jane << std::endl << *jack << std::endl;

		std::cout << "change office cubical..." << std::endl;
		auto janeNew = EmployeeFactory::NewMainOfficeEmployee("Jane E", 127);
		std::cout << *janeNew << std::endl << *jack << std::endl;
	}
}

namespace PrototypeExample_2
{
	struct animal {
		virtual ~animal() = default;

		//虚拟构造函数/复制构造函数技术允许在c++中多态创建和复制对象，方法是通过使用虚方法将创建和复制对象的行为委托给派生类。
		virtual std::unique_ptr<animal> create() = 0;		//虚拟构造函数
		virtual std::unique_ptr<animal> clone() = 0;		//虚拟拷贝构造函数
	};

	struct dog : animal {
		dog() {
			std::cout << "call dog ctor of animal " << std::endl;
		}
		std::unique_ptr<animal> create() { 
			std::cout << "call dog create" << std::endl;
			return std::make_unique<dog>();
		}
		std::unique_ptr<animal> clone() { 
			std::cout << "call dog clone" << std::endl;
			return std::make_unique<dog>(*this); 
		}
	};

	struct cat : animal {
		cat() {
			std::cout << "call cat ctor of animal " << std::endl;
		}
		std::unique_ptr<animal> create() { 
			std::cout << "call cat create" << std::endl;
			return std::make_unique<cat>();
		}
		std::unique_ptr<animal> clone() {
			std::cout << "call cat clone" << std::endl;
			return std::make_unique<cat>(*this);
		}
	};

	//在不知道对象具体类型的情况下，处理对象的创建和拷贝，并使用
	void who_am_i(animal *who) {
		auto new_who = who->create();	   // `create` the object of same type i.e. pointed by who ?
		auto duplicate_who = who->clone(); // `copy` object of same type i.e. pointed by who ?    
		std::cout << who << " " << new_who << " " << duplicate_who << std::endl;
		delete who;
	}

	void Test1()
	{
		dog* d = new dog();
		who_am_i(d);			//output: 01306F90 01306DE8 01306E18
		cat* c = new cat;
		who_am_i(c);			//output: 01306F90 01306DE8 01306E18
	}
}