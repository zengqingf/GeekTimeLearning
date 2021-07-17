#include "AbstractFactory_1.h"

void Creator_AbstractFactory::ClientCode(const AbstractFactory& factory)
{
	const AbstractProductA* product_a = factory.CreateProductA();
	const AbstractProductB* product_b = factory.CreateProductB();
	std::cout << product_b->UsefulFunctionB() << std::endl;
	std::cout << product_b->AnotherUsefulFunctionB(*product_a) << std::endl;
	delete product_a;
	delete product_b;
}

void Creator_AbstractFactory::TestAbstractFactory_1()
{
	std::cout << "Client: Testing client code with the first factory type:\n";
	ConcreteFactory1 *f1 = new ConcreteFactory1();
	ClientCode(*f1);
	delete f1;
	std::cout << std::endl;
	std::cout << "Client: Testing the same client code with the second factory type:\n";
	ConcreteFactory2 *f2 = new ConcreteFactory2();
	ClientCode(*f2);
	delete f2;
}