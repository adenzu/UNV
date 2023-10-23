using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlaceholderAI
{

    public static Outputs Predict(Inputs inputs)
    {
        float distance = (inputs.target - inputs.position).magnitude;
        float outputFactor = distance / inputs.targetDeadZone;

        Outputs outputs = new Outputs
        {
            thrust = Mathf.Clamp01(outputFactor / inputs.targetDeadZone),
            steer = (Vector3.Angle(inputs.right, inputs.target - inputs.position) > 90 ? -1 : 1) * Mathf.Clamp01(outputFactor)
        };
        return outputs;
    }

    public struct Inputs
    {
        public Vector3 right;
        public Vector3 position;
        public Vector3 target;
        public float targetDeadZone;
    }

    public struct Outputs
    {
        public float thrust;
        public float steer;
    }
}
