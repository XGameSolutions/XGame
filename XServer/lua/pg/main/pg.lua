
local function includePath()
    local paths = {
        "C:/work/project/XGame/XServer/lua/?.lua",
        "C:/work/project/XGame/XServer/lua/pg/?.lua",
        "C:/work/project/XGame/XCommon/lua/?.lua",
    }
    for k,path in pairs(paths) do
        package.path = package.path .. ";" .. path
    end
end

local function main()
    includePath()
    require("main.pgHead")
    print("pg start...")
    local listener
    local socket
    listener = xd.createListener(function(socket)
        print("new client:")
        _G.g_socket = socket
        socket.isEnd = false
        xd.accept(listener,socket)
        xd.startRead(socket)
        xd.registerSender(socket,s2c)
        xd.registerReader(socket,c2s)

        function socket:close()
            self.isEnd = true
        end

        --s2c.helloTest(socket,false,-1,1,-22,22,-12345678,12345678,14.5789123)
        --s2c.helloTest(socket,false,-1,1,-22,22,-12345678,12345678,14.5789123)
        --s2c.helloTest2(socket,"a","b")
        --s2c.helloTest3(socket,"100")
        s2c.helloTest4(socket,100,"100")
        s2c.helloTest4(socket,200,"200")
        s2c.helloTest4(socket,300,"300")
    end)
    xd.listen(listener,"127.0.0.1",19001)

    xd.addTimer(1000,1000,function()
        --print("timercb")
        if g_socket and not g_socket.isEnd then
            --s2c.helloTest4(g_socket,300,"300")
            s2c.helloTest4(g_socket,400,"400")
            --s2c.helloTest4(g_socket,400,"400")
            --s2c.helloTest(g_sos2c.helloTest4(g_socket,100,"100")cket,false,-1,1,-22,22,-12345678,12345678,14.5789123)
        end
    end)
end

local function traceback(msg)
    print("LUA ERROR:"..tostring(msg))
    print(debug.traceback())
end

xpcall(main,traceback)