namespace("aiMgr")

tbBT = tbBT or {}
tbDebug = tbDebug or {}

function test()
    print("ai test")
end

function init()
    require("btHeader")
    require("ai.aiHeader")
    local function tick()
        bt.time = bt.time + bt.deltaTime
        bt.runLoopFunc()
        for k,btree in pairs(tbBT) do
            if btree then
                btree:update()
                self:checkDebug()
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

function checkDebug()
end

function btStartDebug(id)
    local btree = tbBT[id]
    if not btree then return end
    btree.agent.isBTDebug = true
end

function btStopDebug(id)
    local btree = tbBT[id]
    if not btree then return end
    btree.agent.isBTDebug = false
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