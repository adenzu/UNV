using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static Vector3 YZ(this Vector2 vector)
    {
        return new Vector3(0, vector.x, vector.y);
    }

    public static Vector3 XZ(this Vector2 vector)
    {
        return new Vector3(vector.x, 0, vector.y);
    }

    public static Vector3 XY(this Vector2 vector)
    {
        return new Vector3(vector.x, vector.y, 0);
    }
}
