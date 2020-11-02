local _ENV = Namespace("AssetHelper")

_abPath = _abPath or nil
_abPathURL = _abPathURL or nil
_abPlatform = _abPath or nil
_patchPath = _patchPath or nil
_protocal = _protocal or nil

function init()
    _abPath = CS.ABMgr.abPath
    _abPlatform = CS.ABMgr.abPlatform
    _patchPath = CS.ABMgr.patchPath
    _protocal = CS.ABMgr.protocal
    _abPathURL = string.format("%s%s", _protocal, abPath)
    print(" CS.ABMgr:", CS.ABMgr)
    print("_abPlatform:",_abPlatform)
    print("_abPath:",_abPath)
    print("_patchPath:",_patchPath)
end

function getABPath(abName)
    local patchPath = _patchPath .. abName
    if Util.isFileExist(patchPath) then
        return patchPath
    else
        return _abPath .. abName
    end
end

function getSceneABName(sceneName)
    return Config.AB_SCENE_PREFIX .. string.lower(sceneName)
end

function getManifestName()
    return _abPlatform
end