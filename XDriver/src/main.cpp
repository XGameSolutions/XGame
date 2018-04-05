
#include "uv.h"
#include "driver.h"
#include "test.h"

int main(int argc,char* argv[])
{
	printf("driver start\n");
	//test_net_server();
	//test_net_client();
	//system("pause");
	//return 0;
	const char* file;
	if (argc < 2) {
		printf("param ERROR:need .lua path\n");
		//system("pause");
		//return 0;
		file = "C:/work/project/XGame/XServer/lua/pg/pg.lua";
		//file = "C:/work/project/XGame/XServer/lua/pc/pc.lua";
	}
	else {
		file = argv[1];
	}
	lua_State* L = luaL_newstate();
	XDriver& driver = XDriver::getInstance();
	driver.init(L);
	int r = luaL_dofile(L, file);
	if (r)
	{
		cout << "dofile ERROR:r=" << lua_tostring(L, -1) << endl;
	}
	driver.runLoop();
	return 0;
}