driver = require "xdriver"

if xd == nil then
    xd                  = {}
    xdcb                = {}
    xd.tbListener       = {}
    xd.tbListener.maxId = 0
    xd.tbStream         = {}
    xd.tbStream.maxId   = 0
    xd.tbConn           = {}
    xd.tbConn.maxId     = 0
    xd.tbProtoGroup     = {}
    xd.tbProtoGroup.maxId = 0
end

local tbListener        = xd.tbListener
local tbStream          = xd.tbStream
local tbConn            = xd.tbConn
local tbProtoGroup      = xd.tbProtoGroup

function xd.createListener(cb)
    local listener =  {}
    listener._id = tbListener.maxId
    listener._cb = cb
    listener._listener = driver.createListener(listener._id)
    tbListener[tbListener.maxId] = listener
    tbListener.maxId = tbListener.maxId + 1
    return listener
end

function xd.releaseListener(id)
    local listener = tbListener[id]
    if listener then
        driver.releaseListener(listener._listener)
        tbListener[id] = nil
    end
end

function xd.createStream()
    local stream =  {}
    stream._id = tbStream.maxId
    stream._stream = driver.createStream(stream._id)
    tbStream[stream._id] = stream
    tbStream.maxId = tbStream.maxId + 1
    return stream
end

function xd.releaseStream(id)
    local stream = tbStream[id]
    if stream then
        driver.releaseStream(stream._stream)
        tbStream[id] = nil
    end
end

function xd.createConnector(cb)
    local conn =  {}
    conn._id = tbConn.maxId
    conn._cb = cb
    conn.stream = xd.createStream()
    conn._conn = driver.createConnector(conn._id, conn.stream._stream)
    tbConn[conn._id] = conn
    tbConn.maxId = tbConn.maxId + 1
    return conn
end

function xd.releaseConnector(id)
    local conn = tbConn[id]
    if conn then
        xd.releaseStream(conn.stream._id)
        driver.releaseConnector(conn._conn)
        tbConn[id] = nil
    end
end

function xd.sender()
    local send =  {}
    send._id = tbProtoGroup.maxId
    send._pg = driver.createProtoGroup(send._id)
    send._tbProtoId =  {}
    send.registerProto = function(funcname, format)
        local protoId = driver.registerProto(send._pg, format)
        send._tbProtoId[funcname] = protoId
        send[funcname] = function(tbSocket, ...)
            local protoId = send._tbProtoId[funcname]
            driver.sendData(tbSocket._stream, protoId, ...)
        end
    end
    tbProtoGroup[send._id] = send
    tbProtoGroup.maxId = tbProtoGroup.maxId + 1
    return send
end

function xd.reader()
    local rece =  {}
    rece._id = tbProtoGroup.maxId
    rece._pg = driver.createProtoGroup(rece._id)
    rece._tbProtoId =  {}
    rece.registerProto = function(funcname, format)
        local protoId = driver.registerProto(rece._pg, format)
        rece._tbProtoId[protoId] = funcname
    end
    tbProtoGroup[rece._id] = rece
    tbProtoGroup.maxId = tbProtoGroup.maxId + 1
    return rece
end

function xd.registerSender(stream, pg)
    stream._sendPgId = pg._id
    driver.registerSender(stream._stream, pg._pg)
end

function xd.registerReader(stream, pg)
    stream._readPgId = pg._id
    driver.registerReceiver(stream._stream, pg._pg)
end

function xd.listen(listener, ip, port)
    local r = driver.listen(listener._listener, ip, port)
    print("xd.listen:", r)
end

function xd.accept(listener, stream)
    local r = driver.accept(listener._listener, stream._stream)
    if r == 0 then
        return stream
    else
        local error = driver.errorName(status)
        print("xd.accept:", r, error)
        return nil
    end
end

function xd.connect(connector, ip, port)
    driver.connect(connector._conn, ip, port)
end

function xd.startRead(stream)
    local r = driver.readStart(stream._stream)
end

function xd.send(stream, msg)
    driver.sendString(stream._stream, msg, string.len(msg))
end

function xdcb.errorCb(err)
    print("xdcb.errorCb:", err)
end

function xdcb.listenCb(id)
    print("xd.listenCb:", id)
    local listener = tbListener[id]
    if listener then
        local stream = xd.createStream()
        if listener._cb then
            listener._cb(stream)
        end
    end
end

function xdcb.listenErrorCb(id, status)
    local error = driver.errorName(status)
    print("xdcb.listenErrorCb:", id, status, error)
    xd.releaseListener(id)
end

function xdcb.dataCb(streamId,protoId,...)
    local stream = tbStream[streamId]
    if stream then
        local pg = tbProtoGroup[stream._readPgId]
        if pg then
            local funcname = pg._tbProtoId[protoId]
            pg[funcname](stream,...)
        end
    end
end

function xdcb.readErrorCb(id, nread)
    local error = driver.errorName(nread)
    print("xdcb.readErrorCb:", id, nread, error)
    xd.releaseStream(id)
end

function xdcb.endCb(id)
    print("xdcb.endCb:", id)
    xd.releaseStream(id)
end

function xdcb.connectCb(id)
    print("xdcb.connectCb:", id)
    local conn = tbConn[id]
    if conn then
        if conn._cb then
            conn._cb(conn.stream)
        end
    end
end

function xdcb.connectErrorCb(id, status)
    local error = driver.errorName(status)
    print("xdcb.connectErrorCb:", id, status, error)
end

function xdcb.connectCloseCb(id)
    print("xdcb.connectCloseCb:", id)
end

driver.initCallback()