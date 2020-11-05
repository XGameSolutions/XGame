local _ENV = Namespace("UIUtil")

UE_EventTrigger = UE.EventSystems.EventTrigger

function getComponent(transform, classtype, childPath)
    if childPath == nil then
        return transform.gameObject:GetComponent(classtype)
    else
        local childObj = transform.transform:Find(childPath)
        if not Util.isNil(childObj) then
            return childObj.gameObject:GetComponent(classtype)
        else
            printError(string.format("UI.getComponent: can't find %s in : %s, childPath : %s", 
                tostring(classtype), transform.name, childPath))
            return nil
        end
    end
end

function getGameobject(transform, childPath)
    if childPath == nil then
        return transform.gameObject
    end
    local tr = transform.transform:Find(childPath)
    if tr then 
        return tr.gameObject
    else
        printError(string.format("UI.getGameobject: can't find gameObject in : %s, childPath : %s", 
                tostring(classtype), transform.name, childPath))
        return nil
    end
end

function addBtnListener(btn, callback)
    assert(btn.onClick ~= nil)
    btn.onClick:AddListener(callback)
end

function addEventListener(gameObject, strEventType, callback)
    local eventTrigger = Util.getOrAddComponent(gameObject, typeof(UE_EventTrigger))
    local entry = UE_EventTrigger.Entry()
    local event = UE.EventSystems.EventTriggerType[strEventType]
    if event then
        entry.eventID = event
        entry.callback:AddListener(callback)
        eventTrigger.triggers:Add(entry)
    else
        printError(string.format("UI.addEventListener:  strEventType is nil"))
    end
end

function setActive(transform, flag, childPath)
    if childPath == nil then
        return transform.gameObject:SetActive(flag)
    end
    local tr = transform.transform:Find(childPath)
    if tr then 
        return tr.gameObject:SetActive(flag)
    else
        printError(string.format("UI.setActive: can't find gameObject in : %s, childPath : %s", 
                tostring(classtype), transform.name, childPath))
        return nil
    end
end