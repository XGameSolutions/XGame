
function btStartDebug(id)
    print("btStartDebug:",id)
    c2s.btStartDebug(g_socket,id)
end

function btStopDebug(id)
    print("btStopDebug:",id)
    c2s.btStopDebug(g_socket,id)
end

function btStart(id)
    c2s.btStart(g_socket,id)
end

function btPause(id)
    c2s.btPause(g_socket,id)
end

function btStop(id)
    c2s.btStop(g_socket,id)
end

function btSubTree(id,subTreeId)
    c2s.btSubTree(id,subTreeId)
end
