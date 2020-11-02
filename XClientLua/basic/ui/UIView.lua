local _ENV = Namespace("UIView")

--------------------------------------------
-- UI
-- view.gameObject
-- view.canvas
-- view.isOpen
-- view.onOpenView()
-- view.onCloseView()
-- 
--------------------------------------------

_uiCamera = _uiCamera or nil
_uiRoot = _uiRoot or nil

_views = _views or {}

function init()
    print("UIView.init")
    _uiRoot = UE.GameObject.Find("UIRoot")
    if not _uiRoot then
        _uiRoot = UE.GameObject("UIRoot")
        _uiRoot.transform.localPosition = UE.Vector3(0,1000,0)
    end
    UE.Object.DontDestroyOnLoad(_uiRoot)

    local uiCameraObj = UE.GameObject.Find("UICamera")
    if not uiCameraObj then
        uiCameraObj = UE.GameObject("UICamera")
        uiCameraObj.transform.localPosition = UE.Vector3(0,1000,0)
    end
    _uiCamera = Util.getOrAddComponent(uiCameraObj,typeof(UE.Camera))
    _uiCamera.clearFlags = UE.CameraClearFlags.Nothing
    _uiCamera.cullingMask = 1 << UE.LayerMask.NameToLayer("UI")
    _uiCamera.orthographic = true
    _uiCamera.depth = 100
    _uiCamera.tag = "UICamera"
    UE.Object.DontDestroyOnLoad(uiCameraObj)

    local eventSystem = UE.GameObject.Find("EventSystem")
    if not eventSystem then
        eventSystem = UE.GameObject("EventSystem")
        eventSystem:AddComponent(typeof(UE.EventSystems.EventSystem))
        eventSystem:AddComponent(typeof(UE.EventSystems.StandaloneInputModule))
    end
    UE.Object.DontDestroyOnLoad(eventSystem)
end

function openView(view, callback, ...)
    assert(view ~= nil, "view is nil, check require")
    assert(view.onOpenView ~= nil)
    local args = Util.safePack(...)
    Assets.loadPrefab(view.uiABName, view.uiAssetName, function(prefab)
        print("openView:",view.__spaceName__)
        local gameObject = UE.GameObject.Instantiate(prefab)
        UE.Object.DontDestroyOnLoad(gameObject)
        gameObject.name = "UI_" .. view.uiAssetName
        gameObject.transform:SetParent(_uiRoot.transform)
        gameObject.transform.localPosition = UE.Vector3(0,0,0)

        view.gameObject = gameObject
        view.index = 0
        view.name = view.__spaceName__
        view.canvas = Util.getOrAddComponent(gameObject, typeof(UE.Canvas))
        view.canvas.renderMode = UE.RenderMode.ScreenSpaceCamera
        view.canvas.worldCamera = _uiCamera
        view.uiType = view.uiType or UIConst.UIType.BaseUI
        view.isOpen = true
        _addView(view)

        if callback ~= nil then
            assert(type(callback) == "function")
            view.onOpenView()
            callback(Util.safeUnpack(args))
        else
            view.onOpenView(Util.safeUnpack(args))
        end
    end)
end

function closeView(view)
    assert(view.onCloseView ~= nil)
    view.isOpen = false
    UE.GameObject.DestroyImmediate(view.gameObject)
    table.remove(_views[view.uiType], view.index)
    view.onCloseView()
    print("closeView:",view.name, #_views[view.uiType])
end

function closeSpecifiedView(...)
    for i=1, select("#", ...) do
        local view = select(i, ...)
        closeView(view)
    end
end

function closeAllView()
    for k, list in pairs(_views) do
        for i=#list,1,-1 do
            closeView(list[i])
        end
    end
end

function closeAllViewButExclude(...)
    local exclude = {...}
    for k, list in pairs(_views) do
        for i=#list,1,-1 do
            if not TableUtil.contains(exclude, list[i]) then
                closeView(list[i])
            end
        end
    end
end

function _addView(view)
    if not _views[view.uiType] then
        _views[view.uiType] = {}
    end
    local list = _views[view.uiType]
    local flag, index = TableUtil.contains(list, view)
    if flag then
        table.remove(list, index)
        table.insert(list, view)
    else
        table.insert(list, view)
    end
    for k,view in pairs(list) do
        view.index = k
        if view.isOpen then
            view.canvas.sortingOrder = view.uiType * 1000 + k
        end
    end
end