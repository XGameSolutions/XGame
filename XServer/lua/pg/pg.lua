
local function includePath()
    local paths = {
        "C:/work/project/XGame/XServer/lua/?.lua",
        "C:/work/project/XGame/XCommon/lua/?.lua",
    }
    for k,path in pairs(paths) do
        package.path = package.path .. ";" .. path
    end
end

local function main()
    includePath()
    require("pg.pgHead")
    print("pg start...")
    local listener
    local socket
    listener = xd.createListener(function(socket)
        print("new client:")
        xd.accept(listener,socket)
        xd.startRead(socket)
        xd.registerSender(socket,s2c)
        xd.registerReader(socket,c2s)
        ---s2c.helloTest(socket,false,-1,1,-22,22,-12345678,12345678,14.5789123,"hello")
        s2c.helloTest2(socket,"a","b")
    end)
    xd.listen(listener,"127.0.0.1",19001)
end

local function traceback(msg)
    print("LUA ERROR:"..tostring(msg))
    print(debug.traceback())
end

xpcall(main,traceback)