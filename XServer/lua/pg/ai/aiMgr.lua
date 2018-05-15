namespace("aiMgr")

tbBT = tbBT or {}

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
            end
            if bt.time > 20 then
                if btree then
                    --btree:destroy()
                    --btree = nil
                end
            end
        end
    end
    xd.addTimer(0,bt.deltaTime * 1000,tick)
end

function addTestAi()
    local btree = bt.BehaviourTree.new()
    btree:load("test")
    btree.id = 1
    tbBT[btree.id] = btree
end

function btStartDebug(id)
end

function btStopDebug(id)
end

function btStart(id)
end

function btPause(id)
end

function btStop(id)
end