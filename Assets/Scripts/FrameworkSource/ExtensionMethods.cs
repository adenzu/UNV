using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static Vector3 YZ(this Vector2 vector, float x = 0)
    {
        return new Vector3(x, vector.x, vector.y);
    }

    public static Vector3 XZ(this Vector2 vector, float y = 0)
    {
        return new Vector3(vector.x, y, vector.y);
    }

    public static Vector3 XY(this Vector2 vector, float z = 0)
    {
        return new Vector3(vector.x, vector.y, z);
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

    public static float Angle(this Vector2 vector)
    {
        return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
    }
}
