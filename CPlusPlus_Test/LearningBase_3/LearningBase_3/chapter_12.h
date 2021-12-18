#pragma once
#ifndef _CHAPTER_12_H_
#define _CHAPTER_12_H_

#include <iostream>
#include <string>
#include <cassert>

#include <array>
#include <vector>
#include <deque>

#include <set>
#include <map>

#include <unordered_map>
#include <unordered_set>

#include <stack>

#if __cplusplus >= 201703
#include <multiset>
#include <multimap>
#include <unordered_multimap>
#include <unordered_multiset>
#endif

using std::array;
using std::vector;
using std::deque;

using std::set;
using std::map;

using std::unordered_map;
using std::unordered_set;

/*
C++ 容器里存储的是元素的拷贝、副本，而不是引用
建议：
容器操作元素的很大一块成本就是值的拷贝, 将元素加入到容器时 使用std::move()来 转移，减少元素复制的成本

如果容器中存放的不是元素本身，而是元素的指针，来间接保存元素，
会导致容器本身的自动销毁元素的特性不能销毁通过指针间接引用的元素了

如果必须通过指针存放，考虑使用unique_ptr(独立语义)和shared_ptr(共享语义)
推荐使用shared_ptr，其共享语义和容器值语义基本一致

@注意：
	多用类型别名， 不要写死容器定义；当需要切换容器时，只要变动别名定义即可（因为容器大部分接口都是相同的）
	用vector 替代 new / delete 创建动态数组 
*/

class Point final
{
public:
	int x = 0;
public:
	Point(int a) noexcept : x(a)
	{}

	Point() noexcept {}

	~Point() noexcept {}

	Point(const Point& p) noexcept
	{
		x = p.x;
	}

	Point(Point&& p) noexcept
	{
		x = std::move(p.x);
	}
};

class Chapter_12
{
	//array 和 vector 直接对应 C 的内置数组，内存布局与 C 完全兼容，所以是开销最低、速度最快的容器。

public:

	//顺序容器：元素次序根据插入的次序决定，访问元素也按照最初插入顺序
	void TestContainer1()
	{
		array<int, 2> arr;						//初始一个arrary  length = 2
		assert(arr.size() == 2);				

		//动态增长数组
		vector<int> v(2);						//初始化一个vector，length == 2
		for (size_t i = 0; i < 10; i++)
		{
			v.emplace_back(i);					//直接在容器中构造元素，不需要拷贝或者转移，在最后追加元素
		}
		assert(v.size() == 12);
		
		vector<Point> v1;
		v1.reserve(10);
		/*
		当 vector 的容量到达上限的时候（capacity），
		它会再分配一块两倍大小的新内存，然后把旧元素拷贝或者移动过去。
		这个操作的成本是非常大的，所以，你在使用 vector 的时候最好能够“预估”容量，
		使用 reserve 提前分配足够的空间，减少动态扩容的拷贝代价。
		*/

		Point p;
		v1.push_back(p);
		v1.push_back(std::move(p));
		v1.emplace_back(1);


		/*-----------------------------------------------------------------------------------*/

		//动态增长数组
		deque<int> dq;									//可以两端插入删除元素
		dq.emplace_back(9);
		dq.emplace_front(1);
		assert(dq.size() == 2);


		//动态增长链表
		//list：双向链表，可以向前或者向后遍历
		//forward_list：单向链表，只能向前遍历

		/*
		deque、list 的的扩容策略就“保守”多了，
		只会按照固定的“步长”（例如 N 个字节、一个节点）去增加容量。
		但在短时间内插入大量数据的时候就会频繁分配内存，
		效果反而不如 vector 一次分配来得好。
		*/
	}

	//有序容器：元素插入后会按照某种规则自动排序
	//C++使用树（通常为红黑树，最好查找性能的二叉树）
	//在定义有序容器时，需要指定key的比较函数，通常为less函数
	//C++中 int \ string 等基本类型都支持比较排序，可以直接放入有序容器

	//集合关系用 set  关联数组用 map

	//成本：有序容器在插入的时候会自动排序，所以就有隐含的插入排序成本，当数据量很大的时候，内部的位置查找、树旋转成本可能会比较高。
	//需要实时插入排序，那么选择 set/map 是没问题的。
	//如果是非实时，那么最好还是用 vector，全部数据插入完成后再一次性排序，效果肯定会更好。
	void TestContainer2()
	{
		//方法1
		set<Point> s;
		s.emplace(7);
		s.emplace(3);
		for (auto& x : s) {
			std::cout << x.x << ",";
		}

		std::cout << std::endl;

		//方法2
		set<int> s1 = { 7, 3, 9 };
		for (auto& x : s1) {
			std::cout << x << ",";
		}  //output : 3,7,9

		std::cout << std::endl;

		auto comp = [](auto a, auto b) //定义lambda 比较大小
		{
			return a > b;
		};

		set<int, decltype(comp)> gs(comp);			//使用decltype 得到 lambda类型
		std::copy(begin(s1), end(s1),					//拷贝算法，拷贝数据
			std::inserter(gs, gs.begin()));				//使用插入迭代器

		for (auto& x : gs) {
			std::cout << x << ",";
		} //output : 9,7,3

		std::cout << std::endl;
	}

	//无序容器
	//C++使用散列表（hash table），元素位置取决于计算的散列值

	//key要求: 可以计算hash值， 能够执行相等比较操作
	/*
	第一个是因为散列表的要求，只有计算 hash 值才能放入散列表，
	第二个则是因为 hash 值可能会冲突，所以当 hash 值相同时，就要比较真正的 key 值。
	*/
	void TestContainer3()
	{
		using map_type =
			unordered_map<int, std::string>;

		map_type dict;
		dict[1] = "one";
		dict.emplace(2, "two");
		dict[10] = "ten";

		for (auto& x : dict) {						//顺序不确定
			std::cout << x.first << "=>"
				<< x.second << ",";
		}

		std::cout << std::endl;


		auto hasher = [](const auto& p)
		{
			return std::hash<int>()(p.x);
		};

		unordered_set<Point, decltype(hasher)> s3(10, hasher);
		s3.emplace(7);
		s3.emplace(3);
		for (auto& x : s3) {
			std::cout << x.x << ",";
		}

		std::cout << std::endl;
	}	


	/*
	有序容器 和 无序容器
	等价（equivalent） vs. 相等 （equality）
	等价： !(x < y) && !(x > y)  基于次序关系，对象不一定相同
	相等： 两个对象相同
	*/




	/*
	insert vs emplace vs operator[] in c++ map
	https://stackoverflow.com/questions/17172080/insert-vs-emplace-vs-operator-in-c-map

	The operator[] is a find-or-add operator
		它将尝试在映射中找到具有给定键的元素，
		如果元素存在，它将返回对存储值的引用。
		如果没有，它将创建一个用默认初始化插入的新元素，并返回对它的引用。

	The insert function (in the single element flavor) 
		takes a value_type (std::pair<const Key,Value>), 
		它使用键(第一个成员)并尝试插入它。
		因为std::map不允许重复，所以如果存在一个现有的元素，它就不会插入任何东西。

	*/
	void TestContainer4()
	{
		std::map<int, int> m{ {5, 0} };
		std::cout << "test 1 map insert or [], " << m[5] << std::endl; //output : 0
		m[5] = 10;
		std::cout << "test 2 map insert or [], " << m[5] << std::endl; //output : 10
		m.insert(std::make_pair(5, 15));
		//make_pair == m.insert(std::pair<int, int>(5, 15));
		std::cout << "test 3 map insert or [], " << m[5] << std::endl; //output : 10
	}

	void reportStackSize(const std::stack<int>& s)
	{
		std::cout << s.size() << " elements on stack\n";
	}

	void reportStackTop(const std::stack<int>& s)
	{
		std::cout << "Top element: " << s.top() << '\n';
	}

	void TestContainer5()
	{
		std::stack<int> s;
		s.push(1);
		s.push(2);
		s.push(4);

		reportStackSize(s);
		reportStackTop(s);

		reportStackSize(s);
		s.pop();

		reportStackSize(s);
		reportStackTop(s);

		s.pop();

		reportStackSize(s);
		reportStackTop(s);

		s.pop();

		reportStackSize(s);

		//@注意：
		//如果stack size 为0时 不能直接进行top()
		//reportStackTop(s);
		if (s.size() > 0)
		{
			std::cout << "Top element: " << s.top() << '\n';
		}
	}

	/*
	map中

	operator[] vs. insert vs. emplace
	ref: https://stackoverflow.com/questions/17172080/insert-vs-emplace-vs-operator-in-c-map
		http://blog.guorongfei.com/2016/03/16/cppx-stdlib-empalce/

	operator[]: find-or-add operator 
		m[k]=v
		1.尝试找给定键的元素，若元素存在，则返回元素值的引用，若不存在，则创建并插入一个默认初始化的元素，并返回其引用
		2.需要一个能够构造默认初始化值，对于不支持默认可构造或者可赋值的，不能使用operator[]
		3.若类型是默认可构造和可赋值的，并且需要一个对象的默认初始化以及复制到该对象
		4.operator[]会覆盖存在键的值

	insert(): value_type（std::pair<const key, value>）
			K t; V u;
			std::map<K,V> m;   // std::map<K,V>::value_type is std::pair<const K,V>

		1. 使用pair的first作为key插入，由于std::map不支持重复key，所以如果存在key，就不会执行插入
		2. insert() 参数有不同的方式
		3. 外部创建value_type并复制到map中
		4. 会产生临时对象
		5. 如果make_pair<> 模板推导参数不一致但接近（如map为<const int,int>, make_pair(10, 10)） 
			会创建一个正确类型的临时对象拷贝初始化，然后拷贝到map中，过程中会有两份副本

	emplace(): 
		1.c++11 通过可变模板和完美转发，可以通过emplace(就地创建，in place)将元素添加到容器中，
			不同的容器emplace()基本上功能相同，通过转发而不是复制，将元素存到容器中的对象的构造函数
		”变参模板”使得 emplace 可以接受任意参数，这样就可以适用于任意对象的构建。
		”完美转发”使得接收下来的参数 能够原样的传递给对象的构造函数，
			这带来另一个方便性就是即使是构造函数声明为 explicit 它还是可以正常工作，因为它不存在临时变量和隐式转换。
		2.避免不必要的临时对象的产生，在map中，the std::pair<const K, V> is not created and passed to emplace,
			but rather references to the t and u object are passed to emplace that forwards them to 
				the constructor of the value_type subobject inside the data structure.
		3.不会覆盖已存在的键值
	*/
	void TestContainer6()
	{
		std::map<int, int> m1{ {1, 2}, {2, 4}, {3, 6}, {4, 8}, {5, 10} };
		m1[5] = 15;
		std::cout << m1[5] << std::endl;			//15

		m1[6] = 12;
		std::cout << m1[6] << std::endl;
		m1.insert(std::make_pair(7, 14));
		std::cout << m1[7] << std::endl;

		m1.insert(std::make_pair(5, 20));		
		std::cout << m1[5] << std::endl;			//15
//是模板方法 
/*
template <typename T, typename U>
std::pair<T,U> make_pair(T const & t, U const & u );
*/
		m1.insert(std::make_pair<const int, int>(5, 21));
		std::cout << m1[5] << std::endl;			//15

		m1.insert(std::make_pair<const int, int>(5, 21));
		std::cout << m1[5] << std::endl;			//15

		m1.insert(std::pair<const int, int>(5, 25));
		std::cout << m1[5] << std::endl;			//15
		m1.insert(std::map<int, int>::value_type(5, 30));		//使用value_type可以避免插入错误的类型
		std::cout << m1[5] << std::endl;			//15


		m1.emplace(5, 35);
		m1.emplace(10, 50);

		map<string, std::complex<double>> scp;
		scp.emplace("hello", 1, 2);  // 无法区分哪个参数用来构造 key 哪些用来构造 value
									 // string s("hello", 1), complex<double> cpx(2) ???
									 // string s("hello"), complex<double> cpx(1, 2) ???

		// map 虽然避免了临时变量的构造，但是却需要构建两个 tuple
		//std::map<string, complex<double>> scp;
		//scp.emplace(piecewise_construct,
		//	forward_as_tuple("hello"),
		//	forward_as_tuple(1, 2));

		// 使用insert更直观，临时对象如果是基础类型，可以使用insert
		//scp.insert({"world", {1, 2}});
	}

	struct Foo {
		Foo(int n, double x);
	};
	struct Bar {
		Bar(int a) {}
		explicit Bar(int a, double b) {}
	};
	void TestContainer7()
	{
		std::vector<Foo> av;
		//av.emplace(42, 3.1416);        // 没有临时变量产生
		//av.insert(Foo(42, 3.1416));    // 需要产生一个临时变量
		//av.insert({ 42, 3.1416 });     // 需要产生一个临时变量

		std::vector<Bar> bv;
		bv.push_back(1);			 // 隐式转换生成临时变量
		bv.push_back(Bar(1));		 // 显示构造临时变量
		bv.emplace_back(1);			 // 没有临时变量

		//bv.push_back({1, 2.0});   // 无法进行隐式转换
		bv.push_back(Bar(1, 2.0));  // 显示构造临时变量
		bv.emplace_back(1, 2.0);    // 没有临时变量
	}
};

//自定义比较运算
bool operator < (const Point& a, const Point& b)
{
	return a.x < b.x;
}

bool operator == (const Point& a, const Point& b)
{
	return a.x == b.x;
}

#endif //_CHAPTER_12_H_