local _ENV = Namespace("RoleGen")

_infos = _infos or {}

function update(deltaTime)
    local count = #_infos
    if count == 0 then return end
    local info = table.remove(_infos)
    if info.roleType == RoleConst.RoleType.LocalPlayer then
        Role.addLocalPlayer(info, info.onLoaded)
    elseif info.roleType == RoleConst.RoleType.Player then
    elseif info.roleType == RoleConst.RoleType.Monster then
    end
end

function createLocalPlayer(roleId, x, y, z, face, callback)
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
    info.x = x or 0
    info.y = y or 0
    info.z = z or 0
    info.face = face or 0
    info.onLoaded = callback
    return info
end