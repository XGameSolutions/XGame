
local function includePath()
    local paths = {
        "../XServer/lua/?.lua",
        "../XServer/lua/pg/?.lua",
        "../XCommon/lua/?.lua",
        "../../LuaBT/LuaBT/?.lua",
    }
    for k,path in pairs(paths) do
        package.path = package.path .. ";" .. path
    end
end

local function init()
    aiMgr.init()
end

local function listen()
    local listener
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

    end)
    xd.listen(listener,"127.0.0.1",19001)

    xd.addTimer(1000,1000,function()
        if g_socket and not g_socket.isEnd then

        end
    end)
end

local function main()
    includePath()
    require("main.pgHead")
    print("pg start...")
    init()
    listen()
    aiMgr.addTestAi()
end

local function traceback(msg)
    print("LUA ERROR:"..tostring(msg))
    print(debug.traceback())
end

xpcall(main,traceback)