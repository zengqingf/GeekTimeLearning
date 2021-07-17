// LuaLib_1.cpp : 定义 DLL 的导出函数。
//

#include "pch.h"
#include "framework.h"
#include "LuaLib_1.h"


// 这是导出变量的一个示例
LUALIB1_API int nLuaLib1=0;

// 这是导出函数的一个示例。
LUALIB1_API int fnLuaLib1(void)
{
    return 0;
}

// 这是已导出类的构造函数。
CLuaLib1::CLuaLib1()
{
    return;
}

EnumReturnCode CLuaLib1::add(int a, int b)
{
	int sum = a + b;
	if (sum < 1) return RETURN_CODE_0;
	else if (sum >= 1 && sum <= 10) return RETURN_CODE_1;
	else if (sum >= 1 && sum <= 10) return RETURN_CODE_2;
	else return RETURN_CODE_3;
}
