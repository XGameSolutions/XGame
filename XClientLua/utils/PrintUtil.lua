local _ENV = Namespace("PrintUtil")

ALL     = 0
DEBUG   = 1
WARN    = 2
INFO    = 3
PROTO   = 4
VITAL   = 5
ERROR   = 6


function _format(...)
    local tb = {}
    for i=1, select("#", ...) do
        table.insert(tb, tostring(select(i, ...)))
    end
    return table.concat(tb, " ")
end

function init()
    _G.oldPrint = _G.print
    _G.XLog = CS.XDebug.XLog

    _G.print = function (...)
        XLog.Debug(_format(...))
    end

    _G.printWarn = function(...)
        XLog.Warn(_format(...))
    end

    _G.printInfo = function(...)
        XLog.Info(_format(...))
    end

    _G.printVital = function(...)
        XLog.Vital(_format(...))
    end

    _G.printError = function (...)
        XLog.Error(_format(...))
    end

    _G.printErrorTrace = function(...)
        XLog.Error(_format(...) .. "\n" .. debug.traceback())
    end

    printVital("Print Init","OK")
end


init()