using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEditor;
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

    public static Vector3 GetIntersectionPoint(Vector3 offset1, Vector3 direction1, Vector3 offset2, Vector3 direction2)
    {
        Vector3 cross = Vector3.Cross(direction1, direction2);
        Vector3 cross2 = Vector3.Cross(offset2 - offset1, direction2);
        float planarFactor = Vector3.Dot(offset1 - offset2, cross) / cross.sqrMagnitude;
        float s = Vector3.Dot(cross2, cross) / cross.sqrMagnitude;
        return offset1 + direction1 * (planarFactor + s);
    }

    public static float GetLinesDistance(Vector2 from1, Vector2 to1, Vector2 from2, Vector2 to2)
    {
        return 0;
    }

<<<<<<< Updated upstream:Assets/Scripts/FrameworkSource/Util.cs
    public static bool HandleMonoScriptFieldClassInheritance(MonoScript monoScript, Type parentClass, string fieldName)
=======
    /// <summary>
    /// Checks if a MonoScript field is a subclass of a specified parent class.
    /// </summary>
    /// <param name="monoScript">The MonoScript to check.</param>
    /// <param name="parentClass">The parent class to compare against.</param>
    /// <param name="fieldName">The name of the field being checked.</param>
    /// <returns>True if the MonoScript is a subclass of the parent class, false otherwise.</returns>
    public static bool HandleMonoScriptFieldClassInheritance(MonoScript monoScript, Type parentClass, string fieldName, bool logIfValid = false)
>>>>>>> Stashed changes:Assets/Scripts/Util.cs
    {
        if (monoScript == null)
        {
            Debug.LogError($"{fieldName} script is not assigned.");
        }
        else if (monoScript.GetClass() == null)
        {
            Debug.LogError($"{fieldName} script is not a class.");
        }
<<<<<<< Updated upstream:Assets/Scripts/FrameworkSource/Util.cs
        else if (!monoScript.GetClass().GetInterfaces().Contains(parentClass))
=======
        else if (monoScript.GetClass().IsInterface)
        {
            Debug.LogError($"{fieldName} script is an interface.");
        }
        else if (monoScript.GetClass().IsAbstract)
        {
            Debug.LogError($"{fieldName} script is abstract.");
        }
        else if (!parentClass.IsAssignableFrom(monoScript.GetClass()))
>>>>>>> Stashed changes:Assets/Scripts/Util.cs
        {
            Debug.LogError($"{fieldName} script is not a subclass of {parentClass.Name}.");
        }
        else
        {
<<<<<<< Updated upstream:Assets/Scripts/FrameworkSource/Util.cs
            Debug.Log($"{fieldName} script is valid.");
=======
            if (logIfValid)
            {
                Debug.Log($"{fieldName} script is valid.");
            }
>>>>>>> Stashed changes:Assets/Scripts/Util.cs
            return true;
        }
        return false;
    }
<<<<<<< Updated upstream:Assets/Scripts/FrameworkSource/Util.cs
=======

    public static T InstantiateMonoScriptObject<T>(MonoScript monoScript) where T : class
    {
        if (monoScript == null)
        {
            return null;
        }
        return (T)monoScript.GetClass().GetConstructor(new Type[] { }).Invoke(new object[] { });
    }
>>>>>>> Stashed changes:Assets/Scripts/Util.cs
}
