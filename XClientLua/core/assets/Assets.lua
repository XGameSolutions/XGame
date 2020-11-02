local _ENV = Namespace("Assets")

--------------------------------------------
-- 资源加载
-- 只提供异步加载接口，非AB加载的回调延时一帧调用。
-- 
-- 对外接口：
-- 
-- 
--------------------------------------------

_isAbRes = _isABRes or false

function isABMode()
    return _isAbRes
end

function init()
    print("Assets.init")
    AssetHelper.init()
    _isAbRes = CS.GameStart.Instance.IsAbRes
end

function load(abName, assetName, assetType, callback, ...)
    if abName == nil then
        return
    end
    if isABMode() then
        ABLoader.load(abName, assetName, assetType, callback, ...)
    else
        local paths = CS.UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(abName, assetName)
        if paths.Length > 0 then
            local asset = CS.UnityEditor.AssetDatabase.LoadAssetAtPath(paths[0], assetType)
            local args = Util.safePack(asset, ...)
            callback(Util.safeUnpack(args))
        else
            printWarn("can't find asset, load from ab:", abName, assetName)
            ABLoader.load(abName, assetName, nil, callback, ...)
        end
    end
end

function loadAB(abName, callback, ...)
    ABLoader.load(abName, nil, nil, callback, ...)
end

function loadPrefab(abName, assetName, callback, ...)
    load(abName, assetName, typeof(UE.GameObject), callback, ...)
end

function loadGameObject(abName, assetName, callback, ...)
    load(abName, assetName, typeof(UE.GameObject), callback, ...)
end

function loadTexture(abName, assetName, callback, ...)
end

function loadScene(sceneName, callback, ...)
    local abName = AssetHelper.getSceneABName(sceneName)
    if isABMode() then
        ABLoader.load(abName, nil, nil, callback, ...)
    else
        local paths = CS.UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(abName, sceneName)
        if paths.Length > 0 then
            callback(...)
        else
            printWarn("can't find scene, load from ab:", abName, sceneName)
            ABLoader.load(abName, nil, nil, callback, ...)
        end
    end
end

