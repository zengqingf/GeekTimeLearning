#include "Builder_1.h"


void BuilderExample_1::Test1()
{
	auto builder = HtmlElement::build("ul");
	builder->addChild("li", "hello")->addChild("li", "world");
	std::cout << builder->str() << std::endl;
}

PersonBuilder Person::Create(std::string name)
{
	return PersonBuilder { name };
}

ostream & operator<<(ostream & os, const Person & obj)
{
	return os << obj.m_name
		<< std::endl
		<< "lives : " << std::endl
		<< "at " << obj.m_streetAddress
		<< " with postcode " << obj.m_postcode
		<< " in " << obj.m_city
		<< std::endl
		<< "works : " << std::endl
		<< "with " << obj.m_companyName
		<< " as a " << obj.m_position
		<< " earning " << obj.m_annualIncome;
}

PersonBuilder&  PersonBuilder::Lives() { return *this; }

PersonBuilder&  PersonBuilder::Works() { return *this; }

PersonBuilder&  PersonBuilder::With(string company_name) {
	person.m_companyName = company_name;
	return *this;
}

PersonBuilder&  PersonBuilder::AsJob(string position) {
	person.m_position = position;
	return *this;
}

PersonBuilder&  PersonBuilder::Earning(string annual_income) {
	person.m_annualIncome = annual_income;
	return *this;
}

PersonBuilder&  PersonBuilder::At(std::string street_address) {
	person.m_streetAddress = street_address;
	return *this;
}

PersonBuilder&  PersonBuilder::WithPostcode(std::string post_code) {
	person.m_postcode = post_code;
	return *this;
}

PersonBuilder&  PersonBuilder::In(std::string city) {
	person.m_city = city;
	return *this;
}

void TestBuilder1::Test1()
{
	Person p = Person::Create("jj")
		.Lives()
		.At("HZ")
		.WithPostcode("310000")
		.In("CN")
		.Works()
		.With("TM")
		.AsJob("SDK")
		.Earning("10w");
	//std::cout << p << std::endl;
}