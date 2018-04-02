#include "driver.h"

void XDriver::init()
{
	loop = uv_default_loop();
	L = luaL_newstate();
	luaL_openlibs(L);
	loadLib(L);
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
