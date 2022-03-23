#pragma once

#define UWA_SLUA_PROFILE 0

#include "CoreMinimal.h"
#include "PlatformManager.h"
#include "UWALib/Public/LuaProfiler.h"

#if UWA_SLUA_PROFILE
#include "LuaState.h"
#include "lua.h"

#define LUA_VALUATE(lua_native_method) reg_##lua_native_method((lua_native_method##Func)&slua::lua_native_method);
#endif

class UWAGOT_API LuaManager
{
public:
	LuaManager();
	~LuaManager();
	static void InitLuaEnvDump();
	static void UpdateLuaData(int FrameNum);
	static void EndLuaTest();
};