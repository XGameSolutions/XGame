#include "socket.h"
#include "c2lua.h"

static void listenCb(uv_stream_t* tcp, int status)
{
	XListener* listener = (XListener*)tcp->data;
	if (listener->isOver()) {
		listener->onListenCb(tcp, status);
	}
}

static void listenCloseCb(uv_handle_t* handle)
{
	XListener* listener = (XListener*)handle->data;
	listener->onCloseCb();
}

static void streamCloseCb(uv_handle_t* handle)
{
	XStream* stream = (XStream*)handle->data;
	stream->onCloseCb();
}

static void streamAllacCb(uv_handle_t* handle, size_t size, uv_buf_t* buf) {
	XStream* stream = (XStream*)handle->data;
	stream->onAllocCb(size, buf);
}

static void streamReadCb(uv_stream_t* tcp, ssize_t nread, const uv_buf_t* buf) {
	XStream* stream = (XStream*)tcp->data;
	stream->onReadCb(nread, buf);
}

static void streamWriteCb(uv_write_t* req, int status) {
	XStream* stream = (XStream*)req->handle->data;
	stream->onWriteCb(req, status);
	delete req;
}

static void connectErrorCloseCb(uv_handle_t* handle) {
	XConnector* conn = (XConnector*)handle->data;
	if (conn->getState() == XConnector::eConn_ClosingAndStop) {
		conn->onCloseCb();
	}
	else {
		if (conn->getState() != XConnector::eConn_Closing) {
			return;
		}
		conn->setState(XConnector::eConn_Over);
		conn->onConnectCb(&conn->getConnReq(), conn->getStatus());
	}
}

static void connectCb(uv_connect_t* req, int status) {
	XConnector* conn = (XConnector*)req->data;
	XStream* stream = conn->getStream();
	conn->setStatus(status);
	if (status == 0) {
		conn->setState(XConnector::eConn_Over);
		stream->getTcp().data = stream;
		conn->onConnectCb(req, status);
	}
	else {
		conn->setState(XConnector::eConn_Closing);
		uv_tcp_t* tcp = &stream->getTcp();
		tcp->data = conn;
		uv_close((uv_handle_t*)tcp, connectErrorCloseCb);
	}
}

static void connectCloseCb(uv_handle_t* handle) {
	XConnector* conn = (XConnector*)handle->data;
	conn->onCloseCb();
}

#pragma region XStream
XStream::XStream() :
	readCache(NULL),
	readTotalBytes(0),
	readWaitBytes(0)
{
	this->over = false;
	this->reason = 0;
	this->tcp.data = this;
}

XStream::~XStream()
{
	this->tcp.data = NULL;
	freeCache();
}

bool XStream::isWritable()
{
	uv_stream_t* stream = (uv_stream_t*)&tcp;
	if (uv_is_writable(stream) && !over) {
		return true;
	}
	return false;
}

int XStream::readStart()
{
	int r = 0;
	r = uv_read_start((uv_stream_t*)&tcp, streamAllacCb, streamReadCb);
	return r;
}

int XStream::readStop()
{
	int r = uv_read_stop((uv_stream_t*)&tcp);
	return r;
}

void XStream::readError(ssize_t nread)
{
	if (nread > 0) return;
	if (over) return;
	over = true;
	if (nread == UV_EOF || nread == UV_ECONNRESET) {
		this->close();
		cbcall(L, lcb_endCb, id);
	}
	else {
		this->close();
		cbcall(L, lcb_readErrorCb, id, nread);
	}
}

int XStream::read(uint32_t len)
{
	char* data = this->readCache;
	int r = 1;
	while (r) {
		r = reader->readProto(data, len);
		if (isOver()) {
			freeCache();
			return r;
		}
	}
	int32_t error = reader->getReadError();
	if (error != 0) {
		return error;
	}
	uint32_t lenRead = reader->getReadTotalBytes();
	//printf("XStream::read:%d,%d\n",lenRead,len);
	if (lenRead == len) {
		freeCache();
	}
	else {
		this->readWaitBytes = len - lenRead;
		memmove(readCache, (data + lenRead), readWaitBytes);
	}
	return r;
}

void XStream::write(char * data, int len)
{
	if (!isWritable()) return;
	//printf("XStream::write:%s,len=%d\n",data,len);
	uv_buf_t buf;
	uv_write_t* req = new uv_write_t;
	char* dest = new char[len];
	memcpy(dest, data, len);
	req->data = (void*)dest;
	buf = uv_buf_init(dest, len);
	uv_stream_t* stream = (uv_stream_t*)&tcp;
	uv_write(req, stream, &buf, 1, streamWriteCb);
}

void XStream::writeError()
{
	//printf("XStream::writeError\n");
	if (over) return;
	this->over = true;
	this->close();
	cbcall(L, lcb_endCb, id);
}

void XStream::freeCache()
{
	if (readCache != NULL) {
		delete readCache;
		readCache = NULL;
		readTotalBytes = 0;
		readWaitBytes = 0;
	}
}

void XStream::close()
{
	uv_handle_t* handle = (uv_handle_t*)&tcp;
	if (uv_is_closing(handle)) {
		uv_close(handle, streamCloseCb);
	}
}

void XStream::onCloseCb()
{
}

void XStream::onAllocCb(size_t size, uv_buf_t * buf)
{
	if (this->readCache == NULL) {
		this->readCache = new char[size];
		this->readTotalBytes = size;
		//printf("new freeCache:%d\n", readCache);
	}
	if (this->readWaitBytes > size) {
		buf->base = NULL;
		buf->len = 0;
		return;
	}
	uint32_t lenRead = this->readWaitBytes;
	buf->base = this->readCache + lenRead;
	buf->len = size - lenRead;
}

void XStream::onReadCb(ssize_t nread, const uv_buf_t * buf)
{
	//printf("XStream::onReadCb:%d\n", nread);
	if (nread < 0) {
		this->readError(nread);
		if (buf->base != NULL) {
			delete buf->base;
		}
	}
	else {
		uint32_t lenRecv = (uint32_t)((buf->base - this->readCache) + nread);
		read(lenRecv);
		this->reader->setLenRead(0);
	}
}

void XStream::onWriteCb(uv_write_t * req, int status)
{
	//printf("XStream::onWriteCb:%d\n", status);
	delete req->data;
	if (status != 0) {
		this->writeError();
	}
}

#pragma endregion

#pragma region XConnector
XConnector::XConnector(XStream* stream) :
	stream(stream),
	loop(NULL)
{
}
XConnector::~XConnector()
{
	this->stream = NULL;
	this->loop = NULL;
}

void XConnector::connect(const char * ip, int port)
{
	struct sockaddr* addr = NULL;
	struct sockaddr_in addr4;
	struct sockaddr_in6 addr6;
	if (strstr(ip, ":")) {
		uv_ip6_addr(ip, port, &addr6);
		addr = (sockaddr*)&addr6;
	}
	else {
		uv_ip4_addr(ip, port, &addr4);
		addr = (sockaddr*)&addr4;
	}
	uv_tcp_t* tcp = &stream->getTcp();
	tcp->data = (void*)this;
	connectReq.data = (void*)this;
	uv_tcp_init(loop, tcp);
	errorStatus = uv_tcp_connect(&connectReq, tcp, addr, connectCb);
	if (errorStatus == 0) {
		state = eConn_Connecting;
	}
	else {
		state = eConn_Closing;
		tcp->data = this;
		uv_close((uv_handle_t*)tcp, connectErrorCloseCb);
	}
}
void XConnector::stop()
{
	if (state == eConn_Over) return;
	if (state == eConn_Closing) {
		state = eConn_ClosingAndStop;
		return;
	}
	state = eConn_Over;
	uv_close((uv_handle_t*)&stream->getTcp(), connectCloseCb);
}

void XConnector::onConnectCb(uv_connect_t * req, int status)
{
	if (status == 0) {
		cbcall(L, lcb_connectCb, id);
	}
	else {
		cbcall(L, lcb_connectErrorCb, id, status);
	}
}

void XConnector::onCloseCb()
{
	cbcall(L, lcb_connectCloseCb, id);
}
#pragma endregion


#pragma region XListener
XListener::XListener() :
	loop(0),
	L(0)
{
}

XListener::~XListener()
{
}

int XListener::listen(const char * ip, int port)
{
	server.data = this;
	struct sockaddr_in addr;
	int r;
	r = uv_ip4_addr(ip, port, &addr);
	r = uv_tcp_init(loop, &server);
	r = uv_tcp_bind(&server, (struct sockaddr*)&addr, 0);
	r = uv_listen((uv_stream_t*)&server, 1, listenCb);
	if (r == 0) {
		over = true;
	}
	return r;
}

int XListener::accept(XStream* stream)
{
	uv_tcp_t* client = &stream->getTcp();
	uv_tcp_init(loop, client);
	int r = uv_accept((uv_stream_t*)&server, (uv_stream_t*)client);
	return r;
}

void XListener::stop()
{
	if (over) return;
	over = true;
	uv_close((uv_handle_t*)&server, listenCloseCb);
}

void XListener::onListenCb(uv_stream_t * server, int status)
{
	if (status == 0) {
		cbcall(L, lcb_listenCb, id);
	}
	else {
		cbcall(L, lcb_listenErrorCb, id, status);
	}
}

void XListener::onCloseCb()
{
	printf("onCloseCb\n");
}
#pragma endregion


