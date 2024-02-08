using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UNV.Utils;

namespace UNV.Path
{
    public static class PathProcessing
    {
        public static Vector3[] SimplifyPath(Vector3[] points, float angle = 0f)
        {
            List<Vector3> simplifiedPoints = new List<Vector3>();
            simplifiedPoints.Add(points[0]);
            for (int i = 1; i < points.Length - 1; i++)
            {
                Vector3 prev = points[i - 1];
                Vector3 curr = points[i];
                Vector3 next = points[i + 1];
                Vector3 prevToCurr = curr - prev;
                Vector3 currToNext = next - curr;
                float angleBetween = Vector3.Angle(prevToCurr, currToNext);
                if (angleBetween > angle)
                {
                    simplifiedPoints.Add(curr);
                }
            }
            simplifiedPoints.Add(points[points.Length - 1]);
            return simplifiedPoints.ToArray();
        }

        public static Vector3[] AddMidPoints(Vector3[] points)
        {
            Vector3[] newPoints = new Vector3[2 * points.Length - 1];
            for (int i = 0; i < points.Length - 1; i++)
            {
                newPoints[2 * i] = points[i];
                newPoints[2 * i + 1] = (points[i] + points[i + 1]) / 2;
            }
            newPoints[newPoints.Length - 1] = points[points.Length - 1];
            return newPoints;
        }

        public static Vector3[] GetBezierPath(Vector3[] points, int numSamples = 10)
        {
            int numMidPoints = points.Length - 1;
            int totalNumSamplesPerPoint = numSamples + 1;
            int totalNumSamples = totalNumSamplesPerPoint * numMidPoints;
            List<Vector3> bezierPoints = new List<Vector3>(totalNumSamples + 1);
            points = AddMidPoints(points);
            for (int i = 1; i < points.Length - 2; i += 2)
            {
                bezierPoints.Add(points[i]);
                bezierPoints.AddRange(BezierCurve.QuadraticSampleWithoutEnds(points[i], points[i + 1], points[i + 2], numSamples));
            }
            bezierPoints.Add(points[points.Length - 1]);
            return bezierPoints.ToArray();
        }
    }
}
