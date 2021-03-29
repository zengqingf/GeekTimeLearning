#include "template_test.h"

double Account::m_rate = 8.0;                         //定义   （变量获得内存，对象范畴, static参数必须在class外做定义）

SingletonA SingletonA::a;							  //静态对象在cpp中定义 非声明  此时分配内存了  
													  //定义同时 静态对象创建时 会调用 构造函数
SingletonAA& SingletonAA::getInstance()
{
	static SingletonAA a; //自c以来的规范： 只有调用到这个函数   static a才创建
	return a;
}