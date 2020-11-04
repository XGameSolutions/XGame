local _ENV = Namespace("Util")


function isNil(obj)
    return obj == nil or obj:IsNull()
end

function safePack(...)
    local tb = {...}
    tb.n = select("#" , ...)
    return tb
end

function safeUnpack(tb)
    return table.unpack(tb, 1, tb.n)
end

function getOrAddComponent(gameObject, classType)
    local com = gameObject.gameObject:GetComponent(classType)
    if Util.isNil(com) then
        com = gameObject.gameObject:AddComponent(classType)
    end
    return com 
end

function isFileExist(path)
    local file = io.open(path, "rb")
    local exist = file ~= nil
    if file then file:close() end
    return exist
end

function gcAll()
    UE.Resources.UnloadUnusedAssets()
    CS.System.GC.Collect()
    collectgarbage("collect")
end
