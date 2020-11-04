
local _ENV = Namespace("LoginView")

uiABName    = "ui_login"
uiAssetName = "login"
uiType      = UIConst.UIType.BaseUI

_btnLogin = _btnLogin or nil

function onOpenView()
    _btnLogin = UIUtil.getComponent(gameObject, typeof(UE.UI.Button), "btnLogin")

    UIUtil.addListener(_btnLogin, _onClickBtnLogin)
end

function onCloseView()
end

function _onClickBtnLogin()
    print("click btn login")
    --UIView.openView(LoadingView)
    Scene.switchSceneByName("battle", function()
        print("switch to scene: battle")
        RoleGen.createLocalPlayer(1)
    end)
end