using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNV.Pathfinding
{
    public class GridManager : MonoBehaviour, IGrid
    {
        public static GridManager Instance { get; private set; }

        [SerializeField] private LayerMask _obstacleMask;
        [SerializeField] private Vector2 _gridWorldSize;
        [SerializeField, Min(0.01f)] private float _nodeSize;

        [SerializeField] private bool _showGrid = false;

        [SerializeField] private GridType _gridType = GridType.SquareTiling;

        private GridType _previousGridType = GridType.SquareTiling;

        private IGrid _grid = new SquareTilingGrid();

        public int Width => _grid.Width;

        public int Height => _grid.Height;

        public float NodeSize => _grid.NodeSize;

        public int NodeCount => _grid.NodeCount;

        public bool DilatedBefore { get; private set; } = false;

        [ContextMenu("Regenerate Grid")]
        private void RegenerateGrid()
        {
            SetGridType(_gridType);
            SetGridValues();
            GenerateGrid();
        }

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
            SetGridType(_gridType);
            SetGridValues();
            GenerateGrid();
        }

        private void OnValidate()
        {
            if (_gridType != _previousGridType)
            {
                SetGridType(_gridType);
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
                    _grid = new SquareTilingGrid();
                    break;
                case GridType.QuadTree:
                    // _grid = new QuadTreeGrid();
                    break;
            }
            _previousGridType = _gridType;
            _gridType = gridType;
        }

        private void SetGridValues()
        {
            _grid.SetObstacleMask(_obstacleMask);
            _grid.SetGridWorldSize(_gridWorldSize);
            _grid.SetNodeSize(_nodeSize);
            _grid.SetShow(_showGrid);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, _gridWorldSize.XZ(1));
            if (_showGrid)
            {
                foreach (NodeBase node in _grid.GetNodes())
                {
                    Gizmos.color = node.walkable ? Color.white : Color.red;
                    Gizmos.DrawCube(node.worldPosition, Vector3.one * (_nodeSize - .1f));
                }
            }
        }

        public void GenerateGrid()
        {
            GenerateGrid(transform.position);
        }

        public void GenerateGrid(Vector3 origin)
        {
            _grid.GenerateGrid(origin);
            DilatedBefore = false;
        }

        public List<NodeBase> GetNeighbours(NodeBase pathfindingNode)
        {
            return _grid.GetNeighbours(pathfindingNode);
        }

        public NodeBase GetAt(Vector3 worldPosition)
        {
            return _grid.GetAt(worldPosition);
        }

        public Vector2 GetDirection(NodeBase from, NodeBase to)
        {
            return _grid.GetDirection(from, to);
        }

        public void DilateObstacles(int radius)
        {
            _grid.DilateObstacles(radius);
            DilatedBefore = true;
        }

        public void EdgeObstacles(int radius)
        {
            _grid.EdgeObstacles(radius);
        }

        public void SetObstacleMask(LayerMask obstacleMask)
        {
            _grid.SetObstacleMask(obstacleMask);
        }

        public void SetGridWorldSize(Vector2 gridWorldSize)
        {
            _grid.SetGridWorldSize(gridWorldSize);
        }

        public void SetNodeSize(float nodeSize)
        {
            _grid.SetNodeSize(nodeSize);
        }

        public void SetShow(bool show)
        {
            _grid.SetShow(show);
        }

        public IEnumerable<NodeBase> GetNodes()
        {
            throw new System.NotImplementedException();
        }

        public enum GridType
        {
            SquareTiling,
            QuadTree
        }
    }
}
