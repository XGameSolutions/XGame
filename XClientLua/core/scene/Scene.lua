local _ENV = Namespace("Scene")

--------------------------------------------
-- 场景管理
-- 
-- 对外接口：
-- switchScene(nSceneId)
-- switchSceneByName(strSceneName, onCompeleted, onLoading)
--
--------------------------------------------

_isLoading = _isLoading or false
_asyncOperation = _asyncOperation or nil
_onCompeleted = _onCompeleted or nil
_onLoading = _onLoading or nil

-- 场景切换前的回调
function _onBeforeScene()
    LocalPlayerMove.stopWork()
    UIView.closeAllViewButExclude(LoadingView)
    Util.gcAll()
end

-- 场景切换完成后的回调
function _onAfterScene()
    Util.gcAll()
    LocalPlayerMove.startWork()

end

function _loadSceneCompeleted()
    _onAfterScene()
    _isLoading = false
    _asyncOperation = nil
    UIView.closeView(LoadingView)
    if _onCompeleted then
        _onCompeleted()
    end
end

function update(deltaTime)
    if not _isLoading then return end
    if _onLoading then _onLoading() end
    if _asyncOperation then
        if _asyncOperation.progress >= 0.89 then
            _asyncOperation.allowSceneActivation = true
            if _asyncOperation.isDone then
                _loadSceneCompeleted()
            end
        end
    end
end

function switchScene(nSceneId)
    --TODO: switchSceneByName
end

function switchSceneByName(strSceneName, onCompeleted, onLoading)
    _isLoading = true
    _onCompeleted = onCompeleted
    _onLoading = onLoading
    _asyncOperation = nil

    UIView.openView(LoadingView, function()
        _onBeforeScene()
        Assets.loadScene("empty", function()
            UE.SceneManagement.SceneManager.LoadScene("empty")
            Util.gcAll()
            Assets.loadScene(strSceneName, function()
                _asyncOperation = UE.SceneManagement.SceneManager.LoadSceneAsync(strSceneName)
                _asyncOperation.allowSceneActivation = false
            end)
        end)
    end)
end
