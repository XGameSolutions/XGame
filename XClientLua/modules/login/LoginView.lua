
local _ENV = Namespace("LoginView")

uiABName    = "ui_login"
uiAssetName = "login"
uiType      = UIConst.UIType.BaseUI

_btnLogin = _btnLogin or nil

function onOpenView()
    _btnLogin = UIUtil.getComponent(gameObject, typeof(UE.UI.Button), "btnLogin")

    UIUtil.addBtnListener(_btnLogin, _onClickBtnLogin)
end

function onCloseView()
end

function _onClickBtnLogin()
    Map.enterBattle()
end