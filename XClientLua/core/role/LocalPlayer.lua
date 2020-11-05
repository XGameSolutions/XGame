local _ENV = Namespace("LocalPlayer")

_rotationX = _rotationX or 0
_rotationY = _rotationY or 0 
_mouseSpeed = _mouseSpeed or 1

function update(deltaTime)
    if not Role.localPlayer then return end
    LocalPlayerMove.update(deltaTime)
end

function updateDire(moveDelta)
    if moveDelta == nil then return end
    if not Role.localPlayer then return end
    _rotationX = _rotationX + moveDelta.x * _mouseSpeed
    Role.localPlayer.transform.localEulerAngles = UE.Vector3(0, _rotationX, 0)
    Role.localPlayer.face = (_rotationX + 360) % 360
end