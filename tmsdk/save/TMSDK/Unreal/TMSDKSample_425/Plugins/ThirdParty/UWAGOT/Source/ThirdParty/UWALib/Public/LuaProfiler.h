// Fill out your copyright notice in the Description page of Project Settings.
#pragma once
#include "uwa.h"
#include "string.h"
#include "set"
#include "map"
#include "list"

using std::string;
using std::set;
using std::map;
using std::list;

//#define LUA_REGISTRYINDEX	(-10000)

#define TABLE 1
#define FUNCTION 2
#define SOURCE 3
#define THREAD 4
#define USERDATA 5
#define MARK 6

// Lua GC Options
#define LUA_GCSTOP			0
#define LUA_GCRESTART		1
#define LUA_GCCOLLECT		2
#define LUA_GCCOUNT			3
#define LUA_GCCOUNTB		4
#define LUA_GCSTEP			5
#define LUA_GCSETPAUSE		6
#define LUA_GCSETSTEPMUL	7

//LUA_HOOKÖ´ÐÐÊÂ¼þ
#define LUA_HOOKCALL	0
#define LUA_HOOKRET		1
#define LUA_HOOKLINE	2
#define LUA_HOOKCOUNT	3
#define LUA_HOOKTAILRET 4

// Event masks
#define LUA_MASKCALL	(1 << LUA_HOOKCALL)
#define LUA_MASKRET		(1 << LUA_HOOKRET)
#define LUA_MASKLINE	(1 << LUA_HOOKLINE)
#define LUA_MASKCOUNT	(1 << LUA_HOOKCOUNT)

#define LUA_TNONE		(-1)
#define LUA_TNIL			0
#define LUA_TBOOLEAN		1
#define LUA_TLIGHTUSERDATA	2
#define LUA_TNUMBER			3
#define LUA_TSTRING			4
#define LUA_TTABLE			5
#define LUA_TFUNCTION		6
#define LUA_TUSERDATA		7
#define LUA_TTHREAD			8

#define LUA_MINSTACK		20
#define LUA_MAXSTACK		3000
#define __LUAL_BUFFERSIZE	(BUFSIZ > 16384 ? 8192 : BUFSIZ)

#define LUA_NUMBER	double
typedef LUA_NUMBER __lua_Number;

#define LUA_DECLARE(lua_native_method) \
	DLL_API void reg_##lua_native_method(lua_native_method##Func f);

struct _lua_State {
	char dummy[10];
};
typedef struct _lua_State __lua_State;

struct _luaL_Buffer {
	char *p;			/* current position in buffer */
	int lvl;  /* number of strings in the stack (level) */
	__lua_State *L;
	char buffer[__LUAL_BUFFERSIZE];
};
typedef struct _luaL_Buffer __luaL_Buffer;

#define __LUA_IDSIZE	256	/* Size of lua_Debug.short_src. */

struct _lua_Debug {
	int event;
	const char *name;	/* (n) */
	const char *namewhat;	/* (n) `global', `local', `field', `method' */
	const char *what;	/* (S) `Lua', `C', `main', `tail' */
	const char *source;	/* (S) */
	int currentline;	/* (l) */
	int nups;		/* (u) number of upvalues */
	int linedefined;	/* (S) */
	int lastlinedefined;	/* (S) */
	char short_src[__LUA_IDSIZE]; /* (S) */
	/* private part */
	int i_ci;  /* active function */
};
typedef struct _lua_Debug __lua_Debug;

typedef void(*lua_Hook) (__lua_State *L, __lua_Debug *ar);
typedef int(*lua_sethookFunc)(__lua_State *L, lua_Hook func, int mask, int count);
typedef int(*lua_CFunction) (__lua_State *L);
typedef int(*lua_getinfoFunc)(__lua_State *L, const char *what, __lua_Debug *ar);
typedef int(*lua_getstackFunc)(__lua_State *L, int level, __lua_Debug *ar);
typedef int(*lua_gcFunc)(__lua_State *L, int what, int data);
typedef void(*luaL_checkversionFunc)(__lua_State *L);
typedef int(*lua_typeFunc)(__lua_State *L, int idx);
typedef void(*lua_rawsetpFunc)(__lua_State *L, int idx, const void *p);
typedef void(*lua_rawgetpFunc)(__lua_State *L, int idx, const void *p);
typedef void(*lua_getuservalueFunc)(__lua_State *L, int idx);
typedef void(*lua_pushbooleanFunc)(__lua_State *L, int b);
typedef const void* (*lua_topointerFunc)(__lua_State *L, int idx);
typedef void(*lua_pushstringFunc)(__lua_State *L, const char *s);
typedef double(*lua_tonumberFunc) (__lua_State *L, int idx);
typedef double(*lua_tonumberxFunc) (__lua_State *L, int idx, void *x);
typedef int(*lua_tobooleanFunc)(__lua_State *L, int idx);
typedef const char*(*lua_typenameFunc)(__lua_State *L, int tp);
typedef int(*lua_getmetatableFunc)(__lua_State *L, int objindex);
typedef void(*lua_rawgetFunc)(__lua_State *L, int idx);
typedef int(*lua_isstringFunc)(__lua_State *L, int idx);
typedef void(*lua_pushnilFunc)(__lua_State *L);
typedef int(*lua_nextFunc)(__lua_State *L, int idx);
typedef void(*lua_pushvalueFunc)(__lua_State *L, int idx);
typedef const char *(*lua_getupvalueFunc)(__lua_State *L, int funcindex, int n);
typedef int(*lua_iscfunctionFunc)(__lua_State *L, int idx);
typedef void(*luaL_buffinitFunc)(__lua_State *L, __luaL_Buffer *B);
typedef void(*luaL_addstringFunc)(__luaL_Buffer *B, const char *s);
typedef void(*luaL_pushresultFunc)(__luaL_Buffer *B);
typedef __lua_State *(*lua_tothreadFunc)(__lua_State *L, int idx);
typedef int(*lua_gettopFunc)(__lua_State *L);
typedef const char *(*lua_getlocalFunc)(__lua_State *L, const __lua_Debug *ar, int n);
typedef void*(*lua_touserdataFunc)(__lua_State *L, int idx);
typedef const char*(*lua_tolstringFunc)(__lua_State *L, int idx, size_t *len);
typedef const char *(*luaL_checklstringFunc)(__lua_State *L, int numArg, size_t *l);
typedef void(*lua_settopFunc)(__lua_State *L, int idx);
typedef void(*lua_createtableFunc)(__lua_State *L, int narr, int nrec);
typedef __lua_State *(*luaL_newstateFunc)(void);
typedef void(*lua_pushlstringFunc)(__lua_State *L, const char *s, size_t l);
typedef void(*luaL_checkstackFunc)(__lua_State *L, int sz, const char *msg);
typedef void(*lua_closeFunc)(__lua_State *L);
typedef void(*lua_pushcclosureFunc)(__lua_State *L, lua_CFunction fn, int n);
typedef int(*lua_pushthreadFunc)(__lua_State *L);
typedef int(*luaL_errorFunc)(__lua_State *L, const char *fmt, ...);
typedef void(*lua_setfieldFunc)(__lua_State *L, int idx, const char *k);
typedef void(*lua_pushlightuserdataFunc)(__lua_State *L, void *p);
typedef void(*lua_insertFunc)(__lua_State *L, int idx);
typedef void(*lua_rawsetFunc)(__lua_State *L, int idx);
typedef int(*luaL_callmetaFunc)(__lua_State *L, int obj, const char *e);
typedef const char*(*lua_pushfstringFunc)(__lua_State *L, const char *fmt, ...);
typedef int(*lua_rawequalFunc)(__lua_State *L, int idx1, int idx2);
typedef void(*lua_rawgetiFunc)(__lua_State *L, int idx, int n);

typedef int(*luaL_refFunc)(__lua_State *L, int t);
typedef void(*luaL_unrefFunc)(__lua_State *L, int t, int ref);
typedef void(*lua_rotateFunc)(__lua_State *L, int idx, int n);
typedef void(*luaL_checkversion_Func)(__lua_State *L, __lua_Number ver, size_t sz);

LUA_DECLARE(lua_sethook)
LUA_DECLARE(lua_getinfo)
LUA_DECLARE(lua_getstack)
LUA_DECLARE(lua_gc)
LUA_DECLARE(lua_type)
LUA_DECLARE(lua_pushboolean)
LUA_DECLARE(lua_topointer)
LUA_DECLARE(lua_pushstring)
LUA_DECLARE(lua_tonumberx)
LUA_DECLARE(lua_toboolean)
LUA_DECLARE(lua_typename)
LUA_DECLARE(lua_getmetatable)
LUA_DECLARE(lua_rawget)
LUA_DECLARE(lua_isstring)
LUA_DECLARE(lua_pushnil)
LUA_DECLARE(lua_next)
LUA_DECLARE(lua_pushvalue)
LUA_DECLARE(lua_getupvalue)
LUA_DECLARE(lua_iscfunction)
LUA_DECLARE(luaL_buffinit)
LUA_DECLARE(luaL_addstring)
LUA_DECLARE(luaL_pushresult)
LUA_DECLARE(lua_tothread)
LUA_DECLARE(lua_gettop)
LUA_DECLARE(lua_getlocal)
LUA_DECLARE(lua_touserdata)
LUA_DECLARE(lua_tolstring)
LUA_DECLARE(luaL_checklstring)
LUA_DECLARE(lua_settop)
LUA_DECLARE(lua_createtable)
LUA_DECLARE(luaL_newstate)
LUA_DECLARE(lua_pushlstring)
LUA_DECLARE(luaL_checkstack)
LUA_DECLARE(lua_close)
LUA_DECLARE(lua_pushcclosure)
LUA_DECLARE(lua_pushthread)
LUA_DECLARE(luaL_error)
LUA_DECLARE(lua_setfield)
LUA_DECLARE(lua_pushlightuserdata)
LUA_DECLARE(lua_insert)
LUA_DECLARE(lua_rawset)
LUA_DECLARE(luaL_checkversion)
LUA_DECLARE(lua_rawsetp)
LUA_DECLARE(lua_rawgetp)
LUA_DECLARE(lua_getuservalue)
LUA_DECLARE(luaL_callmeta)
LUA_DECLARE(lua_pushfstring)
LUA_DECLARE(lua_rawequal)
LUA_DECLARE(lua_rawgeti)

LUA_DECLARE(luaL_ref)
LUA_DECLARE(luaL_unref)
LUA_DECLARE(lua_rotate)
LUA_DECLARE(luaL_checkversion_)

void mark_object(__lua_State *L, const void * parent, const char *desc);
void mark_table(__lua_State * L, const void * parent, const char * desc);
void mark_userdata(__lua_State *L, const void * parent, const char *desc);
void mark_function(__lua_State *L, const void * parent, const char *desc);
void mark_thread(__lua_State *L, const void * parent, const char *desc);
const void * readobject(__lua_State *L, const void *parent, const char *desc);
const char * keystring(__lua_State *L, int index, char * buffer);

//extern void LuaStop();
//extern void LuaClearStack();
//extern bool lua_setmap_path(const char* obj_idmap_path, const char* lua_keymap_path);

extern void StartLuaOn(const char* Finalpath);
DLL_API void StopLua();
DLL_API void LuaDoUpdate(int id, __lua_State * Main, const char* Finalpath, int Rtable);


