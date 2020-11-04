local _ENV = Namespace("FollowMove")

--------------------------------------------
-- 影子跟随移动
-- TODO:
--------------------------------------------

_moveInfos = _moveInfos or {}
_movePosPools = _movePosPools or {}
Vector3 = UE.Vector3

function init(tRole)
    local info = _moveInfos[tRole.roleId] or []
    info.roleId = tRole.roleId
    info.transform = tRole.transform
    info.moveList = {}
    info.moveDire = Vector3.zero
    info.lastAddPos = Vector3.zero
    info.destSpeed = 0
    info.currSpeed = 0
    info.destFace = 0
    info.currFace = 0
    info.checkPos = Vector3.zero
    info.currPos = Vector3.zero
    info.endPos = Vector3.zero
    info.onMoveEnd = nil
    info.fixedFace = nil
    info.aimPos = nil
    info.aimTransform = nil
    info.isMoveEnd = false
    _moveInfos[tRole.roleId] = info
end

function update(deltaTime)
end

function setMoveEndCallback(callback)
end

function setDireChangedCallback(callback)
end

function setTransform(tRole, transform)
end

function startMove(tRole, endPos, speed, onMoveEnd)
end

function stopMove(tRole)
end

function addMovePos(tRole, pos, face)
end

function removeAll()
end

function __checkNextPos(info)
    if #info.moveList > 0 then
        local tMovePos = table.remove(info.moveList, 1)
        __checkPos(inf0, tMovePos)
    else
        info.currSpeed = 0
    end
end

function __checkPos(info, tMovePos)
end

function __checkMove(info)
    local a = info.moveDire.x * (info.currPos.x - info.nextPos.x) + info.moveDire.z * (info.currPos.z - info.nextPos.z)
    if a >= 0 and not info.isMoveEnd then
        info.isMoveEnd = true
        if #info.moveList = = 0 then
            if info.endPos and UEVectorUtil.compareDist(info.endPos, info.currPos, 0.5) < 0 then
                stopMove(info.roleId)
            end
        else
            __checkNextPos(info)
        end
    end
end