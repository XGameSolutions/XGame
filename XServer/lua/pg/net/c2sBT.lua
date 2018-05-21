
function c2s.btStartDebug(tbSocket,id)
    print("c2s.btStartDebug:",id)
    aiMgr.btStartDebug(id)
end

function c2s.btStopDebug(tbSocket,id)
    print("c2s.btStopDebug:",id)
    aiMgr.btStopDebug(id)
end

function c2s.btStart(tbSocket,id)
    print("c2s.btStart:",id)
    aiMgr.btStart(id)
end

function c2s.btPause(tbSocket,id)
    print("c2s.btPause:",id)
    aiMgr.btPause(id)
end

function c2s.btStop(tbSocket,id)
    print("c2s.btStop:",id)
    aiMgr.btStop(id)
end

function c2s.btSubTree(tbSocket,id,subTreeId)
    print("c2s.btSubTree:",id,subTreeId)
end