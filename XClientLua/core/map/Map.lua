local _ENV = Namespace("Map")

function enterCity()
    Scene.switchSceneByName("city", function()
        _onEnterCity()
    end)
end


function enterBattle()
    Scene.switchSceneByName("battle", function()
        _onEnterBattle()
    end)
end

function _onEnterCity()
    RoleGen.createLocalPlayer(1)
    UIView.openView(MainView)
end

function _onEnterBattle()
    RoleGen.createLocalPlayer(1)
    UIView.openView(MainView)
end