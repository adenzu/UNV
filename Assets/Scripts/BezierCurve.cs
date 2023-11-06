using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNV.Path2D {
    public static class BezierCurve
    {
        public static Vector2[] GetBezierCurvePathPoints(Vector2[] controlPoints, int pathSegmentCount)
        {
            float t = 0;
            Vector2[] points = new Vector2[pathSegmentCount + 1];
            points[0] = BezierPoint(t, controlPoints);
            for (int i = 1; i <= pathSegmentCount; i++)
            {
                t = i / (float)pathSegmentCount;
                points[i] = BezierPoint(t, controlPoints);
            }
            return points;
        }

        public static Vector2 BezierPoint(float t, Vector2[] points)
        {
            int n = points.Length - 1;
            Vector2 p = Vector2.zero;

            for (int i = 0; i <= n; i++)
            {
                float coef = BinomialCoefficient(n, i) * Mathf.Pow(1 - t, n - i) * Mathf.Pow(t, i);
                p += coef * points[i];
            }

            return p;
        }

        public static int BinomialCoefficient(int n, int k)
        {
            return Factorial(n) / (Factorial(k) * Factorial(n - k));
        }

        public static int Factorial(int n)
        {
            if (n <= 1)
                return 1;

            int result = 1;
            for (int i = 2; i <= n; i++)
            {
                result *= i;
            }

            return result;
        }
    }
}

