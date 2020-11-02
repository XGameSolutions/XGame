local _ENV = Namespace("Role")

localPlayer = localPlayer or nil
_roles = _roles or {}

function init()
end

function addLocalPlayer(tRole, onLoaded)
    tRole = RoleHelper.initRole(tRole)
    localPlayer = tRole
    RoleHelper.loadModel(tRole, onLoaded)
    _roles[tRole.roleId] = tRole
    return tRole
end


function getRole(nRoleId)
    return _roles[nRoleId]
end

function addRole(tRole)
    _roles[tRole.nRoleId] = tRole
end

