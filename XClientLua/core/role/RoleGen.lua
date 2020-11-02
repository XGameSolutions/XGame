local _ENV = Namespace("RoleGen")

_infos = _infos or {}

function update(deltaTime)
    local count = #_infos
    if count == 0 then return end
    local info = table.remove(_infos)
    if info.roleType == RoleConst.RoleType.LocalPlayer then
        printError("RoleGen:addLocalPlayer")
        Role.addLocalPlayer(info)
    elseif info.roleType == RoleConst.RoleType.Player then
    elseif info.roleType == RoleConst.RoleType.Monster then
    end
end

function createLocalPlayer(roleId, x, y, z, face, callback)
    printError("RoleGen.createLocalPlayer")
    local info = _initInfo(roleId, RoleConst.RoleType.LocalPlayer, x, y, z, face, callback)
    table.insert(_infos, info)
end

function createPlayer(roleId, x, y, z, face, callback)
    local info = _initInfo(roleId, RoleConst.RoleType.Player, x, y, z, face,callback)
    table.insert(_infos, info)
end

function createMonster(roleId, x, y, z, face, callback)
    local info = _initInfo(roleId, RoleConst.RoleType.Monster, x, y, z, face,callback)
    table.insert(_infos, info)
end

function _initInfo(roleId, roleType, x, y, z, face, callback)
    local info = {}
    info.roleId = roleId
    info.roleType = roleType
    info.x = x
    info.y = y
    info.z = z
    info.face = face
    info.callback = callback
    return info
end