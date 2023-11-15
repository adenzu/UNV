using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public bool walkable;
    public int gridX;
    public int gridY;
    public int costFromStart;
    public int costToTarget;
    public Vector3 worldPosition;
    public PathNode parent;

    public PathNode(bool walkable, Vector3 worldPosition, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
    }

    public int fCost
    {
        get
        {
            return costFromStart + costToTarget;
        }
    }
}
