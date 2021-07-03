#pragma once

#ifndef _PRE_CLASS_ALLOCATOR_1_H_
#define _PRE_CLASS_ALLOCATOR_1_H_

#include <cstddef>
#include <iostream>
using namespace std;

class Screen
{
public:
	Screen(int x) : m_i(x) {};
	int get() { return m_i; }
	 

	void* operator new(size_t);
	void operator delete(void*, size_t);

private:
	Screen* next;
	static Screen* freeStore;
	static const int screenChunk;

private:
	int m_i;
};

#endif //_PRE_CLASS_ALLOCATOR_1_H_