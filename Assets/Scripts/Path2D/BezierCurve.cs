using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNV.Utils
{
    public static class BezierCurve
    {
        public static Vector3 Quadratic(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            float u = 1 - t;
            float uu = u * u;
            float tt = t * t;
            return uu * p0 + 2 * u * t * p1 + tt * p2;
        }

        public static Vector3[] QuadraticSample(Vector3 p0, Vector3 p1, Vector3 p2, int numSamples)
        {
            Vector3[] samples = new Vector3[numSamples];
            int numSamplesMinusOne = numSamples - 1;
            for (int i = 0; i < numSamples; i++)
            {
                samples[i] = Quadratic(p0, p1, p2, (float)i / numSamplesMinusOne);
            }
            return samples;
        }

        public static Vector3[] QuadraticSampleWithoutEnds(Vector3 p0, Vector3 p1, Vector3 p2, int numSamples)
        {
            Vector3[] samples = new Vector3[numSamples];
            int numSamplesPlusOne = numSamples + 1;
            for (int i = 1; i < numSamplesPlusOne; i++)
            {
                samples[i - 1] = Quadratic(p0, p1, p2, (float)i / numSamplesPlusOne);
            }
            return samples;
        }
    }
}

