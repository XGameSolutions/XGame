local _ENV = Namespace("Role")

localPlayer = localPlayer or nil
_roles = _roles or {}

function init()
end

function addLocalPlayer(tRole, onLoaded)
    tRole = RoleHelper.initRole(tRole)
    localPlayer = tRole
    _roles[tRole.roleId] = tRole
    RoleHelper.setLocalPlayerComponent(tRole)
    RoleHelper.loadModel(tRole, function()
        CameraMgr.setTarget(tRole.transform)
        if onLoaded then
            onLoaded()
        end
    end)
    return tRole
end


function getRole(nRoleId)
    return _roles[nRoleId]
end

function addRole(tRole)
    _roles[tRole.nRoleId] = tRole
end

function setFace(tRole, face)
    tRole.transform.localEulerAngles = UE.Vector3(0, face, 0)
    tRole.face = face
end

