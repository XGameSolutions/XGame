local _ENV = Namespace("Manifest")

_manifest = _manifest or nil

function init(cb)
    local abName = AssetHelper.getManifestName()
    Assets.loadAB(abName, function(ab)
        _manifest = ab:LoadAsset("AssetBundleManifest")
        if cb then
            cb()
        end
    end)
end

function getDepList(abName)
    if not _manifest then
        return nil
    end
    return _manifest:GetAllDependencies(abName)
end