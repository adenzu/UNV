using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UNV.Pathfinding
{
    public class TilingManager : MonoBehaviour, ITiling
    {
        [SerializeField] private MonoScript tilingScript;
        [SerializeField] private LayerMask obstacleMask;
        [SerializeField] private Vector2 tilingWorldSize;
        [SerializeField, Min(0.01f)] private float nodeSize;

        [SerializeField] private bool showTiling = false;

        public ITiling Tiling => tiling;
        private ITiling tiling;

        public int Width => tiling.Width;

        public int Height => tiling.Height;

        public float NodeSize => tiling.NodeSize;

        public int NodeCount => tiling.NodeCount;

        public bool DilatedBefore { get; private set; } = false;

        private void Awake()
        {
            if (tilingScript != null)
            {
                tiling = Util.InstantiateMonoScriptObject<ITiling>(tilingScript);
                SetTilingValues();
            }
        }

        private void OnValidate()
        {
            if (!Util.HandleMonoScriptFieldClassInheritance(tilingScript, typeof(ITiling), "Tiling"))
            {
                tilingScript = null;
            }
        }

        private void SetTilingValues()
        {
            tiling.SetObstacleMask(obstacleMask);
            tiling.SetTilingWorldSize(tilingWorldSize);
            tiling.SetNodeSize(nodeSize);
            tiling.SetShow(showTiling);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, tilingWorldSize.XZ(1));
            if (showTiling)
            {
                int counter = 0;
                foreach (NodeBase node in tiling.GetNodes())
                {
                    if (node.walkable)
                    {
                        Gizmos.color = Color.white;
                    }
                    else
                    {
                        counter++;
                        Gizmos.color = Color.red;
                    }
                    Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeSize - .1f));
                }
            }
        }

        public void GenerateTiling()
        {
            GenerateTiling(transform.position);
        }

        public void GenerateTiling(Vector3 origin)
        {
            tiling.GenerateTiling(origin);
            DilatedBefore = false;
        }

        public List<NodeBase> GetNeighbours(NodeBase pathfindingNode)
        {
            return tiling.GetNeighbours(pathfindingNode);
        }

        public NodeBase GetAt(Vector3 worldPosition)
        {
            return tiling.GetAt(worldPosition);
        }

        public Vector2 GetDirection(NodeBase from, NodeBase to)
        {
            return tiling.GetDirection(from, to);
        }

        public void DilateObstacles(int radius)
        {
            tiling.DilateObstacles(radius);
            DilatedBefore = true;
        }

        public void EdgeObstacles(int radius)
        {
            tiling.EdgeObstacles(radius);
        }

        public void SetObstacleMask(LayerMask obstacleMask)
        {
            tiling.SetObstacleMask(obstacleMask);
        }

        public void SetTilingWorldSize(Vector2 tilingWorldSize)
        {
            tiling.SetTilingWorldSize(tilingWorldSize);
        }

        public void SetNodeSize(float nodeSize)
        {
            tiling.SetNodeSize(nodeSize);
        }

        public void SetShow(bool show)
        {
            tiling.SetShow(show);
        }

        public IEnumerable<NodeBase> GetNodes()
        {
            return tiling.GetNodes();
        }

        public bool IsClearPath(Vector3 from, Vector3 to)
        {
            return tiling.IsClearPath(from, to);
        }
    }
}
