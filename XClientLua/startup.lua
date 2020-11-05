function initIncludePath()
    local platform = UE.Application.platform
    local path = ""
    if platform == UE.RuntimePlatform.WindowsEditor or platform == UE.RuntimePlatform.OSXEditor then
        local dataPath = UE.Application.dataPath
        path = path .. dataPath .. "/../../XCommon/lua/?.lua"
    end
    package.path = package.path .. ";" .. path
end

function init()
    _G.UE = CS.UnityEngine
    _G.isPatchEnd = false
    initIncludePath()
end

function main()
    CS.XDebug.XLog.Vital("lua startup...")
    init()
    checkPatch()
end

function checkPatch()
    require "utils/Namespace"
    require "utils/PrintUtil"
    require "launch/LaunchView"
    require "CSharpCallLua"
    require "patch/Patcher"
    CS.GameStart.Instance:Startup()
    LaunchView.init()
    Patcher.check(patchEnd)
end

function patchEnd()
    require "header"
    printVital("patch end")
    _G.isPatchEnd = true
    Launcher.start()
end

function connect()
    local ip = "127.0.0.1"
    local port = 19001
    local conn
    conn = xd.createConnector(function(socket)
        print("connect success!")
        xd.startRead(socket)
        xd.registerSender(socket, c2s)
        xd.registerReader(socket, s2c)
        _G.g_socket = socket
    end)
    xd.connect(conn, ip, port)
end

function traceback(msg)
    print("LUA ERROR:" .. tostring(msg) .. "\n" .. debug.traceback())
end

xpcall(main, traceback)

