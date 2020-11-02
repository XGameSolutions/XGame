Namespace("TableUtil")

function strToTable(str)
    if str == nil or type(str) ~= "string" then
        return
    end
    return load("return " .. str)()
end

function contains(tb, value)
    for k, v in pairs(tb) do
        if v == value then return true, k end
    end
    return false, 0
end