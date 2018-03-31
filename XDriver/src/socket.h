
#ifndef SOCKET_H
#define SOCKET_H

#include "uv.h"
#include "c2lua.h"
#include "proto.h"

class XStream
{
protected:
	uint32_t id;
	bool over;
	int reason;
	uv_tcp_t tcp;
	lua_State* L;
	char* readCache;
	uint32_t readTotalBytes;
	uint32_t readWaitBytes;

public:
	class XProtoGroup* sender;
	class XProtoGroup* reader;

	XStream();
	~XStream();

	bool isOver() { return over; }
	void setId(uint32_t id) { this->id = id; }
	void setOver(bool over) { this->over = over; }
	void setLuaState(lua_State* L) { this->L = L; }
	void setSender(XProtoGroup* pg) { this->sender = pg; }
	void setReader(XProtoGroup* pg) { this->reader = pg; }

	uint32_t getId() { return id; }
	uv_tcp_t& getTcp() { return tcp; }
	lua_State* getLuaState() { return this->L; }

	bool isWritable();
	int readStart();
	int readStop();
	int read(uint32_t len);
	void readError(ssize_t nread);
	void write(char* data, int len);
	void writeError();
	void freeCache();
	void close();

	void onCloseCb();
	void onAllocCb(size_t size, uv_buf_t* buf);
	void onReadCb(ssize_t nread, const uv_buf_t* buf);
	void onWriteCb(uv_write_t* req, int status);
};

class XConnector
{
protected:
	int id;
	int state;
	int errorStatus;
	lua_State* L;
	uv_loop_t * loop;
	XStream* stream;
	uv_connect_t connectReq;

public:
	XConnector(XStream* stream);
	~XConnector();

	enum {
		eConn_Connecting,
		eConn_Closing,
		eConn_ClosingAndStop,
		eConn_Over
	};
	void setId(int id) { this->id = id; }
	void setState(int state) { this->state = state; }
	void setStatus(int status) { this->errorStatus = status; }
	void setLoop(uv_loop_t* loop) { this->loop = loop; }
	void setStream(XStream* stream) { this->stream = stream; }
	void setLuaState(lua_State* L) { this->L = L; }
	int getStatus() { return this->errorStatus; }
	int getState() { return state; }
	XStream* getStream() { return this->stream; }
	uv_connect_t& getConnReq() { return this->connectReq; }

	void connect(const char* ip, int port);
	void stop();
	void onConnectCb(uv_connect_t* req, int status);
	void onCloseCb();
};

class XListener
{
protected:
	bool over;
	uint32_t id;
	uv_tcp_t server;
	uv_loop_t* loop;
	lua_State* L;

public:
	XListener();
	~XListener();

	bool isOver() { return over; }
	void setId(uint32_t id) { this->id = id; }
	void setLoop(uv_loop_t* loop) { this->loop = loop; }
	void setLuaState(lua_State* L) { this->L = L; }

	int listen(const char* ip, int port);
	int accept(XStream* stream);
	void stop();
	void onListenCb(uv_stream_t* server, int status);
	void onCloseCb();
};

#endif // !SOCKET_H

