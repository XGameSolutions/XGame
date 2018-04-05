#include "driver.h"

void XDriver::init(lua_State* L)
{
	this->L = L;
	loop = uv_default_loop();
	luaL_openlibs(this->L);
	loadLib(this->L);
}

void XDriver::runLoop()
{
	while (true) {
		this->runOnce();
		Sleep(10);
	}
}

void XDriver::runOnce()
{
	int r = uv_run(loop, UV_RUN_NOWAIT);
}

void XDriver::close()
{
	lua_close(L);
}
