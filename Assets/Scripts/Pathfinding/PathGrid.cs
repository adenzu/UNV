using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[ExecuteInEditMode]
public class PathGrid : MonoBehaviour
{
    [SerializeField] private LayerMask _unwalkableMask;
    [SerializeField] private Vector2 _gridWorldSize;
    [SerializeField, Min(0)] public float _nodeSize;

    [SerializeField] bool _showGrid = false;

    public List<PathNode> path;

    private PathNode[,] grid;

    private int _gridSizeX, _gridSizeY;

    private static readonly int[] _neighbourDX = { -1, -1, -1, 0, 0, 1, 1, 1 };
    private static readonly int[] _neighbourDY = { -1, 0, 1, -1, 1, -1, 0, 1 };

    private void Start()
    {
        CreateGrid();
    }

    public void CreateGrid()
    {
        _gridSizeX = Mathf.RoundToInt(_gridWorldSize.x / _nodeSize);
        _gridSizeY = Mathf.RoundToInt(_gridWorldSize.y / _nodeSize);

        grid = new PathNode[_gridSizeX, _gridSizeY];

        Vector3 worldBottomLeft = transform.position - Vector3.right * _gridWorldSize.x / 2 - Vector3.forward * _gridWorldSize.y / 2;

        float halfNodeSize = _nodeSize / 2;
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * _nodeSize + halfNodeSize) + Vector3.forward * (y * _nodeSize + halfNodeSize);
                bool walkable = !Physics.CheckSphere(worldPoint, halfNodeSize, _unwalkableMask);
                grid[x, y] = new PathNode(walkable, worldPoint, x, y);
            }
        }
    }

    public List<PathNode> GetNeighbours(PathNode pathNode)
    {
        List<PathNode> neighbours = new List<PathNode>(8);

        for (int i = 0; i < _neighbourDX.Length; i++)
        {
            int neighbourX = pathNode.gridX + _neighbourDX[i];
            int neighbourY = pathNode.gridY + _neighbourDY[i];

            if (
                neighbourX >= 0 &&
                neighbourX < _gridSizeX &&
                neighbourY >= 0 &&
                neighbourY < _gridSizeY
            )
            {
                neighbours.Add(grid[neighbourX, neighbourY]);
            }
        }

        return neighbours;
    }

    public PathNode PathNodeFromWorldPoint(Vector3 worldPoint)
    {
        float percentX = (worldPoint.x + _gridWorldSize.x / 2) / _gridWorldSize.x;
        float percentY = (worldPoint.z + _gridWorldSize.y / 2) / _gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);

        return grid[x, y];
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(_gridWorldSize.x, 1, _gridWorldSize.y));

        if (_showGrid && grid != null)
        {
            foreach (PathNode node in grid)
            {
                Gizmos.color = node.walkable ? Color.white : Color.red;
                if (path != null && path.Contains(node))
                {
                    Gizmos.color = Color.green;
                }
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (_nodeSize - .1f));
            }
        }
    }
}
