#ifndef XDRIVER_H
#define XDRIVER_H

#include "uv.h"
#include "lua2c.h"

extern "C"
{
#include "lua.h"
#include "lualib.h"
#include "lauxlib.h"
}

#include <iostream>
using namespace std;

class XDriver
{
public:
	static XDriver& getInstance() {
		static XDriver instance;
		return instance;
	}

	XDriver(const XDriver&) = delete;
	XDriver& operator=(const XDriver&) = delete;

	uv_loop_t* getLoop() { return loop; }
	void init(const string& file);
	void run();
	void close();
private:
	XDriver() {}
	lua_State *L;
	uv_loop_t *loop;
	string filename;
};

#endif // !XDRIVER_H.H
