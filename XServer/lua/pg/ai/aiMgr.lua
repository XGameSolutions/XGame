namespace("aiMgr")
require("btHeader")
require("ai.aiHeader")

tbBT = tbBT or {}

function aiMgr.test()
    print("ai test")
end

function aiMgr.init()
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

function aiMgr.addTestAi()
    local btree = bt.BehaviourTree.new()
    btree:load("test")
    btree.id = 1
    tbBT[btree.id] = btree
end

function aiMgr.btStartDebug(id)
end

function aiMgr.btStopDebug(id)
end

function aiMgr.btStart(id)
end

function aiMgr.btPause(id)
end

function aiMgr.btStop(id)
end