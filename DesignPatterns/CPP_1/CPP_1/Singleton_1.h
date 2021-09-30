#pragma once

#include <stdlib.h>

#include <iostream>
#include <map>
#include <string>
#include <iosfwd>
#include <fstream>
#include <vector>
#include <cassert>
#include <memory>

using std::getline;
using std::vector;
using std::string;
using std::shared_ptr;
using std::map;
/*
http://www.vishalchovatiya.com/singleton-design-pattern-in-modern-cpp/?ref=hackernoon.com
*/

/*
To ensure one & only one instance of a class exist at any point in time.

The Singleton Design Pattern ensures that a class has only one instance and provides a global point of access to that instance. This is useful when exactly one object need to coordinate actions across the system.
So, essentially, the Singleton Design Pattern is nothing more than specifying a lifetime.

单例优点：
1. 应用程序配置全局访问点，对将来应用程序配置扩展可以在单个位置合并
2. 重构旧代码，将大量历史使用的全局变量移到单个类中，使其称为单例；可能是程序内联到更健壮的面向对象结构的中间步骤
3. 增强了可维护性，提供了对特定实例的单点访问
*/

namespace SingletonExample_1
{
	/* 
	自定义 country.txt
	*/
	struct Database
	{
		virtual int32_t get_population(const std::string& country) = 0;
	};
							//struct Database;			//add for Dependency Injection 
	class SingletonDatabase 
							:public Database			//add for Dependency Injection 
	{
	private:
		std::map<std::string, int32_t>  m_country;

		SingletonDatabase() {
			std::ifstream ifs("country.txt");

			std::string city, population;
			while (getline(ifs, city)) {
				getline(ifs, population);
				m_country[city] = stoi(population);
			}
		}

	public:
		//防止拷贝构造和拷贝赋值
		SingletonDatabase(SingletonDatabase const &) = delete;
		SingletonDatabase &operator=(SingletonDatabase const &) = delete;

		static SingletonDatabase &get() {
			static SingletonDatabase db;
			return db;
		}

		int32_t get_population(const std::string &name)
												override //add for Dependency Injection 
		{ return m_country[name]; }
	};

	struct SingletonRecordFinder
	{
		static int32_t GetTotalPopulation(const vector<string>& countries)
		{
			int32_t result = 0;
			for (auto& country : countries) {
				result += SingletonDatabase::get().get_population(country);
			}
			return result;
		}
	};

	void Test1()
	{
		vector<string> countries = { "China", "America" };
		assert(SingletonExample_1::SingletonRecordFinder::GetTotalPopulation(countries)
			== 1400000000 + 12345678);
	}


	//Dependency Injection

	//@注意：struct不支持前置，需要使用时，需要提前声明
	//struct Database
	//{
	//	virtual int32_t get_population(const std::string& country) = 0;
	//};

	class DummyDatabase : public Database
	{
		std::map<string, int32_t> m_countries;
	public:
		DummyDatabase() : m_countries{{"alpha", 1}, {"beta", 2}, {"gamma", 3}}
		{}
		int32_t get_population(const string& country) override
		{ return m_countries[country]; }
	};

	class ConfigurableRecordFinder
	{
	private:
		Database& m_db;

	public:
		ConfigurableRecordFinder() : m_db(SingletonDatabase::get()) {}
		ConfigurableRecordFinder(Database& db) : m_db{ db } {}
		int32_t GetTotalPopulation(const vector<string>& countries)
		{
			int32_t result = 0;
			for (auto& country : countries) {
				result += m_db.get_population(country);
			}
			return result;
		}
	};

	void Test2()
	{
		DummyDatabase db;
		ConfigurableRecordFinder crf(db);
		crf.GetTotalPopulation({ "alpha", "beta" });

		ConfigurableRecordFinder crf2(SingletonDatabase::get());
		crf.GetTotalPopulation({ "China", "America" });
		
	}
}

/*
Multiton is a variation to singleton but not directly linked to it. 
Remember that singleton prevents you to have additional instances
while Multiton Design Pattern sets up kind of key-value pair along with the limitation for the number of instance creation.
*/

namespace MultitonExample_1
{
	enum class Importance {PRIMARY, SECONDARY, TERITARY};

	template<typename T, typename Key=std::string>
	struct Multiton {
		static shared_ptr<T> get(const Key& key) {
			//if (const auto it = m_instance.find(key); it != m_instance.end()) { // C++17
			if(m_instance.find(key) != m_instance.end()){
				return m_instance[key];
			}
			return m_instance[key] = std::make_shared<T>();
		}
	private:
		static map<Key, shared_ptr<T>> m_instance;
	};

	template<typename T, typename Key>
	map<Key, shared_ptr<T>> Multiton<T, Key>::m_instance;	//Just initialization of static data member


	struct Printer {
		Printer() { std::cout << "Total instances so far = " << ++m_instCnt << std::endl; }

	private:
		static int m_instCnt;
	};
	int Printer::m_instCnt = 0;

	void Test1()
	{
		using mt = Multiton<Printer, Importance>;
		auto main = mt::get(Importance::PRIMARY);
		auto aux = mt::get(Importance::SECONDARY);
		auto aux2 = mt::get(Importance::SECONDARY);// Will not create additional instances
	}
}