using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNV.Pathfinding
{
    public class SquareTilingNode : NodeBase
    {
        public int gridX;
        public int gridY;

        public SquareTilingNode(bool walkable, Vector3 worldPosition, int gridX, int gridY)
            : base(walkable, worldPosition)
        {
            this.gridX = gridX;
            this.gridY = gridY;
        }
    }
}