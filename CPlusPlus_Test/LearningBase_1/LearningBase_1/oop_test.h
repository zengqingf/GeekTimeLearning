#pragma once

/*
Object Oriented Programming   OOP
Object Oriented Design	      OOD

类和类之间的关系：大致分为三类    

###############################################################################
							Inheritance 继承
							Composition 组合
							Delegation  委托
###############################################################################

### Composition 组合 表示 has-a

						适用的设计模式：《Adapter》
									
STL示例：
从内存角度（内存大小）

template <class T>
class queue {
protected:
	deque<T> c;						//sizeof : 40
	...
};

template <class T>
class deque {
protected:
	Itr<T> start;					//sizeof : 16 * 2 + 4 + 4
	Itr<T> finish;
	T**    map;
	unsigned int map_size;
};

template <class T>
struct Itr {
	T* cur;							//sizeof: 4 * 4
	T* first;
	T* last;
	T** node;
};


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



示例：
//file String.h
class StringRep;
class String {								//字符串对外接口（Handle）
public:
	String();
	String(const char* s);
	String(const String& s);
	String &operator=(const String& s);
	~String();
...
private:
	StringRep* rep;							//piml   pointer to implement   (Handle  /  Body)
};

//file: String.cpp
#include "String.h"
namespace {
	class StringRep {                       //字符串实现（Body）
		friend class String;
		StringRep(const char* s);
		~StringRep(); 
		int count;
		char* rep;
	};
}
String::String()  {...}
...





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
	: public _List_node_base  // : private _List_node_base  // : protected _List_node_base	//不同于C# Java 默认是public继承
{
	_Tp _M_data;
}


Base   空心箭头 <1----   Derived

<构造由内而外>   @注意：这里的内和外可以通过画图表示清楚：Derived(外)包含了Base(内) 
Derived的构造函数首先调用Base的default的构造函数，然后才执行自己
Derived::Derived() : Base() {};                                   ----->  :Base() 是编译器默认会调用的  如果不需要执行父类其他构造函数 不需要写出来

<析构由外而内>
Derived的析构函数首先执行自己，然后才调用Base的析构函数
Derived::~Derived() {... ~Base()};

@注意：  Base class的dtor 必须是 virtual, 否则会出现 undefined behavior

*/


/*
虚函数  virtual       +    继承

non-virtaul函数			 derived class 不重新定义(override  重载)

virtual函数				 derived class 重新定义，已有默认定义

pure virtual函数		 derived class 一定要重新定义 
*/

#include <string>
class Shape {

public:
	virtual void draw() const = 0;						//pure virtual
	virtual void error(const std::string& msg);		    //impure virtual
	int objectID() const;								//non-virtual
	//...
};

class Rectangle :public Shape {};
class Ellipse : public Shape {};


//Application Framework
class CDocument {
public:
	void OnFileOpen() 
	{ 
		//...
		Serialize();						// 实际上调用 是 this->Serialize();   this是隐藏传入的 &myDoc  所以会调用到 derived class override的函数
		//...
	}		

	virtual void Serialize() {};			//这种设计被称为 《Template Method 模板方法》  不同于C++的模板方法
};

//Application
class CMyDoc : public CDocument {
	virtual void Serialize() 
	{
		//只有application才知道如何读取自己的文件(格式)
	}
};

/*
//模拟调用
int main() {
	CMyDoc myDoc;
	myDoc.OnFileOpen();  //==>  实际执行的是  CDocument::OnFileOpen(&myDoc);  其中 &myDoc是隐藏的
};
*/



/*
Inheritance + Composition  构造和析构的情况分析
*/


class OOP_Base_1 {
public:
	OOP_Base_1();
	~OOP_Base_1();
};

class OOP_DerivedPart_1 {
public:
	OOP_DerivedPart_1();
	~OOP_DerivedPart_1();
};

class OOP_Derived_1 : public OOP_Base_1 {
public:
	OOP_Derived_1();
	~OOP_Derived_1();
	
protected:
	OOP_DerivedPart_1 oop_dp_1;
};

class OOP_BasePart_2 {
public:
	OOP_BasePart_2();
	~OOP_BasePart_2();
};

class OOP_Base_2 {
public:
	OOP_Base_2();
	~OOP_Base_2();

protected:
	OOP_BasePart_2 oop_bp_2;
};

class OOP_Derived_2 : public OOP_Base_2 {
public:
	OOP_Derived_2();
	~OOP_Derived_2();
};



/************************************************************************************/

/*** Delegation 委托 + Inheritance 继承 ***/


//《观察者模式》

class Subject;
class Observer
{
public:
	virtual void update(Subject* sub, int value) = 0;
};

#include<vector>
using namespace std;
class Subject
{
	int m_value;
	vector<Observer*> m_views;
public:
	void attach(Observer* obs)
	{
		m_views.push_back(obs);
	}

	void deAttachAll()
	{
		m_views.clear();
	}

	void set_val(int value)
	{
		m_value = value;
		notify();
	}

	void notify()
	{
		for (int i = 0; i < m_views.size(); ++i)
		{
			m_views[i]->update(this, m_value);
		}
	}
};



class Component
{
	int value;
public:
	Component(int val) { value = val; }
	virtual void add(Component* comp) {}					//这里不能设计成纯虚函数
};

class Composite :public Component
{
	vector<Component*> c;
public:
	Composite(int val) :Component(val) {}
	void add(Component* elem) {
		c.push_back(elem);
	}
};

class Primitive :public Component 
{
public:
	Primitive(int val) : Component(val) {}
};



//《Prototype  原型模式》
//创建未知class对象  自己创建自己

#include <iostream>
enum  imageType
{
	LSAT, SPOT
};

//父类
class Image
{
public:
	virtual void draw() = 0;
	static Image* findAndClone(imageType);
protected:
	virtual imageType returnType() = 0;
	virtual Image* clone() = 0;

	//As each subclass of Image is declared, it registers its prototype
	static void addPrototype(Image* image)
	{
		_prototypes[_nextSlot++] = image;
	}

private:
	//addPrototype() saves each registered prototype here
	static Image* _prototypes[10];
	static int _nextSlot;
};


//子类1
class LandSatImage : public Image
{
public:
	imageType returnType() {
		return LSAT;
	}
	void draw() {
		cout << "LandSatImage::draw" << _id << endl;
	}
	//When clone() is called, call the one-argument ctor with a dummy arg
	Image* clone() {
		return new LandSatImage(1);
	}
protected:
	//This is only called from clone()
	LandSatImage(int dummy) {  //虚拟参数 为了和默认构造函数区分
		_id = _count++;
	}


private:
	//Mechanism for initializing an Image subclass - this causes the
	//default ctor to be called, which registers the subclass's prototype
	static LandSatImage _landSatImage;
	//This is only called when the private static data member is inited
	LandSatImage() {
		addPrototype(this);
	}
	int _id;
	static int _count;
};

/*
应用：
const int NUM_IMAGES = 8;
imageType input[NUM_IMAGES] =
{
	LSAT,LSAT,LSAT,LSAT,SPOT,SPOT,SPOT,LSAT
};
int main()
{
	Image *images[NUM_IMAGES];
	for (int i = 0; i < NUM_IMAGES; ++i)
	{
		images[i] = Image::findAndClone(input[i]);
	}
	for (int i = 0; i < NUM_IMAGES; ++i)
	{
		images[i]->draw();
	}
	for (int i = 0; i < NUM_IMAGES; ++i)
	{
		delete images[i];
	}
}*/