

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
end

local function traceback(msg)
    print("LUA ERROR:"..tostring(msg))
    print(debug.traceback())
end

xpcall(main,traceback)

