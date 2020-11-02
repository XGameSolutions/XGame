local _ENV = Namespace("CameraMgr")

_mainCameraRoot = _mainCameraRoot or nil
_mainCamera = _mainCamera or nil

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