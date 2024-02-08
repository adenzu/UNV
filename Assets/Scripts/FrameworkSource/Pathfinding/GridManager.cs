using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNV.Pathfinding
{
    public class GridManager : MonoBehaviour, IGrid
    {
        [SerializeField] private LayerMask obstacleMask;
        [SerializeField] private Vector2 gridWorldSize;
        [SerializeField, Min(0.01f)] private float nodeSize;

        [SerializeField] private bool showGrid = false;

        [SerializeField] private GridType gridType = GridType.SquareTiling;

        private GridType previousGridType = GridType.SquareTiling;

        private IGrid grid = new SquareTilingGrid();

        public int Width => grid.Width;

        public int Height => grid.Height;

        public float NodeSize => grid.NodeSize;

        public int NodeCount => grid.NodeCount;

        public bool DilatedBefore { get; private set; } = false;

        [ContextMenu("Regenerate Grid")]
        private void RegenerateGrid()
        {
            SetGridType(gridType);
            SetGridValues();
            GenerateGrid();
        }

        private void Start()
        {
            SetGridType(gridType);
            SetGridValues();
            GenerateGrid();
            SetupBorders();
        }

        private void SetupBorders()
        {
            if (transform.childCount != 4)
            {
                return;
            }

            Transform side = transform.GetChild(0);
            side.localScale = new Vector3(gridWorldSize.x, 1, nodeSize);
            side.localPosition = new Vector3(0, 0, gridWorldSize.y / 2 + nodeSize / 2);

            side = transform.GetChild(1);
            side.localScale = new Vector3(gridWorldSize.x, 1, nodeSize);
            side.localPosition = new Vector3(0, 0, -gridWorldSize.y / 2 - nodeSize / 2);

            side = transform.GetChild(2);
            side.localScale = new Vector3(nodeSize, 1, gridWorldSize.y);
            side.localPosition = new Vector3(gridWorldSize.x / 2 + nodeSize / 2, 0, 0);

            side = transform.GetChild(3);
            side.localScale = new Vector3(nodeSize, 1, gridWorldSize.y);
            side.localPosition = new Vector3(-gridWorldSize.x / 2 - nodeSize / 2, 0, 0);
        }

        private void OnValidate()
        {
            if (gridType != previousGridType)
            {
                SetGridType(gridType);
                SetGridValues();
                GenerateGrid();
            }
            else
            {
                SetGridValues();
            }
        }

        public void SetGridType(GridType gridType)
        {
            switch (gridType)
            {
                case GridType.SquareTiling:
                    grid = new SquareTilingGrid();
                    break;
                case GridType.QuadTree:
                    // _grid = new QuadTreeGrid();
                    break;
            }
            previousGridType = this.gridType;
            this.gridType = gridType;
        }

        private void SetGridValues()
        {
            grid.SetObstacleMask(obstacleMask);
            grid.SetGridWorldSize(gridWorldSize);
            grid.SetNodeSize(nodeSize);
            grid.SetShow(showGrid);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, gridWorldSize.XZ(1));
            if (showGrid)
            {
                foreach (NodeBase node in grid.GetNodes())
                {
                    Gizmos.color = node.walkable ? Color.white : Color.red;
                    Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeSize - .1f));
                }
            }
        }

        public void GenerateGrid()
        {
            GenerateGrid(transform.position);
        }

        public void GenerateGrid(Vector3 origin)
        {
            grid.GenerateGrid(origin);
            DilatedBefore = false;
        }

        public List<NodeBase> GetNeighbours(NodeBase pathfindingNode)
        {
            return grid.GetNeighbours(pathfindingNode);
        }

        public NodeBase GetAt(Vector3 worldPosition)
        {
            return grid.GetAt(worldPosition);
        }

        public Vector2 GetDirection(NodeBase from, NodeBase to)
        {
            return grid.GetDirection(from, to);
        }

        public void DilateObstacles(int radius)
        {
            grid.DilateObstacles(radius);
            DilatedBefore = true;
        }

        public void EdgeObstacles(int radius)
        {
            grid.EdgeObstacles(radius);
        }

        public void SetObstacleMask(LayerMask obstacleMask)
        {
            grid.SetObstacleMask(obstacleMask);
        }

        public void SetGridWorldSize(Vector2 gridWorldSize)
        {
            grid.SetGridWorldSize(gridWorldSize);
        }

        public void SetNodeSize(float nodeSize)
        {
            grid.SetNodeSize(nodeSize);
        }

        public void SetShow(bool show)
        {
            grid.SetShow(show);
        }

        public IEnumerable<NodeBase> GetNodes()
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            grid.Clear();
        }

        public bool IsClearPath(Vector3 from, Vector3 to)
        {
            return grid.IsClearPath(from, to);
        }

        public enum GridType
        {
            SquareTiling,
            QuadTree
        }
    }
}
