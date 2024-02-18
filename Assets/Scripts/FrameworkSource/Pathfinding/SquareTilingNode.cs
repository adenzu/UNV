using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNV.Pathfinding
{
    public class GridNodeWithRotation : NodeBase
    {
        public int gridX;
        public int gridY;

        public float angleRange;

        public GridNodeWithRotation(bool walkable, Vector3 worldPosition, int gridX, int gridY)
            : base(walkable, worldPosition)
        {
            this.gridX = gridX;
            this.gridY = gridY;
        }
    }
}