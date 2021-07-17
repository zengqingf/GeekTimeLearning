#include "TestLua_2.h"

//定义了 因为是struct
lua_State* gL;
void TestLua_2()
{
	//初始化lua
	gL = luaL_newstate();   //In Lua 5.1 lua_open is just a macros     #define lua_open()  luaL_newstate()

	//载入Lua基本库
	luaL_openlibs(gL);

	//注册函数，函数必须为static
	lua_register(gL, "average", average);

	//运行lua脚本
	luaL_dofile(gL, "LuaDemo/my_avg_1.lua");

	//清楚lua
	lua_close(gL);

	printf("press enter to exit...");
	getchar();
}