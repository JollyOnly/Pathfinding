using System;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    Queue<PathResult> pathResults = new Queue<PathResult>();

    private static PathRequestManager instance;
    Pathfinding pathfinding;

    bool isProcessingPath;

    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    private void Update()
    {
        if (pathResults.Count > 0)
        {
            int itemsInQueue = pathResults.Count;
            lock (pathResults)
            {
                for (int i = 0; i < itemsInQueue; i++)
                {
                    PathResult result = pathResults.Dequeue();
                    result.callback(result.path, result.success);
                }
            }
        }
    }
    public static void RequestPath(PathRequest pathRequest)
    {
        ThreadStart threadStart = delegate
        {
            instance.pathfinding.FindPath(pathRequest, instance.FinishProcessingPath);
        };

        threadStart.Invoke();

    }

    public void FinishProcessingPath(PathResult result)
    {
        lock (pathResults)
        {
            pathResults.Enqueue(result);
        }
    }

}

public struct PathResult
{
    public Vector3[] path;
    public bool success;
    public Action<Vector3[], bool> callback;

    public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback)
    {
        this.path = path;
        this.success = success;
        this.callback = callback;
    }
}

public struct PathRequest
{
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public Action<Vector3[], bool> callback;

    public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        this.pathStart = pathStart;
        this.pathEnd = pathEnd;
        this.callback = callback;
    }
}
