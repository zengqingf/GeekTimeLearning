#include "FactoryMethod_1.h"

using namespace Creator_FactoryMethod;

void Creator_FactoryMethod::ClientCode(const Creator& creator)
{
	std::cout << "Client: I'm not aware of the creator's class, but it still works.\n"
		<< creator.SomeOperation() << std::endl;
}

void Creator_FactoryMethod::TestFactoryMethod_1()
{
	std::cout << "App: Launched with the ConcreteCreator1.\n";
	Creator* creator = new ConcreteCreator1();
	ClientCode(*creator);
	std::cout << std::endl;
	std::cout << "App: Launched with the ConcreteCreator2.\n";
	Creator* creator2 = new ConcreteCreator2();
	ClientCode(*creator2);

	delete creator;
	delete creator2;
}