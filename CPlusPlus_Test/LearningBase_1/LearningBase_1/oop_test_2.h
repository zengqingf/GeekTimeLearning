#pragma once

#include <iostream>


/*
ref: https://zhuanlan.zhihu.com/p/365223471
菱形继承


虚继承，   virtual public 基类
间接继承共同基类时只保留一份基类成员
在声明派生类而不是基类时，指定继承方式声明


继承：派生类对象中包含基类的子对象
*/

namespace OOP_TEST_2_1
{
	class GrandParent {
	public:
		std::string m_00;
		GrandParent()
		{
			m_00 = "grandParent";
			std::cout << "GrandParent constructed !" << std::endl;
		}
	};

	class Father : public GrandParent {
	public:
		std::string m_11;
		Father()
		{
			m_11 = "Father";
			std::cout << "Father constructed !" << std::endl;
		}
	};

	class Mother : public GrandParent {
	public:
		std::string m_22;
		Mother()
		{
			m_22 = "Mother";
			std::cout << "Mother constructed !" << std::endl;
		}
	};

	class GrandSon : public Father, public Mother {
	public:
		std::string m_33;
		GrandSon()
		{
			m_33 = "GrandSon";
			std::cout << "GrandSon constructed !" << std::endl;
		}
	};

	void Test1()
	{
		GrandSon grandSon;

		//访问基类数据需要通过::访问
		//grandSon.m_00  不明确的访问，产生二义性
		std::cout << grandSon.Mother::m_00.c_str() << std::endl;
		std::cout << grandSon.Father::m_00.c_str() << std::endl;

		/*
		output:
		GrandParent constructed !
		Father constructed !
		GrandParent constructed !
		Mother constructed !
		GrandSon constructed !
		grandParent
		grandParent
		
		grandson 存在两份基类，分别存于father和mother中，空间浪费
		*/
	}


	class Father2 : virtual public GrandParent {
	public:
		std::string m_11;
		Father2()
		{
			m_11 = "Father";
			std::cout << "Father constructed !" << std::endl;
		}
	};

	class Mother2 : virtual public GrandParent {
	public:
		std::string m_22;
		Mother2()
		{
			m_22 = "Mother";
			std::cout << "Mother constructed !" << std::endl;
		}
	};

	class GrandSon2 : public Father2, public Mother2 {
	public:
		std::string m_33;
		GrandSon2()
		{
			m_33 = "GrandSon";
			std::cout << "GrandSon constructed !" << std::endl;
		}
	};

	void Test2()
	{
		GrandSon2 grandSon;
		std::cout << grandSon.Mother2::m_00 << std::endl;
		std::cout << grandSon.Father2::m_00 << std::endl;  //先调用这个派生类所对应的基类ctor!!!
		std::cout << grandSon.m_00 << std::endl;
	}
	/*
	output:

	GrandParent constructed !
	Father constructed !
	Mother constructed !
	GrandSon constructed !
	grandParent
	grandParent
	grandParent

	基类GrandParent ctor只调用一次
	可以直接通过子类对象访问基类成员

对于虚基类的初始化是由最后的派生类中负责初始化。
在最后的派生类中不仅要对直接基类进行初始化，还要负责对虚基类初始化；

程序运行时，只有最后的派生类执行对基类的构造函数调用，而忽略其他派生类对虚基类的构造函数调用。
从而避免对基类数据成员重复初始化。因此，虚基类只会构造一次。
	*/
}



namespace OOP_TEST_2_2
{

}