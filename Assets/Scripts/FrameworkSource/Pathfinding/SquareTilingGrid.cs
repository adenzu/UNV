using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNV.Pathfinding
{
    public class GridWithRotation : ITiling
    {
        private LayerMask obstacleMask;
        private Vector2 gridWorldSize;
        private float nodeSize;

        private int width, height;
        private GridNodeWithRotation[,] grid = new GridNodeWithRotation[0, 0];

        private int neighbourRadius = 1;

        private bool showGrid = false;

        public int Width => width;

        public int Height => height;

        public float NodeSize => nodeSize;

        public int NodeCount => grid.Length;

        public void GenerateTiling(Vector3 origin)
        {
            width = Mathf.RoundToInt(gridWorldSize.x / nodeSize);
            height = Mathf.RoundToInt(gridWorldSize.y / nodeSize);

            grid = new GridNodeWithRotation[width, height];

            Vector3 worldBottomLeft = origin - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

            float halfNodeSize = nodeSize / 2;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeSize + halfNodeSize) + Vector3.forward * (y * nodeSize + halfNodeSize);
                    bool walkable = !Physics.CheckSphere(worldPoint, halfNodeSize, obstacleMask);
                    grid[x, y] = new GridNodeWithRotation(walkable, worldPoint, x, y);
                }
            }
        }

        public List<NodeBase> GetNeighbours(NodeBase pathNode)
        {
            GridNodeWithRotation _pathNode = (GridNodeWithRotation)pathNode;

            int neighbourDiameter = 2 * neighbourRadius + 1;
            int neighbourCount = neighbourDiameter * neighbourDiameter - 1;
            List<NodeBase> neighbours = new(neighbourCount);

            foreach (Tuple<int, int> neighbour in Util.GetRelatives(neighbourRadius, neighbourRadius))
            {
                int neighbourX = _pathNode.gridX + neighbour.Item1;
                int neighbourY = _pathNode.gridY + neighbour.Item2;

                if (
                    neighbourX >= 0 &&
                    neighbourX < width &&
                    neighbourY >= 0 &&
                    neighbourY < height
                )
                {
                    neighbours.Add(grid[neighbourX, neighbourY]);
                }
            }

            return neighbours;
        }

        public NodeBase GetAt(Vector3 worldPoint)
        {
            float percentX = (worldPoint.x + gridWorldSize.x / 2) / gridWorldSize.x;
            float percentY = (worldPoint.z + gridWorldSize.y / 2) / gridWorldSize.y;

            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((width - 1) * percentX);
            int y = Mathf.RoundToInt((height - 1) * percentY);

            return grid[x, y];
        }

        public Vector2 GetDirection(NodeBase from, NodeBase to)
        {
            GridNodeWithRotation _from = (GridNodeWithRotation)from;
            GridNodeWithRotation _to = (GridNodeWithRotation)to;
            return new Vector2(_to.gridX - _from.gridX, _to.gridY - _from.gridY);
        }

        public void DilateObstacles(int radius)
        {
            bool[,] obstacleGrid = new bool[width, height];
            Tuple<int, int>[] neighbours = Util.GetRelatives(radius, radius, true);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (!grid[x, y].walkable)
                    {
                        foreach (Tuple<int, int> neighbour in neighbours)
                        {
                            int neighbourX = x + neighbour.Item1;
                            int neighbourY = y + neighbour.Item2;

                            if (
                                neighbourX >= 0 &&
                                neighbourX < width &&
                                neighbourY >= 0 &&
                                neighbourY < height
                            )
                            {
                                obstacleGrid[neighbourX, neighbourY] = true;
                            }
                        }
                    }
                }
            }
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid[x, y].walkable = !obstacleGrid[x, y];
                }
            }
        }

        public void EdgeObstacles(int radius)
        {
            return;
        }

        public void SetObstacleMask(LayerMask obstacleMask)
        {
            this.obstacleMask = obstacleMask;
        }

        public void SetTilingWorldSize(Vector2 gridWorldSize)
        {
            this.gridWorldSize = gridWorldSize;
        }

        public void SetNodeSize(float nodeSize)
        {
            this.nodeSize = nodeSize;
        }

        public void SetShow(bool show)
        {
            showGrid = show;
        }

        public IEnumerable<NodeBase> GetNodes()
        {
            foreach (NodeBase node in grid)
            {
                yield return node;
            }
        }
<<<<<<< Updated upstream:Assets/Scripts/FrameworkSource/Pathfinding/SquareTilingGrid.cs

        public void Clear()
        {
            foreach (NodeBase node in _grid)
            {
                node.angleRange = 0;
                node.costFromStart = 0;
                node.costToTarget = 0;
            }
        }
=======
<<<<<<<< Updated upstream:Assets/Scripts/Pathfinding/SquareTilingGrid.cs
========
>>>>>>> Stashed changes:Assets/Scripts/Pathfinding/SquareTilingGrid.cs

        public bool IsClearPath(Vector3 from, Vector3 to)
        {
            float verticalDistance = to.z - from.z;
            float horizontalDistance = to.x - from.x;

            float originX = from.x;
            float originY = from.z;

<<<<<<< Updated upstream:Assets/Scripts/FrameworkSource/Pathfinding/SquareTilingGrid.cs
            float delta = _nodeSize / 2;
=======
            float delta = nodeSize / 2;
>>>>>>> Stashed changes:Assets/Scripts/Pathfinding/SquareTilingGrid.cs

            if (horizontalDistance == 0)
            {
                int iterations = Mathf.RoundToInt(verticalDistance / delta);

                for (int i = 0; i < iterations; i++)
                {
                    float y = originY + delta * i;
                    if (!GetAt(new Vector3(originX, 0, y)).walkable)
                    {
                        return false;
                    }
                }
            }
            else
            {
                float slope = verticalDistance / horizontalDistance;
                int iterations = Mathf.RoundToInt(horizontalDistance / delta);

                for (int i = 0; i < iterations; i++)
                {
                    float x = originX + delta * i;
                    float y = slope * x + originY;
                    if (!GetAt(new Vector3(x, 0, y)).walkable)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
<<<<<<< Updated upstream:Assets/Scripts/FrameworkSource/Pathfinding/SquareTilingGrid.cs
=======
>>>>>>>> Stashed changes:Assets/Scripts/Pathfinding/GridWithRotation.cs
>>>>>>> Stashed changes:Assets/Scripts/Pathfinding/SquareTilingGrid.cs
    }
}

