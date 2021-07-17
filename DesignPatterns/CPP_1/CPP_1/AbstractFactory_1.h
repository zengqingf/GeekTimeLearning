#pragma once

/*
概念：
抽象工厂模式
是一种创建型设计模式， 它能创建一系列相关的对象， 而无需指定其具体类

抽象工厂模式建议为系列中的每件产品明确声明接口，确保所有产品变体都继承这些接口。
然后需要声明抽象工厂——包含系列中所有产品构造方法的接口，方法必须返回抽象产品类型

对于产品变体：
基于抽象工厂接口创建不同的工厂类，每个工厂类都只能返回特定类别的产品
客户端代码可以通过相应的 抽象接口 调用工厂和产品类
无需修改实际客户端代码，就能更改传递给客户端的工厂类，也能更改客户端代码接收的产品变体

上述流程要求客户端只接触抽象接口
把创建实际工厂对象的时机和位置放到：应用程序初始化阶段创建具体工厂对象（需要根据配置文件或者环境设定选择好工厂类别）


编程实战：
C++:
抽象工厂模式在 C++ 代码中很常见，许多框架和程序库会将它作为扩展和自定义其标准组件的一种方式
识别：可以通过方法来识别该模式——其会返回一个工厂对象，接下来，工厂将被用于创建特定的子组件
*/

#include <string>
#include <iostream>

namespace Creator_AbstractFactory
{
	class AbstractProductA {
	public:
		virtual ~AbstractProductA() {};
		virtual std::string UsefulFunctionA() const = 0;
	};

	class ConcreteProductA1 : public AbstractProductA {
	public:
		std::string UsefulFunctionA() const override {
			return "The result of the product A1.";
		}
	};

	class ConcreteProductA2 : public AbstractProductA {
	public:
		std::string UsefulFunctionA() const override {
			return "The result of the product A2.";
		}
	};

	class AbstractProductB {
	public:
		virtual ~AbstractProductB() {};
		virtual std::string UsefulFunctionB() const = 0;

		/*
		可以与ProductA协作
		抽象工厂确保它创建的所有产品都是相同的变体，因此是兼容的
		*/
		virtual std::string AnotherUsefulFunctionB(const AbstractProductA& collaborator) const = 0;
	};

	class ConcreteProductB1 : public AbstractProductB {
	public:
		std::string UsefulFunctionB() const override {
			return "The result of the product B1.";
		}

		std::string AnotherUsefulFunctionB(const AbstractProductA &collaborator) const override {
			const std::string result = collaborator.UsefulFunctionA();
			return "The result of the B1 collaborating with ( " + result + " )";
		}
	};

	class ConcreteProductB2 : public AbstractProductB {
	public:
		std::string UsefulFunctionB() const override {
			return "The result of the product B2.";
		}

		std::string AnotherUsefulFunctionB(const AbstractProductA &collaborator) const override {
			const std::string result = collaborator.UsefulFunctionA();
			return "The result of the B2 collaborating with ( " + result + " )";
		}
	};


	class AbstractFactory {
	public:
		virtual AbstractProductA *CreateProductA() const = 0;
		virtual AbstractProductB *CreateProductB() const = 0;
	};

	class ConcreteFactory1 : public AbstractFactory {
	public:
		AbstractProductA *CreateProductA() const override
		{
			return new ConcreteProductA1();
		}
		AbstractProductB *CreateProductB() const override
		{
			return new ConcreteProductB1();
		}
	};

	class ConcreteFactory2 : public AbstractFactory {
	public:
		AbstractProductA *CreateProductA() const override
		{
			return new ConcreteProductA2();
		}
		AbstractProductB *CreateProductB() const override
		{
			return new ConcreteProductB2();
		}
	};

	void ClientCode(const AbstractFactory& factory);

	void TestAbstractFactory_1();

	/*--------------------------------------------------------------------------*/
}