using System;
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

    public static Tuple<int, int>[] GetRelatives(int rangeX, int rangeY, bool includeCenter = false)
    {
        Tuple<int, int>[] relatives = new Tuple<int, int>[(rangeX * 2 + 1) * (rangeY * 2 + 1) - (includeCenter ? 0 : 1)];
        int i = 0;
        for (int x = -rangeX; x <= rangeX; x++)
        {
            for (int y = -rangeY; y <= rangeY; y++)
            {
                if (includeCenter || x != 0 || y != 0)
                {
                    relatives[i] = new Tuple<int, int>(x, y);
                    i++;
                }
            }
        }
        return relatives;
    }
}
