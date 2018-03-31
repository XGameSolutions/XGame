#include "driver.h"

void XDriver::init(const string& file)
{
	filename = file;
	loop = uv_default_loop();
	L = luaL_newstate();
	luaL_openlibs(L);
	loadLib(L);
	int r = luaL_dofile(L, filename.c_str());
	if (r)
	{
		cout << "dofile ERROR:r=" << lua_tostring(L, -1) << endl;
	}
}

void XDriver::run()
{
	while (true) {
		int r = uv_run(loop, UV_RUN_NOWAIT);
		Sleep(10);
	}
}

void XDriver::close()
{
	lua_close(L);
}
