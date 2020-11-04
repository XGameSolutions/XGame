local _ENV = Namespace("CameraMgr")

_mainCameraRoot = _mainCameraRoot or nil
_mainCamera = _mainCamera or nil
_followTarget = _followTarget or nil
_cameraVerticalAngle = _cameraVerticalAngle or 0
_rotationSpeed = _rotationSpeed or 200

_rotationX = _rotationX or 0
_rotationY = _rotationY or 0 
_mouseSpeed = _mouseSpeed or 5

_DIST_H = 7
_DIST_V = 4

function init()
    _mainCameraRoot = UE.GameObject.Find("Main Camera")
    if not _mainCameraRoot then
        _mainCameraRoot = UE.GameObject("Main Camera")
        _mainCameraRoot.transform.localPosition = UE.Vector3(0,1000,0)

        _mainCamera = Util.getOrAddComponent(uiCameraObj,typeof(UE.Camera))
        _mainCamera.clearFlags = UE.CameraClearFlags.Nothing
        --_mainCamera.cullingMask = 1 << UE.LayerMask.NameToLayer("UI")
        _mainCamera.orthographic = false
        _mainCamera.depth = 100
        _mainCamera.tag = "UICamera"
    end
    UE.Object.DontDestroyOnLoad(_mainCameraRoot)
end

function setTarget(transform)
    _followTarget = transform
end

function lateUpdate(deltaTime)
    if not _followTarget then return end

    local pos = _followTarget.forward * -1 * _DIST_H + _followTarget.up * _DIST_V + _followTarget.position
    
    _mainCameraRoot.transform.position = pos
    _mainCameraRoot.transform:LookAt(_followTarget)

    local eulerY = _mainCameraRoot.transform.localEulerAngles.y
    _rotationY = _rotationY + Input.getMouseY() * _mouseSpeed
    _rotationY = UE.Mathf.Clamp(_rotationY, -15, 15)
    _mainCameraRoot.transform.localEulerAngles = UE.Vector3(-_rotationY, eulerY, 0)
end