

local function includePath()
    local paths = {
        "C:/work/project/XGame/XClientLuaScript/lua/?.lua",
        "C:/work/project/XGame/XCommon/lua/?.lua",
    }
    for k,path in pairs(paths) do
        package.path = package.path .. ";" .. path
    end
end

local function main()
    includePath()
    require("header")
    print("client start...")
    local ip = "127.0.0.1"
    local port = 19001
    local conn
    conn = xd.createConnector(function(socket)
        print("connect success!")
        xd.startRead(socket)
        xd.registerSender(socket,c2s)
        xd.registerReader(socket,s2c)
        --c2s.myTest(socket,1000)
    end)
    xd.connect(conn,ip,port)
end

local function traceback(msg)
    print("LUA ERROR:"..tostring(msg))
    print(debug.traceback())
end

xpcall(main,traceback)

