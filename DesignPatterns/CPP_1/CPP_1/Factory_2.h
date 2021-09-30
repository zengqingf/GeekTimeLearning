#pragma once

#include <iostream>
#include <cmath>
#include <math.h>
#include <memory>
#include <functional>

//marco for windows
#include <corecrt_math_defines.h>

using std::ostream;
using std::unique_ptr;
using std::make_unique;
using std::function;

/*
ref: http://www.vishalchovatiya.com/factory-design-pattern-in-modern-cpp/?ref=hackernoon.com

For the creation of wholesale(批量创建) objects unlike builder(which creates piecewise，分段创建).

通过初始化过程从构造函数移动到其他结构来减轻构造函数的限制，即使用方法或成员函数来初始化对象

工厂方法的本质：
	不允许使用构造函数创建对象，而是强制用户使用静态方法创建
*/
namespace FactoryExample_1
{
	enum class PointType { CARTESIAN, POLAR }; //笛卡尔坐标系点类型、极坐标系点类型

	class Point {
		float m_x;
		float m_y;
		PointType m_type;

		Point(const float x, const float y, PointType t) : m_x(x), m_y(y), m_type(t) {}

	public:
		friend ostream& operator<<(ostream& os, const Point& obj) 
		{
			return os << "x: " << obj.m_x << " y: " << obj.m_y;
		}

		static Point NewCartesian(float x, float y)
		{
			return { x, y, PointType::CARTESIAN };
		}

		static Point NewPolar(float a, float b)
		{
			return { a * std::cos(b), a * std::sin(b), PointType::POLAR };
		}
	};

	void Test1()
	{
		auto p = Point::NewPolar(5, M_PI_4);
		std::cout << p << std::endl;
	}
}

namespace FactoryExample_2
{
	enum class PointType { CARTESIAN, POLAR }; //笛卡尔坐标系点类型、极坐标系点类型

	class PointFactory;
	class Point
	{
		float m_x;
		float m_y;
		PointType m_type;

		Point(const float x, const float y, PointType t) : m_x(x), m_y(y), m_type(t) {}

		//friend违反了开闭原则
		friend class PointFactory;

	public:
		friend ostream& operator<<(ostream& os, const Point& obj)
		{
			return os << "x: " << obj.m_x << " y: " << obj.m_y;
		}
	};

	//具体工厂模式
	//由于PointFactory和Point没有强联系，对于用户来说，在未详细了解代码结构情况下（类内方法基本为私有），
	//不能很直观知道通过PointFactory去创建Point
	class PointFactory
	{
	public:
		static Point NewCartesian(float x, float y)
		{
			return { x, y, PointType::CARTESIAN };
		}

		static Point NewPolar(float r, float theta)
		{
			return { r * std::cos(theta), r * std::sin(theta), PointType::POLAR };
		}
	};
}

namespace FactoryExample_3
{
	enum class PointType { CARTESIAN, POLAR }; //笛卡尔坐标系点类型、极坐标系点类型
	class Point
	{
		float m_x;
		float m_y;
		PointType m_type;

		Point(const float x, const float y, PointType t) : m_x(x), m_y(y), m_type(t) {}

	public:
		friend ostream& operator<<(ostream& os, const Point& obj)
		{
			return os << "x: " << obj.m_x << " y: " << obj.m_y;
		}

		/*
		使用Inner Factory
		便于用户直观了解到Point的创建方式
		*/
		struct Factory {
			static Point NewCartesian(float x, float y)
			{
				return { x, y, PointType::CARTESIAN };
			}

			static Point NewPolar(float r, float theta)
			{
				return { r * std::cos(theta), r * std::sin(theta), PointType::POLAR };
			}
		};
	};

	void Test1()
	{
		auto p = Point::Factory::NewCartesian(2, 3);
		std::cout << p << std::endl;
	}
}

namespace FactoryExample_4
{
	enum class PointType 
	{ 
		POINT_2D,	
		POINT_3D
	}; 

	struct Point
	{
		virtual ~Point() = default;
		virtual unique_ptr<Point> create() = 0;
		virtual unique_ptr<Point> clone() = 0;
	};

	//使用多态，实现了虚构造函数（create()）和虚赋值（拷贝）构造函数（clone()）
	struct Point2D : Point
	{
		unique_ptr<Point> create() { return make_unique<Point2D>(); }
		unique_ptr<Point> clone() {	return make_unique<Point2D>(*this); }
	};

	struct Point3D : Point
	{
		unique_ptr<Point> create() { return make_unique<Point3D>(); }
		unique_ptr<Point> clone() { return make_unique<Point3D>(*this); }
	};

	void Test1(Point *p)
	{
		std::cout << "raw p: " << p << std::endl;
		auto new_p = p->create();
		std::cout << "create p: " << new_p << std::endl;
		auto duplicate_p = p->clone();
		std::cout << "clone p: " << duplicate_p << std::endl;
	}

	class PointFunctionFactory
	{
		map< PointType, function<unique_ptr<Point>()> > m_factories;

	public:

		//如果需要多个方法创建一个对象时，考虑修改lambda或者引入建造者模式 ------> 函数式工厂
		PointFunctionFactory() {
			m_factories[PointType::POINT_2D] = []() {return make_unique<Point2D>(); };
			m_factories[PointType::POINT_3D] = []() {return make_unique<Point3D>(); };
		}
		unique_ptr<Point> create(PointType type) { return m_factories[type](); }
	};

	void Test2()
	{
		PointFunctionFactory pf;
		auto p3D = pf.create(PointType::POINT_3D);
		std::cout << p3D << std::endl;
	}
}