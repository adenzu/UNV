using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace UNV.Pathfinding
{
    public class Pathfinding : MonoBehaviour
    {
<<<<<<< Updated upstream:Assets/Scripts/FrameworkSource/Pathfinding/Pathfinding.cs
        [SerializeField] private PathRequestManager pathRequestManager;
        [SerializeField] private GridManager gridManager;


        public float angleChangeOverDistance;
=======
<<<<<<< Updated upstream:Assets/Scripts/Pathfinding/Pathfinding.cs
        public static Pathfinding Instance { get; private set; }
>>>>>>> Stashed changes:Assets/Scripts/Pathfinding/Pathfinding.cs

        public delegate Vector2 GetDefaultDirectionDelegate();
        public GetDefaultDirectionDelegate GetDefaultDirection;

<<<<<<< Updated upstream:Assets/Scripts/FrameworkSource/Pathfinding/Pathfinding.cs
=======
        public static GetDefaultDirectionDelegate GetDefaultDirection;
        // public static AngleCostFunctionDelegate AngleCostFunction;
        public static float angleChangeOverDistance;
=======
        [SerializeField] private PathRequestManager pathRequestManager;
        [SerializeField] private TilingManager tilingManager;

        [SerializeField] private MonoScript pathfindingAlgorithmScript;

        public bool InitializedPathfindingAlgorithm { get; private set; } = false;

        private IPathfindingAlgorithm pathfindingAlgorithm;

        private void OnValidate()
        {
            if (!Util.HandleMonoScriptFieldClassInheritance(pathfindingAlgorithmScript, typeof(IPathfindingAlgorithm), "Pathfinding Algorithm"))
            {
                pathfindingAlgorithmScript = null;
            }
        }

        private void Awake()
        {
            if (pathfindingAlgorithmScript != null)
            {
                pathfindingAlgorithm = Util.InstantiateMonoScriptObject<IPathfindingAlgorithm>(pathfindingAlgorithmScript);
                SetupPathfindingAlgorithm();
            }
        }

        private void SetupPathfindingAlgorithm()
        {
            if (tilingManager == null)
            {
                Debug.LogError("Tiling Manager is not set in Pathfinding");
                return;
            }
            if (tilingManager.Tiling == null)
            {
                Invoke(nameof(SetupPathfindingAlgorithm), 0.1f);
                return;
            }
            pathfindingAlgorithm.SetTiling(tilingManager.Tiling);
            InitializedPathfindingAlgorithm = true;
        }

        public void SetParameters(params object[] parameters)
        {
            pathfindingAlgorithm.SetParameters(parameters);
        }
>>>>>>> Stashed changes:Assets/Scripts/FrameworkSource/Pathfinding/Pathfinding.cs
>>>>>>> Stashed changes:Assets/Scripts/Pathfinding/Pathfinding.cs

        public void StartFindingPath(Vector3 startPosition, Vector3 targetPosition)
        {
            StartCoroutine(FindPath(startPosition, targetPosition));
        }

        private IEnumerator FindPath(Vector3 startPosition, Vector3 targetPosition)
        {
<<<<<<< Updated upstream:Assets/Scripts/Pathfinding/Pathfinding.cs
            Vector3[] waypoints = new Vector3[0];
            bool pathSuccess = false;

            NodeBase startNode = gridManager.GetAt(startPosition);
            NodeBase targetNode = gridManager.GetAt(targetPosition);

            startNode.angleRange = 2 * angleChangeOverDistance;

            if (targetNode.walkable)
            {
                HashSet<NodeBase> visitedNodes = new();
                MinHeap<NodeBase> nodesToVisit = new();
                nodesToVisit.Add(startNode);

                void AddNeighbourToNodesToVisit(NodeBase node, NodeBase neighbour, int newCostToNeighbour)
                {
                    Vector3 nodePosition = node.worldPosition;
                    Vector3 neighbourPosition = neighbour.worldPosition;
                    Vector2 nodeDifference = node.pathParent == null ? GetDefaultDirection.Invoke() : (nodePosition - node.pathParent.worldPosition).XZ();
                    Vector2 neighbourDifference = (neighbourPosition - nodePosition).XZ();

                    const float directionEqualityCap = 5f;
                    const float angleRangeCap = 100f;

                    float angle = Vector2.Angle(nodeDifference, neighbourDifference);
                    float angleRangeChange = angleChangeOverDistance * neighbourDifference.magnitude * 2;

                    if (angle < directionEqualityCap)
                    {
                        if (node.angleRange + angleRangeChange <= angleRangeCap)
                        {
                            neighbour.angleRange = node.angleRange + angleRangeChange;
                        }
                    }
                    else
                    {
                        neighbour.angleRange = angleRangeChange;
                    }

                    neighbour.costFromStart = newCostToNeighbour;
                    neighbour.costToTarget = GetDistanceCost(neighbour, targetNode);

                    neighbour.pathParent = node;
                    nodesToVisit.Add(neighbour);
                }

                while (nodesToVisit.Count > 0)
                {
                    NodeBase node = nodesToVisit.Pop();
                    visitedNodes.Add(node);

                    if (node == targetNode)
                    {
                        pathSuccess = true;
                        break;
                    }

                    foreach (NodeBase neighbour in gridManager.GetNeighbours(node))
                    {
                        if (
                            !neighbour.walkable ||
                            visitedNodes.Contains(neighbour) ||
                            (neighbour != targetNode && !CanTurnInTime(node, neighbour))
                        )
                        {
                            continue;
                        }
                        int angleCost = GetAngleCost(node, neighbour);
                        int distanceCost = GetDistanceCost(node, neighbour);
                        int newCostToNeighbour = node.costFromStart + distanceCost + angleCost;
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
=======
            Vector3[] waypoints = pathfindingAlgorithm.FindPath(startPosition, targetPosition);
            bool pathSuccess = waypoints != null && waypoints.Length > 0;
>>>>>>> Stashed changes:Assets/Scripts/FrameworkSource/Pathfinding/Pathfinding.cs

            yield return null;

            pathRequestManager.OnPathProcessFinish(waypoints, pathSuccess);
        }
<<<<<<< Updated upstream:Assets/Scripts/Pathfinding/Pathfinding.cs

        private Vector3[] RetracePath(NodeBase startNode, NodeBase endNode, bool simplify = true)
        {
            List<NodeBase> path = new List<NodeBase>();
            NodeBase currentNode = endNode;
            path.Add(endNode);
            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.pathParent;
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

        private Vector3[] SimplifyPath(List<NodeBase> path)
        {
            List<Vector3> waypoints = new();
            waypoints.Add(path[0].worldPosition);
            for (int i = 1; i < path.Count - 1; i++)
            {
                Vector2 directionTo = gridManager.GetDirection(path[i - 1], path[i]);
                Vector2 directionFrom = gridManager.GetDirection(path[i], path[i + 1]);
                if (Vector2.Angle(directionTo, directionFrom) > angleChangeOverDistance)
                {
                    waypoints.Add(path[i].worldPosition);
                }
            }
            waypoints.Add(path[path.Count - 1].worldPosition);
            return waypoints.ToArray();
        }

        private bool CanTurnInTime(NodeBase from, NodeBase to)
        {
            Vector2 fromDirection = from.pathParent == null ? GetDefaultDirection.Invoke() : (from.worldPosition - from.pathParent.worldPosition).XZ();
            Vector2 toDirection = (to.worldPosition - from.worldPosition).XZ();
            return Vector2.Angle(fromDirection, toDirection) <= from.angleRange;
        }

        private int GetDistanceCost(NodeBase from, NodeBase to)
        {
            Vector2 distance = (from.worldPosition - to.worldPosition).XZ();

            int distanceX = Mathf.RoundToInt(Mathf.Abs(distance.x) / gridManager.NodeSize);
            int distanceY = Mathf.RoundToInt(Mathf.Abs(distance.y) / gridManager.NodeSize);

            const int diagonalCostCoefficient = 14;
            const int straightCostCoefficient = 10;

            return distanceX > distanceY ? diagonalCostCoefficient * distanceY + straightCostCoefficient * (distanceX - distanceY) : diagonalCostCoefficient * distanceX + straightCostCoefficient * (distanceY - distanceX);
        }

        private int GetAngleCost(NodeBase from, NodeBase to)
        {
            return 0;
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
=======
>>>>>>> Stashed changes:Assets/Scripts/FrameworkSource/Pathfinding/Pathfinding.cs
    }
}