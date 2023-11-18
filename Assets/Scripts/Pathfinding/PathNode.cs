using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : IComparable<PathNode>
{
    public bool walkable;
    public int gridX;
    public int gridY;
    public int costFromStart;
    public int costToTarget;

    public int TotalCost => costFromStart + costToTarget;

    public Vector3 worldPosition;
    public PathNode parent;

    public PathNode(bool walkable, Vector3 worldPosition, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
    }

    public int CompareTo(PathNode other)
    {
        return TotalCost.CompareTo(other.TotalCost);
    }
}
