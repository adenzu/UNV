using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNV.Pathfinding
{
    public abstract class NodeBase : IComparable<NodeBase>
    {
        public bool walkable;
        public Vector3 worldPosition;

        public int costFromStart;
        public int costToTarget;
        public int TotalCost => costFromStart + costToTarget;

        public NodeBase pathParent;

        public NodeBase(bool walkable, Vector3 worldPosition)
        {
            this.walkable = walkable;
            this.worldPosition = worldPosition;
        }

        public int CompareTo(NodeBase other)
        {
            return TotalCost.CompareTo(other.TotalCost);
        }
    }
}