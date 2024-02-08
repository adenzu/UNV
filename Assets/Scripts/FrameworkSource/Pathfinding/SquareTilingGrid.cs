using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNV.Pathfinding
{
    public class SquareTilingGrid : IGrid
    {
        private LayerMask _obstacleMask;
        private Vector2 _gridWorldSize;
        private float _nodeSize;

        private int _width, _height;
        private SquareTilingNode[,] _grid = new SquareTilingNode[0, 0];

        private int _neighbourRadius = 1;

        private bool _showGrid = false;

        public int Width => _width;

        public int Height => _height;

        public float NodeSize => _nodeSize;

        public int NodeCount => _grid.Length;

        public void GenerateGrid(Vector3 origin)
        {
            _width = Mathf.RoundToInt(_gridWorldSize.x / _nodeSize);
            _height = Mathf.RoundToInt(_gridWorldSize.y / _nodeSize);

            _grid = new SquareTilingNode[_width, _height];

            Vector3 worldBottomLeft = origin - Vector3.right * _gridWorldSize.x / 2 - Vector3.forward * _gridWorldSize.y / 2;

            float halfNodeSize = _nodeSize / 2;
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * _nodeSize + halfNodeSize) + Vector3.forward * (y * _nodeSize + halfNodeSize);
                    bool walkable = !Physics.CheckSphere(worldPoint, halfNodeSize, _obstacleMask);
                    _grid[x, y] = new SquareTilingNode(walkable, worldPoint, x, y);
                }
            }
        }

        public List<NodeBase> GetNeighbours(NodeBase pathNode)
        {
            SquareTilingNode _pathNode = (SquareTilingNode)pathNode;

            int neighbourDiameter = 2 * _neighbourRadius + 1;
            int neighbourCount = neighbourDiameter * neighbourDiameter - 1;
            List<NodeBase> neighbours = new(neighbourCount);

            foreach (Tuple<int, int> neighbour in Util.GetRelatives(_neighbourRadius, _neighbourRadius))
            {
                int neighbourX = _pathNode.gridX + neighbour.Item1;
                int neighbourY = _pathNode.gridY + neighbour.Item2;

                if (
                    neighbourX >= 0 &&
                    neighbourX < _width &&
                    neighbourY >= 0 &&
                    neighbourY < _height
                )
                {
                    neighbours.Add(_grid[neighbourX, neighbourY]);
                }
            }

            return neighbours;
        }

        public NodeBase GetAt(Vector3 worldPoint)
        {
            float percentX = (worldPoint.x + _gridWorldSize.x / 2) / _gridWorldSize.x;
            float percentY = (worldPoint.z + _gridWorldSize.y / 2) / _gridWorldSize.y;

            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((_width - 1) * percentX);
            int y = Mathf.RoundToInt((_height - 1) * percentY);

            return _grid[x, y];
        }

        public Vector2 GetDirection(NodeBase from, NodeBase to)
        {
            SquareTilingNode _from = (SquareTilingNode)from;
            SquareTilingNode _to = (SquareTilingNode)to;
            return new Vector2(_to.gridX - _from.gridX, _to.gridY - _from.gridY);
        }

        public void DilateObstacles(int radius)
        {
            bool[,] obstacleGrid = new bool[_width, _height];
            Tuple<int, int>[] neighbours = Util.GetRelatives(radius, radius, true);
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (!_grid[x, y].walkable)
                    {
                        foreach (Tuple<int, int> neighbour in neighbours)
                        {
                            int neighbourX = x + neighbour.Item1;
                            int neighbourY = y + neighbour.Item2;

                            if (
                                neighbourX >= 0 &&
                                neighbourX < _width &&
                                neighbourY >= 0 &&
                                neighbourY < _height
                            )
                            {
                                obstacleGrid[neighbourX, neighbourY] = true;
                            }
                        }
                    }
                }
            }
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _grid[x, y].walkable = !obstacleGrid[x, y];
                }
            }
        }

        public void EdgeObstacles(int radius)
        {
            return;
        }

        public void SetObstacleMask(LayerMask obstacleMask)
        {
            _obstacleMask = obstacleMask;
        }

        public void SetGridWorldSize(Vector2 gridWorldSize)
        {
            _gridWorldSize = gridWorldSize;
        }

        public void SetNodeSize(float nodeSize)
        {
            _nodeSize = nodeSize;
        }

        public void SetShow(bool show)
        {
            _showGrid = show;
        }

        public IEnumerable<NodeBase> GetNodes()
        {
            foreach (NodeBase node in _grid)
            {
                yield return node;
            }
        }

        public void Clear()
        {
            foreach (NodeBase node in _grid)
            {
                node.angleRange = 0;
                node.costFromStart = 0;
                node.costToTarget = 0;
            }
        }

        public bool IsClearPath(Vector3 from, Vector3 to)
        {
            float verticalDistance = to.z - from.z;
            float horizontalDistance = to.x - from.x;

            float originX = from.x;
            float originY = from.z;

            float delta = _nodeSize / 2;

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
    }
}

