using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

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

    public static Vector2 UnitSquare(Vector2 vector)
    {
        float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

        Vector2[] possibleResults = {
            new(1, 0),      // 22.5, -22.5
            new(1, 1),      // 77.5, 22.5
            new(0, 1),      // 112.5, 77.5
            new(-1, 1),     // 157.5, 112.5
            new(-1, 0),     // 202.5, 157.5
            new(-1, -1),    // 247.5, 202.5
            new(0, -1),     // 292.5, 247.5
            new(1, -1)      // 337.5, 292.5
        };

        const float halfQuadrant = 45f;

        angle += halfQuadrant / 2;

        if (angle < 0)
        {
            angle += 360f;
        }

        int index = Mathf.FloorToInt(angle / halfQuadrant);

        if (index >= possibleResults.Length)
        {
            index = 0;
        }

        return possibleResults[index];
    }

    public static Vector2 ToVector(float angle)
    {
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    public static Vector2 TransformPoint(Vector2 point, Vector2 origin, Vector2 negativeYAxis)
    {
        const float NEGATIVE_Y_AXIS_ANGLE = 270f * Mathf.Deg2Rad;

        Vector2 translatedPoint = point - origin;

        float angle = Mathf.Atan2(negativeYAxis.y, negativeYAxis.x) - NEGATIVE_Y_AXIS_ANGLE;

        return new Vector2(
            translatedPoint.x * Mathf.Cos(angle) + translatedPoint.y * Mathf.Sin(angle),
            -translatedPoint.x * Mathf.Sin(angle) + translatedPoint.y * Mathf.Cos(angle)
        );
    }
}
