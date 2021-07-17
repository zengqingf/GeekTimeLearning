
#include "pch.h"
#include "framework.h"
#include "LuaLib_2.h"
#include <stdio.h>


/*
@注意：
函数参数里的lua_State是私有的，每一个函数都有自己的栈。
当一个C/C++函数把返回值压入Lua栈以后，该栈会自动被清空。
*/

static int averageFunc(lua_State *L)
{
	int n = lua_gettop(L);
	double sum = 0;
	int i;
	/* 循环求参数之和 */
	for (i = 1; i <= n; i++)
		sum += lua_tonumber(L, i);

	lua_pushnumber(L, sum / n);     //压入平均值  
	lua_pushnumber(L, sum);         //压入和  

	return 2;                       //返回两个结果  
}

static int sayHelloFunc(lua_State *L)
{
	printf("hello world!\n");
	return 0;
}

static int sayHelloFuncWithUpValue(lua_State *L)
{
	int k = lua_tonumber(L, lua_upvalueindex(1));
	const char* another_upvalue = lua_tostring(L, lua_upvalueindex(2));
	printf("upvalue 1: is %d\n", k);
	printf("hello world with upvalue 2 : %s\n", another_upvalue);
	return 0;
}

static const struct luaL_Reg libFunc[] =
{
	{"average", averageFunc},
	{"sayHello", sayHelloFunc},
	{NULL, NULL}					//lua数组中最后一对必须是{NULL, NULL}，用来表示结束
};

static const struct luaL_Reg libFuncWithUpValue[] = 
{
	{"sayHelloUpValue", sayHelloFuncWithUpValue},
	{0,0}
};

LUALIB2_API int luaopen_LuaLib_2(lua_State *L)
{
	//"luacall为自定义字段"  lua那边调用时 luacall.average(111)
	//luaL_register(L, "luacall", myLib);	 // lua version < 502

	lua_newtable(L);  //这个不能省
	luaL_setfuncs(L, libFunc, 0);  //luaL_setfuncs 这个函数可以注册c函数到lua,另外还可以设置闭包函数使用的变量upvalue. 

	lua_pushnumber(L, 100);
	lua_pushstring(L, "this is upvalue");

	//第三个参数 nup 如果非零, 则所有通过luaL_setfuncs注册的函数都共享 nup个 upvalues. 
	//这些 upvalues 必须在注册之前 pushed 到栈上.
	luaL_setfuncs(L, libFuncWithUpValue, 2);

	//以下注册方法 lua层可以调用
	//lua_register(L, "average", averageFunc);
	//lua_register(L, "sayHello", sayHelloFunc);
	return 1;                        // 把myLib表压入了栈中，所以就需要返回1  


	/*
		#if LUA_VERSION_NUM >= 502
		lua_newtable(L);
		luaL_setfuncs(L, ao, 0);
		#else
		luaL_register(L, "ao", ao); // 5.1
		#endif
	*/
}