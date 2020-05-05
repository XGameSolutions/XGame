namespace("aiMgr")

tbBT = tbBT or {}

function test()
    print("ai test")
end

function init()
    require("bt.btHeader")
    require("ai.aiHeader")
    require("ai.api.server.APIAction")
    require("ai.api.server.APICondition")
    bt.ASSERT_DIR = "../../LuaBT/test/"
    local function tick()
        bt.time = bt.time + bt.deltaTime
        bt.runLoopFunc()
        for k,btree in pairs(tbBT) do
            if btree then
                btree:update()
                btree:checkDebugger()
            end
        end
    end
    xd.addTimer(0,bt.deltaTime * 1000,tick)
end

function addTestAi()
    local btree = bt.BehaviourTree.new()
    btree:load("test")
    btree:start()
    btree.id = 1
    tbBT[btree.id] = btree
end

function btStartDebug(id,tbSocket)
    local btree = tbBT[id]
    if not btree then return end
    btree:addDebugger(tbSocket)
end

function btStopDebug(id,tbSocket)
    local btree = tbBT[id]
    if not btree then return end
    btree:delDebugger(tbSocket)
end

function btStart(id)
    local btree = tbBT[id]
    if not btree then return end
    btree:start()
end

function btPause(id)
    local btree = tbBT[id]
    if not btree then return end
    btree:pause()
end

function btStop(id)
    local btree = tbBT[id]
    if not btree then return end
    btree:stop()
end