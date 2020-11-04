
local _ENV = Namespace("LoadingView")

uiABName    = "ui_loading"
uiAssetName = "loading"
uiType      = UIConst.UIType.BaseUI

_slider     = _slider or nil
_txtLeft    = _txtLeft or nil
_txtRight   = _txtRight or nil
_isActive   = _isActive or false

_destProgress = _destProgress or 0
_currProgress = _currProgress or 0
_closeCallback = _closeCallback or 0

function onOpenView()
    print("LoginView:openView")
    _slider     = gameObject.transform:Find("loading/slider").gameObject:GetComponent(typeof(UE.UI.Slider))
    _txtLeft    = gameObject.transform:Find("loading/txtLeft").gameObject:GetComponent(typeof(UE.UI.Text))
    _txtRight   = gameObject.transform:Find("loading/txtRight").gameObject:GetComponent(typeof(UE.UI.Text))
    _slider.value = 0
    _txtLeft.text = "loading"

    UIUtil.setActive(gameObject, false, "version")
end

function onCloseView()
end

function update(deltaTime)
    if not _isActive then return end
    if _currProgress < _destProgress then
        _currProgress = _currProgress + deltaTime * 50
        _slider.value = _currProgress / 100
    else
        _slider.value = _destProgress / 100
        if _slider.value >= 1 then
            closeView()
        end
    end
end

function openView()
    _isActive = true
    _gameObject:SetActive(false)
    _currProgress = 0
    _slider.value = 0
end

function closeView()
    _isActive = false
    _gameObject:SetActive(false)
    if _closeCallback then
        _closeCallback()
        _closeCallback = nil
    end
end

function setProgress(rate, cb)
    _destProgress = rate * 1.0
    _closeCallback = cb
end
