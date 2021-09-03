#pragma once

/*
Design Patterns: Builder Pattern in Modern C++ by
@IndianWestCoast
https://hackernoon.com/design-patterns-builder-pattern-in-modern-c-x1283uy3
*/

#include <iostream>

class PersonBuilder;
class Person
{
private:
	std::string m_name, m_streetAddress, m_postcode, m_city;
	std::string m_companyName, m_position, m_annualIncome;

	Person(std::string name) : m_name(name) {}

public:
	friend class PersonBuilder;
	friend std::ostream& operator <<(std::ostream& os, const Person& obj);
	static PersonBuilder Create(std::string name);
};

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