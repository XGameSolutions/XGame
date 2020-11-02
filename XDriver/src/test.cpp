#include "assert.h"
#include "test.h"
#include "nav.h"
#include "socket.h"
#include <iostream>
using Namespace std;

void test_nav()
{
	printf("test_nav\n");
	BuildContext* ctx;
	ctx = new BuildContext;
	ctx->enableLog(true);

	Nav* nav;
	nav = new Nav;
	nav->setContext(ctx);

	bool flag;
	const char* path = "C:/work/project/ExportScene/obj/scene.obj";
	flag = nav->load(path);
	if (!flag) {
		printf("nav load ERROR:%s\n",path);
		return;
	}
	flag = nav->build();
	if (!flag) {
		printf("nav build ERROR:%s\n",path);
		return;
	}

	float spos[3];
	flag = nav->getRandomPos(spos);
	if (flag) {
		
		float height = 0;
		flag = nav->getHeight(spos,10,&height);
		printf("random spos:(%.1f,%.1f,%.1f),flag=%d,hig=%.1f\n", spos[0], spos[1], spos[2],flag,height);
	}
	float epos[3];
	flag = nav->getRandomPos(epos);
	if (flag) {
		float height = 0;
		flag = nav->getHeight(epos, 10, &height);
		printf("random epos:(%.1f,%.1f,%.1f),flag=%d,hig=%.1f\n", epos[0], epos[1], epos[2], flag, height);
	}
	static const int MAX_POLYS = 256;
	float paths[MAX_POLYS * 3];
	int npath;
	flag = nav->getPath(spos, epos, paths, &npath);
	if (flag) {
		for (int i = 0; i < npath; i++) {
			float x = paths[i * 3];
			float y = paths[i * 3 + 1];
			float z = paths[i * 3 + 2];
			printf("path %d,(%.1f,%.1f,%.1f)\n",i,x,y,z);
		}
	}
}



static void connection_cb(uv_stream_t* server, int status);
static void connect_cb(uv_connect_t* req, int status);
static void write_cb(uv_write_t* req, int status);
static void read_cb(uv_stream_t* stream, ssize_t nread, const uv_buf_t* buf);
static void alloc_cb(uv_handle_t* handle, size_t suggested_size, uv_buf_t* buf);

static uv_tcp_t tcp_server;
static uv_tcp_t tcp_client;
static uv_tcp_t tcp_peer; /* client socket as accept()-ed by server */
static uv_connect_t connect_req;
static uv_write_t write_req;

static int write_cb_called;
static int read_cb_called;

#define TEST_PORT 19001

static void write_cb(uv_write_t* req, int status) {
	assert(status == 0);
	write_cb_called++;
}

static void connect_cb(uv_connect_t* req, int status) {
	printf("connect_cb:%d\n",status);
	uv_buf_t buf = uv_buf_init("PINGPINGPINGPING", 16);
	uv_stream_t* stream;
	int r;

	assert(req == &connect_req);
	assert(status == 0);

	stream = req->handle;

	r = uv_write(&write_req, stream, &buf, 1, write_cb);
	assert(r == 0);

	/* Start reading */
	r = uv_read_start(stream, alloc_cb, read_cb);
	assert(r == 0);
}

static void alloc_cb(uv_handle_t* handle,
	size_t suggested_size,
	uv_buf_t* buf) {
	static char slab[1024];
	buf->base = slab;
	buf->len = sizeof(slab);
}


static void read_cb(uv_stream_t* stream, ssize_t nread, const uv_buf_t* buf) {
	if (nread < 0) {
		fprintf(stderr, "read_cb error: %s\n", uv_err_name(nread));
		assert(nread == UV_ECONNRESET || nread == UV_EOF);

		uv_close((uv_handle_t*)&tcp_server, NULL);
		uv_close((uv_handle_t*)&tcp_peer, NULL);
	}
	else {
		printf("read_cb:%s,nread=%d\n",buf->base, nread);
	}

	read_cb_called++;
}

static void connection_cb(uv_stream_t* server, int status) {
	int r;
	uv_buf_t buf;

	assert(server == (uv_stream_t*)&tcp_server);
	assert(status == 0);

	r = uv_tcp_init(server->loop, &tcp_peer);
	assert(r == 0);

	r = uv_accept(server, (uv_stream_t*)&tcp_peer);
	assert(r == 0);

	r = uv_read_start((uv_stream_t*)&tcp_peer, alloc_cb, read_cb);
	assert(r == 0);

	buf.base = "hellohellohellohellohellohello\n";
	buf.len = 31;

	r = uv_write(&write_req, (uv_stream_t*)&tcp_peer, &buf, 1, write_cb);
	assert(r == 0);
}

void test_net_server()
{
	printf("test_net\n");
	
	struct sockaddr_in addr;
	int r;

	assert(0 == uv_ip4_addr("127.0.0.1", TEST_PORT, &addr));

	r = uv_tcp_init(uv_default_loop(), &tcp_server);
	assert(r == 0);

	r = uv_tcp_bind(&tcp_server, (const struct sockaddr*) &addr, 0);
	assert(r == 0);

	r = uv_listen((uv_stream_t*)&tcp_server, 1, connection_cb);
	assert(r == 0);

	r = uv_run(uv_default_loop(), UV_RUN_DEFAULT);
	assert(r == 0);
}

void test_net_client()
{
	struct sockaddr_in sa;
	assert(0 == uv_ip4_addr("127.0.0.1", TEST_PORT, &sa));
	assert(0 == uv_tcp_init(uv_default_loop(), &tcp_client));

	assert(0 == uv_tcp_connect(&connect_req,
		&tcp_client,
		(const struct sockaddr *)
		&sa,
		connect_cb));

	uv_run(uv_default_loop(), UV_RUN_DEFAULT);
}
