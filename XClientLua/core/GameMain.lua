local _ENV = Namespace("GameMain")

--------------------------------------------
-- 游戏主循环
--
--------------------------------------------

function update(deltaTime, unscaledDeltaTime)
    if not _G.isPatchEnd then return end
    Scene.update(deltaTime)
    ABLoader.update(deltaTime)
    RoleGen.update(deltaTime)
    LocalPlayer.update(deltaTime)
end

function lateUpdate(deltaTime, unscaledDeltaTime)
    if not _G.isPatchEnd then return end
    CameraMgr.lateUpdate(deltaTime)
end

function init()
    Input.init()
end



