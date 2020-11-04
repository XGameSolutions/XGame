local _ENV = Namespace("LocalPlayer")

_rotationX = _rotationX or 0
_rotationY = _rotationY or 0 
_mouseSpeed = _mouseSpeed or 5

function update(deltaTime)
    if not Role.localPlayer then return end
    LocalPlayerMove.update(deltaTime)
    checkDirection(Role.localPlayer)
end

function checkDirection(tRole)
    local mouseX = Input.getMouseX()
    if mouseX == 0 then return end
    _rotationX = _rotationX + mouseX * _mouseSpeed
    tRole.transform.localEulerAngles = UE.Vector3(0, _rotationX, 0)
    tRole.face = (_rotationX + 360) % 360
end