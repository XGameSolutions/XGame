using System;
using NodeCanvas.Framework;
using UnityEngine;

public class DebugCallback
{
    public static Action<Graph, long> syncSubTreeCallback;

    public static void SyncSubTree(Graph graph, long id)
    {
        if (syncSubTreeCallback != null)
        {
            syncSubTreeCallback(graph, id);
        }
    }
}