local _ENV = Namespace("ABLoader")

_commom = _commom or {}
_cached = _cached or {}
_infos = _infos or {}
_waitings = _waitings or {}
_loadings = _loadings or {}
_needLoad = _needLoad or 0
_currLoad = _currLoad or 0

MAX_LOAD_NUM = 1

function update(deltaTime)
    local num = #_loadings
    if num > 0 then
        for i=num,1,-1 do
            local info = _loadings[i]
            if info.www.isDone then
                table.remove(_loadings, i)
                _loadDone(info)
            end
        end
    end
    if num < MAX_LOAD_NUM then
        _checkNextLoad()
    end
end

function load(abName, assetName, assetType, callback, ...)
    print("try load:",abName, assetName)
    local info = _getInfo(abName)
    if callback then
        local asset = {assetName = assetName, assetType = assetType, callback = callback, args = Util.safePack(nil, ...)}
        table.insert(info.assets, asset)
    end
    _loadDep(abName)
    table.insert(_waitings, info)
end

function _getInfo(abName)
    if _infos[abName] then
        return _infos[abName]
    end
    local info = {}
    info.abName = abName
    info.path = AssetHelper.getABPath(abName)
    info.assets = {}
    info.nDep = 0
    _infos[abName] = info
    return info
end

function _loadDep(abName)
    local list = Manifest.getDepList(abName)
    if not list or list.Length <= 0 then return end
    for i=0,list.Length do
        local depName = list[i]
        local info = _getInfo(depName)
        _loadDep(depName)
        table.insert(_waitings, info)
    end
end

function _checkNextLoad()
    if #_waitings <= 0 then return end
    local info = table.remove(_waitings,1)
    info.www = UE.AssetBundle.LoadFromFileAsync(info.path)
    print("loading:",info.abName)
    table.insert(_loadings, info)
end

function _loadError(info)
end

function _loadDone(info)
    local ab = info.www.assetBundle
    if not ab then
        printError(string.format("loadDone ERROR: ab is nil, abName = %s", info.abName))
    else
        print("loadDone:",info.abName)
        if not _cached[info.abName] then
            _cached[info.abName] = ab
        end
    end
    if #info.assets > 0 then
        for k,cbinfo in pairs(info.assets) do
            if cbinfo.assetName == nil or ab == nil then
                cbinfo.args[1] = ab
                cbinfo.callback(Util.safeUnpack(cbinfo.args))
            else
                local asset = nil
                if cbinfo.assetType then
                    asset = ab:LoadAsset(cbinfo.assetName, cbinfo.assetType)
                else
                    asset = ab:LoadAsset(cbinfo.assetName)
                end
                cbinfo.args[1] = asset
                cbinfo.callback(Util.safeUnpack(cbinfo.args))
            end
        end
        info.assets = {}
    end
    if info.www then
        info.www = nil
    end
end
