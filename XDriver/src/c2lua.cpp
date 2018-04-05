
#include "c2lua.h"

struct CBFunc {
	int id;
	char* name;
};

struct CBFunc cbFunc[] = {
	{ lcb_errorCb ,			"errorCb" },
	{ lcb_listenCb ,		"listenCb" },
	{ lcb_listenErrorCb ,	"listenErrorCb" },
	{ lcb_dataCb,			"dataCb"},
	{ lcb_readErrorCb,	    "readErrorCb" },
	{ lcb_endCb,			"endCb"},
	{ lcb_connectCb ,		"connectCb"},
	{ lcb_connectErrorCb ,	"connectErrorCb" },
	{ lcb_connectCloseCb ,	"connectCloseCb" },
	{ lcb_timerCb ,			"timerCb" },
};

int cbFuncLuaRef[lcb_size];

void initCallback(lua_State * L)
{
	lua_getglobal(L,"xdcb");
	for (int i = 0; i < lcb_size; i++) {
		lua_getfield(L,-1,cbFunc[i].name);
		cbFuncLuaRef[cbFunc[i].id] = luaL_ref(L, LUA_REGISTRYINDEX);
	}
}

void cbcall(lua_State * L, int cbFuncId, int id)
{
	cbpush(L,cbFuncId,id);
	cbrun(L,1);
}

void cbcall(lua_State * L, int cbFuncId, int id, int args)
{
	cbpush(L, cbFuncId, id);
	lua_pushinteger(L, args);
	cbrun(L, 2);
}

void cbcall(lua_State * L, int cbFuncId, int id, const char * str,int len)
{
	cbpush(L, cbFuncId, id);
	lua_pushlstring(L,str,len);
	cbrun(L, 2);
}

void cbpush(lua_State * L, int cbFuncId, int id)
{
	lua_rawgeti(L, LUA_REGISTRYINDEX, cbFuncLuaRef[cbFuncId]);
	lua_pushinteger(L, id);
}

void cbrun(lua_State * L, int nargs)
{
	int ret = lua_pcall(L, nargs, 0, 0);
	if (ret != 0) {
		printf("cbcall ERROR:ret = %d,err=%s\n", ret, lua_tostring(L,-1));
	}
}
