#pragma once

/*
Object Oriented Programming   OOP
Object Oriented Design	      OOD

类和类之间的关系：大致分为三类    
Inheritance 继承
Composition 组合
Delegation  委托




### Composition 组合 表示 has-a

适用的设计模式：
Adapter

例: 
class Container {
public:
	Container();
	~Container();
protected:
	Component c1;
}

class Component {
public:
	Component();
	~Component();
}

<构造由内而外>
Container的构造函数先调用Component的default构造函数，然后再执行自己
Container::Container()  :  Component() {...};                            使用构造函数的初值列语法实现
其中 Component() 为编译器默认调用，默认不需要写
可以根据自己需求，手动写上不同的组件构造函数

<析构由外而内>
Container的析构函数首先执行自己，然后才调用Component的析构函数
Container::~Container() { ...  ~Component() }		



### Delegation 委托   Composition by reference （更形象，指针这类常说成reference）
用指针，连接两个对象

又名 聚合 

区别于组合：（没有组合紧密，生命周期不一样，组合的整体不存在则部分也不存在了，聚合的整体不存在但是部分仍然存在）

设计思想：  
pimpl  ==>  pointer to impl  （Handle  /  Body）
指针指向具体实现(Body)   整体提供外部接口（Handle）
编译防火墙：整体（Handle）不用编译

引用计数（reference counting）   ：实现共享字符串
a -->
b ->  n (reference counting) -> Hello   
c -->

共享的时候 需要考虑 改动不影响其他
引出一个概念：copy on write （拷贝一份出来用于修改）




### Inheritance 继承 表示 is-a
C++ 有三种继承

例：
struct _List_node_base
{
	_List_node_base* _M_next;
	_List_node_base* _M_prev;
}

template<typename _Tp>
struct _List_node
	: public _List_node_base  // : private _List_node_base  // : protected _List_node_base
{
	_Tp _M_data;
}


Base   空心箭头 <1----   Derived

<构造由内而外>
Derived的构造函数首先调用Base的default的构造函数，然后才执行自己
Derived::Derived() : Base() {};                                   ----->  :Base() 是编译器默认会调用的  如果不需要执行父类其他构造函数 不需要写出来

<析构由外而内>
Derived的析构函数首先执行自己，然后才调用Base的析构函数
Derived::~Derived() {... ~Base()};

@注意：  Base class的dtor 必须是 virtual, 否则会出现 undefined behavior



*/