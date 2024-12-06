using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    Grid grid;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    public void FindPath(PathRequest pathRequest, Action<PathResult> callback)
    {
        Vector3[] waypointsArray = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFormWorldPoint(pathRequest.pathStart);
        Node targetNode = grid.NodeFormWorldPoint(pathRequest.pathEnd);

        if (startNode.isWalkable && targetNode.isWalkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closeSet = new HashSet<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closeSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in currentNode.neighboursList)
                {
                    if (!neighbour.isWalkable || closeSet.Contains(neighbour)) continue;

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + currentNode.movementPenalty;

                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }

                }
            }
        }

        if (pathSuccess)
        {
            waypointsArray = RetracePath(startNode, targetNode);
            pathSuccess = waypointsArray.Length > 0;
        }

        callback(new PathResult(waypointsArray, pathSuccess, pathRequest.callback));
    }

    private Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] waypointsArray = SimplifyPath(path);
        Array.Reverse(waypointsArray);

        return waypointsArray;

    }

    private Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypointsList = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);

            /*
            if (directionNew != directionOld)
            {
                waypointsList.Add(path[i].worldPosition);
            }
             */

            // adjust waypoints number
            if ((i % 6) == 0)
            {
                waypointsList.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }

        return waypointsList.ToArray();
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);

        return 14 * distX + 10 * (distY - distX);
    }
}
