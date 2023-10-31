using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static Vector3 Combine(Vector3 vector1, Vector3 vector2, bool x, bool y, bool z)
    {
        return new Vector3(
            x ? vector1.x : vector2.x,
            y ? vector1.y : vector2.y,
            z ? vector1.z : vector2.z
        );
    }

    public static Vector3 Combine(Vector3 vector1, Vector3 vector2, uint xyz)
    {
        return new Vector3(
            (xyz & 0b100) == 1 ? vector1.x : vector2.x,
            (xyz & 0b010) == 1 ? vector1.y : vector2.y,
            (xyz & 0b001) == 1 ? vector1.z : vector2.z
        );
    }
}
