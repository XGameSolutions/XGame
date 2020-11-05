
local _ENV = Namespace("MainView")

uiABName    = "ui_main"
uiAssetName = "main"
uiType      = UIConst.UIType.BaseUI

_dragArea = _dragArea or nil

_joystickDragId = _joystickDragId or nil
_cameraDragId = _cameraDragId or nil
_cameraLastDragPos = _cameraLastDragPos or nil

function onOpenView()
    _dragArea = UIUtil.getGameobject(gameObject, "panel/drag_area")

    UIUtil.addEventListener(_dragArea, "BeginDrag", function(e) _onBeginDragArea(e) end)
    UIUtil.addEventListener(_dragArea, "Drag", function(e) _onDragArea(e) end)
    UIUtil.addEventListener(_dragArea, "EndDrag", function(e) _onEndDragArea(e) end)
end

function onCloseView()
end

function _isJoystickArea(pos)
    return pos.x < UE.Screen.width / 2
end

function _isCameraArea(pos)
    return pos.x >= UE.Screen.width / 2
end

function _onBeginDragArea(eventData)
    if _isJoystickArea(eventData.position) then
        _joystickDragId = eventData.pointerId
    else
        if not _cameraDragId then
            _cameraDragId = eventData.pointerId
            _cameraLastDragPos = eventData.position
        end
    end
end

function _onDragArea(eventData)
    if eventData.pointerId == _joystickDragId then
        
    elseif eventData.pointerId == _cameraDragId then
        local delta = eventData.position - _cameraLastDragPos
        LocalPlayer.updateDire(delta)
        CameraMgr.dragMove(delta)
        _cameraLastDragPos = eventData.position
    end
end

function _onEndDragArea(eventData)
    if eventData.pointerId == _joystickDragId then
        _joystickDragId = nil
        -- TODO:
    elseif eventData.pointerId == _cameraDragId then
        _cameraDragId = nil
    end
end

