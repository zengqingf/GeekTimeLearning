#pragma once

class Base_1
{
public:
	static int* array_int;
};



/*
在重载父类的同名虚函数时会出现hides overloaded virtual function编译警告。

重载的虚函数被隐藏了

首先是发生了重载，子类重载了父类的函数，其次被重载的是虚函数，这时这个被重载的父类的虚函数将会被隐藏。
*/

struct Base
{
	virtual void* get(const char* e);
	//virtual void * get(char* e, int index);
};

struct Derived: public Base
{
	using Base::get;
	virtual void* get(const char* e, int index);

//放到私有位置 避免调用Base的get
//private:
//	using Base::get;
};