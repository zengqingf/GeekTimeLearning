#pragma once

#include <iostream>
#include <string>

extern "C"
{
#include "Lua/include/lua.h"
#include "Lua/include/lauxlib.h"
#include "Lua/include/lualib.h"
	//#pragma comment(lib, "Lua/lib/Win64/Lua.lib")
}

class TestLua_1
{
public:
	void Test1();
	void Test2();
};