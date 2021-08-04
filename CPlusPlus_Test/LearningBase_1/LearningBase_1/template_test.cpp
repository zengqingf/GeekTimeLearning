#include "template_test.h"

double Account::m_rate = 8.0;                         //定义   （变量获得内存，对象范畴, static参数必须在class外做定义）

//注意: 下面这句理解错误了！！！
SingletonA SingletonA::a;							  //静态对象在cpp中定义 非声明  此时分配内存了  
													  //定义同时 静态对象创建时 会调用 构造函数
SingletonAA& SingletonAA::getInstance()
{
	static SingletonAA a; //自c以来的规范： 只有调用到这个函数   static a才创建
						  //第一次创建对象a时，会调用SingletonAA的构造函数
						  //后续调用getInstance()时，将不会再创建这个对象了
						  //同时，当SingletonAA的生命周期结束时，对象a会自动调用析构函数
						  //由于getInstance() 返回的是 &，所以不会调用拷贝构造函数了  (&引用 只会被赋值一次)
	return a;
}

SingletonB* SingletonB::instance = nullptr;    //定义 （定义性声明）
SingletonB* SingletonB::getInstance()
{
	if (nullptr == instance)
	{
		instance = new SingletonB();
	}
	return instance;
}

SingletonC* SingletonC::instance = nullptr;//定义（定义性声明）
SingletonC::CGarbo SingletonC::garbo_; //定义（定义性声明）
SingletonC* SingletonC::getInstance()
{
	if (instance == nullptr)
	{
		instance = new SingletonC();
	}
	return instance;
}


SingletonC1* SingletonC1::instance = nullptr;//定义（定义性声明）
SingletonC1::CGarbo SingletonC1::garbo_; //定义（定义性声明）
SingletonC1* SingletonC1::getInstance()
{
	if (instance == nullptr)
	{
		instance = new SingletonC1();
	}
	return instance;
}

SingletonCC1* SingletonCC1::instance_ = nullptr;//定义（定义性声明）
SingletonCC1::CGarbo SingletonCC1::garbo__; //定义（定义性声明）
SingletonCC1* SingletonCC1::getInstance()
{
	if (instance_ == nullptr)
	{
		instance_ = new SingletonCC1();
	}
	return instance_;
}


SingletonD* SingletonD::instance_ = nullptr;//定义（定义性声明）
SingletonD* SingletonD::getInstance() {
	/*if (instance_ == 0) {
		const char* singletonName = getenv("SINGLETON");
		//environment supplies this at startup
		instance_ = Lookup(singletonName);
		//Lookup returns 0 if there's no such singleton
	}*/
	return instance_;
}