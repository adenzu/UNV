using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Pathfinding : MonoBehaviour
{
    [SerializeField] private Transform _seeker;
    [SerializeField] private Transform _target;

    [SerializeField] private PathGrid _pathGrid;

    private void Update()
    {
        if (_pathGrid != null)
        {
            FindPath(_seeker.position, _target.position);
        }
    }

    private void FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        PathNode startNode = _pathGrid.PathNodeFromWorldPoint(startPosition);
        PathNode targetNode = _pathGrid.PathNodeFromWorldPoint(targetPosition);

        List<PathNode> nodesToVisit = new List<PathNode>();
        List<int> nodesToVisitFCost = new List<int>();

        void AddNeighbourToNodesToVisit(PathNode node, PathNode neighbour, int newCostToNeighbour)
        {
            neighbour.costFromStart = newCostToNeighbour;
            neighbour.costToTarget = GetDistance(neighbour, targetNode);
            neighbour.parent = node;
            int index = nodesToVisitFCost.BinarySearch(neighbour.fCost);
            if (index < 0)
            {
                nodesToVisit.Insert(~index, neighbour);
                nodesToVisitFCost.Insert(~index, neighbour.fCost);
            }
            else
            {
                nodesToVisit.Insert(index, neighbour);
                nodesToVisitFCost.Insert(index, neighbour.fCost);
            }
        }

        HashSet<PathNode> visitedNodes = new HashSet<PathNode>();

        nodesToVisit.Add(startNode);
        nodesToVisitFCost.Add(0);

        while (nodesToVisit.Count > 0)
        {
            PathNode node = nodesToVisit[0];

            nodesToVisit.RemoveAt(0);
            nodesToVisitFCost.RemoveAt(0);

            visitedNodes.Add(node);

            if (node == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (PathNode neighbour in _pathGrid.GetNeighbours(node))
            {
                if (!neighbour.walkable || visitedNodes.Contains(neighbour))
                {
                    continue;
                }
                int newCostToNeighbour = node.costFromStart + GetDistance(node, neighbour);
                if (!nodesToVisit.Contains(neighbour))
                {
                    AddNeighbourToNodesToVisit(node, neighbour, newCostToNeighbour);
                }
                else if (newCostToNeighbour < neighbour.costFromStart)
                {
                    nodesToVisit.Remove(neighbour);
                    nodesToVisitFCost.Remove(neighbour.fCost);
                    AddNeighbourToNodesToVisit(node, neighbour, newCostToNeighbour);
                }
            }
        }
    }

    private void RetracePath(PathNode startNode, PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        PathNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        _pathGrid.path = path;
    }

    private int GetDistance(PathNode nodeA, PathNode nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        const int sqrt2x10 = 14;
        const int sqrt1x10 = 10;

        return distanceX > distanceY ? sqrt2x10 * distanceY + sqrt1x10 * (distanceX - distanceY) : sqrt2x10 * distanceX + sqrt1x10 * (distanceY - distanceX);
    }
}
