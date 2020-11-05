local _ENV = Namespace("RoleHelper")

function initRole(tRole)
    local goName = string.format("%s_%d", RoleConst.RoleType[tRole.roleType], tRole.roleId)
    tRole.gameObject    = UE.GameObject(goName)
    tRole.transform     = tRole.gameObject.transform
    tRole.model         = nil
    tRole.isLoaded      = false
    Role.setFace(tRole, tRole.face or 0)
    return tRole 
end

function loadModel(tRole, callback)
    local abName = "model_role"
    local assetName = "role"
    print("loadModel:",tRole.roleId)
    Assets.loadGameObject(abName, assetName, function(prefab)
        local mode = UE.GameObject.Instantiate(prefab)
        setModel(tRole,mode)
        if callback then
            callback()
        end
    end)
end

function setModel(tRole, model)
    model.transform:SetParent(tRole.gameObject.transform)
    model.transform.localPosition = UE.Vector3.zero
    model.transform.localEulerAngles = UE.Vector3.zero
    tRole.model = model
    tRole.isLoaded = true
end

function setLocalPlayerComponent(tRole)
    if tRole.characterController then return end
    local cc = Util.getOrAddComponent(tRole.gameObject, typeof(UE.CharacterController))
    cc.radius = 0.4
    cc.height = 1.6
    cc.skinWidth = 0.01
    cc.slopeLimit = 45
    cc.stepOffset = 0.1
    cc.minMoveDistance = 0
    cc.center = UE.Vector3(0,0,0)

    tRole.characterController = cc
end
    