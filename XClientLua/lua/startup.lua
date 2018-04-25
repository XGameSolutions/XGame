

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
    print(c2s._id)
    print(s2c._id)
    local conn = xd.createConnector(function (socket)
        print("connect success!")
        xd.startRead(socket)
        xd.registerSender(socket,c2s)
        xd.registerReader(socket,s2c)
    end)
    xd.connect(conn,"127.0.0.1",19001)
end

local function traceback(msg)
    print("LUA ERROR:"..tostring(msg))
    print(debug.traceback())
end

xpcall(main,traceback)

