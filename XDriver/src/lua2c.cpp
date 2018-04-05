
#include "lua2c.h"
#include "c2lua.h"
#include "socket.h"
#include "driver.h"
#include "proto.h"

#ifdef LUA_BUILD_AS_DLL
#define LUA_API __declspec(dllexport)
#endif // LUA_BUILD_AS_DLL

static int xd_createListener(lua_State* L) {
	uint32_t id = (uint32_t)lua_tointeger(L, 1);
	XDriver& driver = XDriver::getInstance();
	XListener* listener = new XListener;
	listener->setLoop(driver.getLoop());
	listener->setLuaState(L);
	listener->setId(id);
	lua_pushlightuserdata(L, listener);
	return 1;
}

static int xd_releaseListener(lua_State* L) {
	XListener* listener = (XListener *)lua_touserdata(L, 1);
	delete listener;
	return 0;
}

static int xd_createStream(lua_State* L) {
	uint32_t id = (uint32_t)lua_tointeger(L, 1);
	XDriver& driver = XDriver::getInstance();
	XStream* stream = new XStream;
	stream->setLuaState(L);
	stream->setId(id);
	lua_pushlightuserdata(L, stream);
	return 1;
}

static int xd_releaseStream(lua_State* L) {
	XStream* stream = (XStream *)lua_touserdata(L, 1);
	delete stream;
	return 0;
}

static int xd_createConnector(lua_State* L) {
	uint32_t id = (uint32_t)lua_tointeger(L, 1);
	XStream* stream = (XStream *)lua_touserdata(L, 2);
	XDriver& driver = XDriver::getInstance();
	XConnector* conn = new XConnector(stream);
	conn->setId(id);
	conn->setLoop(driver.getLoop());
	conn->setLuaState(L);
	lua_pushlightuserdata(L, conn);
	return 1;
}

static int xd_releaseConnector(lua_State* L) {
	XConnector* conn = (XConnector *)lua_touserdata(L, 1);
	delete conn;
	return 0;
}

static int xd_listen(lua_State* L) {
	XListener* listener = (XListener*)lua_touserdata(L, 1);
	const char* ip = lua_tostring(L, 2);
	int port = (int)lua_tointeger(L, 3);
	printf("listen:ip=%s,port=%d\n", ip, port);
	int r = listener->listen(ip, port);
	lua_pushinteger(L, r);
	return 1;
}

static int xd_accept(lua_State* L) {
	XListener* listener = (XListener*)lua_touserdata(L, 1);
	XStream* stream = (XStream *)lua_touserdata(L, 2);
	int r = listener->accept(stream);
	lua_pushinteger(L, r);
	return 1;
}

static int xd_connect(lua_State* L) {
	XConnector* conn = (XConnector*)lua_touserdata(L, 1);
	const char* ip = lua_tostring(L, 2);
	int port = (int)lua_tointeger(L, 3);
	conn->connect(ip, port);
	return 0;
}

static int xd_readStart(lua_State* L) {
	XStream* stream = (XStream *)lua_touserdata(L, 1);
	int r = stream->readStart();
	lua_pushinteger(L, r);
	return 1;
}

static int xd_readStop(lua_State* L) {
	XStream* stream = (XStream *)lua_touserdata(L, 1);
	int r = stream->readStop();
	lua_pushinteger(L, r);
	return 1;
}

static int xd_sendString(lua_State* L) {
	XStream* stream = (XStream *)lua_touserdata(L, 1);
	const char* msg = lua_tostring(L, 2);
	int len = (int)lua_tointeger(L, 3);
	stream->write((char*)msg, len);
	return 0;
}

static int xd_sendData(lua_State* L) {
	XStream* stream = (XStream *)lua_touserdata(L, 1);
	uint16_t protoId = (uint16_t)lua_tointeger(L, 2);
	//int test1 = lua_tointeger(L, 3);
	//int test2 = lua_tointeger(L, 4);
	//int test3 = lua_tointeger(L, 5);
	//XProto* proto = stream->sender->protos[protoId];
	//printf("send:%s,%d,%d,%d\n", proto->format, test1, test2, test3);
	stream->sender->writeProto(L, 3, protoId);

	return 1;
}

static int xd_createProtoGroup(lua_State* L) {
	uint32_t id = (int)luaL_checkinteger(L, 1);
	cout << "create pg:" << id << endl;
	XProtoGroup* pg = new XProtoGroup;
	pg->id = id;
	lua_pushlightuserdata(L, (void *)pg);
	return 1;
}

static int xd_registerSender(lua_State* L) {
	XStream* stream = (XStream *)lua_touserdata(L, 1);
	XProtoGroup* pg = (XProtoGroup *)lua_touserdata(L, 2);
	stream->setSender(pg);
	pg->setStream(stream);
	return 0;
}

static int xd_registerReceiver(lua_State* L) {
	XStream* stream = (XStream*)lua_touserdata(L, 1);
	XProtoGroup* pg = (XProtoGroup*)lua_touserdata(L, 2);
	stream->setReader(pg);
	pg->setStream(stream);
	return 0;
}

static int xd_registerProto(lua_State* L) {
	XProtoGroup* pg = (XProtoGroup *)lua_touserdata(L, 1);
	if (pg == nullptr) {
		return 0;
	}
	const char* format = lua_tostring(L, 2);
	uint32_t id = pg->registerProto(format);
	lua_pushinteger(L, id);
	return 1;
}

static int xd_initCallback(lua_State* L) {
	initCallback(L);
	return 0;
}

static int xd_errorName(lua_State* L) {
	int err = (int)lua_tointeger(L, 1);
	const char* name = uv_err_name(err);
	lua_pushstring(L, name);
	return 1;
}

void timerCb(uv_timer_t* handle) {
	uint32_t timerId = *(uint32_t*)(handle->data);
	lua_State* L = XDriver::getInstance().getLuaState();
	cbcall(L, lcb_timerCb, timerId);
}

static int xd_createTimer(lua_State* L) {
	uint32_t id = (uint32_t)lua_tointeger(L, 1);
	uint64_t timeout = (uint64_t)luaL_checkinteger(L, 2);
	uint64_t repeat = (uint64_t)luaL_checkinteger(L, 3);
	uv_timer_t* timer = new uv_timer_t;
	timer->data = new uint32_t;
	*(int32_t*)timer->data = id;
	XDriver& driver = XDriver::getInstance();
	driver.getLoop();
	uv_timer_init(driver.getLoop(), timer);
	uv_timer_start(timer, timerCb, timeout, repeat);
	lua_pushlightuserdata(L, (void *)timer);
	return 0;
}

static int xd_delTimer(lua_State* L) {
	uv_timer_t* timer = (uv_timer_t *)lua_touserdata(L, 1);
	uv_timer_stop(timer);
	uv_close((uv_handle_t*)timer, NULL);
	delete timer->data;
	delete timer;
	return 0;
}

static struct luaL_Reg callFunc[] =
{
	{"initCallback",		xd_initCallback },

	//-----------------net-----------------
	{"createListener",		xd_createListener },
	{"createStream",		xd_createStream },
	{"createConnector",		xd_createConnector },
	{"createProtoGroup",	xd_createProtoGroup },
	{"releaseListener",		xd_releaseListener },
	{"releaseStream",		xd_releaseStream },
	{"releaseConnector",	xd_releaseConnector },
	{"listen",				xd_listen },
	{"accept",				xd_accept },
	{"connect",				xd_connect },
	{"readStart",			xd_readStart },
	{"readStop",			xd_readStop },
	{"sendString",			xd_sendString },
	{"sendData",			xd_sendData },
	{"errorName",			xd_errorName },
	{"registerSender",		xd_registerSender },
	{"registerReceiver",	xd_registerReceiver },
	{"registerProto",		xd_registerProto },

	//-----------------timer-----------------
	{"createTimer",			xd_createTimer },
	{"delTimer",			xd_delTimer},
	{ NULL, NULL }
};

int luaopen_lib(lua_State * L)
{
	luaL_newlib(L, callFunc);
	luaL_setfuncs(L, callFunc, 0);
	return 1;
}

void loadLib(lua_State * L)
{
	luaL_requiref(L, "xdriver", luaopen_lib, 1);
	lua_pop(L, 1);
}


#ifdef LUA_BUILD_AS_DLL
extern "C"
{
	LUA_API void xdInit(lua_State * L)
	{
		XDriver& driver = XDriver::getInstance();
		driver.init(L);
	}

	LUA_API void xdRegistFunc(lua_State * L)
	{
		int top = lua_gettop(L);
		luaL_requiref(L, "xdriver", luaopen_lib, 1);
		lua_settop(L, top);
	}

	LUA_API void xdRunOnce()
	{
		XDriver& driver = XDriver::getInstance();
		driver.runOnce();
	}
}
#endif // LUA_BUILD_AS_DLL