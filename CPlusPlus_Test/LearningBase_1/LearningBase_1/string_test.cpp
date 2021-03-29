#define _CRT_SECURE_NO_WARNINGS

#include <iostream>
#include "string_test.h"
using namespace std;

ostream& 
operator << (ostream& os, const String& str)
{
	//cout 能接收到 c string pointer 并打印string出来
	os << str.get_c_str();
	return os;
}

