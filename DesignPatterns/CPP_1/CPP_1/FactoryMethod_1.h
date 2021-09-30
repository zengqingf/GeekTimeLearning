#pragma once

/*
概念：
工厂方法模式
(虚拟构造函数、virtual constructor、factory method)

工厂方法模式是一种创建型设计模式， 其在父类中提供一个创建对象的方法， 允许子类决定实例化对象的类型。

工厂方法模式建议使用特殊的工厂方法代替对于对象构造函数的直接调用 （即使用 new运算符）。 
对象仍将通过 new运算符创建， 只是该运算符改在工厂方法中调用罢了。 
工厂方法返回的对象通常被称作 “产品”。


使用场景：
1.无法预知对象确切类别及其依赖关系时，可使用工厂方法
	工厂方法将创建产品的代码与实际使用产品的代码分离， 从而能在不影响其他代码的情况下扩展产品创建部分代码。
	（如果需要向应用中添加一种新产品， 你只需要开发新的创建者子类， 然后重写其工厂方法即可。）

2.用户能扩展你软件库或框架的内部组件，可使用工厂方法

3.复用现有对象来节省系统资源，而不是每次都重新创建对象，可使用工厂方法
	处理大型资源密集型对象 （比如数据库连接、 文件系统和网络资源） 时，需要创建对象池
	创建存储空间来存放所有已经创建的对象--请求对象时， 程序将在对象池中搜索可用对象--返回给客户端--如果没有可用对象， 程序则创建一个新对象 （并将其添加到对象池中）
	既能够创建新对象， 又可以重用现有对象的普通方法 --> 工厂方法



编程实战：
C++:
在代码中提供高层次的灵活性，可以使用工厂方法模式
识别：可以通过构建方法来识别，创建具体类的对象，但以抽象类型或接口的形式返回这些对象
*/


#include <iostream>
#include <string>
//#include <stdio.h>

#ifndef FACTORY_METHOD_1
#define FACTORY_METHOD_1

namespace Creator_FactoryMethod
{
	//产品接口
	//声明了所有实例Product都需要实现的操作
	class Product {
	public:
		virtual ~Product() {}
		virtual std::string Operation() const = 0;
	};

	//具体产品，提供了对于Product的不同的实现
	class ConcreteProduct1 : public Product {
	public:
		std::string Operation() const override {
			return "{Result of the ConcreteProduct1}";
		}
	};
	class ConcreteProduct2 : public Product {
	public:
		std::string Operation() const override {
			return "{Result of the ConcreteProduct2}";
		}
	};
	
//@notice: this is use chinese in file which enciding in utf-8, compile error !!!
///	/*@注意：为了避免忘记delete new的对象，使用wrapper包装以下返回值*/
/// 模拟简单的智能指针
	class ProductWrapper
	{
	public:
		explicit ProductWrapper(Product* ptr = nullptr) : m_ptr(ptr) {
			if (nullptr == m_ptr)
			{
				throw "product wrapper create concrete product failed...";
			}
		}
		~ProductWrapper()
		{
			puts("call product wrapper dtor");
			//std::cout << "call product wrapper dtor" << std::endl;
			delete m_ptr;
		}
		Product& GetProduct() const { return *m_ptr; }
	private:
		Product* m_ptr = nullptr;
	};

	class Creator {
	public:
		virtual ~Creator() {}
		virtual Product* FactoryMethod() const = 0;

		std::string SomeOperation() const {
			//Product* product = this->FactoryMethod();
			////use product
			//std::string result = "Creator: the same creator's code has just worked with " + product->Operation();
			//delete product;

			//改善后的代码
			ProductWrapper pWrapper(this->FactoryMethod());
			std::string result = "Creator: the same creator's code has just worked with " + pWrapper.GetProduct().Operation();

			return result;
		}
	};

	class ConcreteCreator1 : public Creator
	{
	public:
		Product* FactoryMethod() const override
		{
			return new ConcreteProduct1();
		}
	};

	class ConcreteCreator2 : public Creator
	{
	public:
		Product* FactoryMethod() const override
		{
			return new ConcreteProduct2();
		}
	};

	void ClientCode(const Creator& creator);

	void TestFactoryMethod_1();


	class CreatorWrapper
	{
	public:
		explicit CreatorWrapper(Creator* ptr = nullptr) : m_ptr(ptr) {
			if (nullptr == m_ptr)
			{
				throw "creator wrapper create concrete creator failed...";
			}
		}
		~CreatorWrapper()
		{
			puts("call creator wrapper dtor");
			//std::cout << "call creator wrapper dtor" << std::endl;
			delete m_ptr;
		}
		Creator& GetCreator() const { return *m_ptr; }
	private:
		Creator* m_ptr = nullptr;
	};



	/*--------------------------------------------------------------------------*/

	class Button
	{
	public:
		virtual ~Button() {}
		virtual void SetRender() {}
		virtual void OnClick() {}
	};

	class RectButton : public Button
	{
	};

	//新增 圆形按钮
	class RoundButton : public Button
	{
	};

	class UIFramework
	{
	};

	//新增 圆形按钮UI 用以替换 UIFramework
	class UIWithRoundButtons : UIFramework 
	{
	};

	/*--------------------------------------------------------------------------*/
}

#endif //FACTORY_METHOD_1