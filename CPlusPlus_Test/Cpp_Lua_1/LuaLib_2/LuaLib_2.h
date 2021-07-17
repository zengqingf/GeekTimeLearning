#pragma once

extern "C"
{
#include "Lua/include/lua.h"
#include "Lua/include/lauxlib.h"
#include "Lua/include/lualib.h"
}

#ifdef LUALIB2_EXPORTS
#define LUALIB2_API __declspec(dllexport)
#else
#define LUALIB2_API __declspec(dllimport)
#endif

//定义lua导出函数
//luaopen_xxx 后的名称 xxx和项目名一致（因为项目名和dll一样）
extern "C" LUALIB2_API int luaopen_LuaLib_2(lua_State *L);