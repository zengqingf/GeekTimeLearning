#pragma once
#ifndef _CHAPTER_13_H_
#define _CHAPTER_13_H_

#include <iostream>
#include <string>
#include <vector>
#include <algorithm>

#include <array>
#include <cassert>

using std::string;
using std::vector;
using std::array;

/*
算法 algorithm
https://en.cppreference.com/w/cpp/algorithm

*/

class Chapter_13
{
public:
	void TestCase1()
	{
		//使用更高层的抽象和封装
		vector<int> v = { 1, 3, 1, 4, 5 };			//count 算法计算元素数量（统计元素出现的次数）
		auto n1 = std::count(begin(v), end(v), 1);  //begin() end() 获取容器范围

		//相同处理 range-for
		int n2 = 0;
		for (auto x : v) {
			if (x == 1) {
				n2++;
			}
		}

		//算法+lambda  （ 函数式编程 ： 函数套函数 ）
		auto n = std::count_if(
			begin(v), end(v),
			[](auto x) {
				return x > 2;					//lambda中定义判断条件
			}
		);
	}

	/*
	iterator  迭代器
	算法通过迭代器 间接 访问容器及元素，算法能力由迭代器决定
	（泛型编程：分离了数据和操作，算法不用关心容器内部结构，以一致的方式去操作元素，适用范围广，灵活性高）
	*/
	void TestCase2()
	{
		vector<int> v = {1,2,3,4,5};               //vector容器
		auto iter1 = v.begin();					   //成员函数获取迭代器 自动类型推导
		auto iter2 = v.end();

		//建议全局获取迭代器
		auto iter3 = std::begin(v);				  //全局获取迭代器 自动类型推导
		auto iter4 = std::end(v);
	}

	//迭代器前进后退
	void TestCase3()
	{
		array<int, 5> arr = {0, 1, 2, 3, 4};      //array静态数组

		auto b = std::begin(arr);
		auto e = std::end(arr);

		assert(distance(b, e) == 5);			//迭代器的距离	

		auto p = next(b);						//获取"下一个"位置   
		assert(distance(b, p) == 1);
		assert(distance(p, b) == -1);			//反向计算迭代器举例

		advance(p, 2);							//迭代器前进两个位置，指向 '3'
		assert(*p == 3);
		assert(p == prev(e, 2));				//末端迭代器的前两个位置
	}

	//使用for-each代替for
	//在for循环体中，会处理复杂逻辑，导致遍历容器元素和操作元素混杂
	//for_each将遍历容器元素和操作容器元素 分离   实现函数式编程
	void TestCase4()
	{
		vector<int> v = { 5,2,1,1,3,1,4 };
		for (const auto& x : v) {
			std::cout << x << ",";
		}
		std::cout << std::endl;

		auto print = [](const auto& x)
		{
			std::cout << x << ",";
		};
		for_each(cbegin(v), cend(v), print);		//for_each  标准库 算法
		std::cout << std::endl;

		for_each(
			cbegin(v), cend(v),
			[](const auto& x)						//匿名lambda表达式
			{
				std::cout << x << ",";
			}
		);
		std::cout << std::endl;
	}

	//排序
	//排序算法对迭代器要求较高，通常是随机访问迭代器
	//最好用于 顺序容器 array / vector 
	void TestCase5()
	{
		vector<int> v { 5, 6, 4, 3, 2, 6, 7, 9, 3 };
		auto print = [](const auto& x)
		{
			std::cout << x << ",";
		};

		//cbegin() vs begin()  
		/*
		std::vector<int> vec;
		const std::vector<int> const_vec;

		vec.begin(); //iterator
		vec.cbegin(); //const_iterator

		const_vec.begin(); //const_iterator
		const_vec.cbegin(); //const_iterator
		*/

		//sort() 快排，不稳定排序， 全部元素排序
		std::sort(begin(v), end(v));
		_print_foreach_vector(v, print);

		//选出前几名（TopN）
		//top3
		std::partial_sort(begin(v), next(begin(v), 3), end(v));
		_print_foreach_vector(v, print);

		//选出前几名，但不要求再排出名次（BestN）
		//nth_element
		//best3
		std::nth_element(begin(v), next(begin(v), 3), end(v));
		_print_foreach_vector(v, print);
		/*@注意：GCC 4.9(C++11)  输出为 3,2,3,4,5,6,7,9,6
				 clang 5.0 (C++11) 输出为 2,3,3,4,5,6,6,7,9
		*/

		//中位数（Median）、百分位数（Percentile） 
		//nth_element
		//median
		auto mid_iter = next(begin(v), v.size() / 2);			//中位数位置
		std::nth_element(begin(v), mid_iter, end(v));
		std::cout << "median is " << *mid_iter << std::endl;

		//按照某种规则把元素划分成两组
		//partition
		auto pos = std::partition(begin(v), end(v),			
			[](const auto& x)
			{
				return x > 6;									//找出大于6的数
			}
		);
		for_each(begin(v), pos, print);
		std::cout << std::endl;

		//min / max
		auto value = std::minmax_element(cbegin(v), cend(v));	//找出第一和倒数第一 （最大最小）
		std::cout << "min : " << *value.first << ", max : " << *value.second << '\n';
	}


	//二分查找
	void TestCase6()
	{
		std::vector<int> v = { 3, 5, 1, 6, 7, 10, 42, 99, 31 };
		auto print = [](const auto& x)
		{
			std::cout << x << ",";
		};

		std::sort(begin(v), end(v));								 //二分查找，先排序，再查找
		auto found = binary_search(cbegin(v), cend(v), 7);           //二分查找 确定元素在否
		std::cout << "is 7 exists ? " << (found ? "true" : "false") << std::endl;

		//在已序容器中执行二分查找 
		//使用 lower_bound （返回第一个大于或等于值的位置）   
		//判断的条件有两个，一个是迭代器是否有效，另一个是迭代器的值是不是要找的值
		decltype(cend(v)) pos;     //声明一个迭代器
		pos = std::lower_bound(cbegin(v), cend(v), 7);				//找到第一个>=7的位置
		std::cout << "find first pos where >=7 in vector ? " << *pos << std::endl;
		found = (pos != cend(v)) && (*pos == 7);
		std::cout << "find 7 in vector ? " << found << std::endl;
		//assert(found);

		pos = std::lower_bound(cbegin(v), cend(v), 9);
		found = (pos != cend(v)) && (*pos == 9);
		std::cout << "find first pos where >=9 in vector ? " << *pos << std::endl;
		std::cout << "find 9 in vector ? " << found << std::endl;
		//assert(found);

		pos = std::upper_bound(cbegin(v), cend(v), 7);       //找到第一个 >7 的位置
		std::cout << "find first pos where >7 in vector ? " << *pos << std::endl;


		/*
		lower_bound 和 upper_bound 返回一个区间 

		begin < x <= lower_bound < upper_bound < end

		equal_range() 一次获取 [lower_bound, upper_bound]
		*/


		//对于有序容器 有等价的 find / lower_bound / upper_bound
		std::multiset<int> s = { 3, 5, 1, 6, 7, 7 , 7, 10, 42, 99, 31 };  //允许重复
		auto s_pos = s.find(7);
		assert(s_pos != s.end());				   //与end()比较才知道是否找到

		auto lower_pos = s.lower_bound(7);        //获取区间左端点
		auto upper_pos = s.upper_bound(7);		  //获取区间右端点
		for_each(lower_pos, upper_pos, print);    //输出 7,7,7,
	}


	//不使用二分查找的查找方法
	//用于查找区间：find_first_of / find_end
	void TestCase7()
	{
		std::vector<int> v = { 3, 5, 1, 6, 7, 10, 42, 99, 7, 31 };
		decltype(v.end()) pos;                                        //声明一个迭代器

		pos = std::find(begin(v), end(v), 7);                     //查找第一个出现的位置
		assert(pos != end(v));

		pos = std::find_if(
			begin(v), end(v),
			[](auto x) {
			return x % 2 == 0;				//是否为偶数
			}
		);
		assert(pos != end(v));

		array<int, 2> arr = {2, 4};
		pos = std::find_first_of(begin(v), end(v), begin(arr), end(arr));		//查找一个子区间
		assert(pos != end(v));
		std::cout << *pos;
	}


	void TestCase8()
	{
		std::vector<int> v = { 3, 5, 1, 6, 7, 10, 42, 99, 7, 31 };


		//使用逆序迭代(reverse)
		for_each(
			rbegin(v), rend(v),
			[](const auto& x)						//匿名lambda表达式
		{
			std::cout << x << ",";
		}
		);
	}

private:
	template<class Container, class Pri>
	void _print_foreach_vector(Container& v, Pri pri)
	{
		for_each(cbegin(v), cend(v), pri);
		std::cout << std::endl;
	}
};

#endif // _CHAPTER_13_H_