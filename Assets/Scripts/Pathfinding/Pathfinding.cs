using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance { get; private set; }

    public static int NeighbourRadius = 1;

    public delegate Vector2 GetDefaultDirectionDelegate();
    public delegate int AngleCostFunctionDelegate(float angle);

    public static GetDefaultDirectionDelegate GetDefaultDirection;
    public static AngleCostFunctionDelegate AngleCostFunction;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void StartFindingPath(Vector3 startPosition, Vector3 targetPosition)
    {
        Instance.StartCoroutine(Instance.FindPath(startPosition, targetPosition));
    }

    private IEnumerator FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        PathNode startNode = PathGrid.PathNodeFromWorldPoint(startPosition);
        PathNode targetNode = PathGrid.PathNodeFromWorldPoint(targetPosition);

        if (targetNode.walkable)
        {
            HashSet<PathNode> visitedNodes = new HashSet<PathNode>();
            MinHeap<PathNode> nodesToVisit = new MinHeap<PathNode>();
            nodesToVisit.Add(startNode);

            void AddNeighbourToNodesToVisit(PathNode node, PathNode neighbour, int newCostToNeighbour)
            {
                neighbour.costFromStart = newCostToNeighbour;
                neighbour.costToTarget = GetDistanceCost(neighbour, targetNode);
                neighbour.parent = node;
                nodesToVisit.Add(neighbour);
            }

            while (nodesToVisit.Count > 0)
            {
                PathNode node = nodesToVisit.Pop();
                visitedNodes.Add(node);

                if (node == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (PathNode neighbour in PathGrid.GetNeighbours(node, NeighbourRadius))
                {
                    if (!neighbour.walkable || visitedNodes.Contains(neighbour))
                    {
                        continue;
                    }
                    int angleCost = GetAngleCost(node, neighbour);
                    if (angleCost == int.MaxValue)
                    {
                        continue;
                    }
                    int newCostToNeighbour = node.costFromStart + GetDistanceCost(node, neighbour) + angleCost;
                    if (!nodesToVisit.Contains(neighbour))
                    {
                        AddNeighbourToNodesToVisit(node, neighbour, newCostToNeighbour);
                    }
                    else if (newCostToNeighbour < neighbour.costFromStart)
                    {
                        nodesToVisit.Remove(neighbour);
                        AddNeighbourToNodesToVisit(node, neighbour, newCostToNeighbour);
                    }
                }
            }
        }

        yield return null;

        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }

        PathRequestManager.OnPathProcessFinish(waypoints, pathSuccess);
    }

    private Vector3[] RetracePath(PathNode startNode, PathNode endNode, bool simplify = true)
    {
        List<PathNode> path = new List<PathNode>();
        PathNode currentNode = endNode;
        path.Add(endNode);
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        if (simplify)
        {
            return SimplifyPath(path).Reverse().ToArray();
        }
        else
        {
            return path.Select(node => node.worldPosition).Reverse().ToArray();
        }
    }

    private Vector2 GetDirection(PathNode nodeA, PathNode nodeB)
    {
        return new Vector2(nodeB.gridX - nodeA.gridX, nodeB.gridY - nodeA.gridY);
    }

    private Vector3[] SimplifyPath(List<PathNode> path)
    {
        List<Vector3> waypoints = new List<Vector3>();

        waypoints.Add(path[0].worldPosition);

        for (int i = 1; i < path.Count - 1; i++)
        {
            Vector2 directionTo = GetDirection(path[i - 1], path[i]);
            Vector2 directionFrom = GetDirection(path[i], path[i + 1]);
            if (directionTo != directionFrom)
            {
                waypoints.Add(path[i].worldPosition);
            }
        }

        waypoints.Add(path[path.Count - 1].worldPosition);
        return waypoints.ToArray();
    }

    private int GetDistanceCost(PathNode nodeA, PathNode nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        const int diagonalCostCoefficient = 14;
        const int straightCostCoefficient = 10;

        return distanceX > distanceY ? diagonalCostCoefficient * distanceY + straightCostCoefficient * (distanceX - distanceY) : diagonalCostCoefficient * distanceX + straightCostCoefficient * (distanceY - distanceX);
    }

    private int GetAngleCost(PathNode nodeA, PathNode nodeB)
    {
        Vector2 comingDirection = nodeA.parent == null ? GetDefaultDirection.Invoke() : GetDirection(nodeA.parent, nodeA);
        Vector2 goingDirection = GetDirection(nodeA, nodeB);
        return AngleCostFunction.Invoke(Vector2.Angle(comingDirection, goingDirection));
    }

    private class MinHeap<T>
    {
        private List<T> _list;

        public int Count => _list.Count;

        public MinHeap()
        {
            _list = new List<T>();
        }

        public int Add(T item)
        {
            int index = _list.BinarySearch(item);
            if (index < 0)
            {
                index = ~index;
            }
            _list.Insert(index, item);
            return index;
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void Remove(T item)
        {
            _list.Remove(item);
        }

        public T Peek()
        {
            return _list[0];
        }

        public T Pop()
        {
            T item = _list[0];
            _list.RemoveAt(0);
            return item;
        }

        public void Clear()
        {
            _list.Clear();
        }
    }
}
