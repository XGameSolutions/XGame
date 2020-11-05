local _ENV = Namespace("Input")


_lookSensitivity = _lookSensitivity or 1.0
_invertYAxis = _invertYAxis or false
_invertXAxis = _invertXAxis or false

AXIS_HORIZONTAL = "Horizontal"
AXIS_VERTICAL = "Vertical"

k_MouseAxisNameVertical     = "Mouse Y"
k_MouseAxisNameHorizontal   = "Mouse X"
k_AxisNameJoystickLookVertical      = "Look Y"
k_AxisNameJoystickLookHorizontal    = "Look X"

function init()
    --UE.Curor.visible = false
    --UE.Curor.lockState = UE.CursorLockMode.Locked
end

function canInput()
    return true
end

function getMoveDire()
    --TODO:
    return UE.Vector3(UE.Input.GetAxis(AXIS_HORIZONTAL), 0, UE.Input.GetAxis(AXIS_VERTICAL))
end

function getMouseX()
    return UE.Input.GetAxis(k_MouseAxisNameHorizontal)
end

function getMouseY()
    return UE.Input.GetAxis(k_MouseAxisNameVertical)
end

function getMouseOrStickLookAxis(mouseInputName, stickInputName)
    if canInput() then
        local isGamepad = UE.Input.GetAxis(stickInputName) ~= 0
        local i = 0
        if isGamepad then
            i = UE.Input.GetAxis(stickInputName)
        else
            i = UE.Input.GetAxis(mouseInputName)
        end
        if _invertYAxis then
            i = i * -1.0
        end
        i = i * _lookSensitivity
        if isGamepad then
            i = i * UE.Time.deltaTime
        else
            i = i * 0.01
        end
        return i
    else
        return 0
    end
end

function getLookInputsVertical()
    return getMouseOrStickLookAxis(k_MouseAxisNameVertical, k_AxisNameJoystickLookVertical)
end

function getLookInputsHorizontal()
    return getMouseOrStickLookAxis(k_MouseAxisNameHorizontal, k_AxisNameJoystickLookHorizontal)
end