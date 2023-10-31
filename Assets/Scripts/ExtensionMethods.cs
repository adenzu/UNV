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

    public static Vector2 YZ(this Vector3 vector)
    {
        return new Vector2(vector.y, vector.z);
    }

    public static Vector2 XZ(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }

    public static Vector2 XY(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.y);
    }

    public static Vector3 XZY(this Vector3 vector)
    {
        return new Vector3(vector.x, vector.z, vector.y);
    }

    public static Vector3 YXZ(this Vector3 vector)
    {
        return new Vector3(vector.y, vector.x, vector.z);
    }

    public static Vector3 YZX(this Vector3 vector)
    {
        return new Vector3(vector.y, vector.z, vector.x);
    }

    public static Vector3 ZXY(this Vector3 vector)
    {
        return new Vector3(vector.z, vector.x, vector.y);
    }

    public static Vector3 ZYX(this Vector3 vector)
    {
        return new Vector3(vector.z, vector.y, vector.x);
    }
}
