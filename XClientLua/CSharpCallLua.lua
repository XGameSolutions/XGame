--------------------------------------------
-- C#调用Lua的接口
-- 统一都放在这里，尽量不要做太多处理，分发出去
-- 
--------------------------------------------

function update(deltaTime, unscaledDeltaTime)
    LaunchView.update(deltaTime)
    GameMain.update(deltaTime, unscaledDeltaTime)
end

function lateUpdate(deltaTime, unscaledDeltaTime)
    GameMain.lateUpdate(deltaTime)
end

function fixedUpdate(fixedDeltaTime)
end

