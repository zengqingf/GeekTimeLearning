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