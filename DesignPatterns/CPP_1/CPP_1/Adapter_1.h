#pragma once

/*
To get the interface you want from the interface you have.

适配器允许两个不兼容的类一起工作，方法是将一个类的接口转换为客户端/ api用户所期望的接口，
而不需要更改它们。基本上，添加中间类，即适配器。

使用场景：
1.当想使用一些现有类，但是接口和现有代码不兼容时
2.当想重用一些现有派生类，但是这些类缺少一些公共功能而不能添加到超类
例如：有一个函数接收天气对象并打印摄氏度，新增需求需要调整为打印华氏度，这种不兼容情况下，使用适配器


桥接设计模式和适配器设计模式有什么区别?
-适配器通常与现有的应用程序一起使用，以使一些其他不兼容的类很好地一起工作。
-桥接通常是预先设计的，让您可以独立地开发应用程序的各个部分。


装饰器和适配器设计模式的区别是什么?
-适配器转换一个接口到另一个，没有添加额外的功能
-装饰器添加新的功能到现有的界面。


代理和适配器设计模式之间的区别是什么?
-适配器设计模式将一个类的接口转换为一个兼容但不同的接口。
-代理提供了相同但简单的接口，或者有时作为唯一的包装器。
*/

#include <iostream>
#include <functional>
#include <memory>

namespace Adapter_Example_1
{
	struct Point {
		int32_t m_x;
		virtual void draw() { std::cout << "Point \n"; }
	};

	struct Point2D : Point {
		int32_t m_y;
		void draw() override { std::cout << "Point2D \n"; }
	};

	void DrawPoint(Point &p) {
		p.draw();
	}

	struct Line {
		Point2D m_start;
		Point2D m_end;
		void draw() { std::cout << "Line \n"; }
	};

	struct LineAdapter : Point {
		Line& m_line;
		LineAdapter(Line &line) : m_line(line) {}
		void draw() { m_line.draw(); }
	};


	//泛型，处理类似Point
	template<class T>
	struct GenericLineAdapter :Point {
		T&		m_line;
		GenericLineAdapter(T &line) : m_line(line) {}
		void draw() { m_line.draw(); }
	};

	void Test1()
	{
		Line l;
		LineAdapter lineAdapter(l);
		DrawPoint(lineAdapter);
	}


	//可插拔适配器 Pluggable Adapter
	//适配器应该支持不相关且具有不同接口的适配者使用客户端已知相同的老的目标接口
	//通过lambda实现
	struct Beverage {
		~Beverage() {
			std::cout << "Beverage dtor" << std::endl;
		}
		virtual void GetBeverage() = 0;
	};

	struct CoffeeMaker : Beverage {
		~CoffeeMaker() {
			std::cout << "CoffeeMaker dtor" << std::endl;
		}
		void Brew() { std::cout << "brewing coffee" << std::endl; }
		void GetBeverage() override { Brew(); }
	};

	void MakeDrink(Beverage &drink) {
		drink.GetBeverage();
	}

	//没有继承的新增类
	struct JuiceMaker {
		~JuiceMaker() {
			std::cout << "JuiceMaker dtor" << std::endl;
		}
		void Squeeze() { std::cout << "making juice" << std::endl; }
	};

	struct BeverageAdapter : Beverage {
		function<void()> m_request;

		//可配置适配器：为它所适应的每一种类型提供构造函数
		//可以直接绑定接口，而不需要传入对象
		//当适配器和适配者数量不一致时，也可以起作用
		BeverageAdapter(shared_ptr<CoffeeMaker> cm) { m_request = [cm]() { cm->Brew(); }; }
		BeverageAdapter(shared_ptr<JuiceMaker> jm) { m_request = [jm]() {jm->Squeeze(); }; }

		void GetBeverage() override { m_request(); }
	};

	//class BeverageWrapper{
	//public:
	//	explicit BeverageWrapper(Beverage* btr) : m_btr(btr){}
	//	~BeverageWrapper() {
	//		delete m_btr;
	//	}
	//	Beverage* GetBeveragePtr() const { return m_btr; }
	//private:
	//	Beverage* m_btr = nullptr;
	//};

	void Test2()
	{
		shared_ptr<CoffeeMaker> cptr(new CoffeeMaker());
		BeverageAdapter adp1(cptr);
		MakeDrink(adp1);

		shared_ptr<JuiceMaker> jptr(new JuiceMaker());
		BeverageAdapter adp2(jptr);
		MakeDrink(adp2);
	}
}