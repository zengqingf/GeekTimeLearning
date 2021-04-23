#pragma once

/*
工厂方法模式建议使用特殊的工厂方法代替对于对象构造函数的直接调用 （即使用 new运算符）。 
对象仍将通过 new运算符创建， 只是该运算符改在工厂方法中调用罢了。 
工厂方法返回的对象通常被称作 “产品”。
*/

#include <string>
using namespace std;