#pragma once
#ifndef _CHAPTER_15_H_
#define _CHAPTER_15_H_

#include <iostream>
#include <string>
#include <vector>
#include <map>
#include <cassert>

#include "thirdparty/json.hpp"

#include "thirdparty/msgpack-c-cpp_master/include/msgpack.hpp"

#include <boost/version.hpp>
#include <boost/config.hpp>

using std::string;
using std::vector;
using std::map;
/*
序列化

json库：JSON for Modern C++
https://github.com/nlohmann/json

MessagePack  轻量级数据交换格式库   msgpack-c
https://github.com/msgpack/msgpack-c/tree/cpp_master

*/
class Chapter_15
{
	using json_t = nlohmann::json;

public:
	void Test1()
	{
		json_t j;
		j["age"] = 23;
		j["name"] = "spiderman";
		j["gear"]["suits"] = "2099";
		j["jobs"] = { "superhero" };

		vector<int> v = { 1, 2, 3 };
		j["numbers"] = v;

		map<string, int> m = { {"one", 1}, {"two", 2} };
		j["kv"] = m;

		std::cout << j.dump() << std::endl;
		std::cout << j.dump(2) << std::endl;
	}

	void Test2()
	{
		string str = R"({
			"name": "peter",
			"age": 28,
			"married": true
			})";

		auto j = json_t::parse(str);
		assert(j["age"] == 28);
		assert(j["name"] == "peter");
	}

	void Test3()
	{
		std::string txt = "bad:data";
		try
		{
			auto j = json_t::parse(txt);
		}
		catch (std::exception& e)
		{
			std::cout << e.what() << std::endl;
		}
	}

	void Test4()
	{
		std::cout << BOOST_VERSION << std::endl;
		std::cout << BOOST_LIB_VERSION << std::endl;
	}

	void Test5()
	{
		vector<int> v = { 1, 2, 3, 4, 5 };
		msgpack::sbuffer sbuf;					//输出缓存区
		msgpack::pack(sbuf, v);					//序列化（传入序列化输出目标和被序列化的对象）

		std::cout << sbuf.size() << std::endl;
	}
};

#endif  // _CHAPTER_15_H_