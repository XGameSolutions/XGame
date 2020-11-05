local _ENV = Namespace("Launcher")

--------------------------------------------
-- 游戏启动流程
--
--------------------------------------------

function start()
    LaunchView.setLeftText("init...")
    Assets.init()
    UIView.init()
    CameraMgr.init()
    step1_loadManifest()
end

function step1_loadManifest()
    print("step1_loadManifest")
    Manifest.init(function()
        step2_loadShader()
    end)
end

function step2_loadShader()
    print("====step2_loadShader")
    step3_()
end

function step3_()
    step4_loginScene()
end

function step4_loginScene()
    Scene.switchSceneByName("login",function()
        step5_loginView()
    end)
end

function step5_loginView()
    UIView.openView(LoginView, function()
        LaunchView.setProgress(100)
        GameMain.init()
    end)
end

