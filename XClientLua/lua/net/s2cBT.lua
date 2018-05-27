
function s2c.btInfo(tbSocket,info)
    print("btInfo:",info)
    local tbInfo = tableTool.strToTable(info)
    local btObj = CS.UnityEngine.GameObject.Find('bt_test')
    local btSrc = btObj:GetComponent("BehaviourTreeOwner")
    for k,v in pairs(tbInfo) do
        local nodeId = k - 1
        local status = v[1]
        btSrc:UpdateNodeStatus(nodeId,status)
        for i = 2,#v do
            btSrc:UpdateNodeConnectionStatus(nodeId,i-2,v[i])
        end
    end
end