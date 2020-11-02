#ifndef C2LUA_H
#define C2LUA_H

extern "C"
{
#include "lua.h"
#include "lualib.h"
#include "lauxlib.h"
}

#include<iostream>
using Namespace std;

enum cbEnum {
	lcb_errorCb,
	lcb_listenCb,
	lcb_listenErrorCb,
	lcb_dataCb,
	lcb_readErrorCb,
	lcb_endCb,
	lcb_connectCb,
	lcb_connectErrorCb,
	lcb_connectCloseCb,
	lcb_timerCb,
	lcb_size,
};

void initCallback(lua_State * L);

void cbcall(lua_State* L, int cbFuncId, int id);
void cbcall(lua_State* L, int cbFuncId, int id, int args);
void cbcall(lua_State* L, int cbFuncId, int id, const char* args, int size);
void cbpush(lua_State * L, int cbFuncId, int id);
void cbrun(lua_State * L, int nargs);

#endif // !C2LUA_H