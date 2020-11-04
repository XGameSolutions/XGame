local _ENV = Namespace("UEVectorUtil")

forward = UE.Vector3.forward

function compareDist(startPos, endPos, dist)
    local dire = endPos - startPos
    local compareDist = dist * dist
    local sqrm = dire.sqrMagnitude
    if sqrm > dist then return 1
    elseif sqrm < dist then return -1
    else return 0 end
end

function getFace(currPos, destPos)
    if currPos == destPos then return 0 end
    local dire = (destPos - currPos).normalized
    return getDireAngle(dire)
end

function getAngle(currDire, destDire)
    if getCrossY(currDire, destDire) > 0 then
        return UE.Vector3.Angle(currDire, destDire)
    else
        return (360 - UE.Vector3.Angle(currDire, destDire) + 360) % 360
    end
end

function getDireAngle(dire)
    dire.y = 0
    return getAngle(forward, dire)
end

function getCrossY(v1, v2)
    return v1.z * v2.x - v1.x * v2.z
end

function tostring(v)
    return string.format("(%.1f, %.1f, %.1f)", v.x, v.y, v.z)
end