#include "../Public/LuaManager.h"

LuaManager::LuaManager()
{

}

LuaManager::~LuaManager()
{

}

void LuaManager::InitLuaEnvDump() {
#if UWA_SLUA_PROFILE
	LUA_VALUATE(lua_sethook)
	LUA_VALUATE(lua_getinfo)
	LUA_VALUATE(lua_getstack)
	LUA_VALUATE(lua_gc)
	LUA_VALUATE(lua_type)
	LUA_VALUATE(lua_pushboolean)
	LUA_VALUATE(lua_topointer)
	LUA_VALUATE(lua_pushstring)
	LUA_VALUATE(lua_tonumberx)
	LUA_VALUATE(lua_toboolean)
	LUA_VALUATE(lua_typename)
	LUA_VALUATE(lua_getmetatable)
	LUA_VALUATE(lua_rawget)
	LUA_VALUATE(lua_isstring)
	LUA_VALUATE(lua_pushnil)
	LUA_VALUATE(lua_next)
	LUA_VALUATE(lua_pushvalue)
	LUA_VALUATE(lua_getupvalue)
	LUA_VALUATE(lua_iscfunction)
	LUA_VALUATE(luaL_buffinit)
	LUA_VALUATE(luaL_addstring)
	LUA_VALUATE(luaL_pushresult)
	LUA_VALUATE(lua_tothread)
	LUA_VALUATE(lua_gettop)
	LUA_VALUATE(lua_getlocal)
	//LUA_VALUATE(luaL_addlstring)
	LUA_VALUATE(lua_touserdata)
	LUA_VALUATE(lua_tolstring)
	LUA_VALUATE(luaL_checklstring)
	LUA_VALUATE(lua_settop)
	LUA_VALUATE(lua_createtable)
	LUA_VALUATE(luaL_newstate)
	LUA_VALUATE(lua_pushlstring)
	LUA_VALUATE(luaL_checkstack)
	//LUA_VALUATE(luaL_prepbuffer)
	LUA_VALUATE(lua_close)
	LUA_VALUATE(lua_pushcclosure)
	LUA_VALUATE(lua_pushthread)
	LUA_VALUATE(luaL_error)
	LUA_VALUATE(lua_setfield)
	LUA_VALUATE(lua_pushlightuserdata)
	LUA_VALUATE(lua_rawset)

	LUA_VALUATE(lua_rotate)
	LUA_VALUATE(luaL_checkversion_)
	LUA_VALUATE(lua_rawsetp)
	LUA_VALUATE(lua_rawgetp)
	LUA_VALUATE(lua_getuservalue)

	LUA_VALUATE(luaL_callmeta)
	//LUA_VALUATE(lua_tointegerx)
	LUA_VALUATE(lua_pushfstring)
	LUA_VALUATE(lua_rawequal)
	LUA_VALUATE(lua_rawgeti)
	//LUA_VALUATE(lua_rawgeti_int)

	LUA_VALUATE(luaL_ref)
	LUA_VALUATE(luaL_unref)

	auto state = slua::LuaState::get();
	__lua_State* mstate = (__lua_State*)(state->getLuaState());
	StartLuaTest(TCHAR_TO_ANSI(*FPlatformManager::Get().GetCurrentDataDirectory()));
#endif
}

void LuaManager::UpdateLuaData(int FrameNum)
{
#if UWA_SLUA_PROFILE
	auto state = slua::LuaState::get();
	FString fileName = FPlatformManager::Get().GetCurrentDataDirectory() + FString::FromInt(FrameNum);
	LuaDoUpdate(FrameNum, (__lua_State*)state->getLuaState(), TCHAR_TO_ANSI(*fileName), LUA_REGISTRYINDEX);
#endif
}

void LuaManager::EndLuaTest()
{
#if UWA_SLUA_PROFILE
	StopLua();
#endif
}