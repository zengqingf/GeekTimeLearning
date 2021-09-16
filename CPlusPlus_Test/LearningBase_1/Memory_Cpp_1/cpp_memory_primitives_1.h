#pragma once

#ifndef _CPP_MEMORY_PRIMITIVES_1_H_
#define _CPP_MEMORY_PRIMITIVES_1_H_

#include <iostream>
#include <cassert>
using std::cout;
using std::endl;

namespace TestCppMemoryPrimitives
{
	void Test1();

	void Test2();

	void Test3();


	class A
	{
	public:
		int id;

		A() : id(0) { cout << "default ctor. this=" << this << " id=" << id << endl; }
		A(int i) : id(i) { cout << "ctor. this=" << this << " id=" << id << endl; }
		~A() { cout << "dtor. this=" << this << " id=" << id << endl; }
	};

	void Test4();

	void Test5();

	void Test6();

	void Test7();
	
}	//namespace TestCppMemoryPrimitives

#endif //_CPP_MEMORY_PRIMITIVES_1_H_