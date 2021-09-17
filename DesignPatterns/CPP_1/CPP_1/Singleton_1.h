#pragma once

#include <stdlib.h>

#include <iostream>
#include <map>
#include <string>
#include <iosfwd>
#include <fstream>
#include <vector>
#include <cassert>

using std::getline;
using std::vector;
using std::string;

/*
http://www.vishalchovatiya.com/singleton-design-pattern-in-modern-cpp/?ref=hackernoon.com
*/

namespace SingletonExample_1
{
	/* 
	自定义 country.txt
	*/
							class Database;	//add for Dependency Injection 
	class SingletonDatabase 
							:public Database	
	{
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
		SingletonDatabase(SingletonDatabase const &) = delete;
		SingletonDatabase &operator=(SingletonDatabase const &) = delete;

		static SingletonDatabase &get() {
			static SingletonDatabase db;
			return db;
		}

		int32_t get_population(const std::string &name)
												override 	//add for Dependency Injection 
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
	struct Database
	{
		virtual int32_t get_population(const std::string& country) = 0;
	};

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

namespace SingletonExample_2
{

}