#include "proto.h"

XProtoGroup::XProtoGroup() :
	stream(NULL),
	protoIndex(0),
	writeOffset(0),
	readError(0),
	readTotalLen(0),
	curProtoId(-1),
	strParamLen(0),
	strParamIndex(0)
{
}

XProtoGroup::~XProtoGroup()
{
}

uint32_t XProtoGroup::registerProto(const char * params)
{
	XProto* proto = new XProto();
	uint16_t id = (uint16_t)protos.size();
	uint32_t lenConst = 2;
	uint32_t lenParams = strlen(params);
	uint32_t numStrParam = 0;
	for (uint32_t i = 0; i < lenParams; i++) {
		switch (params[i]) {
		case 'b':
		case 'c':
		case 'C':
			lenConst += 1;
			break;
		case 's':
		case 'S':
			lenConst += 2;
			break;
		case 'i':
		case 'I':
			lenConst += 4;
			break;
		case 'd':
			lenConst += 8;
			break;
		case 'a':
			numStrParam++;
			XProtoStrParam strParam;
			strParam.pos = (uint8_t)i;
			strParam.offset = lenConst;
			proto->strParams.push_back(strParam);
			lenConst += 2;
			break;
		}
	}
	proto->id = id;
	proto->params = (char*)params;
	proto->numParams = lenParams;
	proto->lenConst = lenConst;
	proto->numStrParams = numStrParam;

	protos.push_back(proto);
	return id;
}

int XProtoGroup::readProto(char * cache, uint32_t len)
{
	
	if (this->readError != 0) return 0;
	uint32_t lenRecv = len;
	char* data = cache + this->readTotalLen;
	uint32_t lenLeft = lenRecv - this->readTotalLen;
	if (lenLeft < 3) return 0;
	if (this->curProtoId < 0) {
		uint16_t id = *((uint16_t*)data);
		this->curProtoId = id;
		if (id < 0 || id>this->protos.size()) {
			readError = re_INVALID_ID;
			return 0;
		}
		/*uint8_t num = *((uint8_t*)(data + 2));
		if (num != this->protoIndex) {
			printf("ERROR:currIndex=%d,needIndex=%d\n", num, this->protoIndex);
			readError = re_INVALID_NUM;
			return 0;
		}*/
	}
	XProto* proto = this->protos[this->curProtoId];
	if (lenLeft < proto->lenConst) {
		return 0;
	}
	if (proto->numStrParams <= 0) {
		readProto(data, proto);
		return 1;
	}
	uint32_t lenCurrent = proto->lenConst + this->strParamLen;
	//printf("XProtoGroup::readProto:readLen=%d,strnum=%d,lenLeft=%d,lenCurr=%d\n",
	//	readTotalLen, proto->numStrParams, lenLeft, lenCurrent);
	if (lenLeft < lenCurrent) {
		return 0;
	}
	if (this->strParamIndex >= proto->numStrParams) {
		readProto(data, proto);
		return 1;
	}
	while (lenLeft >= lenCurrent) {
		uint8_t strOffset = proto->strParams[this->strParamIndex].offset;
		uint16_t strBytes = *(uint16_t*)(data + strOffset + this->strParamLen);
		this->strParamLen += strBytes;
		this->strParamIndex++;
		lenCurrent = proto->lenConst + this->strParamLen;
		if (this->strParamIndex >= proto->numStrParams) {
			break;
		}
	}
	if (lenLeft >= lenCurrent) {
		readProto(data, proto);
		return 1;
	}
	return 0;
}

void XProtoGroup::readProto(char * data, XProto * proto)
{
	callProto(data, proto);
	this->readTotalLen += proto->lenConst + this->strParamLen;
	//printf("XProtoGroup::readProto:readTotalLen=%d\n", readTotalLen);
	++(this->protoIndex);
	waitNewProto();
}

void XProtoGroup::callProto(char * data, XProto * proto)
{
	lua_State* L = this->stream->getLuaState();
	char* format = proto->params;
	uint32_t lenFormat = proto->numParams;
	uint32_t offset = 0;
	uint32_t nargs = lenFormat + 2;
	lua_checkstack(L, nargs + 1);
	uint16_t id = read_uint16(data, &offset);
	//package num
	//offset += 1;
	cbpush(L, lcb_dataCb, this->stream->getId());
	lua_pushinteger(L, curProtoId);
	int32_t val = 0;
	for (uint32_t i = 0; i < lenFormat; i++) {
		switch (format[i]) {
		case 'b':
			lua_pushboolean(L, read_int8(data, &offset));
			break;
		case 'c':
			lua_pushinteger(L, read_int8(data, &offset));
			break;
		case 'C':
			lua_pushinteger(L, read_uint8(data, &offset));
			break;
		case 's':
			lua_pushinteger(L, read_int16(data, &offset));
			break;
		case 'S':
			lua_pushinteger(L, read_uint16(data, &offset));
			break;
		case 'i':
			lua_pushinteger(L, read_int32(data, &offset));
			break;
		case 'I':
			lua_pushinteger(L, read_uint32(data, &offset));
			break;
		case 'd':
			lua_pushnumber(L, read_double(data, &offset));
			break;
		case 'a':
			uint16_t len = 0;
			char* str = read_string(data, &offset, &len);
			lua_pushlstring(L, str, len);
			break;
		}
	}
	//printf("XProtoGroup::callProto£ºid=%d\n", curProtoId);
	cbrun(L, nargs);
}

void XProtoGroup::waitNewProto()
{
	this->curProtoId = -1;
	this->strParamLen = 0;
	this->strParamIndex = 0;
}

void XProtoGroup::writeProto(lua_State * L, uint32_t luaStackIndex, uint16_t protoId)
{
	if (stream == NULL || !stream->isWritable()) return;
	if (protoId<0 || protoId>protos.size()) return;
	XProto* proto = protos[protoId];
	uint32_t strBytes = 0;
	for (uint32_t i = 0; i < proto->numStrParams; i++) {
		uint32_t len = 0;
		uint32_t pos = proto->strParams[i].pos;
		lua_tolstring(L, luaStackIndex + pos, (size_t*)&len);
		strBytes += len;
	}
	uint32_t totalBytes = proto->lenConst + strBytes;
	uint32_t lenFormat = proto->numParams;
	char* format = proto->params;
	char* data = new char[totalBytes];
	this->writeOffset = 0;
	write_uint16(data, protoId);
	//write_uint8(data, protoIndex);
	for (uint32_t i = 0; i < lenFormat; i++) {
		uint32_t index = luaStackIndex + i;
		switch (format[i]) {
		case 'b':
			write_int8(data, (int8_t)lua_toboolean(L, index));
			break;
		case 'c':
			write_int8(data, (int8_t)lua_tointeger(L, index));
			break;
		case 'C':
			write_uint8(data, (uint8_t)lua_tointeger(L, index));
			break;
		case 's':
			write_int16(data, (int16_t)lua_tointeger(L, index));
			break;
		case 'S':
			write_uint16(data, (uint16_t)lua_tointeger(L, index));
			break;
		case 'i':
			write_int32(data, (int32_t)lua_tointeger(L, index));
			break;
		case 'I':
			write_uint32(data, (uint32_t)lua_tointeger(L, index));
			break;
		case 'd':
			write_double(data, (double)lua_tonumber(L, index));
			break;
		case 'a':
			size_t len = 0;
			const char* str = lua_tolstring(L, index, &len);
			write_string(data, str, (uint16_t)len);
			break;
		}
	}
	stream->write(data, totalBytes);
	//printf("send:index=%d,bytes=%d\n", protoIndex,totalBytes);
	protoIndex++;
	writeOffset = 0;
}

void XProtoGroup::write_int8(char * data, int8_t value)
{
	*(int8_t*)(data + writeOffset) = value;
	writeOffset += 1;
}

void XProtoGroup::write_uint8(char * data, uint8_t value)
{
	*(uint8_t*)(data + writeOffset) = value;
	writeOffset += 1;
}

void XProtoGroup::write_int16(char * data, int16_t value)
{
	*(int16_t*)(data + writeOffset) = value;
	writeOffset += 2;
}

void XProtoGroup::write_uint16(char * data, uint16_t value)
{
	*(uint16_t*)(data + writeOffset) = value;
	writeOffset += 2;
}

void XProtoGroup::write_int32(char * data, int32_t value)
{
	*(int32_t*)(data + writeOffset) = value;
	writeOffset += 4;
}

void XProtoGroup::write_uint32(char * data, uint32_t value)
{
	*(uint32_t*)(data + writeOffset) = value;
	writeOffset += 4;
}

void XProtoGroup::write_double(char * data, double value)
{
#if defined(__APPLE__) || defined(__ANDROID__)
	memcpy(data, &value, sizeof(double));
#else
	*(double*)(data + writeOffset) = value;
#endif
	writeOffset += 8;
}

void XProtoGroup::write_string(char * data, const char * str, uint16_t len)
{
	write_uint16(data, len);
	memcpy(data + writeOffset, str, len);
	writeOffset += len;
}

int8_t XProtoGroup::read_int8(char * data, uint32_t * offset)
{
	int8_t value = *(int8_t*)(data + *offset);
	*offset = *offset + 1;
	return value;
}

uint8_t XProtoGroup::read_uint8(char * data, uint32_t * offset)
{
	uint8_t value = *(uint8_t*)(data + *offset);
	*offset = *offset + 1;
	return value;
}

int16_t XProtoGroup::read_int16(char * data, uint32_t * offset)
{
	int16_t value = *(int16_t*)(data + *offset);
	*offset = *offset + 2;
	return value;
}

uint16_t XProtoGroup::read_uint16(char * data, uint32_t * offset)
{
	uint16_t value = *(uint16_t*)(data + *offset);
	*offset = *offset + 2;
	return value;
}

int32_t XProtoGroup::read_int32(char * data, uint32_t * offset)
{
	int32_t value = *(int32_t*)(data + *offset);
	*offset = *offset + 4;
	return value;
}

uint32_t XProtoGroup::read_uint32(char * data, uint32_t * offset)
{
	uint32_t value = *(uint32_t*)(data + *offset);
	*offset = *offset + 4;
	return value;
}

double XProtoGroup::read_double(char * data, uint32_t * offset)
{
	double value;
#if defined(__APPLE__) || defined(__ANDROID__)
	memcpy(&value, data, sizeof(double));
#else
	value = *(double*)(data + *offset);
#endif
	*offset = *offset + 8;
	return value;
}

char * XProtoGroup::read_string(char * data, uint32_t * offset, uint16_t* len)
{
	*len = read_uint16(data, offset);
	char* str = (char*)(data + *offset);
	*offset = (*offset) + (uint32_t)len;
	return str;
}

void XProtoGroup::release()
{
	std::vector<XProto*>::iterator it, end;
	it = this->protos.begin();
	end = this->protos.end();
	for (; it != end; ++it) {
		delete *it;
		*it = NULL;
	}
	this->protos.clear();
}
