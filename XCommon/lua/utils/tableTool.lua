namespace("tableTool")

function strToTable(str)
    if str == nil or type(str) ~= "string" then
        return
    end
    return load("return " .. str)()
end