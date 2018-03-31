
#ifndef PROTO_H
#define PROTO_H

#include "socket.h"
extern "C"
{
#include "lua.h"
#include "lualib.h"
#include "lauxlib.h"
}

#include <iostream>
#include <vector>

enum ReadError {
	re_INVALID_ID,
	re_INVALID_NUM
};

class XProtoStrParam
{
public:
	uint8_t		pos;		//第几个参数
	uint8_t		offset;		//字节位置
};

/**
*	b:	bool				bool
*	c:	signed char			int8_t
*	C:	unsigned char		uint8_t
*	s:	short				int16_t
*	S:	unsigned short		uint16_t
*	i:	int					int32_t
*	I:	unsigned int		uint32_t
*	d:	double				double
*	a:	string				string
*/
class XProto
{
public:
	uint16_t	id;
	char*		params;
	uint32_t	numParams;
	uint32_t	lenConst;
	uint32_t	numStrParams;
	std::vector<XProtoStrParam> strParams;
};

class XProtoGroup
{
private:
	uint8_t protoIndex;
	uint32_t writeOffset;
	class XStream* stream;
	int32_t readError;
	uint32_t readTotalLen;
	int32_t curProtoId;
	uint32_t strParamLen;
	uint32_t strParamIndex;

	void readProto(char* data, XProto* proto);
	void callProto(char* data, XProto* proto);
	void waitNewProto();

	void write_int8(char* data, int8_t value);
	void write_uint8(char* data, uint8_t value);
	void write_int16(char* data, int16_t value);
	void write_uint16(char* data, uint16_t value);
	void write_int32(char* data, int32_t value);
	void write_uint32(char* data, uint32_t value);
	void write_double(char* data, double value);
	void write_string(char* data, const char* str, uint16_t len);

	int8_t read_int8(char* data, uint32_t* offset);
	uint8_t read_uint8(char* data, uint32_t* offset);
	int16_t read_int16(char* data, uint32_t* offset);
	uint16_t read_uint16(char* data, uint32_t* offset);
	int32_t read_int32(char* data, uint32_t* offset);
	uint32_t read_uint32(char* data, uint32_t* offset);
	double read_double(char* data, uint32_t* offset);
	char* read_string(char* data, uint32_t* offset, uint16_t* len);

public:
	XProtoGroup();
	~XProtoGroup();

	void setStream(XStream* stream) { this->stream = stream; }
	uint32_t getReadTotalBytes() { return readTotalLen; }
	int32_t getReadError() { return readError; }
	void setLenRead(uint32_t len) { readTotalLen = len; }

	uint32_t registerProto(const char* format);

	int readProto(char* cache, uint32_t len);
	void writeProto(lua_State* L, uint32_t luaStackIndex, uint16_t protoId);
	void release();

	uint32_t				id;
	std::vector<XProto *>	protos;
};

#endif // !PROTO_H

