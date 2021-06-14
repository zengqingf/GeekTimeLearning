#pragma once
#ifndef _CHAPTER_13_H_
#define _CHAPTER_13_H_

#include <iostream>
#include <string>
#include <vector>
#include <algorithm>

using std::string;
using std::vector;


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

	}
};

#endif // _CHAPTER_13_H_