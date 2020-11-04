local _ENV = Namespace("LocalPlayerMove")

_isWork = _isWork or false
GRAVITY = 9.8

function init()
end

function startWork()
    _isWork = true
end

function stopWork()
    _isWork = false
end

function update(deltaTime)
    if not _isWork then return end
    if not Role.localPlayer then return end
    if not Role.localPlayer.isLoaded then return end
    local localPlayer = Role.localPlayer
    local cc = Role.localPlayer.characterController
    if Util.isNil(cc) then return end
    deltaTime = UE.Time.smoothDeltaTime
    local dire = Input.getMoveDire()
    if cc.isGrounded then
        if dire == UE.Vector3.zero then
            __stopMove(localPlayer)
            return
        end
    end
    __startMove(localPlayer)
    localPlayer.moveDire = __getMoveDire(localPlayer, dire)
    local speed = 4
    local deltaDist = localPlayer.moveDire * speed * deltaTime
    deltaDist.y = - GRAVITY * deltaTime
    cc:Move(deltaDist)
end

function __startMove(localPlayer)
    --TODO: 改变状态
end

function __stopMove(localPlayer)
    --TODO: 改变状态
end

function __getMoveDire(tRole, dire)
    local face = math.rad(tRole.face)
    local sin = math.sin(face)
    local cos = math.cos(face)
    local dire = UE.Vector3(dire.x * cos + dire.z * sin, 0, -dire.x * sin + dire.z * cos)
    return dire.normalized
end

