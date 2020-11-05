local _ENV = Namespace("Patcher")

--------------------------------------------
-- 补丁更新系统
-- 
--------------------------------------------

_patchEndCallback = _patchEndCallback or nil

function check(cb)
    LaunchView.setLeftText("check patch ...")
    _patchEndCallback = cb
    --TODO:
    _pathDone()
end

function update(deltaTime)
end

function _step1_localVersion()
end

function _step2_remoteVersion()
end

function _pathDone()
    if _patchEndCallback then
        _patchEndCallback()
    end
end