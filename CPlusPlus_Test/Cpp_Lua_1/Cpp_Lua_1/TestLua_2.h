#pragma once

#include <stdio.h>
extern "C"
{
#include "Lua/include/lua.h"
#include "Lua/include/lauxlib.h"
#include "Lua/include/lualib.h"
}


//定义在.h中可能会报错：重复定义
//只要两个或者以上的.cpp同时包含了一个定义了变量的.h文件，就会提示错误
//别在.h中定义变量，或者初始化静态成员变量
/*指向lua解释器的指针*/
//lua_State* gL;  ==>次数为定义

static int average(lua_State *L)
{
	/*得到参数个数*/
	int n = lua_gettop(L);

	double sum = 0;
	int i;
	for (i = 1; i <= n; i++)
	{
		sum += lua_tonumber(L, i);
	}

	//压入平均值
	lua_pushnumber(L, sum / n);
	//压入和
	lua_pushnumber(L, sum);

	//返回 返回值 的个数
	return 2;
}

void TestLua_2();