

tbNamespace = tbNamespace or {}
tbRequireName = tbRequireName or {}

function Namespace(strSpaceName, tData)
    if not _G[strSpaceName] then
        _G[strSpaceName] = tData or {}
        _G[strSpaceName].__spaceName__ = strSpaceName
        setmetatable(_G[strSpaceName], {__index = _G})
        tbNamespace[strSpaceName] = true
    else
        printWarn("ERRPR:Namespace have existed : " .. strSpaceName)
    end
    return _G[strSpaceName]
end

local oldRequire = require
require = function (str)
    if not tbRequireName[str] then
        local ret = oldRequire(str)
        tbRequireName[str] = true
        return ret
    else
        print("ERROR:require is repeated : " .. str)
        return nil
    end
end

function clearAllNamespace()
    for k,v in pairs(tbNamespace) do
        _G[k] = nil
    end
    for k,v in pairs(tbRequireName) do
        package.loaded[k] = nil
    end
    tbNamespace = {}
    tbRequireName = {}
end