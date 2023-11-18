using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGrid : MonoBehaviour
{
    public static PathGrid Instance { get; private set; }

    [SerializeField] private LayerMask _unwalkableMask;
    [SerializeField] private Vector2 _gridWorldSize;
    [SerializeField, Min(0)] private float _nodeSize;

    [SerializeField] private bool _showGrid = false;

    public static float NodeSize => Instance._nodeSize;

    private PathNode[,] grid;

    private int _gridSizeX, _gridSizeY;

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

    private void Start()
    {
        CreateGrid();
    }

    public static void CreateGrid()
    {
        Instance._gridSizeX = Mathf.RoundToInt(Instance._gridWorldSize.x / Instance._nodeSize);
        Instance._gridSizeY = Mathf.RoundToInt(Instance._gridWorldSize.y / Instance._nodeSize);

        Instance.grid = new PathNode[Instance._gridSizeX, Instance._gridSizeY];

        Vector3 worldBottomLeft = Instance.transform.position - Vector3.right * Instance._gridWorldSize.x / 2 - Vector3.forward * Instance._gridWorldSize.y / 2;

        float halfNodeSize = Instance._nodeSize / 2;
        for (int x = 0; x < Instance._gridSizeX; x++)
        {
            for (int y = 0; y < Instance._gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * Instance._nodeSize + halfNodeSize) + Vector3.forward * (y * Instance._nodeSize + halfNodeSize);
                bool walkable = !Physics.CheckSphere(worldPoint, halfNodeSize, Instance._unwalkableMask);
                Instance.grid[x, y] = new PathNode(walkable, worldPoint, x, y);
            }
        }
    }

    public static List<PathNode> GetNeighbours(PathNode pathNode, int radius = 1)
    {
        List<PathNode> neighbours = new List<PathNode>(4 * (radius * radius + radius));

        foreach (Tuple<int, int> neighbour in Util.GetRelatives(radius, radius))
        {
            int neighbourX = pathNode.gridX + neighbour.Item1;
            int neighbourY = pathNode.gridY + neighbour.Item2;

            if (
                neighbourX >= 0 &&
                neighbourX < Instance._gridSizeX &&
                neighbourY >= 0 &&
                neighbourY < Instance._gridSizeY
            )
            {
                neighbours.Add(Instance.grid[neighbourX, neighbourY]);
            }
        }

        return neighbours;
    }

    public static PathNode PathNodeFromWorldPoint(Vector3 worldPoint)
    {
        float percentX = (worldPoint.x + Instance._gridWorldSize.x / 2) / Instance._gridWorldSize.x;
        float percentY = (worldPoint.z + Instance._gridWorldSize.y / 2) / Instance._gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((Instance._gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((Instance._gridSizeY - 1) * percentY);

        return Instance.grid[x, y];
    }

    public static void DilateObstacles(int gridRadius)
    {
        bool[,] obstacleGrid = new bool[Instance._gridSizeX, Instance._gridSizeY];
        Tuple<int, int>[] neighbours = Util.GetRelatives(gridRadius, gridRadius, true);
        for (int x = 0; x < Instance._gridSizeX; x++)
        {
            for (int y = 0; y < Instance._gridSizeY; y++)
            {
                if (!Instance.grid[x, y].walkable)
                {
                    foreach (Tuple<int, int> neighbour in neighbours)
                    {
                        int neighbourX = x + neighbour.Item1;
                        int neighbourY = y + neighbour.Item2;

                        if (
                            neighbourX >= 0 &&
                            neighbourX < Instance._gridSizeX &&
                            neighbourY >= 0 &&
                            neighbourY < Instance._gridSizeY
                        )
                        {
                            obstacleGrid[neighbourX, neighbourY] = true;
                        }
                    }
                }
            }
        }
        for (int x = 0; x < Instance._gridSizeX; x++)
        {
            for (int y = 0; y < Instance._gridSizeY; y++)
            {
                Instance.grid[x, y].walkable = !obstacleGrid[x, y];
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(_gridWorldSize.x, 1, _gridWorldSize.y));

        if (_showGrid && grid != null)
        {
            foreach (PathNode node in grid)
            {
                Gizmos.color = node.walkable ? Color.white : Color.red;
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (_nodeSize - .1f));
            }
        }
    }
}
