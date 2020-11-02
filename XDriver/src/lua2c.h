#ifndef LUA2C_H
#define LUA2C_H

#include "uv.h"
extern "C"
{
#include "lua.h"
#include "lualib.h"
#include "lauxlib.h"
}

#include<iostream>
using Namespace std;

void loadLib(lua_State * L);

#ifdef LUA_BUILD_AS_DLL
extern "C"
{
	LUA_API void xdInit(lua_State* L);
	LUA_API void xdRegistFunc(lua_State* L);
	LUA_API void xdRunOnce();
}
#endif // LUA_BUILD_AS_DLL

#endif // !LUA2C_H

