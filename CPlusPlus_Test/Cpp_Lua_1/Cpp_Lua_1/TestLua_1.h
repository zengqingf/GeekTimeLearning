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

namespace Textures
{
	enum ID
	{
		//Airplanes
		Airplane1 = 1,
		Airplane2 = 2,
		Airplane3 = 3,
		//Backgrounds
		Background1 = 100,
		Background2 = 101,
	};
}

class TestLua_1
{
public:
	void Test1();
	void Test2();
	void Test3();
};