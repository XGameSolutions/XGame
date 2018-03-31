
local function includePath()
    local paths = {
        "C:/work/project/XGame/XServer/lua/?.lua",
        "C:/work/project/XGame/XAll/lua/?.lua",
    }
    for k,path in pairs(paths) do
        package.path = package.path .. ";" .. path
    end
end

local function main()
    includePath()
    require("pc.pcHead")
    print("pc start...")
    local ip = "127.0.0.1"
    local port = 19001
    local conn
    conn = xd.createConnector(function(socket)
        xd.startRead(socket)
        xd.registerSender(socket,c2s)
        xd.registerReader(socket,s2c)
        --c2s.myTest(socket,true,-1,1,-22,22,-12345678,12345678,14.5789123,"hello")
    end)
    xd.connect(conn,ip,port)
end

local function traceback(msg)
    print("LUA ERROR:"..tostring(msg))
    print(debug.traceback())
end

xpcall(main,traceback)