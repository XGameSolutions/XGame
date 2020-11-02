local _ENV = Namespace("RoleHelper")

function initRole(tRole)
    local goName = string.format("%s_%d", RoleConst.RoleType[tRole.roleType], tRole.roleId)
    tRole.gameObject    = UE.GameObject(goName)
    tRole.transform     = tRole.gameObject.transform
    tRole.model         = nil
    tRole.isLoaded      = false

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